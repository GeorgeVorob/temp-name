using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class enemyBasic : MonoBehaviour
{
    Random random = new Random();
    float horizontal;
    public float speed = 7;// Переменная скорости перемещения

    public float shootCoolDownTime = 4.0f;
    public int shootBurstAmount = 3;
    private float CurrentshootCoolDownTime = 0.0f;
    private float burstInterval = 0.2f;
    private float CurrentburstInterval = 0.0f;
    private int CurrentshootBurstAmount=0;
    private bool shootAvalible = true;

    Rigidbody2D body;
    enum Status {idle, approaching, Battle, Shoot}
    Status status = Status.idle;

    public GameObject projectilePrefab;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        CurrentshootBurstAmount = shootBurstAmount;
    }

    // Update is called once per frame
    void Update()
    {
        switch(status)
        {
            case Status.approaching:
                statusBattle();
                break;
            case Status.idle:
                statusIdle();
                break;
            case Status.Battle:
                statusBattle();
                break;
            case Status.Shoot:
                statusShoot();
                break;
        }
    }
    void statusBattle()
    {
        if (shootAvalible)
        {
            status = Status.Shoot;
            return;
        }
            
        CurrentshootCoolDownTime -= Time.deltaTime;
        if (CurrentshootCoolDownTime <= 0)
        {
            shootAvalible = true;
            CurrentshootBurstAmount = shootBurstAmount;
        }
        if (Math.Abs((MainCharacter.body.position - body.position).magnitude) > 5.0f)
        {
            status = Status.idle;
            return;
        }
        Debug.Log("AAAAAAA");
    }
    void statusIdle()
    {
        if (Math.Abs((MainCharacter.body.position - body.position).magnitude) <= 5.0f)
        {
            status = Status.approaching;
        }
        else
        {
            status = Status.idle;
        }
    }
    void statusShoot()
    {
        int horizontal;
        if ((MainCharacter.body.position - body.position).x > 0)
        {
            horizontal = 1;
        }
        else
        {
            horizontal = -1;
        }

        if (CurrentburstInterval <= 0)
        {
            GameObject projectileObject = Instantiate(projectilePrefab, body.position + Vector2.up * 0.5f, Quaternion.identity);
            projectileObject.layer = 11;
            projectileBullet projectile = projectileObject.GetComponent<projectileBullet>();
            projectile.Launch(new Vector2(horizontal, 0));

            CurrentshootBurstAmount--;
            CurrentburstInterval = burstInterval;
            Debug.Log(CurrentshootBurstAmount);
            if (CurrentshootBurstAmount <= 0)
            {
                CurrentshootCoolDownTime = shootCoolDownTime;
                shootAvalible = false;
                status = Status.Battle;
            }
        }
        else
        {
            CurrentburstInterval -= Time.deltaTime;
        }
    }
}