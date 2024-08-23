using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFirebaseGamePanelManager : MonoBehaviour
{
    #region public ����
    public Text textTitle;
    public Text textName;
    public Text textClass;
    public Text textLevel;
    public Text textAddress;

    public Dropdown dropdownClass;

    public Button buttonLevelup;
    public Button buttonSendMessage;

    public InputField inputAddress;

    public CFirebaseReceiveMessagePopup rPopup;
    public CFirebaseSendMessagePopup sPopup;
    #endregion

    void Awake()
    {
        // �ɼ� ����Ʈ
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

        foreach (string characterClass in Enum.GetNames(typeof(UserData.EClass)))
        {
            options.Add(new Dropdown.OptionData(characterClass));
        }
        dropdownClass.options = options;

        dropdownClass.onValueChanged.AddListener(OnClassDropdownChange);

        buttonLevelup.onClick.AddListener(OnLevelupButtonClick);
        buttonSendMessage.onClick.AddListener(OnSendMessageButtonClick);

        inputAddress.onSubmit.AddListener(OnAddressInputSubmit);
    }

    void Start()
    {
        SetUserData(CFirebaseManager.Instance.userData);

        CFirebaseManager.Instance.OnReceive += rPopup.OnReceiveMessage;
    }

    /// <summary>
    /// Text�鿡 ���� ������ �Է����ش�.
    /// </summary>
    /// <param name="data">���� ����</param>
    public void SetUserData(UserData data)
    {
        textTitle.text = $"�ȳ��ϼ���. {data.userName}";
        textName.text = data.userName;
        textClass.text = data.characterClass.ToString();
        textLevel.text = data.level.ToString();
        textAddress.text = data.address;
    }

    /// <summary>
    /// ���� Dropdown�� ���� �ٲ���� ��
    /// </summary>
    /// <param name="value"></param>
    public void OnClassDropdownChange(int value)
    {
        CFirebaseManager.Instance.UpdateCharacterClass((UserData.EClass)value, () => 
        {
            SetUserData(CFirebaseManager.Instance.userData);
        });
    }

    /// <summary>
    /// ������ ��ư Ŭ������ ��
    /// </summary>
    public void OnLevelupButtonClick()
    {
        CFirebaseManager.Instance.UpdateCharacterLevel(() =>
        {
            SetUserData(CFirebaseManager.Instance.userData);
        });
    }

    /// <summary>
    /// �ּ� ��ǲ�� �ּҸ� �Է����� ��
    /// </summary>
    /// <param name="value">�ּ�</param>
    public void OnAddressInputSubmit(string value)
    {
        CFirebaseManager.Instance.UpdateCharacterAddress(inputAddress.text ,() =>
        {
            SetUserData(CFirebaseManager.Instance.userData);
        });
    }

    /// <summary>
    /// �޼��� ������ ��ư Ŭ��
    /// </summary>
    public void OnSendMessageButtonClick()
    {
        sPopup.gameObject.SetActive(true);
    }
}