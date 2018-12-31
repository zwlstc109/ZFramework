using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
namespace Zframework
{
    public static class ScreenAspect
    {
        [MenuItem("ZFramework/Print Device Aspect")]
        public static void PrintScreenAspect()
        {
            float curAscpect = GetAspect();
            foreach (var aspect in mAspectLst)
            {
                if (curAscpect.IsMatchRatio(aspect.lhs / aspect.rhs))
                {
                    Debug.LogFormat("当前设备分辨率 {0}:{1} ...是{2}\nW:{3} H:{4}", (int)aspect.lhs, (int)aspect.rhs, aspect.Device,Screen.width,Screen.height);
                    return;
                }
            }
            Debug.LogFormat("未知的设备分辨率...\nW:{0} H:{1}", Screen.width, Screen.height);
        }
        /// <summary>
        /// 获取屏幕分辨率 
        /// </summary>
        /// <returns></returns>
        public static float GetAspect()
        {
            //横屏/竖屏
            var isLandscape = Screen.width > Screen.height;//编辑器内的话 就是Game视图里选的值
            return isLandscape ? (float)Screen.width / Screen.height : (float)Screen.height / Screen.width;
        }

        /// <summary>
        /// 比例匹配
        /// </summary>
        /// <param name="f">源比例</param>
        /// <param name="dstRatio">目标比例</param>
        /// <param name="error">误差</param>
        /// <returns></returns>
        public static bool IsMatchRatio(this float f, float dstRatio, float error = 0.05f)
        {
            return (dstRatio - error) < f && f < (dstRatio + error);
        }

        struct Aspect
        {
            public float lhs;//分辨率左边的数
            public float rhs;//右
            public string Device;//设备
        }

        private static List<Aspect> mAspectLst = new List<Aspect>();//所有已知的分辨率类型
        static ScreenAspect()
        {
            mAspectLst.Add(new Aspect() {lhs=4.0f,rhs=3.0f,Device="Pad" });
            mAspectLst.Add(new Aspect() { lhs = 18.0f, rhs = 9.0f, Device = "Phone" });
            mAspectLst.Add(new Aspect() { lhs = 16.0f, rhs = 9.0f, Device = "Phone" });
            mAspectLst.Add(new Aspect() { lhs = 3.0f, rhs = 2.0f, Device = "Phone" });
            mAspectLst.Add(new Aspect() { lhs = 812.0f, rhs = 375.0f, Device = "IPhoneX 2436:1125" });
        }
       
    }
}
#endif