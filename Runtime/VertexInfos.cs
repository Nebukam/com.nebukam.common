﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;

namespace Nebukam
{

    public interface IVertexInfos
    {
        float3 pos { get; set; }
    }

    public partial struct VertexInfos : IVertexInfos
    {

        #region IVertexInfos

        public float3 m_pos;

        public float3 pos
        {
            get { return m_pos; }
            set { m_pos = value; }
        }

        #endregion

    }
}
