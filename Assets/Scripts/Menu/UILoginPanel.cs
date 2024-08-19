using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoginPanel : MonoBehaviour
{
    #region public 변수
    public InputField inputID;
    public InputField inputPassword;

    public Button buttonSignup;
    public Button buttonLogin;
    #endregion

    #region private 변수
    string oNickName;
    #endregion

    void Awake()
    {
        buttonSignup.onClick.AddListener(OnSignupButtonClick);
        buttonLogin.onClick.AddListener(OnLoginButtonClick);
    }

    void OnEnable()
    {
        inputID.interactable = true;
        inputPassword.interactable = true;

        buttonSignup.interactable = true;
        buttonLogin.interactable = true;
    }

    void Start()
    {
        oNickName = $"PLAYER {Random.Range(100, 1000)}";
    }

    /// <summary>
    /// 로그인 버튼 클릭
    /// </summary>
    public void OnLoginButtonClick()
    {
        CSqlManager.Instance.Login(inputID.text, inputPassword.text, LoginSuccess, LoginFailure);
    }

    /// <summary>
    /// 회원가입 버튼 클릭
    /// </summary>
    public void OnSignupButtonClick()
    {
        CSqlManager.Instance.Signup(inputID.text, inputPassword.text, oNickName, SignupSuccess, SignupFailure);
    }

    /// <summary>
    /// 로그인이 성공했을 때, 포톤 네크워크에 연결한다.
    /// </summary>
    void LoginSuccess(string nickName)
    {
        PhotonNetwork.LocalPlayer.NickName = nickName;
        PhotonNetwork.ConnectUsingSettings();

        inputID.interactable = false;
        inputPassword.interactable = false;

        buttonSignup.interactable = false;
        buttonLogin.interactable = false;
    }

    /// <summary>
    /// 로그인이 실패했을 때, 로그 메세지를 띄운다.
    /// </summary>
    void LoginFailure()
    {
        UILogManager.Log("아이디 또는 비밀번호가 일치하지 않습니다.");
    }

    /// <summary>
    /// 회원가입 성공했을 때, 로그 메세지를 띄운다.
    /// </summary>
    void SignupSuccess()
    {
        UILogManager.Log("회원가입에 성공했습니다.");
    }

    /// <summary>
    /// 회원가입 실패했을 때, 로그 메세지를 띄운다.
    /// </summary>
    void SignupFailure()
    {
        UILogManager.Log("이메일이 중복되었습니다.");
    }
}