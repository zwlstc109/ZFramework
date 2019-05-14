using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{
 
   /// <summary>
   /// 框架入口
   /// </summary>
    public static partial class Z
    {   //关于为何不直接写在UniRx源码里 因为考虑到耦合性 同时也能对自己到底加了哪些东西有所管理 
        /// <summary>
        /// Observable工具
        /// </summary>
        public static class Obs
        {
            #region 常用Observable
            public readonly static IObservable<long> EveryUpdate = UniRx.Observable.EveryUpdate();
            public readonly static IObservable<long> EveryFixedUpdate = UniRx.Observable.EveryFixedUpdate();
            /// <summary>鼠标左键的observable</summary>
            public static IObservable<long> LeftClick()
            {
                return EveryUpdate.Where(_ => UnityEngine.Input.GetMouseButtonDown(0));
            }
            /// <summary>鼠标右键的observable</summary>
            public static IObservable<long> RightClick()
            {
                return EveryUpdate.Where(_ =>UnityEngine. Input.GetMouseButtonDown(1));
            }
            public static IObservable<long> KeyDown(KeyCode key)
            {
                return EveryUpdate.Where(_ => UnityEngine.Input.GetKeyDown(key));
            }
          
            #endregion
            #region 计时器
           
            public static IObservable<long> Timer(float delay)
            {
                return Observable.Timer(TimeSpan.FromSeconds(delay));
            }
            public static IObservable<UniRx.Unit> DelayFrame(int frame)
            {
                return Observable.ReturnUnit().DelayFrame(frame);
            }
            public static IObservable<long> Timer(float dueTime, float period)
            {
                return Observable.Timer(TimeSpan.FromSeconds(dueTime), TimeSpan.FromSeconds(period));
            }
          
            #endregion
            #region for循环
            public static void ForLoop(int loopTime,Action<int> action)
            {
                Observable.Range(0, loopTime).Subscribe(_ => action(_));
            }
            #endregion

            
        }

    }
}