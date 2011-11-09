using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DlImageParsr
{
    public struct Pixel
    {
        public Pixel(int x, int y) : this()
        {
            X = x;
            Y = y;
        }

        public int X { get; private set; }
        public int Y { get; private set; }
    }
}
