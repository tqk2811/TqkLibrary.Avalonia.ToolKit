using Avalonia.Data.Converters;
using System.Collections.ObjectModel;
using System.Globalization;

namespace TqkLibrary.Avalonia.ToolKit.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public class ConverterChain : Collection<IValueConverter>, IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return this
                .Aggregate(value, (current, converter) => converter.Convert(current, targetType, parameter, culture));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return this
                .Reverse()
                .Aggregate(value, (current, converter) => converter.ConvertBack(current, targetType, parameter, culture));
        }
    }
}
