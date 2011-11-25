using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using DlImageParsr.Orchestration;
using DlImageParsr.Contracts;
using DlImageParsr.Model;

namespace DlImageParsrTests
{
    [TestFixture]
    public class MachineTest
    {

        [TestCase(true, true, false)]
        [TestCase(false, true, true)]
        [TestCase(true, true, false)]
        [Test]
        public void ctor__fails_on_null(bool p1, bool p3, bool p2)
        {
            Func<DlImageParsr.Model.Dive, IImageParser> theFunc = d => null;
            Action act = () => {
                new Machine((p1) ? new Mock<IDiveRepository>().Object : null,
                            (p3) ? new Mock<ISampleFactory>().Object : null,
                            (p2) ? theFunc : null);
            };

            act.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void ctor__stores_given_objects_in_properties()
        {
            var dr = new Mock<IDiveRepository>().Object;
            var sf = new Mock<ISampleFactory>().Object;
            Func<DlImageParsr.Model.Dive, IImageParser> facFunc = d => new Mock<IImageParser>().Object;

            var machine = new Machine(dr, sf, facFunc);

            machine.DiveRepository.Should().Be(dr);
            machine.SampleFactory.Should().Be(sf);
            machine.ImageParserFactory.Should().Be(facFunc);
        }

        [Test]
        public void Process__gets_data_from_repository()
        {
            var dr = new Mock<IDiveRepository>();
            var sf = new Mock<ISampleFactory>();
            Func<DlImageParsr.Model.Dive, IImageParser> facFunc = d => new Mock<IImageParser>().Object;

            var machine = new Machine(dr.Object, sf.Object, facFunc);
            machine.Process();

            dr.Verify(o => o.LoadDives(), Times.Once());
        }

        [Test]
        public void Process__gets_Image_parser_from_factory_method()
        {
            var accessed = 0;

            var dives = new[] { new Dive(15, 50, 23, ""), new Dive(24, 20, 33, "") };
            var dr = new Mock<IDiveRepository>();
            dr.Setup(inp => inp.LoadDives()).Returns(dives);
            var sf = new Mock<ISampleFactory>();
            Func<DlImageParsr.Model.Dive, IImageParser> facFunc = d => { accessed += d.DiveLogId; return new Mock<IImageParser>().Object; };

            var machine = new Machine(dr.Object, sf.Object, facFunc);

            machine.Process();

            accessed.Should().Be(39);
        }

        [Test]
        public void Process__raises_message_before_processing_dive()
        {
            var accessed = 0;

            var dives = new[] { new Dive(15, 50, 23, ""), new Dive(24, 20, 33, "") };
            var dr = new Mock<IDiveRepository>();
            dr.Setup(inp => inp.LoadDives()).Returns(dives);
            var sf = new Mock<ISampleFactory>();
            Func<DlImageParsr.Model.Dive, IImageParser> facFunc = d => { return new Mock<IImageParser>().Object; };

            var machine = new Machine(dr.Object, sf.Object, facFunc);
            machine.ProcessMessage += (d) => accessed++;

            machine.Process();

            accessed.Should().BeGreaterOrEqualTo(2);
        }

        [Test]
        public void Process__gets_Image_parser_from_factory_method_but_not_for_null_values()
        {
            var accessed = 0;

            var dives = new[] { new Dive(15, 50, 23, ""), new Dive(24, 20, 33, ""), null };
            var dr = new Mock<IDiveRepository>();
            dr.Setup(inp => inp.LoadDives()).Returns(dives);
            var sf = new Mock<ISampleFactory>();
            Func<DlImageParsr.Model.Dive, IImageParser> facFunc = d => { accessed += d.DiveLogId; return new Mock<IImageParser>().Object; };

            var machine = new Machine(dr.Object, sf.Object, facFunc);

            machine.Process();

            accessed.Should().Be(39);
        }

        [Test]
        public void Process__gets_parses_the_images()
        {
            var accessed = 0;

            var parserMock = new Mock<IImageParser>(); 
            
            var dives = new[] { new Dive(14, 50, 23, ""), new Dive(24, 20, 33, "") };
            var dr = new Mock<IDiveRepository>();
            dr.Setup(inp => inp.LoadDives()).Returns(dives);
            var sf = new Mock<ISampleFactory>();
            Func<DlImageParsr.Model.Dive, IImageParser> facFunc = d => { if (dives.Contains(d)) accessed++; return parserMock.Object; };
             
            var machine = new Machine(dr.Object, sf.Object, facFunc);

            machine.Process();

            parserMock.Verify(d => d.ReadDocument(), Times.Exactly(2));
        }

        [Test]
        public void Process__put_pixels_into_SampleFactory()
        {
            var accessed = 0;

            var parserMock = new Mock<IImageParser>();
            var pixels = new [] { new Pixel(2,3), new Pixel(3,4), new Pixel(5,4) };
            parserMock.Setup(d => d.ReadDocument()).Returns(pixels);

            var dives = new[] { new Dive(14, 50, 23, ""), new Dive(24, 20, 33, "") };
            var dr = new Mock<IDiveRepository>();
            dr.Setup(inp => inp.LoadDives()).Returns(dives);
            
            var sf = new Mock<ISampleFactory>();
           
            Func<DlImageParsr.Model.Dive, IImageParser> facFunc = d => { if (dives.Contains(d)) accessed++; return parserMock.Object; };
            var machine = new Machine(dr.Object, sf.Object, facFunc);

            machine.Process();

            sf.Verify(inp => inp.Create(pixels, dives[0]), Times.Once());
            sf.Verify(inp => inp.Create(pixels, dives[1]), Times.Once());
        }

        [Test]
        public void Process__dont_put_pixels_into_SampleFactory_but_not_for_null_values()
        {
            var accessed = 0;

            var parserMock = new Mock<IImageParser>();
            Pixel[] pixels = null;
            parserMock.Setup(d => d.ReadDocument()).Returns(pixels);

            var dives = new[] { new Dive(14, 50, 23, ""), new Dive(24, 20, 33, "") };
            var dr = new Mock<IDiveRepository>();
            dr.Setup(inp => inp.LoadDives()).Returns(dives);

            var sf = new Mock<ISampleFactory>();

            Func<DlImageParsr.Model.Dive, IImageParser> facFunc = d => { if (dives.Contains(d)) accessed++; return parserMock.Object; };
            var machine = new Machine(dr.Object, sf.Object, facFunc);

            machine.Process();

            sf.Verify(inp => inp.Create(pixels, It.IsAny<Dive>()), Times.Never());
            parserMock.Verify(inp => inp.Dispose(), Times.Exactly(2));
        }



        [Test]
        public void Process__dont_put_pixels_into_SampleFactory_but_not_for_empty_pixel_array()
        {
            var parserMock = new Mock<IImageParser>();
            Pixel[] pixels = new Pixel[0];
            parserMock.Setup(d => d.ReadDocument()).Returns(pixels);

            var dives = new[] { new Dive(14, 50, 23, ""), new Dive(24, 20, 33, "") };
            var dr = new Mock<IDiveRepository>();
            dr.Setup(inp => inp.LoadDives()).Returns(dives);

            var sf = new Mock<ISampleFactory>();

            Func<DlImageParsr.Model.Dive, IImageParser> facFunc = d => { return parserMock.Object; };
            var machine = new Machine(dr.Object, sf.Object, facFunc);

            machine.Process();

            sf.Verify(inp => inp.Create(pixels, It.IsAny<Dive>()), Times.Never());
            parserMock.Verify(inp => inp.Dispose(), Times.Exactly(2));
        }

        [Test]
        public void Process__disposes_the_image_parser()
        {
            var parserMock = new Mock<IImageParser>();
            var pixels = new[] { new Pixel(2, 3), new Pixel(3, 4), new Pixel(5, 4) };
            parserMock.Setup(d => d.ReadDocument()).Returns(pixels);

            var dives = new[] { new Dive(14, 50, 23, "")};
            var dr = new Mock<IDiveRepository>();
            dr.Setup(inp => inp.LoadDives()).Returns(dives);

            var sf = new Mock<ISampleFactory>();


            Func<DlImageParsr.Model.Dive, IImageParser> facFunc = d => { return parserMock.Object; };

            var machine = new Machine(dr.Object, sf.Object, facFunc);

            machine.Process();

            parserMock.Verify(dd => dd.Dispose(), Times.Once());
        }

        [Test]
        public void Process__raises_message_event_before_saving_dive()
        {
            var accessed = 0;

            var parserMock = new Mock<IImageParser>();
            var pixels = new[] { new Pixel(2, 3), new Pixel(3, 4), new Pixel(5, 4) };
            parserMock.Setup(d => d.ReadDocument()).Returns(pixels);

            var dives = new[] { new Dive(14, 50, 23, ""), new Dive(24, 20, 33, "") };
            var dr = new Mock<IDiveRepository>();
            dr.Setup(inp => inp.LoadDives()).Returns(dives);
            
            var sf = new Mock<ISampleFactory>();
            var processed = new[] { new ProcessedDive(dives[0]), new ProcessedDive(dives[0]) };
            sf.Setup(inp => inp.Create(It.IsAny< IEnumerable<Pixel>>(), dives[0])).Returns(processed[0]);
            sf.Setup(inp => inp.Create(It.IsAny < IEnumerable<Pixel>>(), dives[1])).Returns(processed[1]);

            Func<DlImageParsr.Model.Dive, IImageParser> facFunc = d => { return parserMock.Object; };

            var machine = new Machine(dr.Object, sf.Object, facFunc);
            machine.ProcessMessage += (d) => accessed++;
            machine.Process();

            accessed.Should().BeGreaterOrEqualTo(4);
        }

        [Test]
        public void Process__bubbles_processed_dive_before_saving_dive()
        {
            var accessed = 0;

            var parserMock = new Mock<IImageParser>();
            var pixels = new[] { new Pixel(2, 3), new Pixel(3, 4), new Pixel(5, 4) };
            parserMock.Setup(d => d.ReadDocument()).Returns(pixels);

            var dives = new[] { new Dive(15, 50, 23, ""), new Dive(24, 20, 33, "") };
            var dr = new Mock<IDiveRepository>();
            dr.Setup(inp => inp.LoadDives()).Returns(dives);

            var sf = new Mock<ISampleFactory>();
            var processed = new[] { new ProcessedDive(dives[0]), new ProcessedDive(dives[0]) };
            sf.Setup(inp => inp.Create(It.IsAny<IEnumerable<Pixel>>(), dives[0])).Returns(processed[0]);
            sf.Setup(inp => inp.Create(It.IsAny<IEnumerable<Pixel>>(), dives[1])).Returns(processed[1]);

            Func<DlImageParsr.Model.Dive, IImageParser> facFunc = d => { return parserMock.Object; };

            var machine = new Machine(dr.Object, sf.Object, facFunc);
            machine.BeforeSavingDive += (dive) => accessed += dive.DiveLogId;
            machine.Process();

            accessed.Should().BeGreaterOrEqualTo(25);
        }

        [Test]
        public void Process__stores_processed_dives_but_not_nulls()
        {
            var accessed = 0;

            var parserMock = new Mock<IImageParser>();
            var pixels = new[] { new Pixel(2, 3), new Pixel(3, 4), new Pixel(5, 4) };
            parserMock.Setup(d => d.ReadDocument()).Returns(pixels);

            var dives = new[] { new Dive(14, 50, 23, ""), new Dive(14, 50, 23, "") };
            var dr = new Mock<IDiveRepository>();
            dr.Setup(inp => inp.LoadDives()).Returns(dives);

            var sf = new Mock<ISampleFactory>();
            var processed = new[] { new ProcessedDive(dives[0]), null };
            sf.Setup(inp => inp.Create(It.IsAny<IEnumerable<Pixel>>(), dives[0])).Returns(processed[0]);
            sf.Setup(inp => inp.Create(It.IsAny<IEnumerable<Pixel>>(), dives[1])).Returns(processed[1]);

            Func<DlImageParsr.Model.Dive, IImageParser> facFunc = d => { if (dives.Contains(d)) accessed++; return parserMock.Object; };

            var machine = new Machine(dr.Object, sf.Object, facFunc);

            machine.Process();

            dr.Verify(inp => inp.SaveDive(It.IsAny<ProcessedDive>() ), Times.Once());
        }
    }
}
