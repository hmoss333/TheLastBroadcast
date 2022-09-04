//This is intended to show an illustration of how Settings UI behaves
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Calcatz.ArrivalGUI {
    public class Settings : FadingMenuBase {

        public RectTransform menuHighlighter;
        public float highlighterMoveDuration = 0.3f;
        public List<RectTransform> menu;

        private Coroutine highliterMoveCoroutine;

        [SerializeField] private int m_selectedChoice;
        public int selectedChoice {
            get { return m_selectedChoice; }
            set {
                if (highliterMoveCoroutine != null) {
                    StopCoroutine(highliterMoveCoroutine);
                }
                highliterMoveCoroutine = StartCoroutine(HighlightMenu(value));
            }
        }

        public Color selectedTextColor;
        public Color unselectedTextColor;

        public KeyCode upKey;
        public KeyCode downKey;

        /// <summary>
        /// Called on Awake
        /// </summary>
        private void Awake() {
            //Add all fade-able graphics
            List<Graphic> graphics = new List<Graphic>();
            graphics.AddRange(GetComponentsInChildren<Graphic>());
            InitializeGraphicAlphas(graphics);
        }

        private void Start() {
            if (menu.Count > 0) {
                selectedChoice = 0;
            }
        }

        private IEnumerator HighlightMenu(int _index) {
            //Restore the previous menu's text color
            menu[m_selectedChoice].GetComponent<Text>().color = unselectedTextColor;
            m_selectedChoice = _index;
            //Set the current highlighted menu's text color
            menu[m_selectedChoice].GetComponent<Text>().color = selectedTextColor;

            for (float t = 0; t < highlighterMoveDuration; t += Time.deltaTime) {   //Animate menu highlighter's movement
                Vector2 targetPos = menuHighlighter.anchoredPosition;
                targetPos.y = menu[m_selectedChoice].anchoredPosition.y;
                menuHighlighter.anchoredPosition = Vector2.Lerp(menuHighlighter.anchoredPosition, targetPos, Mathf.Clamp01(t / highlighterMoveDuration));
                yield return null;
            }
        }

        /// <summary>
        /// Called each frame
        /// </summary>
        protected override void Update() {
            base.Update();

            //Handles the choice selection input using up and down key.
            if (Input.GetKeyDown(upKey)) {
                int newIndex = selectedChoice;
                for (int i = 0; i < menu.Count; i++) {
                    newIndex--;
                    //Prevent index from reaching negative values.
                    if (newIndex < 0) newIndex = menu.Count - 1;
                    if (menu[newIndex] != null && menu[newIndex].gameObject.activeSelf) {
                        selectedChoice = newIndex;
                        break;
                    }
                }
            }
            else if (Input.GetKeyDown(downKey)) {
                int newIndex = selectedChoice;
                for (int i = 0; i < menu.Count; i++) {
                    newIndex++;
                    //Prevent index from reaching maximum value.
                    if (newIndex > menu.Count - 1) newIndex = 0;
                    if (menu[newIndex] != null && menu[newIndex].gameObject.activeSelf) {
                        selectedChoice = newIndex;
                        break;
                    }
                }
            }
        }
    }
}
