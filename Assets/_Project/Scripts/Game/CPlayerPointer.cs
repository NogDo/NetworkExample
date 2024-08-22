using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerPointer : MonoBehaviour
{
    #region public ����
    public LayerMask mask;
    #endregion

    #region private ����
    Camera cam;
    #endregion

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000, mask))
        {
            transform.position = hit.point;
        }
    }
}