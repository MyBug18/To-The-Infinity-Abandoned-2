using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infinity.Core.Planet
{
    public class Planet : MonoBehaviour
    {
        public readonly bool IsInhabitable;

        public Planet(bool IsInhabitable)
        {
            this.IsInhabitable = IsInhabitable;
        }
    }
}
