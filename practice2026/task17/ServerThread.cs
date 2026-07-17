using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task17;

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _taskQueue = new();
    private readonly Thread _worker;
    private readonly IScheduler? _scheduler;
    private Action _currentMode;
    private bool _active = true;

    public ServerThread(IScheduler? scheduler = null)
    {
        _scheduler = scheduler;
        _currentMode = ProcessNormal;
        _worker = new Thread(MainLoop);
    }

    public void Start() => _worker.Start();

    public void EnqueueCommand(ICommand command)
    {
        try
        {
            if (!_taskQueue.IsAddingCompleted)
                _taskQueue.Add(command);
        }
        catch (InvalidOperationException)
        {
        }
    }

    public void WaitUntilFinished(int timeoutMs = Timeout.Infinite)
    {
        _worker.Join(timeoutMs);
    }

    public bool IsAlive => _worker.IsAlive;

    public void ChangeBehavior(Action newMode)
    {
        _currentMode = newMode;
    }

    public void ExecuteHardStop()
    {
        ValidateCallingThread();
        _active = false;
    }

    public void ExecuteSoftStop()
    {
        ValidateCallingThread();
        _taskQueue.CompleteAdding();
        ChangeBehavior(ProcessUntilEmpty);
    }

    private void MainLoop()
    {
        while (_active)
        {
            _currentMode();
        }
    }

    private void ProcessNormal()
    {
        ICommand? command = null;
        try
        {
            if (_scheduler == null)
            {
                command = _taskQueue.Take();
                ExecuteCommand(command);
            }
            else
            {
                int waitTime = _scheduler.HasCommand() ? 0 : Timeout.Infinite;

                if (_taskQueue.TryTake(out var cmd, waitTime))
                {
                    command = cmd;
                    ExecuteCommand(command);
                }
                else if (_scheduler.HasCommand())
                {
                    command = _scheduler.Select();
                    ExecuteCommand(command);
                }
                else if (_taskQueue.IsCompleted)
                {
                    _active = false;
                }
            }
        }
        catch (InvalidOperationException)
        {
            _active = false;
        }
        catch (Exception ex)
        {
            if (command != null)
            {
                ExceptionHandler.OnException?.Invoke(ex, command);
            }
        }
    }

    private void ProcessUntilEmpty()
    {
        ICommand? command = null;
        try
        {
            if (_taskQueue.TryTake(out var cmd))
            {
                command = cmd;
                ExecuteCommand(command);
            }
            else if (_scheduler != null && _scheduler.HasCommand())
            {
                command = _scheduler.Select();
                ExecuteCommand(command);
            }
            else
            {
                _active = false;
            }
        }
        catch (Exception ex)
        {
            if (command != null)
            {
                ExceptionHandler.OnException?.Invoke(ex, command);
            }
        }
    }

    private void ExecuteCommand(ICommand cmd)
    {
        cmd.Execute();

        if (_scheduler != null && cmd is ILongCommand longCmd && !longCmd.IsCompleted)
        {
            _scheduler.Add(cmd);
        }
    }

    private void ValidateCallingThread()
    {
        if (Thread.CurrentThread.ManagedThreadId != _worker.ManagedThreadId)
        {
            throw new InvalidOperationException("Команда остановки должна выполняться внутри ServerThread.");
        }
    }
}