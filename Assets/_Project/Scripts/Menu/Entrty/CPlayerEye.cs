using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPlayerEye
{
    GLASSES,
    EYES,
    LENS
}

public class CPlayerEye : MonoBehaviour
{
    #region private ����
    [SerializeField]
    EPlayerEye playerEye;
    #endregion

    public EPlayerEye Eye
    {
        get
        {
            return playerEye;
        }
    }
}
