using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFirebaseGamePanelManager : MonoBehaviour
{
    #region public 변수
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
        // 옵션 리스트
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
    /// Text들에 유저 정보를 입력해준다.
    /// </summary>
    /// <param name="data">유저 정보</param>
    public void SetUserData(UserData data)
    {
        textTitle.text = $"안녕하세요. {data.userName}";
        textName.text = data.userName;
        textClass.text = data.characterClass.ToString();
        textLevel.text = data.level.ToString();
        textAddress.text = data.address;
    }

    /// <summary>
    /// 직업 Dropdown의 값이 바뀌었을 때
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
    /// 레벨업 버튼 클릭했을 때
    /// </summary>
    public void OnLevelupButtonClick()
    {
        CFirebaseManager.Instance.UpdateCharacterLevel(() =>
        {
            SetUserData(CFirebaseManager.Instance.userData);
        });
    }

    /// <summary>
    /// 주소 인풋에 주소를 입력했을 때
    /// </summary>
    /// <param name="value">주소</param>
    public void OnAddressInputSubmit(string value)
    {
        CFirebaseManager.Instance.UpdateCharacterAddress(inputAddress.text ,() =>
        {
            SetUserData(CFirebaseManager.Instance.userData);
        });
    }

    /// <summary>
    /// 메세지 보내기 버튼 클릭
    /// </summary>
    public void OnSendMessageButtonClick()
    {
        sPopup.gameObject.SetActive(true);
    }
}