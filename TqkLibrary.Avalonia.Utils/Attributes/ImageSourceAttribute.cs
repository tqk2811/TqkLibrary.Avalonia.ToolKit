using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Svg.Skia;

namespace TqkLibrary.Avalonia.Utils.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ImageSourceAttribute : Attribute
    {
        readonly string _path_or_key;
        /// <summary>
        /// null is auto detech extension
        /// </summary>
        public bool? IsSVG { get; set; } = null;

        /// <summary>
        /// Key (priority): search in <see cref="ResourceDictionary"/> <br/>
        /// Path: Example 'avares://MyApp/Assets/file.png' or '/Assets/file.svg' (load from WorkingDir)
        /// </summary>
        public ImageSourceAttribute(string key_or_path)
        {
            this._path_or_key = key_or_path;
        }

        public IImage? GetImage()
        {
            if (Application.Current?.TryGetResource(_path_or_key, null, out var res) == true)
            {
                if (res is IImage img)
                    return img;

                if (res is string s)
                {
                    return LoadFromPath(s, IsSVG);
                }
            }
            return LoadFromPath(_path_or_key, IsSVG);
        }

        static IImage LoadFromPath(string path, bool? isSvg = null)
        {
            Stream stream = path.StartsWith("avares://", StringComparison.OrdinalIgnoreCase)
            ? AssetLoader.Open(new Uri(path))
            : File.OpenRead(path);
            if ((isSvg is null && path.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)) || isSvg == true)
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
