using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media.Media3D;

namespace PdgEngine3D
{
    class Engine
    {
        private Mesh mesh;
        private Vector3D vCamera = new Vector3D(0, 0, 0);
        private PictureBox box;
        private float fTheta = 0.0f;
        private int BitMapSpread = 1000;
        private Matrix4x4 mat = new Matrix4x4();
        bool disp = true;
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
                Thread.Sleep(30);
            }
        }
        public void ChangeObject(string fileName)
        {
            disp = false;
            Thread.Sleep(500);
            UserCreate(fileName, null);
            StartDisplaying();
        }
        public void Update(float angle)
        {
            UserUpdate(angle, null);
        }
        public void OnUserCreate(object sender, EventArgs e)
        {
            if (sender != null)
            {
                try
                {
                    var filePath = (string)sender;
                    mesh.LoadFromObject(filePath);
                }
                catch (Exception) { }
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

            float fNear = 0.1f;
            float fFar = 1000.0f;
            float fFov = 90.0f;
            float fAspectRatio = 1.0f;
            float fFovRad = 1.0f / (float)Math.Tan(fFov * 0.5f / 180.0f * Math.PI);

            mat = new Matrix4x4(fAspectRatio * fFovRad, 0, 0, 0,
                                0, fFovRad, 0, 0,
                                0, 0, fFar / (fFar - fNear), 1.0f,
                                0, 0, (-fFar * fNear) / (fFar - fNear), 0.0f);


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

            //Matrix to rotate around X axis
            Matrix4x4 matRotX = new Matrix4x4((float)Math.Cos(fTheta), (float)Math.Sin(fTheta), 0, 0,
                                               -(float)Math.Sin(fTheta), (float)Math.Cos(fTheta), 0, 0,
                                               0, 0, 1.0f, 0,
                                               0, 0, 0, 1.0f);
            //Matrix to rotate around Z axis
            Matrix4x4 matRotZ = new Matrix4x4(1.0f, 0, 0, 0,
                                               0, (float)Math.Cos(fTheta * 0.5F), (float)Math.Sin(fTheta * 0.5F), 0,
                                               0, -(float)Math.Sin(fTheta * 0.5F), (float)Math.Cos(fTheta * 0.5F), 0,
                                               0, 0, 0, 1.0f);



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
                MultiplyMatrixVector(tria.p[0], ref triaRotatedX.p[0], matRotX);
                MultiplyMatrixVector(tria.p[1], ref triaRotatedX.p[1], matRotX);
                MultiplyMatrixVector(tria.p[2], ref triaRotatedX.p[2], matRotX);

                //Rotate in Z-Axis
                MultiplyMatrixVector(triaRotatedX.p[0], ref triaRotatedZX.p[0], matRotZ);
                MultiplyMatrixVector(triaRotatedX.p[1], ref triaRotatedZX.p[1], matRotZ);
                MultiplyMatrixVector(triaRotatedX.p[2], ref triaRotatedZX.p[2], matRotZ);

                // Offset into the screen
                triaTranslated = triaRotatedZX;
                triaTranslated.p[0].Z = triaRotatedZX.p[0].Z + 2.5f;
                triaTranslated.p[1].Z = triaRotatedZX.p[1].Z + 2.5f;
                triaTranslated.p[2].Z = triaRotatedZX.p[2].Z + 2.5f;
                triaProjected.p = new Vector3D[] { new Vector3D(), new Vector3D(), new Vector3D() };



                //Cross product
                Vector3D normal = new Vector3D();
                Vector3D line2 = new Vector3D();
                Vector3D line1 = new Vector3D();

                line1.X = triaTranslated.p[1].X - triaTranslated.p[0].X;
                line1.Y = triaTranslated.p[1].Y - triaTranslated.p[0].Y;
                line1.Z = triaTranslated.p[1].Z - triaTranslated.p[0].Z;

                line2.X = triaTranslated.p[2].X - triaTranslated.p[0].X;
                line2.Y = triaTranslated.p[2].Y - triaTranslated.p[0].Y;
                line2.Z = triaTranslated.p[2].Z - triaTranslated.p[0].Z;

                normal = Vector3D.CrossProduct(line1, line2);

                // It's normallY normal to normalise the normal
                normal.Normalize();

                //if (normal.Z < 0)
                if (normal.X * (triaTranslated.p[0].X - vCamera.X) +
                   normal.Y * (triaTranslated.p[0].Y - vCamera.Y) +
                   normal.Z * (triaTranslated.p[0].Z - vCamera.Z) < 0.0f)
                {
                    //Project triangle 3D -> 2D
                    MultiplyMatrixVector(triaTranslated.p[0], ref triaProjected.p[0], mat);
                    MultiplyMatrixVector(triaTranslated.p[1], ref triaProjected.p[1], mat);
                    MultiplyMatrixVector(triaTranslated.p[2], ref triaProjected.p[2], mat);

                    triaProjected.p[0].X += 1.0f;
                    triaProjected.p[1].X += 1.0f;
                    triaProjected.p[2].X += 1.0f;
                    triaProjected.p[0].Y += 1.0f;
                    triaProjected.p[1].Y += 1.0f;
                    triaProjected.p[2].Y += 1.0f;

                    triaProjected.p[0].X *= BitMapSpread / 2;
                    triaProjected.p[1].X *= BitMapSpread / 2;
                    triaProjected.p[2].X *= BitMapSpread / 2;
                    triaProjected.p[0].Y *= BitMapSpread / 2;
                    triaProjected.p[1].Y *= BitMapSpread / 2;
                    triaProjected.p[2].Y *= BitMapSpread / 2;
                    ChooseColor(ref triaProjected, normal);
                    drawList.Add(triaProjected);
                }


            }

            //Draw triangles
            Bitmap btm = new Bitmap(BitMapSpread, BitMapSpread);
            
                using (Graphics g = Graphics.FromImage(btm))
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    foreach (var triaProjected in drawList)
                    {
                        DrawTriangle(g, (int)triaProjected.p[0].X, (int)triaProjected.p[0].Y,
                            (int)triaProjected.p[1].X, (int)triaProjected.p[1].Y,
                            (int)triaProjected.p[2].X, (int)triaProjected.p[2].Y,
                            triaProjected.color, Color.Black);
                    }
                }
            box.Image = btm;

        }
        public void ChooseColor(ref Triangle tria, Vector3D normal)
        {
            Vector3D light_direction = new Vector3D{ X = 0.0f, Y = 0.0f, Z = -1.0f };
            light_direction.Normalize();
            double dp = Vector3D.DotProduct(normal, light_direction);
            int pixel_lum = (int)(255.0f * dp);
            tria.color = Color.FromArgb(pixel_lum, pixel_lum, pixel_lum);
        }
        public void DrawTriangle(Graphics g, int x1, int y1, int x2, int y2, int x3, int y3, Color FillColor, Color EdgeColor)
        {
            Pen pen1 = new Pen(EdgeColor, 4.0f);
            g.DrawLine(pen1, x1, y1, x2, y2);
            g.DrawLine(pen1, x2, y2, x3, y3);
            g.DrawLine(pen1, x1, y1, x3, y3);

            Pen pen2 = new Pen(FillColor);
            g.FillPolygon(pen2.Brush, new Point[] { new Point() { X = x1, Y = y1 }, new Point() { X = x2, Y = y2 }, new Point() { X = x3, Y = y3 } });
        }

        internal void UpdateUser(float elapsedTime) => UserUpdate?.Invoke(elapsedTime, null);

        internal void CreateUser() => UserUpdate?.Invoke(null, null);
    }
}
