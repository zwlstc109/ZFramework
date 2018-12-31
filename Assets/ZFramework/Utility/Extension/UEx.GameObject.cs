using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{
    
    public static partial class UnityExtension
    {
         public static T GetOrAddComponent<T>(this GameObject gameObject) where T: Component
        {
            var com = gameObject.GetComponent<T>();
            if (com==null)
            {
                com = gameObject.AddComponent<T>();
            }
            return com;
        }
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            var com = component.GetComponent<T>();
            if (com == null)
            {
                com = component.gameObject.AddComponent<T>();
            }
            return com;
        }
    }
}