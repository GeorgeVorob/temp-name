using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyBasic : MonoBehaviour
{
    float horizontal;// Переменная перемещения по оси х
    public float speed = 7;// Переменная скорости перемещения
    public float shootCoolDownTime = 1.0f;
    private float shootTimer = 0.0f;
    private bool shootAvalible = true;
    Rigidbody2D body;
    enum Status {idle, approaching}
    Status status = Status.idle;

    public GameObject projectilePrefab;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Math.Abs((MainCharacter.body.position - body.position).magnitude)<=3.0f)
        {
            status = Status.approaching;
        }
        else
        {
            status = Status.idle;
        }
        switch(status)
        {
            case Status.approaching:
                statusApproaching();
                break;
        }
    }
    void statusApproaching()
    {
        if (shootAvalible)
            shoot();
        else
            shootTimer -= Time.deltaTime;

        if (shootTimer <= 0)
            shootAvalible = true;
        Debug.Log("AAAAAAA");
    }
    void shoot()
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

        GameObject projectileObject = Instantiate(projectilePrefab, body.position + Vector2.up * 0.5f, Quaternion.identity);
        projectileObject.layer = 11;
        projectileBullet projectile = projectileObject.GetComponent<projectileBullet>();
        projectile.Launch(new Vector2(horizontal, 0));
        shootTimer = shootCoolDownTime;
        shootAvalible = false;
    }
}