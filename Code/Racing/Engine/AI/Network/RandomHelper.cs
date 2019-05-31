using System;
using System.Collections.Generic;
using System.Linq;
using Racing.Engine.Level;

namespace Racing.Engine.AI.Network
{
    static class RandomHelper
    {
        public static double Range(double lowerBound, double upperBound)
        {
            return lowerBound + Playfield.RNG.NextDouble() * (upperBound - lowerBound);
        }

        public static double Gaussian(double mean, double standardDeviation)
        {
            double u1 = 1.0 - Playfield.RNG.NextDouble();
            double u2 = 1.0 - Playfield.RNG.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + standardDeviation * randStdNormal;
        }
    }
}
