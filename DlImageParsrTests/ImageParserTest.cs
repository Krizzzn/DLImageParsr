using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using DlImageParsr;
using System.Drawing;

namespace DlImageParsrTests
{
    public class ImageParserTest
    {
        [Test]
        public void NextMove__moves_down_if_pixel_type_water()
        {
            var called = false;
            var reader = new Mock<IImageReader>();
            reader.Setup(i => i.CurrentPixelType()).Returns(PixelType.Water);
            reader.Setup(i => i.NextRow()).Callback(() => called = true);

            var imageParser = new ImageParser(reader.Object);
            var move = imageParser.NextMove();

            called.Should().BeFalse();
            move();
            called.Should().BeTrue();
        }

        [Test]
        public void NextMove__moves_up_if_pixel_type_ground()
        {
            var called = false;
            var reader = new Mock<IImageReader>();
            reader.Setup(i => i.CurrentPixelType()).Returns(PixelType.Ground);
            reader.Setup(i => i.PrevRow()).Callback(() => called = true);

            var imageParser = new ImageParser(reader.Object);
            var move = imageParser.NextMove();

            called.Should().BeFalse();
            move();
            called.Should().BeTrue();
        }

        [Test]
        public void NextMove__returns_last_move_if_pixel_type_undefined()
        {
            var called = 0;
            var reader = new Mock<IImageReader>();
            reader.Setup(i => i.CurrentPixelType()).Returns(() => { return (called == 0) ? PixelType.Water : PixelType.undefined; });
            reader.Setup(i => i.NextRow()).Callback(() => called++);

            var imageParser = new ImageParser(reader.Object);
            var firstMove = imageParser.NextMove();
            firstMove();
            var secondMove = imageParser.NextMove();
            secondMove();
            var thirdMove = imageParser.NextMove();
            thirdMove();

            called.Should().Be(3);
            firstMove.Should().Be(secondMove).And.Be(thirdMove);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void ReadCurrentRow__throws_exception_on_infinite_loop()
        {
            var reader = new Mock<IImageReader>();
            reader.Setup(i => i.FrameHeight).Returns(50);
            reader.Setup(i => i.NextRow()).Returns(true);

            var imageParser = new ImageParser(reader.Object);
            var res = imageParser.ReadCurrentRow();
        }

        [Test]
        public void ReadCurrentRow__moves_through_complete_row()
        {
            var p = new Pixel(50, 25);
            var called = 0;
            var reader = new Mock<IImageReader>();
            reader.Setup(i => i.CurrentPixelType()).Returns(PixelType.Water);
            reader.Setup(i => i.NextRow()).Returns(() => { called++; return called < 50; });
            reader.Setup(i => i.CurrentPixel).Returns(p);
            reader.Setup(i => i.FrameHeight).Returns(50);

            var imageParser = new ImageParser(reader.Object);

            var res = imageParser.ReadCurrentRow();

            res.Should().Be(p);
            called.Should().BeGreaterOrEqualTo(49);
        }

        [Test]
        public void ReadCurrentRow__stops_loop_on_changeing_pixel_type()
        {
            var p = new Pixel(50, 25);
            var called = 0;
            var reader = new Mock<IImageReader>();
            reader.Setup(i => i.CurrentPixelType()).Returns(() => { return (called < 5) ? PixelType.Water : PixelType.Ground; });
            reader.Setup(i => i.NextRow()).Returns(() => { return called++ < 50; });
            reader.Setup(i => i.PrevRow()).Returns(() => { return false; });
            reader.Setup(i => i.CurrentPixel).Returns(p);
            reader.Setup(i => i.FrameHeight).Returns(50);

            var imageParser = new ImageParser(reader.Object);

            var res = imageParser.ReadCurrentRow();

            res.Should().Be(p);
            called.Should().BeLessOrEqualTo(5);
        }

        [Test]
        public void ReadCurrentRow__stops_loop_on_changeing_pixel_type_skipping_undefined_pixels()
        {
            var p = new Pixel(50, 25);
            var called = 0;
            var reader = new Mock<IImageReader>();
            reader.Setup(i => i.CurrentPixelType()).Returns(() => { if (called < 5) return PixelType.Water; else if (called == 5) return PixelType.undefined; else return PixelType.Ground; });
            reader.Setup(i => i.NextRow()).Returns(() => { return called++ < 50; });
            reader.Setup(i => i.PrevRow()).Returns(() => { return called++ < 50; });
            reader.Setup(i => i.CurrentPixel).Returns(p);
            reader.Setup(i => i.FrameHeight).Returns(50);

            var imageParser = new ImageParser(reader.Object);

            var res = imageParser.ReadCurrentRow();

            res.Should().Be(p);
            called.Should().BeGreaterThan(5);
        }

        [Test]
        public void ReadCurrentRow__stops_loop_on_changeing_pixel_type_and_ends_on_a_water_type()
        {
            var p = new Pixel(50, 25);
            var called = 0;
            var reader = new Mock<IImageReader>();
            reader.Setup(i => i.CurrentPixelType()).Returns(() => { return (called < 1) ? PixelType.Water : PixelType.Ground; });
            reader.Setup(i => i.NextRow()).Returns(() => { called++; return true; });
            reader.Setup(i => i.PrevRow()).Returns(() => { called--; return true; });
            reader.Setup(i => i.FrameHeight).Returns(50);

            var imageParser = new ImageParser(reader.Object);

            var res = imageParser.ReadCurrentRow();

            reader.Object.CurrentPixelType().Should().Be(PixelType.Water);
        }

        [Test]
        public void ReadCurrentRow__stops_loop_on_changeing_pixel_type_trys_to_get_water_but_ends_if_PrevRow_returns_false()
        {
            var p = new Pixel(50, 25);
            var called = 0;
            var reader = new Mock<IImageReader>();
            reader.Setup(i => i.CurrentPixelType()).Returns(() => { return (called < 1) ? PixelType.Water : PixelType.Ground; });
            reader.Setup(i => i.NextRow()).Returns(() => { called++; return true; });
            reader.Setup(i => i.PrevRow()).Returns(() => { return false; });
            reader.Setup(i => i.FrameHeight).Returns(50);

            var imageParser = new ImageParser(reader.Object);

            var res = imageParser.ReadCurrentRow();

            reader.Object.CurrentPixelType().Should().Be(PixelType.Ground);
        }

        [Test]
        public void ReadDocument__loops_through_all_columns()
        {
            var columns = 0;
            var called = 0;
            var reader = new Mock<IImageReader>();
            reader.Setup(i => i.CurrentPixelType()).Returns(() => { return (called % 5 == 1) ? PixelType.Water : PixelType.Ground; });
            reader.Setup(i => i.NextRow()).Returns(() => { called++; return true; });
            reader.Setup(i => i.PrevRow()).Returns(() => { called++; return true; });
            reader.Setup(i => i.FrameHeight).Returns(50);
            reader.Setup(i => i.NextColumn()).Returns(() => { return (columns++ < 20); });

            var imageParser = new ImageParser(reader.Object);
            var pixels = imageParser.ReadDocument();
            pixels.Should().HaveCount((c) => c >= 20 );
        }

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
    }
}
