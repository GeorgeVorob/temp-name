using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainCharacter : MonoBehaviour
{
    Rigidbody2D body;
    float horizontal;// Переменная перемещения по оси х
    public float speed;// Переменная скорости перемещения
    public bool grounded=false;// Переиенная состояния "на земле"
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal= Input.GetAxis("Horizontal"); 
        /* Обработка нажатия кнопки прыжка */
        if (Input.GetButtonDown("Jump") && grounded)
        {
            body.AddForce(new Vector2(0f, 500f)); // -В чём сила брат? -В ньютанах. 
            grounded = false;
        }

    }

    void FixedUpdate()
    {
        Vector2 position = body.position;
        Vector2 move = new Vector2(horizontal, 0);
        position += move * speed * Time.deltaTime;
        body.position = position;
    }

}
