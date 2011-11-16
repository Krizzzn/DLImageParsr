using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using FluentAssertions;
using DlImageParsr;
using System.Drawing;
using DlImageParsr.ImageParsing;
using DlImageParsr.Model;

namespace DlImageParsrTests.Integration
{
    [TestFixture]
    public class ImageReaderTest
    {
        [Test(Description = "Integration")]
        public void CurrentPixelType__receives_pixel_from_image_typed_as_water()
        {
            var bmp = new Bitmap("Testimages/testimage1.png");
            try {
                var imageRead = new ImageReader(bmp);
                int i = 0;
                while (i++ < 50)
                    imageRead.NextColumn();

                var result = imageRead.CurrentPixelType();

                imageRead.CurrentColumn.Should().BeGreaterThan(30);
                result.Should().Be(PixelType.Water);
            }
            finally {
                bmp.Dispose();
            }
        }

        [Test(Description = "Integration")]
        public void CurrentPixelType__receives_pixel_from_image_typed_as_ground()
        {
            var bmp = new Bitmap("Testimages/testimage1.png");
            try {
                var imageRead = new ImageReader(bmp);
                int i = 0;
                while (i++ < 42)
                    imageRead.NextRow();

                var result = imageRead.CurrentPixelType();

                imageRead.CurrentRow.Should().BeGreaterThan(40);
                result.Should().Be(PixelType.Ground);
            }
            finally {
                bmp.Dispose();
            }
        }

        [Test(Description = "Integration")]
        public void CurrentPixelType__receives_pixel_from_image_typed_as_undefined()
        {
            var bmp = new Bitmap("Testimages/testimage1.png");
            try {
                var imageRead = new ImageReader(bmp);

                imageRead.FrameX--;
                imageRead.FrameY--;

                var result = imageRead.CurrentPixelType();

                result.Should().Be(PixelType.undefined);
            }
            finally {
                bmp.Dispose();
            }
        }

        [Test(Description = "Integration")]
        public void CurrentPixelType__origin_of_coordinate_system_is_correct()
        {
            var bmp = new Bitmap("Testimages/testimage1.png");
            try {
                var imageRead = new ImageReader(bmp);

                var result = imageRead.CurrentPixelType();

                imageRead.CurrentColumn.Should().Be(0);
                imageRead.CurrentRow.Should().Be(0);
                result.Should().Be(PixelType.Water);
            }
            finally {
                bmp.Dispose();
            }
        }
    }
}
