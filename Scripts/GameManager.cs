using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Game setup ja ohjaus tänne kiitos
    
    public static GameManager instance;
    public GameObject[] specialButtons;
    public GameObject specialCD, dashCD, attackCD, levelBar, levelFill;
    public Sprite[] levelSprites;





    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        specialButtons[(int)PlayerData.instance.faction].SetActive(true);
        
    }
    
}
