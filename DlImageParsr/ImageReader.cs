using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DlImageParsr
{
    public class ImageReader : DlImageParsr.IImageReader
    {
        private Bitmap _image;
        private Func<int, int, PixelType> getPixelFromImage;
        private bool _skipCurrentColumn;

        public ImageReader(Bitmap image)
        {
            _image = image;
            getPixelFromImage = (x, y) => (PixelType)_image.GetPixel(x, y).G;

            FrameX = 21;
            FrameY = 1;
            FrameHeight = 300;
            FrameWidth = 300;
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
            if (CurrentRow + 1 >= FrameHeight || _skipCurrentColumn)
                return false;

            CurrentRow++;
            return true;
        }

        public bool NextColumn()
        {
            if (CurrentColumn + 1 >= FrameWidth)
                return false;

            CurrentColumn++;
            PeekCurrentColumn();

            return true;
        }

        private void PeekCurrentColumn()
        {
            int peekPixelCount = 10;
            if (FrameHeight < peekPixelCount)
                peekPixelCount = FrameHeight;

            var startPixel = (CurrentRow - (peekPixelCount / 2));
            var endPixel = (CurrentRow + (peekPixelCount / 2));

            while (startPixel < 0) {
                startPixel++;
                endPixel++;
            }
            while (endPixel >= FrameHeight) {
                startPixel--;
                endPixel--;
            }

            var foundValidPixel = false;
            while (startPixel < endPixel && !foundValidPixel) {
                if (GetPixelType(CurrentColumn, startPixel) != PixelType.undefined)
                    foundValidPixel = true;
                startPixel++;
            }
            _skipCurrentColumn = !foundValidPixel;
        }

        private PixelType GetPixelType(int x, int y)
        {
            if (_skipCurrentColumn)
                return PixelType.Skip;

            return getPixelFromImage(FrameX + x, FrameY + y);
        }

        public PixelType CurrentPixelType()
        {
            return GetPixelType(CurrentColumn, CurrentRow);
        }

        public bool PrevRow()
        {
            if (CurrentRow - 1 < 0)
                return false;

            CurrentRow--;
            return true;
        }


        public Pixel CurrentPixel
        {
            get
            {
                if (_skipCurrentColumn)
                    return new SkipPixel(FrameX + CurrentColumn, FrameY + CurrentRow);
                return new Pixel(FrameX + CurrentColumn, FrameY + CurrentRow);
            }
        }
    }
}
