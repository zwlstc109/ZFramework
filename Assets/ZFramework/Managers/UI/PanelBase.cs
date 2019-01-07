using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Zframework
{
    /// <summary>
    /// UI面板类 
    /// </summary>
    public class PanelBase : MonoBehaviour
    {

        internal int mId = -1;
        public int Id { get { return mId; } }

        private bool InPool = false;

        [HideInInspector] public CanvasGroup CanvasGroup;
        //internal string Path;
        //是否允许多个实例 TODO
        public bool AllowMultInstance = false;
        //所在的UI组（树）
        internal UIGroup UIGroup;//面板的所有回调都是基于其所在的UI组进行触发
        //所在的节点
        internal TreeNode<PanelBase> NodeInGroup;
        //实例所在的unit壳
        internal Unit Unit;//主要用于释放面板 入池

        internal int _UnitGroupIndex = -1;
        //所在的Unit组Index
        protected int UnitGroupIndex { get { return _UnitGroupIndex; } }

        
        //是否响应交互和update
        protected bool mAvailable = true;
        public virtual bool Available {
            get { return mAvailable; }
            protected set  //很尴尬的访问修饰符 protected是为了让程序集外的子类访问 但是我需要在程序集里也能访问 只能再加一个internal方法
            {
                mAvailable = value;
                CanvasGroup.interactable = value;
            }
        }    
        internal void SetAvailable(bool value) { Available = value; }
        protected bool mVisible = true;
        public bool Visible {
            get { return mVisible; }
            protected set{
                mVisible = value;
                gameObject.SetActive(value);
            } }
      
        /// <summary>
        /// 面板初始化时触发
        /// </summary>
        /// <param name="userData"></param>
        public virtual void OnLoad(object userData = null)
        {
            //Available = true;
            //Visible = true;
        }
        public virtual void OnOpen(object userData=null)
        {
            //TODO 通用dotween
            Available = true;
            Visible = true;
        }
        public virtual void OnClose(object userData = null)
        {
            Available = false;
            Visible = false;
        }
        public virtual void OnUpdate(object userData = null)
        {
            if (Available==false)
            {
                return;
            }
        }
        /// <summary>
        /// UI组中被其他面板覆盖时触发
        /// </summary>
        /// <param name="userData"></param>
        public virtual void OnCover(object userData = null)
        {
            Available = false;
        }
        /// <summary>
        /// 相对于OnCover
        /// </summary>
        /// <param name="userData"></param>
        public virtual void OnReveal(object userData = null)
        {
            Available = true;
        }
        /// <summary>
        /// 同一父节点的面板切换时触发
        /// </summary>
        /// <param name="userData"></param>
        public virtual void OnSwitch(object userData = null)
        {
            //Visible = false;  
        }
        public virtual void OnUnLoad(object userData = null)
        {
            Available = false;
            Visible = false;
          
        }
        /// <summary>
        /// 在本面板下打开新面板
        /// </summary>
        /// <param name="path"></param>
        /// <param name="userData"></param>
        /// <param name="open">是否直接打开</param>
        protected PanelBase Open(string path,Transform parent=null,bool await=false, object userData=null,int unitGroupIndex=-1, bool open = true)
        {
            if (!Available)
            {
                return null;
            }
           return UIGroup.Open(NodeInGroup, path, parent==null?transform:parent,await, userData,unitGroupIndex, open);
        }
        /// <summary>
        /// 在本面板下打开旧有面板
        /// </summary>
        /// <param name="path"></param>
        /// <param name="userData"></param>
        /// <param name="open">是否直接打开</param>
        protected void Open(PanelBase child,bool await=false, object userData=null)
        {
            if (!Available)
            {
                return ;
            }
            UIGroup.Open(NodeInGroup, child,await, userData);
        }
        /// <summary>
        /// 在UIGroup中关闭自己
        /// </summary>
        protected void CloseSelf()
        {
            if (!Available)
            {
                return;
            }
            
            UIGroup.Close(NodeInGroup);
        }
        /// <summary>
        /// 在UIGroup中 向同一层级的panel发出切换请求
        /// </summary>
        //protected void SwitchTo(PanelBase panel,object userData=null)
        //{
        //    if (!Available)
        //    {
        //        return;
        //    }
        //    if (panel==this)
        //    {
        //        Z.Debug.Warning("Panel试图切换到自己？");
        //        return;
        //    }
        //    UIGroup.Switch(NodeInGroup.Parent, panel, userData);
        //}
        /// <summary>
        /// 释放面板控制权 TODO
        /// </summary>
        protected void ReleaseSelf(bool destroy=false,object userData=null)
        {
            if (!Available)
            {
                return;
            }
            UIGroup.Release(NodeInGroup,destroy,userData);
        }
        /// <summary>
        /// 在UIGroup中锁定自己 使得UIGroup不接受任何打开请求
        /// </summary>
        protected void LockSelf(bool value)
        {
            if (!Available)
            {
                return;
            }
            UIGroup.Lock = value;
        }


        public static bool operator !=(PanelBase lhs, PanelBase rhs)
        {
            if (System.Object.Equals(rhs, null))
            {
                if (System.Object.Equals(lhs, null))
                {
                    return false;
                }

                if (lhs.InPool ||!lhs.gameObject)
                {
                    return false;
                }
            }
            return !ReferenceEquals(lhs, rhs);
        }
        
        public static bool operator ==(PanelBase lhs, PanelBase rhs)
        {
            if (System.Object.Equals(rhs, null))
            {
                if (System.Object.Equals(lhs, null))
                {
                    return true;
                }
                if (lhs.InPool || !lhs.gameObject)
                {
                    return true;
                }
            }
            return ReferenceEquals(lhs, rhs);
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}