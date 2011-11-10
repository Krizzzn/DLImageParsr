using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DlImageParsr
{
    public class Pixel
    {
        public Pixel(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; private set; }
        public int Y { get; private set; }
    }

    public class SkipPixel : Pixel
    {
        public SkipPixel(int x, int y)
            : base(x, y)
        {}
    }
}
