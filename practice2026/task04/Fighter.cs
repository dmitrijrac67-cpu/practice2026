namespace task04
{
    public class Fighter : ISpaceship
    {
        public int Speed => 100;
        public int FirePower => 50;
        
        public int Position { get; set; } = 0;
        public int Rotation { get; set; } = 0;
        public int ShotsFired { get; set; } = 0;
        
        public void MoveForward()
        {
            Position += Speed;
        }
        
        public void Rotate(int degrees)
        {
            Rotation = (Rotation + degrees) % 360;
        }
        
        public void Fire()
        {
            ShotsFired++;
        }
    }
}