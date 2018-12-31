using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zframework;
using System;
public class PanelStart : PanelBase
{
    [SerializeField]private Button mbtnStart = null;
    [SerializeField] private Button mbtnQuit = null;

    public override void OnLoad(object userData = null)
    {
        base.OnLoad(userData);
        mbtnStart.onClick.AddListener(() =>
        {
            CloseUI();
            Z.Scene.LoadSceneAsync("TestScene1", _ => Z.Procedure.ChangeProcedure<ProcedurePlay>());
        });


        mbtnQuit.onClick.AddListener(() => Application.Quit());
    }

    public override void OnOpen(object userData = null)
    {
        base.OnOpen(userData);
        gameObject.Show(); 
    }
    public override void OnClose(object userData = null)
    {
        base.OnClose(userData);
        gameObject.Hide();
    }

}