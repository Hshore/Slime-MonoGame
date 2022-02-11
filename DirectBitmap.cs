using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace monoSlime2
{
    public class DirectBitmap : IDisposable
    {
        public Bitmap Bitmap { get; private set; }
        public int[] Bits { get; private set; }
        public bool Disposed { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        protected GCHandle BitsHandle { get; private set; }

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new int[width * height];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppArgb, BitsHandle.AddrOfPinnedObject());
        }


        public void SetPixel_byID(int pixelid, int a, int r, int g, int b)
        {
            int col = Color.FromArgb(a, r, g, b).ToArgb();
        }

        public void SetPixel_intArray(int[] pixelColors_packedInts)
        {
            //int index = x + (y * Width);
            //int index = x + (y * Width);
            Bits = pixelColors_packedInts;

            // Bits[pixelid] = col;

            // Bits[pixelid] = 255;
            // Bits[pixelid + 1] = 255;
            // Bits[pixelid + 2] = 255;
            // Bits[pixelid + 3] = 255;

        }
        public void SetPixel_bytes_byID(int pixelRGBid, int a, int r, int g, int b)
        {
            //int index = x + (y * Width);
            //int index = x + (y * Width);
            // int col = Color.FromArgb(a, r, g, b).ToArgb();

            //  Bits[pixelid] = col;

            Bits[pixelRGBid] = (byte)a;
            Bits[pixelRGBid + 1] = (byte)r;
            Bits[pixelRGBid + 2] = (byte)g;
            Bits[pixelRGBid + 3] = (byte)b;

        }

        public Color GetPixel(int x, int y)
        {
            int index = x + (y * Width);
            int col = Bits[index];
            Color result = Color.FromArgb(col);

            return result;
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            Bitmap.Dispose();
            BitsHandle.Free();
        }
    }
}