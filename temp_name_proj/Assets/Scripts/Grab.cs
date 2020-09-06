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
        public float range = 2.2f;
        public float power = 150000f;
        public bool crutch = true;
        public bool pullOnCoolDown = false;
        private bool tryingToUnhold = false;

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
        public void Start(GameObject grabbingObject)
        {
            crutch = true;
            grabbingObject.layer = 8;
            var newbody = grabbingObject.GetComponent<Rigidbody2D>();
                holding = true;
                this.body = newbody;
                this.grabbingObject = grabbingObject;
                body.gravityScale = 0;
        }
        public void Hold()
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
            Vector2 force = (Dir.normalized * 30000f * Dir.magnitude) - prevForce * 0.92f;
            prevForce = Dir.normalized * 30000f * Dir.magnitude;
            body.AddForce(force);
            if(tryingToUnhold)
            {
                Stop();
            }
        }
        public void Pull(GameObject grabbingObject) //притягивает объект перед захватом
        {
            if (!pullOnCoolDown)
            {
                Rigidbody2D body = grabbingObject.GetComponent<Rigidbody2D>();
                Vector2 dir = ownerBody.position - body.position;
                dir.y += 1f;
                if (dir.magnitude <= range * 1.2)
                {
                    this.body = body;
                   // body.isKinematic = true;
                    body.velocity = Vector2.zero;  //Возможно неадекватное поведение физики из-за body.isKinematic?
                  //  body.isKinematic = false;
                    this.Start(grabbingObject);
                    Debug.Log("Holding");
                    return;
                }
                body.AddForce(dir * power / (float)Math.Pow(dir.magnitude, 3));
            }
        }
        //public void PullReverse(Vector2 point) //Притягивание к стене
        //{
        //    Vector2 dir = point - ownerBody.position;
        //    ownerBody.AddForce(dir * power/20 / (float)Math.Pow(dir.magnitude, 1));
        //}
        public void Hlabysh()
        {
            Rigidbody2D throwingbody;
            if (holding)
            {
                 throwingbody = this.body;
                pullOnCoolDown = true;
            }
            else 
            {
                RaycastHit2D hit = Physics2D.Raycast(ownerBody.position, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - ownerBody.position, range * 2.5f);
                Debug.DrawRay(ownerBody.position, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - ownerBody.position, Color.red, 0.8f);
                if (hit.collider != null)
                {
                    throwingbody = hit.collider.gameObject.GetComponent<Rigidbody2D>();
                }
                else return;
            }
            Vector2 dir = throwingbody.position - ownerBody.position;
            if (dir.magnitude <= range*2) 
            {
                holding = false;
                throwingbody.AddForce(dir*900f,ForceMode2D.Impulse);
                if(throwingbody = this.body)
                {
                    this.Stop();
                }
            }
        }
        public void Stop()
        {
            tryingToUnhold = true;
            ContactFilter2D c = new ContactFilter2D();
            Collider2D[] l = new Collider2D[1];
            int i = body.OverlapCollider(c, l);
            if (i != 0) return;
            grabbingObject.layer = 0;
            holding = false;
            body.gravityScale = 1;
            prevForce.Set(0f, 0f);
            Debug.Log("Not holding");
            tryingToUnhold = false;
        }
    }
}
