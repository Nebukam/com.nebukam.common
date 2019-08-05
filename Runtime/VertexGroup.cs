using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Nebukam.Common
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
        IVertex Remove(IVertex v);

        /// <summary>
        /// Removes the vertex at the given index from the group .
        /// </summary>
        /// <param name="index"></param>
        /// <param name="keepProxies"></param>
        /// <returns></returns>
        IVertex RemoveAt(int index);


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

    public class VertexGroup<V> : IVertexGroup
        where V : Vertex, IVertex, new()
    {


        protected bool m_locked = false;
        public bool locked { get { return m_locked; } }

        protected List<IVertex> m_lockedVertices = new List<IVertex>();
        protected List<IVertex> m_vertices = new List<IVertex>();
        public List<IVertex> vertices { get { return m_vertices; } }

        public int Count { get { return m_vertices.Count; } }
        
        public IVertex this[int index] { get { return m_vertices[index]; } }
        public int this[IVertex v] { get { return m_vertices.IndexOf(v); } }
              

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
            IVertex vert = new V();
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

            m_vertices.Insert(index, vertex);
            m_vertices.RemoveAt(m_vertices.Count-1);
            return vertex;
        }

        /// <summary>
        /// Create a vertex in the group at the given index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public virtual IVertex Insert(int index, float3 v)
        {
            IVertex vert = new V();
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
        public IVertex Remove(IVertex v)
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
        public IVertex RemoveAt(int index)
        {
            IVertex result = m_vertices[index];
            m_vertices.RemoveAt(index);
            OnVertexRemoved(result as V);
            return result;
        }

        protected virtual void OnVertexAdded(V v)
        {

        }

        protected virtual void OnVertexRemoved(V v)
        {

        }
        
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
