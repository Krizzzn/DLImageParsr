using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using DlImageParsr.Filter;
using DlImageParsr.Model;

namespace DlImageParsrTests
{
    [TestFixture]
    public class ReduceSamplesTest
    {
        [Test]
        public void Process__throws_on_null()
        {
            ReduceSamples rds = new ReduceSamples();

            Action a = () => rds.Process(null);

            a.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void Process__reduces_sample_count_by_factor_2()
        {
            ReduceSamples rds = new ReduceSamples();
            ProcessedDive pd = new ProcessedDive(new Dive(15, 500, 300, ""));
            pd.Samples.AddRange(new[] { new Sample(100, 222), new Sample(100, 222), new Sample(100, 222), new Sample(100, 222), new Sample(100, 222), new Sample(100, 222) });

            rds.Process(pd);

            pd.Samples.Should().HaveCount(3);
        }

        [Test]
        public void Process__removes_every_second_sample()
        {
            ReduceSamples rds = new ReduceSamples();
            ProcessedDive pd = new ProcessedDive(new Dive(15, 500, 300, ""));
            pd.Samples.AddRange(new[] { new Sample(100, 222), new Sample(50, 222), new Sample(100, 222), new Sample(50, 222), new Sample(100, 222), new Sample(50, 222) });

            rds.Process(pd);

            pd.Samples.Where(m => m.Depth == 50).Should().HaveCount(0);
        }

        [Test]
        public void Process__increases_sample_resolution()
        {
            ReduceSamples rds = new ReduceSamples();
            ProcessedDive pd = new ProcessedDive(new Dive(15, 500, 300, ""));
            pd.Samples.AddRange(new[] { new Sample(100, 222), new Sample(50, 222), new Sample(100, 222), new Sample(50, 222), new Sample(100, 222), new Sample(50, 222) });

            rds.Process(pd);

            pd.SampleRateInSeconds.Should().Be(300 / 3);
        }
    }
}
