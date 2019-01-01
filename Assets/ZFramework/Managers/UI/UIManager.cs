using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{
    public enum BuiltinUIGroup
    {
        Default,
        Count
    }
    public class UIManager : BaseManager
    {
        protected override int MgrIndex { get { return (int)ManagerIndex.UI; } }
        private List<UIGroup> mGroupLst = new List<UIGroup>();
        [SerializeField] internal Transform CanvasRoot=null;
        public string SceneLoadingPath=null;
        //private Dictionary<string, List<PanelBase>> mPanelDic = new Dictionary<string, List<PanelBase>>();
        internal override void Init()
        {
            Z.UI = this;
            mGroupLst.Add(new UIGroup());
            Z.Subject.GetSubject("StartLoadScene").Subscribe(_ =>OpenUI(SceneLoadingPath));//TODO 等于直接绑死了一种loading界面 
        }
        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="path">固定地址的路径</param>    //TODO 路径当参数很怪
        /// <param name="userData"></param>
        public void OpenUI(string path,object userData=null)//TODO 现在一种路径只能有一个实例 
        {
            PanelBase panel = null;
            if (!mGroupLst[0].HasPanel(path))
            {
                panel = LoadUI(path, userData);
            }
            mGroupLst[0].Open(path, userData);
        }

        internal PanelBase LoadUI(string path,object userData)
        {
            var panel = Z.Unit.LoadUnit(path, (int)BuiltinGroup.UI).GetComponent<PanelBase>();
            panel.CanvasGroup = panel.GetOrAddComponent<CanvasGroup>();
            panel.Path = path;
            panel.AddTo(mGroupLst[0]);
            panel.transform.SetParent(CanvasRoot,false);
            panel.OnLoad(userData);
            return panel;
        }


        internal override void MgrUpdate()
        {
            for (int i = 0; i < mGroupLst.Count; i++)
            {
                mGroupLst[i].Update();
            }
        }

        internal override void ShutDown()
        {
           
        }
    }
    /// <summary>
    /// 控制UI显示关闭的组 同组的受管理 不同组的不受影响 TODO暂时只有一个默认组 
    /// </summary>
    /// 同组的panel 当选择
    public class UIGroup
    {
        private List<PanelBase> mPnlLst = new List<PanelBase>();
        private Stack<PanelBase> mPnlStack = new Stack<PanelBase>();
        //private PanelBase mCurPanel = null;

        internal void Add(PanelBase panel,object userData=null)
        {          
            mPnlLst.Add(panel);
        }
        internal bool HasPanel(string path)
        {
            return mPnlLst.Find(p => ReferenceEquals(path, p.Path))!=null;
        }
      
        internal void Open(string path,object userData = null)
        {
       
            if (mPnlStack.Count>0)
            {
                var curPanel = mPnlStack.Peek();
                curPanel.OnPause();
            }
            var panel = mPnlLst.Find(p => ReferenceEquals(path, p.Path));
            mPnlStack.Push(panel);
            panel.OnOpen(userData);
        }
        internal void Update()
        {
            if (mPnlStack.Count > 0)
            {
                var curPanel = mPnlStack.Peek();
                curPanel.OnUpdate();
            }
        }
        internal void Close()
        {

            if (mPnlStack.Count>0)
            {
                var curPanel = mPnlStack.Pop();
                curPanel.OnClose();
                if (mPnlStack.Count>0)
                {
                    var curcurPanel = mPnlStack.Peek();
                    curcurPanel.OnResume();
                }
            }
        }
    }
}