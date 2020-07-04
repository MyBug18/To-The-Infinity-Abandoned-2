using System;
using System.Collections.Generic;

namespace Infinity.Core.Planet
{
    public class Planet
    {
        public readonly bool IsInhabitable;

        public Planet(bool IsInhabitable)
        {
            this.IsInhabitable = IsInhabitable;
        }
    }
}
