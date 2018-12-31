using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{
    
    public class Logger
    {
        static bool isInitialized = false;
        static bool isDebugBuild = false;

        public string Name { get; private set; }
        protected readonly Action<LogEntry> mLogPublisher;

        public Logger(string loggerName)
        {
            Name = loggerName;
            mLogPublisher = Z.Log.RegisterLogger(this);
        }

        /// <summary>Output LogType.Log but only enables isDebugBuild</summary>
        public virtual void Debug(object message, UnityEngine.Object context = null)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                isDebugBuild = UnityEngine.Debug.isDebugBuild;
            }

            if (isDebugBuild)
            {
                mLogPublisher(new LogEntry(
                    message: (message != null) ? message.ToString() : "",
                    logType: LogType.Log,
                    timestamp: DateTime.Now,
                    loggerName: Name,
                    context: context));
            }
        }

        /// <summary>Output LogType.Log but only enables isDebugBuild</summary>
        public virtual void DebugFormat(string format, params object[] args)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                isDebugBuild = UnityEngine.Debug.isDebugBuild;
            }

            if (isDebugBuild)
            {
                mLogPublisher(new LogEntry(
                    message: (format != null) ? string.Format(format, args) : "",
                    logType: LogType.Log,
                    timestamp: DateTime.Now,
                    loggerName: Name,
                    context: null));
            }
        }

        public virtual void Log(object message, UnityEngine.Object context = null)
        {
            mLogPublisher(new LogEntry(
                message: (message != null) ? message.ToString() : "",
                logType: LogType.Log,
                timestamp: DateTime.Now,
                loggerName: Name,
                context: context));
        }

        public virtual void LogFormat(string format, params object[] args)
        {
            mLogPublisher(new LogEntry(
                message: (format != null) ? string.Format(format, args) : "",
                logType: LogType.Log,
                timestamp: DateTime.Now,
                loggerName: Name,
                context: null));
        }

        public virtual void Warning(object message, UnityEngine.Object context = null)
        {
            mLogPublisher(new LogEntry(
                message: (message != null) ? message.ToString() : "",
                logType: LogType.Warning,
                timestamp: DateTime.Now,
                loggerName: Name,
                context: context));
        }

        public virtual void WarningFormat(string format, params object[] args)
        {
            mLogPublisher(new LogEntry(
                message: (format != null) ? string.Format(format, args) : "",
                logType: LogType.Warning,
                timestamp: DateTime.Now,
                loggerName: Name,
                context: null));
        }

        public virtual void Error(object message, UnityEngine.Object context = null)
        {
            mLogPublisher(new LogEntry(
                message: (message != null) ? message.ToString() : "",
                logType: LogType.Error,
                timestamp: DateTime.Now,
                loggerName: Name,
                context: context));
        }

        public virtual void ErrorFormat(string format, params object[] args)
        {
            mLogPublisher(new LogEntry(
                message: (format != null) ? string.Format(format, args) : "",
                logType: LogType.Error,
                timestamp: DateTime.Now,
                loggerName: Name,
                context: null));
        }

        public virtual void Exception(Exception exception, UnityEngine.Object context = null)
        {
            mLogPublisher(new LogEntry(
                message: (exception != null) ? exception.ToString() : "",
                exception: exception,
                logType: LogType.Exception,
                timestamp: DateTime.Now,
                loggerName: Name,
                context: context));
        }

        ///// <summary>Publish raw LogEntry.</summary>
        //public virtual void Raw(LogEntry logEntry)
        //{
        //    mLogPublisher(logEntry);
        //}
    }
}