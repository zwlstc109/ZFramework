using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{/// <summary>
/// Debug管理
/// </summary>  
    public class DebugManager : BaseManager
    {   
        protected override int MgrIndex { get { return (int)ManagerIndex.Log; } }
        //日志subject
        private readonly Subject<LogEntry> logSubject = new Subject<LogEntry>();
        private Logger mDefaultLogger;
        internal override void Init()
        {
            Z.Debug = this;
            //默认关联UnityDebug
           logSubject.Where(e => ReferenceEquals(e.LoggerName, "DefaultLogger")).Subscribe(new UnityDebugSink());//有个不好的地方 用默认日志输出器就不能双击console来到debug.log的地方了
            //默认日志输出器                                                     //但可以使用Debug.log 不强制使用Z.log   Z.log用来以后可以把输出整理到一个UI上 持久化到文件上 甚至上传到服务器之类的需求
            mDefaultLogger = new Logger("DefaultLogger");
         
            Z.Debug.Log("LogManager init");
        }

        public Action<LogEntry> RegisterLogger(Logger logger)
        {           
            return logSubject.OnNext;
        }
        /// <summary>Output LogType.Log but only enables isDebugBuild</summary>
        public void Debug(object message, UnityEngine.Object context = null)
        {
            mDefaultLogger.Debug(message);
        }

        /// <summary>Output LogType.Log but only enables isDebugBuild</summary>
        public void DebugFormat(string format, params object[] args)
        {
            mDefaultLogger.DebugFormat(format, args);
        }

        public virtual void Log(object message, UnityEngine.Object context = null)
        {
            mDefaultLogger.Log(message);
        }

        public virtual void LogFormat(string format, params object[] args)
        {
            mDefaultLogger.LogFormat(format,args);
        }

        public virtual void Warning(object message, UnityEngine.Object context = null)
        {
            mDefaultLogger.Warning(message);
        }

        public virtual void WarningFormat(string format, params object[] args)
        {
            mDefaultLogger.WarningFormat(format,args);
        }

        public virtual void Error(object message, UnityEngine.Object context = null)
        {
            mDefaultLogger.Error(message);
        }

        public virtual void ErrorFormat(string format, params object[] args)
        {
            mDefaultLogger.ErrorFormat(format, args);
        }

        public virtual void Exception(Exception exception, UnityEngine.Object context = null)
        {
            mDefaultLogger.Exception(exception);
        }
        internal override void MgrUpdate()
        {
           
        }

        internal override void ShutDown()
        {
            
        }
    }

    
}