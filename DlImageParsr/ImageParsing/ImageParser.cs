using System;
using System.Collections.Generic;
using System.Text;
using DlImageParsr.Contracts;
using DlImageParsr.Model;
using System.Drawing;

namespace DlImageParsr.ImageParsing
{
    public class ImageParser : IImageParser
    {
        private IImageReader _reader;
        private Func<bool> _lastMove;

        public ImageParser(IImageReader reader)
        {
            _reader = reader;
            _lastMove = reader.NextRow;
        }

        public IEnumerable<Pixel> ReadDocument()
        {
            List<Pixel> pixels = new List<Pixel>();
            do {
                var p = ReadCurrentRow();
                pixels.Add(p);
            } while (_reader.NextColumn());

            return pixels;
        }

        public Pixel ReadCurrentRow()
        {
            int loopCount = 0;

            var firstPixelType = _reader.CurrentPixelType();
            if (firstPixelType == PixelType.undefined) {
                for (int i = 0; i < 5; i++)
                    _reader.PrevRow();
                firstPixelType = _reader.CurrentPixelType();
            }

            do {
                if (loopCount++ > _reader.FrameHeight * 2)
                    throw new Exception("Infinite Loop Detected");

                if (firstPixelType != _reader.CurrentPixelType() && _reader.CurrentPixelType() != PixelType.undefined)
                    break;
            }
            while (NextMove()());

            while (_reader.CurrentPixelType() == PixelType.Ground) {
                var hasRows = _reader.PrevRow();
                if (!hasRows)
                    break;
            }

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

        public static IImageParser GetImageParserForDive(Dive dive)
        {
            if (dive == null)
                throw new ArgumentNullException("dive");

            var bmp = new Bitmap(dive.ImagePath);
            var imageReader = new ImageReader(bmp);

            return new ImageParser(imageReader);
        }
    }
}
