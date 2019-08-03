using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ProjectileScript : MonoBehaviour
{
    Rigidbody rb;
    public float speed;
    public GameObject currentTarget;
    float damage;

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (currentTarget == null)
        {
            Destroy(gameObject);
            return;
        }
        rb.velocity = (currentTarget.transform.position - transform.position).normalized * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != currentTarget)
        {
            return;
        }
        if(PhotonNetwork.IsMasterClient) currentTarget.GetComponent<Healthbar>().photonView.RPC("RpcTakeDamage", RpcTarget.All, damage, false);
        Destroy(gameObject);
    }
    public void SetTarget(GameObject target, float damage)
    {
        this.damage = damage;
        currentTarget = target;
    }

}
