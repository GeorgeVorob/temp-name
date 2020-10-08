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
    public float visionRadius = 5.0f;
    private float CurrentshootCoolDownTime = 0.0f;
    public float burstInterval = 0.2f;
    public int scatterDegree = 6;
    private float CurrentburstInterval = 0.0f;
    private int CurrentshootBurstAmount=0;
    private bool shootAvalible = true;
    //private CircleCollider2D visionCollider;

    Rigidbody2D body;
    enum Status {idle, approaching, Battle, Shoot}
    Status status = Status.idle;

    public GameObject projectilePrefab;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        CurrentshootBurstAmount = shootBurstAmount;
        //visionCollider = transform.GetChild(0).GetComponent<CircleCollider2D>();
    }
    // Update is called once per frame
    void Update()
    {
        switch(status)
        {
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
        if (Math.Abs((MainCharacter.body.position - body.position).magnitude) > visionRadius)
        {
            status = Status.idle;
            return;
        }
        Debug.Log("AAAAAAA");
    }
    void statusIdle()
    {
        if (Math.Abs((MainCharacter.body.position - body.position).magnitude) <= visionRadius)
        {
            status = Status.Battle;
        }
        else
        {
            status = Status.idle;
        }
    }
    void statusShoot()
    {
        if (CurrentburstInterval <= 0)
        {
            GameObject projectileObject = Instantiate(projectilePrefab, body.position + Vector2.up * 0.5f, Quaternion.identity);
            projectileObject.layer = 11;
            projectileBullet projectile = projectileObject.GetComponent<projectileBullet>();
            Vector2 launchdir = (MainCharacter.body.position - body.position).normalized;
            launchdir = Quaternion.Euler(0, 0, random.Next(scatterDegree*-1, scatterDegree)) * launchdir;
            projectile.Launch(launchdir);
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