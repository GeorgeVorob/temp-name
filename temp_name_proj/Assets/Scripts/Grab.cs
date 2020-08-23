using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class Grab
    {
        public float range = 3.0f;

        Vector2 mousePos;
        Vector2 Dir;
        private Rigidbody2D body;
        private Rigidbody2D ownerBody; //Тот, кто использует захват. Определяется в конструкторе класса.
        private GameObject owner;
        private GameObject grabbingObject;
        private Vector2 prevForce = new Vector2(0f, 0f);
        public Boolean working { get; set; }
        public Boolean avalible { get; set; }
        public Grab(GameObject owner)
        {
            working = false;
            avalible = false;
            this.owner = owner;
            this.ownerBody = owner.GetComponent<Rigidbody2D>();
        }
        public bool Start(GameObject grabbingObject)
        {
            var newbody = grabbingObject.GetComponent<Rigidbody2D>();
            Vector2 distance = newbody.transform.position - ownerBody.transform.position;
            if ((distance.magnitude<=range*1.3)&&(grabbingObject.GetComponent<Rigidbody2D>() != ownerBody))
            {
                working = true;
                this.body = newbody;
                this.grabbingObject = grabbingObject;
                body.gravityScale = 0;
                return true;
            }
            return false;
        }
        public void Work()
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //позиция курсора
            Vector2 point = mousePos - (Vector2)ownerBody.transform.position;
            point = point.normalized * range;
            Dir = ((Vector2)ownerBody.transform.position + point) - (Vector2)body.transform.position;

            //RaycastHit2D hit = Physics2D.Raycast(ownerBody.position+point, body.position,Mathf.Infinity, LayerMask.GetMask("Default"));
            //Debug.DrawLine(ownerBody.transform.position, hit.transform.position, Color.white, 0.3f);
            //Debug.Log(hit.collider);

            if (Dir.magnitude > range*2) //TODO: добавить проверку на наличие перпятствия между игроком и объектом
            {
                    this.Stop();
                    return;
            }
            body.AddForce((Dir.normalized * 30000f * Dir.magnitude) - prevForce * 0.92f); //собственно, движение объекта
            prevForce = Dir.normalized * 30000f * Dir.magnitude;
        }
        public void Stop()
        {
            working = false;
            body.gravityScale = 1;
            prevForce.Set(0f, 0f);
        }
    }
}
