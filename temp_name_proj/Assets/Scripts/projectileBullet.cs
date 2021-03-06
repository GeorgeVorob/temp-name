﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileBullet : MonoBehaviour
{
    Rigidbody2D bulletBody;
    // Start is called before the first frame update
    void Awake()
    {
        bulletBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.magnitude > 10000.0f)
        {
            Destroy(gameObject);
        }
    }
    public void Launch(Vector2 direction)
    {
        bulletBody.AddForce(direction * 500f);
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        Damageable e = other.collider.GetComponent<Damageable>();
        if (e != null)
        {
            e.ChangeHealth(-10);
        }
        Debug.Log("Projectile Collision with " + other.gameObject);
        Destroy(gameObject);
    }
}
