using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UniRx;
namespace Zframework
{

#if UNITY_EDITOR
    using UnityEditor;
    public class ExportUnityPackage
    {                                                  //%e 表示关联快捷键 Ctrl+E
        [MenuItem("ZFramework/ExportFrameworkUnityPackage %e")]
        private static void menuItem()
        {
            var assetPathName = "Assets/ZFramework";//要打包的文件夹
            var fileName = "ZFramework_" + DateTime.Now.ToString("yyyyMMdd_hh") + ".unitypackage";//包名  ps:大写的M是月份
            AssetDatabase.ExportPackage(assetPathName, fileName, ExportPackageOptions.Recurse);
            Application.OpenURL("file:///" + Path.Combine(Application.dataPath, "../"));//打开一下Assets文件夹(导出的包放在这里了)
                                                                                        //生成上一级的目录要用这个api
        }

        public static void ExcuteMenuItem()
        {   //代码调用菜单
            EditorApplication.ExecuteMenuItem("ZFramework/ExportFrameworkUnityPackage %e");
        }

        //复制内容到剪贴板
        public static void Copy2ClipBoard(string content)
        {
            GUIUtility.systemCopyBuffer = content;
        }

    }
#endif
}