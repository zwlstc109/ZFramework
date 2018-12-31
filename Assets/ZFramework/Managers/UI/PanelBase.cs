using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Zframework
{
    
    public class PanelBase:MonoBehaviour
    {
        
        internal int mId;
        public int Id { get { return mId; } }
        //每个Panel都有各自的资源组和unit组 使得加载的资源可以Panel为单位进行卸载
        [HideInInspector]public int mUnitGroupIndex;//TODO 由于申请unit组时会连带申请一个res组 和下面有点重复 待优化
        [HideInInspector]public int mResGroupIndex;

        [HideInInspector] public CanvasGroup CanvasGroup;
        internal string Path;
        //是否允许多个实例
        public bool AllowMultInstance=false;
        //所在的UI组
        internal UIGroup UIGroup;//TODO 组有点多...
        public virtual void OnLoad(object userData = null)
        {
            mUnitGroupIndex = Z.Unit.RegistUnitGroup();//TODO 由于申请unit组时会连带申请一个res组 和下面有点重复 待优化
            mResGroupIndex = Z.Resource.RegistGroup();
        }
        public virtual void OnOpen(object userData=null)
        {
            //TODO 通用dotween
        }
        public virtual void OnClose(object userData = null)
        {

        }
        public virtual void OnUpdate(object userData = null)
        {

        }
        public virtual void OnPause(object userData = null)
        {

        }
        public virtual void OnResume(object userData = null)
        {
            
        }
        public virtual void OnUnLoad(object userData = null)
        {
            Z.Unit.Release(mUnitGroupIndex);
            Z.Unit.Release(mResGroupIndex);
        }

        protected void CloseUI()
        {
            UIGroup.Close();
        }
    }
}