using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using PdgEngine3D.Helpers;

namespace PdgEngine3D
{
    class Engine
    {
        private Mesh mesh;
        private Vector3D vCamera;
        private PictureBox box;
        private float fTheta;
        private int BitMapSpread = 1000;
        bool disp = true;
        bool updating = false;
        public Engine()
        {
            UserCreate += OnUserCreate;
            UserUpdate += OnUserUpdate;
            mesh = new Mesh();
        }

        event EventHandler UserCreate;
        event EventHandler UserUpdate;
        public void Start(PictureBox Box)
        {
            box = Box;
            Thread.Sleep(1000);
            UserCreate(null, null);
            StartDisplaying();

        }

        public void StartDisplaying()
        {
            disp = true;
            while (disp)
            {
                Update(0.02f);
                Thread.Sleep(25);
            }
        }
        public void ChangeObject(string fileName)
        {
            disp = false;
            SpinWait.SpinUntil(() => !updating);
            Thread.Sleep(100);
            UserCreate(fileName, null);
            StartDisplaying();
        }
        public void Update(float angle)
        {
            updating = true;
            UserUpdate(angle, null);
            updating = false;
        }
        public void OnUserCreate(object sender, EventArgs e)
        {
            if (sender != null)
            {
                try
                {
                    fTheta = 0.0f;
                    vCamera = new Vector3D(0, 0, 0);
                    var filePath = (string)sender;
                    mesh.LoadFromObject(filePath);

                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                mesh.tris = new Triangle[]
                {
                new Triangle(){ p = new Vector3D[]{ new Vector3D(0.0f, 0.0f, 0.0f), new Vector3D(0.0f, 1.0f, 0.0f), new Vector3D(1.0f, 1.0f, 0.0f)}},
                new Triangle(){ p = new Vector3D[]{ new Vector3D(0.0f, 0.0f, 0.0f), new Vector3D(1.0f, 1.0f, 0.0f), new Vector3D(1.0f, 0.0f, 0.0f)}},

                new Triangle(){ p = new Vector3D[]{ new Vector3D(1.0f, 0.0f, 0.0f), new Vector3D(1.0f, 1.0f, 0.0f), new Vector3D(1.0f, 1.0f, 1.0f) }},
                new Triangle(){ p = new Vector3D[]{ new Vector3D(1.0f, 0.0f, 0.0f), new Vector3D(1.0f, 1.0f, 1.0f), new Vector3D(1.0f, 0.0f, 1.0f)}},

                new Triangle(){ p = new Vector3D[]{ new Vector3D(1.0f, 0.0f, 1.0f), new Vector3D(1.0f, 1.0f, 1.0f), new Vector3D(0.0f, 1.0f, 1.0f)}},
                new Triangle(){ p = new Vector3D[]{ new Vector3D(1.0f, 0.0f, 1.0f), new Vector3D(0.0f, 1.0f, 1.0f), new Vector3D(0.0f, 0.0f, 1.0f)}},

                new Triangle(){ p = new Vector3D[]{ new Vector3D(0.0f, 0.0f, 1.0f), new Vector3D(0.0f, 1.0f, 1.0f), new Vector3D(0.0f, 1.0f, 0.0f)}},
                new Triangle(){ p = new Vector3D[]{ new Vector3D(0.0f, 0.0f, 1.0f), new Vector3D(0.0f, 1.0f, 0.0f), new Vector3D(0.0f, 0.0f, 0.0f)}},

                new Triangle(){ p = new Vector3D[]{ new Vector3D(0.0f, 1.0f, 0.0f), new Vector3D(0.0f, 1.0f, 1.0f), new Vector3D(1.0f, 1.0f, 1.0f)}},
                new Triangle(){ p = new Vector3D[]{ new Vector3D(0.0f, 1.0f, 0.0f), new Vector3D(1.0f, 1.0f, 1.0f), new Vector3D(1.0f, 1.0f, 0.0f)}},

                new Triangle(){ p = new Vector3D[]{ new Vector3D(1.0f, 0.0f, 1.0f), new Vector3D(0.0f, 0.0f, 1.0f), new Vector3D(0.0f, 0.0f, 0.0f)}},
                new Triangle(){ p = new Vector3D[]{ new Vector3D(1.0f, 0.0f, 1.0f), new Vector3D(0.0f, 0.0f, 0.0f), new Vector3D(1.0f, 0.0f, 0.0f)}},
                };
            }


        }

        public void MultiplyMatrixVector(Vector3D input, ref Vector3D output, Matrix4x4 mat)
        {
            output.X = input.X * mat.M11 + input.Y * mat.M21 + input.Z * mat.M31 + mat.M41;
            output.Y = input.X * mat.M12 + input.Y * mat.M22 + input.Z * mat.M32 + mat.M42;
            output.Z = input.X * mat.M13 + input.Y * mat.M23 + input.Z * mat.M33 + mat.M43;
            float w = (float)(input.X * mat.M14 + input.Y * mat.M24 + input.Z * mat.M34 + mat.M44);

            if (w != 0.0f)
            {
                output.X /= w;
                output.Y /= w;
                output.Z /= w;
            }

        }

        /// <summary>
        /// Event called to refresh console (each frame)
        /// </summary>
        /// <param name="sender"> -- rotation angle </param>
        /// <param name="e"></param>
        public void OnUserUpdate(object sender, EventArgs e)
        {
            fTheta += (float)sender;

            //Calculate triangles
            List<Triangle> drawList = new List<Triangle>();
            foreach (var tria in mesh.tris)
            {
                Triangle triaTranslated = new Triangle();
                Triangle triaRotatedX = new Triangle
                {
                    p = new Vector3D[] { new Vector3D(), new Vector3D(), new Vector3D() }
                };
                Triangle triaProjected = new Triangle
                {
                    p = new Vector3D[] { new Vector3D(), new Vector3D(), new Vector3D() }
                };
                Triangle triaRotatedZX = new Triangle
                {
                    p = new Vector3D[] { new Vector3D(), new Vector3D(), new Vector3D() }
                };


                //Rotate in X-Axis
                RotateAroundXAxis(tria, ref triaRotatedX, fTheta);

                //Rotate in Z-Axis
                RotateAroundZAxis(triaRotatedX, ref triaRotatedZX, fTheta);

                // Offset into the screen
                ZoomInOutTriangle(triaRotatedZX, ref triaTranslated, 15f);

                //Cross product
                Vector3D normal = GetCrossProductOfTriangle(triaTranslated);
                normal.Normalize();

                //if (normal.Z < 0)
                if (normal.X * (triaTranslated.p[0].X - vCamera.X) +
                   normal.Y * (triaTranslated.p[0].Y - vCamera.Y) +
                   normal.Z * (triaTranslated.p[0].Z - vCamera.Z) < 0.0f)
                {
                    //Project triangle 3D -> 2D
                    ProjectTriangle(triaTranslated, ref triaProjected);

                    OffsetTriangle(ref triaProjected);

                    ScaleTriangleToBitmap(ref triaProjected);

                    ChooseColor(ref triaProjected, normal);

                    drawList.Add(triaProjected);
                }
            }

            SetDrawingOrder(ref drawList);

            DrawObjectOnScreen(drawList);
        }


        private void SetDrawingOrder(ref List<Triangle> drawList)
        {
            drawList = drawList.OrderByDescending(t => t.p.Sum(s => s.Z) / 3.0f).ToList();
        }
        private void DrawObjectOnScreen(List<Triangle> drawList)
        { 
            //Draw triangles
            Bitmap btm = new Bitmap(BitMapSpread, BitMapSpread);

            using (Graphics g = Graphics.FromImage(btm))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                foreach (var triaProjected in drawList)
                {
                    DrawTriangle(g, (int)triaProjected.p[0].X, (int)triaProjected.p[0].Y,
                        (int)triaProjected.p[1].X, (int)triaProjected.p[1].Y,
                        (int)triaProjected.p[2].X, (int)triaProjected.p[2].Y,
                        triaProjected.color, triaProjected.color);
                }
            }
            box.Image = btm;
        }
        private void ZoomInOutTriangle(Triangle input, ref Triangle output, float scale)
        {
            output = input;
            output.p[0].Z = input.p[0].Z + scale;
            output.p[1].Z = input.p[1].Z + scale;
            output.p[2].Z = input.p[2].Z + scale;
        }
        private void RotateAroundXAxis(Triangle input, ref Triangle output, float angle)
        {
            var matRotX = MatrixHelper.GetMatrixRotateX(angle);
            MultiplyMatrixVector(input.p[0], ref output.p[0], matRotX);
            MultiplyMatrixVector(input.p[1], ref output.p[1], matRotX);
            MultiplyMatrixVector(input.p[2], ref output.p[2], matRotX);
        }
        private void RotateAroundYAxis(Triangle input, ref Triangle output, float angle)
        {
            var matRotX = MatrixHelper.GetMatrixRotateY(angle);
            MultiplyMatrixVector(input.p[0], ref output.p[0], matRotX);
            MultiplyMatrixVector(input.p[1], ref output.p[1], matRotX);
            MultiplyMatrixVector(input.p[2], ref output.p[2], matRotX);
        }
        private void RotateAroundZAxis(Triangle input, ref Triangle output, float angle)
        {
            var matRotX = MatrixHelper.GetMatrixRotateZ(angle);
            MultiplyMatrixVector(input.p[0], ref output.p[0], matRotX);
            MultiplyMatrixVector(input.p[1], ref output.p[1], matRotX);
            MultiplyMatrixVector(input.p[2], ref output.p[2], matRotX);
        }
        public void ChooseColor(ref Triangle tria, Vector3D normal)
        {
            Vector3D light_direction = new Vector3D { X = 0.0f, Y = 0.0f, Z = -1.0f };
            light_direction.Normalize();
            double dp = Vector3D.DotProduct(normal, light_direction);
            int pixel_lum = (int)(255.0f * dp);
            pixel_lum = pixel_lum > 255 ? 255 : pixel_lum < 0 ? 0 : pixel_lum;
            tria.color = Color.FromArgb(pixel_lum, pixel_lum, pixel_lum);
        }

        private void ProjectTriangle(Triangle input, ref Triangle output)
        {
            var mat = MatrixHelper.GetProjectionMatrix();
            MultiplyMatrixVector(input.p[0], ref output.p[0], mat);
            MultiplyMatrixVector(input.p[1], ref output.p[1], mat);
            MultiplyMatrixVector(input.p[2], ref output.p[2], mat);
        }

        private void OffsetTriangle(ref Triangle tria)
        {
            tria.p[0].X += 1.0f;
            tria.p[1].X += 1.0f;
            tria.p[2].X += 1.0f;
            tria.p[0].Y += 1.0f;
            tria.p[1].Y += 1.0f;
            tria.p[2].Y += 1.0f;
        }

        private void ScaleTriangleToBitmap(ref Triangle tria)
        {
            tria.p[0].X *= BitMapSpread / 2;
            tria.p[1].X *= BitMapSpread / 2;
            tria.p[2].X *= BitMapSpread / 2;
            tria.p[0].Y *= BitMapSpread / 2;
            tria.p[1].Y *= BitMapSpread / 2;
            tria.p[2].Y *= BitMapSpread / 2;
        }
        private Vector3D GetCrossProductOfTriangle(Triangle tria)
        {
            Vector3D line2 = new Vector3D();
            Vector3D line1 = new Vector3D();

            line1.X = tria.p[1].X - tria.p[0].X;
            line1.Y = tria.p[1].Y - tria.p[0].Y;
            line1.Z = tria.p[1].Z - tria.p[0].Z;

            line2.X = tria.p[2].X - tria.p[0].X;
            line2.Y = tria.p[2].Y - tria.p[0].Y;
            line2.Z = tria.p[2].Z - tria.p[0].Z;

            return Vector3D.CrossProduct(line1, line2);

        }
        public void DrawTriangle(Graphics g, int x1, int y1, int x2, int y2, int x3, int y3, Color FillColor, Color EdgeColor)
        {
            Pen pen1 = new Pen(EdgeColor, 1.0f);
            g.DrawLine(pen1, x1, y1, x2, y2);
            g.DrawLine(pen1, x2, y2, x3, y3);
            g.DrawLine(pen1, x1, y1, x3, y3);

            Pen pen2 = new Pen(FillColor);
            g.FillPolygon(pen2.Brush, new Point[] { new Point() { X = x1, Y = y1 }, new Point() { X = x2, Y = y2 }, new Point() { X = x3, Y = y3 } });
        }

        internal void UpdateUser(float rotationAngle) => UserUpdate?.Invoke(rotationAngle, null);

        internal void CreateUser() => UserUpdate?.Invoke(null, null);
    }
}
