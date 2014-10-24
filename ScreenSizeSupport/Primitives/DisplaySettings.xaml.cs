using Microsoft.Phone.Controls;
using ScreenSizeSupport.Misc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ScreenSizeSupport.Primitives
{

  public partial class DisplaySettings : UserControl, INotifyPropertyChanged
  {
    List<StandardScreenSize> StandardScreenSizes;

    public DisplaySettings()
    {
      InitializeComponent();

      StandardScreenSizes = new List<StandardScreenSize>();
      StandardScreenSizes.Add(new StandardScreenSize { Name = "HTC 8S or Lumia 520", DiagonalSize = 4, HorizontalResolution = 480, AspectRatio = DisplayConstants.AspectRatio15To9 });
      StandardScreenSizes.Add(new StandardScreenSize { Name = "Lumia 822", DiagonalSize = 4.2, HorizontalResolution = 480, AspectRatio = DisplayConstants.AspectRatio15To9 });
      StandardScreenSizes.Add(new StandardScreenSize { Name = "HTC 8X", DiagonalSize = 4.3, HorizontalResolution = 720, AspectRatio = DisplayConstants.AspectRatio16To9 });
      StandardScreenSizes.Add(new StandardScreenSize { Name = "Lumia 920", DiagonalSize = 4.5, HorizontalResolution = 768, AspectRatio = DisplayConstants.AspectRatio15To9 });
      StandardScreenSizes.Add(new StandardScreenSize { Name = "Samsung ATIV S", DiagonalSize = 4.8, HorizontalResolution = 720, AspectRatio = DisplayConstants.AspectRatio16To9 });
      StandardScreenSizes.Add(new StandardScreenSize { Name = "Generic 5\" 720p", DiagonalSize = 5, HorizontalResolution = 720, AspectRatio = DisplayConstants.AspectRatio16To9 });
      StandardScreenSizes.Add(new StandardScreenSize { Name = "Generic 5.5\" Full HD", DiagonalSize = 5.5, HorizontalResolution = 1080, AspectRatio = DisplayConstants.AspectRatio16To9 });
      StandardScreenSizes.Add(new StandardScreenSize { Name = "Lumia 1320", DiagonalSize = 6, HorizontalResolution = 720, AspectRatio = DisplayConstants.AspectRatio16To9 });
      StandardScreenSizes.Add(new StandardScreenSize { Name = "Lumia 1520", DiagonalSize = 6, HorizontalResolution = 1080, AspectRatio = DisplayConstants.AspectRatio16To9 });
      StandardScreenSizes.Add(new StandardScreenSize { Name = "Generic 6.5\" Full HD", DiagonalSize = 6.5, HorizontalResolution = 1080, AspectRatio = DisplayConstants.AspectRatio16To9 });

      physicalTemplates.ItemsSource = StandardScreenSizes;
      physicalTemplates.SelectedIndex = -1;
    }

    private void TemplateSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var item = physicalTemplates.SelectedItem as StandardScreenSize;
      if (item == null)
        return;

      NewDiagonal = item.DiagonalSize;
      NewAspectRatio = item.AspectRatio;
      NewWidth = item.HorizontalResolution;
    }

    public DisplayInformationEx DisplayInformationEx { get; set; }

    public string LegacyHostResolutionDescription
    {
      get
      {
        return String.Format("{0:#.} x {1:#.} px", Application.Current.Host.Content.ActualWidth,
                             Application.Current.Host.Content.ActualHeight);
      }
    }

    public string LegacyHostScaleFactorDescription
    {
      get
      {
        return String.Format("{0:#.0}", Application.Current.Host.Content.ScaleFactor / 100d);
      }
    }
    
    public string LegacyHostRawResolutionDescription
    {
      get
      {
        var scaleFactor = Application.Current.Host.Content.ScaleFactor / 100d;
        return String.Format("{0:#.} x {1:#.} px", Application.Current.Host.Content.ActualWidth * scaleFactor,
                             Application.Current.Host.Content.ActualHeight * scaleFactor);
      }
    }

    public string LegacyHostAspectRatioDescription
    {
      get
      {
        var aspectRatio = Application.Current.Host.Content.ActualHeight /
                          (double) Application.Current.Host.Content.ActualWidth;
        return String.Format("{0} ({1:#.##})", SizeHelpers.GetFriendlyAspectRatio(aspectRatio), aspectRatio);
      }
    }

    public string PhysicalDiagonalSizeDescription
    {
      get
      {
        return String.Format("{0:#.0}\"", DisplayInformationEx.PhysicalDiagonal);
      }
    }

    public string PhysicalResolutionDescription
    {
      get
      {
        return String.Format("{0:#.} x {1:#.} px", DisplayInformationEx.PhysicalResolution.Width,
                             DisplayInformationEx.PhysicalResolution.Height);
      }
    }

    public string RawDpiDescription
    {
      get
      {
        return String.Format("{0:#.##}", DisplayInformationEx.RawDpi);
      }
    }

    public string AspectRatioDescription
    {
      get
      {
        return String.Format("{0} ({1:#.##})", SizeHelpers.GetFriendlyAspectRatio(DisplayInformationEx.AspectRatio), DisplayInformationEx.AspectRatio);
      }
    }

    public string PhysicalSizeDescription
    {
      get
      {
        return String.Format("{0:#.0#}\" x {1:#.0#}\"", DisplayInformationEx.PhysicalSize.Width,
                             DisplayInformationEx.PhysicalSize.Height);
      }
    }
    
    public string InformationSourceDescription
    {
      get
      {
        switch (DisplayInformationEx.InformationSource)
        {
          case DisplayInformationSource.Custom:
            return "custom values";

          case DisplayInformationSource.DesignTimeFallback:
            return "designer fallback";

          case DisplayInformationSource.Hardware:
            return "hardware";

          case DisplayInformationSource.LegacyDefault:
            return "legacy default";

          default:
            return "unknown";
        }
      }
    }

    public string ViewResolutionDescription
    {
      get
      {
        return String.Format("{0:#.} x {1:#.} px", DisplayInformationEx.ViewResolution.Width,
                             DisplayInformationEx.ViewResolution.Height);
      }
    }

    public string RuntimeRawResolutionDescription
    {
      get
      {
        return String.Format("{0:#.} x {1:#.} px", DisplayInformationEx.PhysicalResolution.Width,
                             DisplayInformationEx.PhysicalResolution.Height);
      }
    }

    public string RawPixelsPerViewPixelDescription
    {
      get
      {
        return String.Format("{0:#.0#}", DisplayInformationEx.RawPixelsPerViewPixel);
      }
    }

    public string ViewPixelsPerHostPixelDescription
    {
      get
      {
        return String.Format("{0:#.0#}", DisplayInformationEx.ViewPixelsPerHostPixel);
      }
    }

    public string ViewPixelsPerInchDescription
    {
      get
      {
        return String.Format("{0:#.0#}", DisplayInformationEx.ViewPixelsPerInch);
      }
    }

    public string AbsoluteScaleFactorBeforeNormalizingDescription
    {
      get
      {
        return String.Format("{0:0.0#}", DisplayInformationEx.AbsoluteScaleFactorBeforeNormalizing);
      }
    }
    
    public double NewDiagonal
    {
      get { return Math.Round(newDiagonal, 1); }
      set { SetProperty(ref newDiagonal, value); }
    }
    private double newDiagonal;

    public double NewWidth
    {
      get { return newWidth; }
      set { SetProperty(ref newWidth, value); }
    }
    private double newWidth;

    public double NewAspectRatio
    {
      get { return newAspectRatio; }
      set { SetProperty(ref newAspectRatio, value); }
    }
    private double newAspectRatio;

    TaskCompletionSource<bool> showCompletionSource;

    public Task<bool> ShowAsync()
    {
      if (showCompletionSource != null)
        throw new InvalidOperationException("Can't show while already showing");

      DataContext = null;
      DataContext = this;

      showCompletionSource = new TaskCompletionSource<bool>();
      NewAspectRatio = DisplayInformationEx.AspectRatio;
      NewDiagonal = DisplayInformationEx.PhysicalDiagonal;
      NewWidth = DisplayInformationEx.PhysicalResolution.Width;

      rootScroller.ScrollToVerticalOffset(0);
      physicalTemplates.SelectedIndex = -1;
      physicalTemplates.ScrollIntoView(physicalTemplates.Items[0]);

      hostInformationRow.Height = new GridLength(0);
      hardwareDetailsRow.Height = new GridLength(0);
      newDetailsRow.Height = new GridLength(0);
      physicalTemplates.Visibility = Visibility.Collapsed;

      runtimeValuesRow.Height = new GridLength(0, GridUnitType.Auto);

      (Application.Current.RootVisual as PhoneApplicationFrame).BackKeyPress += DisplaySettings_BackKeyPress;

      Visibility = Visibility.Visible;
      return showCompletionSource.Task;
    }

    void DisplaySettings_BackKeyPress(object sender, CancelEventArgs e)
    {
      if (e.Cancel)
        return;

      Hide(false);
      e.Cancel = true;
    }

    public void Hide(bool valuesChanged)
    {
      Visibility = Visibility.Collapsed;
      (Application.Current.RootVisual as PhoneApplicationFrame).BackKeyPress -= DisplaySettings_BackKeyPress;
      showCompletionSource.SetResult(valuesChanged);
      showCompletionSource = null;
    }

    private void OkClick(object sender, RoutedEventArgs e)
    {
      bool valuesChanged = false;
      if (NewAspectRatio != 0)
      {
        DisplayInformationEx = new DisplayInformationEx(SizeHelpers.MakeSizeFromDiagonal(NewDiagonal, NewAspectRatio), SizeHelpers.MakeSize(NewWidth, NewAspectRatio));
        valuesChanged = true;
      }

      Hide(valuesChanged);
    }

    private void CancelClick(object sender, RoutedEventArgs e)
    {
      Hide(false);
    }

    void SetProperty<T>(ref T existing, T value, [CallerMemberName] string name = "")
    {
      if (existing.Equals(value))
        return;

      existing = value;
      RaisePropertyChanged(name);
    }
    public event PropertyChangedEventHandler PropertyChanged;
    protected void RaisePropertyChanged([CallerMemberName] string name = "")
    {
      var handler = PropertyChanged;
      if (handler != null)
        handler(this, new PropertyChangedEventArgs(name));
    }

    void ToggleZeroOrAuto(RowDefinition rowDefinition)
    {
      if (rowDefinition.Height.GridUnitType == GridUnitType.Auto)
        rowDefinition.Height = new GridLength(0);
      else
        rowDefinition.Height = new GridLength(0, GridUnitType.Auto);
    }

    private void CurrentHardwareTap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      ToggleZeroOrAuto(hardwareDetailsRow);
    }

    private void NewHardwareTap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      ToggleZeroOrAuto(newDetailsRow);

      // XAML bug - the Listbox doesn't disappear when you set the GridRow to zero
      if (newDetailsRow.Height.GridUnitType == GridUnitType.Auto)
        physicalTemplates.Visibility = Visibility.Visible;
      else
        physicalTemplates.Visibility = Visibility.Collapsed;
    }

    private void RuntimeValuesTap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      ToggleZeroOrAuto(runtimeValuesRow);
    }

    private void HostInformationTap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      ToggleZeroOrAuto(hostInformationRow);
    }
  }
}
