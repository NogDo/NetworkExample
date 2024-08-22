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

    public UserData userData;

    public DatabaseReference usersRef;

    /// <summary>
    /// 파이어베이스 앱이 초기화 되어 사용 가능한지 여부
    /// </summary>
    public bool IsInitialized { get; private set; } = false;
    #endregion

    #region private 변수

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
    public async void Login(string email, string password, Action<FirebaseUser> callback = null)
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
        }

        else
        {
            UIFirebasePanelManager.Instance.Dialog("로그인 정보에 문제가 있습니다.");
        }
        

        callback?.Invoke(result.User);
    }

    /// <summary>
    /// 파이어베이스에 회원가입 시도
    /// </summary>
    /// <param name="email">이메일</param>
    /// <param name="password">비밀번호</param>
    /// <param name="callback">콜백함수</param>
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
}
