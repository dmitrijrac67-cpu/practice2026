namespace task04
{
    public interface ISpaceship
    {
        void MoveForward();
        void Rotate(int degrees);
        void Fire();
        int Speed { get; }
        int FirePower { get; }
    }
}