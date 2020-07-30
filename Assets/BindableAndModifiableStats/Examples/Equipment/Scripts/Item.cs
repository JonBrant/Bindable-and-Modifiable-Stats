#region Using Statements

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

#endregion    

public class Item : MonoBehaviour,IPointerDownHandler,IBeginDragHandler,IEndDragHandler,IDragHandler,IPointerEnterHandler,IPointerExitHandler {
    public string ItemName;
    public ItemType ItemType;
    public Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private ItemSlot currentSlot = null;
    [HideInInspector]public Vector2 startingPosition;
    public StatList stats;
    
    //Events
    public Action<Item> OnItemHoverIn;
    public Action<Item> OnItemHoverOut;
    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        startingPosition = rectTransform.anchoredPosition;
        stats = GetComponent<StatList>();
        for (int i = 0; i < stats.Stats.Count; i++) {
            stats.Stats[i].Source = gameObject;
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        //Debug.LogFormat("OnPointerDown");
    }

    public void OnBeginDrag(PointerEventData eventData) {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
        if (currentSlot!=null) {
            currentSlot.GetComponent<CanvasGroup>().blocksRaycasts = true;
            currentSlot.UnequipItem(this);
        }

        currentSlot = null;
    }

    public void OnEndDrag(PointerEventData eventData) {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
    }

    public void OnDrag(PointerEventData eventData) {
        rectTransform.anchoredPosition += eventData.delta/canvas.scaleFactor;
    }

    public void SetItemSlot(ItemSlot inputSlot) {
        currentSlot = inputSlot;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        OnItemHoverIn?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData) {
        OnItemHoverOut?.Invoke(this);
    }
}