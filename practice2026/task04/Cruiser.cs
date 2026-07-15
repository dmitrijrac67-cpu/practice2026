namespace task04
{
    public class Cruiser : ISpaceship
    {
        public int Speed => 50;
        public int FirePower => 100;
        
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