using Anywhere.ArcGIS.Common;
using Xunit;

namespace Anywhere.ArcGIS.Test.Spatial
{
    public class SpatialReferenceTests
    {
        [Fact]
        public void Parse()
        {
            var r1 = SpatialReference.Parse("{ wkid: 1, latestWkid: 2, vcsWkid: 3, latestVcsWkid: 4 }");

            Assert.NotNull(r1);
            Assert.Null(r1.Wkt);
            Assert.Equal(1, r1.Wkid);
            Assert.Equal(2, r1.LatestWkid);
            Assert.Equal(3, r1.VcsWkid);
            Assert.Equal(4, r1.LatestVcsWkid);

            var r2 = SpatialReference.Parse("WKT");

            Assert.Equal("WKT", r2.Wkt);
            Assert.Null(r2.Wkid);
            Assert.Null(r2.LatestWkid);
            Assert.Null(r2.VcsWkid);
            Assert.Null(r2.LatestVcsWkid);

            var r3 = SpatialReference.Parse("1");

            Assert.Null(r3.Wkt);
            Assert.Equal(1, r3.Wkid);
            Assert.Null(r3.LatestWkid);
            Assert.Null(r3.VcsWkid);
            Assert.Null(r3.LatestVcsWkid);
        }

        [Fact]
        public void Parse_Embedded()
        {
            var p1 = Point.Parse("{ x:1.1, y:2.2, spatialReference: { wkid: 1, latestWkid: 2, vcsWkid: 3, latestVcsWkid: 4 } }");

            Assert.NotNull(p1.SpatialReference);
            Assert.Null(p1.SpatialReference.Wkt);
            Assert.Equal(1, p1.SpatialReference.Wkid);
            Assert.Equal(2, p1.SpatialReference.LatestWkid);
            Assert.Equal(3, p1.SpatialReference.VcsWkid);
            Assert.Equal(4, p1.SpatialReference.LatestVcsWkid);

            var p2 = Point.Parse("{ x:1.1, y:2.2, spatialReference: { wkt: 'WKT', wkid: 1, latestWkid: 2, vcsWkid: 3, latestVcsWkid: 4 } }");

            Assert.Equal("WKT", p2.SpatialReference.Wkt);
            Assert.Null(p2.SpatialReference.Wkid);
            Assert.Null(p2.SpatialReference.LatestWkid);
            Assert.Null(p2.SpatialReference.VcsWkid);
            Assert.Null(p2.SpatialReference.LatestVcsWkid);

            var p3 = Point.Parse("{ x:1.1, y:2.2, spatialReference: 1 }");

            Assert.Null(p3.SpatialReference.Wkt);
            Assert.Equal(1, p3.SpatialReference.Wkid);
            Assert.Null(p3.SpatialReference.LatestWkid);
            Assert.Null(p3.SpatialReference.VcsWkid);
            Assert.Null(p3.SpatialReference.LatestVcsWkid);

            var p4 = Point.Parse("{ x:1.1, y:2.2, spatialReference: 'WKT' }");

            Assert.Equal("WKT", p4.SpatialReference.Wkt);
            Assert.Null(p4.SpatialReference.Wkid);
            Assert.Null(p4.SpatialReference.LatestWkid);
            Assert.Null(p4.SpatialReference.VcsWkid);
            Assert.Null(p4.SpatialReference.LatestVcsWkid);
        }
    }
}
