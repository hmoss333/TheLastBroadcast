//This is intended to show an illustration of how AlertBox UI behaves
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Calcatz.ArrivalGUI {
    public class AlertBox : FadingMenuBase {

        public Image dimBackground;
        public RectTransform alertPanel;
        public RectTransform navigationButtons;
        public List<UnityEngine.UI.Button> buttons;

        /// <summary>
        /// Called on Awake
        /// </summary>
        private void Awake() {
            //Add all fade-able graphics
            List<Graphic> graphics = new List<Graphic>();
            graphics.AddRange(GetComponentsInChildren<Graphic>());
            InitializeGraphicAlphas(graphics);
            if (!show) {
                if (dimBackground != null) dimBackground.gameObject.SetActive(false);
                if (alertPanel != null) alertPanel.gameObject.SetActive(false);
                if (navigationButtons != null) navigationButtons.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Called right before Show function begins.
        /// </summary>
        protected override void OnBeforeShow() {
            if (dimBackground != null) dimBackground.gameObject.SetActive(true);
            if (alertPanel != null) alertPanel.gameObject.SetActive(true);
            if (navigationButtons != null) navigationButtons.gameObject.SetActive(true);
            if (buttons.Count > 0) {
                buttons[0].Select();
            }
        }

        /// <summary>
        /// Called right after Close function has been done.
        /// </summary>
        protected override void OnAfterClose() {
            if (dimBackground != null) dimBackground.gameObject.SetActive(false);
            if (alertPanel != null) alertPanel.gameObject.SetActive(false);
            if (navigationButtons != null) navigationButtons.gameObject.SetActive(false);
        }

        /// <summary>
        /// Called each frame
        /// </summary>
        protected override void Update() {
            base.Update();
        }
    }
}
