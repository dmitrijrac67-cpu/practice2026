using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task17;

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _taskQueue = new();
    private readonly Thread _worker;
    private Action _currentMode;
    private bool _active = true;

    public ServerThread()
    {
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
            command = _taskQueue.Take();
            command.Execute();
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
            if (_taskQueue.TryTake(out command))
            {
                command.Execute();
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

    private void ValidateCallingThread()
    {
        if (Thread.CurrentThread.ManagedThreadId != _worker.ManagedThreadId)
        {
            throw new InvalidOperationException("Команда остановки должна выполняться внутри ServerThread.");
        }
    }
}