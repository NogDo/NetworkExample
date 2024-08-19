using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelManager : MonoBehaviourPunCallbacks
{
    #region static ����
    public static UIPanelManager Instance { get; private set; }
    #endregion

    #region public ����
    public UILoginPanel login;
    public UIMenuPanel menu;
    public UILobbyPanel lobby;
    public UIRoomPanel room;
    #endregion

    #region private ����
    Dictionary<string, GameObject> panelTable;
    #endregion

    void Awake()
    {
        Instance = this;

        panelTable = new Dictionary<string, GameObject>
        {
            { "Login", login.gameObject },
            { "Menu", menu.gameObject },
            { "Lobby", lobby.gameObject },
            { "Room", room.gameObject }
        };

        PanelOpen("Login");
        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnEnable()
    {
        //base.OnEnable();
    }

    public override void OnDisable()
    {
        //base.OnDisable();
    }

    public override void OnConnected()
    {
        PanelOpen("Menu");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        UILogManager.Log($"disconnected cause : {cause}");

        PanelOpen("Login");
    }

    public override void OnJoinedLobby()
    {
        PanelOpen("Lobby");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        lobby.UpdateRoomList(roomList);
    }

    public override void OnLeftLobby()
    {
        PanelOpen("Menu");
    }

    public override void OnJoinedRoom()
    {
        PanelOpen("Room");
    }

    public override void OnCreatedRoom()
    {
        PanelOpen("Room");
    }

    public override void OnLeftRoom()
    {
        PanelOpen("Menu");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        room.JoinPlayer(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        room.LeavePlayer(otherPlayer);
    }

    /// <summary>
    /// �г��� Ȱ��ȭ ��Ų��.
    /// </summary>
    /// <param name="panelName">Ȱ��ȭ ��ų �г� Key��</param>
    public void PanelOpen(string panelName)
    {
        foreach (var row in panelTable)
        {
            row.Value.SetActive(row.Key.Equals(panelName));
        }
    }
}