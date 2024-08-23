using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CFirebaseSendMessagePopup : MonoBehaviour
{
    #region public ����
    public InputField inputTo;
    public InputField inputMessage;

    public Button buttonSend;
    #endregion

    void Awake()
    {
        buttonSend.onClick.AddListener(OnSendButtonClick);
    }

    /// <summary>
    /// �޼��� ������ ��ư Ŭ��
    /// </summary>
    public void OnSendButtonClick()
    {
        Message message = new Message()
        {
            sender = CFirebaseManager.Instance.Auth.CurrentUser.UserId, // ���� ����
            message = inputMessage.text,                                // �޼��� ����
            sendTime = DateTime.Now.Ticks                               // ���� �ð�
        };

        CFirebaseManager.Instance.SendMessage(inputTo.text, message);
    }
}
