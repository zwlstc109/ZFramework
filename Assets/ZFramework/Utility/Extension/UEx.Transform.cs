using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{
    //Transform的扩展
    public static partial class UnityExtension
    {
        #region 值设置
        public static void SetLocalPositionX(this Transform trans, float x)
        {
            var localPos = trans.localPosition;
            localPos.x = x;
            trans.localPosition = localPos;
        }
        public static void SetLocalPositionY(this Transform trans, float y)
        {
            var localPos = trans.localPosition;
            localPos.y = y;
            trans.localPosition = localPos;
        }
        public static void SetLocalPositionZ(this Transform trans, float z)
        {
            var localPos = trans.localPosition;
            localPos.z = z;
            trans.localPosition = localPos;
        }
        public static void SetLocalPositionXY(this Transform trans, float x, float y)
        {
            var localPos = trans.localPosition;
            localPos.x = x;
            localPos.y = y;
            trans.localPosition = localPos;
        }
        public static void SetLocalPositionXZ(this Transform trans, float x, float z)
        {
            var localPos = trans.localPosition;
            localPos.x = x;
            localPos.z = z;
            trans.localPosition = localPos;
        }
        public static void SetLocalPositionYZ(this Transform trans, float y, float z)
        {
            var localPos = trans.localPosition;
            localPos.y = y;
            localPos.z = z;
            trans.localPosition = localPos;
        }
        public static void SetLocalEulerAngleX(this Transform trans, float x)
        {
            var localEuler = trans.localEulerAngles;
            localEuler.x = x;
            trans.localEulerAngles = localEuler;
        }
        public static void SetLocalEulerAngleY(this Transform trans, float y)
        {
            var localEuler = trans.localEulerAngles;
            localEuler.y = y;
            trans.localEulerAngles = localEuler;
        }
        public static void SetLocalEulerAngleZ(this Transform trans, float z)
        {
            var localEuler = trans.localEulerAngles;
            localEuler.z = z;
            trans.localEulerAngles = localEuler;
        }
        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="outOfView">是否设置于视野外</param> 
        public static void Reset(this Transform trans, bool outOfView = false)
        {                                       //暂时不知道这个设置最大值会不会导致bug...
            Vector3 pos = outOfView ? new Vector3(float.MaxValue, float.MaxValue, float.MaxValue) : Vector3.zero;
            trans.localPosition = pos;
            trans.localRotation = Quaternion.identity;
            trans.localScale = Vector3.one;
        }
        #endregion

        #region 显示隐藏
        public static void Hide(this Component c)
        {
            c.gameObject.SetActive(false);
        }
        public static void LoadUnit(this Component c)
        {
            c.gameObject.SetActive(true);
        }
        public static void Hide(this GameObject go)
        {
           go.SetActive(false);
        }
        public static void Show(this GameObject go)
        {
            go.SetActive(true);
        }
        #endregion
    }
}