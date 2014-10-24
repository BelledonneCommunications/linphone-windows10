using System;

namespace ScreenSizeSupport.Misc
{
  public static class MathHelpers
  {
    public const double Epsilon = 0.001;
    public static bool IsCloseEnoughTo(this double d1, double d2)
    {
      return (Math.Abs(d1 - d2) < Epsilon);
    }

    public static bool IsCloseEnoughOrSmallerThan(this double d1, double d2)
    {
      return d1 < (d2 + Epsilon);
    }

    public static double NudgeToClosestPoint(this double currentValue, int nudgeValue)
    {
      var newValue = currentValue * 10 / nudgeValue;
      newValue = Math.Floor(newValue + Epsilon);
      return newValue / 10 * nudgeValue;
    }
  }
}
