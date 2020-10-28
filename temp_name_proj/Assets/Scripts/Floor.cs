using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        /*Проверка столкнавения с Главным Героем*/
        MainCharacter mainCharacter = other.gameObject.GetComponent<MainCharacter>();
        if (mainCharacter != null)
        {
            mainCharacter.grounded = true;
        }
    }
}
