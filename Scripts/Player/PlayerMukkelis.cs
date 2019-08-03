using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMukkelis : MonoBehaviour
{
    public float enemyRadius;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemyRadius);
    }
}
