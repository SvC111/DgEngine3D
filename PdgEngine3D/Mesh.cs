using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PdgEngine3D
{
    struct Mesh
    {
        public Triangle[] tris;

        public void LoadFromObject(string validPath)
        {
            try
            {
                Array.Clear(tris, 0, tris.Length);
                List<Vector3D> vects = new List<Vector3D>();
                using (StreamReader sr = File.OpenText(validPath))
                {
                    while (sr.ReadLine() is string line && line != null)
                    {
                        switch (line[0])
                        {
                            case 'v':
                                Vector3D vect = new Vector3D();
                                var data = line.Split(' ');

                                if (data[0] != 'v'.ToString())
                                    continue;

                                vect.X = double.Parse(data[1]);
                                vect.Y = double.Parse(data[2]);
                                vect.Z = double.Parse(data[3]);
                                vects.Add(vect);
                                break;

                            case 'f':
                                var data1 = line.Split(' ');
                                var index = tris.Length;
                                for(int i=0; i<(data1.Length-3); i++)
                                {
                                    Array.Resize(ref tris, index +(i+1));
                                    var tri = new Triangle()
                                    {
                                        p = new Vector3D[]
                                        {
                                            vects[int.Parse(data1[i+1].Split('/')[0])],
                                            vects[int.Parse(data1[i+2].Split('/')[0])],
                                            vects[int.Parse(data1[i+3].Split('/')[0])]
                                        }
                                    };
                                    tris[index+i] = tri;
                                }
                                break;

                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    };
}
