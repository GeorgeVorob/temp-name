using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class MainCharacter : MonoBehaviour
{
    Rigidbody2D body;
    float horizontal;// Переменная перемещения по оси х
    public float speed;// Переменная скорости перемещения
    public float force;
    public bool grounded = false;// Переиенная состояния "на земле"
    public bool hangedOn = false;

    Telekinesis telekines = new Telekinesis(); //объект телекинеза

    GameObject lastClicked; //последний GameObject, на который нажал игрок (при отключенном захвате)

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        telekines.avalible = true;
       // RigidbodyInterpolation2D a = new RigidbodyInterpolation2D();
       // var a =body.interpolation;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump") && grounded)
        {
            //body.AddForce(); // -В чём сила брат? -В ньютанах.
            body.isKinematic = false;
            body.AddForce(new Vector2(0f, force));
            grounded = false;
            hangedOn = false;
            //body.gravityScale = 1;
            //body.simulated = true;
        }

        if (!hangedOn)
        {
            RaycastHit2D leftFeet = Physics2D.Raycast(body.position + new Vector2(-0.5f, -0.9f), Vector2.down, 0.05f);
            RaycastHit2D RightFeet = Physics2D.Raycast(body.position + new Vector2(0.5f, -0.9f), Vector2.down, 0.05f);
            if (leftFeet.collider != null || RightFeet.collider != null)
            {
                grounded = true;
            }
            else
            {
                grounded = false;
            }

            horizontal = Input.GetAxis("Horizontal");

            if (horizontal > 0)
            {
                RaycastHit2D climpBottom = Physics2D.Raycast(body.position + new Vector2(0.55f, -0.9f), Vector2.right, 0.1f);
                RaycastHit2D climptop = Physics2D.Raycast(body.position + new Vector2(0.55f, -0.7f), Vector2.right, 0.1f);
                if (climpBottom.collider != null && climptop.collider == null)
                {
                    body.transform.position += new Vector3(0.1f, 0.2f, 0.0f);
                    body.velocity = new Vector2(body.velocity.x, 0);
                }
                RaycastHit2D HangOnBottom = Physics2D.Raycast(body.position + new Vector2(0.55f, 0.7f), Vector2.right, 0.1f);
                RaycastHit2D HangOnTop = Physics2D.Raycast(body.position + new Vector2(0.55f, 0.9f), Vector2.right, 0.1f);
                if (HangOnBottom.collider != null && HangOnTop.collider == null && body.velocity.y <= 0)
                {
                    grounded = true;
                    hangedOn = true;
                    //body.gravityScale = 0;
                    Vector2 cornerRightTop = new Vector2(0.5f, 0.9f);
                    body.position = HangOnBottom.collider.ClosestPoint(body.position + cornerRightTop) - cornerRightTop;
                    body.isKinematic = true;
                    body.velocity = new Vector2(0, 0);
                    //body.gravityScale = 0;
                }
            }

            if (horizontal < 0)
            {
                RaycastHit2D climpBottom = Physics2D.Raycast(body.position + new Vector2(-0.55f, -0.9f), Vector2.left, 0.1f);
                RaycastHit2D climptop = Physics2D.Raycast(body.position + new Vector2(-0.55f, -0.7f), Vector2.left, 0.1f);
                if (climpBottom.collider != null && climptop.collider == null)
                {
                    body.transform.position += new Vector3(-0.1f, 0.2f, 0.0f);
                    body.velocity = new Vector2(body.velocity.x, 0);
                }
                RaycastHit2D HangOnBottom = Physics2D.Raycast(body.position + new Vector2(-0.55f, 0.7f), Vector2.left, 0.1f);
                RaycastHit2D HangOnTop = Physics2D.Raycast(body.position + new Vector2(-0.55f, 0.9f), Vector2.left, 0.1f);
                if (HangOnBottom.collider != null && HangOnTop.collider == null && body.velocity.y <= 0)
                {
                    grounded = true;
                    hangedOn = true;
                    //body.gravityScale = 0;
                    Vector2 cornerRightTop = new Vector2(-0.5f, 0.9f);
                    body.position = HangOnBottom.collider.ClosestPoint(body.position + cornerRightTop) - cornerRightTop;
                    body.isKinematic = true;
                    body.velocity = new Vector2(0, 0);
                    //body.gravityScale = 0;
                }

            }
        }
        else
        {
            if (Input.GetAxis("Vertical") != 0)
            {
                float vertical = Input.GetAxis("Vertical");
                if (vertical < 0)
                {
                    //body.simulated = true;
                    body.isKinematic = false;
                    grounded = false;
                    hangedOn = false;
                }
            }
        }

        /*if (horizontal < 0)
        {
            RaycastHit2D climpBottom = Physics2D.Raycast(body.position + new Vector2(-0.55f, -0.9f), Vector2.left, 0.1f);
            RaycastHit2D climptop = Physics2D.Raycast(body.position + new Vector2(-0.55f, -0.6f), Vector2.left, 0.1f);
            if (climpBottom.collider != null && climptop.collider == null) body.AddForce(new Vector2(0, 900f));
        }*/

        /* Обработка нажатия кнопки прыжка */

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if ((hit.collider != null))
            {
                lastClicked = hit.collider.gameObject;


                if (telekines.avalible)
                {
                    if (!telekines.working)
                    {
                        telekines.Start(lastClicked);
                    }
                    else
                    {
                        telekines.Stop();
                    }
                }
            }
        }

    }

    void FixedUpdate()
    {
        if(!hangedOn)
        body.velocity = new Vector2(horizontal*speed, body.velocity.y);
       // Debug.Log(body.velocity);
        if(telekines.working)
        {
            telekines.Work();
        }

    }

}
