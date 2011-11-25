using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FluentAssertions;
using DlImageParsr.Model;

namespace DlImageParsrTests.Model
{
    [TestFixture]
    public class ProcessedDiveTest
    {
        [Test]
        public void ctor__creates_dive_from_dive()
        {
            var dive = new Dive(3, 5, 7, "Testimages/outputimage.2.bmp");

            var proc = new ProcessedDive(dive);

            proc.ImagePath.Should().Be(dive.ImagePath);
            proc.MaxDepthInCentimeters.Should().Be(dive.MaxDepthInCentimeters);
            proc.DiveLogId.Should().Be(dive.DiveLogId);
            proc.DurationinSeconds.Should().Be(dive.DurationinSeconds);
        }

        [Test]
        public void ctor__creates_dive_from_dive_2()
        {
            var dive = new Dive(1, 233, 287, "Testimages/outputimage.8.bmp");

            var proc = new ProcessedDive(dive);

            proc.ImagePath.Should().Be(dive.ImagePath);
            proc.MaxDepthInCentimeters.Should().Be(dive.MaxDepthInCentimeters);
            proc.DiveLogId.Should().Be(dive.DiveLogId);
            proc.DurationinSeconds.Should().Be(dive.DurationinSeconds);
        }

        [Test]
        public void ctor__creates_dive_containg_initilized_sample_list()
        {
            var dive = new Dive(1, 233, 287, "Testimages/outputimage.8.bmp");

            var proc = new ProcessedDive(dive);

            proc.Samples.Should().NotBeNull();
        }

        [Test]
        public void SampleRateInSeconds__calculates_sample_rate()
        {
            var dive = new Dive(1, 233, 300, "Testimages/outputimage.8.bmp");

            var proc = new ProcessedDive(dive);
            proc.Samples.AddRange(new[] { new Sample(100, 222), new Sample(50, 222), new Sample(100, 222), new Sample(50, 222), new Sample(100, 222), new Sample(50, 222) });

            proc.SampleRateInSeconds.Should().Be(50);
        }

        [Test]
        public void SampleRateInSeconds__updates_sample_rate()
        {
            var dive = new Dive(1, 233, 300, "Testimages/outputimage.8.bmp");

            var proc = new ProcessedDive(dive);
            proc.Samples.AddRange(new[] { new Sample(100, 222), new Sample(50, 222), new Sample(100, 222), new Sample(50, 222), new Sample(100, 222), new Sample(50, 222) });

            proc.SampleRateInSeconds.Should().Be(50);

            proc.Samples.RemoveRange(0, 3);
            proc.SampleRateInSeconds.Should().Be(100);
        }

        [Test]
        public void SampleRateInSeconds__can_handle_no_samples() {
            var dive = new Dive(1, 233, 300, "Testimages/outputimage.8.bmp");

            var proc = new ProcessedDive(dive);

            proc.SampleRateInSeconds.Should().Be(0);
        }

    }
}
