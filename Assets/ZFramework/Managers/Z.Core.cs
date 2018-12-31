using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Linq;

namespace Zframework
{
    /// <summary>
    /// 组件索引
    /// </summary>
    /// 按照这个顺序初始化和更新
    public enum ManagerIndex
    {
        Pool = 0,
        Log,
        Core,
        Resource,
        Audio,
        Subject,
        Fsm,
        Scene,
        Unit,
        UI,
        Procedure,
        Count
    }

    public static partial class Z
    {
        internal static class Core
        {
            // 所有的Manager  
            private static List<BaseManager> mMgrLst;
       
            private static bool mErrorFlag=false;
            static Core()
            {
                //根据枚举长度初始化一样数量的
                mMgrLst = new List<BaseManager>(new BaseManager[(int)ManagerIndex.Count]);
            }
            // 注册Manager 由所有BaseManager Awake时自动调用, BaseManager用MB的方式，是为了今后要做Inspector扩展
            internal static void RegisterManager(BaseManager mgr, int index)
            {
                if (mMgrLst[index] != null)
                {
                    Debug.LogError("重复的ManagerIndex");
                    mErrorFlag = true;
                    //return;
                }
                mMgrLst[index] = mgr;

                // CoreManager 特殊处理     update、init交给CoreManager负责
                var core = mgr as CoreManager;
                if (core&&!mErrorFlag)
                {
                    core.CoreStart(CoreUpdate,CoreInit);
                }
            }

            //回调所有Manager的update
            internal static void CoreUpdate()
            {
                for (int i = 0; i < mMgrLst.Count; i++)
                {
                    mMgrLst[i].MgrUpdate();
                }
            }
            //按顺序初始化所有Manager 
            internal static void CoreInit()
            {
                for (int i = 0; i < mMgrLst.Count; i++)
                {
                    mMgrLst[i].Init();
                }
            }
        }
        #region 框架组件
        /// <summary> AssetBundleManager</summary>
        //internal static AssetBundleManager AssetBundle { get; set; }
        /// <summary> ResrouceManager</summary>
        public static ResourceManager Resource { get; internal set; }
        /// <summary>AudioManager</summary>
        public static AudioManager Audio { get; internal set; }
        /// <summary>SubjectManager </summary>
        public static SubjectManager Subject { get; internal set; }
        /// <summary>FSMManager</summary>
        public static FsmManager Fsm { get; internal set; }
        /// <summary>SceneManager</summary>
        public static SceneManager Scene { get; internal set; }
        /// <summary>ProcedureManager</summary>
        public static ProcedureManager Procedure { get; internal set; }
        /// <summary>LogManager</summary>
        public static LogManager Log { get; internal set; }
        /// <summary>PoolManager</summary>
        public static PoolManager Pool { get; internal set; }
        /// <summary>UnitManager</summary>
        public static UnitManager Unit { get; internal set; }
        /// <summary>UIManager</summary>
        public static UIManager UI { get; internal set; }
        #endregion


    }
}