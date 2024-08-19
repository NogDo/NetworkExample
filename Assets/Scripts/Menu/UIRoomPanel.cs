using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRoomPanel : MonoBehaviour
{
    #region public ����
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
            // �÷��̾� ��Ͽ� �÷��̾� �̸�ǥ �ϳ��� ����
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
    /// ���� ���� ��ư Ŭ��
    /// </summary>
    void OnStartButtonClick()
    {

    }

    /// <summary>
    /// �� ������ ��ư Ŭ��
    /// </summary>
    void OnCancelButtonClick()
    {
        PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    /// �÷��̾� ������ ȣ���� �޼���, �÷��̾� ������ ��� ���ӿ�����Ʈ�� �����Ѵ�.
    /// </summary>
    /// <param name="newPlayer">������ �÷��̾�</param>
    public void JoinPlayer(Player newPlayer)
    {
        GameObject PlayerName = Instantiate(playerTextPrefab, playerList, false);

        PlayerName.name = newPlayer.NickName;
        PlayerName.GetComponent<Text>().text = newPlayer.NickName;
    }

    /// <summary>
    /// �÷��̾ ���� ������ ȣ���� �޼���, �ش� �÷��̾��� ���ӿ�����Ʈ�� �ı��Ѵ�.
    /// </summary>
    /// <param name="gonePlayer">���� �÷��̾�</param>
    public void LeavePlayer(Player gonePlayer)
    {
        Destroy(playerList.Find(gonePlayer.NickName).gameObject);
    }
}