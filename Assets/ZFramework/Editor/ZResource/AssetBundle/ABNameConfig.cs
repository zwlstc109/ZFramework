using System.Collections.Generic;
using UnityEngine;
namespace Zframework.Editor
{
    //[CreateAssetMenu(fileName = "ABNameConfig", menuName = "CreatABNameConfig", order = 0)]
    public class ABNameConfig : ScriptableObject
    {     
        //单个文件所在文件夹路径，会遍历这个文件夹下面所有Prefab,所有的Prefab的名字不能重复，必须保证名字的唯一性
        public List<string> AllPrefabAB = new List<string>();//一个prefab一个包 
        public List<FolderABName> AllFolderAB = new List<FolderABName>();//单个文件夹里的所有资源打一个包

        [System.Serializable]
        public struct FolderABName
        {
            public string ABName;
            public string Path;
        }
    }
}