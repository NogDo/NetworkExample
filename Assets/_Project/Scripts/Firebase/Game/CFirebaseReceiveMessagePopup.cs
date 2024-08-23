using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CFirebaseReceiveMessagePopup : MonoBehaviour
{
    #region public 변수
    public Text textMessage;

    public Button buttonReceive;
    #endregion

    void Awake()
    {
        buttonReceive.onClick.AddListener(OnReceiveButtonClick);
    }

    /// <summary>
    /// 받기 버튼 클릭
    /// </summary>
    public void OnReceiveButtonClick()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 메세지를 받았을 때, 메세지 창을 활성화시킨다.
    /// </summary>
    /// <param name="message">받은 메세지</param>
    public void OnReceiveMessage(string message)
    {
        textMessage.text = message;
        gameObject.SetActive(true);
    }
}
