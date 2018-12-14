using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuItemPrintTime 
{
#if UNITY_EDITOR 
    [MenuItem("Qframework/GeneratePackageName")]
#endif
    static void LogTime()
    {                                                    //大写的M是月份
        Debug.Log("QFramework_" + DateTime.Now.ToString("yyyyMMdd_hh"));
    }
}
