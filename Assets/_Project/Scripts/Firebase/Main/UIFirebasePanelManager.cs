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

    #region static ����
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

    #region public ����
    public UIFirebaseLoginPanel login;
    public UIFirebaseSignupPanel signup;
    public UIFirebaseUserInfoPanel info;
    public UIFirebaseUserInfoUpdatePanel update;

    public UIFirebaseDialog dialog;
    #endregion

    #region private ����
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
    /// ���̾�α׸� Ȱ��ȭ ��Ų��.
    /// </summary>
    /// <param name="message">���̾�α׿� ���� �޼���</param>
    public void Dialog(string message)
    {
        dialog.text.text = message;
        dialog.gameObject.SetActive(true);
    }

    /// <summary>
    /// �г��� Ȱ��ȭ ��Ű�� �ش� �г��� ��ȯ�Ѵ�.
    /// </summary>
    /// <param name="type">�г� Ÿ��</param>
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
    /// �г��� Ȱ��ȭ ��Ű�� �ش� �г��� ��ȯ�Ѵ�. ���׸����� �ۼ��ߴ�.
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
