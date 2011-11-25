using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FluentAssertions;
using DlImageParsr;
using DlImageParsr.Model;
using DlImageParsr.Orchestration;
using Moq;
using DlImageParsr.Contracts;

namespace DlImageParsrTests
{
    [TestFixture]
    public class SampleFactoryTest
    {
        [Test]
        public void RemoveLastPlainFromPixelList__wont_fail_on_null_or_empty()
        {
            var dive = new Dive(1, 1, 2, "image");
            var fac = new SampleFactory();
            fac.RemoveLastPlainFromPixelList(null);

            fac.RemoveLastPlainFromPixelList(new List<Pixel> { });

            true.Should().BeTrue();
        }

        [Test]
        public void RemoveLastPlainFromPixelList__removes_the_last_pixels_from_list()
        {
            var dive = new Dive(1, 1, 2, "image");
            var pixel = (new[] { new Pixel(1, 1), new Pixel(2, 3), new Pixel(3, 5), new Pixel(4, 1), new Pixel(5, 1), new Pixel(6, 1), new Pixel(7, 1), new Pixel(8, 1) }).ToList();

            var fac = new SampleFactory();
            fac.RemoveLastPlainFromPixelList(pixel);

            pixel.Count().Should().Be(4);
            pixel[0].X.Should().Be(1);
            pixel[1].X.Should().Be(2);
            pixel[2].X.Should().Be(3);
            pixel[3].X.Should().Be(4);
        }

        [Test]
        public void GetDepthResolution__wont_fail_on_null_or_empty()
        {
            var dive = new Dive(1, 1, 2, "image");
            var fac = new SampleFactory();
            var ret1 = fac.GetDepthResolution(null, null);

            var ret2 = fac.GetDepthResolution(new List<Pixel> { }, null);

            ret1.Should().Be(-1);
            ret2.Should().Be(-1);
        }

        [Test]
        public void GetDepthResolution__returns_correct_value()
        {
            var pixel = (new[] { new Pixel(1, 1), new Pixel(2, 3), new Pixel(3, 5), new Pixel(4, 10), new Pixel(5, 1), new Pixel(6, 1), new Pixel(7, 1), new Pixel(8, 1) }).ToList();
            var dive = new Dive(2, 30, 2, "image");
            var fac = new SampleFactory();
            var ret1 = fac.GetDepthResolution(pixel, dive);

            ret1.Should().Be(3);
        }

        [Test]
        public void GetDepthResolution__returns_correct_value_2()
        {
            var pixel = (new[] { new Pixel(1, 1), new Pixel(2, 3), new Pixel(3, 15), new Pixel(4, 10), new Pixel(5, 1), new Pixel(6, 1), new Pixel(7, 1), new Pixel(8, 1) }).ToList();
            var dive = new Dive(2, 30, 2, "image");
            var fac = new SampleFactory();
            var ret1 = fac.GetDepthResolution(pixel, dive);

            ret1.Should().Be(2);
        }

        [Test]
        public void GetDepthResolution__returns_correct_value_with_rounding_up()
        {
            var pixel = (new[] { new Pixel(1, 1), new Pixel(2, 16), new Pixel(3, 15), new Pixel(4, 10), new Pixel(5, 1), new Pixel(6, 1), new Pixel(7, 1), new Pixel(8, 1) }).ToList();
            var dive = new Dive(2, 30, 2, "image");
            var fac = new SampleFactory();
            var ret1 = fac.GetDepthResolution(pixel, dive);

            ret1.Should().Be(2);
        }

        [Test]
        public void GetDepthResolution__returns_correct_value_with_rounding_down()
        {
            var pixel = (new[] { new Pixel(0, 0), new Pixel(2, 21), new Pixel(3, 15), new Pixel(4, 10), new Pixel(5, 1), new Pixel(6, 1), new Pixel(7, 1), new Pixel(8, 1) }).ToList();
            var dive = new Dive(2, 30, 2, "image");
            var fac = new SampleFactory();
            var ret1 = fac.GetDepthResolution(pixel, dive);

            ret1.Should().Be(1);
        }

        [Test]
        public void GetDepthResolution__uses_origin_and_returns_correct_value()
        {
            var pixel = (new[] { new Pixel(51, 71), new Pixel(52, 73), new Pixel(53, 75), new Pixel(54, 80), new Pixel(55, 71), new Pixel(56, 71), new Pixel(57, 71), new Pixel(58, 71) }).ToList();
            var dive = new Dive(2, 30, 2, "image");
            var fac = new SampleFactory();
            var ret1 = fac.GetDepthResolution(pixel, dive);

            ret1.Should().Be(3);
        }

        [Test]
        public void GetTimeResolution__removes_the_last_pixels_from_list()
        {
            var dive = new Dive(1, 1, 2, "image");
            var pixel = (new[] { new Pixel(1, 1), new Pixel(2, 3), new Pixel(3, 5), new Pixel(4, 1), new Pixel(5, 1), new Pixel(6, 1), new Pixel(7, 1), new Pixel(8, 1) }).ToList();

            var fac = new SampleFactory();
            fac.GetTimeResolution(pixel, dive);

            pixel.Count().Should().Be(4);
        }

        [Test]
        public void GetTimeResolution__gets_resolution()
        {
            var dive = new Dive(1, 1, 30, "image");
            var pixel = (new[] { new Pixel(1, 1), new Pixel(2, 3), new Pixel(3, 5), new Pixel(4, 1) }).ToList();

            var fac = new SampleFactory();
            var resoluion = fac.GetTimeResolution(pixel, dive);

            resoluion.Should().Be(10);
        }

        [Test]
        public void GetTimeResolution__gets_resolution_round_up()
        {
            var dive = new Dive(1, 1, 68, "image");
            var pixel = (new[] { new Pixel(1, 1), new Pixel(2, 3), new Pixel(3, 5), new Pixel(4, 5), new Pixel(5, 3), new Pixel(6, 1) }).ToList();

            var fac = new SampleFactory();
            var resoluion = fac.GetTimeResolution(pixel, dive);

            resoluion.Should().Be(14);
        }

        [Test]
        public void GetTimeResolution__gets_resolution_round_down()
        {
            var dive = new Dive(1, 1, 66, "image");
            var pixel = (new[] { new Pixel(1, 1), new Pixel(2, 3), new Pixel(3, 5), new Pixel(4, 5), new Pixel(5, 2), new Pixel(6, 1) }).ToList();

            var fac = new SampleFactory();
            var resoluion = fac.GetTimeResolution(pixel, dive);

            resoluion.Should().Be(13);
        }

        [Test]
        public void Create__result_contains_samples()
        {
            var dive = new Dive(1, 400, 400, "image");
            var pixel = (new[] { new Pixel(1, 1), new Pixel(2, 3), new Pixel(3, 5), new Pixel(4, 5), new Pixel(5, 1) }).ToList();

            var fac = new SampleFactory();

            var processedDive = fac.Create(pixel, dive);
            processedDive.Samples.Count.Should().Be(pixel.Count);
        }

        [Test]
        public void Create__samples_have_correct_values()
        {
            var dive = new Dive(1, 400, 400, "image");
            var pixel = (new[] { new Pixel(0, 0), new Pixel(1, 2), new Pixel(2, 4), new Pixel(3, 4), new Pixel(4, 0) }).ToList();

            var fac = new SampleFactory();

            var processedDive = fac.Create(pixel, dive);

            processedDive.Samples[0].Depth.Should().Be(0);
            processedDive.Samples[0].SecondsSinceStart.Should().Be(0);

            processedDive.Samples[1].Depth.Should().Be(200);
            processedDive.Samples[1].SecondsSinceStart.Should().Be(100);

            processedDive.Samples[2].Depth.Should().Be(400);
            processedDive.Samples[2].SecondsSinceStart.Should().Be(200);

            processedDive.Samples[3].Depth.Should().Be(400);
            processedDive.Samples[3].SecondsSinceStart.Should().Be(300);

            processedDive.Samples[4].Depth.Should().Be(0);
            processedDive.Samples[4].SecondsSinceStart.Should().Be(400);
        }

        [Test]
        public void Create__using_first_pixel_as_origin_and_returns_samples_having_correct_values()
        {
            var dive = new Dive(1, 400, 400, "image");
            var pixel = (new[] { new Pixel(70, 50), new Pixel(71, 52), new Pixel(72, 54), new Pixel(73, 54), new Pixel(74, 50) }).ToList();

            var fac = new SampleFactory();

            var processedDive = fac.Create(pixel, dive);

            processedDive.Samples[0].Depth.Should().Be(0);
            processedDive.Samples[0].SecondsSinceStart.Should().Be(0);

            processedDive.Samples[1].Depth.Should().Be(200);
            processedDive.Samples[1].SecondsSinceStart.Should().Be(100);

            processedDive.Samples[2].Depth.Should().Be(400);
            processedDive.Samples[2].SecondsSinceStart.Should().Be(200);

            processedDive.Samples[3].Depth.Should().Be(400);
            processedDive.Samples[3].SecondsSinceStart.Should().Be(300);

            processedDive.Samples[4].Depth.Should().Be(0);
            processedDive.Samples[4].SecondsSinceStart.Should().Be(400);
        }

        [Test]
        public void Create__calculate_average_for_skip_pixels()
        {
            throw new NotImplementedException("implement");
            var dive = new Dive(1, 400, 400, "image");
            var pixel = (new[] { new Pixel(0, 0), new SkipPixel(1, 2), new SkipPixel(2, 4), new Pixel(3, 4), new Pixel(4, 0) }).ToList();

            var fac = new SampleFactory();
            var processedDive = fac.Create(pixel, dive);

            processedDive.Samples.Should().HaveCount(3);

            processedDive.Samples[0].Depth.Should().Be(0);
            processedDive.Samples[0].SecondsSinceStart.Should().Be(0);

            processedDive.Samples[1].Depth.Should().Be(400);
            processedDive.Samples[1].SecondsSinceStart.Should().Be(300);

            processedDive.Samples[2].Depth.Should().Be(0);
            processedDive.Samples[2].SecondsSinceStart.Should().Be(400);
        }

        [Test]
        public void Create__runs_post_processing()
        {
            var dive = new Dive(1, 400, 400, "image");
            var pixel = (new[] { new Pixel(0, 0), new SkipPixel(1, 2), new SkipPixel(2, 4), new Pixel(3, 4), new Pixel(4, 0) }).ToList();
            var postProcess = new Mock<IPostProcessing>();

            var fac = new SampleFactory();
            fac.AddPostProcessing(postProcess.Object);

            var processedDive = fac.Create(pixel, dive);

            postProcess.Verify(pp => pp.Process(processedDive), Times.Once());
        }

        [Test]
        public void Create__runs_mulitple_post_processing()
        {
            var dive = new Dive(1, 400, 400, "image");
            var pixel = (new[] { new Pixel(0, 0), new SkipPixel(1, 2), new SkipPixel(2, 4), new Pixel(3, 4), new Pixel(4, 0) }).ToList();
            var postProcess1 = new Mock<IPostProcessing>();
            var postProcess2 = new Mock<IPostProcessing>();

            var fac = new SampleFactory();
            fac.AddPostProcessing(postProcess1.Object);
            fac.AddPostProcessing(postProcess2.Object);

            var processedDive = fac.Create(pixel, dive);

            postProcess1.Verify(pp => pp.Process(processedDive), Times.Once());
            postProcess2.Verify(pp => pp.Process(processedDive), Times.Once());
        }

        [Test]
        public void AddPostProcessing__adds_post_processing()
        {
            var postProcessMock = new Mock<IPostProcessing>();
            var postProcessObj = postProcessMock.Object;
            var fac = new SampleFactory();
            fac.AddPostProcessing(postProcessObj);

            fac.PostProcessingFilters.Should().Contain(postProcessObj);
        }
    }
}
