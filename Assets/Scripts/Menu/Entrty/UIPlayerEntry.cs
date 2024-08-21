using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerEntry : MonoBehaviour
{
    #region public 변수
    public Text textPlayerName;
    public Toggle toggleReady;

    public GameObject oReadyLabel;
    public Player player;

    public Toggle[] toggles;
    public ToggleGroup toggleGroup;

    public bool isMine => player == PhotonNetwork.LocalPlayer;
    #endregion

    #region private 변수

    #endregion

    void Awake()
    {
        //toggleReady.isOn = false; => onValueChagned가 호출... 이러면 안됨
        // 아래는 알림 없이 값을 변경하는 방법이다.
        // isOn의 값을 수정하지만 onValueChanged가 호출되지 않는다.
        toggleReady.SetIsOnWithoutNotify(false);
    }
}
