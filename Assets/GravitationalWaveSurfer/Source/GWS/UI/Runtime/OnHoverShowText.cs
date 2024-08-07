using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GWS.UI.Runtime
{
    /// <summary>
    /// Displays some text when the cursor is hovered above this element.
    /// </summary>
    public class OnHoverShowText: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private TextMeshProUGUI text;

        public void OnPointerEnter(PointerEventData eventData)
        {
            text.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            text.gameObject.SetActive(false);
        }

        private void Awake()
        {
            // text.gameObject.SetActive(false);
        }
    }
}
