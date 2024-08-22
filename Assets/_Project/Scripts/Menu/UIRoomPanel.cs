using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public enum EDifficulty
{
    EASY,
    NORMAL,
    HARD
}


public class UIRoomPanel : MonoBehaviourPunCallbacks
{
    #region public ����
    public Text textRoomTitle;

    public RectTransform playerList;
    public GameObject playerTextPrefab;

    public Button buttonStart;
    public Button buttonCancel;

    public Dropdown dropdownDiff;
    public Text textDiff;

    // �濡 ���� ��� �÷��̾���� ���θ� �˰� �ֵ��� ����� dictionary
    public Dictionary<int, UIPlayerEntry> playerEntries = new Dictionary<int, UIPlayerEntry>();
    #endregion

    #region private ����
    // ������ ���, �÷��̾���� ready ���¸� ������ dictionary
    Dictionary<int, bool> playersReady;
    #endregion

    void Awake()
    {
        buttonStart.onClick.AddListener(OnStartButtonClick);
        buttonCancel.onClick.AddListener(OnCancelButtonClick);

        dropdownDiff.ClearOptions();
        foreach (object diff in Enum.GetValues(typeof(EDifficulty)))
        {
            Dropdown.OptionData option = new Dropdown.OptionData(diff.ToString());

            dropdownDiff.options.Add(option);
        }

        dropdownDiff.onValueChanged.AddListener(DifficultyValueChange);
    }

    public override void OnEnable()
    {
        base.OnEnable();

        textRoomTitle.text = PhotonNetwork.CurrentRoom.Name;

        if (PhotonNetwork.IsMasterClient)
        {
            playersReady = new Dictionary<int, bool>();
        }

        else
        {
            textDiff.text = ((EDifficulty)dropdownDiff.value).ToString();
        }

        buttonStart.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        dropdownDiff.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        textDiff.gameObject.SetActive(!PhotonNetwork.IsMasterClient);

        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            // �÷��̾� ��Ͽ� �÷��̾� �̸�ǥ �ϳ��� ����
            JoinPlayer(player);

            if (player.CustomProperties.ContainsKey("Ready"))
            {
                SetPlayerReady(player.ActorNumber, (bool)player.CustomProperties["Ready"]);
            }
        }

        // �濡 ���� ���� ��, ������ �� �ε� ���ο� ���� �Բ� �� �ε�
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnDisable()
    {
        base.OnDisable();

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
        // Photon�� ���� �÷��̾��� ���� ����ȭ�Ͽ� �ε�
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    /// <summary>
    /// �� ������ ��ư Ŭ��
    /// </summary>
    void OnCancelButtonClick()
    {
        PhotonHashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;

        customProps["Dictionary"] = playersReady;

        PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);

        PhotonNetwork.LeaveRoom();
        // �ð� �������� ���� ���� �����Ͽ��µ� ������ ���� �ݿ� ���� ���� �Ѿ�°��� ����
        PhotonNetwork.AutomaticallySyncScene = false;
    }

    /// <summary>
    /// Ready Toggle�� Ŭ������ ��, Custom Properties ����
    /// </summary>
    /// <param name="isOn">��� ����</param>
    public void ReadyToggleClick(bool isOn)
    {
        Player localPlayer = PhotonNetwork.LocalPlayer;

        // PhotonNetwork�� Custom Properties�� Hashtable ������ Ȱ���Ѵ�.
        // Hashtable�� Dotnet���� �ƴ� ����ȭ ������ Hashtable Ŭ������ ���� ����
        PhotonHashtable customProps = localPlayer.CustomProperties;

        customProps["Ready"] = isOn;

        localPlayer.SetCustomProperties(customProps);
    }


    public void OnEyeToggleClick(bool isOn)
    {
        if (isOn)
        {
            Player localPlayer = PhotonNetwork.LocalPlayer;
            PhotonHashtable customProrps = localPlayer.CustomProperties;
            ToggleGroup group = playerEntries[PhotonNetwork.LocalPlayer.ActorNumber].toggleGroup;

            foreach (Toggle toggle in group.ActiveToggles())
            {
                customProrps["Eyes"] = toggle.GetComponent<CPlayerEye>().Eye;
            }

            localPlayer.SetCustomProperties(customProrps);

            print(((EPlayerEye)customProrps["Eyes"]).ToString());
        }

        else
        {
            return;
        }
    }

    /// <summary>
    /// �ٸ� �÷��̾ Ready Toggle�� �������� ��� �� Ŭ�����̾�Ʈ�� �ݿ�
    /// </summary>
    /// <param name="actorNumber"></param>
    /// <param name="isReady"></param>
    public void SetPlayerReady(int actorNumber, bool isReady)
    {
        playerEntries[actorNumber].oReadyLabel.gameObject.SetActive(isReady);

        if (PhotonNetwork.IsMasterClient)
        {
            playersReady[actorNumber] = isReady;
            CheckReady();
        }
    }

    /// <summary>
    /// ������ ��� �ٸ� �÷��̾ ��� ready �������� Ȯ���Ͽ� Start��ư�� Ȱ��ȭ ���θ� ����
    /// </summary>
    void CheckReady()
    {
        bool allReady = playersReady.Values.All(x => x);

        //foreach (bool isReady in playersReady.Values)
        //{
        //    if (isReady)
        //    {
        //        continue;
        //    }

        //    else
        //    {
        //        allReady = false;
        //        break;
        //    }
        //}

        buttonStart.interactable = allReady;
    }

    /// <summary>
    /// �÷��̾� ������ ȣ���� �޼���, �÷��̾� ������ ��� ���ӿ�����Ʈ�� �����Ѵ�.
    /// </summary>
    /// <param name="newPlayer">������ �÷��̾�</param>
    public void JoinPlayer(Player newPlayer)
    {
        UIPlayerEntry playerEntry = Instantiate(playerTextPrefab, playerList, false).GetComponent<UIPlayerEntry>();

        playerEntry.player = newPlayer;
        playerEntry.textPlayerName.text = newPlayer.NickName;

        Toggle toggleReady = playerEntry.toggleReady;

        if (PhotonNetwork.LocalPlayer.ActorNumber == newPlayer.ActorNumber)
        {
            toggleReady.onValueChanged.AddListener(ReadyToggleClick);

            foreach (Toggle toggleEye in playerEntry.toggles)
            {
                toggleEye.onValueChanged.AddListener(OnEyeToggleClick);
            }

            PhotonHashtable customProps = PhotonNetwork.LocalPlayer.CustomProperties;
            customProps["Eyes"] = (EPlayerEye)0;

            PhotonNetwork.LocalPlayer.SetCustomProperties(customProps);
        }

        else
        {
            toggleReady.gameObject.SetActive(false);
        }

        playerEntries[newPlayer.ActorNumber] = playerEntry;

        if (PhotonNetwork.IsMasterClient)
        {
            playersReady[newPlayer.ActorNumber] = false;
            CheckReady();
        }

        SortPlayers();

        PhotonHashtable props = PhotonNetwork.CurrentRoom.CustomProperties;

        OnRoomPropertiesUpdate(props);
    }

    /// <summary>
    /// �÷��̾ ���� ������ ȣ���� �޼���, �ش� �÷��̾��� ���ӿ�����Ʈ�� �ı��Ѵ�.
    /// </summary>
    /// <param name="gonePlayer">���� �÷��̾�</param>
    public void LeavePlayer(Player gonePlayer)
    {
        GameObject leaveTarget = playerEntries[gonePlayer.ActorNumber].gameObject;
        playerEntries.Remove(gonePlayer.ActorNumber);

        Destroy(leaveTarget);

        if (PhotonNetwork.IsMasterClient)
        {
            playersReady.Remove(gonePlayer.ActorNumber);
            CheckReady();
        }

        SortPlayers();
    }

    /// <summary>
    /// �÷��̾� ����Ʈ ����
    /// </summary>
    public void SortPlayers()
    {
        foreach (int actorNumber in playerEntries.Keys)
        {
            print(actorNumber);

            // SetSiblingIndex => Hierachy�� �� �θ� �ȿ��� �ٸ� ��ü �� ������ �����ϰ� ������ ���
            playerEntries[actorNumber].transform.SetSiblingIndex(actorNumber);
        }
    }

    /// <summary>
    /// �� ���̵� ����
    /// </summary>
    /// <param name="value">���̵�</param>
    void DifficultyValueChange(int value)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        PhotonHashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        customProps["Diff"] = value;

        PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);
    }

    public override void OnRoomPropertiesUpdate(PhotonHashtable props)
    {
        if (props.ContainsKey("Diff"))
        {
            print($"Room Difficulty Changed : {props["Diff"]}");
            textDiff.text = ((EDifficulty)props["Diff"]).ToString();
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
    {
        if (changedProps.ContainsKey("Ready"))
        {
            SetPlayerReady(targetPlayer.ActorNumber, (bool)changedProps["Ready"]);
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            PhotonHashtable props = PhotonNetwork.CurrentRoom.CustomProperties;

            if (props.ContainsKey("Dictionary"))
            {
                playersReady = (Dictionary<int, bool>)props["Dictionary"];

                buttonStart.gameObject.SetActive(true);
                dropdownDiff.gameObject.SetActive(true);
                textDiff.gameObject.SetActive(false);
            }
        }
    }
}