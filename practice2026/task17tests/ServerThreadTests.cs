using System;
using Xunit;
using task17;

namespace task17tests;

public class ServerThreadTests
{
    private const int TimeoutMs = 3000;

    private class TestCommand : ICommand
    {
        public bool Executed { get; private set; }
        private readonly Action? _logic;

        public TestCommand(Action? logic = null)
        {
            _logic = logic;
        }

        public void Execute()
        {
            Executed = true;
            _logic?.Invoke();
        }
    }

    [Fact]
    public void HardStop_TerminatesImmediately_IgnoresRemainingCommands()
    {
        var server = new ServerThread();
        var firstCmd = new TestCommand();
        var stopCmd = new HardStopCommand(server);
        var ignoredCmd = new TestCommand();

        server.EnqueueCommand(firstCmd);
        server.EnqueueCommand(stopCmd);
        server.EnqueueCommand(ignoredCmd);

        server.Start();
        server.WaitUntilFinished(TimeoutMs);

        Assert.True(firstCmd.Executed);
        Assert.False(ignoredCmd.Executed);
        Assert.False(server.IsAlive);
    }

    [Fact]
    public void SoftStop_ProcessesRemainingCommands_BeforeTermination()
    {
        var server = new ServerThread();
        var firstCmd = new TestCommand();
        var stopCmd = new SoftStopCommand(server);
        var lastCmd = new TestCommand();

        server.EnqueueCommand(firstCmd);
        server.EnqueueCommand(stopCmd);
        server.EnqueueCommand(lastCmd);

        server.Start();
        server.WaitUntilFinished(TimeoutMs);

        Assert.True(firstCmd.Executed);
        Assert.True(lastCmd.Executed);
        Assert.False(server.IsAlive);
    }

    [Fact]
    public void StopCommands_ExecutedOnWrongThread_ThrowsException()
    {
        var server = new ServerThread();
        var hardStop = new HardStopCommand(server);
        var softStop = new SoftStopCommand(server);

        var exHard = Assert.Throws<InvalidOperationException>(() => hardStop.Execute());
        Assert.Equal("Команда остановки должна выполняться внутри ServerThread.", exHard.Message);

        var exSoft = Assert.Throws<InvalidOperationException>(() => softStop.Execute());
        Assert.Equal("Команда остановки должна выполняться внутри ServerThread.", exSoft.Message);
    }

    [Fact]
    public void CommandException_IsCaught_AndSentToHandler()
    {
        var server = new ServerThread();
        var expectedEx = new ArgumentException("Test Error");
        var faultyCmd = new TestCommand(() => throw expectedEx);
        var stopCmd = new HardStopCommand(server);

        Exception? capturedEx = null;
        ICommand? capturedCmd = null;

        ExceptionHandler.OnException = (ex, cmd) =>
        {
            capturedEx = ex;
            capturedCmd = cmd;
        };

        server.EnqueueCommand(faultyCmd);
        server.EnqueueCommand(stopCmd);

        server.Start();
        server.WaitUntilFinished(TimeoutMs);

        Assert.Same(expectedEx, capturedEx);
        Assert.Same(faultyCmd, capturedCmd);
    }
}