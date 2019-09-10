// Copyright (c) 2019 Timothé Lapetite - nebukam@gmail.com.
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

using System.Collections.Generic;

namespace Nebukam.Collections
{
    public class ListDictionary<TKey, TValue> : Pooling.PoolItem, Pooling.IRequireCleanUp
    {

        protected List<TKey> m_keyList = new List<TKey>();
        protected Dictionary<TKey, List<TValue>> m_dictionary = new Dictionary<TKey, List<TValue>>();
        
        public int KeyCount { get { return m_dictionary.Count; } }

        public int ValueCount(TKey key)
        {
            List<TValue> entries;
            if (m_dictionary.TryGetValue(key, out entries))
                return entries.Count;

            return 0;
        }

        public TValue Add(TKey key, TValue value)
        {
            List<TValue> entries;
            if (m_dictionary.TryGetValue(key, out entries))
                entries.AddOnce(value);
            else
                m_dictionary[key] = new List<TValue>() { value };

            return value;
        }

        public bool Remove(TKey key, TValue value)
        {
            List<TValue> entries;
            if (m_dictionary.TryGetValue(key, out entries))
            {
                if (entries.Remove(value))
                {
                    if (entries.Count == 0)
                        m_dictionary.Remove(key);

                    return true;
                }
            }

            return false;
        }

        public bool Remove(TKey key)
        {
            List<TValue> entries;
            if (m_dictionary.TryGetValue(key, out entries))
            {
                entries.Clear();
                m_dictionary.Remove(key);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CleanUp()
        {
            int count = m_keyList.Count;
            for(int i = 0; i < count; i++)
                m_dictionary[m_keyList[i]].Clear();

            m_keyList.Clear();
            m_dictionary.Clear();
        }

    }
}
