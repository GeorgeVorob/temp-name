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
        private Rigidbody2D body;
        private Vector3 prevForce = new Vector3(0f, 0f, 0f);
        public Boolean working { get; set; }
        public Boolean avalible { get; set; }
        public Telekinesis()
        {
            working = false;
            avalible = false;
        }
        public void Start(Rigidbody2D body)
        {
            working = true;
            this.body = body;
        }
        public void Work()
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //позиция курсора
            var mouseDir = mousePos - body.transform.position; //направление от объекта до курсора
            mouseDir.z = 0.0f; //TODO: перевести этот ужас в Vector2
            if (mouseDir.magnitude > 5f) //если расстояние между курсором и объектом слишком велико (а вместе с ним и скорость объекта, то всё сбрасывается
            {
                working = false;
                body.gravityScale = 1;
                prevForce.Set(0f, 0f, 0f);
            }
            body.AddForce((mouseDir.normalized * 20000f * mouseDir.magnitude) - prevForce * 0.92f); //собственно, движение объекта
            prevForce = mouseDir.normalized * 20000f * mouseDir.magnitude;
            Debug.Log(prevForce);
        }
        public void Stop()
        {
            working = false;
        }
    }
}
