using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(PhotonView))]
public class Healthbar : MonoBehaviourPunCallbacks, IPunObservable
{

    Vector3 localScale;
    
    //Healths and the amount the health increases when leveling up.
    public float maxHealth;
    public float health;
    public float levelUpAmount = 12f;
    public float regen = 1f;
    public bool stunned;
    private float stunTime;

    public bool dead;

    public float damageReduction;


    public int side = 0;
    public string enemyType;

    public Material[] teamColor;
    public Transform barpos;
    public GameObject hpbar;
    public GameObject barPrefab;
    public MeshRenderer targetCircle;
    
    public GameObject enemyHero;
    public GameObject deathClouds;

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;


    //public PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        targetCircle = transform.Find("PlaneForTarget").GetComponent<MeshRenderer>();
        targetCircle.enabled = false;
        stunned = false;

        if (enemyHero == null)
        {
            if(side == 1)
            {
                enemyHero = MultiplayerGameManager.instance.player2;
            }
            else
            {
                enemyHero = MultiplayerGameManager.instance.player1;
            }
        }

        if (GetComponent<UnitCreepController>() != null)
        {
            enemyType = "Creep";
        }
        else if(GetComponent<PC_LIIKKUU_10>() != null)
        {
            enemyType = "Hero";
        }
        else if (GetComponent<TowerLogic>() != null)
        {
            enemyType = "Tower";
        }
        else if (GetComponent<EnemySpawner>() != null)
        {
            enemyType = "Spawner";
        }

            barpos = transform.Find("HPBarPos");
        if (barpos != null)
        {
            if (barPrefab != null)
            {
                hpbar = Instantiate(barPrefab, FindObjectOfType<Canvas>().transform);
            }
            else
            {
                Debug.LogError("BarPrefab is not set in Healthbar Script of: " + gameObject.name);
            }
        }
        // localScale = transform.localScale;
    }
    private void LateUpdate()
    {

        if (hpbar != null)
        {
            hpbar.GetComponent<Slider>().value = (health / maxHealth);
            Vector3 position = Camera.main.WorldToScreenPoint(barpos.position);
            //new Vector2(position.x, position.y);
            hpbar.GetComponent<RectTransform>().position = position;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(stunned)
        {
            stunTime -= Time.deltaTime;

            if(stunTime <= 0f)
            {
                stunned = false;
            }
        }

        

        if(enemyType == "Hero")
        {
            health += Time.deltaTime * regen;
            if (health >= maxHealth) health = maxHealth;
        }
        SkinnedMeshRenderer renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (teamColor.Length == 3 && renderer != null)
        {
            switch (side)
            {
                case 0:
                    renderer.material = teamColor[side];
                    break;
                case 1:
                    renderer.material = teamColor[side];
                    break;
                case 2:
                    renderer.material = teamColor[side];
                    break;
            }
        }
        MeshRenderer[] mrenderers = GetComponentsInChildren<MeshRenderer>();
        if (teamColor.Length == 3 && mrenderers != null)
        {
            foreach (MeshRenderer mrenderer in mrenderers)
            {
                if (mrenderer.gameObject.CompareTag("IndicatorPlane")) continue;
                switch (side)
                {
                    case 0:
                        mrenderer.material = teamColor[side];
                        break;
                    case 1:
                        mrenderer.material = teamColor[side];
                        break;
                    case 2:
                        mrenderer.material = teamColor[side];
                        break;
                }
            }
        }

        //localScale.x = UnitCreepController.Health;
        //&&transform.localScale = localScale;
    }
    [PunRPC]
    public void RpcTakeDamage(float damage, bool sourceIsPlayer)
    {
        if (GetComponent<PC_LIIKKUU_10>() != null)
        {
            if (dead) return;
            if (GetComponent<PC_LIIKKUU_10>().channelingSpecial)
            {
                damage *= 0.3f;
            }
            else
            {
                health -= damage;
            }
        }
        else
        {
            health -= damage;
        }

        if (health <= 0 )
        {
            if (!PhotonNetwork.IsMasterClient) return;
            photonView.RPC("DestroyOnClients", RpcTarget.All);
            if (enemyType == "Tower" || enemyType == "Spawner") return;
            if (enemyType == "Hero")
            {
                enemyHero.GetComponent<PC_LIIKKUU_10>().photonView.RPC("GainXp",RpcTarget.All,(int) 9);
            }
            else if (sourceIsPlayer)
            {
                enemyHero.GetComponent<PC_LIIKKUU_10>().photonView.RPC("GainXp", RpcTarget.All, (int) 5);
            }
            else
            {
                enemyHero.GetComponent<PC_LIIKKUU_10>().photonView.RPC("GainXp", RpcTarget.All, (int) 3);
            }

        }


    }

    [PunRPC]
    public void RpcSetSide(int newSide)
    {
        Debug.Log("RPC SetSide called");
        this.side = newSide;
    }
    [PunRPC]
    public void RpcSetPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        transform.SetPositionAndRotation(position, rotation);
        spawnPosition = position;
        spawnRotation = rotation;
        
    }
    [PunRPC]
    public void DestroyOnClients()
    {
        if(enemyType != "Hero")
        {
            if (gameObject.GetComponent<Animator>() != null) gameObject.GetComponent<Animator>().SetTrigger("Death");
            dead = true;
            GameObject clouds = Instantiate(deathClouds, gameObject.transform.position, gameObject.transform.rotation);
            Destroy(clouds, 1f);
            Destroy(gameObject, 1f);
        }
        else if(enemyType == "Spawner") {
            MultiplayerGameManager.instance.EndGame(side);
        }else
        {
            Death();
            targetCircle.enabled = false;
        }
    }
    [PunRPC]
    public void RpcStun(float stunTime)
    {
        stunned = true;
        this.stunTime = stunTime;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(health);
            stream.SendNext(side);
        }
        else if (stream.IsReading)
        {
            // Network player, receive data
            this.health = (float)stream.ReceiveNext();
            this.side = (int)stream.ReceiveNext();
        }
    }
    public void Setup(int side)
    {
        this.side = side;
    }
    private void OnDestroy()
    {
        Destroy(hpbar);
    }
    public void LevelUp()
    {
        health += levelUpAmount;
        maxHealth += levelUpAmount;
    }

    void Death()
    {
        gameObject.GetComponent<Animator>().SetTrigger("Death");
        dead = true;
        Invoke("Respawn", 15);
        health = maxHealth;
    }
    void Respawn()
    {
        gameObject.GetComponent<Animator>().SetTrigger("Reset");
        gameObject.transform.SetPositionAndRotation(spawnPosition, spawnRotation);
        dead = false;
    }
}
