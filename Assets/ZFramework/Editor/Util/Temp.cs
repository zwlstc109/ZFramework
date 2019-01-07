using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UniRx;
#if UNITY_EDITOR
using UnityEditor;
namespace Zframework.Editor
{
    public static partial class Util
    {
        public static class Path
        {
            //public static string Guid2AssetName(string guid)
            // {
            //     string path = AssetDatabase.GUIDToAssetPath(guid);
            //     path = path.Remove(0, path.LastIndexOf('/') + 1);
            //     return path;
            // }
            public static string SingleAsset2ABName(string path)
            {
                path = path.Remove(0, path.LastIndexOf('/') + 1);
                path = path.Replace('.', '_');
                return path;
            }
        }
    }
}
#endif