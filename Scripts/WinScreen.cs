using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerData.instance == null) {
            gameObject.SetActive(false);
            return;
        }
        switch (PlayerData.instance.gamewon)
        {
            case 0:
                gameObject.SetActive(false);
                break;
            case 1:
                GetComponentInChildren<Text>().text = "You Won the Match!";
                break;
            case 2:
                GetComponentInChildren<Text>().text = "You Lost the Match!";
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
