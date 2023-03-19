using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JPEG
{
    internal static class FourierTransformer
    {

        public static Bitmap Padding(this Bitmap image)
        {
            int w = image.Width;
            int h = image.Height;
            int n = 0;
            var size = 1024;
            double horizontal_padding = size - w;
            double vertical_padding = size - h;
            int left_padding = (int)Math.Floor(horizontal_padding / 2);
            int top_padding = (int)Math.Floor(vertical_padding / 2);

            BitmapData image_data = image.LockBits(
                new Rectangle(0, 0, w, h),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            int bytes = image_data.Stride * image_data.Height;
            byte[] buffer = new byte[bytes];
            Marshal.Copy(image_data.Scan0, buffer, 0, bytes);
            image.UnlockBits(image_data);

            Bitmap padded_image = new Bitmap(size, size);

            BitmapData padded_data = padded_image.LockBits(
                new Rectangle(0, 0, size, size),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);

            int padded_bytes = padded_data.Stride * padded_data.Height;
            byte[] result = new byte[padded_bytes];

            for (int i = 3; i < padded_bytes; i += 4)
            {
                result[i] = 255;
            }

            for (int y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++)
                {
                    int image_position = y * image_data.Stride + x * 4;
                    int padding_position = y * padded_data.Stride + x * 4;
                    for (int i = 0; i < 3; i++)
                    {
                        result[padded_data.Stride * top_padding + 4 * left_padding + padding_position + i] = buffer[image_position + i];
                    }
                }
            }

            Marshal.Copy(result, 44, padded_data.Scan0, padded_bytes);
            padded_image.UnlockBits(padded_data);

            return padded_image;
        }


    }
}
