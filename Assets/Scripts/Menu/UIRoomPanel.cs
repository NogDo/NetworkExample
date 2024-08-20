using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRoomPanel : MonoBehaviour
{
    #region public 변수
    public Text textRoomTitle;

    public RectTransform playerList;
    public GameObject playerTextPrefab;

    public Button buttonStart;
    public Button buttonCancel;
    #endregion

    void Awake()
    {
        buttonStart.onClick.AddListener(OnStartButtonClick);
        buttonCancel.onClick.AddListener(OnCancelButtonClick);
    }

    void OnEnable()
    {
        textRoomTitle.text = PhotonNetwork.CurrentRoom.Name;

        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            // 플레이어 목록에 플레이어 이름표 하나씩 생성
            JoinPlayer(player);
        }

        buttonStart.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        // 방에 입장 했을 때, 방장의 씬 로드 여부에 따라 함께 씬 로드
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void OnDisable()
    {
        foreach (Transform child in playerList.transform)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// 게임 시작 버튼 클릭
    /// </summary>
    void OnStartButtonClick()
    {
        // Photon을 통해 플레이어들과 씬을 동기화하여 로드
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    /// <summary>
    /// 방 나가기 버튼 클릭
    /// </summary>
    void OnCancelButtonClick()
    {
        PhotonNetwork.LeaveRoom();
        // 시간 지연으로 인해 방을 퇴장하였는데 방장의 시작 콜에 의해 씬이 넘어가는것을 방지
        PhotonNetwork.AutomaticallySyncScene = false;
    }

    /// <summary>
    /// 플레이어 참가시 호출할 메서드, 플레이어 정보가 담긴 게임오브젝트를 생성한다.
    /// </summary>
    /// <param name="newPlayer">참가한 플레이어</param>
    public void JoinPlayer(Player newPlayer)
    {
        GameObject PlayerName = Instantiate(playerTextPrefab, playerList, false);

        PlayerName.name = newPlayer.NickName;
        PlayerName.GetComponent<Text>().text = newPlayer.NickName;
    }

    /// <summary>
    /// 플레이어가 방을 나가면 호출할 메서드, 해당 플레이어의 게임오브젝트를 파괴한다.
    /// </summary>
    /// <param name="gonePlayer">나간 플레이어</param>
    public void LeavePlayer(Player gonePlayer)
    {
        Destroy(playerList.Find(gonePlayer.NickName).gameObject);
    }
}