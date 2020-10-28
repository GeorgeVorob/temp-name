using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public float maxHP = 100f;
    private float HP;
    public float invictibleTime = 0.5f;

    bool isInvincible;
    float invincibleTimer;
    // Start is called before the first frame update
    void Start()
    {
        HP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
    }
    public void ChangeHealth(float value)
    {
        if (value < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = invictibleTime;
        }
        HP = Mathf.Clamp(HP + value, 0, maxHP);

        Debug.Log("HP:" + HP);
        if (HP <= 0)
        {
            if (gameObject.name == "MainCharacter")
                Util.GameOver();
            else
                Destroy(gameObject);
        }
    }

}
