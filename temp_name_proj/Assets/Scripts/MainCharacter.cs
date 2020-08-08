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

    Vector3 prevForce = new Vector3(0f, 0f, 0f);
    private bool grabbing = false;
    GameObject lastClicked; //последний GameObject, на который нажал игрок (при отключенном захвате)

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
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
            if (grabbing)
            {
                grabbing = false;
            }
            else
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero); 

                if ((hit.collider != null) && (grabbing == false))
                {
                    lastClicked = hit.collider.gameObject;
                    grabbing = true;
                }
            }
        }
        if(grabbing) //полет Rigidbody2D за курсором
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //позиция курсора
            var mouseDir = mousePos - lastClicked.transform.position; //направление от объекта до курсора
            mouseDir.z = 0.0f; //TODO: перевести этот ужас в Vector2
            if (mouseDir.magnitude > 5f) //если расстояние между курсором и объектом слишком велико (а вместе с ним и скорость объекта, то всё сбрасывается
            {
                grabbing = false;
                lastClicked.GetComponent<Rigidbody2D>().gravityScale = 1;
                prevForce.Set(0f, 0f, 0f);
            }
            lastClicked.GetComponent<Rigidbody2D>().AddForce((mouseDir.normalized * 3000f * mouseDir.magnitude) - prevForce * 0.92f); //собственно, движение объекта
            prevForce = mouseDir.normalized * 3000f * mouseDir.magnitude;
        }

    }

    void FixedUpdate()
    {
        
        Vector2 position = body.position;
        //Vector2 move = new Vector2(, 0);
        //position.x += horizontal * speed * Time.deltaTime;
        // body.position = position;
        body.velocity = new Vector2(horizontal*speed, body.velocity.y);
        Debug.Log(body.velocity);
        
    }

}
