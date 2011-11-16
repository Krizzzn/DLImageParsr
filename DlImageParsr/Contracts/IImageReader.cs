using System;
using System.Collections;
using System.Collections.Generic;
using DlImageParsr.Model;

namespace DlImageParsr.Contracts
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
