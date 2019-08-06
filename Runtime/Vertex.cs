using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Nebukam.Common
{

    public interface IVertex
    {
        float3 pos { get; set; }
        float2 XY { get; }
        float2 XZ { get; }
    }
    
    public class Vertex : IVertex
    {

        internal float3 m_pos = float3(false);
        public float3 pos {
            get { return m_pos; }
            set {
                m_pos = value;
                m_XY = float2(value.x, value.y);
                m_XZ = float2(value.x, value.z);
            }
        }

        internal float2 m_XY = float2(false);
        public float2 XY { get { return m_XY; } }

        internal float2 m_XZ = float2(false);
        public float2 XZ { get { return m_XZ; } }

        public Vertex()
        {

        }

        public Vertex(float3 v3)
        {
            pos = v3;
        }

        public Vertex(float x, float y, float z = 0f)
        {
            pos = float3(x, y, z);
        }
        
        public static implicit operator float3(Vertex p) { return p.m_pos; }

    }


}
