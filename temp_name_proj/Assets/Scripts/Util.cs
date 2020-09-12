using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class Util
    {
        public static Vector2 AimDIr()
        {
            if(false) //TODO: Если от стика, отвечающего за аим, инпут не равен нулю
            {
                //Сварить из этого нормализованный вектор и вернуть
            }
            else//В противном случае смотреть на мышь
            {
                return ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - MainCharacter.body.position).normalized;
            }
        }
    }
}