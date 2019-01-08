using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.SceneManagement;
namespace Zframework
{
    #region additional
    public enum FadeMode
    {
        None,
        FadeIn,
        FadeOut,
        FadeInOut,
        WaitingThenFadeOut
    }
    public class FadeData:ZObject
    {
        public FadeMode Mode;
        public Action FadeInDoneCallback;
        public Action FadeEnterCallback;
        public FadeData Fill(FadeMode mode, Action fadeInDoneAction,Action fadeEnterCallback=null)
        {
            Mode = mode;
            FadeInDoneCallback = fadeInDoneAction;
            FadeEnterCallback = fadeEnterCallback;
            return this;
        }
        public static void Clean(FadeData data)
        {
            data.Mode = default;
            data.FadeInDoneCallback = null;
            data.FadeEnterCallback = null;
        }
    }
    #endregion
    /// <summary>
    /// 场景管理 包括淡入淡出等幕布类功能
    /// </summary>
    public sealed class SceneManager : BaseManager //TODO 现在场景没有结合AB包
    {
        protected override int MgrIndex { get { return (int)ManagerIndex.Scene; } }
        public AsyncOperation Async { get; private set; }
        public float LoadingProgress { get; private set; }
        //public string SceneName { get; private set; }
        public bool Already { get; private set; }//同步加载时当场景包异步加载是否完毕使用; 异步加载时，当加载流程是否完毕使用
        private Action<object> mDoneCallback = null;
        public string LoadingUIPath = null;
        public string FadeUIPath = null;
        private IDisposable mFadeDispose;
        private bool FadeInDone = false;//当前淡入是否结束
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
        /// <param name="fadeInDoneCallback">淡入完成后执行</param>
        public void Fade(FadeMode mode,Action fadeInDoneCallback=null,Action fadeEnterCallback=null)
        {
            if (string.IsNullOrEmpty(FadeUIPath))
            {
                Z.Debug.Warning("请在Scene节点下设置渐变面板路径");
                return;
            }
            switch (mode)
            {
                case FadeMode.None:
                    fadeEnterCallback?.Invoke();
                    fadeInDoneCallback?.Invoke();
                    break;
                case FadeMode.FadeIn://淡入后 执行fadeInDone 随后释放幕布
                    Z.UI.Open(FadeUIPath, Z.UI.Top, userData:Z.Pool.Take<FadeData>().Fill(mode,fadeInDoneCallback, fadeEnterCallback));
                  
                    break;
                case FadeMode.FadeOut://淡出后 释放幕布
                    Z.UI.Open(FadeUIPath, Z.UI.Top, userData: Z.Pool.Take<FadeData>().Fill(mode, null));
                  
                    break;
                case FadeMode.FadeInOut://淡入后 执行fadeInDone 随后等待事件"Z_FadeOutAction"触发后淡出 最后释放幕布
                    Z.UI.Open(FadeUIPath, Z.UI.Top, userData: Z.Pool.Take<FadeData>().Fill(mode, fadeInDoneCallback, fadeEnterCallback));                  
                    break;
                case FadeMode.WaitingThenFadeOut://（等待实现可由面板重写) 等待事件"Z_FadeOutAction"触发后淡出 最后释放幕布
                    Z.UI.Open(FadeUIPath, Z.UI.Top, userData: Z.Pool.Take<FadeData>().Fill(mode, fadeInDoneCallback));
                    break;
                default:
                    Z.Debug.Warning("不存在的FadeMode");
                    break;
            }
        }
        /// <summary>
        /// 轻量级同步加载场景（没有进度条 但是可以用幕布遮盖）
        /// </summary>
        /// <param name="scenePath"></param>
        public void LoadScene(string scenePath,FadeMode mode=FadeMode.FadeInOut,Action loadedCallBack=null)
        {
            Already = false;
            FadeInDone = mode==FadeMode.WaitingThenFadeOut||mode==FadeMode.FadeOut||mode==FadeMode.None?true:false;
            _LoadSceneBundleAnsyc(scenePath,loadedCallBack/*,mode*/);
          
            Fade(mode,()=> {
                FadeInDone = true;
                if (Already)/*//下面异步加载结束时也会类似的判断FadeInDone  相当于两个并发事件醒来时，两个标志位都为true 才会执行*/
                {
                    _DoLoadScene(scenePath, loadedCallBack);
                }          
            });
        }
       /// <summary>
       /// 读条式加载场景 适用于额外附带大量动态加载资源的重型场景切换
       /// </summary>
       /// <param name="scenePath"></param>
       /// <param name="doneCallback"></param>
       /// <param name="fadeIn"></param>
       /// <param name="fadeOut"></param>
       /// <param name="userData"></param>
        public void LoadSceneAsync(string scenePath,Action<object> doneCallback=null,bool fadeIn=true,bool fadeOut=true, object userData=null)
        {
            Already = false;
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
          
            //现在clearCache在下面这个方法内调用      //   TODO 可能这前面还要加上加载其他资源的代码 先加再减避免重复加载 等于是要再前面封装一个 一股脑加载资源的进度输出
            _LoadSceneAB(scenePath);
            string sceneName = Z.Str.GetAssetNoExtensionName(scenePath);
            AsyncOperation asyncScene = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            asyncScene.allowSceneActivation = false;
                
            if (asyncScene != null && !asyncScene.isDone)
            {
                while (asyncScene.progress < 0.9f)
                {
                    targetProgress = (int)asyncScene.progress * 100;//没毛病 如果想用time.deltaTime还不是得乘法 这种反而算是技巧
                    yield return null;
                    yield return StartCoroutine(_WaitProgress(targetProgress));
                }
                asyncScene.allowSceneActivation = true;
                //Z.Debug.Log(asyncScene.progress);
                //自行加载剩余的10%
                targetProgress = 100;
                while (LoadingProgress < targetProgress - 2)
                {
                    ++LoadingProgress;
                    //Z.Debug.Log(asyncScene.progress);
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
        /// 平滑追踪读条值
        /// </summary>
        /// <param name="progress">目标值</param>
        /// <returns></returns>
        IEnumerator _WaitProgress(int progress)
        {
            while (LoadingProgress<progress)
            {
                ++LoadingProgress;
                yield return null;
            }
        } 
        /// <summary>
        /// 通过Z.Resource找到ResItem 并调用AssetBundleManager的加载AB包方法
        /// </summary>
        /// <param name="scenePath"></param>
       
        private void _LoadSceneAB(string scenePath)
        {
            if (Z.Resource.LoadFromAssetBundle)
            {
                ResourceItem resItem = Z.Resource.ResourceItemDic[scenePath];
                if (resItem.AssetBundle == null)
                {
                    if (AssetBundleManager.LoadAssetBundle(resItem))
                    {
                        ClearCache();
                        Z.Resource.IncreaseRefCount(resItem, 0);
                    }
                }
            }
            else
                ClearCache();
        }
        /// <summary>
        /// 异步加载场景包
        /// </summary>
        /// <param name="scenePath"></param>
        ///
        private void _LoadSceneBundleAnsyc(string scenePath,Action loadedCallback/*,FadeMode mode*/)
        {
            
            if (Z.Resource.LoadFromAssetBundle)
            {
                ResourceItem resItem = Z.Resource.ResourceItemDic[scenePath];
                if (!resItem.AnsycLoaded)
                {
                    AssetBundleManager.LoadAssetBundleAsync(resItem);
                    AssetBundleManager.GetAssetBundleLoadedSubject(resItem).Subscribe(_ =>
                    {
                        if (FadeInDone)//如果渐变已经完成 
                            _DoLoadScene(scenePath, loadedCallback);
                       
                        Already = true;//标识已经加载完毕
                    });
                }
            }
            //else
            //    ClearCache();
        }
        private void _DoLoadScene(string scenePath,Action loadedCallback)
        {
            string sceneName = Z.Str.GetAssetNoExtensionName(scenePath);
            ClearCache();//只要保证执行在 增加引用计数的函数前就行 因为是在这个函数里把场景加入了资源组0 
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            if (Z.Resource.LoadFromAssetBundle)
                Z.Resource.IncreaseRefCount(Z.Resource.ResourceItemDic[scenePath], 0);
            loadedCallback?.Invoke();
            Z.Subject.Fire("Z_FadeOutAction", null);
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