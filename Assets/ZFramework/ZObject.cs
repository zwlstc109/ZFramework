using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{
    
    public class ZObject
    {
        internal bool destroy = false;
        internal static int TopInstanceId = 0;
        public int InstanceId { get; internal set; } = TopInstanceId++;
        public void Destroy()
        {
            destroy = true;
        }
        public override string ToString()
        {
            if (destroy)
            {
                return "null";
            }
            return base.ToString();
        }
        public static bool operator !=(ZObject lhs, ZObject rhs)
        {
            if (System.Object.Equals(rhs, null))
            {
                if (System.Object.Equals(lhs, null))
                {
                    return false;
                }
                if (lhs.destroy)
                {
                    return false;
                }
            }
            return !ReferenceEquals(lhs, rhs);
        }
        public static bool operator ==(ZObject lhs, ZObject rhs)
        {

            if (System.Object.Equals(rhs, null))
            {
                if (System.Object.Equals(lhs, null))
                {
                    return true;
                }
                if (lhs.destroy)
                {
                    return true;
                }
            }
            return ReferenceEquals(lhs, rhs);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }

}
