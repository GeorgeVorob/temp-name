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
        public float pullPower = 100f;  //Эти предустановленные значения не имеют смысла из-за конструктора, скорее просто настройки по умолчанию
        public float hlabyshPower = 900f;
        public bool crutch = true;
        public bool pullOnCoolDown = false;
        private bool tryingToUnhold = false;
        private bool tryingToGetOut = false;

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
        public Grab(GameObject owner, float range, float pull_power,float hlabysh_power)
        {
            this.owner = owner;
            this.ownerBody = owner.GetComponent<Rigidbody2D>();
            ownercollider = owner.GetComponent<BoxCollider2D>();
            this.range = range;
            pullPower = pull_power;
            hlabyshPower = hlabysh_power;
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
            Vector2 point = Util.AimDIr() * range;
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

            //Collider2D[] bodyCollider = new Collider2D[1];
            //grabbingbody.GetAttachedColliders(bodyCollider);
            //if ((owner.transform.position.y + 0.1f >= grabbingObject.transform.position.y + grabbingcollider.size.y / 2.0f + ownercollider.size.y / 2.0f) && (Dir.y <= 0.0f))
            //{
            //    grabbingObject.layer = 0;
            //}
            //else
            //{
            //    grabbingObject.layer = 8;
            //}
            //Debug.Log($"DIFFERENCE:{owner.transform.position.y + 1.0f - (grabbingObject.transform.position.y + grabbingcollider.size.y / 2.0f + ownercollider.size.y / 2.0f)}");

            hit = Physics2D.Raycast(grabbingbody.position,Dir,1000f,~2);

            if(hit.collider!=null && (Math.Abs(Dir.x) >= ownercollider.size.x + grabbingcollider.size.x || Math.Abs(Dir.y) >= ownercollider.size.y + grabbingcollider.size.y || Dir.magnitude >= range))
            {
                grabbingObject.layer = 8;
            }
            else
            {
                tryingToGetOut = true;
            }
            Vector2 force = (Dir.normalized * 30000f * Dir.magnitude) - prevForce * 0.92f;
            prevForce = Dir.normalized * 30000f * Dir.magnitude;
            grabbingbody.AddForce(force);
            Debug.DrawRay(grabbingbody.position,Dir,Color.cyan);
            if(tryingToGetOut)
            {
                if (!IsGrabbingObjectInsideCharacter()) grabbingObject.layer = 0;
                tryingToGetOut = false;
            }
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
                Debug.DrawLine(ownerBody.position, body.position,Color.green);
                dir.y += 1f;
                //Debug.Log($"magnitude: {{dir.magnitude}}");
                if (dir.magnitude <= range * 1.5 && holdAvalible) //ЕСЛИ ПЕРСОНАЖ ДВИГАЕТ САМ СЕБЯ И PULL НЕ ПЕРЕХОДИТ В HOLD ТО ЭТО ИЗ-ЗА ЭТОГО
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
                    body.AddForce(dir * (pullPower * 1000f) / (float)Math.Pow(dir.magnitude, 3));
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
                    if (IsGrabbingObjectInsideCharacter()) return;
                    throwingbody = this.grabbingbody;
                    pullOnCoolDown = true;
                }
                else
                {
                    RaycastHit2D hit = Physics2D.Raycast(ownerBody.position,Util.AimDIr(), range * 2.5f);
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
                    throwingbody.AddForce(dir * hlabyshPower, ForceMode2D.Impulse);
                    if (throwingbody == this.grabbingbody)
                    {
                        this.Stop(true);
                    }
                }
            }
        }
        public void Stop(bool force = false)
        {
            if (!force)
            {
                tryingToUnhold = true;
                if (IsGrabbingObjectInsideCharacter()) return;
            }
            grabbingObject.layer = 0;
            holding = false;
            grabbingbody.gravityScale = 1;
            prevForce.Set(0f, 0f);
            Debug.Log("Not holding");
            tryingToUnhold = false;
        }
        private bool IsGrabbingObjectInsideCharacter()
        {
            ContactFilter2D c = new ContactFilter2D();
            List<Collider2D> l = new List<Collider2D>();
            int i = grabbingbody.OverlapCollider(c, l);
            return l.Contains(ownercollider);
        }
    }
}
