using ScreenSizeSupport.Misc;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ScreenSizeSupport
{
  [TemplatePart(Name = ZoomBox.ContentHolderPartName, Type = typeof(UIElement))]
  public class ZoomBox : ContentControl
  {
    public const string ContentHolderPartName = "contentHolder";

    readonly ScaleTransform transform = new ScaleTransform();
    UIElement contentHolder;

    public static readonly DependencyProperty ZoomFactorProperty = DependencyProperty.Register("ZoomFactor", typeof(double), typeof(ZoomBox), new PropertyMetadata(1.0, OnZoomFactorPropertyChanged));
    public double ZoomFactor
    {
      get { return (double)GetValue(ZoomFactorProperty); }
      set { SetValue(ZoomFactorProperty, value); }
    }

    static void OnZoomFactorPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
    {
      var control = (ZoomBox)source;
      if (double.IsNaN((double)e.NewValue) || (double)e.NewValue <= 0)
      {
        control.ZoomFactor = (double)e.OldValue;
        throw new ArgumentOutOfRangeException("ZoomFactor", "must be a positive number");
      }
      control.InvalidateMeasure();
    }
    
    public DisplayInformationEx DisplayInformationEx { get; private set; }

    public static ZoomBox GetForElement(UIElement element)
    {
      var currentElement = element;
      while (currentElement != null)
      {
        if (currentElement is ZoomBox)
          return currentElement as ZoomBox;

        currentElement = VisualTreeHelper.GetParent(currentElement) as UIElement;
      }

      return null;
    }

    public ZoomBox()
    {
      transform = new ScaleTransform { ScaleX = 1, ScaleY = 1 };
      DisplayInformationEx = DisplayInformationEx.Default;
      DefaultStyleKey = typeof(ZoomBox);
    }

    public override void OnApplyTemplate()
    {
      if (contentHolder != null)
        contentHolder.RenderTransform = null;
      contentHolder = null;

      base.OnApplyTemplate();

      var temp = GetTemplateChild(ContentHolderPartName) as UIElement;
      if (temp == null)
        return;

      contentHolder = temp;
      if (ZoomFactor != 1)
        contentHolder.RenderTransform = transform;
    }

    protected override Size ArrangeOverride(Size finalSizeInHostCoordinates)
    {
      var effectiveZoomFactor = ZoomFactor;
      var finalSizeInViewCoordinates = finalSizeInHostCoordinates.Scale(effectiveZoomFactor);
      var requiredSizeInViewCoordinates = base.ArrangeOverride(finalSizeInViewCoordinates);
      var requiredSizeInHostCoordinates = requiredSizeInViewCoordinates.Scale(1 / effectiveZoomFactor);

      if (effectiveZoomFactor != 1)
      {
        transform.ScaleX = transform.ScaleY = 1 / effectiveZoomFactor;
        contentHolder.RenderTransform = transform;
      }
      else
        contentHolder.RenderTransform = null;

#if LOG_MEASURE_ARRANGE_OVERRIDE
      Debug.WriteLine("*** ArrangeOverride ***");
      Debug.WriteLine("  input size (host coordinates):    {0:#.} x {1:#.}", finalSizeInHostCoordinates.Width, finalSizeInHostCoordinates.Height);
      Debug.WriteLine("  converted (view coordinates):     {0:#.} x {1:#.}", finalSizeInViewCoordinates.Width, finalSizeInViewCoordinates.Height);
      Debug.WriteLine("  required size (view coordinates): {0:#} x {1:#.}", requiredSizeInViewCoordinates.Width, requiredSizeInViewCoordinates.Height);
      Debug.WriteLine("  scaling factor:                   {0:#.##}", ZoomFactor);
      Debug.WriteLine("  returning (host coordinates):     {0:#.} x {1:#.}", requiredSizeInHostCoordinates.Width, requiredSizeInHostCoordinates.Height);
#endif

      return requiredSizeInHostCoordinates;
    }

    protected override Size MeasureOverride(Size availableSizeInHostCoordinates)
    {
      var effectiveZoomFactor = ZoomFactor;
      var availableSizeInViewCoordinates = availableSizeInHostCoordinates.Scale(effectiveZoomFactor);
      var desiredSizeInViewCoordinates = base.MeasureOverride(availableSizeInViewCoordinates);
      var desiredSizeInHostCoordinates = desiredSizeInViewCoordinates.Scale(1 / effectiveZoomFactor);

#if LOG_MEASURE_ARRANGE_OVERRIDE
      Debug.WriteLine("*** MeasureOverride ***");
      Debug.WriteLine("  input size (host coordinates):    {0:#.} x {1:#.}", availableSizeInHostCoordinates.Width, availableSizeInHostCoordinates.Height);
      Debug.WriteLine("  converted (view coordinates):     {0:#.} x {1:#.}", availableSizeInViewCoordinates.Width, availableSizeInViewCoordinates.Height);
      Debug.WriteLine("  required size (view coordinates): {0:#} x {1:#.}", desiredSizeInViewCoordinates.Width, desiredSizeInViewCoordinates.Height);
      Debug.WriteLine("  scaling factor:                   {0:#.##}", ZoomFactor);
      Debug.WriteLine("  returning (host coordinates):     {0:#.} x {1:#.}", desiredSizeInHostCoordinates.Width, desiredSizeInHostCoordinates.Height);
#endif

      return desiredSizeInHostCoordinates;
    }
  }
}
