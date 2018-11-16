namespace Anywhere.ArcGIS.Common
{
    using System;

    public interface IHttpOperation
    {
        IEndpoint Endpoint { get; set; }

        Action BeforeRequest { get; set; }

        Action AfterRequest { get; set; }
    }
}
