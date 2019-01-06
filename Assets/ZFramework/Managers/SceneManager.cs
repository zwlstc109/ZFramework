using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.SceneManagement;
namespace Zframework
{  
    public enum FadeMode
    {
        None,
        FadeIn,
        FadeOut,
        FadeInOut
    }
    public class FadeData:ZObject
    {
        public FadeMode Mode;
        public Action FadeInCallback;
        public FadeData Fill(FadeMode mode, Action fadeInAction)
        {
            Mode = mode;
            FadeInCallback = fadeInAction;
            return this;
        }
        public static void Clean(FadeData data)
        {
            data.Mode = default;
            data.FadeInCallback = null;
        }
    }
    /// <summary>
    /// 场景管理 包括淡入淡出等幕布类功能
    /// </summary>
    public class SceneManager : BaseManager //TODO 现在场景没有结合AB包
    {
        protected override int MgrIndex { get { return (int)ManagerIndex.Scene; } }
        public AsyncOperation Async { get; private set; }
        public float LoadingProgress { get; private set; }
        //public string SceneName { get; private set; }
        public bool Already { get; private set; }
        private Action<object> mDoneCallback = null;
        public string LoadingUIPath = null;
        public string FadeUIPath = null;
        private IDisposable mFadeDispose;
        internal override void Init()
        {
            //Z.Debug.Log("SceneManager init");
            Z.Scene = this;
            Z.Pool.RegisterClassCustomPool(() => new FadeData(), FadeData.Clean, 2);
        }
        /// <summary>
        /// 开启一个黑幕渐变 (期间锁定所有UI响应)
        /// </summary>
        /// <param name="mode">渐变模式</param>
        /// <param name="fadeInCallback">淡入完成后执行</param>
        public void Fade(FadeMode mode,Action fadeInCallback=null)
        {
            if (string.IsNullOrEmpty(FadeUIPath))
            {
                Z.Debug.Warning("请在Scene节点下设置渐变面板路径");
                return;
            }
            switch (mode)
            {
                case FadeMode.None:
                    fadeInCallback?.Invoke();
                    break;
                case FadeMode.FadeIn://淡入后 执行fadeInDone 随后释放幕布
                    Z.UI.Open(FadeUIPath, Z.UI.Top, userData:Z.Pool.Take<FadeData>().Fill(mode,fadeInCallback));
                  
                    break;
                case FadeMode.FadeOut://淡出后 释放幕布
                    Z.UI.Open(FadeUIPath, Z.UI.Top, userData: Z.Pool.Take<FadeData>().Fill(mode, null));
                  
                    break;
                case FadeMode.FadeInOut://淡入后 执行fadeInDone 随后等待事件"Z_FadeOutAction"触发后淡出 最后释放幕布
                    Z.UI.Open(FadeUIPath, Z.UI.Top, userData: Z.Pool.Take<FadeData>().Fill(mode, fadeInCallback));                  
                    break;
                default:
                    Z.Debug.Warning("不存在的FadeMode");
                    break;
            }
        }
        /// <summary>
        /// 同步加载一个场景 
        /// </summary>
        /// <param name="scenePath"></param>
        public void LoadScene(string scenePath,FadeMode mode,Action loadedCallBack=null)
        {
            string sceneName = Z.Str.GetAssetNoExtensionName(scenePath);
            Fade(mode, () =>
            {
                //先加载场景AB包
                ClearCache();
                _LoadSceneAB(scenePath);
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
                loadedCallBack?.Invoke();
                if (mode == FadeMode.FadeInOut)
                    Z.Subject.Fire("Z_FadeOutAction", null);
            });
                  
        }
       

        public void LoadSceneAsync(string scenePath,Action<object> doneCallback=null,bool fadeIn=true,bool fadeOut=true, object userData=null)
        {
            //先加载场景AB包
          
            if (fadeIn)
            {
                Fade(FadeMode.FadeIn,() => _LoadSceneAsync(scenePath, doneCallback, fadeOut, userData));   
            }
            else
                _LoadSceneAsync(scenePath, doneCallback,fadeIn, userData);
        }
        private void _LoadSceneAsync(string scenePath, Action<object> doneCallback ,bool fade ,object userData = null)
        {
            mDoneCallback = doneCallback;
            StartCoroutine(LoadAsync(scenePath, userData,fade));
            Z.UI.Open(LoadingUIPath, Z.UI.Top);
        }
        public void SetLoadScenePath(string path)
        {
            
        }
        IEnumerator LoadAsync(string scenePath, object userData,bool fade)
        {
            LoadingProgress = 0;
            int targetProgress = 0;
            Already = false;
            //加载场景包前先清0组缓存
            ClearCache();//TODO 可能这前面还要加上加载其他资源的代码 先加再减避免重复加载 等于是要再前面封装一个 一股脑加载资源的进度输出
            _LoadSceneAB(scenePath);
            string sceneName = Z.Str.GetAssetNoExtensionName(scenePath);
            AsyncOperation asyncScene = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            asyncScene.allowSceneActivation = false;
                
            if (asyncScene != null && !asyncScene.isDone)
            {

                while (asyncScene.progress < 0.9f)
                {
                    targetProgress = (int)asyncScene.progress * 100;//TODO 
                    yield return null;
                    //平滑过渡
                    while (LoadingProgress < targetProgress)
                    {
                        ++LoadingProgress;
                        yield return null;
                    }

                }
                asyncScene.allowSceneActivation = true;

                //自行加载剩余的10%
                targetProgress = 100;
                while (LoadingProgress < targetProgress - 2)
                {
                    ++LoadingProgress;
                    yield return null;
                }
                LoadingProgress = 100;
                Already = true;
                mDoneCallback?.Invoke(userData);
                mDoneCallback = null;
                if (fade)
                    Fade(FadeMode.FadeOut);

            }
          
        }
        /// <summary>
        /// 通过Z.Resource找到ResItem 并调用AssetBundleManager的加载AB包方法
        /// </summary>
        /// <param name="scenePath"></param>
        public void _LoadSceneAB(string scenePath)
        {
            if (Z.Resource.LoadFromAssetBundle)
            {
                ResourceItem resItem = Z.Resource.ResourceItemDic[scenePath];
                if (resItem.AssetBundle == null)
                {
                    if (AssetBundleManager.LoadResourceAB(resItem))
                    {
                        Z.Resource.IncreaseRefCount(resItem, 1);//TODO 暂时将场景ResItem放到预加载组
                    }
                }
            }
        }
        //转场清理默认资源组
        private void ClearCache()
        {
            Z.Resource.Release(0);
            Z.Unit.Release(0);
        }

        internal override void MgrUpdate()
        {
           
        }

        internal override void ShutDown()
        {
           
        }
    }
}