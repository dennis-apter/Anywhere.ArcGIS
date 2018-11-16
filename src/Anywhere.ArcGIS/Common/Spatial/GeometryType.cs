using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Anywhere.ArcGIS.Operation;

namespace Anywhere.ArcGIS.Common
{
    [Serializable]
    public enum GeometryType
    {
        /// <summary>
        /// esriGeometryNull
        /// </summary>
        [EnumMember(Value = GeometryTypes.Null)]
        esriGeometryNull = 0,

        /// <summary>
        /// esriGeometryPoint
        /// </summary>
        [EnumMember(Value = GeometryTypes.Point)]
        Point = 1,

        /// <summary>
        /// esriGeometryMultipoint
        /// </summary>
        [EnumMember(Value = GeometryTypes.Multipoint)]
        Multipoint = 2,

        /// <summary>
        /// esriGeometryPolyline
        /// </summary>
        [EnumMember(Value = GeometryTypes.Polyline)]
        Polyline = 3,

        /// <summary>
        /// esriGeometryPolygon
        /// </summary>
        [EnumMember(Value = GeometryTypes.Polygon)]
        Polygon = 4,

        /// <summary>
        /// esriGeometryEnvelope
        /// </summary>
        [EnumMember(Value = GeometryTypes.Envelope)]
        Envelope = 5,

        /// <summary>
        /// esriGeometryPath
        /// </summary>
        [EnumMember(Value = GeometryTypes.Path)]
        Path = 6,

        /// <summary>
        /// esriGeometryAny
        /// </summary>
        [EnumMember(Value = GeometryTypes.Any)]
        esriGeometryAny = 7,

        /// <summary>
        /// esriGeometryMultiPatch
        /// </summary>
        [EnumMember(Value = GeometryTypes.MultiPatch)]
        esriGeometryMultiPatch = 9,

        /// <summary>
        /// esriGeometryRing
        /// </summary>
        [EnumMember(Value = GeometryTypes.Ring)]
        Ring = 11,

        /// <summary>
        /// esriGeometryLine
        /// </summary>
        [EnumMember(Value = GeometryTypes.Line)]
        esriGeometryLine = 13,

        /// <summary>
        /// esriGeometryCircularArc
        /// </summary>
        [EnumMember(Value = GeometryTypes.CircularArc)]
        esriGeometryCircularArc = 14,

        /// <summary>
        /// esriGeometryBezier3Curve
        /// </summary>
        [EnumMember(Value = GeometryTypes.Bezier3Curve)]
        esriGeometryBezier3Curve = 15,

        /// <summary>
        /// esriGeometryEllipticArc
        /// </summary>
        [EnumMember(Value = GeometryTypes.EllipticArc)]
        esriGeometryEllipticArc = 16,

        /// <summary>
        /// esriGeometryBag
        /// </summary>
        [EnumMember(Value = GeometryTypes.Bag)]
        esriGeometryBag = 17,

        /// <summary>
        /// esriGeometryTriangleStrip
        /// </summary>
        [EnumMember(Value = GeometryTypes.TriangleStrip)]
        esriGeometryTriangleStrip = 18,

        /// <summary>
        /// esriGeometryTriangleFan
        /// </summary>
        [EnumMember(Value = GeometryTypes.TriangleFan)]
        esriGeometryTriangleFan = 19,

        /// <summary>
        /// esriGeometryRay
        /// </summary>
        [EnumMember(Value = GeometryTypes.Ray)]
        esriGeometryRay = 20,

        /// <summary>
        /// esriGeometrySphere
        /// </summary>
        [EnumMember(Value = GeometryTypes.Sphere)]
        esriGeometrySphere = 21,

        /// <summary>
        /// esriGeometryTriangles
        /// </summary>
        [EnumMember(Value = GeometryTypes.Triangles)]
        esriGeometryTriangles = 22,
    }

    public static class GeometryTypes
    {
        public readonly static Dictionary<Type, Func<string>> TypeMap = new Dictionary<Type, Func<string>>
        {
            { typeof(Point), () => Point },
            { typeof(MultiPoint), () => Multipoint },
            { typeof(Extent), () => Envelope },
            { typeof(Polygon), () => Polygon },
            { typeof(Polyline), () => Polyline }
        };

        public readonly static Dictionary<string, Func<Type>> ToTypeMap = new Dictionary<string, Func<Type>>
        {
            { Point, () => typeof(Point) },
            { Multipoint, () => typeof(MultiPoint) },
            { Envelope, () => typeof(Extent) },
            { Polygon, () => typeof(Polygon) },
            { Polyline, () => typeof(Polyline) }
        };

        public const string Null = "esriGeometryNull";
        public const string Point = "esriGeometryPoint";
        public const string Multipoint = "esriGeometryMultipoint";
        public const string Polyline = "esriGeometryPolyline";
        public const string Polygon = "esriGeometryPolygon";
        public const string Envelope = "esriGeometryEnvelope";
        public const string Path = "esriGeometryPath";
        public const string Any = "esriGeometryAny";
        public const string MultiPatch = "esriGeometryMultiPatch";
        public const string Ring = "esriGeometryRing";
        public const string Line = "esriGeometryLine";
        public const string CircularArc = "esriGeometryCircularArc";
        public const string Bezier3Curve = "esriGeometryBezier3Curve";
        public const string EllipticArc = "esriGeometryEllipticArc";
        public const string Bag = "esriGeometryBag";
        public const string TriangleStrip = "esriGeometryTriangleStrip";
        public const string TriangleFan = "esriGeometryTriangleFan";
        public const string Ray = "esriGeometryRay";
        public const string Sphere = "esriGeometrySphere";
        public const string Triangles = "esriGeometryTriangles";
        public const string Prefix = "esriGeometry";

        public static string ToTypeString(this GeometryType value)
        {
            return Prefix + value;
        }

        public static GeometryType FromTypeString(string value)
        {
            if (value.StartsWith(Prefix))
            {
                value = value.Substring(Prefix.Length);
            }

            return (GeometryType)Enum.Parse(typeof(GeometryType), value, true);
        }
    }
}
