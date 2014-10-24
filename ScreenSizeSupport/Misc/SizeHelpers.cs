using System;
using System.Windows;

namespace ScreenSizeSupport.Misc
{
  public static class SizeHelpers
  {
    public static double GetHypotenuse(this Size rect)
    {
      return Math.Sqrt(Math.Pow(rect.Width, 2) + Math.Pow(rect.Height, 2));
    }

    public static readonly Size WvgaPhysicalResolution = new Size(480, 800);
    public static readonly Size Hd720PhysicalResolution = new Size(720, 1280);
    public static readonly Size WxgaPhysicalResolution = new Size(768, 1280);
    public static readonly Size FullHd1080PhysicalResolution = new Size(1080, 1920);

    public static Size Scale(this Size size, double scaleFactor)
    {
      Size scaledSize = new Size();
      var h = size.Height;
      scaledSize.Height = Double.IsInfinity(h) ? h : h * scaleFactor;
      var w = size.Width;
      scaledSize.Width = Double.IsInfinity(w) ? w : w * scaleFactor;
      return scaledSize;
    }

    public static double GetWidthInInchesFromDiagonal(double diagonal, double aspectRatio)
    {
      if (aspectRatio.IsCloseEnoughTo(DisplayConstants.AspectRatio16To9))
        return diagonal * DisplayConstants.DiagonalToWidthRatio16To9;
      if (aspectRatio.IsCloseEnoughTo(DisplayConstants.AspectRatio15To9))
        return diagonal * DisplayConstants.DiagonalToWidthRatio15To9;
      if (aspectRatio.IsCloseEnoughTo(0))
        return 0;

      throw new ArgumentOutOfRangeException("aspectRatio");
    }

    public static string GetFriendlyAspectRatio(double aspectRatio)
    {
      if (aspectRatio.IsCloseEnoughTo(DisplayConstants.AspectRatio16To9))
        return "16:9";
      if (aspectRatio.IsCloseEnoughTo(DisplayConstants.AspectRatio15To9))
        return "15:9";
      if (aspectRatio.IsCloseEnoughTo(0))
        return "N/A";

      throw new ArgumentOutOfRangeException("aspectRatio");
    }

    public static double GetAspectRatioFromFriendlyName(string aspectRatio)
    {
      if (aspectRatio.Trim() == "16:9")
        return DisplayConstants.AspectRatio16To9;
      if (aspectRatio.Trim() == "15:9")
        return DisplayConstants.AspectRatio15To9;

      return 0;
    }

    public static double GetDiagonalFromWidth(double width, double aspectRatio)
    {
      return Math.Sqrt(Math.Pow(width, 2) + Math.Pow(width * aspectRatio, 2));
    }

    public static Size MakeSize(double width, double aspectRatio)
    {
      return new Size(width, width * aspectRatio);
    }

    public static Size MakeSizeFromDiagonal(double diagonal, double aspectRatio)
    {
      var width = GetWidthInInchesFromDiagonal(diagonal, aspectRatio);
      return MakeSize(width, aspectRatio);
    }

    public static Size Round(this Size normalSize)
    {
      return new Size((int)normalSize.Width, (int)normalSize.Height);
    }
  }
}
