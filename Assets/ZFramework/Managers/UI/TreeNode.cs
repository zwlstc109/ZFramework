using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{
    /// <summary>
    /// 树节点
    /// </summary>
    /// <typeparam name="T"></typeparam>  
    public class TreeNode<T>:ZObject where T:class
    {
       
        public TreeNode<T> Parent=null;
        public T Value;
        public List<TreeNode<T>> Children = new List<TreeNode<T>>();
        public string Name;//测试用
        public TreeNode(string name) { Name = name; }
        public TreeNode() { }
        public void Fill(string name) { Name = name; }
        public void Fill(T value) { Value = value; }
        public void AddChild(TreeNode<T> node)
        {
            Children.Add(node);
            node.Parent = this;
        }
        public TreeNode<T> AddChild(T value)
        {
            var node = _GetOneEmptyChild();//先尝试找到一个空的节点  （任何操作都不会删除节点，只会置空他的value）
            if (node == null)
            {
                node = Z.Pool.Take<TreeNode<T>>();
                AddChild(node);
            }
            node.Fill(value);
            return node;
        }
        public void RemoveChild(TreeNode<T> node)
        {
            var child = Children.Find(n => n == node);
            if (child==null)
            {
                return;
            }
            child.ActionRecursive(n=>
            {
                Parent.Children.Remove(n);
                Z.Pool.Return(n);
            } );         
        }
        public void RemoveSelf()
        {
            if (Parent==null)
            {
                Z.Debug.Warning("无法移除一个根");
                return;
            }
            ActionRecursive(n =>
            {
                Parent.Children.Remove(n);
                Z.Pool.Return(n);
            });
        }
        private  TreeNode<T> _GetOneEmptyChild()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].Value==null)
                {
                    return Children[i];
                }
            }
            return null;
        }
        public void Clean()
        {
            Value = null;
            Children.Clear();
        }

        public void ActionOnBrothers(Action<TreeNode<T>> action)
        {
            for (int i = 0; i < Parent.Children.Count; i++)
            {
                if (Parent.Children[i]==this||Parent.Children[i].Value==null)
                    return;
                action?.Invoke(Parent.Children[i]);
            }
        }
        /// <summary>
        /// 从下往上递归执行action 
        /// </summary>
        /// <param name="action"></param>
        public void ActionRecursive(Action<TreeNode<T>> action)
        {
           for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].Value==null)
                {
                    continue;
                }
                Children[i].ActionRecursive(action);
            }
            //Z.Debug.LogFormat("node {0} actioned", Name);
            action?.Invoke(this);
        }
    }
    /// <summary>
    /// 树
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Tree<T>where T : class
    {
        public TreeNode<T> Root=new TreeNode<T>("root");
    }
}