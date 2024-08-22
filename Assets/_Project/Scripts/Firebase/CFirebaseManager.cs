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

    public UserData userData;

    public DatabaseReference usersRef;

    /// <summary>
    /// ���̾�̽� ���� �ʱ�ȭ �Ǿ� ��� �������� ����
    /// </summary>
    public bool IsInitialized { get; private set; } = false;
    #endregion

    #region private ����

    #endregion

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        InitializeAsync();
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
    public async void Login(string email, string password, Action<FirebaseUser> callback = null)
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
        }

        else
        {
            UIFirebasePanelManager.Instance.Dialog("�α��� ������ ������ �ֽ��ϴ�.");
        }
        

        callback?.Invoke(result.User);
    }

    /// <summary>
    /// ���̾�̽��� ȸ������ �õ�
    /// </summary>
    /// <param name="email">�̸���</param>
    /// <param name="password">��й�ȣ</param>
    /// <param name="callback">�ݹ��Լ�</param>
    public async void Signup(string email, string password, Action<FirebaseUser> callback = null)
    {
        try
        {
            AuthResult result = await Auth.CreateUserWithEmailAndPasswordAsync(email, password);

            usersRef = DB.GetReference($"users/{result.User.UserId}");

            UserData userData = new UserData(result.User.UserId);

            string userDataJson = JsonConvert.SerializeObject(userData);

            await usersRef.SetRawJsonValueAsync(userDataJson);

            this.userData = userData;

            callback?.Invoke(result.User);
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
}
