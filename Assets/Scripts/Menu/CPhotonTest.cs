using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPhotonTest : MonoBehaviour
{
    #region private ����
    ClientState photonState = 0;    // cache �뵵�� Ȱ���ϱ� �빮�� private���� ���Ƶд�.
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