#region Using Statements

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#endregion    

public class ItemSlot : MonoBehaviour,IDropHandler {
    public ItemType SlotType;
    private Item currentItem = null;
    
    //Events
    public Action<Item> OnItemEquipped;
    public Action<Item> OnItemUnequipped;
    
    public void OnDrop(PointerEventData eventData) {
        if (eventData.pointerDrag != null) {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            Item item = eventData.pointerDrag.GetComponent<Item>();
            if (item.ItemType == SlotType) {
                EquipItem(item);
                GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
            else {
                item.GetComponent<RectTransform>().anchoredPosition = item.startingPosition;
            }
            
            
        }
    }

    public void EquipItem(Item inputItem) {
        if (inputItem != null) {
            inputItem.SetItemSlot(this);
            GetComponent<Image>().enabled = false;
            currentItem = inputItem;
            OnItemEquipped?.Invoke(inputItem);
        }else {
            Debug.LogErrorFormat("{0} received a null item to Unequip!", gameObject.name);
        }
    }

    public void UnequipItem(Item inputItem) {
        if (inputItem != null) {
            GetComponent<Image>().enabled = true;
            OnItemUnequipped?.Invoke(inputItem);
        }
        else {
            Debug.LogErrorFormat("{0} received a null item to Unequip!", gameObject.name);
        }
    }
}