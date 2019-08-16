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

using UnityEngine;

namespace Nebukam
{
    public abstract class SingletonBehaviour<T> : MonoBehaviour, ISingleton, System.IDisposable
        where T : Component, new()
    {

        public static void StaticInitialize() { T i = Get; }

        private static T m_instance = null;
        private static GameObject m_instanceGameObject = null;
        
        public static T Get {
            get {
                if(m_instance == null)
                {
                    m_instanceGameObject = new GameObject();
                    m_instanceGameObject.name = typeof(T).Name;
                    m_instance = m_instanceGameObject.AddComponent<T>();

                    ISingleton i = m_instance as ISingleton;
                    i.InternalInit();
                }
                return m_instance;
            }
        }

        protected bool m_init = false;
        void ISingleton.InternalInit()
        {
            if (m_init) { return; }
            Init();
            m_init = true;
        }

        protected abstract void Init();

        private void Update(){ Tick(Time.deltaTime); }
        protected abstract void Tick(float delta);

        private void LateUpdate() { LateTick(Time.deltaTime); }
        protected abstract void LateTick(float delta);

        private void FixedUpdate() { FixedTick(); }
        protected abstract void FixedTick();

        private void OnDestroy() { Dispose(); }

        #region System.IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) { return; }

            if(m_instance == this)
            {
                if (m_instanceGameObject != null)
                {
                    Destroy(m_instanceGameObject);
                    m_instanceGameObject = null;
                }
                m_instance = null;
            }
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            System.GC.SuppressFinalize(this);
        }
        
        #endregion

    }
}
