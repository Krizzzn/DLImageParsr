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

        public ImageReader(Bitmap image){
            _image = image;

            getPixelFromImage = (x, y) => (PixelType)_image.GetPixel(x, y).G;
        }

        public ImageReader(Func<int, int, PixelType> pixelReceiver) {
            getPixelFromImage = pixelReceiver;
        }

        public int FrameX {get;set;}
        public int FrameY {get;set;}
        public int FrameWidth {get;set;}
        public int FrameHeight {get;set;}
 
        public int CurrentColumn{get;private set;}
        public int CurrentRow{get;private set;}

        public bool NextRow(){
            return true;
        }

        public bool NextColumn(){
            return true;
        }

        public PixelType CurrentPixel() {
            return PixelType.undefined;
        }
    }
}
