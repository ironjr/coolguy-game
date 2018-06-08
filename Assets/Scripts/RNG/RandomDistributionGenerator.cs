using System;

public static class RandomDistributionGenerator
{
    public static double NextGaussianDouble(this Random r)
    {
        double u;
        double v;
        double sumSquare;
        do
        {
            u = 2.0 * r.NextDouble() - 1.0;
            v = 2.0 * r.NextDouble() - 1.0;
            sumSquare = u * u + v * v;
        }
        while (sumSquare >= 1.0);

        double fac = Math.Sqrt(-2.0 * Math.Log(sumSquare) / sumSquare);
        return u * fac;
    }
}
