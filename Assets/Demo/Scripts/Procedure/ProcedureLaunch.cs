using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Threading.Tasks;

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
            
            Z.Scene.LoadScene("Assets/Demo/Scenes/Start.unity", FadeMode.FadeOut,()=> Z.UI.Open("Assets/GameData/Prefabs/UGUI/Panel/pnlStartMenu.prefab"));

            //int[] numbers = new int[10000] ;
            //System.Random rd = new System.Random();
            //Parallel.For(0,numbers.Length, i=>{
            //    int factor = Z.Rd.SysHitPercent(50) ? -1 : 1;
            //    numbers[i] = 100 + factor *Z.Rd.SysNumber(50, 99);
            //});
            //FunnySort sort = new FunnySort(numbers);
            //sort.Sort();

#if UNITY_EDITOR
            //UnityEditor.EditorUtility.
#endif
            
            


            //TestClass c = new TestClass();
            //var temp = c;
            //c.Destroy();
            //Debug.Log(temp == null);
        }

    }

    class TestClass:ZObject
    {
        

    }
}