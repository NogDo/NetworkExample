using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuPanel : MonoBehaviour
{
    #region public 변수
    public Text playerName;

    [Header("플레이어 닉네임 변경 관련")]
    public InputField inputPlayerName;
    public Button buttonPlayerNameChange;

    [Header("메인 메뉴 관련")]
    public RectTransform panelMainMenu;
    public Button buttonCreateRoom;
    public Button buttonFindRoom;
    public Button buttonRandomRoom;
    public Button buttonLogout;

    [Header("방 생성 관련")]
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
        playerName.text = $"안녕하세요, {PhotonNetwork.LocalPlayer.NickName}";

        panelMainMenu.gameObject.SetActive(true);
        panelCreateRoomMenu.gameObject.SetActive(false);
    }

    /// <summary>
    /// 플레이어 닉네임 변경 버튼 클릭
    /// </summary>
    void OnPlayerNameChangeButtonClick()
    {
        CSqlManager.Instance.ChangeNickName(inputPlayerName.text, ChangeNickNameSuccess, ChangeNickNameFailure);
    }

    /// <summary>
    /// 방 생성 버튼 클릭
    /// </summary>
    void OnCreateRoomButtonClick()
    {
        panelMainMenu.gameObject.SetActive(false);
        panelCreateRoomMenu.gameObject.SetActive(true);
    }

    /// <summary>
    /// 방 찾기 버튼 클릭
    /// </summary>
    void OnFindRoomButtonClick()
    {
        // 방 목록을 받아오기 위해 로비에 입장.
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// 랜덤 방 들어가기 버튼 클릭
    /// </summary>
    void OnRandomRoomButtonClick()
    {
        // 간소화
        RoomOptions option = new()
        {
            MaxPlayers = 8
        };

        PhotonNetwork.JoinRandomOrCreateRoom(roomOptions: option);
    }

    /// <summary>
    /// 로그아웃 버튼 클릭
    /// </summary>
    void OnLogoutButtonClick()
    {
        panelMainMenu.gameObject.SetActive(false);

        PhotonNetwork.Disconnect();
    }

    /// <summary>
    /// 방 생성 확인 버튼 클릭
    /// </summary>
    void OnCreateButtonClick()
    {
        string roomName = inputRoomName.text;
        int maxPlayer = int.Parse(inputPlayerNum.text);

        if (string.IsNullOrEmpty(roomName))
        {
            // 1 / 1000확률로 이름이 중복될 수도 있으니 유효성 검사를 해야된다.
            // 지금은 시간이 없으니 하지않음...
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
    /// 방 생성 취소 버튼 클릭
    /// </summary>
    void OnCancelButtonClick()
    {
        panelMainMenu.gameObject.SetActive(true);
        panelCreateRoomMenu.gameObject.SetActive(false);
    }

    /// <summary>
    /// 닉네임 변경에 성공했을 때, 포톤 서버 로컬 플레이어의 닉네임과 Text를 변경한다.
    /// </summary>
    void ChangeNickNameSuccess()
    {
        PhotonNetwork.LocalPlayer.NickName = inputPlayerName.text;

        playerName.text = $"안녕하세요, {PhotonNetwork.LocalPlayer.NickName}";
    }

    /// <summary>
    /// 닉네임 변경에 실패했을 때, 로그 메세지를 띄운다.
    /// </summary>
    void ChangeNickNameFailure()
    {
        UILogManager.Log("닉네임 변경 실패.");
    }
}