using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Net;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using JPEG.Images;
using JPEG.Processor;
using JPEG.Utilities;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace JPEG;

public class DCT
{
    static DCT() => FindCos();
    private const int Size = JpegProcessor.DCTSize;
    public static double[,] Cos;
    private const double Beta = 1.0 / Size + 1.0 / Size;
    private static double AlphaS = 1 / Math.Sqrt(2);
    public static double[,] DCT2D(Pixel[,] matrix, Func<Pixel, double> selector, int xOffset, int yOffset
    )
    {
        var coeffs = new double[Size, Size];
        for (var u = 0; u < Size; u++)
            for (var v = 0; v < Size; v++)
            {
                MathSum(matrix, selector, xOffset, yOffset, u, v, coeffs);
            }
        return coeffs;
    }

    private static void MathSum(Pixel[,] matrix, Func<Pixel, double> selector, int xOffset, int yOffset, int u, int v,
        double[,] coeffs)
    {
        var sum = 0.0;
        for (var i = 0; i < Size; i++)
            for (var j = 0; j < Size; j++)
                sum += (selector(matrix[i + xOffset, j + yOffset]) - 128) * Cos[i, u] * Cos[j, v];
        coeffs[u, v] = sum * Beta * Alpha(u) * Alpha(v);
    }
    public static void IDCT2D(
        double[,] coeffs,
        double[,] output,
        int iOffset,
        int jOffset)
    {
     
        for (var i = 0; i < Size; i++)
        for (var j = 0; j < Size; j++)
        {
            var sum = 0.0;
            for (var u = 0; u < Size; u++)
            for (var v = 0; v < Size; v++)
                sum += coeffs[u, v] * Cos[i, u] * Cos[j, v] * Alpha(u) * Alpha(v);

            output[i + iOffset, j + jOffset] = sum * Beta + 128;
        }
    }

    private static void FindCos()
    {
        Cos = new double[Size, Size];
        for (var i = 0; i < Size; i++)
        for (var j = 0; j < Size; j++)
            Cos[i, j] = Math.Cos((2 * i + 1) * j * Math.PI / (2 * Size));
    }
    private static double Alpha(int u)
    {
        return u == 0 ? AlphaS : 1;
    }
}