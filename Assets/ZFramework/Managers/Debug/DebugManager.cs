using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{ 
    /// <summary>
    /// Debug管理
    /// </summary> 
    public sealed class DebugManager : BaseManager
    {   
        
        protected override int MgrIndex { get { return (int)ManagerIndex.Log; } }
        private bool mIsDebugBuild;
        //日志subject
        private readonly Subject<LogEntry> logSubject = new Subject<LogEntry>();//用法: 为需要接收日志消息的地方实现Iobserver<logEntry> 然后订阅这个subject  然后Z.debug.xxx 就相当于发送日志消息
        //默认日志
        private const string mDefaultLoggerName = "DefaultLogger";
        private DebugHub mHub;
       
        
        internal override void Init()
        {
            Z.Debug = this;
            mIsDebugBuild = UnityEngine.Debug.isDebugBuild;
            //默认关联UnityDebug where中可以自定义自己的过滤方式
            logSubject.Where(e => ReferenceEquals(e.LoggerName, mDefaultLoggerName)).Subscribe(new UnityDebugSink());//有个不好的地方 用默认日志输出器就不能双击console来到debug.log的地方了
                                                                                                                
            mHub = GameObject.Find("DebugHub").GetComponent<DebugHub>();
            Z.Debug.Log(mHub.GetInstanceID());
            //Z.Debug.Log("LogManager init");
        }

       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="context"></param>
        /// <param name="name">loggerName</param>
        public void Debug(object message, UnityEngine.Object context = null,string loggerName=mDefaultLoggerName)
        {
            if (mIsDebugBuild)
            {
                logSubject.OnNext(new LogEntry(
                    message: (message != null) ? message.ToString() : "",
                    logType: LogType.Log,
                    timestamp: DateTime.Now,
                    loggerName: loggerName,
                    context: context));
            }
        }

        /// <summary>Output LogType.Log but only enables isDebugBuild</summary>
        public void DebugFormat(string format, string loggerName = mDefaultLoggerName, params object[] args)
        {
            if (mIsDebugBuild)
            {
                logSubject.OnNext(new LogEntry(
                    message: (format != null) ? string.Format(format, args) : "",
                    logType: LogType.Log,
                    timestamp: DateTime.Now,
                    loggerName: loggerName,
                    context: null));
            }
        }

        public  void Log(object message, UnityEngine.Object context = null, string loggerName = mDefaultLoggerName)
        {
            logSubject.OnNext(new LogEntry(
               message: (message != null) ? message.ToString() : "",
               logType: LogType.Log,
               timestamp: DateTime.Now,
               loggerName: loggerName,
               context: context));
        }

        public  void LogFormat(string format, string loggerName = mDefaultLoggerName, params object[] args)
        {
            logSubject.OnNext(new LogEntry(
               message: (format != null) ? string.Format(format, args) : "",
               logType: LogType.Log,
               timestamp: DateTime.Now,
               loggerName: loggerName,
               context: null));
        }

        public  void Warning(object message, UnityEngine.Object context = null, string loggerName = mDefaultLoggerName)
        {
            logSubject.OnNext(new LogEntry(
                message: (message != null) ? message.ToString() : "",
                logType: LogType.Warning,
                timestamp: DateTime.Now,
                loggerName: loggerName,
                context: context));
        }

        public  void WarningFormat(string format, string loggerName = mDefaultLoggerName, params object[] args)
        {
            logSubject.OnNext(new LogEntry(
                 message: (format != null) ? string.Format(format, args) : "",
                 logType: LogType.Warning,
                 timestamp: DateTime.Now,
                 loggerName: loggerName,
                 context: null));
        }

        public  void Error(object message, UnityEngine.Object context = null, string loggerName = mDefaultLoggerName)
        {
            logSubject.OnNext(new LogEntry(
                message: (message != null) ? message.ToString() : "",
                logType: LogType.Error,
                timestamp: DateTime.Now,
                loggerName: loggerName,
                context: context));
        }

        public  void ErrorFormat(string format, string loggerName = mDefaultLoggerName, params object[] args)
        {
            logSubject.OnNext(new LogEntry(
                message: (format != null) ? string.Format(format, args) : "",
                logType: LogType.Error,
                timestamp: DateTime.Now,
                loggerName: loggerName,
                context: null));
        }

        public  void Exception(Exception exception, UnityEngine.Object context = null, string loggerName = mDefaultLoggerName)
        {
            logSubject.OnNext(new LogEntry(
                message: (exception != null) ? exception.ToString() : "",
                exception: exception,
                logType: LogType.Exception,
                timestamp: DateTime.Now,
                loggerName: loggerName,
                context: context));
        }
        internal override void MgrUpdate()
        {
            //if (Input.GetKeyDown(KeyCode.K))
            //{
            //    Hub.Show();
            //}
            //if (Input.GetKeyDown(KeyCode.L))
            //{
            //    Hub.Hide();
            //}
        }

        internal override void ShutDown()
        {
            
        }
    }

    
}