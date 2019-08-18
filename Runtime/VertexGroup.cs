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

using Nebukam.Pooling;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Nebukam
{

    public interface IVertexGroup
    {
        
        List<IVertex> vertices { get; }
        int Count { get; }
        IVertex this[int index] { get; }
        int this[IVertex v] { get; }
        
        /// <summary>
        /// Adds a vertex in the group.
        /// </summary>
        /// <param name="v">The vertex to be added.</param>
        /// <param name="ownVertex">Whether or not this group gets ownership over the vertex.</param>
        /// <returns></returns>
        IVertex Add(IVertex v);

        /// <summary>
        /// Create a vertex in the group, from a Vector3.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        IVertex Add(float3 v);
        
        /// <summary>
        /// Inserts a vertex at a given index in the group.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="v"></param>
        /// <param name="ownVertex"></param>
        /// <param name="allowProxy"></param>
        /// <returns></returns>
        IVertex Insert(int index, IVertex v);

        /// <summary>
        /// Create a vertex in the group at the given index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        IVertex Insert(int index, float3 v);

        /// <summary>
        /// Removes a given vertex from the group.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="keepProxies"></param>
        /// <returns></returns>
        IVertex Remove(IVertex v, bool release = false);

        /// <summary>
        /// Removes the vertex at the given index from the group .
        /// </summary>
        /// <param name="index"></param>
        /// <param name="keepProxies"></param>
        /// <returns></returns>
        IVertex RemoveAt(int index, bool release = false);

        /// <summary>
        /// Inverse vertices's order
        /// </summary>
        void Reverse();

        IVertex Shift(bool release = false);

        IVertex Pop(bool release = false);

        void Clear(bool release = false);

        void Offset(float3 offset);

        #region Nearest vertex in group

        /// <summary>
        /// Return the vertex index in group of the nearest IVertex to a given IVertex v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        int GetNearestVertexIndex(IVertex v);

        /// <summary>
        /// Return the the nearest IVertex in group to a given IVertex v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        IVertex GetNearestVertex(IVertex v);

        /// <summary>
        /// Return the vertex index in group of the nearest IVertex to a given v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        int GetNearestVertexIndex(float3 v);

        /// <summary>
        /// Return the nearest IVertex in group of the nearest IVertex to a given v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        IVertex GetNearestVertex(float3 v);


        #endregion

    }

    public class VertexGroup<V> : Pooling.PoolItem, IVertexGroup
        where V : Vertex, IVertex, new()
    {

        protected Pooling.Pool.OnItemReleased m_onVertexReleasedCached;

        protected bool m_locked = false;
        public bool locked { get { return m_locked; } }

        protected List<IVertex> m_vertices = new List<IVertex>();
        public List<IVertex> vertices { get { return m_vertices; } }

        public int Count { get { return m_vertices.Count; } }
        
        public IVertex this[int index] { get { return m_vertices[index]; } }
        public int this[IVertex v] { get { return m_vertices.IndexOf(v); } }
        
        public VertexGroup()
        {
            m_onVertexReleasedCached = OnVertexReleased;
        }


        /// <summary>
        /// Adds a vertex in the group.
        /// </summary>
        /// <param name="v">The vertex to be added.</param>
        /// <param name="ownVertex">Whether or not this group gets ownership over the vertex.</param>
        /// <returns></returns>
        public IVertex Add(IVertex v)
        {
            if (m_vertices.Contains(v)) { return null; }
            m_vertices.Add(v);
            OnVertexAdded(v as V);
            return v;
        }

        /// <summary>
        /// Create a vertex in the group, from a Vector3.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public virtual IVertex Add(float3 v)
        {
            IVertex vert = Pooling.Pool.Rent<V>();
            vert.pos = v;
            return Add(vert);
        }
        
        /// <summary>
        /// Inserts a vertex at a given index in the group.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="v"></param>
        /// <param name="ownVertex"></param>
        /// <param name="allowProxy"></param>
        /// <returns></returns>
        public IVertex Insert(int index, IVertex v)
        {
            IVertex vertex = Add(v);

            if (vertex == null)
                return null;

            int currentIndex = m_vertices.IndexOf(v);
            if(currentIndex == index) { return v; }
            if (currentIndex != -1)
            {
                m_vertices.RemoveAt(currentIndex);
                if(currentIndex < index)
                    m_vertices.Insert(index-1, v);
                else
                    m_vertices.Insert(index, v);
            }
            else
            {
                m_vertices.Insert(index, v);
                OnVertexAdded(v as V);
            }
            return v;
        }

        /// <summary>
        /// Create a vertex in the group at the given index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public virtual IVertex Insert(int index, float3 v)
        {
            IVertex vert = Pooling.Pool.Rent<V>();
            vert.pos = v;

            m_vertices.Insert(index, vert);
            OnVertexAdded(v as V);
            return vert;
        }

        /// <summary>
        /// Removes a given vertex from the group.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="keepProxies"></param>
        /// <returns></returns>
        public IVertex Remove(IVertex v, bool release = false)
        {
            int index = m_vertices.IndexOf(v);
            return RemoveAt(index);
        }

        /// <summary>
        /// Removes the vertex at the given index from the group .
        /// </summary>
        /// <param name="index"></param>
        /// <param name="keepProxies"></param>
        /// <returns></returns>
        public IVertex RemoveAt(int index, bool release = false)
        {
            IVertex result = m_vertices[index];
            m_vertices.RemoveAt(index);
            OnVertexRemoved(result as V);
            if (release) { result.Release(); }
            return result;
        }

        protected virtual void OnVertexAdded(V v)
        {
            //v.OnRelease(m_onVertexReleasedCached);
        }

        protected virtual void OnVertexRemoved(V v)
        {
            //v.OffRelease(m_onVertexReleasedCached);
        }

        protected virtual void OnVertexReleased(IPoolItem vertex)
        {
            Remove(vertex as IVertex);
        }

        /// <summary>
        /// Inverse vertices's order
        /// </summary>
        public void Reverse()
        {
            m_vertices.Reverse();
        }

        /// <summary>
        /// Removes and return the first item in the group
        /// </summary>
        /// <returns></returns>
        public IVertex Shift(bool release = false)
        {
            int count = m_vertices.Count;
            if (count == 0) { return null; }
            return RemoveAt(0, release);
        }

        /// <summary>
        /// Removes and return the last item in the group
        /// </summary>
        /// <returns></returns>
        public IVertex Pop(bool release = false)
        {
            int count = m_vertices.Count;
            if(count == 0) { return null; }
            return RemoveAt(count - 1, release);
        }

        /// <summary>
        /// Removes all vertices from the group.
        /// </summary>
        public virtual void Clear(bool release = false)
        {      
            int count = m_vertices.Count;
            while (count != 0)
            {
                RemoveAt(count - 1, release);
                count = m_vertices.Count;
            }            
        }

        /// <summary>
        /// Offset all vertices
        /// </summary>
        /// <param name="offset"></param>
        public void Offset(float3 offset)
        {
            for(int i = 0, count = m_vertices.Count; i < count; i++)
                m_vertices[i].pos += offset;
        }

        #region PoolItem

        protected virtual void CleanUp()
        {
            Clear(false);
        }

        #endregion

        #region Nearest vertex in group

        /// <summary>
        /// Return the vertex index in group of the nearest IVertex to a given IVertex v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public int GetNearestVertexIndex(IVertex v)
        {
            int index = -1, count = m_vertices.Count;
            float dist, sDist = float.MaxValue;
            float3 A = v.pos, B, C;
            IVertex oV;
            for (int i = 0; i < count; i++)
            {
                oV = m_vertices[i];

                if (oV == v) { continue; }

                B = oV.pos;
                C = float3(A.x - B.x, A.y - B.y, A.z - B.z);
                dist = C.x * C.x + C.y * C.y + C.z * C.z;

                if (dist > sDist)
                {
                    sDist = dist;
                    index = i;
                }
            }

            return index;
        }

        /// <summary>
        /// Return the the nearest IVertex in group to a given IVertex v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public IVertex GetNearestVertex(IVertex v)
        {
            int index = GetNearestVertexIndex(v);
            if (index == -1) { return null; }
            return m_vertices[index];
        }

        /// <summary>
        /// Return the vertex index in group of the nearest IVertex to a given v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public int GetNearestVertexIndex(float3 v)
        {
            int index = -1, count = m_vertices.Count;
            float dist, sDist = float.MaxValue;
            float3 B, C;
            for (int i = 0; i < count; i++)
            {
                B = m_vertices[i].pos;
                C = float3(v.x - B.x, v.y - B.y, v.z - B.z);
                dist = C.x * C.x + C.y * C.y + C.z * C.z;

                if (dist > sDist)
                {
                    sDist = dist;
                    index = i;
                }
            }

            return index;
        }

        /// <summary>
        /// Return the nearest IVertex in group of the nearest IVertex to a given v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public IVertex GetNearestVertex(float3 v)
        {
            int index = GetNearestVertexIndex(v);
            if (index == -1) { return null; }
            return m_vertices[index];
        }



        #endregion
        
    }
    
}
