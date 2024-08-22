using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFirebaseUserInfoUpdatePanel : MonoBehaviour
{
    #region public ����
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
    /// ���� ������ ����
    /// </summary>
    /// <param name="user">����</param>
    public void SetUserInfo(FirebaseUser user)
    {
        inputDisplayName.text = user.DisplayName;
        textEmail.text = user.Email;
        inputPassword.text = "";
        textUID.text = user.UserId;
    }

    /// <summary>
    /// ���� ���� ��ư Ŭ��
    /// </summary>
    public void OnUpdateButtonClick()
    {
        CFirebaseManager.Instance.UpdateUser(inputDisplayName.text, inputPassword.text, () =>
        {
            UIFirebasePanelManager.Instance.Dialog("������ �����Ǿ����ϴ�.");
            UIFirebasePanelManager.Instance.PanelOpen<UIFirebaseUserInfoPanel>().SetUserInfo(CFirebaseManager.Instance.Auth.CurrentUser);
        });
    }

    /// <summary>
    /// ��� ��ư Ŭ��
    /// </summary>
    public void OnCancelButtonClick()
    {
        _ = UIFirebasePanelManager.Instance.PanelOpen<UIFirebaseUserInfoPanel>();
    }
}