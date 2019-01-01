using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{
    
    public class ProcedureLaunch:ProcedureBase
    {
        public override void OnEnter(object userData = null)
        {
            Z.Debug.Log("ProcedureLaunch enter");
            //...加载一些东西可能
            //...
            //...
            //...
            //...
            Z.Scene.LoadScene("Start");//TODO 如何让场景加载和UI开启配合起来 还有串联Procedure切换
            Z.UI.OpenUI("Assets/GameData/Prefabs/UGUI/Panel/pnlStartMenu.prefab");
        }
        public override void OnUpdate()
        {
           
        }
    }
}