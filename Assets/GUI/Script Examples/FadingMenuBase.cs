//Base Class for Fade-able Menu

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Calcatz.ArrivalGUI {
    public class FadingMenuBase : MonoBehaviour {

        [SerializeField] protected bool show = true;
        public float fadeDuration = 1.5f;

        protected Dictionary<Graphic, float> initialAlphas;
        private Coroutine showCoroutine;
        private bool isShowing = false;

        //Save alpha values of all children graphics
        protected void InitializeGraphicAlphas(List<Graphic> _graphics) {
            initialAlphas = new Dictionary<Graphic, float>();
            foreach (Graphic graphic in _graphics) {
                initialAlphas.Add(graphic, graphic.color.a);
                Color col = graphic.color;
                col.a = 0;
                graphic.color = col;
            }
            if (show) {
                Show();
            }
        }

        /// <summary>
        /// Called each frame
        /// </summary>
        protected virtual void Update() {
            if (!show && isShowing) {
                Close();
            }
            else if (show && !isShowing) {
                Show();
            }
        }

        /// <summary>
        /// Show the panel, and starts the show animation.
        /// </summary>
        public void Show() {
            if (showCoroutine != null) {
                StopCoroutine(showCoroutine);
            }
            showCoroutine = StartCoroutine(ShowCoroutine());
            isShowing = true;
        }

        /// <summary>
        /// Close the panel, and starts the close animation.
        /// </summary>
        public void Close() {
            if (showCoroutine != null) {
                StopCoroutine(showCoroutine);
            }
            showCoroutine = StartCoroutine(CloseCoroutine());
            isShowing = false;
        }

        /// <summary>
        /// Animates panel fade-in.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShowCoroutine() {
            OnBeforeShow();
            for (float t = 0; t < fadeDuration; t += Time.deltaTime) {
                foreach (KeyValuePair<Graphic, float> pair in initialAlphas) {
                    Color targetColor = pair.Key.color;
                    targetColor.a = pair.Value;
                    pair.Key.color = Color.Lerp(pair.Key.color, targetColor, Mathf.Clamp01(t / fadeDuration));
                }
                yield return null;
            }
        }

        /// <summary>
        /// Animates panel fade-out.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CloseCoroutine() {
            for (float t = 0; t < fadeDuration; t += Time.deltaTime) {
                foreach (KeyValuePair<Graphic, float> pair in initialAlphas) {
                    Color targetColor = pair.Key.color;
                    targetColor.a = 0f;
                    pair.Key.color = Color.Lerp(pair.Key.color, targetColor, Mathf.Clamp01(t / fadeDuration));
                }
                yield return null;
            }
            OnAfterClose();
        }


        /// <summary>
        /// Called right before Show function begins.
        /// </summary>
        protected virtual void OnBeforeShow() {

        }

        /// <summary>
        /// Called right after Close function has been done.
        /// </summary>
        protected virtual void OnAfterClose() {

        }
    }
}
