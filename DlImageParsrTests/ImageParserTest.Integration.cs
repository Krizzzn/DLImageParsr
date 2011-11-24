using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using FluentAssertions.Assertions;
using FluentAssertions.Common;
using Moq;
using NUnit.Framework;
using DlImageParsr;
using System.Drawing;
using DlImageParsr.ImageParsing;
using DlImageParsr.Model;

namespace DlImageParsrTests.Integration
{
    public class ImageParserTest
    {
        [Test(Description = "Integration")]
        public void ReadCurrentRow__parses_images()
        {
            var bmp = new Bitmap("Testimages/testimage1.png");
            try {
                var imageRead = new ImageReader(bmp);

                while (imageRead.CurrentColumn < 25)
                    imageRead.NextColumn();
                var imageParser = new ImageParser(imageRead);
                var result = imageParser.ReadCurrentRow();

                result.Y.Should().BeGreaterThan(45);
                result.X.Should().BeGreaterThan(45);
            }
            finally {
                bmp.Dispose();
            }
        }

        [Test(Description = "Integration")]
        public void ReadDocument__parses_images()
        {
            var bmp = new Bitmap("Testimages/testimage1.png");
            try {
                var imageRead = new ImageReader(bmp);
                var imageParser = new ImageParser(imageRead);

                IEnumerable<Pixel> pixels = null;

                Action action = () => { pixels = imageParser.ReadDocument(); };
                action.ExecutionTime().ShouldNotExceed(1.Seconds());

                pixels.Should().HaveCount((c) => c > 200);
            }
            finally {
                bmp.Dispose();

            }
        }

        [Test(Description = "Integration")]
        public void ReadDocument__parses_images_contains_skip_pixels()
        {
            var bmp = new Bitmap("Testimages/testimage1.png");
            try {
                var imageRead = new ImageReader(bmp);
                var imageParser = new ImageParser(imageRead);

                IEnumerable<Pixel> pixels = imageParser.ReadDocument();

                pixels.Should().Contain(pxl => pxl is SkipPixel);
            }
            finally {
                bmp.Dispose();
            }
        }

        [Test(Description = "Integration")]
        public void ReadDocument__write_read_pixels_onto_image()
        {
            var bmp = new Bitmap(@"Testimages/testimage3.bmp");
            try {
                var imageRead = new ImageReader(bmp);
                var imageParser = new ImageParser(imageRead);

                IEnumerable<Pixel> pixels = imageParser.ReadDocument();

                foreach (Pixel p in pixels)
                    bmp.SetPixel(p.X, p.Y, Color.DeepPink);
                bmp.Save("Testimages/outputimage.1.bmp");

                System.IO.File.Exists("Testimages/outputimage.1.bmp").Should().BeTrue();

            }
            finally {
                bmp.Dispose();
            }
        }

        [Test(Description = "Integration")]
        public void ReadDocument__edge_cases_edge_detection_1()
        {
            var bmp = new Bitmap(@"Testimages/TestCase1.bmp");
            try {
                var imageRead = new ImageReader(bmp);
                imageRead.FrameHeight = bmp.Height;
                imageRead.FrameWidth = bmp.Width;
                imageRead.FrameX = imageRead.FrameY = 0;

                var imageParser = new ImageParser(imageRead);

                var pixels = imageParser.ReadDocument().ToList();
                pixels.ForEach(p => bmp.SetPixel(p.X, p.Y, Color.DeepPink));
                bmp.Save("Testimages/outputimage.2.bmp");

                pixels[4].Y.Should().BeGreaterThan(pixels[3].Y + 5);

                System.IO.File.Exists("Testimages/outputimage.2.bmp").Should().BeTrue();

            }
            finally {
                bmp.Dispose();
            }
        }

        [Test(Description = "Integration")]
        public void ReadDocument__edge_cases_edge_detection_2()
        {
            var bmp = new Bitmap(@"Testimages/TestCase2.bmp");
            try {
                var imageRead = new ImageReader(bmp);
                imageRead.FrameHeight = bmp.Height;
                imageRead.FrameWidth = bmp.Width;
                imageRead.FrameX = imageRead.FrameY = 0;

                var imageParser = new ImageParser(imageRead);

                var pixels = imageParser.ReadDocument().ToList();
                pixels.ForEach(p => bmp.SetPixel(p.X, p.Y, Color.DeepPink));
                bmp.Save("Testimages/outputimage.3.bmp");

                pixels[17].Y.Should().NotBe(pixels[16].Y);
                pixels[18].Y.Should().NotBe(pixels[17].Y);

                System.IO.File.Exists("Testimages/outputimage.3.bmp").Should().BeTrue();

            }
            finally {
                bmp.Dispose();
            }
        }

        [Test]
        public void GetImageParserForDive__throws_on_null()
        {
            Action a = () => { ImageParser.GetImageParserForDive(null); };

            a.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void GetImageParserForDive__gets_valid_parser_for_dive()
        {
            Dive d = new Dive(1, 2, 3, "Testimages/outputimage.3.bmp");
            var result = ImageParser.GetImageParserForDive(d);

            result.Should().NotBeNull();
            result.Dispose();
        }

    }
}
