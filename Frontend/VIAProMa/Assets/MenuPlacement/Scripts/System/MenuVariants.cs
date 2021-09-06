using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MenuPlacement {

    [Serializable]
    public class MenuVariants {

        public GameObject floatingMenu;
        public GameObject compactMenu;

        public override string ToString() {
            return floatingMenu + " and " + compactMenu; 
        }

    }
}