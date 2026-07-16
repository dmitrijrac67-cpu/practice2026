using System;

namespace task14;

public class OneThreadIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step)
    {
        int parts = (int)Math.Round((b - a) / step);
        if (parts < 1) parts = 1;

        double actualStep = (b - a) / parts;
        double sum = 0.0;

        for (int i = 0; i < parts; i++)
        {
            double x1 = a + i * actualStep;
            double x2 = a + (i + 1) * actualStep;
            sum += (function(x1) + function(x2)) / 2.0 * actualStep;
        }

        return sum;
    }
}