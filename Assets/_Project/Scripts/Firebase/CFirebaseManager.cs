using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CFirebaseManager : MonoBehaviour
{
    #region static ����
    private static CFirebaseManager instance;

    public static CFirebaseManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CFirebaseManager>();
            }

            return instance;
        }
    }
    #endregion

    #region public ����
    /// <summary>
    /// ���̾�̽� �⺻ ��
    /// </summary>
    public FirebaseApp App { get; private set; }
    /// <summary>
    /// ����(�α���)��� ����
    /// </summary>
    public FirebaseAuth Auth { get; private set; }
    /// <summary>
    /// �����ͺ��̽� ��� ����
    /// </summary>
    public FirebaseDatabase DB { get; private set; }

    /// <summary>
    /// ���̾�̽��� �ʱ�ȭ�Ǹ� ȣ���� �̺�Ʈ
    /// </summary>
    public event Action OnInit;
    /// <summary>
    /// ���� ������ �޼����� ������ ȣ���� �̺�Ʈ
    /// </summary>
    public event Action<string> OnReceive;
    /// <summary>
    /// �α��� �Ǵ� ȸ������ �Ŀ� ȣ���� �̺�Ʈ
    /// </summary>
    public event Action<FirebaseUser> OnLogin;

    public UserData userData;
    public PlayerCurrentEye eyeData;

    public DatabaseReference usersRef;

    /// <summary>
    /// ���̾�̽� ���� �ʱ�ȭ �Ǿ� ��� �������� ����
    /// </summary>
    public bool IsInitialized { get; private set; } = false;
    #endregion

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);

        OnLogin += OnLogined;
    }

    void Start()
    {
        InitializeAsync();
    }

    /// <summary>
    /// �α��� ���� �� ȣ���� �޼���
    /// </summary>
    /// <param name="user">�α����� ����</param>
    void OnLogined(FirebaseUser user)
    {
        DatabaseReference msgRef = DB.GetReference($"msg/{user.UserId}");
        msgRef.ChildAdded += ReceiveMessageEventHandler;
    }

    /// <summary>
    /// async�� ������� �ʴ� ���� ���α׷������� ���̾�̽� �ʱ�ȭ
    /// </summary>
    void Initialize()
    {
        // ���̾�̽� �� �ʱ�ȭ
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread
            (
                (Task<DependencyStatus> task) =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.LogWarning($"���̾�̽� �ʱ�ȭ ���� : {task.Status}");
                    }

                    else if (task.IsCompleted)
                    {

                        print($"���̾�̽� �ʱ�ȭ ���� : {task.Status}");

                        if (task.Result == DependencyStatus.Available)
                        {
                            App = FirebaseApp.DefaultInstance;
                            Auth = FirebaseAuth.DefaultInstance;

                            IsInitialized = true;
                        }
                    }
                }
            );
    }

    /// <summary>
    /// async�� Ű���带 ���� �񵿱� ���α׷������� ���̾�̽� �ʱ�ȭ
    /// </summary>
    async void InitializeAsync()
    {
        DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();

        if (status == DependencyStatus.Available)
        {
            print("���̾�̽� �ʱ�ȭ ����!");

            App = FirebaseApp.DefaultInstance;
            Auth = FirebaseAuth.DefaultInstance;
            DB = FirebaseDatabase.DefaultInstance;

            IsInitialized = true;
            OnInit?.Invoke();
        }

        else
        {
            Debug.LogWarning($"���̾�̽� �ʱ�ȭ ���� : {status}");
        }
    }

    /// <summary>
    /// ���̾�̽��� �α��� �õ�
    /// </summary>
    /// <param name="email">�̸���</param>
    /// <param name="password">��й�ȣ</param>
    /// <param name="callback">�ݹ��Լ�</param>
    public async void Login(string email, string password, Action<FirebaseUser> callback = null, Action<UserData> userDataCallback = null)
    {
        AuthResult result = await Auth.SignInWithEmailAndPasswordAsync(email, password);

        usersRef = DB.GetReference($"users/{result.User.UserId}");

        DataSnapshot userDataValues = await usersRef.GetValueAsync();

        if (userDataValues.Exists)
        {
            // ���� ��������
            string json = userDataValues.GetRawJsonValue();

            // �׸񺰷� ��������
            var address = userDataValues.Child("address");

            if (address.Exists)
            {
                print($"�ּ� : {address.GetValue(false)}");
            }

            userData = JsonConvert.DeserializeObject<UserData>(json);

            print(json);

            OnLogin?.Invoke(result.User);
            callback?.Invoke(result.User);
            userDataCallback?.Invoke(userData);

            GetEyeType();
        }

        else
        {
            UIFirebasePanelManager.Instance.Dialog("�α��� ������ ������ �ֽ��ϴ�.");
        }
    }

    /// <summary>
    /// ���̾�̽��� ȸ������ �õ�
    /// </summary>
    /// <param name="email">�̸���</param>
    /// <param name="password">��й�ȣ</param>
    /// <param name="callback">�ݹ��Լ�</param>
    public async void Signup(string email, string password, Action<FirebaseUser> callback = null, Action<UserData> userDataCallback = null)
    {
        try
        {
            AuthResult result = await Auth.CreateUserWithEmailAndPasswordAsync(email, password);

            usersRef = DB.GetReference($"users/{result.User.UserId}");

            UserData userData = new UserData(result.User.UserId);

            string userDataJson = JsonConvert.SerializeObject(userData);

            await usersRef.SetRawJsonValueAsync(userDataJson);

            this.userData = userData;

            OnLogin?.Invoke(result.User);
            callback?.Invoke(result.User);
            userDataCallback?.Invoke(userData);
        }

        catch (FirebaseException e)
        {
            Debug.Log(e.Message);
        }
    }

    /// <summary>
    /// ���� �������� ���� �α׾ƿ�
    /// </summary>
    public void Logout()
    {
        Auth.SignOut();
    }

    /// <summary>
    /// ���� �������� ������ ������ ������Ʈ�Ѵ�.
    /// </summary>
    /// <param name="name">�̸�</param>
    /// <param name="password">��й�ȣ</param>
    /// <param name="callback">�ݹ��Լ�</param>
    public async void UpdateUser(string name, string password, Action callback = null)
    {
        UserProfile profile = new UserProfile()
        {
            DisplayName = name
        };

        await Auth.CurrentUser.UpdateUserProfileAsync(profile);

        if (!string.IsNullOrWhiteSpace(password))
        {
            await Auth.CurrentUser.UpdatePasswordAsync(password);
        }

        callback?.Invoke();
    }

    /// <summary>
    /// ���� �������� ������ ������ ������Ʈ�Ѵ�.
    /// </summary>
    /// <param name="class">������ ����</param>
    /// <param name="callback">�ݹ� �Լ�</param>
    public async void UpdateCharacterClass(UserData.EClass @class, Action callback = null)
    {
        //DatabaseReference user = usersRef.Child("characterClass");

        // �̷� ����� ����
        string refKey = nameof(UserData.characterClass);
        DatabaseReference classRef = usersRef.Child(refKey);

        await classRef.SetValueAsync((int)@class);

        userData.characterClass = @class;

        callback?.Invoke();
    }

    /// <summary>
    /// ���� �������� ������ ������ ������Ʈ�Ѵ�.
    /// </summary>
    /// <param name="callback">�ݹ� �Լ�</param>
    public async void UpdateCharacterLevel(Action callback = null)
    {
        int level = userData.level + 1;

        string refKey = nameof(UserData.level);
        DatabaseReference levelRef = usersRef.Child(refKey);

        await levelRef.SetValueAsync(level);

        userData.level = level;

        callback?.Invoke();
    }

    /// <summary>
    /// ���� �������� ������ �ּҸ� ������Ʈ�Ѵ�.
    /// </summary>
    /// <param name="address">�ּ�</param>
    /// <param name="callback">�ݹ� �Լ�</param>
    public async void UpdateCharacterAddress(string address, Action callback = null)
    {
        string refKey = nameof(UserData.address);
        DatabaseReference addressRef = usersRef.Child(refKey);

        await addressRef.SetValueAsync(address);

        userData.address = address;

        callback?.Invoke();
    }

    /// <summary>
    /// ���� �������� ���� ������ ������Ʈ�Ѵ�. (�ߺ������� �Լ�)
    /// </summary>
    /// <param name="childName">�����ͺ��̽� Key �̸�</param>
    /// <param name="value"></param>
    /// <param name="callback"></param>
    public async void UpdateUserData(string childName, object value, Action<object> callback = null)
    {
        DatabaseReference targetRef = usersRef.Child(childName);

        await targetRef.SetValueAsync(value);
    }

    /// <summary>
    /// �޼����� ������.
    /// </summary>
    /// <param name="receiver">���� ���</param>
    /// <param name="message">�޼��� Ŭ����</param>
    public void SendMessage(string receiver, Message message)
    {
        DatabaseReference msgRef = DB.GetReference($"msg/{receiver}");

        string msgJson = JsonConvert.SerializeObject(message);

        msgRef.Child(message.sender + message.sendTime).SetRawJsonValueAsync(msgJson);
    }

    /// <summary>
    /// �޼����� �޾��� �� ������ �̺�Ʈ �ڵ鷯
    /// </summary>
    /// <param name="sender">�̺�Ʈ�� ȣ���� ��ü�� ���� Key ������ ���� ��ü</param>
    /// <param name="args"></param>
    public void ReceiveMessageEventHandler(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError);
            return;
        }

        else
        {
            string rawJson = args.Snapshot.GetRawJsonValue();

            Message msg = JsonConvert.DeserializeObject<Message>(rawJson);

            string message = $"������ : {msg.sender}\n���� : {msg.message}\n���� �ð� : {msg.GetSendTime()}";

            OnReceive?.Invoke(message);
        }
    }

    /// <summary>
    /// �÷��̾� �� Ÿ���� �����ͺ��̽��� �����Ѵ�.
    /// </summary>
    /// <param name="currentEye">���� �� Ÿ��</param>
    public void SetEyeType(PlayerCurrentEye currentEye)
    {
        DatabaseReference eyesRef = DB.GetReference($"equip/{Auth.CurrentUser.UserId}");

        string eyesJson = JsonConvert.SerializeObject(currentEye);

        eyesRef.SetRawJsonValueAsync(eyesJson);
    }

    /// <summary>
    /// ���� �÷��̾ �������� Eye�� �����͸� �����´�, ���� �������� �ʴ´ٸ� �����͸� �����Ѵ�.
    /// </summary>
    public async void GetEyeType()
    {
        DatabaseReference eyeRef = DB.GetReference($"equip/{Auth.CurrentUser.UserId}");

        DataSnapshot eyeValues = await eyeRef.GetValueAsync();

        DataSnapshot eye = eyeValues.Child("eye");

        if (eye.Exists)
        {
            eyeData.eye = (EPlayerEye)int.Parse(eye.GetValue(false).ToString());
        }

        else
        {
            eyeData.eye = EPlayerEye.GLASSES;
            SetEyeType(eyeData);
        }
    }


    /// <summary>
    /// ���� �������� ������ �̸��� ������Ʈ�Ѵ�.
    /// </summary>
    /// <param name="name">�̸�</param>
    /// <param name="callback">�ݹ� �Լ�</param>
    public async void UpdatePlayerName(string name, Action callback = null)
    {
        string refKey = nameof(UserData.userName);
        DatabaseReference nameRef = usersRef.Child(refKey);

        await nameRef.SetValueAsync(name);

        userData.userName = name;

        callback?.Invoke();
    }
}
