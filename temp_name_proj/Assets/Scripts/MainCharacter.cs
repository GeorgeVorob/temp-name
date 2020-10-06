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
    public float maxHP = 100f;
    private float HP;
    public float invictibleTime = 0.5f;

    bool isInvincible;
    float invincibleTimer;

    public float jump_delay = 0.05f;
    public bool grounded = false;// Переиенная состояния "на земле"
    public bool hangedOn = false;
    public bool wall = false;
    public bool is_jumped = false;

    private float jump_delay_timer;
    private Vector2 bottom_middle_point;
    private Vector2 top_middle_point;
    private Vector2 step_middle_point;
    private Vector2 hang_middle_point;

    private bool buf = false;
    private bool buf1 = false;

    private Collider2D hang_object;

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
        step_middle_point = transform.localScale * new Vector2(0, -0.75f);
        hang_middle_point = transform.localScale * new Vector2(0, 0.8f);

        telekines.avalible = true;
        grab = new Grab(this.gameObject, grab_range, grab_pull_power, grab_hlabysh_power);
        grab.holdAvalible = grab_hold_avalible;
        grab.pullAvalible = grab_pull_avalible;
        grab.hlabyshAvalible = grab_hlabysh_avalible;
        HP = maxHP;
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

        if (Input.GetButtonDown("Jump") && grounded && !is_jumped)
        {
            set_free();
            body.AddForce(new Vector2(0f, force));
            is_jumped = true;
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

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButton(1)) //Этот метод вызывается каждый кадр, если зажата ПКМ, не путать с Input.GetMouseButtonDown
        {
            RaycastHit2D hit = Physics2D.Raycast(body.position, Util.AimDIr(), 1000f,Util.LayerPhysObjectsOnly());
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
        if (!hangedOn)
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
            HangOn(horizontal, top_checker, collision);
        }
        if (hang_object != null && collision.collider==hang_object)
        {
            if (buf) buf1 = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //Debug.Log(bottom_middle_point);
        if (collision.collider == hang_object && (!buf||!buf1))
        {
             set_free();
             hang_object = null;
        }
        if (buf) buf = false;
    }

    private void Climp(float horizontal, Vector2 central_checker)
    {
        horizontal = Math.Sign(horizontal);
        if (central_checker.y > (bottom_middle_point + (Vector2)this.transform.position).y
            && central_checker.y < (step_middle_point + (Vector2)this.transform.position).y
            && Math.Sign(central_checker.x - body.position.x) == horizontal)
        {
            Debug.Log(body.position.x + " // " + central_checker.x);
            body.position = central_checker + new Vector2(horizontal * -0.5f, 1f);
            Debug.Log(body.position.x + "//");
            body.velocity = new Vector2(body.velocity.x, 0);
        }
    }

    private void HangOn(float horizontal, Vector2 top_checker, Collision2D collision)
    {
        horizontal = Math.Sign(horizontal);
        //RaycastHit2D HangOnBottom = Physics2D.Raycast(body.position + new Vector2(0.55f * horizontal, 0.7f), new Vector2(horizontal, 0), 0.1f);
        //RaycastHit2D HangOnTop = Physics2D.Raycast(body.position + new Vector2(0.55f * horizontal, 0.9f), new Vector2(horizontal, 0), 0.1f);
        if (Math.Round(top_checker.y, 1) < Math.Round((top_middle_point + (Vector2)this.transform.position).y, 1)
            && top_checker.y > (hang_middle_point + (Vector2)this.transform.position).y
            && body.velocity.y <= 0
            && (Math.Sign(top_checker.x - this.transform.position.x) == horizontal))
        {
            Debug.Log(top_checker.x + "/*/" + (top_middle_point + (Vector2)this.transform.position).x);
            grounded = true;
            hangedOn = true;
            Vector2 cornerTop = new Vector2(0.5f * horizontal, 1f)*transform.localScale;
            top_checker.x = (float)Math.Round(top_checker.x, 1);
            top_checker.y = (float)Math.Round(top_checker.y, 1);
            body.position = top_checker - cornerTop;
            body.isKinematic = true;
            body.velocity = new Vector2(0, 0);
            hang_object = collision.collider;
            buf = true;
        }
    }

    private void set_free()
    {
        body.isKinematic = false;
        grounded = false;
        hangedOn = false;
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
            Util.GameOver();
        }
    }

}
