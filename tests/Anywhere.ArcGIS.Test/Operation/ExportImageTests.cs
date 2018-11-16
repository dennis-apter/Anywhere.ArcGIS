using System.Collections.Generic;
using System.Threading.Tasks;
using Anywhere.ArcGIS.Operation;
using Anywhere.ArcGIS.Test.AspNetCore;
using Anywhere.ArcGIS.Test.AspNetCore.Extensions;
using Xunit;

namespace Anywhere.ArcGIS.Test.Operation
{
    public class ExportImageTests
    {
        [Fact]
        public void LayerIdsToExport()
        {
            var op = new ExportImage("/");

            Assert.Null(op.LayerIdsMethod);
            Assert.Null(op.LayerIdsToExport);
            Assert.Null(op.LayerIdsToExportValue);

            op.LayerIdsToExportValue = "show:1,2,3";

            Assert.NotNull(op.LayerIdsToExport);
            Assert.Equal(3, op.LayerIdsToExport.Count);
            Assert.Equal(LayerIdsMethod.Show, op.LayerIdsMethod);

            op.LayerIdsToExportValue = "1,3";

            Assert.Equal(2, op.LayerIdsToExport.Count);
            Assert.Null(op.LayerIdsMethod);

            op.LayerIdsToExportValue = "include:2,5,7,9";

            Assert.Equal(4, op.LayerIdsToExport.Count);
            Assert.Equal(LayerIdsMethod.Include, op.LayerIdsMethod);

            op.LayerIdsToExportValue = "hide:";

            Assert.Equal(LayerIdsMethod.Hide, op.LayerIdsMethod);
            Assert.Null(op.LayerIdsToExport);
            Assert.Null(op.LayerIdsToExportValue);

            op.LayerIdsToExportValue = "  ";

            Assert.Null(op.LayerIdsMethod);
            Assert.Null(op.LayerIdsToExport);
            Assert.Null(op.LayerIdsToExportValue);

            op.LayerIdsMethod = LayerIdsMethod.Show;
            Assert.Null(op.LayerIdsToExportValue);

            op.LayerIdsToExport = new List<int> { 11, 22, 33 };
            Assert.Equal("show:11,22,33", op.LayerIdsToExportValue);

            op.LayerIdsToExport.Clear();
            Assert.Null(op.LayerIdsToExportValue);

            op.LayerIdsMethod = null;
            op.LayerIdsToExport = new List<int> { 11, 22, 33 };
            Assert.Equal("11,22,33", op.LayerIdsToExportValue);
        }

        [Fact]
        public void LayerDefinitions()
        {
            var op = new ExportImage("/");

            Assert.Null(op.LayerDefinitions);
            Assert.Null(op.LayerDefinitionsValue);

            // Simple Syntax
            op.LayerDefinitionsValue = "1:def1;2:def2";

            Assert.NotNull(op.LayerDefinitions);
            Assert.Equal(2, op.LayerDefinitions.Count);
            Assert.True(op.LayerDefinitions.ContainsKey(1));
            Assert.True(op.LayerDefinitions.ContainsKey(2));
            Assert.Equal("def1", op.LayerDefinitions[1]);
            Assert.Equal("def2", op.LayerDefinitions[2]);

            // JSON Syntax
            op.LayerDefinitionsValue = "{ \"1\": \"def1\", \"2\": \"def2\" }";

            Assert.NotNull(op.LayerDefinitions);
            Assert.Equal(2, op.LayerDefinitions.Count);
            Assert.True(op.LayerDefinitions.ContainsKey(1));
            Assert.True(op.LayerDefinitions.ContainsKey(2));
            Assert.Equal("def1", op.LayerDefinitions[1]);
            Assert.Equal("def2", op.LayerDefinitions[2]);

            op.LayerDefinitions.Clear();
            Assert.Null(op.LayerDefinitionsValue);

            op.LayerDefinitions[11] = "def11";
            op.LayerDefinitions[22] = "def22";
            Assert.Equal("11:def11;22:def22", op.LayerDefinitionsValue);
        }

        [Fact]
        public async Task ModelBinding()
        {
            // Arrange
            var binder = new ArcGISServerOperationBinder(typeof(ExportImage));
            var context = new ArcGISServerOperationModelBindingContext(
                "?f=json&bbox=1,2,3,4&bboxSr=5&imageSr=6&size=100,200&_=1540963663393");

            // Act
            await binder.BindModelAsync(context);

            // Assert
            var model = context.Result.Model as ExportImage;
            Assert.NotNull(model);

            Assert.Equal("json", model.Format);

            Assert.NotNull(model.BoundingBox);
            Assert.Equal("1,2,3,4", model.BoundingBox.ToDataString());

            Assert.NotNull(model.BoundingBoxSpatialReference);
            Assert.Equal(5, model.BoundingBoxSpatialReference.Wkid);

            Assert.NotNull(model.ImageSpatialReference);
            Assert.Equal(6, model.ImageSpatialReference.Wkid);

            Assert.NotNull(model.Size);
            Assert.Equal(100, model.Size.Width);
            Assert.Equal(200, model.Size.Height);
        }
    }
}
