using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : MonoBehaviour
{
    public static PlayerData instance;
    public Faction faction = Faction.Sorceress;
    public string playerName;
    public int gamewon = 0;
    [SerializeField]
    private InputField nameInputField;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(PlayerData.instance.gameObject);
            instance = this;
            DontDestroyOnLoad(this);
        }
    }
    public void SetFaction(int faction)
    {
        this.faction = (Faction) faction;
    }
    public void CatchName()
    {
        playerName = nameInputField.text;
    }
}
