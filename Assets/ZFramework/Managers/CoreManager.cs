using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{
    public class CoreManager : BaseManager
    {
        private Action mCoreUpdate;
        private Action mCoreInit;

        protected override int MgrIndex { get { return (int)ManagerIndex.Core; } }
        private IDisposable mCoreStopper;
        bool mInitial = false;
        internal override void Init()
        {
            //ObservableLogger.Listener.LogToUnityDebug();
            Z.Log.Log("CoreManager init" + Time.frameCount.ToString());
            if (!mInitial)
            {
                DontDestroyOnLoad(gameObject);
            }
            mInitial = true;
            //Debug.Log();     
        }
        /// <summary>
        /// ZFrameworkCore 启动！
        /// </summary>
        /// <param name="coreUpdate"></param>
        /// <param name="coreInit"></param>
        internal void CoreStart(Action coreUpdate,Action coreInit)
        {
            mCoreUpdate = coreUpdate;
            mCoreInit = coreInit;
            mCoreStopper= Observable.TimerFrame(0).Subscribe(_ =>//写了0 但是他会在1帧后执行
            {
                Observable.EveryUpdate().Subscribe(__ => mCoreUpdate());
                coreInit();
            });
           
        }
        private void OnApplicationQuit()
        {
#if UNITY_EDITOR          
            //Resources.UnloadUnusedAssets();
            //Debug.Log("清空编辑器缓存");
#endif
        }
        internal override void MgrUpdate()
        {
           
        }

        internal override void ShutDown()
        {
            mCoreStopper.Dispose();
        }
    }
}
