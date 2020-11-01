using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    class Util
    {
        public static Vector2 AimDIr()
        {
            if (false) //TODO: Если от стика, отвечающего за аим, инпут не равен нулю
            {
                //Сварить из этого нормализованный вектор и вернуть
            }
            else//В противном случае смотреть на мышь
            {
                return ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - MainCharacter.body.position).normalized;
            }
        }
        public static void GameOver()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); //Перезагрузка сцены
        }
        public static LayerMask LayerPhysObjectsOnly() //Если проблемы со слоями нормально не решатся и у нас 
                                                       //физ. объекты для притягивания будут на больше чем одном слою, то дописать их сюда в маску
        {
            return LayerMask.GetMask("Default");
        }
        public static LayerMask LayerMaterialObjectsOnly()
        {
            return LayerMask.GetMask("Default", "StaticPhysicalObjects", "Platform");
        }
    }
}