using System;
using Anywhere.ArcGIS.Common;
using Anywhere.ArcGIS.GeoJson;
using Xunit;

namespace ArcGIS.Test
{
    public class GeoJsonPointTest
    {
        [Fact]
        public void Parse()
        {
            var p1 = GeoJsonPoint.Parse("{ type: 'Point', coordinates: [2.2, 1.1] }");

            Assert.Equal(2.2, p1.X);
            Assert.Equal(1.1, p1.Y);
            Assert.Equal(GeoJsonPoint.Parse(p1.ToDataString()), p1);

            var p2 = GeoJsonPoint.Parse("{ type: 'Point', coordinates: [3.3, 2.2, 1.1] }");

            Assert.Equal(3.3, p2.X);
            Assert.Equal(2.2, p2.Y);
            Assert.Equal(1.1, p2.Z);
            Assert.Equal(GeoJsonPoint.Parse(p2.ToDataString(), PointStyles.AllowZ), p2);

            var p3 = GeoJsonPoint.Parse("{ type: 'Point', coordinates: [4.4, 3.3, 2.2, 1.1] }");

            Assert.Equal(4.4, p3.X);
            Assert.Equal(3.3, p3.Y);
            Assert.Equal(2.2, p3.Z);
            Assert.Equal(1.1, p3.M);
            Assert.Equal(GeoJsonPoint.Parse(p3.ToDataString(), PointStyles.Any), p3);

            Assert.Throws<FormatException>(() => GeoJsonPoint.Parse("1,1;2,2"));
            Assert.Throws<FormatException>(() => GeoJsonPoint.Parse("1,1,2,2"));
            Assert.Throws<FormatException>(() => GeoJsonPoint.Parse("1.1"));
            Assert.Throws<FormatException>(() => GeoJsonPoint.Parse("1.1,1.1,1.1,1.1,1.1"));
            Assert.Throws<ArgumentException>(() => GeoJsonPoint.Parse(""));
            Assert.Throws<ArgumentException>(() => GeoJsonPoint.Parse(null));
        }
    }

    public class GeoJsonLineStringTest
    {
        [Fact]
        public void Parse()
        {
            var ls = GeoJsonLineString.Parse("{ type: 'LineString', coordinates: [[1.1, 2.2],[3.3, 4.4]] }");

            Assert.Equal(2, ls.Coordinates.Count);
            Assert.Equal(1.1, ls.Coordinates[0][0]);
            Assert.Equal(2.2, ls.Coordinates[0][1]);
            Assert.Equal(3.3, ls.Coordinates[1][0]);
            Assert.Equal(4.4, ls.Coordinates[1][1]);
        }
    }
}
