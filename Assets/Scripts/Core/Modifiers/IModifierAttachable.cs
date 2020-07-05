using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infinity.Core.Modifier
{
    public interface IModifierAttachable
    {
        void AddModifier(BasicModifier modifier);

        void RemoveModifier(BasicModifier modifier);
    }
}
