#region Using Statements
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

[Serializable]
public class StatModifier {
    #region Public Variables
    public float Value;
    public StatModType Type;
    public int Order;
    public object Source;
    #endregion

    #region Private Variables
    #endregion

    #region Unity Methods
    void Start() { }

    void Update() { }
    #endregion

    #region Public Methods
    public StatModifier(float value, StatModType type, int order, object source) // Added "source" input parameter
    {
        Value = value;
        Type = type;
        Order = order;
        Source = source; // Assign Source to our new input parameter
    }

    public StatModifier(float value, StatModType type) : this(value, type, (int) type, null) { }

    // Requires Value, Type and Order. Sets Source to its default value: null
    public StatModifier(float value, StatModType type, int order) : this(value, type, order, null) { }

    // Requires Value, Type and Source. Sets Order to its default value: (int)Type
    public StatModifier(float value, StatModType type, object source) : this(value, type, (int) type, source) { }
    #endregion

    #region Private Methods
    #endregion
}