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
        public float range = 2.7f;
        public float power = 150000f;
        public bool crutch = true;

        Vector2 mousePos;
        Vector2 Dir;
        public Rigidbody2D body;
        private Rigidbody2D ownerBody; //Тот, кто использует захват. Определяется в конструкторе класса.
        private GameObject owner;
        private GameObject grabbingObject;
        private Vector2 prevForce = new Vector2(0f, 0f);
        public Boolean holding { get; set; }
        public Boolean avalible { get; set; }
        public Grab(GameObject owner)
        {
            avalible = false;
            this.owner = owner;
            this.ownerBody = owner.GetComponent<Rigidbody2D>();
        }
        public bool Start(GameObject grabbingObject)
        {
            crutch = true;
            grabbingObject.layer = 2;
            var newbody = grabbingObject.GetComponent<Rigidbody2D>();
            Vector2 distance = newbody.transform.position - ownerBody.transform.position;
            if ((distance.magnitude<=range*1.3)&&(grabbingObject.GetComponent<Rigidbody2D>() != ownerBody))
            {
                holding = true;
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
            Dir = (Vector2)ownerBody.transform.position + point - (Vector2)body.transform.position;
            Vector2 owner_body = body.position - ownerBody.position;
            RaycastHit2D hit = Physics2D.Raycast(ownerBody.position, owner_body,owner_body.magnitude);
            Debug.Log(hit.collider);

            if ((hit.collider!=null&&hit.collider.name=="Tilemap")||(Dir.magnitude > range*2))
            {
                    this.Stop();
                    return;
            }
            body.AddForce((Dir.normalized * 30000f * Dir.magnitude) - prevForce * 0.92f);
            //Debug.Log("Da");
            prevForce = Dir.normalized * 30000f * Dir.magnitude;
        }
        public void Pull(GameObject grabbingObject) //притягивает объект перед захватом
        {
            Rigidbody2D body = grabbingObject.GetComponent<Rigidbody2D>();
            Vector2 dir = ownerBody.position - body.position;
            dir.y += 1f;
            body.AddForce(dir * power/(float)Math.Pow(dir.magnitude,3));
            if(dir.magnitude<=range)
            {
                this.body = body;
                body.velocity = Vector2.zero;
                this.Start(grabbingObject);
                Debug.Log("Holding");
            }
        }
        //public void PullReverse(Vector2 point) //Притягивание к стене
        //{
        //    Vector2 dir = point - ownerBody.position;
        //    ownerBody.AddForce(dir * power/20 / (float)Math.Pow(dir.magnitude, 1));
        //}
        public void Hlabysh(Rigidbody2D body)
        {
            Vector2 dir = body.position - ownerBody.position;
            if (dir.magnitude <= range*2) 
            {
                holding = false;
                body.AddForce(dir*900f,ForceMode2D.Impulse);
                if(body = this.body)
                {
                    this.Stop();
                }
            }
        }
        public void Stop()
        {
            grabbingObject.layer = 0;
            holding = false;
            body.gravityScale = 1;
            prevForce.Set(0f, 0f);
            Debug.Log("Not olding");
        }
    }
}
