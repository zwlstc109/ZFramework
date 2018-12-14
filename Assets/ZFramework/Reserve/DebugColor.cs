using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Zframework
{
    public enum LogColor
    {
        black,
        yellow,
        pink,
        blue,
        orange
    }
    public static class ZLog
    {
        public static readonly string colorBlack = "#000000"; 
        public static readonly string colorYellow = "#FFFF00";
        public static readonly string colorPink = "#FF00FF";
        public static readonly string colorBlue = "#00FFFF";
        public static readonly string colorOrange = "#FF7812";

        public static void Log(string content,LogColor color=LogColor.pink)//建议使用粉色..这个颜色在白底的编辑器下还算看的清楚...算是多一个颜色把
        {
            string colorStr="";
            switch (color)
            {
                case LogColor.black:
                    colorStr = colorBlack;
                    break;
                case LogColor.yellow:
                    colorStr = colorYellow;
                    break;
                case LogColor.pink:
                    colorStr = colorPink;
                    break;
                case LogColor.blue:
                    colorStr = colorBlue;
                    break;
                case LogColor.orange:
                    colorStr = colorOrange;
                    break;
            }
            Debug.LogFormat("<color={0}>{1}</color>", colorStr, content);
        }
        public static void LogFormat(string format,params object[] args)
        {
            string tempStr = string.Format(format, args);
            Log(tempStr);
        }
    }
}