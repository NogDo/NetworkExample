using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerEntry : MonoBehaviour
{
    #region public ����
    public Text textPlayerName;
    public Toggle toggleReady;

    public GameObject oReadyLabel;
    public Player player;

    public Toggle[] toggles;
    public ToggleGroup toggleGroup;

    public bool isMine => player == PhotonNetwork.LocalPlayer;
    #endregion

    #region private ����

    #endregion

    void Awake()
    {
        //toggleReady.isOn = false; => onValueChagned�� ȣ��... �̷��� �ȵ�
        // �Ʒ��� �˸� ���� ���� �����ϴ� ����̴�.
        // isOn�� ���� ���������� onValueChanged�� ȣ����� �ʴ´�.
        toggleReady.SetIsOnWithoutNotify(false);
    }
}
