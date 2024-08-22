using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILogManager : MonoBehaviour
{
    #region static 변수
    public static UILogManager Instance { get; private set; }
    #endregion

    #region public 변수
    public RectTransform logContent;
    public Text logText;
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 로그를 띄운다.
    /// </summary>
    /// <param name="message">화면에 띄울 메세지</param>
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