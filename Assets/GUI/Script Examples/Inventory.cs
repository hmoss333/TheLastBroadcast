//This is intended to show an illustration of how Inventory UI behaves
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Calcatz.ArrivalGUI {
    public class Inventory : FadingMenuBase {

        public bool matchBackgroundImagesWithItem = true;
        public List<RectTransform> items;
        public List<Image> backgroundElements;
        public Image currentItemImage;

        /// <summary>
        /// Called on Awake
        /// </summary>
        private void Awake() {
            //Add all fade-able graphics
            List<Graphic> graphics = new List<Graphic>();
            graphics.AddRange(GetComponentsInChildren<Graphic>());
            InitializeGraphicAlphas(graphics);
            if (items.Count > 0) {
                UnityEngine.UI.Button itemButton = items[0].GetComponentInChildren<UnityEngine.UI.Button>();
                if (itemButton != null) {
                    itemButton.Select();
                }
            }

            //Add select event, to change sprites using the selected item.
            foreach(RectTransform item in items) {
                Button itemButton = item.GetComponentInChildren<Button>();
                if (itemButton != null) {
                    Sprite itemSprite = item.GetChild(1).GetComponent<Image>().sprite;
                    EventTrigger trigger = itemButton.gameObject.AddComponent<EventTrigger>();
                    EventTrigger.Entry entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.Select;
                    entry.callback.AddListener(delegate {
                        currentItemImage.sprite = itemSprite;
                        if (matchBackgroundImagesWithItem) {
                            foreach (Image background in backgroundElements) {
                                background.sprite = itemSprite;
                            }
                        }
                    });
                    trigger.triggers.Add(entry);
                }
            }
        }

    }
}
