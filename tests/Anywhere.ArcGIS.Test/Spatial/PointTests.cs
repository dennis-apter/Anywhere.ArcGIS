using System;
using Anywhere.ArcGIS.Common;
using Xunit;

namespace Anywhere.ArcGIS.Test.Spatial
{
    public class PointTests
    {
        [Theory]
        [InlineData("1.1,2.2")]
        [InlineData("1.1,2.2,3.3")]
        [InlineData("1.1,2.2,3.3,4.4")]
        public void ToDataString(string s)
        {
            Assert.Equal(s, Point.Parse(s, PointStyles.Any).ToDataString());
        }

        [Fact]
        public void Parse()
        {
            var p1 = Point.Parse("1.1,2.2");

            Assert.Equal(1.1, p1.X);
            Assert.Equal(2.2, p1.Y);

            Assert.Throws<FormatException>(() => Point.Parse("3.3,2.2,1.1"));
            var p3 = Point.Parse("3.3,2.2,1.1", PointStyles.AllowZ);

            Assert.Equal(3.3, p3.X);
            Assert.Equal(2.2, p3.Y);
            Assert.Equal(1.1, p3.Z);

            Assert.Throws<FormatException>(() => Point.Parse("4.4,3.3,2.2,1.1"));
            Assert.Throws<FormatException>(() => Point.Parse("4.4,3.3,2.2,1.1", PointStyles.AllowZ));
            var p4 = Point.Parse("4.4,3.3,2.2,1.1", PointStyles.Any);

            Assert.Equal(4.4, p4.X);
            Assert.Equal(3.3, p4.Y);
            Assert.Equal(2.2, p4.Z);
            Assert.Equal(1.1, p4.M);
        }

        [Fact]
        public void Parse_Json()
        {
            var p2 = Point.Parse("{ x:1.1, y:2.2 }");

            Assert.Equal(1.1, p2.X);
            Assert.Equal(2.2, p2.Y);
            Assert.Equal(Point.Parse(p2.ToDataString()), p2);

            var p3 = Point.Parse("{ x:3.3, y:2.2, z:1.1 }");

            Assert.Equal(3.3, p3.X);
            Assert.Equal(2.2, p3.Y);
            Assert.Equal(1.1, p3.Z);
            Assert.Equal(Point.Parse(p3.ToDataString(), PointStyles.AllowZ), p3);

            var p4 = Point.Parse("{ x:4.4, y:3.3, z:2.2, m:1.1 }");

            Assert.Equal(4.4, p4.X);
            Assert.Equal(3.3, p4.Y);
            Assert.Equal(2.2, p4.Z);
            Assert.Equal(1.1, p4.M);
            Assert.Equal(Point.Parse(p4.ToDataString(), PointStyles.Any), p4);
        }

        [Fact]
        public void Parse_Compact()
        {
            var p2 = Point.Parse("[1.1,2.2]");

            Assert.Equal(1.1, p2.X);
            Assert.Equal(2.2, p2.Y);

            var p3 = Point.Parse("[3.3, 2.2, 1.1]");

            Assert.Equal(3.3, p3.X);
            Assert.Equal(2.2, p3.Y);
            Assert.Equal(1.1, p3.Z);

            var p4 = Point.Parse("[4.4, 3.3, 2.2, 1.1]");

            Assert.Equal(4.4, p4.X);
            Assert.Equal(3.3, p4.Y);
            Assert.Equal(2.2, p4.Z);
            Assert.Equal(1.1, p4.M);
        }

        [Fact]
        public void Parse_Invalid()
        {
            Assert.Throws<FormatException>(() => Point.Parse("1,1;2,2"));
            Assert.Throws<FormatException>(() => Point.Parse("1,1,2,2"));
            Assert.Throws<FormatException>(() => Point.Parse("1.1"));
            Assert.Throws<FormatException>(() => Point.Parse("1.1,1.1,1.1,1.1,1.1"));
            Assert.Throws<ArgumentException>(() => Point.Parse(""));
            Assert.Throws<ArgumentException>(() => Point.Parse(null));
        }
    }
}
