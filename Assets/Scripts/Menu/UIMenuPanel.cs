using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuPanel : MonoBehaviour
{
    #region public ����
    public Text playerName;

    [Header("�÷��̾� �г��� ���� ����")]
    public InputField inputPlayerName;
    public Button buttonPlayerNameChange;

    [Header("���� �޴� ����")]
    public RectTransform panelMainMenu;
    public Button buttonCreateRoom;
    public Button buttonFindRoom;
    public Button buttonRandomRoom;
    public Button buttonLogout;

    [Header("�� ���� ����")]
    public RectTransform panelCreateRoomMenu;
    public InputField inputRoomName;
    public InputField inputPlayerNum;
    public Button buttonCreate;
    public Button buttonCancel;
    #endregion

    void Awake()
    {
        buttonPlayerNameChange.onClick.AddListener(OnPlayerNameChangeButtonClick);

        buttonCreateRoom.onClick.AddListener(OnCreateRoomButtonClick);
        buttonFindRoom.onClick.AddListener(OnFindRoomButtonClick);
        buttonRandomRoom.onClick.AddListener(OnRandomRoomButtonClick);
        buttonLogout.onClick.AddListener(OnLogoutButtonClick);

        buttonCreate.onClick.AddListener(OnCreateButtonClick);
        buttonCancel.onClick.AddListener(OnCancelButtonClick);
    }

    void OnEnable()
    {
        playerName.text = $"�ȳ��ϼ���, {PhotonNetwork.LocalPlayer.NickName}";

        panelMainMenu.gameObject.SetActive(true);
        panelCreateRoomMenu.gameObject.SetActive(false);
    }

    /// <summary>
    /// �÷��̾� �г��� ���� ��ư Ŭ��
    /// </summary>
    void OnPlayerNameChangeButtonClick()
    {
        CSqlManager.Instance.ChangeNickName(inputPlayerName.text, ChangeNickNameSuccess, ChangeNickNameFailure);
    }

    /// <summary>
    /// �� ���� ��ư Ŭ��
    /// </summary>
    void OnCreateRoomButtonClick()
    {
        panelMainMenu.gameObject.SetActive(false);
        panelCreateRoomMenu.gameObject.SetActive(true);
    }

    /// <summary>
    /// �� ã�� ��ư Ŭ��
    /// </summary>
    void OnFindRoomButtonClick()
    {
        // �� ����� �޾ƿ��� ���� �κ� ����.
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// ���� �� ���� ��ư Ŭ��
    /// </summary>
    void OnRandomRoomButtonClick()
    {
        // ����ȭ
        RoomOptions option = new()
        {
            MaxPlayers = 8
        };

        PhotonNetwork.JoinRandomOrCreateRoom(roomOptions: option);
    }

    /// <summary>
    /// �α׾ƿ� ��ư Ŭ��
    /// </summary>
    void OnLogoutButtonClick()
    {
        panelMainMenu.gameObject.SetActive(false);

        PhotonNetwork.Disconnect();
    }

    /// <summary>
    /// �� ���� Ȯ�� ��ư Ŭ��
    /// </summary>
    void OnCreateButtonClick()
    {
        string roomName = inputRoomName.text;
        int maxPlayer = int.Parse(inputPlayerNum.text);

        if (string.IsNullOrEmpty(roomName))
        {
            // 1 / 1000Ȯ���� �̸��� �ߺ��� ���� ������ ��ȿ�� �˻縦 �ؾߵȴ�.
            // ������ �ð��� ������ ��������...
            roomName = $"Room{Random.Range(0, 1000)}";
        }

        if (maxPlayer <= 0)
        {
            maxPlayer = 8;
        }

        PhotonNetwork.CreateRoom
            (
            roomName,
            roomOptions: new RoomOptions() { MaxPlayers = maxPlayer }
            );
    }

    /// <summary>
    /// �� ���� ��� ��ư Ŭ��
    /// </summary>
    void OnCancelButtonClick()
    {
        panelMainMenu.gameObject.SetActive(true);
        panelCreateRoomMenu.gameObject.SetActive(false);
    }

    /// <summary>
    /// �г��� ���濡 �������� ��, ���� ���� ���� �÷��̾��� �г��Ӱ� Text�� �����Ѵ�.
    /// </summary>
    void ChangeNickNameSuccess()
    {
        PhotonNetwork.LocalPlayer.NickName = inputPlayerName.text;

        playerName.text = $"�ȳ��ϼ���, {PhotonNetwork.LocalPlayer.NickName}";
    }

    /// <summary>
    /// �г��� ���濡 �������� ��, �α� �޼����� ����.
    /// </summary>
    void ChangeNickNameFailure()
    {
        UILogManager.Log("�г��� ���� ����.");
    }
}