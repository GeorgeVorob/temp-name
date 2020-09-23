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
    public float jump_delay = 0.5f;
    public bool grounded = false;// Переиенная состояния "на земле"
    public bool hangedOn = false;
    public bool wall = false;
    public bool is_jumped = false;

    private float jump_delay_timer;
    private Vector2 bottom_middle_point;
    private Vector2 top_middle_point;
    private Vector2 step_middle_point;
    private Vector2 hang_middle_point;

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

        jump_delay_timer = jump_delay;
        bottom_middle_point = transform.localScale * new Vector2(0, -1f);
        top_middle_point = transform.localScale * new Vector2(0, 1f);
        step_middle_point = transform.localScale * new Vector2(0, -0.8f);
        hang_middle_point = transform.localScale * new Vector2(0, 0.8f);

        telekines.avalible = true;
        grab = new Grab(this.gameObject, grab_range, grab_pull_power, grab_hlabysh_power);
        grab.holdAvalible = grab_hold_avalible;
        grab.pullAvalible = grab_pull_avalible;
        grab.hlabyshAvalible = grab_hlabysh_avalible;
    }

    // Update is called once per frame
    void Update()
    {
        if (is_jumped)
        {
            jump_delay_timer -= Time.deltaTime;
            if (jump_delay_timer <= 0)
            {
                is_jumped = false;
                jump_delay_timer = jump_delay;
            }
        }

        if (hangedOn)
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
                if (HangOnRight.collider == null && HangOnLeft.collider == null)
                {
                    set_free();
                }
            }
        }
        /* Обработка нажатия кнопки прыжка */
        if (Input.GetButtonDown("Jump") && grounded && !is_jumped)
        {
            body.AddForce(new Vector2(0f, force));
            is_jumped = true;
            set_free();
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (grab.holding) //Вернуть  && !grab.crutch для второго клика для сброса захвата
            {
                grab.Stop();
            }
            grab.crutch = false;
            grab.pullOnCoolDown = false;
        }
        if (Input.GetMouseButtonDown(0))
        {
            grab.Hlabysh();
        }
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButton(1)) //Этот метод вызывается каждый кадр, если зажата ПКМ, не путать с Input.GetMouseButtonDown
        {
            RaycastHit2D hit = Physics2D.Raycast(body.position, Util.AimDIr(), 1000f);
            Debug.DrawRay(body.position, Util.AimDIr()); //DrawRay не умеет в длину луча, действительный луч имеет длину в 1000f

            if (hit.collider != null)
            {
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

        horizontal = Input.GetAxis("Horizontal");
        if(!hangedOn)
        body.velocity = new Vector2(horizontal * speed, body.velocity.y);
        if (telekines.working)
        {
            telekines.Work();
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Vector2 Bottom_checker = collision.collider.ClosestPoint((Vector2)this.transform.position + (bottom_middle_point) + new Vector2(0, 0.1f));
        //Debug.Log((Vector2)this.transform.position + (bottom_middle_point) + "///" + Bottom_checker);
        if ((Bottom_checker.y < ((Vector2)transform.position + bottom_middle_point).y) && !is_jumped) grounded = true;

        if (horizontal != 0)
        {
            Vector2 central_checker = collision.collider.ClosestPoint(transform.position);
            Vector2 top_checker = collision.collider.ClosestPoint((Vector2)transform.position + top_middle_point);
            Climp(horizontal, central_checker);
            //HangOn(horizontal, top_checker);
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //Debug.Log(bottom_middle_point);
        Vector2 bottom_checker = collision.collider.ClosestPoint((Vector2)this.transform.position + (bottom_middle_point));
        if (bottom_checker.y < ((Vector2)this.transform.position + bottom_middle_point).y) grounded = false;
    }

    private void Climp(float horizontal, Vector2 central_checker)
    {
        horizontal = (float)Math.Round(horizontal);
        if (central_checker.y > (bottom_middle_point + (Vector2)this.transform.position).y 
            && central_checker.y< (step_middle_point + (Vector2)this.transform.position).y)
        {
            body.position = central_checker + new Vector2(0.5f * horizontal * -1, 1f);
            body.velocity = new Vector2(body.velocity.x, 0);
        }
    }

    private void HangOn(float horizontal, Vector2 top_checker)
    {
        horizontal = (float)Math.Round(horizontal);
        //RaycastHit2D HangOnBottom = Physics2D.Raycast(body.position + new Vector2(0.55f * horizontal, 0.7f), new Vector2(horizontal, 0), 0.1f);
        //RaycastHit2D HangOnTop = Physics2D.Raycast(body.position + new Vector2(0.55f * horizontal, 0.9f), new Vector2(horizontal, 0), 0.1f);
        if (top_checker.y < (top_middle_point + (Vector2)this.transform.position).y 
            && top_checker.y > (hang_middle_point + (Vector2)this.transform.position).y 
            && body.velocity.y <= 0)
        {
            grounded = true;
            hangedOn = true;
            Vector2 cornerTop = new Vector2(0.5f * horizontal, 0.9f);
            body.position = top_checker - cornerTop;
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

}
