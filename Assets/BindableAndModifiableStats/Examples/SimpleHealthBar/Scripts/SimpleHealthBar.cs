#region Using Statements
using System;
using System.Collections;
using System.Collections.Generic;
using Com.BAMS.TheNegativeOne;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion    

public class SimpleHealthBar : MonoBehaviour {
    public Image HealthBarFill;
    public TextMeshProUGUI HealthBarText;
    private void Start() {
        CharacterStat HealthStat = GetComponent<StatList>().GetStat("Health");
        //You should always nullcheck
        if (HealthStat != null) {
            HealthStat.OnValueChanged += OnHealthChanged;
            //This just invokes OnValueChanged, without changing anything.
            //This is good for UI initialization.
            HealthStat.Refresh();
        }
    }

    private void OnDestroy() {
        //This should be broken up, to check if GetStat found the value.
        GetComponent<StatList>().GetStat("Health").OnValueChanged -= OnHealthChanged;
    }

    void OnHealthChanged(CharacterStat input) {
        HealthBarFill.fillAmount = input.Value / input.MaxValue;
        HealthBarText.text = string.Format("{0} / {1}", input.Value,input.MaxValue);
    }
}