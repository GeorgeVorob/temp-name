using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class Telekinesis
    {
        Vector2 mousePos;
        Vector2 mouseDir;
        private Rigidbody2D body;
        private Vector2 prevForce = new Vector2(0f, 0f);
        public Boolean working { get; set; }
        public Boolean avalible { get; set; }
        public Telekinesis()
        {
            working = false;
            avalible = false;
        }
        public void Start(GameObject grabbongObject)
        {
            if (grabbongObject.name!="MainCharacter")
            {
                working = true;
                this.body = grabbongObject.GetComponent<Rigidbody2D>();
            }
        }
        public void Work()
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //позиция курсора
            mouseDir = mousePos - (Vector2)body.transform.position; //направление от объекта до курсора
            if (mouseDir.magnitude > 5f) //если расстояние между курсором и объектом слишком велико (а вместе с ним и скорость объекта, то всё сбрасывается
            {
                this.Stop();
                return;
            }
            body.AddForce((mouseDir.normalized * 20000f * mouseDir.magnitude) - prevForce * 0.92f); //собственно, движение объекта
            prevForce = mouseDir.normalized * 20000f * mouseDir.magnitude;
            Debug.Log(prevForce);
        }
        public void Stop()
        {
            working = false;
            body.gravityScale = 1;
            prevForce.Set(0f, 0f);
        }
    }
}
