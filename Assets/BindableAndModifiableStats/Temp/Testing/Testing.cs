#region Using Statements

using System;
using System.Collections;
using System.Collections.Generic;
using Com.BAMS.TheNegativeOne;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#endregion

public class Testing : MonoBehaviour {
    public StatList TargetStatList;

    private void Start() {
        int agiIndex = -1;
        for (int i = 0; i < TargetStatList.Stats.Count; i++) {
            TargetStatList.Stats[i].OnValueChanged += onStatChangedHandler;
            if (TargetStatList.Stats[i].name == "Agility") {
                Debug.LogFormat("Found Agility!");
                agiIndex = i;
                //break;
            }
        }


        TargetStatList.Stats[agiIndex].AddModifier(new StatModifier(7, StatModType.Flat, this));
        TargetStatList.Stats[agiIndex].AddModifier(new StatModifier(7, StatModType.PercentMult, this));
    }

    private void OnDestroy() {
        for (int i = 0; i < TargetStatList.Stats.Count; i++) {
            TargetStatList.Stats[i].OnValueChanged -= onStatChangedHandler;
        }
    }

    void onStatChangedHandler(CharacterStat inputStat) {
        TargetStatList.SetText(inputStat,string.Format("{0} - {1}", inputStat.name,inputStat.Value));
    }
}