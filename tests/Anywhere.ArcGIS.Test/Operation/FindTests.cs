using System.Threading.Tasks;
using Anywhere.ArcGIS.Operation;
using Anywhere.ArcGIS.Test.AspNetCore;
using Anywhere.ArcGIS.Test.AspNetCore.Extensions;
using Xunit;

namespace Anywhere.ArcGIS.Test.Operation
{
    public class FindTests
    {
        [Fact]
        public async Task ModelBinding()
        {
            // Arrange
            var binder = new ArcGISServerOperationBinder(typeof(Find));
            var context = new ArcGISServerOperationModelBindingContext(
                "?searchText=%20Search%20Text%20&contains=false&layers=&searchFields=*&f=json&_=1540963663393");

            // Act
            await binder.BindModelAsync(context);

            // Assert
            var model = context.Result.Model as Find;
            Assert.NotNull(model);

            Assert.Equal("json", model.Format);
            Assert.Equal(" Search Text ", model.SearchText);
            Assert.False(model.Contains);
            Assert.Equal(string.Empty, model.LayerIdsToSearchValue);
            Assert.Equal("*", model.SearchFieldsValue);
        }
    }
}
