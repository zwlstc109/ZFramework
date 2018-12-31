using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
namespace Zframework
{
    public abstract class BaseManager : MonoBehaviour
    {

        private void Awake()
        {
            Z.Core.RegisterManager(this, MgrIndex);            
        }
        /// <summary>
        /// 初始化组件
        /// </summary>
        internal abstract void Init();
        internal virtual void MgrUpdate()
        {

        }
        internal abstract void ShutDown();
        /// <summary>
        /// 组件枚举Id
        /// </summary>
        protected abstract int MgrIndex { get; }
    }
}

