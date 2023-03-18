using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using JPEG.Images;
using JPEG.Processor;
using JPEG.Utilities;

namespace JPEG;

public class DCT
{
    private const int Size = JpegProcessor.DCTSize;
    public static double[,] Cos;
    public static double[,] DCT2D(Pixel[,] input, Func<Pixel, double> selector, int xOffset, int yOffset,int shiftValue
    )
    {
        var coeffs = new double[Size, Size];
        FindCos();
        for (var u = 0; u < Size; u++)
        for (var v = 0; v < Size; v++)
        {
            MathSum(input, selector, xOffset, yOffset, shiftValue, u, v, coeffs);
        }
        return coeffs;
    }

    private static void MathSum(Pixel[,] input, Func<Pixel, double> selector, int xOffset, int yOffset, int shiftValue, int u, int v,
        double[,] coeffs)
    {
        double sum = 0;
        for (var i = 0; i < Size; i++)
        for (var j = 0; j < Size; j++)
            sum += (selector(input[i + xOffset, j + yOffset]) + shiftValue) * Cos[i, u] * Cos[j, v];
        coeffs[u, v] = sum * Beta(Size, Size) * Alpha(u) * Alpha(v);
    }

    public static void IDCT2D(
        double[,] coeffs,
        double[,] output,
        int iOffset,
        int jOffset)
    {
        FindCos();
        for (var i = 0; i < Size; i++)
        for (var j = 0; j < Size; j++)
        {
            double sum = 0;
            for (var u = 0; u < Size; u++)
            for (var v = 0; v < Size; v++)
                sum += coeffs[u, v] * Cos[i, u] * Cos[j, v] * Alpha(u) * Alpha(v);

            output[i + iOffset, j + jOffset] = sum * beta + 128;
        }
    }

    private static double beta = 1.0 / Size + 1.0 / Size;
    private static double AlphaS = 1 / Math.Sqrt(2);
    
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

    private static double Beta(int height, int width)
    {
        return 1d / width + 1d / height;
    }
}