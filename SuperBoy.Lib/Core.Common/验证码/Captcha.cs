using System;
using System.Drawing;

public static class Captcha
{
    private static double[] AddVector(double[] a, double[] b)
    {
        return new double[] { a[0] + b[0], a[1] + b[1], a[2] + b[2] };
    }

    private static double[] ScalarProduct(double[] vector, double scalar)
    {
        return new double[] { vector[0] * scalar, vector[1] * scalar, vector[2] * scalar };
    }

    private static double DotProduct(double[] a, double[] b)
    {
        return a[0] * b[0] + a[1] * b[1] + a[2] * b[2];
    }

    private static double Norm(double[] vector)
    {
        return Math.Sqrt(DotProduct(vector, vector));
    }

    private static double[] Normalize(double[] vector)
    {
        return ScalarProduct(vector, 1.0 / Norm(vector));
    }

    private static double[] CrossProduct(double[] a, double[] b)
    {
        return new double[] 
                { 
                    (a[1] * b[2] - a[2] * b[1]), 
                    (a[2] * b[0] - a[0] * b[2]), 
                    (a[0] * b[1] - a[1] * b[0]) 
                };
    }

    private static double[] VectorProductIndexed(double[] v, double[] m, int i)
    {
        return new double[]
                {
                    v[i + 0] * m[0] + v[i + 1] * m[4] + v[i + 2] * m[8] + v[i + 3] * m[12],
                    v[i + 0] * m[1] + v[i + 1] * m[5] + v[i + 2] * m[9] + v[i + 3] * m[13],
                    v[i + 0] * m[2] + v[i + 1] * m[6] + v[i + 2] * m[10]+ v[i + 3] * m[14],
                    v[i + 0] * m[3] + v[i + 1] * m[7] + v[i + 2] * m[11]+ v[i + 3] * m[15]
                };
    }

    private static double[] VectorProduct(double[] v, double[] m)
    {
        return VectorProductIndexed(v, m, 0);
    }

    private static double[] MatrixProduct(double[] a, double[] b)
    {
        var o1 = VectorProductIndexed(a, b, 0);
        var o2 = VectorProductIndexed(a, b, 4);
        var o3 = VectorProductIndexed(a, b, 8);
        var o4 = VectorProductIndexed(a, b, 12);

        return new double[]
                {
                    o1[0], o1[1], o1[2], o1[3],
                    o2[0], o2[1], o2[2], o2[3],
                    o3[0], o3[1], o3[2], o3[3],
                    o4[0], o4[1], o4[2], o4[3]
                };
    }

    private static double[] CameraTransform(double[] c, double[] a)
    {
        var w = Normalize(AddVector(c, ScalarProduct(a, -1)));
        var y = new double[] { 0, 1, 0 };
        var u = Normalize(CrossProduct(y, w));
        var v = CrossProduct(w, u);
        var t = ScalarProduct(c, -1);

        return new double[]
                {
                    u[0], v[0], w[0], 0,
                    u[1], v[1], w[1], 0,
                    u[2], v[2], w[2], 0,
                    DotProduct(u, t), DotProduct(v, t), DotProduct(w, t), 1
                };
    }

    private static double[] ViewingTransform(double fov, double n, double f)
    {
        fov *= (Math.PI / 180);
        var cot = 1.0 / Math.Tan(fov / 2);
        return new double[] { cot, 0, 0, 0, 0, cot, 0, 0, 0, 0, (f + n) / (f - n), -1, 0, 0, 2 * f * n / (f - n), 0 };
    }

    public static Image Generate(string captchaText)
    {
        var fontsize = 24;
        var font = new Font("Arial", fontsize);

        SizeF sizeF;
        using (var g = Graphics.FromImage(new Bitmap(1, 1)))
        {
            sizeF = g.MeasureString(captchaText, font, 0, StringFormat.GenericDefault);
        }

        var image2DX = (int)sizeF.Width;
        var image2DY = (int)(fontsize * 1.3);

        var image2D = new Bitmap(image2DX, image2DY);
        var black = Color.Black;
        var white = Color.White;

        using (var g = Graphics.FromImage(image2D))
        {
            g.Clear(black);
            g.DrawString(captchaText, font, Brushes.White, 0, 0);
        }

        var rnd = new Random();
        var T = CameraTransform(new double[] { rnd.Next(-90, 90), -200, rnd.Next(150, 250) }, new double[] { 0, 0, 0 });
        T = MatrixProduct(T, ViewingTransform(60, 300, 3000));

        var coord = new double[image2DX * image2DY][];

        var count = 0;
        for (var y = 0; y < image2DY; y += 2)
        {
            for (var x = 0; x < image2DX; x++)
            {
                var xc = x - image2DX / 2;
                var zc = y - image2DY / 2;
                var yc = -(double)(image2D.GetPixel(x, y).ToArgb() & 0xff) / 256 * 4;
                var xyz = new double[] { xc, yc, zc, 1 };
                xyz = VectorProduct(xyz, T);
                coord[count] = xyz;
                count++;
            }
        }

        var image3DX = 256;
        var image3DY = image3DX * 9 / 16;
        var image3D = new Bitmap(image3DX, image3DY);
        var fgcolor = Color.White;
        var bgcolor = Color.Black;
        using (var g = Graphics.FromImage(image3D))
        {
            g.Clear(bgcolor);
            count = 0;
            var scale = 1.75 - (double)image2DX / 400;
            for (var y = 0; y < image2DY; y += 2)
            {
                for (var x = 0; x < image2DX; x++)
                {
                    if (x > 0)
                    {
                        var x0 = coord[count - 1][0] * scale + image3DX / 2;
                        var y0 = coord[count - 1][1] * scale + image3DY / 2;
                        var x1 = coord[count][0] * scale + image3DX / 2;
                        var y1 = coord[count][1] * scale + image3DY / 2;
                        g.DrawLine(new Pen(fgcolor), (float)x0, (float)y0, (float)x1, (float)y1);
                    }
                    count++;
                }
            }
        }
        return image3D;
    }
}

