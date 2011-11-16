using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using FluentAssertions;
using DlImageParsr;
using System.Drawing;
using DlImageParsr.ImageParsing;
using DlImageParsr.Model;

namespace DlImageParsrTests
{
    [TestFixture]
    public class ImageReaderTest
    {
        [Test]
        public void ctor_initializes_row_and_column()
        {
            var imageRead = new ImageReader((x, y) => PixelType.Ground);

            imageRead.CurrentColumn.Should().Be(0);
            imageRead.CurrentRow.Should().Be(0);
        }

        [Test]
        public void NextRow__steps_to_next_row()
        {
            var imageRead = new ImageReader((x, y) => PixelType.Ground);
            imageRead.FrameHeight = 40;
            var ret = imageRead.NextRow();

            ret.Should().BeTrue();
            imageRead.CurrentRow.Should().Be(1);
        }

        [Test]
        public void NextRow__stops_at_height_boundary()
        {
            var imageRead = new ImageReader((x, y) => PixelType.Ground);
            imageRead.FrameHeight = 2;

            imageRead.NextRow();
            var ret = imageRead.NextRow();

            ret.Should().BeFalse();
            imageRead.CurrentRow.Should().Be(1);
        }

        [Test]
        public void NextColumn__steps_to_next_column()
        {
            var imageRead = new ImageReader((x, y) => PixelType.Ground);
            imageRead.FrameWidth = 40;
            var ret = imageRead.NextColumn();

            ret.Should().BeTrue();
            imageRead.CurrentColumn.Should().Be(1);
        }

        [Test]
        public void NextColumn__stops_at_width_boundary()
        {
            var imageRead = new ImageReader((x, y) => PixelType.Ground);
            imageRead.FrameWidth = 2;

            imageRead.NextColumn();
            var ret = imageRead.NextColumn();

            ret.Should().BeFalse();
            imageRead.CurrentColumn.Should().Be(1);
        }

        [Test]
        public void NextColumn__current_row_stay_same()
        {
            var imageRead = new ImageReader((x, y) => PixelType.Ground);
            imageRead.FrameWidth = 40;
            imageRead.NextRow();
            imageRead.NextRow();
            var currentCol = imageRead.CurrentRow;
            var ret = imageRead.NextColumn();

            ret.Should().BeTrue();
            imageRead.CurrentRow.Should().Be(currentCol);
        }

        [Test]
        public void PreviousRow__steps_to_previous_row()
        {
            var imageRead = new ImageReader((x, y) => PixelType.Ground);
            imageRead.FrameHeight = 40;
            imageRead.NextRow();
            imageRead.NextRow();

            var ret = imageRead.PrevRow();

            ret.Should().BeTrue();
            imageRead.CurrentRow.Should().Be(1);
        }

        [Test]
        public void PreviousRow__stops_at_zero()
        {
            var imageRead = new ImageReader((x, y) => PixelType.Ground);
            imageRead.FrameHeight = 40;

            var ret = imageRead.PrevRow();

            ret.Should().BeFalse();
            imageRead.CurrentRow.Should().Be(0);
        }

        [Test]
        public void CurrentPixelType__returns_pixel_from_image()
        {
            var called = false;
            var imageRead = new ImageReader((x, y) => { called = true; return PixelType.Ground; });
            imageRead.FrameHeight = 40;

            imageRead.CurrentPixelType().Should().Be(PixelType.Ground);
            called.Should().BeTrue();
        }

        [Test]
        public void CurrentPixelType__should_use_coordinate_system_to_pixel_receive()
        {
            var called = false;
            var imageRead = new ImageReader((x, y) => { called = (x == 10 && y == 15); return PixelType.Ground; });
            imageRead.FrameX = 10;
            imageRead.FrameY = 15;
            imageRead.FrameHeight = 40;

            imageRead.CurrentPixelType().Should().Be(PixelType.Ground);
            called.Should().BeTrue();
        }

        [Test]
        public void CurrentPixelType__should_add_coordinate_system_to_movement_for_pixel_receive()
        {
            var called = false;
            var imageRead = new ImageReader((x, y) => { called = (x == 13 && y == 17); return PixelType.Ground; });
            imageRead.FrameX = 10;
            imageRead.FrameY = 15;
            imageRead.FrameHeight = 40;
            imageRead.FrameWidth = 40;

            imageRead.NextRow();
            imageRead.NextRow();

            imageRead.NextColumn();
            imageRead.NextColumn();
            imageRead.NextColumn();

            imageRead.CurrentPixelType().Should().Be(PixelType.Ground);
            called.Should().BeTrue();
        }

        [Test]
        public void CurrentPixel__returns_current_position()
        {
            var imageRead = new ImageReader((x, y) => PixelType.undefined);
            imageRead.FrameX = 10;
            imageRead.FrameY = 15;
            imageRead.FrameHeight = 40;
            imageRead.FrameWidth = 40;

            imageRead.NextRow();
            imageRead.NextRow();

            imageRead.NextColumn();
            imageRead.NextColumn();
            imageRead.NextColumn();

            var result = imageRead.CurrentPixel;

            result.X.Should().Be(13);
            result.Y.Should().Be(17);
        }

        [Test]
        public void NextColumn__Reads_Ahead_and_defines_a_column_as_skip_pixel()
        {
            var imageRead = new ImageReader((x, y) => PixelType.undefined);
            imageRead.NextColumn();
            imageRead.FrameHeight = 40;
            imageRead.FrameWidth = 40;

            imageRead.NextColumn();

            imageRead.CurrentPixelType().Should().Be(PixelType.Skip);
            imageRead.CurrentPixel.Should().BeOfType<SkipPixel>();
            imageRead.NextRow().Should().BeFalse();
        }
    }
}
