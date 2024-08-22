using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIFirebaseUserInfoPanel : MonoBehaviour
{
    #region public 변수
    public Text textGreeting;
    public Text textDisplayName;
    public Text textEmail;
    public Text textPhoneNumber;
    public Text textUID;

    public Button buttonLogout;
    public Button buttonUpdate;
    public Button buttonGameStart;
    #endregion

    private void Awake()
    {
        buttonLogout.onClick.AddListener(OnLogOutButtonClick);
        buttonUpdate.onClick.AddListener(OnUpdateButtonClick);
        buttonGameStart.onClick.AddListener(OnGameStartButtonClick);
    }

    /// <summary>
    /// 유저 정보 세팅
    /// </summary>
    /// <param name="user">유저</param>
    public void SetUserInfo(FirebaseUser user)
    {
        textGreeting.text = $"안녕하세요, {user.DisplayName}님";
        textDisplayName.text = user.DisplayName;
        textEmail.text = user.Email;
        textPhoneNumber.text = user.PhoneNumber;
        textUID.text = user.UserId;
    }

    /// <summary>
    /// 로그아웃 버튼 클릭
    /// </summary>
    public void OnLogOutButtonClick()
    {
        CFirebaseManager.Instance.Logout();
        UIFirebasePanelManager.Instance.PanelOpen<UIFirebaseLoginPanel>().SetUIInteractable(true);
    }

    /// <summary>
    /// 정보 업데이트 버튼 클릭
    /// </summary>
    public void OnUpdateButtonClick()
    {
        UIFirebasePanelManager.Instance.PanelOpen<UIFirebaseUserInfoUpdatePanel>().SetUserInfo(CFirebaseManager.Instance.Auth.CurrentUser);
    }

    /// <summary>
    /// 게임 시작 버튼 클릭
    /// </summary>
    public void OnGameStartButtonClick()
    {
        
    }
}