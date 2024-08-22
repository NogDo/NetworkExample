using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBomb : MonoBehaviour
{
    #region public 변수
    public ParticleSystem particlePrefab;

    public Rigidbody rb { get; private set; }
    public Player owner { get; set; }

    public float fExplosionRadius = 1.5f;
    #endregion

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        ParticleSystem particle = Instantiate(particlePrefab, transform.position, particlePrefab.transform.rotation);

        particle.Play();

        Destroy(particle.gameObject, 3.0f);    // 오브젝트 풀링을 써야한다

        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        Destroy(gameObject, 0.1f);

        Collider[] contactedColliders = Physics.OverlapSphere(transform.position, fExplosionRadius);
        foreach (Collider collider in contactedColliders)
        {
            // 초급 방식
            //if (collider.CompareTag("Player"))
            //{
            //    collider.SendMessage("TakeDamage", 1, SendMessageOptions.DontRequireReceiver);
            //}

            if (collider.TryGetComponent<CPlayerContoller>(out CPlayerContoller playerContoller))
            {
                // local플레이어인 내가 폭탄에 맞은 플레이어와 동일인일 경우 true
                bool isMine = PhotonNetwork.LocalPlayer.ActorNumber == playerContoller.photonView.Owner.ActorNumber;

                if (isMine)
                {
                    playerContoller.TakeDamage(1.0f);
                }

                print($"{owner.NickName}이 던진 폭탄이 {playerContoller.photonView.Owner.NickName}에게 맞음");
            }
        }
    }
}