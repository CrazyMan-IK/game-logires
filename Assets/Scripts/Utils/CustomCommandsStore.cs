using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IngameDebugConsole;
using Tayx.Graphy;

namespace CrazyGames.Logires
{
    public class CustomCommandsStore : MonoBehaviour
    {
        [SerializeField] private GraphyManager _graphyManager = null;

        private void Start()
        {
            DebugLogConsole.AddCommand("d-info-main", "View fps and other debug info", ToggleMainDebugInfo);
            DebugLogConsole.AddCommand("d-info-advanced", "View device hardware info", ToggleAdvancedDebugInfo);
        }

        private void ToggleMainDebugInfo()
        {
            ToggleModule(GraphyManager.ModuleType.FPS);
            ToggleModule(GraphyManager.ModuleType.RAM);
            ToggleModule(GraphyManager.ModuleType.AUDIO);
        }

        private void ToggleAdvancedDebugInfo()
        {
            ToggleModule(GraphyManager.ModuleType.ADVANCED);
        }

        private void ToggleModule(GraphyManager.ModuleType module)
        {
            GraphyManager.ModuleState state = GraphyManager.ModuleState.OFF;

            switch (module)
            {
                case GraphyManager.ModuleType.FPS:
                    state = _graphyManager.FpsModuleState;
                    break;
                case GraphyManager.ModuleType.RAM:
                    state = _graphyManager.RamModuleState;
                    break;
                case GraphyManager.ModuleType.AUDIO:
                    state = _graphyManager.AudioModuleState;
                    break;
                case GraphyManager.ModuleType.ADVANCED:
                    state = _graphyManager.AdvancedModuleState;
                    break;
            }

            _graphyManager.SetModuleMode(module, state == GraphyManager.ModuleState.OFF ? GraphyManager.ModuleState.FULL : GraphyManager.ModuleState.OFF);
        }
    }
}
