﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{
    public enum BuiltinUIGroup //UI组仅用来控制组内面板关系 和资源释放无关
    {
        Default,
        Custom,
        Count
    }
    public class UIManager : BaseManager
    {
        protected override int MgrIndex { get { return (int)ManagerIndex.UI; } }
        //用来迅速判断面板是否加载过 的字典
        internal Dictionary<string, PanelBase> PathPnlDic = new Dictionary<string, PanelBase>();
        //所有的UI组
        private List<UIGroup> mGroupLst = new List<UIGroup>();
        [SerializeField] internal Transform CanvasRoot=null;
      
        IReadOnlyReactiveProperty<int> mTreeNodePoolCount;//TEST
        //private Dictionary<string, List<PanelBase>> mPanelDic = new Dictionary<string, List<PanelBase>>();
        internal override void Init()
        {
            Z.UI = this;
            Z.Pool.RegisterClassCustomPool(() => new TreeNode<PanelBase>(), UIGroup.Clean, 150);

            mTreeNodePoolCount =Z.Pool.mObjectPoolDic[typeof(TreeNode<PanelBase>).FullName].ObserveEveryValueChanged(p=>p.Count).ToReactiveProperty<int>();
            DebugHub.Instance.RegistText(mTreeNodePoolCount);

            Z.Obs.ForLoop((int)BuiltinUIGroup.Count, i=> mGroupLst.Add(new UIGroup(i)));
        }


        /// <summary>
        /// 在root下打开一个新的面板
        /// </summary>
        /// <param name="path"></param>
        /// <param name="userData"></param>
        /// <param name="uiGroupIndex">指定在哪个uiGroup</param>
        /// <param name="unitGroupIndex"></param>
        public PanelBase Open(string path, object userData = null, int uiGroupIndex = 0, int unitGroupIndex = (int)BuiltinGroup.UI, bool open = true)
        {
            return mGroupLst[uiGroupIndex].Open(path, userData, unitGroupIndex, true);
        }
        public void Open(PanelBase panel,object userData = null,int uiGroupIndex=0)
        {
            mGroupLst[uiGroupIndex].Open(panel, userData);
        }

        internal PanelBase Load(string path,object userData,UIGroup uiGroup, int unitGroupIndex)
        {
            var pnl = PathPnlDic.GetValue(path);
            if (pnl != null && !pnl.AllowMultInstance)
            {
                Z.Debug.Warning("此面板不允许多个实例:" + pnl.Unit.ResItem.Path);
                return null;
            }

            var unit = Z.Unit.LoadUnit(path, unitGroupIndex);
            var panel =unit.GO.GetComponent<PanelBase>();
            panel.Unit = unit;
            panel._UnitGroupIndex = unitGroupIndex;
            panel.CanvasGroup = panel.GetOrAddComponent<CanvasGroup>();
            panel.UIGroup=uiGroup;
            panel.transform.SetParent(CanvasRoot,false);
            panel.OnLoad(userData);

            if (pnl == null)
            {
                PathPnlDic.Add(path, panel);
            }
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
    /// TODO 考虑UI组只是提供同组panel之间的关系并调用相关回调 具体实现 由panel自己重写  
    public class UIGroup
    {
        //面板树 考虑把面板的关系抽象成树 同一个父节点的面板之间可以切换，并且纵向的，也有栈的功能 TODO 进一步思考树和group的关系 暂时看上去UIGroup可以不要了 只用维护一个panel树就可以 毕竟即使有多棵树也可以合并成一棵
        private Tree<PanelBase> mPanelTree = new Tree<PanelBase>();
 
        internal PanelBase FocusPanel = null;
        internal bool Lock = false;
        internal int Id = -1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId">UIManager中groupLst的索引</param>
        public UIGroup(int groupId)
        {
            Id = groupId;
        }

        
        /// <summary>
        /// 在根节点上打开新的
        /// </summary>
        /// <param name="path"></param>
        /// <param name="userData"></param>
        /// <param name="open">是否自动打开</param>
        internal PanelBase Open(string path, object userData,int unitGroupIndex=-1,bool open=true)
        {
            return Open(mPanelTree.Root,path, userData,(int)BuiltinGroup.UI, open);
        }
        /// 在某节点上打开新的
        /// </summary>
        /// <param name="parentNode">某节点</param>
        /// <param name="path"></param>
        /// <param name="userData"></param>
        /// <param name="unitGroupIndex"></param>
        /// <param name="open">是否自动打开</param>
        /// <returns></returns>
        internal PanelBase Open(TreeNode<PanelBase> parentNode, string path, object userData,  int unitGroupIndex = -1, bool open = true)
        {
            if (Lock )
            {
                return null;
            }                                                 //-1则用父节点的Unit组Id 否则用参数指定的Id
            PanelBase newPanel = Z.UI.Load(path, userData, this, unitGroupIndex == -1 ? parentNode.Value._UnitGroupIndex : unitGroupIndex);

            if (newPanel == null)
            {
                return null;
            }
            var node = parentNode.AddChild(newPanel);
            newPanel.NodeInGroup = node;
         
            if (open)
            {
                _Open(parentNode, node, userData);
            }
         
            return newPanel;
        }
        /// <summary>
        /// 在根节点上打开旧的
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="userData"></param>
        /// <param name="open"></param>
        /// <returns></returns>
        internal void Open(PanelBase panel, object userData)
        {
            Open(mPanelTree.Root, panel, userData);
        }
        /// <summary>
        /// 在某节点上打开旧的
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="child"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        internal void Open(TreeNode<PanelBase> parentNode, PanelBase child, object userData)
        {
            if (child.Unit.GO == null)
            {
                Z.Debug.Warning("要打开的面板的unit里的GO为空");
                return;
            }
            if (Lock)
            {
                return;
            }
            var childNode = parentNode.Children.Find(n => n.Value == child);
            _Open(parentNode,childNode,userData);
        }
        private void _Open(TreeNode<PanelBase> parentNode, TreeNode<PanelBase> childNode,object userData)
        {
            if (childNode != null)
            {
                childNode.Value.OnOpen(userData);
                bool coverFlag = true;
                for (int i = 0; i < parentNode.Children.Count; i++)
                {                   
                    var curPanel = parentNode.Children[i].Value;
                    if (curPanel==null||curPanel == childNode.Value)//自排除
                    {
                        continue;
                    }
                    if (curPanel.Available || curPanel.Visible)//有一个可用的兄弟就不用调onCover
                    {
                        coverFlag = false;
                        break;
                    }
                }
                if (coverFlag)
                    parentNode.Value?.OnCover(userData);

           
                for (int i = 0; i < parentNode.Children.Count; i++)//为什么不用ActionOnBrothers 因为要传userData 闭包捕获可能会有Gc
                {
                    if (parentNode.Children[i].Value==null)
                    {
                        continue;
                    }
                    if (parentNode.Children[i].Value != childNode.Value)//自排除
                    {
                        parentNode.Children[i].Value.OnSwitch(userData);
                    }
                }

            }
        }
        /// <summary>
        /// 切换（同一个父节点下的子节点切换）
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="child"></param>
        /// <param name="userData"></param>
        internal void Switch(TreeNode<PanelBase> parentNode, PanelBase child, object userData)
        {
            Open(parentNode, child, userData);
        }
        /// <summary>
        /// 关闭某个面板自己
        /// </summary>
        /// <param name="node"></param>
        /// <param name="userData"></param>
        internal void Close(TreeNode<PanelBase> node)
        {                   
            node.ActionRecursive(n =>
            {
                n.Value.OnClose();
            });
            _RevealParent(node.Parent, node);
        }
        /// <summary>
        /// 关闭所有子节点
        /// </summary>
        /// <param name="parent"></param>
        internal void CloseChildren(TreeNode<PanelBase> parent)
        {
            for (int i = 0; i < parent.Children.Count; i++)
            {
                parent.Children[i].ActionRecursive(n =>
                {
                    n.Value.OnClose();
                });
            }
            parent.Value?.OnReveal();
        }
        internal void Release(TreeNode<PanelBase> node,bool destroy)
        {
            if (!node.Value.AllowMultInstance)
            {
                Z.UI.PathPnlDic.Remove(node.Value.Unit.ResItem.Path);
            }
            _RevealParent(node.Parent, node);
            node.ActionRecursive(n =>
            {
                n.Value.OnUnLoad();
                n.Value.Unit.ReleaseSelf(destroy);
                Clean(n);
            });
           
        }

        private void _RevealParent(TreeNode<PanelBase> parentNode,TreeNode<PanelBase> closeNode)
        {
            for (int i = 0; i < parentNode.Children.Count; i++)
            {
                var curPanel = parentNode.Children[i].Value;
                if (curPanel==null|| curPanel == closeNode.Value)
                {
                    continue;
                }
                if (curPanel.Available || curPanel.Visible)
                {
                    return;
                }
            }
            parentNode.Value?.OnReveal();
        }

        internal void Update()
        {
            mPanelTree.Root.ActionRecursive(n => //TODO 递归改循环
            {
                if (n.Value!=null)
                {
                    n.Value.OnUpdate();
                }
            });
        }
   
        //清理节点
        internal static void Clean(TreeNode<PanelBase> node)
        {
            node.Value = null;
            node.Children.Clear();
           
        }
    }
}