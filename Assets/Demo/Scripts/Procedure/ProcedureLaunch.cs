using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

namespace Zframework
{
    
    public class ProcedureLaunch:ProcedureBase
    {
        /*ZCatridge c;*///for test
        public override void OnEnter(object userData = null)
        {
            Z.Debug.Log("ProcedureLaunch enter");
            //...加载一些东西可能
            //...
            //...
            //...
            //...
            
            Z.Scene.LoadScene("Assets/Demo/Scenes/Start.unity", FadeMode.WaitingThenFadeOut,()=> Z.UI.Open("Assets/GameData/Prefabs/UGUI/Panel/pnlStartMenu.prefab"));

            //Z.Input.CurrentKeyDown.Subscribe(s=>Debug.Log(s));

            //Z.Pool.RegisterClassPool<ZCatridge>();

            //c = Z.Pool.Take<ZCatridge>();


            //Z.Obs.KeyDown(KeyCode.Q).Subscribe(_=>c.Fill(() => Debug.Log("a")));
            //Z.Obs.KeyDown(KeyCode.W).Subscribe(_ => c.Fire());


            //Z.Obs.KeyDown(KeyCode.V).Subscribe(_ => {
            //    var e = Event.current;
            //    if (e.isKey)
            //    {
            //        Debug.Log(e.keyCode);
            //    }

            //});
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
        public override void OnUpdate()
        {
            base.OnUpdate();
            
        }
    }

    
    class TestClass:ZObject
    {
        

    }
}