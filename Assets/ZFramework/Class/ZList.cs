using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{
    
    public class ZList<T>:IList<T> where T:ZObject //考虑一种永远不删除元素的列表 0GC 
    {
        public IList<T> mList;

        public int Count => throw new NotImplementedException();

        

        public bool IsReadOnly => throw new NotImplementedException();

        public T this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        private readonly int mFindAmountInOneFrame = 10;//一帧内遍历的个数
        private int mAvailableIndex;

        IEnumerator _FindNullFromStart()
        {
            int findcount = 0;
            for (int i = 0; i < mList.Count; i++)
            {
                if (++findcount== mFindAmountInOneFrame)
                {
                    findcount = 0;
                    yield return null;
                }
                if (mList[i]==null)
                {
                    mAvailableIndex = i;
                    break;
                }
            }
            mAvailableIndex = -1;
        }
        IEnumerator _FindNullFromLast()//其实感觉同时从头尾没有什么卵用 一帧内还是固定的迭代次数 
        {
            int findcount = 0;
            for (int i = mList.Count-1; i >=0; i--)
            {
                if (++findcount == mFindAmountInOneFrame)
                {
                    findcount = 0;
                    yield return null;
                }
                if (mList[i] == null)
                {
                    mAvailableIndex = i;
                    break;
                }
            }
            mAvailableIndex = -1;
        }
        public ZList(IList<T> lst)
        {
            mList = lst;
        }

        public int IndexOf(T item)
        {
            return mList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
           
        }

        public void RemoveAt(int index)
        {
            
            return;
        }

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}