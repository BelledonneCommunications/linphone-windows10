using System;
using System.Windows;

namespace ScreenSizeSupport.Misc
{
  public class ScreenInfo
  {
    public static readonly ScreenInfo NullValue = new ScreenInfo(0, 0, 0);

    public Size PhysicalSize { get; private set; }
    public Size PhysicalResolution { get; private set; }
    public double AspectRatio { get; private set; }

    public ScreenInfo(double physicalDiagonal, double physicalResolutionWidth, double aspectRatio)
      : this(SizeHelpers.MakeSizeFromDiagonal(physicalDiagonal, aspectRatio), SizeHelpers.MakeSize(physicalResolutionWidth, aspectRatio))
    {
    }

    public ScreenInfo(Size physicalSize, Size physicalResolution)
    {
      PhysicalSize = physicalSize;
      PhysicalResolution = physicalResolution;
      if (PhysicalResolution.Width == 0)
      {
        AspectRatio = 0;
        return;
      }

      AspectRatio = physicalSize.Height / physicalSize.Width;

      if (!AspectRatio.IsCloseEnoughTo(physicalResolution.Height / physicalResolution.Width))
        throw new ArgumentOutOfRangeException("physicalResolution", "only square pixels supported");
    }
  }
}
