using System.Collections;
using System.Collections.Generic;

namespace Infinity.Core.Planet
{
    public class ResourceTank
    {
        public float Energy { get; private set; } = 0;

        public float Mineral { get; private set; } = 0;

        public float Food { get; private set; } = 0;

        public float Alloy { get; private set; } = 0;

        public float Money { get; private set; } = 0;

        public float PhysicsResearch { get; private set; } = 0;

        public float SocietyResearch { get; private set; } = 0;

        public float EngineerResearch { get; private set; } = 0;
    }
}
