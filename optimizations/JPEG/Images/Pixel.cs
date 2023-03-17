using System;
using System.Linq;

namespace JPEG.Images;

public class Pixel
{
	private readonly PixelFormat format;

	public Pixel()
	{
    }

    public Pixel SetPixelRGB(double firstComponent, double secondComponent, double thirdComponent)
    {
        r = firstComponent;
        g = secondComponent;
        b = thirdComponent;
        Y = 16.0 + (65.738 * R + 129.057 * G + 24.064 * B) / 256.0;
        Cb = 128.0 + (-37.945 * R - 74.494 * G + 112.439 * B) / 256.0;
        Cr = 128.0 + (112.439 * R - 94.154 * G - 18.285 * B) / 256.0;
        return this;
    }
    public Pixel SetPixelYCbCr(double firstComponent, double secondComponent, double thirdComponent)
    {
        y = firstComponent;
        cb = secondComponent;
        cr = thirdComponent;
        R = (298.082 * y + 408.583 * Cr) / 256.0 - 222.921;
        G = (298.082 * Y - 100.291 * Cb - 208.120 * Cr) / 256.0 + 135.576;
        B = (298.082 * Y + 516.412 * Cb) / 256.0 - 276.836;
        return this;
    }

    private double r;
	private double g;
	private double b;
    private double y;
	private double cb;
	private double cr;
    public double R;
    public double G;
    public double B;
    public double Y;
	public double Cb; 
    public double Cr;
}