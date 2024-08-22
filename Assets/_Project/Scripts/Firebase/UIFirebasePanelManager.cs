using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFirebasePanelManager : MonoBehaviour
{
    public enum EPanelType
    {
        LOGIN,
        SIGNUP,
        INFO,
        UPDATE
    }

    #region static 변수
    private static UIFirebasePanelManager instance;

    public static UIFirebasePanelManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIFirebasePanelManager>();
            }

            return instance;
        }
    }
    #endregion

    #region public 변수
    public UIFirebaseLoginPanel login;
    public UIFirebaseSignupPanel signup;
    public UIFirebaseUserInfoPanel info;
    public UIFirebaseUserInfoUpdatePanel update;

    public UIFirebaseDialog dialog;
    #endregion

    #region private 변수
    Dictionary<EPanelType, MonoBehaviour> panels;
    #endregion

    private void Awake()
    {
        instance = this;

        panels = new()
        {
            { EPanelType.LOGIN, login },
            { EPanelType.SIGNUP, signup },
            { EPanelType.INFO, info },
            { EPanelType.UPDATE, update }
        };
    }


    private void Start()
    {
        _ = PanelOpen(EPanelType.LOGIN);
        dialog.gameObject.SetActive(false);
    }

    /// <summary>
    /// 다이얼로그를 활성화 시킨다.
    /// </summary>
    /// <param name="message">다이얼로그에 적힐 메세지</param>
    public void Dialog(string message)
    {
        dialog.text.text = message;
        dialog.gameObject.SetActive(true);
    }

    /// <summary>
    /// 패널을 활성화 시키고 해당 패널을 반환한다.
    /// </summary>
    /// <param name="type">패널 타입</param>
    /// <returns></returns>
    public GameObject PanelOpen(EPanelType type)
    {
        GameObject returnPanel = null;

        foreach (KeyValuePair<EPanelType, MonoBehaviour> row in panels)
        {
            bool isMatch = type == row.Key;

            if (isMatch)
            {
                returnPanel = row.Value.gameObject;
            }

            row.Value.gameObject.SetActive(isMatch);
        }

        return returnPanel;
    }

    /// <summary>
    /// 패널을 활성화 시키고 해당 패널을 반환한다. 제네릭으로 작성했다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T PanelOpen<T>() where T : MonoBehaviour
    {
        T returnPanel = null;

        foreach (KeyValuePair<EPanelType, MonoBehaviour> row in panels)
        {
            bool isMatch = typeof(T) == row.Value.GetType();

            if (isMatch)
            {
                returnPanel = (T)row.Value;
            }

            row.Value.gameObject.SetActive(isMatch);
        }

        return returnPanel;
    }
}
