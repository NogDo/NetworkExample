using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoginPanel : MonoBehaviour
{
    #region public ����
    public InputField inputID;
    public InputField inputPassword;

    public Button buttonSignup;
    public Button buttonLogin;
    #endregion

    #region private ����
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
    /// �α��� ��ư Ŭ��
    /// </summary>
    public void OnLoginButtonClick()
    {
        CSqlManager.Instance.Login(inputID.text, inputPassword.text, LoginSuccess, LoginFailure);
    }

    /// <summary>
    /// ȸ������ ��ư Ŭ��
    /// </summary>
    public void OnSignupButtonClick()
    {
        CSqlManager.Instance.Signup(inputID.text, inputPassword.text, oNickName, SignupSuccess, SignupFailure);
    }

    /// <summary>
    /// �α����� �������� ��, ���� ��ũ��ũ�� �����Ѵ�.
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
    /// �α����� �������� ��, �α� �޼����� ����.
    /// </summary>
    void LoginFailure()
    {
        UILogManager.Log("���̵� �Ǵ� ��й�ȣ�� ��ġ���� �ʽ��ϴ�.");
    }

    /// <summary>
    /// ȸ������ �������� ��, �α� �޼����� ����.
    /// </summary>
    void SignupSuccess()
    {
        UILogManager.Log("ȸ�����Կ� �����߽��ϴ�.");
    }

    /// <summary>
    /// ȸ������ �������� ��, �α� �޼����� ����.
    /// </summary>
    void SignupFailure()
    {
        UILogManager.Log("�̸����� �ߺ��Ǿ����ϴ�.");
    }
}