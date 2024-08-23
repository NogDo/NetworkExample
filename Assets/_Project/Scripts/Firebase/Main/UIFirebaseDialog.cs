using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFirebaseDialog : MonoBehaviour
{
    #region public º¯¼ö
    public Text text;

    public Button buttonClose;
    #endregion

    private void Awake()
    {
        buttonClose.onClick.AddListener(() => gameObject.SetActive(false));
    }
}