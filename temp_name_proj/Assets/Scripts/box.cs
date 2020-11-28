using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class box : MonoBehaviour
{
    private Rigidbody2D body;

    public float tenacity_max = 0;
    private float tenacity_curent;
    private float vel = 0;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        tenacity_curent = tenacity_max;
    }

    // Update is called once per frame
    void Update()
    {
        float loc_vel = (float)System.Math.Round(body.velocity.magnitude, 2);
        if (loc_vel != 0)
            vel = loc_vel;
        if (tenacity_curent <= 0) Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.rigidbody.bodyType == RigidbodyType2D.Dynamic)
        {
            float coll_mass = collision.collider.gameObject.GetComponent<Rigidbody2D>().mass;
            float coll_vel = collision.collider.gameObject.GetComponent<box>().vel;
            float f = coll_vel * coll_mass + vel * body.mass;
            Debug.Log(this.name + " // " + vel +" // "+ coll_vel);
            tenacity_curent -= f;
            float proc = tenacity_curent / tenacity_max;
            GetComponent<SpriteRenderer>().color = new Color(1.0f, proc, proc);
        }
        else
        {
            float f =vel * body.mass;
            tenacity_curent -= f;
            float proc = tenacity_curent / tenacity_max;
            GetComponent<SpriteRenderer>().color = new Color(1.0f, proc, proc);
        }
        //Debug.Log(f);

    }
}
