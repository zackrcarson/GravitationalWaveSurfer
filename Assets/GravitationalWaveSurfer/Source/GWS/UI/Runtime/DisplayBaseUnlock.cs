using GWS.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GWS.UI.Runtime
{
    public abstract class DisplayBaseUnlock : MonoBehaviour
    {
        [SerializeField] 
        protected Image image;

        [SerializeField]
        protected new TextMeshProUGUI name;

        [SerializeField] 
        protected TextMeshProUGUI description;

        [SerializeField] 
        protected TextMeshProUGUI lockSymbol;

        protected abstract void PopulateFields();

        protected abstract bool IsUnlocked();

        private void OnEnable()
        {
            DisplayItem();
            UnlockManager.OnUnlock += DisplayItem;
        }

        private void OnDisable()
        {
            UnlockManager.OnUnlock -= DisplayItem;
        }

        private void DisplayItem()
        {
            if (!IsUnlocked())
            {
                lockSymbol.gameObject.SetActive(true);
                SetElements(false);
                return;
            }
            SetElements(true);
            lockSymbol.gameObject.SetActive(false);
            PopulateFields();
        }

        protected virtual void SetElements(bool state)
        {
            name.enabled = state;
            image.enabled = state;
            description.enabled = state;
        }
    }
}
