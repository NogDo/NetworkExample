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
    #region static 변수
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

    #region public 변수
    /// <summary>
    /// 파이어베이스 기본 앱
    /// </summary>
    public FirebaseApp App { get; private set; }
    /// <summary>
    /// 인증(로그인)기능 전용
    /// </summary>
    public FirebaseAuth Auth { get; private set; }
    /// <summary>
    /// 데이터베이스 기능 전용
    /// </summary>
    public FirebaseDatabase DB { get; private set; }

    /// <summary>
    /// 파이어베이스가 초기화되면 호출할 이벤트
    /// </summary>
    public event Action OnInit;
    /// <summary>
    /// 현재 유저가 메세지를 받으면 호출할 이벤트
    /// </summary>
    public event Action<string> OnReceive;
    /// <summary>
    /// 로그인 또는 회원가입 후에 호출할 이벤트
    /// </summary>
    public event Action<FirebaseUser> OnLogin;

    public UserData userData;
    public PlayerCurrentEye eyeData;

    public DatabaseReference usersRef;

    /// <summary>
    /// 파이어베이스 앱이 초기화 되어 사용 가능한지 여부
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
    /// 로그인 됐을 때 호출할 메서드
    /// </summary>
    /// <param name="user">로그인한 유저</param>
    void OnLogined(FirebaseUser user)
    {
        DatabaseReference msgRef = DB.GetReference($"msg/{user.UserId}");
        msgRef.ChildAdded += ReceiveMessageEventHandler;
    }

    /// <summary>
    /// async를 사용하지 않는 동기 프로그래밍으로 파이어베이스 초기화
    /// </summary>
    void Initialize()
    {
        // 파이어베이스 앱 초기화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread
            (
                (Task<DependencyStatus> task) =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.LogWarning($"파이어베이스 초기화 실패 : {task.Status}");
                    }

                    else if (task.IsCompleted)
                    {

                        print($"파이어베이스 초기화 성공 : {task.Status}");

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
    /// async를 키워드를 통한 비동기 프로그래밍으로 파이어베이스 초기화
    /// </summary>
    async void InitializeAsync()
    {
        DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();

        if (status == DependencyStatus.Available)
        {
            print("파이어베이스 초기화 성공!");

            App = FirebaseApp.DefaultInstance;
            Auth = FirebaseAuth.DefaultInstance;
            DB = FirebaseDatabase.DefaultInstance;

            IsInitialized = true;
            OnInit?.Invoke();
        }

        else
        {
            Debug.LogWarning($"파이어베이스 초기화 실패 : {status}");
        }
    }

    /// <summary>
    /// 파이어베이스에 로그인 시도
    /// </summary>
    /// <param name="email">이메일</param>
    /// <param name="password">비밀번호</param>
    /// <param name="callback">콜백함수</param>
    public async void Login(string email, string password, Action<FirebaseUser> callback = null, Action<UserData> userDataCallback = null)
    {
        AuthResult result = await Auth.SignInWithEmailAndPasswordAsync(email, password);

        usersRef = DB.GetReference($"users/{result.User.UserId}");

        DataSnapshot userDataValues = await usersRef.GetValueAsync();

        if (userDataValues.Exists)
        {
            // 전부 가져오기
            string json = userDataValues.GetRawJsonValue();

            // 항목별로 가져오기
            var address = userDataValues.Child("address");

            if (address.Exists)
            {
                print($"주소 : {address.GetValue(false)}");
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
            UIFirebasePanelManager.Instance.Dialog("로그인 정보에 문제가 있습니다.");
        }
    }

    /// <summary>
    /// 파이어베이스에 회원가입 시도
    /// </summary>
    /// <param name="email">이메일</param>
    /// <param name="password">비밀번호</param>
    /// <param name="callback">콜백함수</param>
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
    /// 현재 접속중인 유저 로그아웃
    /// </summary>
    public void Logout()
    {
        Auth.SignOut();
    }

    /// <summary>
    /// 현재 접속중인 유저의 정보를 업데이트한다.
    /// </summary>
    /// <param name="name">이름</param>
    /// <param name="password">비밀번호</param>
    /// <param name="callback">콜백함수</param>
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
    /// 현재 접속중인 유저의 직업을 업데이트한다.
    /// </summary>
    /// <param name="class">변경할 직업</param>
    /// <param name="callback">콜백 함수</param>
    public async void UpdateCharacterClass(UserData.EClass @class, Action callback = null)
    {
        //DatabaseReference user = usersRef.Child("characterClass");

        // 이런 용법도 존재
        string refKey = nameof(UserData.characterClass);
        DatabaseReference classRef = usersRef.Child(refKey);

        await classRef.SetValueAsync((int)@class);

        userData.characterClass = @class;

        callback?.Invoke();
    }

    /// <summary>
    /// 현재 접속중인 유저의 레벨을 업데이트한다.
    /// </summary>
    /// <param name="callback">콜백 함수</param>
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
    /// 현재 접속중인 유저의 주소를 업데이트한다.
    /// </summary>
    /// <param name="address">주소</param>
    /// <param name="callback">콜백 함수</param>
    public async void UpdateCharacterAddress(string address, Action callback = null)
    {
        string refKey = nameof(UserData.address);
        DatabaseReference addressRef = usersRef.Child(refKey);

        await addressRef.SetValueAsync(address);

        userData.address = address;

        callback?.Invoke();
    }

    /// <summary>
    /// 현재 접속중인 유저 정보를 업데이트한다. (중복제거한 함수)
    /// </summary>
    /// <param name="childName">데이터베이스 Key 이름</param>
    /// <param name="value"></param>
    /// <param name="callback"></param>
    public async void UpdateUserData(string childName, object value, Action<object> callback = null)
    {
        DatabaseReference targetRef = usersRef.Child(childName);

        await targetRef.SetValueAsync(value);
    }

    /// <summary>
    /// 메세지를 보낸다.
    /// </summary>
    /// <param name="receiver">받을 사람</param>
    /// <param name="message">메세지 클래스</param>
    public void SendMessage(string receiver, Message message)
    {
        DatabaseReference msgRef = DB.GetReference($"msg/{receiver}");

        string msgJson = JsonConvert.SerializeObject(message);

        msgRef.Child(message.sender + message.sendTime).SetRawJsonValueAsync(msgJson);
    }

    /// <summary>
    /// 메세지를 받았을 때 실행할 이벤트 핸들러
    /// </summary>
    /// <param name="sender">이벤트를 호출한 객체의 고유 Key 구분을 위한 객체</param>
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

            string message = $"보낸이 : {msg.sender}\n내용 : {msg.message}\n보낸 시간 : {msg.GetSendTime()}";

            OnReceive?.Invoke(message);
        }
    }

    /// <summary>
    /// 플레이어 눈 타입을 데이터베이스에 저장한다.
    /// </summary>
    /// <param name="currentEye">현재 눈 타입</param>
    public void SetEyeType(PlayerCurrentEye currentEye)
    {
        DatabaseReference eyesRef = DB.GetReference($"equip/{Auth.CurrentUser.UserId}");

        string eyesJson = JsonConvert.SerializeObject(currentEye);

        eyesRef.SetRawJsonValueAsync(eyesJson);
    }

    /// <summary>
    /// 현재 플레이어가 착용중인 Eye의 데이터를 가져온다, 만약 존재하지 않는다면 데이터를 생성한다.
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
    /// 현재 접속중인 유저의 이름을 업데이트한다.
    /// </summary>
    /// <param name="name">이름</param>
    /// <param name="callback">콜백 함수</param>
    public async void UpdatePlayerName(string name, Action callback = null)
    {
        string refKey = nameof(UserData.userName);
        DatabaseReference nameRef = usersRef.Child(refKey);

        await nameRef.SetValueAsync(name);

        userData.userName = name;

        callback?.Invoke();
    }
}
