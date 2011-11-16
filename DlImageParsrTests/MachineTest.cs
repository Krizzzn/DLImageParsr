using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using DlImageParsr.Orchestration;
using DlImageParsr.Contracts;

namespace DlImageParsrTests
{
    [TestFixture]
    public class MachineTest
    {

        [TestCase(true, false)]
        [TestCase(false, true)]
        [Test]
        public void ctor__fails_on_null(bool p1, bool p3)
        {

            Action act = () => {
                new Machine((p1) ? new Mock<IDiveRepository>().Object : null,
                            (p3) ? new Mock<ISampleFactory>().Object : null);
            };

            act.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void ctor__stores_given_objects_in_properties()
        {
            var dr = new Mock<IDiveRepository>().Object;
            var sf = new Mock<ISampleFactory>().Object;

            var machine = new Machine(dr, sf);

            machine.DiveRepository.Should().Be(dr);
            machine.SampleFactory.Should().Be(sf);
        }

        [Test]
        public void Process__gets_data_from_repository()
        {
            var dr = new Mock<IDiveRepository>();
            var sf = new Mock<ISampleFactory>();


            var machine = new Machine(dr.Object, sf.Object);
            machine.Process();

            dr.Verify(o => o.LoadDives(), Times.Once());
        }

        //[Test]
        //public void Process
    }
}
