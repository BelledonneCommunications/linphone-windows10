using Microsoft.Phone.Controls;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Linphone.Model
{
    /// <summary>
    /// Returns the Hub tile width corresponding to a tile size.
    /// </summary>
    public class TileSizeToWidthConverterFix : IValueConverter
    {
        /// <summary>
        /// Converts from a tile size to the corresponding width.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double baseWidth = 0;

            switch ((TileSize)value)
            {
                case TileSize.Default:
                    baseWidth = 173;
                    break;

                case TileSize.Small:
                    baseWidth = 99;
                    break;

                case TileSize.Medium:
                    baseWidth = 210;
                    break;

                case TileSize.Large:
                    baseWidth = 432;
                    break;
            }

            double multiplier;

            if (parameter == null || double.TryParse(parameter.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out multiplier) == false)
            {
                multiplier = 1;
            }

            return baseWidth * multiplier;
        }

        /// <summary>
        /// Not used.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Returns the Hub tile height corresponding to a tile size.
    /// </summary>
    public class TileSizeToHeightConverterFix : IValueConverter
    {
        /// <summary>
        /// Converts from a tile size to the corresponding height.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double baseHeight = 0;

            switch ((TileSize)value)
            {
                case TileSize.Default:
                    baseHeight = 173;
                    break;

                case TileSize.Small:
                    baseHeight = 99;
                    break;

                case TileSize.Medium:
                    baseHeight = 210;
                    break;

                case TileSize.Large:
                    baseHeight = 210;
                    break;
            }

            double multiplier;

            if (parameter == null || double.TryParse(parameter.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out multiplier) == false)
            {
                multiplier = 1;
            }

            return baseHeight * multiplier;
        }

        /// <summary>
        /// Not used.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
