using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFirebaseLoginPanel : MonoBehaviour
{
    #region public ����
    public InputField inputEmail;
    public InputField inputPassword;

    public Button buttonSignup;
    public Button buttonLogin;
    #endregion

    #region private ����

    #endregion

    private void Awake()
    {
        buttonSignup.onClick.AddListener(OnSignupButtonClick);
        buttonLogin.onClick.AddListener(OnLoginButtonClick);

        CFirebaseManager.Instance.OnInit += () => { SetUIInteractable(true); };
    }

    private void Start()
    {
        SetUIInteractable(CFirebaseManager.Instance.IsInitialized);
    }

    /// <summary>
    /// �α��� �гο� �ִ� UI���� ��ȣ�ۿ� ���θ� �����Ѵ�.
    /// </summary>
    /// <param name="isTrue">��ȣ�ۿ� ��������</param>
    public void SetUIInteractable(bool isTrue)
    {
        inputEmail.interactable = isTrue;
        inputPassword.interactable = isTrue;
        buttonSignup.interactable = isTrue;
        buttonLogin.interactable = isTrue;
    }

    /// <summary>
    /// �α��� ��ư Ŭ��
    /// </summary>
    public void OnLoginButtonClick()
    {
        SetUIInteractable(false);
        CFirebaseManager.Instance.Login(inputEmail.text, inputPassword.text, (user) =>
        {
            UIFirebasePanelManager.Instance.PanelOpen<UIFirebaseUserInfoPanel>().SetUserInfo(user);
        });
    }

    /// <summary>
    /// ȸ������ ��ư Ŭ��
    /// </summary>
    public void OnSignupButtonClick()
    {
        _ = UIFirebasePanelManager.Instance.PanelOpen<UIFirebaseSignupPanel>();
    }
}