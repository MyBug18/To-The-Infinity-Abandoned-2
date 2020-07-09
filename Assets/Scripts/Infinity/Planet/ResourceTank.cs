namespace Infinity.Planet
{
    public class ResourceTank
    {
        public float Energy { get; private set; } = 0;

        public float Mineral { get; private set; } = 0;

        public float Food { get; private set; } = 0;

        public float Alloy { get; private set; } = 0;

        public static ResourceTank operator -(ResourceTank a)
        {
            return new ResourceTank
            {
                Energy = -a.Energy,
                Mineral = -a.Mineral,
                Food = -a.Food,
                Alloy = -a.Alloy,
            };
        }

        public static ResourceTank operator +(ResourceTank a, ResourceTank b)
        {
            return new ResourceTank
            {
                Energy = a.Energy + b.Energy,
                Mineral = a.Mineral + b.Mineral,
                Food = a.Food + b.Food,
                Alloy = a.Alloy + b.Alloy,
            };
        }

        public static ResourceTank operator -(ResourceTank a, ResourceTank b)
        {
            return a + -b;
        }
    }
}
