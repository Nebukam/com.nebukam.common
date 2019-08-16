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

using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Nebukam
{

    public interface IVertex : Pooling.IPoolItem
    {
        float3 pos { get; set; }
        float2 XY { get; }
        float2 XZ { get; }
        float2 Pair(AxisPair pair);
    }
    
    public class Vertex : Pooling.PoolItem, IVertex
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

        public float2 Pair(AxisPair pair)
        {
            return pair == AxisPair.XY ? m_XY : m_XZ;
        }

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
