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

        public SwitchType switchType;

        public PlacementMessage() {
            switchType = SwitchType.NoSwitch;
        }

        public PlacementMessage(SwitchType type) {
            switchType = type;
        }

    }
}

