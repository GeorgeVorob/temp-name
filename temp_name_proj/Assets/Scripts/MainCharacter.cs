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
    public static Rigidbody2D body;
    float horizontal;// Переменная перемещения по оси х
    public float speed;// Переменная скорости перемещения
    public float force;
    public bool grounded = false;// Переиенная состояния "на земле"
    public bool hangedOn = false;

    Telekinesis telekines = new Telekinesis(); //объект телекинеза

    Grab grab;
    public bool grab_pull_avalible = true;
    public bool grab_hold_avalible = true;
    public bool grab_hlabysh_avalible = true;
    public float grab_range = 2.0f;
    public float grab_pull_power = 100f;
    public float grab_hlabysh_power = 900f;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        telekines.avalible = true;
        grab = new Grab(this.gameObject, grab_range,grab_pull_power,grab_hlabysh_power);
        grab.holdAvalible = grab_hold_avalible;
        grab.pullAvalible = grab_pull_avalible;
        grab.hlabyshAvalible = grab_hlabysh_avalible;

        // RigidbodyInterpolation2D a = new RigidbodyInterpolation2D();
        // var a =body.interpolation;
    }

    // Update is called once per frame
    void Update()
    {

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

            if (horizontal != 0)
            {
                Climp(horizontal);
                HangOn(horizontal);
            }
            
        }
        else
        {
            if (Input.GetAxis("Vertical") != 0)
            {
                float vertical = Input.GetAxis("Vertical");
                if (vertical < 0)
                {
                    set_free();
                }
            }
            else
            {
                RaycastHit2D HangOnRight = Physics2D.Raycast(body.position + new Vector2(0.55f, 0.8f), Vector2.right, 0.1f);
                RaycastHit2D HangOnLeft = Physics2D.Raycast(body.position + new Vector2(-0.55f, 0.8f), Vector2.left, 0.1f);
                if (HangOnRight.collider == null && HangOnLeft.collider==null)
                {
                    set_free();
                }
            }
        }
        /* Обработка нажатия кнопки прыжка */
        if (Input.GetButtonDown("Jump") && grounded)
        {
            horizontal = Input.GetAxis("Horizontal");
            //body.AddForce(); // -В чём сила брат? -В ньютанах.
            Debug.Log(horizontal);
            body.AddForce(new Vector2(0f, force));
            set_free();
        }

        if (Input.GetMouseButtonUp(1))
        {
            if(grab.holding) //Вернуть  && !grab.crutch для второго клика для сброса захвата
            {
                grab.Stop();
            }
            grab.crutch = false;
            grab.pullOnCoolDown = false;
        }
        if(Input.GetMouseButtonDown(0))
        {
                grab.Hlabysh();
        }
    }

    private void Climp(float horizontal)
    {
        horizontal=(float)Math.Round(horizontal);
        RaycastHit2D climpBottom = Physics2D.Raycast(body.position + new Vector2(0.55f*horizontal, -0.9f), new Vector2(horizontal,0), 0.1f);
        RaycastHit2D climptop = Physics2D.Raycast(body.position + new Vector2(0.55f * horizontal, -0.7f), new Vector2(horizontal, 0), 0.1f);
        if (climpBottom.collider != null && climptop.collider == null)
        {
            body.position = climpBottom.collider.ClosestPoint(body.position + new Vector2(0.55f * horizontal, -0.7f)) + new Vector2(0.5f * horizontal*-1, 1f);
            body.velocity = new Vector2(body.velocity.x, 0);
        }
    }

    private void HangOn(float horizontal)
    {
        horizontal = (float)Math.Round(horizontal);
        RaycastHit2D HangOnBottom = Physics2D.Raycast(body.position + new Vector2(0.55f* horizontal, 0.7f), new Vector2(horizontal, 0), 0.1f);
        RaycastHit2D HangOnTop = Physics2D.Raycast(body.position + new Vector2(0.55f* horizontal, 0.9f), new Vector2(horizontal, 0), 0.1f);
        if (HangOnBottom.collider != null && HangOnTop.collider == null && body.velocity.y <= 0)
        {
            grounded = true;
            hangedOn = true;
            Vector2 cornerTop = new Vector2(0.5f*horizontal, 0.9f);
            body.position = HangOnBottom.collider.ClosestPoint(body.position + cornerTop) - cornerTop;
            body.isKinematic = true;
            body.velocity = new Vector2(0, 0);
        }
    }

    private void set_free()
    {
        body.isKinematic = false;
        grounded = false;
        hangedOn = false;
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButton(1)) //Этот метод вызывается каждый кадр, если зажата ПКМ, не путать с Input.GetMouseButtonDown
        {
                RaycastHit2D hit = Physics2D.Raycast(body.position, Util.AimDIr(),1000f);
                Debug.DrawRay(body.position, Util.AimDIr()); //DrawRay не умеет в длину луча, действительный луч имеет длину в 1000f

                if (hit.collider != null)
                {
                Debug.Log(hit.collider.name);
                    if (!grab.holding)
                    {
                        if (hit.collider.name != "Tilemap")
                        {
                            grab.Pull(hit.collider.gameObject);
                        }
                        else
                        {
                        //grab.PullReverse(hit.point);
                        }
                    }
                }
        }
        if (grab.holding)
        {
            grab.Hold();
        }


        if (!hangedOn)
        body.velocity = new Vector2(horizontal*speed, body.velocity.y);
       // Debug.Log(body.velocity);
        if(telekines.working)
        {
            telekines.Work();
        }

    }

}
