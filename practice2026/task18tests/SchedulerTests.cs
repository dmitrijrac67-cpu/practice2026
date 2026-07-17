using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using ScottPlot;
using Xunit;
using task17;

namespace task18tests;

public class SchedulerTests
{
    private const int TimeoutMs = 3000;
    private const int ChartWidth = 800;
    private const int ChartHeight = 600;

    private class ChunkedTask : ILongCommand
    {
        private readonly string _name;
        private readonly int _totalChunks;
        private readonly List<string> _executionLog;
        private int _chunksDone = 0;

        public ChunkedTask(string name, int totalChunks, List<string> executionLog)
        {
            _name = name;
            _totalChunks = totalChunks;
            _executionLog = executionLog;
        }

        public bool IsCompleted => _chunksDone >= _totalChunks;

        public void Execute()
        {
            if (IsCompleted) return;

            _chunksDone++;
            lock (_executionLog)
            {
                _executionLog.Add($"{_name}[{_chunksDone}]");
            }
        }
    }

    private class TrackedTask : ILongCommand
    {
        private readonly int _iterations;
        private readonly int _delay;
        private readonly Stopwatch _stopwatch;
        private int _currentIter = 0;

        public List<double> Timestamps { get; } = new();
        public List<double> Progress { get; } = new();

        public TrackedTask(int iterations, int delay, Stopwatch stopwatch)
        {
            _iterations = iterations;
            _delay = delay;
            _stopwatch = stopwatch;

            Timestamps.Add(0);
            Progress.Add(0);
        }

        public bool IsCompleted => _currentIter >= _iterations;

        public void Execute()
        {
            if (IsCompleted) return;

            _currentIter++;
            Thread.Sleep(_delay);

            Timestamps.Add(_stopwatch.Elapsed.TotalMilliseconds);
            Progress.Add((double)_currentIter / _iterations * 100.0);
        }
    }

    [Fact]
    public void Scheduler_DistributesExecution_Equally()
    {
        var log = new List<string>();
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);

        server.EnqueueCommand(new ChunkedTask("TaskA", 3, log));
        server.EnqueueCommand(new ChunkedTask("TaskB", 2, log));
        server.EnqueueCommand(new SoftStopCommand(server));

        server.Start();
        server.WaitUntilFinished(TimeoutMs);

        var expected = new[] { "TaskA[1]", "TaskB[1]", "TaskA[2]", "TaskB[2]", "TaskA[3]" };
        Assert.Equal(expected, log);
    }

    [Fact]
    public void HardStop_Prevents_ScheduledTasksExecution()
    {
        var log = new List<string>();
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);

        server.EnqueueCommand(new ChunkedTask("TaskA", 8, log));
        server.EnqueueCommand(new ChunkedTask("TaskB", 8, log));
        server.EnqueueCommand(new HardStopCommand(server));

        server.Start();
        server.WaitUntilFinished(TimeoutMs);

        Assert.Contains("TaskA[1]", log);
        Assert.Contains("TaskB[1]", log);
        Assert.DoesNotContain("TaskA[8]", log);
    }

    [Fact]
    public void BuildPerformanceReport_AndChart()
    {
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);
        var timer = Stopwatch.StartNew();

        var job1 = new TrackedTask(3, 10, timer);
        var job2 = new TrackedTask(4, 15, timer);
        var job3 = new TrackedTask(2, 20, timer);

        server.EnqueueCommand(job1);
        server.EnqueueCommand(job2);
        server.EnqueueCommand(job3);
        server.EnqueueCommand(new SoftStopCommand(server));

        server.Start();
        server.WaitUntilFinished(TimeoutMs);

        var plot = new Plot();
        plot.Title("Динамика выполнения потоковых задач (Round Robin)");
        plot.XLabel("Затраченное время (мс)");
        plot.YLabel("Общий прогресс (%)");

        var line1 = plot.Add.Scatter(job1.Timestamps.ToArray(), job1.Progress.ToArray());
        line1.LegendText = "Задача 1";
        line1.LineWidth = 2.5f;
        line1.MarkerSize = 6;

        var line2 = plot.Add.Scatter(job2.Timestamps.ToArray(), job2.Progress.ToArray());
        line2.LegendText = "Задача 2";
        line2.LineWidth = 2.5f;
        line2.MarkerSize = 6;

        var line3 = plot.Add.Scatter(job3.Timestamps.ToArray(), job3.Progress.ToArray());
        line3.LegendText = "Задача 3";
        line3.LineWidth = 2.5f;
        line3.MarkerSize = 6;

        plot.ShowLegend();

        string rootDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        plot.SavePng(Path.Combine(rootDir, "progress_chart.png"), ChartWidth, ChartHeight);

        using var writer = new StreamWriter(Path.Combine(rootDir, "report.txt"));
        writer.WriteLine($"Время завершения Задачи 1: {job1.Timestamps.Last():F4} мс");
        writer.WriteLine($"Время завершения Задачи 2: {job2.Timestamps.Last():F4} мс");
        writer.WriteLine($"Время завершения Задачи 3: {job3.Timestamps.Last():F4} мс");
    }
}