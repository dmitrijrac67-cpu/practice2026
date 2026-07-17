using System;
using System.Collections.Generic;

namespace task17;

public class RoundRobinScheduler : IScheduler
{
    private readonly Queue<ICommand> _pendingTasks = new();

    public bool HasCommand()
    {
        return _pendingTasks.Count > 0;
    }

    public ICommand Select()
    {
        if (_pendingTasks.Count == 0)
        {
            throw new InvalidOperationException("Нет доступных команд для выполнения.");
        }

        return _pendingTasks.Dequeue();
    }

    public void Add(ICommand cmd)
    {
        if (cmd == null)
        {
            throw new ArgumentNullException(nameof(cmd), "Команда не может быть null.");
        }

        _pendingTasks.Enqueue(cmd);
    }
}