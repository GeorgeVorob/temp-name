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
    float horizontal = 0;// Переменная перемещения по оси х
    float vertical = 0;
    float speed;
    public float defualt_speed;
    public float sprint_speed;
    public float walk_speed;// Переменная скорости перемещения
    public float force;

    public float jump_delay = 0.05f;
    public bool grounded = false;// Переиенная состояния "на земле"
    public bool hangedOn = false;
    public bool wall = false;
    public bool is_jumped = false;

    private float jump_delay_timer;
    private Vector2 bottom_middle_point;
    private Vector2 top_middle_point;

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

        speed = defualt_speed;

        telekines.avalible = true;
        grab = new Grab(this.gameObject, grab_range, grab_pull_power, grab_hlabysh_power);
        grab.holdAvalible = grab_hold_avalible;
        grab.pullAvalible = grab_pull_avalible;
        grab.hlabyshAvalible = grab_hlabysh_avalible;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Vertical"))
        {
            if (Input.GetButton("Jump"))
            {
                gameObject.layer = 14;
            }
        }
        else
        {
            if (Input.GetButtonDown("Jump") && grounded && !is_jumped)
            {
                set_free();
                body.AddForce(new Vector2(0f, force));
                is_jumped = true;
            }
        }

        if (Input.GetButtonUp("Vertical") || Input.GetButtonUp("Jump"))
        {
            gameObject.layer = 10;
        }

        if (is_jumped)
        {
            jump_delay_timer -= Time.deltaTime;
            if (jump_delay_timer <= 0)
            {
                is_jumped = false;
                jump_delay_timer = jump_delay;
            }
        }

        if (Input.GetButton("Sprint") && grounded)
        {
            speed = sprint_speed;
        }

        if (Input.GetButtonUp("Sprint") && grounded)
        {
            speed = defualt_speed;
        }

        if (Input.GetButton("Walk") && grounded)
        {
            speed = walk_speed;
        }

        if (Input.GetButtonUp("Walk") && grounded)
        {
            speed = defualt_speed;
        }

        if (hangedOn)
        {
            if (vertical < 0)
            {
                set_free();
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
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButton(1)) //Этот метод вызывается каждый кадр, если зажата ПКМ, не путать с Input.GetMouseButtonDown
        {
            RaycastHit2D hit = Physics2D.Raycast(body.position, Util.AimDIr(), 1000f, Util.LayerAllPhysObjects());
            Debug.DrawRay(body.position, Util.AimDIr()); //DrawRay не умеет в длину луча, действительный луч имеет длину в 1000f

            if (hit.collider != null && !Util.IsInLayerMask(hit.collider.gameObject.layer, Util.LayerStaticPhysObjectsOnly()))
            {
                if (!grab.holding)
                {

                    grab.Pull(hit.collider.gameObject);

                }
            }
        }
        if (grab.holding)
        {
            grab.Hold();
        }

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (horizontal != 0)
        {
            climp(horizontal);
            if (body.velocity.y < 0)
                hangOn(horizontal);
        }

        if (!hangedOn) body.velocity = new Vector2(horizontal * speed, body.velocity.y);

        if (telekines.working)
        {
            telekines.Work();
        }

    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        Vector2 Bottom_checker = collision.collider.ClosestPoint((Vector2)this.transform.position + (bottom_middle_point) + new Vector2(0, 0.1f));
        if ((Bottom_checker.y < ((Vector2)transform.position + bottom_middle_point).y) && !is_jumped)
        {
            grounded = true;
            if (!Input.GetButton("Sprint") || !Input.GetButtonUp("Walk")) speed = defualt_speed;
        }
        if (collision.collider == hang_object && !wall) wall = true;

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log(collision.collider + "//" + hang_object);
        if (collision.collider == hang_object)
        {
            if (wall)
            {
                set_free();
                hang_object = null;
                wall = false;
            }
            else wall = true;
        }
    }

    private void climp(float horizontal)
    {
        horizontal = Math.Sign(horizontal);
        RaycastHit2D low_climp_hit = Physics2D.Raycast((Vector2)transform.position + new Vector2(horizontal * 0.5f, -1f) * transform.localScale, new Vector2(horizontal, 0), 0.05f, 1);
        if (low_climp_hit)
        {
            Vector2 top_climp_point = (Vector2)transform.position + new Vector2(horizontal * 0.5f, -0.75f) * transform.localScale;
            RaycastHit2D top_climp_hit = Physics2D.Raycast(top_climp_point, new Vector2(horizontal, 0), 0.05f, 1);
            if (!top_climp_hit)
            {
                body.position = low_climp_hit.collider.ClosestPoint(top_climp_point) + new Vector2(-horizontal * 0.5f, 1f) * transform.localScale;
            }
        }
    }

    private void hangOn(float horizontal)
    {
        horizontal = Math.Sign(horizontal);

        RaycastHit2D low_hang_hit = Physics2D.Raycast((Vector2)transform.position + new Vector2(horizontal * 0.5f, 0.7f) * transform.localScale, new Vector2(horizontal, 0), 0.15f, Util.LayerMaterialObjectsOnly());
        if (low_hang_hit)
        {
            Vector2 top_point = (Vector2)transform.position + new Vector2(horizontal * 0.5f, 1f) * transform.localScale;
            RaycastHit2D high_hang_hit = Physics2D.Raycast(top_point, new Vector2(horizontal, 0), 0.15f, Util.LayerMaterialObjectsOnly());
            if (!high_hang_hit)
            {
                Vector2 top_checker = low_hang_hit.collider.ClosestPoint((Vector2)transform.position + top_middle_point);
                Vector2 cornerTop = new Vector2(0.5f * horizontal, 1f) * transform.localScale;
                top_checker.x = (float)Math.Round(top_checker.x, 1);
                top_checker.y = (float)Math.Round(top_checker.y, 1);
                body.position = top_checker - cornerTop;
                body.isKinematic = true;
                body.velocity = new Vector2(0, 0);
                grounded = true;
                hangedOn = true;
                hang_object = low_hang_hit.collider;
            }
        }
    }

    private void set_free()
    {
        body.isKinematic = false;
        grounded = false;
        hangedOn = false;
    }
}