#region Using Statements

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion    

public class ObjectClick : MonoBehaviour {
    public static event Action<GameObject> ObjectClicked;
    public StatList ObjectStatList;

    private void OnMouseDown() {
        ObjectClicked?.Invoke(this.gameObject);
    }
}