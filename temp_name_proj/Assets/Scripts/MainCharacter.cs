using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainCharacter : MonoBehaviour
{
    Rigidbody2D body;
    float horizontal;// Переменная перемещения по оси х
    public float speed;// Переменная скорости перемещения
    public float force;
    public bool grounded=false;// Переиенная состояния "на земле"

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
            body.AddForceAtPosition(new Vector2(0f, force), body.position);
            grounded = false;
        }
        horizontal= Input.GetAxis("Horizontal");
        /* Обработка нажатия кнопки прыжка */

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if ((hit.collider != null))
            {
                lastClicked = hit.collider.gameObject;
            }

            if (telekines.avalible)
            {
                if(!telekines.working)
                {
                    telekines.Start(lastClicked.GetComponent<Rigidbody2D>());
                }
                else
                {
                    telekines.Stop();
                }
            }
        }

    }

    void FixedUpdate()
    {
        
        Vector2 position = body.position;
        //Vector2 move = new Vector2(, 0);
        //position.x += horizontal * speed * Time.deltaTime;
        // body.position = position;
        body.velocity = new Vector2(horizontal*speed, body.velocity.y);
       // Debug.Log(body.velocity);
        if(telekines.working)
        {
            telekines.Work();
        }
        
    }

}
