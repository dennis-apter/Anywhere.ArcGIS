using System;
using System.Globalization;
using Anywhere.ArcGIS.Serialization;
using Newtonsoft.Json;

namespace Anywhere.ArcGIS.Common
{
    [JsonConverter(typeof(SizeConverter))]
    public class Size
    {
        public Size() { }

        public Size(int widthAndHeight)
        {
            Width = widthAndHeight;
            Height = widthAndHeight;
        }

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public Size(Point point)
        {
            Width = (int)point.X;
            Height = (int)point.Y;
        }

        public Size(int[] dimensions)
        {
            if (dimensions == null)
            {
                throw new ArgumentNullException();
            }

            if (dimensions.Length == 1)
            {
                Height = Width = dimensions[0];
            }
            else if (dimensions.Length == 2)
            {
                Width = dimensions[0];
                Height = dimensions[1];
            }
            else
            {
                throw new FormatException();
            }
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public bool IsEmpty => Width == 0 || Height == 0;

        public static Size Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException();
            }

            value = value.Trim();
            if (value.StartsWith("{") || value.StartsWith("["))
            {
                return JsonConvert.DeserializeObject<Size>(value);
            }

            var parts = value.Split(',');
            if (parts.Length < 1 || parts.Length > 2)
            {
                throw new FormatException();
            }

            var dimensions = new int[parts.Length];
            for (var i = 0; i < parts.Length; i++)
            {
                if (int.TryParse(
                    parts[i],
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out var v))
                    dimensions[i] = v;
                else
                    throw new FormatException();
            }

            return new Size(dimensions);
        }
    }
}
