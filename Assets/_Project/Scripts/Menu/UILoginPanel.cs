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
    public Button buttonSignupHW;
    public Button buttonLoginHW;
    #endregion

    #region private ����
    string oNickName;
    #endregion

    void Awake()
    {
        //buttonSignup.onClick.AddListener(OnSignupButtonClick);
        //buttonLogin.onClick.AddListener(PhotonConnectButtonClick);

        buttonSignup.onClick.AddListener(OnSignupButtonClick);
        buttonLogin.onClick.AddListener(OnLoginButtonClick);

        buttonSignupHW.onClick.AddListener(OnSignupButtonClick_HW0820);
        buttonLoginHW.onClick.AddListener(OnLoginButtonClick_HW0820);
    }

    void OnEnable()
    {
        inputID.interactable = true;
        inputPassword.interactable = true;

        buttonSignup.interactable = true;
        buttonLogin.interactable = true;
    }

    IEnumerator Start()
    {
        inputID.interactable = false;
        inputPassword.interactable = false;
        buttonSignup.interactable = false;
        buttonLogin.interactable = false;

        yield return new WaitUntil(() => CFirebaseManager.Instance.IsInitialized);

        inputID.interactable = true;
        inputPassword.interactable = true;
        buttonSignup.interactable = true;
        buttonLogin.interactable = true;
    }

    /// <summary>
    /// �α��� ��ư Ŭ��
    /// </summary>
    public void PhotonConnectButtonClick()
    {
        PhotonNetwork.LocalPlayer.NickName = oNickName;
        PhotonNetwork.ConnectUsingSettings();

        inputID.interactable = false;
        inputPassword.interactable = false;

        buttonSignup.interactable = false;
        buttonLogin.interactable = false;
    }

    /// <summary>
    /// ȸ������ ��ư Ŭ��
    /// </summary>
    public void OnSignupButtonClick()
    {
        buttonSignup.interactable = false;

        CFirebaseManager.Instance.Signup(inputID.text, inputPassword.text, (user) =>
        {
            print($"ȸ������ ����!");
            buttonSignup.interactable = true;
        });
    }

    /// <summary>
    /// �α��� ��ư Ŭ��
    /// </summary>
    public void OnLoginButtonClick()
    {
        buttonLogin.interactable = false;

        CFirebaseManager.Instance.Login(inputID.text, inputPassword.text, (user) =>
        {
            buttonLogin.interactable = true;
        });
    }

    /// <summary>
    /// �α��� ��ư Ŭ��(����)
    /// </summary>
    public void OnLoginButtonClick_HW0820()
    {
        CSqlManager.Instance.Login(inputID.text, inputPassword.text, LoginSuccess, LoginFailure);
    }

    /// <summary>
    /// ȸ������ ��ư Ŭ��(����)
    /// </summary>
    public void OnSignupButtonClick_HW0820()
    {
        oNickName = $"PLAYER {Random.Range(100, 1000)}";
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