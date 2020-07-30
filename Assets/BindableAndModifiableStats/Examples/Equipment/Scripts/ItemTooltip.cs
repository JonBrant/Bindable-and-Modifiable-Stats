#region Using Statements

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
#endregion    

public class ItemTooltip : MonoBehaviour {
    public TextMeshProUGUI ItemNameText;
    public Transform StatsContainer;
    public TextMeshProUGUI StatPrefab;
    public Vector3 Offset;
    List<Item> items = new List<Item>();
    private CanvasGroup canvasGroup;
    private float defaultAlpha = 0.6f;
    private bool hovering = false;
    private void Start() {
        canvasGroup = GetComponent<CanvasGroup>();
        items = GameObject.FindObjectsOfType<Item>().ToList();
        for (int i = 0; i < items.Count; i++) {
            items[i].OnItemHoverIn += OnItemHoverInHandler;
            items[i].OnItemHoverOut += OnItemHoverOutHandler;
        }
    }

    //Always make sure to unsubscribe actions/events
    private void OnDestroy() {
        for (int i = 0; i < items.Count; i++) {
            items[i].OnItemHoverIn -= OnItemHoverInHandler;
            items[i].OnItemHoverOut -= OnItemHoverOutHandler;
        }
    }

    private void Update() {
        if (hovering) {
            transform.position = Input.mousePosition+Offset;
        }
    }

    void OnItemHoverInHandler(Item hoveredItem) {
        hovering = true;
        //In case there is a FadeOut coroutine running.
        StopAllCoroutines();
        //Fade in
        StartCoroutine(FadeTo(defaultAlpha, 0.25f));
        ItemNameText.text = hoveredItem.ItemName;
        for (int i = 0; i < hoveredItem.stats.Stats.Count; i++) {
            TextMeshProUGUI currentStat = Instantiate(StatPrefab, StatsContainer);
            currentStat.gameObject.SetActive(true);
            currentStat.text = string.Format("{0} - {1}",hoveredItem.stats.Stats[i].name, hoveredItem.stats.Stats[i].Value);
        }
    }
    
    void OnItemHoverOutHandler(Item hoveredItem) {
        hovering = false;
        //In case there is a FadeIn coroutine running.
        StopAllCoroutines();
        //Fade out
        StartCoroutine(FadeTo(0, 0.25f));
        for (int i = StatsContainer.childCount - 1; i >= 0; i--) {
            Destroy(StatsContainer.GetChild(i).gameObject);
        }
    }
    
    IEnumerator FadeTo(float aValue, float aTime)
    {
        float alpha = canvasGroup.alpha;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime) {
            canvasGroup.alpha = Mathf.Lerp(alpha, aValue, t);
            yield return null;
        }
        canvasGroup.alpha = aValue;
    }
}