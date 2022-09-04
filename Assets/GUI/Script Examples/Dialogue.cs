//This is intended to show an illustration of how Dialogue UI behaves
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Calcatz.ArrivalGUI {
    public class Dialogue : FadingMenuBase {

        public float textFadeDuration = 0.3f;
        public RectTransform dialogueBox;
        public RectTransform choicesRoot;
        public List<RectTransform> choices;

        public Text dialogueNameText;
        public Text dialogueText;

        public Color selectedTextColor;
        public Color unselectedTextColor;

        public KeyCode upKey;
        public KeyCode downKey;

        [SerializeField] private int m_selectedChoice;
        public int selectedChoice {
            get { return m_selectedChoice; }
            set {
                UnhighlightChoice(choices[m_selectedChoice]);
                m_selectedChoice = value;
                HighlightChoice(choices[m_selectedChoice]);
            }
        }

        private Coroutine nameTextCoroutine;
        private Coroutine textCoroutine;

        /// <summary>
        /// Called on Awake
        /// </summary>
        private void Awake() {
            //Add all fade-able graphics
            List<Graphic> graphics = new List<Graphic>();
            graphics.AddRange(GetComponentsInChildren<Graphic>());
            InitializeGraphicAlphas(graphics);
            if (!show) {
                if (dialogueBox != null) dialogueBox.gameObject.SetActive(false);
                if (choicesRoot != null) choicesRoot.gameObject.SetActive(false);
            }
            foreach(RectTransform choice in choices) {
                UnhighlightChoice(choice);
            }
            selectedChoice = m_selectedChoice;
        }

        /// <summary>
        /// Called right before Show function begins.
        /// </summary>
        protected override void OnBeforeShow() {
            if (dialogueBox != null) dialogueBox.gameObject.SetActive(true);
            if (choicesRoot != null) choicesRoot.gameObject.SetActive(true);
            if (choices.Count > 0) {
                for (int i=0; i<choices.Count; i++) {
                    if (choices[i] != null && choices[i].gameObject.activeSelf) {
                        selectedChoice = i;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Called right after Close function has been done.
        /// </summary>
        protected override void OnAfterClose() {
            if (dialogueBox != null) dialogueBox.gameObject.SetActive(false);
            if (choicesRoot != null) choicesRoot.gameObject.SetActive(false);
        }

        /// <summary>
        /// Set choice into unhighlighted state.
        /// </summary>
        /// <param name="_choice"></param>
        private void UnhighlightChoice(RectTransform _choice) {
            _choice.GetChild(0).GetChild(0).gameObject.SetActive(false);
            _choice.GetChild(1).GetChild(0).gameObject.SetActive(false);
            _choice.GetChild(0).GetChild(1).GetComponent<Image>().color = unselectedTextColor;
            _choice.GetChild(1).GetChild(1).GetComponent<Text>().color = unselectedTextColor;
        }

        /// <summary>
        /// Set choice into highlighted state.
        /// </summary>
        /// <param name="_choice"></param>
        private void HighlightChoice(RectTransform _choice) {
            _choice.GetChild(0).GetChild(0).gameObject.SetActive(true);
            _choice.GetChild(1).GetChild(0).gameObject.SetActive(true);
            _choice.GetChild(0).GetChild(1).GetComponent<Image>().color = selectedTextColor;
            _choice.GetChild(1).GetChild(1).GetComponent<Text>().color = selectedTextColor;
        }

        /// <summary>
        /// Change the text on name text section.
        /// </summary>
        /// <param name="_text"></param>
        public void ChangeNameText(string _text) {
            if (nameTextCoroutine != null) {
                StopCoroutine(nameTextCoroutine);
            }
            nameTextCoroutine = StartCoroutine(NameTextCoroutine(_text));
        }

        /// <summary>
        /// Handles the animation of the name text.
        /// </summary>
        /// <param name="_text"></param>
        /// <returns></returns>
        private IEnumerator NameTextCoroutine(string _text) {
            //Fade-in first.
            for (float t = 0; t < textFadeDuration; t += Time.deltaTime) {
                Color targetColor = dialogueNameText.color;
                targetColor.a = 0;
                dialogueNameText.color = Color.Lerp(dialogueNameText.color, targetColor, Mathf.Clamp01(t / textFadeDuration));
                yield return null;
            }
            dialogueNameText.text = _text;
            //Fade-in the text using the new value
            for (float t = 0; t < textFadeDuration; t += Time.deltaTime) {
                Color targetColor = dialogueNameText.color;
                targetColor.a = initialAlphas[dialogueNameText];
                dialogueNameText.color = Color.Lerp(dialogueNameText.color, targetColor, Mathf.Clamp01(t / textFadeDuration));
                yield return null;
            }
        }

        /// <summary>
        /// Change the text of the content section.
        /// </summary>
        /// <param name="_text"></param>
        public void ChangeText(string _text) {
            if (textCoroutine != null) {
                StopCoroutine(textCoroutine);
            }
            textCoroutine = StartCoroutine(TextCoroutine(_text));
        }
        private IEnumerator TextCoroutine(string _text) {
            for (float t = 0; t < textFadeDuration; t += Time.deltaTime) {
                Color targetColor = dialogueText.color;
                targetColor.a = 0;
                dialogueText.color = Color.Lerp(dialogueText.color, targetColor, Mathf.Clamp01(t / textFadeDuration));
                yield return null;
            }
            dialogueText.text = _text;
            for (float t = 0; t < textFadeDuration; t += Time.deltaTime) {
                Color targetColor = dialogueText.color;
                targetColor.a = initialAlphas[dialogueText];
                dialogueText.color = Color.Lerp(dialogueText.color, targetColor, Mathf.Clamp01(t / textFadeDuration));
                yield return null;
            }
        }

        /// <summary>
        /// Called each frame
        /// </summary>
        protected override void Update() {
            base.Update();
            if (Input.GetKeyDown(upKey)) {
                int newIndex = selectedChoice;
                for (int i = 0; i < choices.Count; i++) {
                    newIndex--;
                    if (newIndex < 0) newIndex = choices.Count - 1;
                    if (choices[newIndex] != null && choices[newIndex].gameObject.activeSelf) {
                        selectedChoice = newIndex;
                        break;
                    }
                }
            }
            else if (Input.GetKeyDown(downKey)) {
                int newIndex = selectedChoice;
                for (int i = 0; i < choices.Count; i++) {
                    newIndex++;
                    if (newIndex > choices.Count - 1) newIndex = 0;
                    if (choices[newIndex] != null && choices[newIndex].gameObject.activeSelf) {
                        selectedChoice = newIndex;
                        break;
                    }
                }
            }
        }

#if UNITY_EDITOR
        [SerializeField] private bool showDebugUI = true;
        private void OnGUI() {
            if (showDebugUI) {
                if (GUILayout.Button("Show Name Text 1")) {
                    ChangeNameText("John");
                }
                if (GUILayout.Button("Show Name Text 2")) {
                    ChangeNameText("Alex");
                }
                if (GUILayout.Button("Show Dialog Text 1")) {
                    ChangeText("Text Example 1");
                }
                if (GUILayout.Button("Show Dialog Text 2")) {
                    ChangeText("This is an example of text 2.");
                }
            }
        }
#endif
    }
}
