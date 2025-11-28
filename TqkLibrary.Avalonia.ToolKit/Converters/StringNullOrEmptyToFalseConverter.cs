using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace TqkLibrary.Avalonia.ToolKit.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public class StringNullOrEmptyToFalseConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Result { get; set; } = false;
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
            if (string.IsNullOrWhiteSpace(value as string))
                return Result;
            return !Result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
