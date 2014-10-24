
using System;
namespace ScreenSizeSupport.Misc
{
  public static class DisplayConstants
  {
    public const double AspectRatio16To9 = 16.0 / 9.0;
    public const double AspectRatio15To9 = 15.0 / 9.0;

    public static readonly double DiagonalToWidthRatio16To9 = 9.0 / Math.Sqrt(Math.Pow(16, 2) + Math.Pow(9, 2));
    public static readonly double DiagonalToWidthRatio15To9 = 9.0 / Math.Sqrt(Math.Pow(15, 2) + Math.Pow(9, 2));

    public const double BaselineDiagonalInInches15To9HighRes = 4.5; // Lumia 920
    public const double BaselineDiagonalInInches15To9LoRes = 4.0; // Lumia 520
    public const double BaselineDiagonalInInches16To9 = 4.3; // HTC 8X

    // We use 15:9 aspect ratio, 4.5" diagonal with 480px view resolution as the baseline for scaling / relayout
    //  * Any size less than 4.5" will get scaled down
    //  * Any size greater than 4.5" will get more layout space
    // Note that 16:9 displays are skinnier than 15:9, so the cutover isn't exactly 4.5" for them
    internal static readonly double BaselineWidthInInches = BaselineDiagonalInInches15To9HighRes * DiagonalToWidthRatio15To9;
    internal const int BaselineWidthInViewPixels = 480;
  }
}
