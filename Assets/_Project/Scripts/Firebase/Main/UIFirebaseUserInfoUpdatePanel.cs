using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFirebaseUserInfoUpdatePanel : MonoBehaviour
{
    #region public 변수
    public InputField inputDisplayName;
    public Text textEmail;
    public InputField inputPassword;
    public Text textUID;

    public Button buttonUpdate;
    public Button buttonCancel;
    #endregion

    private void Awake()
    {
        buttonUpdate.onClick.AddListener(OnUpdateButtonClick);
        buttonCancel.onClick.AddListener(OnCancelButtonClick);
    }

    /// <summary>
    /// 유저 정보를 세팅
    /// </summary>
    /// <param name="user">유저</param>
    public void SetUserInfo(FirebaseUser user)
    {
        inputDisplayName.text = user.DisplayName;
        textEmail.text = user.Email;
        inputPassword.text = "";
        textUID.text = user.UserId;
    }

    /// <summary>
    /// 정보 수정 버튼 클릭
    /// </summary>
    public void OnUpdateButtonClick()
    {
        CFirebaseManager.Instance.UpdateUser(inputDisplayName.text, inputPassword.text, () =>
        {
            UIFirebasePanelManager.Instance.Dialog("정보가 수정되었습니다.");
            UIFirebasePanelManager.Instance.PanelOpen<UIFirebaseUserInfoPanel>().SetUserInfo(CFirebaseManager.Instance.Auth.CurrentUser);
        });
    }

    /// <summary>
    /// 취소 버튼 클릭
    /// </summary>
    public void OnCancelButtonClick()
    {
        _ = UIFirebasePanelManager.Instance.PanelOpen<UIFirebaseUserInfoPanel>();
    }
}