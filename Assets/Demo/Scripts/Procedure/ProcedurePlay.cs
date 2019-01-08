using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Threading.Tasks;
namespace Zframework
{
    public class Test
    {
    public  int value ;
    }
    public class ProcedurePlay : ProcedureBase
    {
        
        public override void OnEnter(object userData = null)
        {
            Z.Audio.PlayBackGroundMusic("Assets/GameData/Sounds/menusound.mp3");

            Z.UI.Open("Assets/GameData/Prefabs/UGUI/Panel/PnlMainMenu.prefab");
            Z.UI.Open("Assets/GameData/Prefabs/UGUI/Panel/PnlMainMenu2.prefab");


              
            Z.Obs.KeyDown(KeyCode.Z).Subscribe(_=> Z.Scene.Fade(FadeMode.FadeInOut));
            Z.Obs.KeyDown(KeyCode.X).Subscribe(_ => Z.Subject.Fire("Z_FadeOutAction", null));
            //Z.Pool.RegisterClassPool<Test>();
            //Z.Pool.Take<Test>();

            //Test t1 = Z.Pool.Take<Test>();
            //t1.value = 1;

            //Test tt1 = t1;

            //Z.Pool.Return(ref t1);

            //Z.Pool.Return(ref tt1);

            //Test s1 = Z.Pool.Take<Test>();
            //Test s2 = Z.Pool.Take<Test>();

            //Debug.Log(s1.value + " " + s2.value);
        }
        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Z.Unit.Release(0, true);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Z.Unit.Release(0, false);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                Z.Obs.ForLoop(1000, _ => Z.Unit.LoadUnit("Assets/GameData/Prefabs/Attack.prefab"));
            }
        }
    }
}