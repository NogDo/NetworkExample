using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILogManager : MonoBehaviour
{
    #region static ����
    public static UILogManager Instance { get; private set; }
    #endregion

    #region public ����
    public RectTransform logContent;
    public Text logText;
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// �α׸� ����.
    /// </summary>
    /// <param name="message">ȭ�鿡 ��� �޼���</param>
    public static void Log(string message)
    {
        if (Instance != null)
        {
            Text logText = Instantiate(Instance.logText, Instance.logContent, false);
            logText.text = message;
        }

        else
        {
            print(message);
        }
    }
}