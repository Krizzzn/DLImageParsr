using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DlImageParsr
{
    public class ImageReader
    {
        private Bitmap _image;
        private Func<int, int, PixelType> getPixelFromImage;

        public ImageReader(Bitmap image)
        {
            _image = image;
            getPixelFromImage = (x, y) => (PixelType)_image.GetPixel(x, y).G;

            FrameX = 21;
            FrameY = 1;
            FrameHeight = 300;
            FrameWidth = 700;
        }

        public ImageReader(Func<int, int, PixelType> pixelReceiver)
        {
            getPixelFromImage = pixelReceiver;
        }

        public int FrameX { get; set; }
        public int FrameY { get; set; }
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }

        public int CurrentColumn { get; private set; }
        public int CurrentRow { get; private set; }

        public bool NextRow()
        {
            if (CurrentRow + 1 >= FrameHeight)
                return false;

            CurrentRow++;
            return true;
        }

        public bool NextColumn()
        {
            if (CurrentColumn + 1 >= FrameWidth)
                return false;

            CurrentColumn++;
            return true;
        }

        public PixelType CurrentPixel()
        {
            return getPixelFromImage(FrameX + CurrentColumn, FrameY + CurrentRow);
        }

        public bool PrevRow()
        {
            if (CurrentRow - 1 < 0)
                return false;

            CurrentRow--;
            return true;
        }
    }
}
