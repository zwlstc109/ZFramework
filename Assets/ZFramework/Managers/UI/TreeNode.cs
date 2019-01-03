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
    public class TreeNode<T> where T:class
    {
       
        public TreeNode<T> Parent=null;
        public T Value;
        public List<TreeNode<T>> Children = new List<TreeNode<T>>();
        public string Name;
        public TreeNode(string name) { Name = name; }
        public TreeNode() { }
        public void Fill(string name) { Name = name; }
        public void Fill(T value) { Value = value; }
        public void AddChild(TreeNode<T> node)
        {
            Children.Add(node);
            node.Parent = this;
        }
        public void RemoveChild(TreeNode<T> node)
        {         
            ActionRecursive(n=>
            {
                Children.Remove(n);
                Z.Pool.Return(ref n);
            } );         
        }
        public void ActionOnBrothers(Action<TreeNode<T>> action)
        {
            for (int i = 0; i < Parent.Children.Count; i++)
            {
                if (Parent.Children[i]==this)
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