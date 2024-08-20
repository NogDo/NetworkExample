using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class CPlayerContoller : MonoBehaviourPun, IPunObservable
{
    #region public 변수
    public Transform pointer;   // 캐릭터가 바라볼 방향
    public Transform shotPoint; // 투사체 생성위치
    public CBomb bombPrefab;    // 폭탄 투사체 프리팹

    public Text textHP;
    public Text textShot;

    public float fMoveSpeed = 5.0f;
    public float fShotPower = 15.0f;
    public float fHP = 100.0f;

    public int nShotCount = 0;
    #endregion

    #region private 변수
    Rigidbody rb;
    Animator animator;
    #endregion

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        pointer.gameObject.SetActive(photonView.IsMine);

        textHP.text = fHP.ToString();
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        Move();

        if (Input.GetButtonDown("Fire1"))
        {
            nShotCount++;
            textShot.text = nShotCount.ToString();

            // 로컬에서만 호출될겁니다.
            //Fire();

            // PhotonNetwork의 RPC를 호출
            photonView.RPC("Fire", RpcTarget.All, shotPoint.position, shotPoint.forward);
        }
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        Rotate();
    }

    /// <summary>
    /// 플레이어 이동
    /// </summary>
    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        rb.velocity = new Vector3(x, 0, z) * fMoveSpeed;
    }

    /// <summary>
    /// 플레이어 회전
    /// </summary>
    void Rotate()
    {
        Vector3 pos = rb.position;  // 내 Rigidbody의 위치
        pos.y = 0;                  // 고저차가 있을 수 있으므로 y축 좌표를 0으로

        Vector3 forward = pointer.position - pos;

        rb.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }

    /// <summary>
    /// 투사체를 발사한다.
    /// Bomb에 Photon View가 붙을 경우 불필요한 패킷이 교환되는 비효율이 발생 하므로,
    /// 특정 클라이언트가 Fire를 호출할 경우 다른 클라이언트에게 RPC를 통해 똑같이 Fire를 호출하도록 하고싶음...
    /// </summary>
    [PunRPC]
    void Fire(Vector3 shotPoint, Vector3 shotDirection, PhotonMessageInfo info)
    {
        // 지연을 보상해서, 서버 시간과 내 클라이언트 시간 차이만큼 값을 보정
        print($"Fire Procedure Called by {info.Sender.NickName}");
        print($"local Time : {PhotonNetwork.Time}");
        print($"server Time : {info.SentServerTime}");

        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);

        CBomb bomb = Instantiate(bombPrefab, shotPoint, Quaternion.identity);

        bomb.rb.AddForce(shotDirection * fShotPower, ForceMode.Impulse);
        bomb.owner = photonView.Owner;

        // 폭탄의 위치에서 폭탄의 운동량만큼 지연시간동안 진행한 위치로 보정
        bomb.rb.position += bomb.rb.velocity * lag;
    }

    /// <summary>
    /// 플레이어 피격
    /// </summary>
    /// <param name="damage">입는 데미지</param>
    public void TakeDamage(float damage)
    {
        fHP -= damage;
        textHP.text = fHP.ToString();
    }

    /// <summary>
    /// 플레이어 체력 회복
    /// </summary>
    /// <param name="amount">회복량</param>
    public void Heal(float amount)
    {
        fHP += amount;
        textHP.text = fHP.ToString();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // stream을 통해서 hp와 shotCount만 동기화
        // stream은 queue의 형태기 때문에 먼저 보낸것을 먼저 받는다.
        if (stream.IsWriting)
        {
            stream.SendNext(fHP);
            stream.SendNext(nShotCount);
        }

        else
        {
            fHP = (float)(stream.ReceiveNext());
            nShotCount = (int)(stream.ReceiveNext());
            textHP.text = fHP.ToString();
            textShot.text = nShotCount.ToString();
        }
    }
}
