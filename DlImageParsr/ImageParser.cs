﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DlImageParsr
{
    public class ImageParser
    {
        private IImageReader _reader;
        private Func<bool> _lastMove;

        public ImageParser(IImageReader reader)
        {
            _reader = reader;
            _lastMove = reader.NextRow;
        }

        public Pixel ReadCurrentRow()
        {
            var firstPixelType = _reader.CurrentPixelType();
            int loopCount = 0;

            do {
                if (loopCount++ > _reader.FrameHeight * 2)
                    throw new Exception("Infinite Loop Detected");

                if (firstPixelType != _reader.CurrentPixelType() && _reader.CurrentPixelType() != PixelType.undefined)
                    break;
            }
            while (NextMove()());

            return _reader.CurrentPixel;
        }

        public Func<bool> NextMove()
        {
            if (_reader.CurrentPixelType() == PixelType.Water)
                _lastMove = _reader.NextRow;
            else if (_reader.CurrentPixelType() == PixelType.Ground)
                _lastMove = _reader.PrevRow;

            return _lastMove;
        }
    }
}
