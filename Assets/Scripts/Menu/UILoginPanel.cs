using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoginPanel : MonoBehaviour
{
    #region public 변수
    public InputField inputID;

    public Button buttonSignup;
    public Button buttonLogin;
    #endregion

    void Awake()
    {
        buttonSignup.onClick.AddListener(OnSignupButtonClick);
        buttonLogin.onClick.AddListener(OnLoginButtonClick);
    }

    void OnEnable()
    {
        inputID.interactable = true;
        buttonLogin.interactable = true;
    }

    void Start()
    {
        inputID.text = $"PLAYER {Random.Range(100, 1000)}";
    }

    /// <summary>
    /// 로그인 버튼 클릭
    /// </summary>
    public void OnLoginButtonClick()
    {
        PhotonNetwork.LocalPlayer.NickName = inputID.text;
        PhotonNetwork.ConnectUsingSettings();

        inputID.interactable = false;
        buttonLogin.interactable = false;
    }

    /// <summary>
    /// 회원가입 버튼 클릭
    /// </summary>
    public void OnSignupButtonClick()
    {

    }
}