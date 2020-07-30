#region Using Statements
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.BAMS.TheNegativeOne;
using TMPro;
using UnityEditor;
using UnityEngine.UI;
#endregion

public class StatList : MonoBehaviour {
    public List<CharacterStat> Stats = new List<CharacterStat>();
    public List<GameObject> TextObjects = new List<GameObject>();
    [Tooltip("Instantiate a copy of each Stat on start, allowing you to modify stats without affecting other StatLists that reference the same Stat ScriptableObject")]
    public bool InstantiateStatCopies = true;

    private void Awake() {
        /*
        for (int i = 0; i < Stats.Count; i++) {
            if (InstantiateStatCopies) {
                CharacterStat currentStat = Stats[i];
                Stats[i] = Instantiate(currentStat);
                Stats[i].name = currentStat.name;
            }
        }
        */
    }

    private void Start() {
        for (int i = 0; i < Stats.Count; i++) {
            CharacterStat currentStat = Stats[i];

            if (currentStat.ResetOnStart) {
                currentStat.BaseValue = currentStat.DefaultValue;
                currentStat.CalculateFinalValue();
            }
        }
    }

    public GameObject GetTextGameObject(CharacterStat inputStat) {
        int index = Stats.IndexOf(inputStat);
        if (index == -1) {
            string debugString = string.Format("Failed to find {0} in {1}. Entries: ", inputStat.name, gameObject.name);

            for (int i = 0; i < Stats.Count; i++) {
                debugString += ", " + Stats[i].name;
            }

            Debug.LogErrorFormat(debugString);
            return null;
        }

        //Debug.LogFormat("[GetTextObject]Stats.Count: {0}, TextObjects.Count:{1}, index: {2}, inputStat: {3}",Stats.Count,TextObjects.Count, index,inputStat.name);
        GameObject textObject = TextObjects[index];
        return textObject;
    }

    public void SetText(CharacterStat inputStat, string inputText) {
        //Debug.LogFormat("[SetText]Stats.Count: {0}, TextObjects.Count:{1}",Stats.Count,TextObjects.Count);
        GameObject textObject = GetTextGameObject(inputStat);
        if (textObject.GetComponent<Text>() != null) {
            textObject.GetComponent<Text>().text = inputText;
        } else if (textObject.GetComponent<TMP_Text>() != null) {
            textObject.GetComponent<TMP_Text>().text = inputText;
        } else if (textObject.GetComponent<TextMeshPro>() != null) {
            textObject.GetComponent<TextMeshPro>().text = inputText;
        }
    }

    public void AddStat(CharacterStat inputStat, object source = null) {
        if (Stats.Find((stat) => stat.name == inputStat.name) == null) {
            CharacterStat newStat = Instantiate(ScriptableObject.CreateInstance<CharacterStat>());
            newStat.name = inputStat.name;
            newStat.Init(0, 0);

            Stats.Add(newStat);
            TextObjects.Add(null);
            AddStat(inputStat, source);
        } else {
            StatModifier newMod = new StatModifier(inputStat.Value, StatModType.Flat, source);
            Stats.Find((stat) => stat.name == inputStat.name).AddModifier(newMod);
        }
    }

    public void RemoveStat(CharacterStat inputStat) {
        if (Stats.Contains(inputStat)) {
            for (int i = Stats.Count - 1; i >= 0; i--) {
                if (Stats[i].name == inputStat.name) {
                    Stats.RemoveAt(i);
                    TextObjects.RemoveAt(i);
                }
            }
        } else {
            Debug.LogFormat("Doesn't contain {0}", inputStat);
        }
    }

    public void RemoveStatsFromSource(object inputSource) {
        for (int i = Stats.Count - 1; i >= 0; i--) {
            Stats[i].RemoveAllModifiersFromSource(inputSource);
        }

        for (int i = Stats.Count - 1; i >= 0; i--) {
            if (Stats[i].Source != null && Stats[i].Source.Equals(inputSource) && Stats[i].StatModifiers.Count == 0) {
                RemoveStat(Stats[i]);
            }

            if (Stats[i].Source == null && Stats[i].Value.Equals(0)) {
                RemoveStat(Stats[i]);
            }
        }
    }

    public void SetText(string inputStatName, string inputText) {
        for (int i = 0; i < Stats.Count; i++) {
            if (inputStatName == Stats[i].name) {
                SetText(Stats[i], inputText);
                break;
            }
        }
    }

    public CharacterStat GetStat(string inputStatName) {
        for (int i = 0; i < Stats.Count; i++) {
            if (inputStatName == Stats[i].name) {
                return Stats[i];
            }
        }

        return null;
    }

    public void SetTextObject(CharacterStat inputStat, GameObject inputTextObject) {
        int index = Stats.IndexOf(inputStat);
        TextObjects[index] = inputTextObject;
        inputStat.OnValueChanged.Invoke(inputStat);
    }
}