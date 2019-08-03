using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class EnemySpawner : MonoBehaviour
{
    public float spawnTime = 5f; //how long between each spawn.
    public Transform[] spawnPoints;
    public int extraSpawns=0;
    public Healthbar hb;
    public int amountOfCreeps;

    public GameObject[] heroes;

    public string prefab;

    // Start is called before the first frame update
    void Start()
    {
        hb = GetComponent<Healthbar>();
        if (PhotonNetwork.IsMasterClient) InvokeRepeating("Spawn", 30f, spawnTime);
    }

    void Update()
    {


        if (heroes == null)
        {
            heroes = GameObject.FindGameObjectsWithTag("Player");
        }

    }
    void Spawn()
    {
        //int spawnPointIndex = Random.Range(0, spawnPoints.Length);
        for (int i = 0; i < 3 + extraSpawns; i++)
        {
            GameObject obj = PhotonNetwork.InstantiateSceneObject(prefab, spawnPoints[i].position, spawnPoints[i].rotation);
            obj.GetComponent<UnitCreepController>().destination = transform.position + transform.forward * 90;
            Healthbar creep = obj.GetComponent<Healthbar>();

            if(creep!=null) creep.Setup(hb.side);

        }
    }
    
}
