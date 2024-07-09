using GWS.Gameplay;
using GWS.UI.Runtime;
using PlasticPipe.PlasticProtocol.Messages;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class DisplayBaseUnlock : MonoBehaviour
{
    [SerializeField]
    protected IUnlock unlock;

    [SerializeField] 
    protected Image image;

    [SerializeField] 
    protected TextMeshProUGUI description;

    [SerializeField] 
    protected TextMeshProUGUI lockSymbol;

    protected abstract void PopulateFields();

    protected abstract bool IsUnlocked();

    private void OnEnable()
    {
        DisplayItem();
        UnlockManager.OnUnlock += OnUnlockOccured;
    }

    private void OnDisable()
    {
        UnlockManager.OnUnlock -= OnUnlockOccured;
    }

    private void OnUnlockOccured(string unlockedItemName)
    {
        DisplayItem();
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
        image.enabled = state;
        description.enabled = state;
    }
}
