using System;

namespace ConsoleApp1
{
    internal class MES_Math_Constants
    {
        static public double[] _2interpolationRanges = { 1, 1 };
        static public double[] _2interpolationPoints = { -1.0 / Math.Sqrt(3.0), 1.0 / Math.Sqrt(3.0) };

        static public double[] _3interpolationRanges = { 5.0 / 9.0, 8.0 / 9.0, 5.0 / 9.0 };
        static public double[] _3interpolationPoints = { -Math.Sqrt(3.0 / 5.0), 0, Math.Sqrt(3.0 / 5.0) };

        static public double[] _4interpolationRanges = { (18.0 - Math.Sqrt(30.0)) / 36.0, (18.0 + Math.Sqrt(30.0)) / 36.0,
                                                       (18.0 + Math.Sqrt(30.0)) / 36.0, (18.0 - Math.Sqrt(30.0)) / 36.0};

        static public double[] _4interpolationPoints = { -Math.Sqrt(3.0/7.0 + 2.0/7.0 * Math.Sqrt(6.0/5.0)), -Math.Sqrt(3.0 / 7.0 - 2.0 / 7.0 * Math.Sqrt(6.0 / 5.0)),
                                                        Math.Sqrt(3.0/7.0 - 2.0/7.0 * Math.Sqrt(6.0/5.0)),  Math.Sqrt(3.0 / 7.0 + 2.0 / 7.0 * Math.Sqrt(6.0 / 5.0))};
    }
}
