using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class CPlayerContoller : MonoBehaviourPun, IPunObservable
{
    #region public ����
    public Transform pointer;   // ĳ���Ͱ� �ٶ� ����
    public Transform shotPoint; // ����ü ������ġ
    public CBomb bombPrefab;    // ��ź ����ü ������

    public Text textHP;
    public Text textShot;

    public float fMoveSpeed = 5.0f;
    public float fShotPower = 15.0f;
    public float fHP = 100.0f;

    public int nShotCount = 0;
    #endregion

    #region private ����
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

            // ���ÿ����� ȣ��ɰ̴ϴ�.
            //Fire();

            // PhotonNetwork�� RPC�� ȣ��
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
    /// �÷��̾� �̵�
    /// </summary>
    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        rb.velocity = new Vector3(x, 0, z) * fMoveSpeed;
    }

    /// <summary>
    /// �÷��̾� ȸ��
    /// </summary>
    void Rotate()
    {
        Vector3 pos = rb.position;  // �� Rigidbody�� ��ġ
        pos.y = 0;                  // �������� ���� �� �����Ƿ� y�� ��ǥ�� 0����

        Vector3 forward = pointer.position - pos;

        rb.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }

    /// <summary>
    /// ����ü�� �߻��Ѵ�.
    /// Bomb�� Photon View�� ���� ��� ���ʿ��� ��Ŷ�� ��ȯ�Ǵ� ��ȿ���� �߻� �ϹǷ�,
    /// Ư�� Ŭ���̾�Ʈ�� Fire�� ȣ���� ��� �ٸ� Ŭ���̾�Ʈ���� RPC�� ���� �Ȱ��� Fire�� ȣ���ϵ��� �ϰ����...
    /// </summary>
    [PunRPC]
    void Fire(Vector3 shotPoint, Vector3 shotDirection, PhotonMessageInfo info)
    {
        // ������ �����ؼ�, ���� �ð��� �� Ŭ���̾�Ʈ �ð� ���̸�ŭ ���� ����
        print($"Fire Procedure Called by {info.Sender.NickName}");
        print($"local Time : {PhotonNetwork.Time}");
        print($"server Time : {info.SentServerTime}");

        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);

        CBomb bomb = Instantiate(bombPrefab, shotPoint, Quaternion.identity);

        bomb.rb.AddForce(shotDirection * fShotPower, ForceMode.Impulse);
        bomb.owner = photonView.Owner;

        // ��ź�� ��ġ���� ��ź�� �����ŭ �����ð����� ������ ��ġ�� ����
        bomb.rb.position += bomb.rb.velocity * lag;
    }

    /// <summary>
    /// �÷��̾� �ǰ�
    /// </summary>
    /// <param name="damage">�Դ� ������</param>
    public void TakeDamage(float damage)
    {
        fHP -= damage;
        textHP.text = fHP.ToString();
    }

    /// <summary>
    /// �÷��̾� ü�� ȸ��
    /// </summary>
    /// <param name="amount">ȸ����</param>
    public void Heal(float amount)
    {
        fHP += amount;
        textHP.text = fHP.ToString();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // stream�� ���ؼ� hp�� shotCount�� ����ȭ
        // stream�� queue�� ���±� ������ ���� �������� ���� �޴´�.
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
