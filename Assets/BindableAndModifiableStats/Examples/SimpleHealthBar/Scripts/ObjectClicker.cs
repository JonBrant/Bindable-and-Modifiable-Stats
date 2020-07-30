#region Using Statements

using System;
using System.Collections;
using System.Collections.Generic;
using Com.BAMS.TheNegativeOne;
using UnityEngine;
using Random = System.Random;

#endregion

public class ObjectClicker : MonoBehaviour {
    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f)) {
                Random randomDamage = new Random();
                int damageAmount = randomDamage.Next(1, 9);
                CharacterStat HealthStat = hit.transform.GetComponent<StatList>().GetStat("Health");
                HealthStat.BaseValue-=damageAmount;
                Debug.LogFormat("{0} took {1} damage! Remaining: {2}/{3}", hit.transform.name, damageAmount, HealthStat.Value,HealthStat.MaxValue); 
            }
        }
    }
}