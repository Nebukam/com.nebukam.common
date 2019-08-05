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

        public float3 m_pos = float3(false);
        public float3 pos { get { return m_pos; } set { m_pos = value; } }

        public float2 XY { get { return float2(m_pos.x, m_pos.y); } }
        public float2 XZ { get { return float2(m_pos.x, m_pos.z); } }

        public Vertex()
        {

        }

        public Vertex(float3 v3)
            : this()
        {
            pos = v3;
        }

        public Vertex(float vx, float vy, float vz)
            : this( float3(vx, vy, vz) )
        {

        }
        
        public static implicit operator float3(Vertex p) { return p.pos; }
        public static implicit operator Vertex(float3 p)
        {
            return new Vertex(p);
        }

    }


}
