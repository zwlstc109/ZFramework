using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{
    
    public class ProcedureLaunch:ProcedureBase
    {
        public override void OnEnter(object userData = null)
        {
            Z.Debug.Log("ProcedureLaunch enter");
            //...加载一些东西可能
            //...
            //...
            //...
            //...
            Z.Scene.LoadScene("Start");//TODO 如何让场景加载和UI开启配合起来 还有串联Procedure切换
            Z.UI.Open("Assets/GameData/Prefabs/UGUI/Panel/pnlStartMenu.prefab");
            //Z.Pool.RegisterClassPool<TreeNode<Test>>(100);

            //Tree<Test> tree = new Tree<Test>();
            //TreeNode<Test> A = new TreeNode<Test>("A"); TreeNode<Test> B = new TreeNode<Test>("B");
            //tree.Root.AddChild(A); tree.Root.AddChild(B);
            //TreeNode<Test> AA1 = new TreeNode<Test>("AA1"); TreeNode<Test> AA2 = new TreeNode<Test>("AA2"); TreeNode<Test> AA3 = new TreeNode<Test>("AA3");
            //A.AddChild(AA1); A.AddChild(AA2); A.AddChild(AA3);
            //tree.Root.RemoveChild(A);
            //Debug.Log(".........................................");
            //tree.Root.ActionRecursive(n => { });

        }
        public override void OnUpdate()
        {
           
        }
    }
}