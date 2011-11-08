using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using FluentAssertions;

namespace DlImageParsrTests
{
    [TestFixture]
    public class ImageTravellerTest
    {
        [Test]
        public void Test() {
            "a".Should().Be("a");
        }

        [Test]
        public void Test1()
        {
            "b".Should().Be("b");
        }
    }
}
