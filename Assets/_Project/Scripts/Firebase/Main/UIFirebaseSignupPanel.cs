using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFirebaseSignupPanel : MonoBehaviour
{
    #region public ����
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
    /// ȸ������ ��ư Ŭ��
    /// </summary>
    public void OnSignupButtonClick()
    {
        if (inputPassword.text.Equals(inputPasswordConfirm.text))
        {
            CFirebaseManager.Instance.Signup(inputId.text, inputPassword.text, SetUser);
        }

        else
        {
            UIFirebasePanelManager.Instance.Dialog("��й�ȣ�� ��ġ���� �ʽ��ϴ�. �ٽ� �Է����ּ���.");
        }
    }

    /// <summary>
    /// ȸ�������� �������� �� ������ �޼���
    /// </summary>
    /// <param name="user"></param>
    void SetUser(FirebaseUser user)
    {
        UIFirebasePanelManager.Instance.Dialog("ȸ������ �Ϸ�");
        UIFirebasePanelManager.Instance.PanelOpen<UIFirebaseUserInfoPanel>().SetUserInfo(user);
    }
}