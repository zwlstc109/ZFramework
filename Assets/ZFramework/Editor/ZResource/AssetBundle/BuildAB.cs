using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Text;

namespace Zframework.Editor
{   
    public static class BuildAB
    {
        enum SingleAssetType
        {
            Prefab,
            Scene
        }
        //AB包打完后 所在的路径
        private static string mABBuildTargetPath = Application.streamingAssetsPath;
        //AB包 包名标记 配置文件 所在路径
        private static string mABNameConfigPath = "Assets/ZFramework/Editor/ZResource/AssetBundle/ABNameConfig.asset";
        //AB包 二进制清单 所在路径 （打包前会根据资源和包的依赖关系 生成XML和二进制各一份,供加载时使用，弃用原生Manifest,二进制文件会打入包中，xml只作查阅使用）
        private static string mABBinaryPath ="Assets/GameData/Data/ABData/AssetManifest.bytes";

        //-------------打两种包 一种找到配置文件夹下所有prefab 每个prefab打一个包 （prefab包）另一种对应配置文件夹下的所有资源打成一个包 （文件夹包）--------
      
        //所有以单个文件夹打包的AB     key 包名, value 全路径
        private static Dictionary<string, string> mAllFolderABDic = new Dictionary<string, string>();
        //所有以单个prefab打包的AB    key 包名，value 依赖全路径
        private static Dictionary<string, List<string>> mAllSingleAssetABDic = new Dictionary<string, List<string>>();
        //路径过滤
        private static List<string> mABPathFilters = new List<string>();     
        //储存所有有效资源路径 （储存的是那些会在运行时主动加载的资源 声音图片预制体之类，即二进制配置表中需要记录的信息）
        private static List<string> mAssetFilters = new List<string>();
        //所有所有的资源对应的AB包名
        private static Dictionary<string, string> mAllFileABNameDic = new Dictionary<string, string>();
        private static ABNameConfig abNameConfig;

        [MenuItem("ZFramework/AssetBundle/前往配置",priority =1)]
        public static void FocusOnABNameConfigAsset()
        {
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(mABNameConfigPath);
        }

        [MenuItem("ZFramework/AssetBundle/打包 (需配置)", priority = 0)]
        public static void Build()
        {
          
            mAssetFilters.Clear();
            mABPathFilters.Clear();
            mAllFolderABDic.Clear();
            mAllSingleAssetABDic.Clear();
            mAllFileABNameDic.Clear();
            abNameConfig = AssetDatabase.LoadAssetAtPath<ABNameConfig>(mABNameConfigPath);
            //------文件夹ab字典添加
            foreach (var folderAB in abNameConfig.AllFolderAB)
            {
                if (mAllFolderABDic.ContainsKey(folderAB.ABName))
                {
                    Debug.LogError("AB包配置名字重复，请检查！");
                }
                else
                {
                    mAllFolderABDic.Add(folderAB.ABName, folderAB.Path);
                    mABPathFilters.Add(folderAB.Path);
                    mAssetFilters.Add(folderAB.Path);
                }
            }
            //------单prefab ab字典添加
            //在配置文件夹下搜索所有prefab
            _FindSingleAsset(SingleAssetType.Prefab);
            //------scene ab字典添加
            //在配置文件夹下搜索所有场景
            _FindSingleAsset(SingleAssetType.Scene);
            foreach (string name in mAllFolderABDic.Keys)
            {
                setABName(name, mAllFolderABDic[name]);
            }

            foreach (string name in mAllSingleAssetABDic.Keys)
            {
                setABName(name, mAllSingleAssetABDic[name]);
            }

            BunildAssetBundle();
            //清除包名标记
            string[] abNames = AssetDatabase.GetAllAssetBundleNames();
            for (int i = 0; i < abNames.Length; i++)
            {
                AssetDatabase.RemoveAssetBundleName(abNames[i], true);
                EditorUtility.DisplayProgressBar("清除AB包名", "名字：" + abNames[i], i * 1.0f / abNames.Length);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
        }
        private static void _FindSingleAsset(SingleAssetType assetType)
        {
            string assetTypeStr = assetType == SingleAssetType.Prefab ? "Prefab" : "Scene";
            var assetLst = assetType == SingleAssetType.Prefab ? abNameConfig.AllPrefabAB : abNameConfig.AllSceneAB;

            string[] assetGuids = AssetDatabase.FindAssets("t:"+ assetTypeStr, assetLst.ToArray());

            for (int i = 0; i < assetGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[i]);
               
                mAssetFilters.Add(assetPath);
                if (!containPathFilter(assetPath))
                {
                    string assetName = Util.Path.SingleAsset2ABName(assetPath);
                    //if (assetName=="TestScene1_unity")
                    //{
                    //    int m = 0;
                    //}
                    //GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(prfPath);
                    string[] allDependences = AssetDatabase.GetDependencies(assetPath);
                    List<string> allDependPath = new List<string>();
                    for (int j = 0; j < allDependences.Length; j++)
                    {
                        EditorUtility.DisplayProgressBar("查找" + assetTypeStr, assetTypeStr + ":" + assetPath, j * 1.0f / allDependences.Length);
                        if (containPathFilter(allDependences[j]) || allDependences[j].EndsWith(".cs")|| allDependences[j].EndsWith("LightingData.asset")
                            || allDependences[j].Contains("Lightmap")|| allDependences[j].Contains("ReflectionProbe")||
                            (assetType == SingleAssetType.Scene&& allDependences[j].EndsWith(".mat")))

                            continue;
                        mABPathFilters.Add(allDependences[j]);
                        if (allDependences[j].Equals(assetPath) && assetType == SingleAssetType.Scene)//Scene过滤时 依赖项剔除自己
                            continue;
                        allDependPath.Add(allDependences[j]);
                    }



                    if (mAllSingleAssetABDic.ContainsKey(assetName))
                    {
                        Debug.LogErrorFormat("存在相同名字的{0}！名字：{1}" , assetTypeStr, assetName);
                    }
                    else
                    {
                        if (assetType == SingleAssetType.Scene)
                        {
                            List<string> temp = null;
                            temp = new List<string>(); temp.Add(assetPath);
                            if (allDependPath.Count>0)
                            {                               
                                Debug.LogWarningFormat("场景 {0} 不应该依赖于额外的资源", assetPath);
                            }
                          
                            mAllSingleAssetABDic.Add(assetName, temp);
                            mAllSingleAssetABDic.Add(Z.Str.Fmt("{0}_{1}", assetName, "Depend"), allDependPath);
                        }
                        else
                            mAllSingleAssetABDic.Add(assetName, allDependPath);
                    }
                }
            }
        }
        //标记包名
        static void setABName(string name, string path)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(path);
            if (assetImporter == null)
            {
                Debug.LogError("不存在此路径文件：" + path);
            }
            else
            {
                assetImporter.assetBundleName = name;
            }
        }

        static void setABName(string name, List<string> paths)
        {
            for (int i = 0; i < paths.Count; i++)
            {
                setABName(name, paths[i]);
            }
        }

        static void BunildAssetBundle()
        {   //所有现存的包名
            string[] abNames = AssetDatabase.GetAllAssetBundleNames();
            //需要写入manifest清单的资源dic  key为全路径，value为所在包名
            Dictionary<string, string> assetPathDic = new Dictionary<string, string>();
            //遍历所有包名
            for (int i = 0; i < abNames.Length; i++)
            {   //在此包名下所有的资源 的路径
                string[] assetPath = AssetDatabase.GetAssetPathsFromAssetBundle(abNames[i]);
                for (int j = 0; j < assetPath.Length; j++)
                {
                   
                    mAllFileABNameDic.Add(assetPath[j], abNames[i]);
                    //Debug.Log("此AB包：" + allBundles[i] + "下面包含的资源文件路径：" + allBundlePath[j]);
                    if (isAssetPath(assetPath[j]))
                    {
                        assetPathDic.Add(assetPath[j], abNames[i]);
                    }
                }
            }

            if (!Directory.Exists(mABBuildTargetPath))
            {
                Directory.CreateDirectory(mABBuildTargetPath);
            }

            DeleteOld();
            //生成自己的配置清单
            WriteData(assetPathDic);

            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(mABBuildTargetPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
            //mAssetLst.Clear();
            //mABPathFilter.Clear();
            //mAllFolderABDic.Clear();
            //mAllPrefabABDic.Clear();
            if (manifest == null)
            {
                Debug.LogError("AssetBundle 打包失败！");
            }
            else
            {
                Debug.Log("AssetBundle 打包完毕");
            }
        }
        /// <summary>
        /// 删除无用AB包
        /// </summary>
        static void DeleteOld()
        {
            string[] abNames = AssetDatabase.GetAllAssetBundleNames();
            DirectoryInfo targetfolderPath = new DirectoryInfo(mABBuildTargetPath);
            //获取打包目标路径下所有文件
            FileInfo[] files = targetfolderPath.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (containABName(files[i].Name, abNames) || files[i].Name.EndsWith(".meta") || files[i].Name.EndsWith(".manifest") 
                    || files[i].Name.Equals("assetmanifest")||files[i].Name.Equals("StreamingAssets"))
                {                                                                       //只有这个是tm驼峰的
                    continue;
                }
                else
                {                  
                    Debug.Log("此AB包已经被删或者改名了：" + files[i].Name);
                    if (File.Exists(files[i].FullName))
                    {
                        File.Delete(files[i].FullName);
                    }
                    if (File.Exists(files[i].FullName + ".manifest"))
                    {
                        File.Delete(files[i].FullName + ".manifest");
                    }
                }
            }
        }
        static void WriteData(Dictionary<string, string> resPathDic)
        {
            AssetManifest manifest = new AssetManifest();
            manifest.AssetLst = new List<AssetElement>();
            foreach (string path in resPathDic.Keys)
            {
                AssetElement element = new AssetElement();
                element.Path = path;
                //element.Crc = Crc32.GetCrc32(path);
                element.ABName = resPathDic[path];
                element.AssetName = path.Remove(0, path.LastIndexOf("/") + 1);
                element.DependAB = new List<string>();
                string[] resDependce = AssetDatabase.GetDependencies(path);
                for (int i = 0; i < resDependce.Length; i++)
                {
                    string tempPath = resDependce[i];
                                        
                    string abName = "";
                    if (mAllFileABNameDic.TryGetValue(tempPath, out abName))
                    {
                        if (abName == resPathDic[path])//依赖资源就在本包内 忽略   （其实就包含了过滤自己）
                            continue;

                        if (!element.DependAB.Contains(abName))
                        {
                            element.DependAB.Add(abName);
                        }
                    }
                }
                manifest.AssetLst.Add(element);
            }

            //写入xml
            string xmlPath = Application.dataPath + "/assetmanifest.xml";
            if (File.Exists(xmlPath)) File.Delete(xmlPath);
            FileStream fileStream = new FileStream(xmlPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
            XmlSerializer xs = new XmlSerializer(manifest.GetType());
            xs.Serialize(sw, manifest);
            sw.Close();
            fileStream.Close();

            //写入二进制
            //foreach (AssetElement abBase in manifest.AssetLst)
            //{
            //    abBase.Path = "";
            //}
            FileStream fs = new FileStream(mABBinaryPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            fs.Seek(0, SeekOrigin.Begin);
            fs.SetLength(0);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, manifest);
            fs.Close();
            AssetDatabase.Refresh();
            setABName("assetmanifest", mABBinaryPath);
        }

        /// <summary>
        /// 所有的包名内是否包含name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="abNames"></param>
        /// <returns></returns>
        static bool containABName(string name, string[] abNames)
        {
           
            for (int i = 0; i < abNames.Length; i++)
            {
                if (name.Equals(abNames[i]))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 是否包含在已经有的AB包里
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static bool containPathFilter(string path)
        {
            for (int i = 0; i < mABPathFilters.Count; i++)
            {   //单prefab包包含过                              //包含在文件夹下                     //确保剔除包含路径后以/开头 以保证是真的包含在文件夹下
                if (path.Equals(mABPathFilters[i]) || (path.Contains(mABPathFilters[i]) && (path.Replace(mABPathFilters[i], "")[0] == '/')))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 是否是有效资源路径 (会被主动加载的资源的路径)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static bool isAssetPath(string path)
        {
            for (int i = 0; i < mAssetFilters.Count; i++)
            {
                if (path.Contains(mAssetFilters[i]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}