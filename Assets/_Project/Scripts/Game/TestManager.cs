using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    #region static ����
    public static TestManager Instance { get; private set; }

    public static bool debugReady = false;
    #endregion

    #region public ����
    public Transform startPositions;
    #endregion

    #region private ����

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
            // ������ �� ���� �� ���� ������ �ǳ� �پ����Ƿ�, �ڵ����� ����׷뿡 �����Ŵ
            StartCoroutine(DebugStart());
        }
    }

    /// <summary>
    /// �Ϲ����� ���� ����.
    /// </summary>
    /// <returns></returns>
    IEnumerator NormalStart()
    {
        // PhotonNetwork�� ��� �÷��̾��� �ε� ���¸� �Ǵ��Ͽ� �ѹ����� ��
        yield return new WaitUntil(() => PhotonNetwork.LocalPlayer.GetPlayerNumber() != -1);

        // Resources ������ �̿��� ������Ʈ�� �����ϴ� ��� -> ���ô����̴�... (������ ������ �� ����� ���)
        //GameObject playerPrefab = Resources.Load<GameObject>("Players/Player");
        //Instantiate(playerPrefab, startPositions.GetChild(0).position, Quaternion.identity);

        // ���ӿ� ������ �濡�� �ο��� �� ��ȣ.
        // Ȱ��ȭ�� ���ؼ��� ���� ���� PlayerNumbering ������Ʈ�� �����ؾ���
        int playerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();

        Transform playerPoistion = startPositions.GetChild(playerNumber);

        // ���濡�� ���ӿ�����Ʈ�� ������ ���� Resources������ �������� �����ͼ� �����ϴ� ������� ����� �Ѵ�.
        GameObject playerObj = PhotonNetwork.Instantiate("Players/Player", playerPoistion.position, playerPoistion.rotation);

        playerObj.name = $"Player {playerNumber}";
        playerObj.GetComponent<CPlayerContoller>().eyes[(int)PhotonNetwork.LocalPlayer.CustomProperties["Eyes"]].SetActive(true);
    }

    /// <summary>
    /// ���� ������ ���� ����� ����.
    /// </summary>
    /// <returns></returns>
    IEnumerator DebugStart()
    {
        // ����� ������ Start ����
        gameObject.AddComponent<CPhotonDebuger>();

        yield return new WaitUntil(() => debugReady);

        StartCoroutine(NormalStart());
    }
}