using System;
using Anywhere.ArcGIS.Common;
using System.Runtime.Serialization;

namespace Anywhere.ArcGIS.Operation
{
    /// <summary>
    /// Base class for calls to an ArcGIS Server operation
    /// </summary>
    public class ArcGISServerOperation : CommonParameters, IHttpOperation, ICloneable
    {
        private IEndpoint _endpoint;

        public ArcGISServerOperation(IEndpoint endpoint, Action beforeRequest = null, Action afterRequest = null)
        {
            Endpoint = endpoint;
            BeforeRequest = beforeRequest;
            AfterRequest = afterRequest;
        }

        public ArcGISServerOperation(string endpoint, Action beforeRequest = null, Action afterRequest = null)
            : this(string.IsNullOrEmpty(endpoint) ? NullEndpoint.Instance : new ArcGISServerEndpoint(endpoint), beforeRequest, afterRequest)
        { }        

        [IgnoreDataMember]
        public Action BeforeRequest { get; set; }

        [IgnoreDataMember]
        public Action AfterRequest { get; set; }

        [IgnoreDataMember]
        public IEndpoint Endpoint
        {
            get => _endpoint;
            set => _endpoint = value ?? NullEndpoint.Instance;
        }

        [IgnoreDataMember]
        public string RelativeUrl => Endpoint.RelativeUrl;

        public string BuildAbsoluteUrl(string rootUrl) => Endpoint.BuildAbsoluteUrl(rootUrl);

        protected virtual ArcGISServerOperation CloneImpl()
        {
            var clone = (ArcGISServerOperation)MemberwiseClone();
            clone.BeforeRequest = null;
            clone.AfterRequest = null;
            clone.Endpoint = null;
            return clone;
        }

        object ICloneable.Clone() => CloneImpl();

        private sealed class NullEndpoint : IEndpoint
        {
            public static readonly IEndpoint Instance = new NullEndpoint();

            public string RelativeUrl => string.Empty;

            public string BuildAbsoluteUrl(string rootUrl)
            {
                return rootUrl;
            }
        }
    }   
}
