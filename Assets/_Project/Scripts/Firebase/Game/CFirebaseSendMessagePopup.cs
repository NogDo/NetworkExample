using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CFirebaseSendMessagePopup : MonoBehaviour
{
    #region public 변수
    public InputField inputTo;
    public InputField inputMessage;

    public Button buttonSend;
    #endregion

    void Awake()
    {
        buttonSend.onClick.AddListener(OnSendButtonClick);
    }

    /// <summary>
    /// 메세지 보내기 버튼 클릭
    /// </summary>
    public void OnSendButtonClick()
    {
        Message message = new Message()
        {
            sender = CFirebaseManager.Instance.Auth.CurrentUser.UserId, // 보낸 유저
            message = inputMessage.text,                                // 메세지 내용
            sendTime = DateTime.Now.Ticks                               // 보낸 시각
        };

        CFirebaseManager.Instance.SendMessage(inputTo.text, message);
    }
}
