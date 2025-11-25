using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Svg.Skia;

namespace TqkLibrary.Avalonia.Utils.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ImageSourceAttribute : Attribute
    {
        readonly string _path;
        /// <summary>
        /// null is auto detech extension
        /// </summary>
        public bool? IsSVG { get; set; } = null;

        /// <summary>
        /// Example: avares://MyApp/Assets/new.png or /Assets/file.svg (load from WorkingDir)
        /// </summary>
        /// <param name="path">Example: avares://MyApp/Assets/new.png</param>
        public ImageSourceAttribute(string path)
        {
            this._path = path;
        }

        public IImage? GetImage()
        {
            Stream stream = _path.StartsWith("avares://", StringComparison.OrdinalIgnoreCase)
                ? AssetLoader.Open(new Uri(_path))
                : File.OpenRead(_path);
            if ((IsSVG is null && _path.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)) || IsSVG == true)
            {
                return new SvgImage
                {
                    Source = SvgSource.LoadFromStream(stream),
                };
            }
            else
            {
                try
                {
                    return new Bitmap(stream);
                }
                finally
                {
                    stream.Dispose();
                }
            }
        }
    }
}
