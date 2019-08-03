using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class TowerLogic : MonoBehaviourPunCallbacks
{
    public List<GameObject> targets;
    public GameObject currentTarget;
    public Transform projectileOrigin;
    public GameObject projectile;

    
    public int attackDamage, faction, healthAmount;
    public float minAttackDistance=0.5f;
    public float attackSpeed;
    public float damage;

    private bool starting = true;
    private float startTimer = 0f;
    private bool canAttack;
    Collider attackRange;
    float distance = 0.1f;


    
    



    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        //Get Components


        attackRange = GetComponentInChildren<SphereCollider>();
        targets = new List<GameObject>();

        


        //Set numbers and bools
        canAttack = true;
        


    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if (starting)
        {
            startTimer += Time.deltaTime;
            if (startTimer > 5f) starting = false;
            return;
        }
        
        //sets the target if there is something in the collider
        targets.RemoveAll(item => item == null || item.GetComponent<Healthbar>().dead);
        currentTarget = null;
        if (targets.Count >= 1 && currentTarget == null)
        {
            currentTarget = targets[0];

            //agent.stoppingDistance = currentTarget.GetComponent<PlayerMukkelis>().enemyRadius -.2f;

        }
        else if (targets.Count > 0 && currentTarget == null)
        {
        }

        if (currentTarget != null)
        {
            distance = (transform.position - currentTarget.transform.position).magnitude;
            FaceTarget();

            if (distance < minAttackDistance && canAttack)
            {
                if (PhotonNetwork.IsMasterClient) Attack();
            }
            else if (distance > minAttackDistance) currentTarget = null;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.isTrigger) return;
        Healthbar other = collider.GetComponent<Healthbar>();
        if (other != null && !other.dead && other.side != GetComponent<Healthbar>().side)
        {
            targets.Add(collider.gameObject);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.isTrigger) return;
        if (targets.Contains(collider.gameObject))
        {
            targets.Remove(collider.gameObject);
        }
    }

    void Attack()
    {
        photonView.RPC("RpcShoot", RpcTarget.All, damage, currentTarget.GetComponent<PhotonView>().ViewID);
        canAttack = false;
        Invoke("AttackCoolDown", attackSpeed);
    }
    void AttackCoolDown()
    {
        canAttack = true;
    }
    [PunRPC]
    public void RpcShoot(float damage, int targetID)
    {
        switch (faction)
        {
            case 0:
                GameObject target = PhotonView.Find(targetID).gameObject;
                GameObject obj = Instantiate(projectile, projectileOrigin.position, Quaternion.LookRotation(target.transform.position - transform.position));
                obj.GetComponent<ProjectileScript>().SetTarget(target, damage);
                break;
            case 1:
                GameObject target1 = PhotonView.Find(targetID).gameObject;
                GameObject obj1 = Instantiate(projectile, projectileOrigin.position, Quaternion.LookRotation(target1.transform.position - transform.position));
                obj1.GetComponent<ProjectileScript>().SetTarget(target1, damage);
                break;
            case 2:
                GameObject target2 = PhotonView.Find(targetID).gameObject;
                GameObject obj2 = Instantiate(projectile, projectileOrigin.position, Quaternion.LookRotation(target2.transform.position - transform.position));
                obj2.GetComponent<ProjectileScript>().SetTarget(target2, damage);
                break;
        }
    }

    void FaceTarget()
    {

        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 1f);

    }
}
