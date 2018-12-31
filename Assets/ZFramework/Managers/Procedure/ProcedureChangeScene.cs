using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{
    
    public class ProcedureChangeScene:ProcedureBase
    {
        public override void OnEnter(object userData = null)
        {
            base.OnEnter(userData);

            string sceneName = (string)userData;

        }
    }
}