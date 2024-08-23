using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CFirebaseReceiveMessagePopup : MonoBehaviour
{
    #region public ����
    public Text textMessage;

    public Button buttonReceive;
    #endregion

    void Awake()
    {
        buttonReceive.onClick.AddListener(OnReceiveButtonClick);
    }

    /// <summary>
    /// �ޱ� ��ư Ŭ��
    /// </summary>
    public void OnReceiveButtonClick()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// �޼����� �޾��� ��, �޼��� â�� Ȱ��ȭ��Ų��.
    /// </summary>
    /// <param name="message">���� �޼���</param>
    public void OnReceiveMessage(string message)
    {
        textMessage.text = message;
        gameObject.SetActive(true);
    }
}
