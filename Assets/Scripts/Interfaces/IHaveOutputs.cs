using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrazyGames.Logires.Interfaces
{
    public interface IHaveOutputs
    {
        IReadOnlyList<Pin> Outputs { get; }
    }
}