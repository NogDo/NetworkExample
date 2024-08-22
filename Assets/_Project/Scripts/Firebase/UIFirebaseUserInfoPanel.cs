using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIFirebaseUserInfoPanel : MonoBehaviour
{
    #region public ����
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
    /// ���� ���� ����
    /// </summary>
    /// <param name="user">����</param>
    public void SetUserInfo(FirebaseUser user)
    {
        textGreeting.text = $"�ȳ��ϼ���, {user.DisplayName}��";
        textDisplayName.text = user.DisplayName;
        textEmail.text = user.Email;
        textPhoneNumber.text = user.PhoneNumber;
        textUID.text = user.UserId;
    }

    /// <summary>
    /// �α׾ƿ� ��ư Ŭ��
    /// </summary>
    public void OnLogOutButtonClick()
    {
        CFirebaseManager.Instance.Logout();
        UIFirebasePanelManager.Instance.PanelOpen<UIFirebaseLoginPanel>().SetUIInteractable(true);
    }

    /// <summary>
    /// ���� ������Ʈ ��ư Ŭ��
    /// </summary>
    public void OnUpdateButtonClick()
    {
        UIFirebasePanelManager.Instance.PanelOpen<UIFirebaseUserInfoUpdatePanel>().SetUserInfo(CFirebaseManager.Instance.Auth.CurrentUser);
    }

    /// <summary>
    /// ���� ���� ��ư Ŭ��
    /// </summary>
    public void OnGameStartButtonClick()
    {
        
    }
}