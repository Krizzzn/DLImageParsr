using System;
using System.Collections;
using System.Collections.Generic;
namespace DlImageParsr
{
    public interface IImageReader
    {
        PixelType CurrentPixelType();
        Pixel CurrentPixel { get; }

        int FrameWidth { get; set; }
        int FrameHeight { get; set; }

        bool NextColumn();
        bool NextRow();
        bool PrevRow();
    }
}
