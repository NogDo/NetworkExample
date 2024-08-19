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

    }

    /// <summary>
    /// 방 나가기 버튼 클릭
    /// </summary>
    void OnCancelButtonClick()
    {
        PhotonNetwork.LeaveRoom();
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