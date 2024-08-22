using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILobbyPanel : MonoBehaviour
{
    #region public 변수
    public RectTransform roomListRect;

    public Button roomButtonPrefab;
    public Button buttonBack;
    #endregion

    #region private 변수
    private List<RoomInfo> currentRoomList = new List<RoomInfo>();
    #endregion

    void Awake()
    {
        buttonBack.onClick.AddListener(OnBackButtonClick);
    }

    void OnDisable()
    {
        foreach (Transform child in roomListRect)
        {
            //Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Room List를 갱신한다.
    /// </summary>
    /// <param name="roomList">방 정보 리스트</param>
    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        // 파괴될 후보
        List<RoomInfo> destroyCandidate = currentRoomList.FindAll
            (
                (x) => false == roomList.Contains(x)
            );

        foreach (RoomInfo roomInfo in roomList)
        {
            if (currentRoomList.Contains(roomInfo))
            {
                continue;
            }

            AddRoomButton(roomInfo);
        }

        foreach (Transform child in roomListRect)
        {
            if (destroyCandidate.Exists((x) => x.Name == child.name))
            {
                Destroy(child.gameObject);
            }
        }

        currentRoomList = roomList;
    }

    /// <summary>
    /// RoomInfoList를 통해 순차적으로 한개씩 방 입장 버튼을 생성
    /// </summary>
    /// <param name="roomInfo">방 정보</param>
    public void AddRoomButton(RoomInfo roomInfo)
    {
        print("AddRoomButton");

        Button joinButton = Instantiate(roomButtonPrefab, roomListRect, false);

        joinButton.gameObject.name = roomInfo.Name;
        joinButton.onClick.AddListener(() => PhotonNetwork.JoinRoom(roomInfo.Name));
        joinButton.GetComponentInChildren<Text>().text = roomInfo.Name;
    }

    /// <summary>
    /// 메인 메뉴로 돌아가기 버튼 클릭
    /// </summary>
    void OnBackButtonClick()
    {
        PhotonNetwork.LeaveLobby();
    }
}