using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    #region static 변수
    public static TestManager Instance { get; private set; }

    public static bool debugReady = false;
    #endregion

    #region public 변수
    public Transform startPositions;
    #endregion

    #region private 변수

    #endregion

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            StartCoroutine(NormalStart());
        }

        else
        {
            // 개발중 방 생성 및 참여 절차를 건너 뛰었으므로, 자동으로 디버그룸에 입장시킴
            StartCoroutine(DebugStart());
        }
    }

    /// <summary>
    /// 일반적인 게임 시작.
    /// </summary>
    /// <returns></returns>
    IEnumerator NormalStart()
    {
        // PhotonNetwork가 모든 플레이어의 로드 상태를 판단하여 넘버링을 함
        yield return new WaitUntil(() => PhotonNetwork.LocalPlayer.GetPlayerNumber() != -1);

        // Resources 폴더를 이용해 오브젝트를 생성하는 방식 -> 구시대적이다... (하지만 포톤은 이 방식을 사용)
        //GameObject playerPrefab = Resources.Load<GameObject>("Players/Player");
        //Instantiate(playerPrefab, startPositions.GetChild(0).position, Quaternion.identity);

        // 게임에 참여한 방에서 부여된 내 번호.
        // 활용화기 위해서는 게임 씬에 PlayerNumbering 컴포넌트가 존재해야함
        int playerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();

        Transform playerPoistion = startPositions.GetChild(playerNumber);

        // 포톤에서 게임오브젝트를 생성할 때는 Resources폴더의 프리팹을 가져와서 생성하는 방식으로 사용을 한다.
        GameObject playerObj = PhotonNetwork.Instantiate("Players/Player", playerPoistion.position, playerPoistion.rotation);

        playerObj.name = $"Player {playerNumber}";
        playerObj.GetComponent<CPlayerContoller>().eyes[(int)PhotonNetwork.LocalPlayer.CustomProperties["Eyes"]].SetActive(true);
    }

    /// <summary>
    /// 빠른 개발을 위한 디버그 시작.
    /// </summary>
    /// <returns></returns>
    IEnumerator DebugStart()
    {
        // 디버스 상태의 Start 절차
        gameObject.AddComponent<CPhotonDebuger>();

        yield return new WaitUntil(() => debugReady);

        StartCoroutine(NormalStart());
    }
}