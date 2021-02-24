using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrazyGames.Logires.Utils
{
    public class PhysicsDisabler : MonoBehaviour
    {
        private void Start()
        {
            Physics.autoSimulation = false;
            //Physics2D.simulationMode = SimulationMode2D.Script;
        }
    }
}
