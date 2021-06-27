using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MenuPlacement {
    public class PlacementMessage {
        public enum SwitchType {
            CompactToFloating,
            FloatingToCompact,
            NoSwitch
        };

        [System.Flags]
        public enum SolverActivated {
            InBetween = 1<<0,
            Follow = 1<<1,
            SurfaceMagnetism = 1<<2,
            HandConstraint = 1<<3
        };

        public SwitchType switchType;

        public SolverActivated solvers;

        public PlacementMessage() {
            switchType = SwitchType.NoSwitch;
            solvers = 0;
        }
        public PlacementMessage(SwitchType switchType, SolverActivated solvers) {
            this.switchType = switchType;
            this.solvers = solvers;

        }

    }
}

