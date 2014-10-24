using Microsoft.Phone.Info;
using ScreenSizeSupport.Misc;
using System;
using System.ComponentModel;
using System.Windows;

namespace ScreenSizeSupport
{

  public enum DisplayInformationSource
  {
    Hardware,
    LegacyDefault,
    DesignTimeFallback,
    Custom,
  }

  public class DisplayInformationEx
  {
    private static readonly string RawDpiValueName = "RawDpiX";
    private static readonly string PhysicalScreenResolutionName = "PhysicalScreenResolution";

    public double PhysicalDiagonal { get; private set; }
    public Size PhysicalSize { get; private set; }
    public Size PhysicalResolution { get; private set; }

    public Size ViewResolution { get; private set; }
    public double ViewPixelsPerInch { get; private set; }
    public double RawDpi { get; private set; }

    public double AspectRatio { get; private set; }
    public double RawPixelsPerViewPixel { get; private set; }

    // Un-normalized value; avoid in most cases
    public double AbsoluteScaleFactorBeforeNormalizing { get; private set; }

    public double ViewPixelsPerHostPixel { get; private set; }
    public double HostPixelsPerViewPixel { get; private set; }

    public DisplayInformationSource InformationSource { get; private set; }

    public double GetViewPixelsForPhysicalSize(double inches)
    {
      return inches * ViewPixelsPerInch;
    }

    public static DisplayInformationEx Default { get; private set; }

    public DisplayInformationEx()
    {
      PhysicalDiagonal = Default.PhysicalDiagonal;
      PhysicalSize = Default.PhysicalSize;
      PhysicalResolution = Default.PhysicalResolution;
      ViewResolution = Default.ViewResolution;
      ViewPixelsPerInch = Default.ViewPixelsPerInch;
      RawDpi = Default.RawDpi;

      AspectRatio = Default.AspectRatio;
      RawPixelsPerViewPixel = Default.RawPixelsPerViewPixel;

      AbsoluteScaleFactorBeforeNormalizing = Default.AbsoluteScaleFactorBeforeNormalizing;

      ViewPixelsPerHostPixel = Default.ViewPixelsPerHostPixel;
      HostPixelsPerViewPixel = Default.HostPixelsPerViewPixel;

      InformationSource = Default.InformationSource;
    }

    public DisplayInformationEx(Size physicalSize, Size physicalResolution) : 
      this(physicalSize, physicalResolution, DisplayInformationSource.Custom)
    {
    }

    private DisplayInformationEx(Size physicalSize, Size physicalResolution, DisplayInformationSource informationSource)
    {
      PhysicalSize = physicalSize;
      PhysicalDiagonal = PhysicalSize.GetHypotenuse();
      PhysicalResolution = physicalResolution;

      AspectRatio = physicalSize.Height / physicalSize.Width;

      if (!(AspectRatio.IsCloseEnoughTo(physicalResolution.Height / physicalResolution.Width)))
        throw new ArgumentOutOfRangeException("physicalResolution", "only square pixels supported");

      RawDpi = physicalResolution.Width / physicalSize.Width;

      AbsoluteScaleFactorBeforeNormalizing = PhysicalSize.Width / DisplayConstants.BaselineWidthInInches;

      RawPixelsPerViewPixel = GenerateRawPixelsPerViewPixel();

      ViewResolution = new Size(PhysicalResolution.Width / RawPixelsPerViewPixel, PhysicalResolution.Height / RawPixelsPerViewPixel);

      ViewPixelsPerInch = RawDpi / RawPixelsPerViewPixel;

      // Adjust for different aspect ratio, if necessary
      ViewPixelsPerHostPixel = Math.Min(ViewResolution.Width / Application.Current.Host.Content.ActualWidth,
        ViewResolution.Height / Application.Current.Host.Content.ActualHeight);

      HostPixelsPerViewPixel = 1 / ViewPixelsPerHostPixel;

      InformationSource = informationSource;
    }

    static DisplayInformationEx()
    {
      if (!DesignerProperties.IsInDesignTool)
      {
        Default = CreateForHardwareOrLegacyFallback();
        return;
      }

      var size = SizeHelpers.MakeSizeFromDiagonal(DisplayConstants.BaselineDiagonalInInches15To9HighRes, DisplayConstants.AspectRatio15To9);
      var resolution = SizeHelpers.MakeSize(SizeHelpers.WxgaPhysicalResolution.Width, DisplayConstants.AspectRatio15To9);
      Default = new DisplayInformationEx(size, resolution, DisplayInformationSource.DesignTimeFallback);
    }

    static DisplayInformationEx CreateForHardwareOrLegacyFallback()
    {
      Size screenResolution;
      double rawDpi;
      object temp;
      if (!DeviceExtendedProperties.TryGetValue(PhysicalScreenResolutionName, out temp))
        return CreateForLegacyHardware();

      screenResolution = (Size)temp;

      if (!DeviceExtendedProperties.TryGetValue(RawDpiValueName, out temp) || (double)temp == 0d)
        return CreateForLegacyHardware();

      rawDpi = (double)temp;

      var physicalSize = new Size(screenResolution.Width / rawDpi, screenResolution.Height / rawDpi);

      return new DisplayInformationEx(physicalSize, screenResolution, DisplayInformationSource.Hardware);
    }

    static DisplayInformationEx CreateForLegacyHardware()
    {
      var scaleFactor = Application.Current.Host.Content.ScaleFactor / 100d;
      var resolutionWidth = Application.Current.Host.Content.ActualWidth * scaleFactor;
      var resolutionHeight = Application.Current.Host.Content.ActualHeight * scaleFactor;
      var aspectRatio = resolutionHeight / resolutionWidth;

      var screenResolultion = new Size(resolutionWidth, resolutionHeight);

      double widthInInches;
      if (aspectRatio.IsCloseEnoughTo(DisplayConstants.AspectRatio15To9))
        if (scaleFactor > 1)
          widthInInches = DisplayConstants.BaselineDiagonalInInches15To9HighRes * DisplayConstants.DiagonalToWidthRatio15To9;
        else
          widthInInches = DisplayConstants.BaselineDiagonalInInches15To9LoRes * DisplayConstants.DiagonalToWidthRatio15To9;
      else
        widthInInches = DisplayConstants.BaselineDiagonalInInches16To9 * DisplayConstants.DiagonalToWidthRatio16To9;

      var physicalSize = new Size(widthInInches, widthInInches * aspectRatio);

      return new DisplayInformationEx(physicalSize, screenResolultion, DisplayInformationSource.LegacyDefault);
    }

    double GenerateRawPixelsPerViewPixel()
    {
      // Never return less than 1
      var scale = Math.Max(1, PhysicalSize.Width / DisplayConstants.BaselineWidthInInches);

      // Never return more view pixels than physical pixels
      var idealViewWidth = Math.Min(DisplayConstants.BaselineWidthInViewPixels * scale, PhysicalResolution.Width);
      var idealScale = PhysicalResolution.Width / idealViewWidth;

      var bucketizedScale = idealScale.NudgeToClosestPoint(1);
      return bucketizedScale;
    }
  }
}
