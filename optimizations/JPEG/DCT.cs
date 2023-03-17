using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using JPEG.Images;
using JPEG.Processor;
using JPEG.Utilities;

namespace JPEG;

public class DCT
{
    private const int DCTSize = JpegProcessor.DCTSize;
    public static double[,] cos;
    public static double[,] DCT2D(Pixel[,] input, Func<Pixel, double> selector, int xOffset, int yOffset,int shiftValue
    )
    {
        var coeffs = new double[DCTSize, DCTSize];
        FindCos();
        for (var u = 0; u < DCTSize; u++)
        for (var v = 0; v < DCTSize; v++)
        {
            MathSum(input, selector, xOffset, yOffset, shiftValue, u, v, coeffs);
        }
        return coeffs;
    }

    private static void MathSum(Pixel[,] input, Func<Pixel, double> selector, int xOffset, int yOffset, int shiftValue, int u, int v,
        double[,] coeffs)
    {
        double sum = 0;
        for (var x = 0; x < DCTSize; x++)
        for (var y = 0; y < DCTSize; y++)
            sum += (selector(input[x + xOffset, y + yOffset]) + shiftValue) * cos[x, u] * cos[y, v];
        coeffs[u, v] = sum * Beta(DCTSize, DCTSize) * Alpha(u) * Alpha(v);
    }

    public static void IDCT2D(
        double[,] coeffs,
        double[,] output,
        int iOffset,
        int jOffset)
    {
        FindCos();
        for (var i = 0; i < DCTSize; i++)
        for (var j = 0; j < DCTSize; j++)
        {
            double sum = 0;
            for (var u = 0; u < DCTSize; u++)
            for (var v = 0; v < DCTSize; v++)
                sum += coeffs[u, v] * cos[i, u] * cos[j, v] * Alpha(u) * Alpha(v);

            output[i + iOffset, j + jOffset] = sum * beta + 128;
        }
    }

    private static double beta = 1.0 / DCTSize + 1.0 / DCTSize;
    private static double AlphaS = 1 / Math.Sqrt(2);
    private static double Alpha(int u)
    {
        if (u == 0)
            return AlphaS;
        return 1;
    }

    private static double Beta(int height, int width)
    {
        return 1d / width + 1d / height;
    }
    private static void FindCos()
    {
        cos = new double[DCTSize, DCTSize];
        for (var i = 0; i < DCTSize; i++)
        for (var j = 0; j < DCTSize; j++)
            cos[i, j] = Math.Cos((2 * i + 1) * j * Math.PI / (2 * DCTSize));
    }
}