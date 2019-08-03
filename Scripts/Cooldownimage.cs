using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cooldownimage : MonoBehaviour
{

    public Image imageCooldown;
    private float Cooldown=1f;
    private float MaxCooldown=1f;
    public bool Unlocked = false;

    // Update is called once per frame
    void Update()
    {
        if (!Unlocked) return;
        Cooldown =Mathf.Clamp(Cooldown - Time.deltaTime,0f,MaxCooldown);
        imageCooldown.fillAmount = Cooldown/MaxCooldown;


    }
    public void StartCooldown(float Cooldown)
    {
        MaxCooldown = Cooldown;
        this.Cooldown = MaxCooldown;

    }
}
