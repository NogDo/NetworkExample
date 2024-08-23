using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFirebaseSignupPanel : MonoBehaviour
{
    #region public 변수
    public InputField inputId;
    public InputField inputPassword;
    public InputField inputPasswordConfirm;

    public Button buttonSignup;
    public Button buttonCancel;
    #endregion

    private void Awake()
    {
        buttonSignup.onClick.AddListener(OnSignupButtonClick);
        buttonCancel.onClick.AddListener(() => { UIFirebasePanelManager.Instance.PanelOpen<UIFirebaseLoginPanel>(); });
    }

    /// <summary>
    /// 회원가입 버튼 클릭
    /// </summary>
    public void OnSignupButtonClick()
    {
        if (inputPassword.text.Equals(inputPasswordConfirm.text))
        {
            CFirebaseManager.Instance.Signup(inputId.text, inputPassword.text, SetUser);
        }

        else
        {
            UIFirebasePanelManager.Instance.Dialog("비밀번호가 일치하지 않습니다. 다시 입력해주세요.");
        }
    }

    /// <summary>
    /// 회원가입이 성공했을 때 실행할 메서드
    /// </summary>
    /// <param name="user"></param>
    void SetUser(FirebaseUser user)
    {
        UIFirebasePanelManager.Instance.Dialog("회원가입 완료");
        UIFirebasePanelManager.Instance.PanelOpen<UIFirebaseUserInfoPanel>().SetUserInfo(user);
    }
}