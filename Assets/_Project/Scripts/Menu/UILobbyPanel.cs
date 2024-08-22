using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILobbyPanel : MonoBehaviour
{
    #region public ����
    public RectTransform roomListRect;

    public Button roomButtonPrefab;
    public Button buttonBack;
    #endregion

    #region private ����
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
    /// Room List�� �����Ѵ�.
    /// </summary>
    /// <param name="roomList">�� ���� ����Ʈ</param>
    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        // �ı��� �ĺ�
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
    /// RoomInfoList�� ���� ���������� �Ѱ��� �� ���� ��ư�� ����
    /// </summary>
    /// <param name="roomInfo">�� ����</param>
    public void AddRoomButton(RoomInfo roomInfo)
    {
        print("AddRoomButton");

        Button joinButton = Instantiate(roomButtonPrefab, roomListRect, false);

        joinButton.gameObject.name = roomInfo.Name;
        joinButton.onClick.AddListener(() => PhotonNetwork.JoinRoom(roomInfo.Name));
        joinButton.GetComponentInChildren<Text>().text = roomInfo.Name;
    }

    /// <summary>
    /// ���� �޴��� ���ư��� ��ư Ŭ��
    /// </summary>
    void OnBackButtonClick()
    {
        PhotonNetwork.LeaveLobby();
    }
}