//This is intended to show an illustration of how ObtainNewLogogram UI behaves
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Calcatz.ArrivalGUI {
    public class ObtainNewLogogram : FadingMenuBase {

        /// <summary>
        /// Called on Awake
        /// </summary>
        private void Awake() {
            //Add all fade-able graphics
            List<Graphic> graphics = new List<Graphic>();
            graphics.AddRange(GetComponentsInChildren<Graphic>());
            InitializeGraphicAlphas(graphics);
        }

    }
}
