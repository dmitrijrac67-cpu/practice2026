using System;
using System.Threading;

namespace task14;

public class DefiniteIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        if (threadsNumber <= 0)
        {
            throw new ArgumentException("Количество потоков должно быть больше нуля.", nameof(threadsNumber));
        }

        if (step <= 0)
        {
            throw new ArgumentException("Размер шага должен быть больше нуля.", nameof(step));
        }

        bool swapped = false;
        if (a > b)
        {
            (a, b) = (b, a);
            swapped = true;
        }

        double totalResult = 0.0;
        double fullInterval = b - a;

        if (fullInterval == 0)
        {
            return 0.0;
        }

        using var barrier = new Barrier(threadsNumber + 1);
        double segmentLength = fullInterval / threadsNumber;
        Thread[] workerThreads = new Thread[threadsNumber];

        for (int i = 0; i < threadsNumber; i++)
        {
            int threadIndex = i;
            workerThreads[i] = new Thread(() =>
            {
                double left = a + threadIndex * segmentLength;
                double right = left + segmentLength;

                int segments = (int)Math.Ceiling((right - left) / step);
                double actualStep = (right - left) / segments;

                double sum = 0.5 * (function(left) + function(right));

                for (int k = 1; k < segments; k++)
                {
                    double x = left + k * actualStep;
                    sum += function(x);
                }

                sum *= actualStep;

                double oldValue;
                double newValue;
                do
                {
                    oldValue = totalResult;
                    newValue = oldValue + sum;
                }
                while (Interlocked.CompareExchange(ref totalResult, newValue, oldValue) != oldValue);

                barrier.SignalAndWait();
            });

            workerThreads[i].Start();
        }

        barrier.SignalAndWait();

        return swapped ? -totalResult : totalResult;
    }
}