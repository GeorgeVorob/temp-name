﻿using System;
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
        private int grabbingOldLayer;
        private float grabbingOldGravityscale;
        private PolygonCollider2D cone;

        private Vector2 prevForce = new Vector2(0f, 0f);
        public Boolean holding { get; set; }
        public Grab(GameObject owner, float range, float pull_power, float hlabysh_power)
        {
            this.owner = owner;
            this.ownerBody = owner.GetComponent<Rigidbody2D>();
            ownercollider = owner.GetComponent<BoxCollider2D>();
            this.range = range;
            pullPower = pull_power;
            hlabyshPower = hlabysh_power;
            cone = owner.AddComponent<PolygonCollider2D>();
            cone.isTrigger = true;
        }
        public void Start(GameObject grabbingObject)
        {
            grabbingOldGravityscale = grabbingObject.GetComponent<Rigidbody2D>().gravityScale;
            grabbingOldLayer = grabbingObject.layer;
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
            RaycastHit2D hit = Physics2D.Raycast(ownerBody.position, owner_body, owner_body.magnitude, Util.LayerStaticPhysObjectsOnly());
            //Debug.DrawRay(ownerBody.position, owner_body);
            if ((hit.collider != null && Util.IsInLayerMask(hit.collider.gameObject.layer,Util.LayerStaticPhysObjectsOnly())) || (Dir.magnitude > range * 3))
            {
                this.Stop();
                return;
            }

            hit = Physics2D.Raycast(grabbingbody.position, Dir, 1000f, ~2);

            if (hit.collider != null && (Math.Abs(Dir.x) >= ownercollider.size.x + grabbingcollider.size.x || Math.Abs(Dir.y) >= ownercollider.size.y + grabbingcollider.size.y || Dir.magnitude >= range))
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
            Debug.DrawRay(grabbingbody.position, Dir, Color.cyan);
            if (tryingToGetOut)
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
                Debug.DrawLine(ownerBody.position, body.position, Color.green);
                dir.y += 1f;
                //Debug.Log($"magnitude: {{dir.magnitude}}");
                if (dir.magnitude <= range * 1.5 && holdAvalible) //ЕСЛИ ПЕРСОНАЖ ДВИГАЕТ САМ СЕБЯ И PULL НЕ ПЕРЕХОДИТ В HOLD ТО ЭТО ИЗ-ЗА ЭТОГО
                {
                    this.grabbingbody = body;
                    body.velocity = Vector2.zero;
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
                    Vector2 dir = throwingbody.position - ownerBody.position;
                    throwingbody.AddForce(dir * hlabyshPower, ForceMode2D.Impulse);
                    this.Stop(true);
                }
                else
                {
                    Vector2[] conePath = new Vector2[3];
                    conePath[0] = new Vector2(0, 0);

                    Vector2 buf = Util.AimDIr() * range * 2.5f;
                    buf = Quaternion.Euler(0, 0, 20) * buf;
                    //buf += MainCharacter.body.position;
                    conePath[1] = buf;

                    buf = Util.AimDIr() * range * 2.5f;
                    buf = Quaternion.Euler(0, 0, -20) * buf;
                    //buf += MainCharacter.body.position;
                    conePath[2] = buf;
                    cone.SetPath(0, conePath);
                    ContactFilter2D filter = new ContactFilter2D();
                    filter.SetLayerMask(Util.LayerPhysObjectsOnly());
                    List<Collider2D> results = new List<Collider2D>();
                    cone.OverlapCollider(filter, results);
                    foreach (Collider2D throwingCollider in results)
                    {
                        Rigidbody2D throwingBody = throwingCollider.GetComponent<Rigidbody2D>();
                        Vector2 dir = throwingBody.position - ownerBody.position;
                        if (Physics2D.Raycast(ownerBody.position, dir, dir.magnitude, Util.LayerStaticPhysObjectsOnly()).collider == null)
                        throwingBody.AddForce(dir * hlabyshPower, ForceMode2D.Impulse);
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
            holding = false;
            prevForce.Set(0f, 0f);
            grabbingObject.GetComponent<Rigidbody2D>().gravityScale = grabbingOldGravityscale;
            grabbingObject.layer = grabbingOldLayer;
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
