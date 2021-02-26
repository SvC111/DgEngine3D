using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PdgEngine3D.Helpers
{
    static class MatrixHelper
    {
        public static Matrix4x4 GetMatrixRotateX(float angle)
        {
            return
                new Matrix4x4
                ((float)Math.Cos(angle), (float)Math.Sin(angle), 0, 0,
                -(float)Math.Sin(angle), (float)Math.Cos(angle), 0, 0,
                0, 0, 1.0f, 0,
                0, 0, 0, 1.0f);
        }
        public static Matrix4x4 GetMatrixRotateY(float angle)
        {
            return
                new Matrix4x4
                ((float)Math.Cos(angle * 0.5F), 0, 0, (float)Math.Sin(angle * 0.5F),
                0, 1.0f, 0, 0,
                -(float)Math.Sin(angle * 0.5F), 0, 0, (float)Math.Cos(angle * 0.5F),
                0, 0, 0, 1.0f);
        }

        public static Matrix4x4 GetMatrixRotateZ(float angle)
        {
            return
                new Matrix4x4
                (1.0f, 0, 0, 0,
                0, (float)Math.Cos(angle * 0.5F), (float)Math.Sin(angle * 0.5F), 0,
                0, -(float)Math.Sin(angle * 0.5F), (float)Math.Cos(angle * 0.5F), 0,
                0, 0, 0, 1.0f);
        }

        
    }
}
