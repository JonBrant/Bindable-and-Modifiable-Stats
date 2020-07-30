#region Using Statements
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;
#endregion

namespace Com.BAMS.TheNegativeOne {
    [Serializable]
    [CreateAssetMenu(fileName = "CharacterStat", menuName = "BAMS/CharacterStat")]
    public class CharacterStat : ScriptableObject {
        #region Public Variables
        public List<StatModifier> StatModifiers;
        public bool ResetOnStart = true;
        public bool ClampValue = false;
        public float BaseValue;
        public float DefaultValue;
        public float MinValue;
        public float MaxValue;

        public object Source;
        public virtual float Value {
            get {
                if (isDirty || !lastBaseValue.Equals(BaseValue)) {
                    lastBaseValue = BaseValue;
                    _value = CalculateFinalValue();
                    isDirty = false;
                    OnValueChanged?.Invoke(this);
                }

                return _value;
            }
        }

        public Action<CharacterStat> OnValueChanged;
        #endregion

        

        #region Private Variables
        
        [HideInInspector] public bool isDirty = true;
        protected float _value;
        protected float lastBaseValue = float.MinValue;
        #endregion

        #region Unity Methods
        void OnEnable() {
            isDirty = true;
            CalculateFinalValue();
        }
        #endregion

        #region Public Methods
        
        public CharacterStat() { StatModifiers = new List<StatModifier>(); }

        public CharacterStat(float baseValue) : this() { BaseValue = baseValue; }
        
        public void Init(float baseValue, float defaultValue, object source = null, float minValue = float.MinValue, float maxValue = float.MaxValue) {
            BaseValue = baseValue;
            DefaultValue = defaultValue;
            Source = source;
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public virtual void AddModifier(StatModifier mod) {
            isDirty = true;
            StatModifiers.Add(mod);
            StatModifiers.Sort(CompareModifierOrder);
        }

        public virtual bool RemoveModifier(StatModifier mod) {
            if (StatModifiers.Remove(mod)) {
                isDirty = true;
                return true;
            }

            return false;
        }

        public virtual bool RemoveAllModifiersFromSource(object source) {
            bool didRemove = false;

            for (int i = StatModifiers.Count - 1; i >= 0; i--) {
                if (StatModifiers[i].Source == source) {
                    isDirty = true;
                    didRemove = true;
                    StatModifiers.RemoveAt(i);
                }
            }

            return didRemove;
        }
        #endregion

        #region Private Methods
        protected virtual int CompareModifierOrder(StatModifier a, StatModifier b) {
            if (a.Order < b.Order)
                return -1;
            else if (a.Order > b.Order)
                return 1;
            return 0; // if (a.Order == b.Order)
        }

        public void Refresh() { OnValueChanged?.Invoke(this); }

        public virtual float CalculateFinalValue() {
            float finalValue = BaseValue;
            float sumPercentAdd = 0; // This will hold the sum of our "PercentAdd" modifiers

            for (int i = 0; i < StatModifiers.Count; i++) {
                StatModifier mod = StatModifiers[i];

                if (mod.Type == StatModType.Flat) {
                    finalValue += mod.Value;
                } else if (mod.Type == StatModType.PercentAdd) // When we encounter a "PercentAdd" modifier
                {
                    sumPercentAdd += mod.Value; // Start adding together all modifiers of this type

                    // If we're at the end of the list OR the next modifer isn't of this type
                    if (i + 1 >= StatModifiers.Count || StatModifiers[i + 1].Type != StatModType.PercentAdd) {
                        finalValue *= 1 + sumPercentAdd; // Multiply the sum with the "finalValue", like we do for "PercentMult" modifiers
                        sumPercentAdd = 0; // Reset the sum back to 0
                    }
                } else if (mod.Type == StatModType.PercentMult) // Percent renamed to PercentMult
                {
                    finalValue *= 1 + mod.Value;
                }
            }

            if (ClampValue) {
                ClampFloat(ref finalValue, MinValue, MaxValue);
            }

            return (float) Math.Round(finalValue, 4);
        }

        void ClampFloat(ref float inputFloat, float minValue, float maxValue) {
            if (inputFloat < minValue) {
                inputFloat = minValue;
            } else if (inputFloat > maxValue) {
                inputFloat = maxValue;
            }
        }
        #endregion
    }
}