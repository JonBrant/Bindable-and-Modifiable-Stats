#region Using Statements

using System;
using System.Collections;
using System.Collections.Generic;
using Com.BAMS.TheNegativeOne;
using UnityEngine;

#endregion

public class StatPanelOne : MonoBehaviour {
    public GameObject TextObject;
    public Transform TextHolder;
    public List<GameObject> TextObjects = new List<GameObject>();
    private StatList currentList;
    private CanvasGroup canvasGroup;
    private void Start() {
        canvasGroup = GetComponent<CanvasGroup>();
        ObjectClick.ObjectClicked += ObjectClickHandler;
    }

    private void OnDestroy() {
        Debug.LogFormat("StatPanelOne.OnDestroy()");
        ObjectClick.ObjectClicked -= ObjectClickHandler;
        RemoveStatCallbacks();
    }

    void RemoveStatCallbacks() {
        if (currentList != null) {
            for (int i = 0; i < currentList.Stats.Count; i++) {
                currentList.Stats[i].OnValueChanged -= OnValueChangedHandler;
            }
        }
    }

    private void ObjectClickHandler(GameObject inputGameObject) {
        for (int i = 0; i < TextObjects.Count; i++) {
            TextObjects[i].SetActive(false);
        }

        RemoveStatCallbacks(); //Make sure you remove the old callbacks before switching!
        currentList = inputGameObject.GetComponent<StatList>();
        Debug.LogFormat("{0} Clicked!", currentList.gameObject.name);
        for (int i = 0; i < currentList.Stats.Count; i++) {
            if (i >= TextObjects.Count) {
                GameObject temp = Instantiate(TextObject, TextHolder);
                temp.SetActive(true);
                TextObjects.Add(temp);
            }


            GameObject currentText = TextObjects[i];
            currentText.SetActive(true);
            currentList.Stats[i].OnValueChanged += OnValueChangedHandler;
            currentList.SetTextObject(currentList.Stats[i], currentText);
        }
        canvasGroup.alpha = 1;
    }

    void OnValueChangedHandler(CharacterStat inputStat) {
        if (currentList == null) {
            Debug.LogErrorFormat("{0}'s currentList was null!", gameObject.name);
            return;
        }

        currentList.SetText(inputStat, string.Format("{0} - {1}", inputStat.name, inputStat.Value));
    }
}