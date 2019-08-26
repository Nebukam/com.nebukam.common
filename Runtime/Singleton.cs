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


namespace Nebukam
{

    internal interface ISingleton
    {
        void InternalInit();
    }

    public abstract class Singleton<T> : ISingleton
        where T : class, new()
    {

        public static void StaticInitialize() { T i = Get; }

        private static T m_instance = null;

        public static T Get
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new T();

                    ISingleton i = m_instance as ISingleton;
                    i.InternalInit();
                }
                return m_instance;
            }
        }

        private bool m_disposing = false;
        protected bool m_init = false;
        void ISingleton.InternalInit()
        {
            if (m_init) { return; }
            Init();

            Static.onUpdate(Update);
            Static.onLateUpdate(LateUpdate);
            Static.onFixedUpdate(FixedUpdate);
            Static.onQuit(OnApplicationQuit);

            m_init = true;
        }

        protected abstract void Init();

        protected virtual void Update() { }
        protected virtual void LateUpdate() { }
        protected virtual void FixedUpdate() { }
        protected virtual void OnApplicationQuit()
        {
            Dispose();
        }

        #region System.IDisposable

        protected virtual void Dispose(bool disposing)
        {

            if (!disposing) { return; }

            Static.offUpdate(Update);
            Static.offLateUpdate(LateUpdate);
            Static.offFixedUpdate(FixedUpdate);
            Static.offQuit(OnApplicationQuit);

        }

        public void Dispose()
        {
            if (m_disposing) { return; }
            m_disposing = true;
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            System.GC.SuppressFinalize(this);
        }

        #endregion

    }
}
