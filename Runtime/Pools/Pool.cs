﻿// Copyright (c) 2019 Timothé Lapetite - nebukam@gmail.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
#if UNITY_EDITOR
using System.Linq;
#endif
using System.Collections.Generic;

namespace Nebukam.Pooling
{

    public static class Pool
    {

        public delegate void OnItemReturned(IPoolItem item);
        private static Dictionary<Type, IPool> m_pools = new Dictionary<Type, IPool>();

        internal static Pool<T> GetPool<T>(Type type)
            where T : PoolItem, IPoolNode, new()
        {
            Pool<T> pool;
            IPool result;
            if (!m_pools.TryGetValue(type, out result))
            {
                pool = new Pool<T>();
                result = pool;
                m_pools.Add(type, pool);
            }
            else
            {
                pool = result as Pool<T>;
            }

            return pool;
        }

        public static bool Return<T>(T item)
            where T : PoolItem, new()
        {
            IPool pool;
            if (!m_pools.TryGetValue(typeof(T), out pool))
                return false;
            else
                return pool.Return(item);
        }

        public static T Rent<T>()
            where T : PoolItem, new()
        {
            return GetPool<T>(typeof(T)).Rent();
        }

        public static void Preload<T>(int count)
            where T : PoolItem, new()
        {
            GetPool<T>(typeof(T)).Preload(count);
        }

        internal static bool ReturnNode(IPoolNode node)
        {
            IPool pool;
            if (!m_pools.TryGetValue(node.GetType(), out pool))
                return false;
            else
                return pool.Return(node);
        }

        /// <summary>
        /// Flushes all pools and force a pass of garbage collection.
        /// Make sure you know what you're doing.
        /// </summary>
        public static void Flush()
        {

        }

#if UNITY_EDITOR

        public static void PrintStats()
        {
            string stats = "";
            KeyValuePair<Type, IPool>[] types = m_pools.ToArray();
            KeyValuePair<Type, IPool> kvp;
            for (int i = 0, count = types.Length; i < count; i++)
            {
                kvp = types[i];
                stats += String.Format("Pool<{0}> : {1} available, {2} created.\n", kvp.Key.Name, kvp.Value.poolSize, kvp.Value.newTicker);
            }
            UnityEngine.Debug.Log(stats);
        }

#endif

    }

    internal interface IPool
    {
        int poolSize { get; }
        int newTicker { get; }
        bool Return(IPoolNode node);
    }

    internal class Pool<T> : IPool
        where T : PoolItem, IPoolNode, new()
    {

        internal int m_poolSize = 0;
        internal int m_newTicker = 0;

        internal T m_tail = null;

        int IPool.poolSize { get{ return m_poolSize; } }
        int IPool.newTicker { get { return m_newTicker; } }

        internal void Preload(int count)
        {

            if(m_tail == null)
            {
                m_tail = new T();
                m_poolSize++;
            }

            T preloaded;
            while(m_poolSize != count)
            {
                preloaded = new T();
                preloaded.__prevNode = m_tail;
                m_tail = preloaded;
            }
        }

        internal T Rent()
        {

            T item;
            IPoolItemEx ex;

            if (m_tail != null)
            {
                item = m_tail;
                m_tail = m_tail.__prevNode as T;
                item.__prevNode = null;
                item.__returned = false;
                m_poolSize--;

                ex = item as IPoolItemEx;
                if (ex != null) { ex.Init(); }

                return item;
            }

            item = new T();
            ex = item as IPoolItemEx;
            if (ex != null) { ex.Init(); }

            m_newTicker++;
            return item;
        }

        bool IPool.Return(IPoolNode node)
        {
            T nAsT = node as T;
            if(nAsT == null) { return false; }
            return Return(nAsT);
        }

        internal bool Return(T node)
        {
            if (node.__returned) { return false; }
            node.__returned = true;

            List<Pool.OnItemReturned> list = node.__onReturn;
            if(list != null)
            {
                for(int i = 0, count = list.Count; i < count; i++)
                    list[i](node);
                
                list.Clear();
            }

            if (m_tail != null)
                node.__prevNode = m_tail;

            m_tail = node;
            m_poolSize++;
            return true;
        }

    }
}
