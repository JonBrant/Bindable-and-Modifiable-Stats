#region Using Statements

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

#endregion

public class Character : MonoBehaviour {
    List<ItemSlot> itemSlots;
    public Transform StatsContainer;
    public TextMeshProUGUI StatPrefab;
    private StatList statList;
    private void Start() {
        statList = GetComponent<StatList>();
        itemSlots = GameObject.FindObjectsOfType<ItemSlot>().ToList();
        for (int i = 0; i < itemSlots.Count; i++) {
            itemSlots[i].OnItemEquipped += OnItemEquippedHandler;
            itemSlots[i].OnItemUnequipped += OnItemUnequippedHandler;
        }
    }

    //Always make sure to unsubscribe actions/events
    private void OnDestroy() {
        for (int i = 0; i < itemSlots.Count; i++) {
            itemSlots[i].OnItemEquipped -= OnItemEquippedHandler;
            itemSlots[i].OnItemUnequipped -= OnItemUnequippedHandler;
        }
    }

    void OnItemEquippedHandler(Item inputItem) {
        Debug.LogFormat("Character Equipped {0}", inputItem.ItemName);
        StatList itemStats = inputItem.GetComponent<StatList>();
        if (itemStats != null) {
            for (int i = 0; i < itemStats.Stats.Count; i++) {
                statList.AddStat(itemStats.Stats[i], inputItem);
            }
        }
        UpdateStatDisplay();
    }
    void OnItemUnequippedHandler(Item inputItem) {
        Debug.LogFormat("Character Unequipped {0}", inputItem.ItemName);
        statList.RemoveStatsFromSource(inputItem);
        UpdateStatDisplay();
    }

    private void UpdateStatDisplay() {
        for (int i = StatsContainer.childCount - 1; i >= 0; i--) {
            Destroy(StatsContainer.GetChild(i).gameObject);
        }

        for (int i = 0; i < statList.Stats.Count; i++) {
            TextMeshProUGUI currentStat = Instantiate(StatPrefab, StatsContainer);
            currentStat.gameObject.SetActive(true);
            currentStat.text = string.Format("{0} - {1}",statList.Stats[i].name, statList.Stats[i].Value);
        }
    }
}