using System;
using System.Threading;

namespace task14;

public class DefiniteIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        if (threadsNumber <= 0) throw new ArgumentException("Количество потоков должно быть > 0");

        double totalResult = 0.0;
        double totalLength = b - a;
        double segmentSize = totalLength / threadsNumber;

        using (var barrier = new Barrier(threadsNumber + 1))
        {
            Thread[] threadPool = new Thread[threadsNumber];

            for (int i = 0; i < threadsNumber; i++)
            {
                int threadId = i;
                threadPool[i] = new Thread(() =>
                {
                    double leftBound = a + threadId * segmentSize;
                    double rightBound = a + (threadId + 1) * segmentSize;

                    int parts = (int)Math.Round((rightBound - leftBound) / step);
                    if (parts < 1) parts = 1;

                    double actualStep = (rightBound - leftBound) / parts;
                    double localSum = 0.0;

                    for (int j = 0; j < parts; j++)
                    {
                        double x1 = leftBound + j * actualStep;
                        double x2 = leftBound + (j + 1) * actualStep;
                        localSum += (function(x1) + function(x2)) / 2.0 * actualStep;
                    }
                    
                    double oldValue, newValue;
                    do
                    {
                        oldValue = totalResult;
                        newValue = oldValue + localSum;
                    }
                    while (Interlocked.CompareExchange(ref totalResult, newValue, oldValue) != oldValue);

                    barrier.SignalAndWait();
                });
                threadPool[i].Start();
            }
            barrier.SignalAndWait();
        }
        return totalResult;
    }
}