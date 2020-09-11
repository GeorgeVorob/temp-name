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
        public float range = 2.0f;
        public float power = 100f;
        public bool crutch = true;
        public bool pullOnCoolDown = false;
        private bool tryingToUnhold = false;

        public bool pullAvalible = true;
        public bool holdAvalible = true;
        public bool hlabyshAvalible = true;

        Vector2 mousePos;
        Vector2 Dir;
        public Rigidbody2D grabbingbody;
        private Rigidbody2D ownerBody; //Тот, кто использует захват. Определяется в конструкторе класса.
        private GameObject owner;
        private GameObject grabbingObject;
        private BoxCollider2D grabbingcollider;
        private BoxCollider2D ownercollider;
        private Vector2 prevForce = new Vector2(0f, 0f);
        public Boolean holding { get; set; }
        public Grab(GameObject owner, float range)
        {
            this.owner = owner;
            this.ownerBody = owner.GetComponent<Rigidbody2D>();
            ownercollider = owner.GetComponent<BoxCollider2D>();
            this.range = range;
        }
        public void Start(GameObject grabbingObject)
        {
            crutch = true;
            grabbingObject.layer = 8;
                holding = true;
                this.grabbingbody = grabbingObject.GetComponent<Rigidbody2D>();
            this.grabbingObject = grabbingObject;
            grabbingcollider = grabbingObject.GetComponent<BoxCollider2D>();
            grabbingbody.gravityScale = 0;
        }
        public void Hold()
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 point = mousePos - (Vector2)ownerBody.transform.position; //TODO: это тоже переделать под геймпады
            point = point.normalized * range;
            Dir = (Vector2)ownerBody.transform.position + point - (Vector2)grabbingbody.transform.position;
            Vector2 owner_body = grabbingbody.position - ownerBody.position;
            RaycastHit2D hit = Physics2D.Raycast(ownerBody.position, owner_body, owner_body.magnitude);
            //Debug.Log(hit.collider);
            //if (hit.collider != null && hit.collider.name == "Tilemap")
            //    Debug.Log("tilemap reason");
            //if (Dir.magnitude > range * 2.2)
            //    Debug.Log("range reason");

            if ((hit.collider != null && hit.collider.name == "Tilemap") || (Dir.magnitude > range * 3))
            {
                this.Stop();
                 return;
            }

            Collider2D[] bodyCollider = new Collider2D[1];
            grabbingbody.GetAttachedColliders(bodyCollider);
            if ((owner.transform.position.y + 0.1f >= grabbingObject.transform.position.y + grabbingcollider.size.y / 2.0f + ownercollider.size.y / 2.0f) && (Dir.y <= 0.0f))
            {
                grabbingObject.layer = 0;
            }
            else
            {
                grabbingObject.layer = 8;
            }
            //Debug.Log($"DIFFERENCE:{owner.transform.position.y + 1.0f - (grabbingObject.transform.position.y + grabbingcollider.size.y / 2.0f + ownercollider.size.y / 2.0f)}");
            Vector2 force = (Dir.normalized * 30000f * Dir.magnitude) - prevForce * 0.92f;
            prevForce = Dir.normalized * 30000f * Dir.magnitude;
            grabbingbody.AddForce(force);
            if (tryingToUnhold)
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
                if (dir.magnitude <= range * 1.2 && holdAvalible)
                {
                    this.grabbingbody = body;
                   // body.isKinematic = true;
                    body.velocity = Vector2.zero;  //Возможно неадекватное поведение физики из-за body.isKinematic?
                  //  body.isKinematic = false;
                    this.Start(grabbingObject);
                    Debug.Log("Holding");
                    return;
                }
                if (pullAvalible)
                {
                    body.AddForce(dir * (power * 1000f) / (float)Math.Pow(dir.magnitude, 3));
                }
            }
        }
        public void Hlabysh()
        {
            if (hlabyshAvalible)
            {
                Rigidbody2D throwingbody;
                if (holding)
                {
                    throwingbody = this.grabbingbody;
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
                if (dir.magnitude <= range * 2)
                {
                    holding = false;
                    throwingbody.AddForce(dir * 900f, ForceMode2D.Impulse);
                    if (throwingbody == this.grabbingbody)
                    {
                        this.Stop();
                    }
                }
            }
        }
        public void Stop()
        {
            tryingToUnhold = true;
            ContactFilter2D c = new ContactFilter2D();
            Collider2D[] l = new Collider2D[1];
            int i = grabbingbody.OverlapCollider(c, l);
            if (i != 0) return;
            grabbingObject.layer = 0;
            holding = false;
            grabbingbody.gravityScale = 1;
            prevForce.Set(0f, 0f);
            Debug.Log("Not holding");
            tryingToUnhold = false;
        }
    }
}
