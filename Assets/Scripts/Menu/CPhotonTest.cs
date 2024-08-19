using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPhotonTest : MonoBehaviour
{
    #region private 변수
    ClientState photonState = 0;    // cache 용도로 활용하기 대문에 private으로 막아둔다.
    #endregion

    void Update()
    {
        if (PhotonNetwork.NetworkClientState != photonState)
        {
            UILogManager.Log($"State Changed : {PhotonNetwork.NetworkClientState}");

            photonState = PhotonNetwork.NetworkClientState;
        }
    }
}