namespace task17;

public class HardStopCommand : ICommand
{
    private readonly ServerThread _target;

    public HardStopCommand(ServerThread targetThread)
    {
        _target = targetThread;
    }

    public void Execute()
    {
        _target.ExecuteHardStop();
    }
}

public class SoftStopCommand : ICommand
{
    private readonly ServerThread _target;

    public SoftStopCommand(ServerThread targetThread)
    {
        _target = targetThread;
    }

    public void Execute()
    {
        _target.ExecuteSoftStop();
    }
}