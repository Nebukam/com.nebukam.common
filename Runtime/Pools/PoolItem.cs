// Copyright (c) 2019 Timothé Lapetite - nebukam@gmail.com
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

namespace Nebukam.Pooling
{

    public interface IPoolItem
    {
        void Release();
    }

    internal interface IPoolItemEx
    {
        void Init();
    }

    internal interface IPoolNode : IPoolItem
    {
        IPoolNode __prevNode { get; set; }
        bool __released { get; set; }
    }
    
    public abstract class PoolItem : IPoolNode
    {

        internal List<Pool.OnItemReturned> __onRelease = null;
        IPoolNode IPoolNode.__prevNode { get; set; } = null;
        bool IPoolNode.__released { get; set; } = false;

        public virtual void Release()
        {
            Pool.ReturnNode(this);
        }

        public static IPoolItem operator +(PoolItem item, Pool.OnItemReturned returnDelegate)
        {

            if ((item as IPoolNode).__released) { return item; }

            List<Pool.OnItemReturned> list = item.__onRelease;

            if (list == null)
            {
                list = new List<Pool.OnItemReturned>();
                item.__onRelease = list;
            }
            
            if(!list.Contains(returnDelegate))
                list.Add(returnDelegate);

            return item;

        }

        public static IPoolItem operator -(PoolItem item, Pool.OnItemReturned returnDelegate)
        {

            if ((item as IPoolNode).__released) { return item; }

            List<Pool.OnItemReturned> list = item.__onRelease;

            if (list == null)
                return item;

            int index = list.IndexOf(returnDelegate);
            if (index != -1)
                list.RemoveAt(index);

            return item;

        }

        public static implicit operator bool(PoolItem item)
        {
            return !(item as IPoolNode).__released;
        }

    }
    
    public abstract class PoolItemEx : PoolItem, IPoolItemEx
    {
        public override void Release() { if (Pool.ReturnNode(this)) { CleanUp(); } }
        public abstract void Init();
        protected abstract void CleanUp();
    }


}
