using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GWS.UI.Runtime
{
    public abstract class DisplayBaseUnlock : MonoBehaviour
    {
        [SerializeField] 
        protected TextMeshProUGUI lockSymbol;

        public abstract void PopulateFields();

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
            
        }
    }
}
