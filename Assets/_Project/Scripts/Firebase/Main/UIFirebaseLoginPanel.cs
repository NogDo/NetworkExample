using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFirebaseLoginPanel : MonoBehaviour
{
    #region public 변수
    public InputField inputEmail;
    public InputField inputPassword;

    public Button buttonSignup;
    public Button buttonLogin;
    #endregion

    #region private 변수

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
    /// 로그인 패널에 있는 UI들의 상호작용 여부를 결정한다.
    /// </summary>
    /// <param name="isTrue">상호작용 가능한지</param>
    public void SetUIInteractable(bool isTrue)
    {
        inputEmail.interactable = isTrue;
        inputPassword.interactable = isTrue;
        buttonSignup.interactable = isTrue;
        buttonLogin.interactable = isTrue;
    }

    /// <summary>
    /// 로그인 버튼 클릭
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
    /// 회원가입 버튼 클릭
    /// </summary>
    public void OnSignupButtonClick()
    {
        _ = UIFirebasePanelManager.Instance.PanelOpen<UIFirebaseSignupPanel>();
    }
}