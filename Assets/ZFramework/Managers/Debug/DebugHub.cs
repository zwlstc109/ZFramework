using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.UI;

namespace Zframework
{
  
    
    public class DebugHub:MonoBehaviour
    {
        public Text[] TextLst;
        public static DebugHub Instance;
        private int canUseIndex=0;
        private void Start()
        {
            Instance = this;
        }

        public void RegistText<T>(IReadOnlyReactiveProperty<T> t)
        {
            t.SubscribeToText(TextLst[canUseIndex++]) ;
        }
    }
}