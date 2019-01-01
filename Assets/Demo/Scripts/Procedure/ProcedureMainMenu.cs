using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{
    
    public class ProcedureMainMenu : ProcedureBase
    {
        public override void OnEnter(object userData = null)
        {
            Z.Debug.Log("ProcedureMainMenu enter");
        }
        public override void OnUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ChangeState<ProcedurePlay>();
            }
        }
    }
}