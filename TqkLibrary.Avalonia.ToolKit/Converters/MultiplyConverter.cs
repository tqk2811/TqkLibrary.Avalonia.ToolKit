using Avalonia.Data.Converters;
using System.Globalization;

namespace TqkLibrary.Avalonia.ToolKit.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public class MultiplyConverter : IMultiValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public virtual object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            double result = 1.0;
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] is double)
                    result *= (double)values[i]!;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public virtual object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}