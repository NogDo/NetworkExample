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
    #region public 변수
    public Text textRoomTitle;

    public RectTransform playerList;
    public GameObject playerTextPrefab;

    public Button buttonStart;
    public Button buttonCancel;

    public Dropdown dropdownDiff;
    public Text textDiff;

    // 방에 들어온 모든 플레이어들이 서로를 알고 있도록 사용할 dictionary
    public Dictionary<int, UIPlayerEntry> playerEntries = new Dictionary<int, UIPlayerEntry>();
    #endregion

    #region private 변수
    // 방장일 경우, 플레이어들의 ready 상태를 저장할 dictionary
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
            // 플레이어 목록에 플레이어 이름표 하나씩 생성
            JoinPlayer(player);

            if (player.CustomProperties.ContainsKey("Ready"))
            {
                SetPlayerReady(player.ActorNumber, (bool)player.CustomProperties["Ready"]);
            }
        }

        // 방에 입장 했을 때, 방장의 씬 로드 여부에 따라 함께 씬 로드
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
        PhotonHashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;

        customProps["Dictionary"] = playersReady;

        PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);

        PhotonNetwork.LeaveRoom();
        // 시간 지연으로 인해 방을 퇴장하였는데 방장의 시작 콜에 의해 씬이 넘어가는것을 방지
        PhotonNetwork.AutomaticallySyncScene = false;
    }

    /// <summary>
    /// Ready Toggle을 클릭했을 때, Custom Properties 변경
    /// </summary>
    /// <param name="isOn">토글 상태</param>
    public void ReadyToggleClick(bool isOn)
    {
        Player localPlayer = PhotonNetwork.LocalPlayer;

        // PhotonNetwork의 Custom Properties는 Hashtable 구조를 활용한다.
        // Hashtable은 Dotnet것이 아닌 간소화 형태의 Hashtable 클래스를 직접 제공
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
    /// 다른 플레이어가 Ready Toggle을 변경했을 경우 내 클라이이언트에 반영
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
    /// 방장일 경우 다른 플레이어가 모두 ready 상태인지 확인하여 Start버튼의 활성화 여부를 결정
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
    /// 플레이어 참가시 호출할 메서드, 플레이어 정보가 담긴 게임오브젝트를 생성한다.
    /// </summary>
    /// <param name="newPlayer">참가한 플레이어</param>
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
    /// 플레이어가 방을 나가면 호출할 메서드, 해당 플레이어의 게임오브젝트를 파괴한다.
    /// </summary>
    /// <param name="gonePlayer">나간 플레이어</param>
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
    /// 플레이어 리스트 정렬
    /// </summary>
    public void SortPlayers()
    {
        foreach (int actorNumber in playerEntries.Keys)
        {
            print(actorNumber);

            // SetSiblingIndex => Hierachy상 내 부모 안에서 다른 객체 중 순서를 지정하고 싶을때 사용
            playerEntries[actorNumber].transform.SetSiblingIndex(actorNumber);
        }
    }

    /// <summary>
    /// 방 난이도 설정
    /// </summary>
    /// <param name="value">난이도</param>
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