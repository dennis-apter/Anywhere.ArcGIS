using System;
using Anywhere.ArcGIS.Common;
using Xunit;

namespace Anywhere.ArcGIS.Test.Spatial
{
    public class ExtentTests
    {
        [Theory]
        [InlineData("1.1,2.2,3.3,4.4")]
        [InlineData("1.1,2.2,3.3,4.4,5.5,6.6")]
        [InlineData("1.1,2.2,3.3,4.4,5.5,6.6,7.7,8.8")]
        public void ToDataString(string s)
        {
            Assert.Equal(s, Extent.Parse(s, PointStyles.Any).ToDataString());
        }

        [Fact]
        public void Parse()
        {
            var e2 = Extent.Parse("1.1,2.2,3.3,4.4");

            Assert.False(e2.IsEmpty);
            Assert.Equal(1.1, e2.XMin);
            Assert.Equal(2.2, e2.YMin);
            Assert.Equal(3.3, e2.XMax);
            Assert.Equal(4.4, e2.YMax);

            Assert.Throws<FormatException>(() => Extent.Parse("1.1,2.2,3.3,4.4,5.5,6.6"));
            var e3 = Extent.Parse("1.1,2.2,3.3,4.4,5.5,6.6", PointStyles.AllowZ);

            Assert.False(e3.IsEmpty);
            Assert.Equal(1.1, e3.XMin);
            Assert.Equal(2.2, e3.YMin);
            Assert.Equal(3.3, e3.ZMin);
            Assert.Equal(4.4, e3.XMax);
            Assert.Equal(5.5, e3.YMax);
            Assert.Equal(6.6, e3.ZMax);

            Assert.Throws<FormatException>(() => Extent.Parse("1.1,2.2,3.3,4.4,5.5,6.6,7.7,8.8"));
            Assert.Throws<FormatException>(() => Extent.Parse("1.1,2.2,3.3,4.4,5.5,6.6,7.7,8.8", PointStyles.AllowZ));
            var e4 = Extent.Parse("1.1,2.2,3.3,4.4,5.5,6.6,7.7,8.8", PointStyles.Any);

            Assert.False(e4.IsEmpty);
            Assert.Equal(1.1, e4.XMin);
            Assert.Equal(2.2, e4.YMin);
            Assert.Equal(3.3, e4.ZMin);
            Assert.Equal(4.4, e4.MMin);
            Assert.Equal(5.5, e4.XMax);
            Assert.Equal(6.6, e4.YMax);
            Assert.Equal(7.7, e4.ZMax);
            Assert.Equal(8.8, e4.MMax);
        }

        [Fact]
        public void Parse_Compact()
        {
            var e2 = Extent.Parse("[1.1,2.2,3.3,4.4]");

            Assert.False(e2.IsEmpty);
            Assert.Equal(1.1, e2.XMin);
            Assert.Equal(2.2, e2.YMin);
            Assert.Equal(3.3, e2.XMax);
            Assert.Equal(4.4, e2.YMax);

            var e3 = Extent.Parse("[1.1,2.2,3.3,4.4,5.5,6.6]");

            Assert.False(e3.IsEmpty);
            Assert.Equal(1.1, e3.XMin);
            Assert.Equal(2.2, e3.YMin);
            Assert.Equal(3.3, e3.ZMin);
            Assert.Equal(4.4, e3.XMax);
            Assert.Equal(5.5, e3.YMax);
            Assert.Equal(6.6, e3.ZMax);

            var e4 = Extent.Parse("[1.1,2.2,3.3,4.4,5.5,6.6,7.7,8.8]");

            Assert.False(e4.IsEmpty);
            Assert.Equal(1.1, e4.XMin);
            Assert.Equal(2.2, e4.YMin);
            Assert.Equal(3.3, e4.ZMin);
            Assert.Equal(4.4, e4.MMin);
            Assert.Equal(5.5, e4.XMax);
            Assert.Equal(6.6, e4.YMax);
            Assert.Equal(7.7, e4.ZMax);
            Assert.Equal(8.8, e4.MMax);
        }

        [Fact]
        public void Parse_Json()
        {
            var e2 = Extent.Parse("{ xmin:1.1, ymin:2.2, xmax:3.3, ymax:4.4 }");

            Assert.Equal(1.1, e2.XMin);
            Assert.Equal(2.2, e2.YMin);
            Assert.Equal(3.3, e2.XMax);
            Assert.Equal(4.4, e2.YMax);
            Assert.Equal(Extent.Parse(e2.ToDataString()), e2);

            var e3 = Extent.Parse("{ xmin:1.1, ymin:2.2, zmin: 3.3, xmax:4.4, ymax:5.5, zmax:6.6 }");

            Assert.Equal(1.1, e3.XMin);
            Assert.Equal(2.2, e3.YMin);
            Assert.Equal(3.3, e3.ZMin);
            Assert.Equal(4.4, e3.XMax);
            Assert.Equal(5.5, e3.YMax);
            Assert.Equal(6.6, e3.ZMax);
            Assert.Equal(Extent.Parse(e3.ToDataString(), PointStyles.AllowZ), e3);

            var e4 = Extent.Parse("{ xmin:1.1, ymin:2.2, zmin: 3.3, mmin: 4.4, xmax:5.5, ymax:6.6, zmax:7.7, mmax:8.8 }");

            Assert.Equal(1.1, e4.XMin);
            Assert.Equal(2.2, e4.YMin);
            Assert.Equal(3.3, e4.ZMin);
            Assert.Equal(4.4, e4.MMin);
            Assert.Equal(5.5, e4.XMax);
            Assert.Equal(6.6, e4.YMax);
            Assert.Equal(7.7, e4.ZMax);
            Assert.Equal(8.8, e4.MMax);
            Assert.Equal(Extent.Parse(e4.ToDataString(), PointStyles.Any), e4);
        }

        [Fact]
        public void Parse_Invalid()
        {
            Assert.Throws<FormatException>(() => Extent.Parse("1,1;2,2;3,3;4,4"));
            Assert.Throws<FormatException>(() => Extent.Parse("1,1,2,2,3,3,4,4"));
            Assert.Throws<FormatException>(() => Extent.Parse("1.1"));
            Assert.Throws<ArgumentException>(() => Extent.Parse(""));
            Assert.Throws<ArgumentException>(() => Extent.Parse(null));
        }
    }
}
