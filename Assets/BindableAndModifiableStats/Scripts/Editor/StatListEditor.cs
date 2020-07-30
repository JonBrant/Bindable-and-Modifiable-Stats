#region Using Statements
using System;
using System.Collections;
using System.Collections.Generic;
using Com.BAMS.TheNegativeOne;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
#endregion

[CustomEditor(typeof(StatList), true)]
public class StatListEditor : Editor {
    SerializedProperty myInnerObjectSO;

    private ReorderableList statList;

    private bool useDefault = false;
    private bool updateEveryFrame = true;

    private float defaultLabelWidth = 40;
    private float largeLabelWidth = 70;
    private float fieldWidth = 35;
    private float defaultSpacing = 10;
    private int lastClickedIndex = -1;
    private string defaultStatPath = "Assets/BindableAndModifiableStats/ScriptableObjects";
    private string playerPrefStatPathKey = "defaultStatPath";
    private string playerPrefStatPath;
    private List<bool> foldouts = new List<bool>() {true, true}; //Needed to handle multiple foldouts. Otherwise they all open/close at the same time.

    void SetUpStatDirectory() {
        if (!PlayerPrefs.HasKey(playerPrefStatPathKey)) {
            PlayerPrefs.SetString(playerPrefStatPathKey, defaultStatPath);
        } else {
            playerPrefStatPath = PlayerPrefs.GetString(playerPrefStatPathKey);
            if (string.IsNullOrEmpty(playerPrefStatPath)) {
                Debug.LogFormat("PlayerPref Stat path was null! Reverting to default.");
                PlayerPrefs.SetString(playerPrefStatPathKey, defaultStatPath);
            }

            if (!Directory.Exists(playerPrefStatPath)) {
                Debug.LogErrorFormat("Specified Stat path does not exist! Setting to default.");
                PlayerPrefs.SetString(playerPrefStatPathKey, defaultStatPath);
            }
        }
    }

    private void OnEnable() {
        //Check PlayerPrefs for path
        SetUpStatDirectory();
        StatList targetStatlist = (StatList) target;
        statList = new ReorderableList(serializedObject, serializedObject.FindProperty("Stats"), true, false, true, true);
        statList.headerHeight = 1.0f;
        statList.onRemoveCallback = (targetStatList) => {
            targetStatlist.Stats.RemoveAt(targetStatList.index);
            targetStatlist.TextObjects.RemoveAt(targetStatList.index);
        };
        statList.onSelectCallback = (targetStatList) => { lastClickedIndex = targetStatList.index; };
        statList.onReorderCallback = (targetStatList) => {
            GameObject temp = targetStatlist.TextObjects[targetStatList.index];
            targetStatlist.TextObjects[targetStatList.index] = targetStatlist.TextObjects[lastClickedIndex];
            targetStatlist.TextObjects[lastClickedIndex] = temp;
        };

        statList.elementHeightCallback = (int index) => {
            float returnValue = EditorGUIUtility.singleLineHeight;
            var element = statList.serializedProperty.GetArrayElementAtIndex(index);
            if (element == null) {
                targetStatlist.Stats.RemoveAt(index);
                targetStatlist.TextObjects.RemoveAt(index);
                return returnValue;
            } else {
            }

            CharacterStat currentCharacterStat = ((StatList) element.serializedObject.targetObject).Stats[index];
            var elementExpanded = statList.serializedProperty.GetArrayElementAtIndex(index);
            if (elementExpanded.isExpanded) {
                returnValue = (currentCharacterStat.StatModifiers.Count + 1) * EditorGUIUtility.singleLineHeight;
            }

            return (int) returnValue;
        };
        statList.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                SerializedProperty element = statList.serializedProperty.GetArrayElementAtIndex(index);
                CharacterStat currentCharacterStat = ((StatList) element.serializedObject.targetObject).Stats[index];
                GameObject currentTextObject = ((StatList) element.serializedObject.targetObject).TextObjects[index];
                if (currentCharacterStat == null) {
                    return;
                }

                Rect rowRect = new Rect(rect.x, rect.y, defaultLabelWidth, EditorGUIUtility.singleLineHeight);

                GUIStyle titleGUIStyle = new GUIStyle();
                titleGUIStyle.fontStyle = FontStyle.Bold;

                //Title (Stat Name)
                rowRect.width = largeLabelWidth;
                EditorGUI.LabelField(rowRect, currentCharacterStat.name, titleGUIStyle);
                rowRect.x += rowRect.width + defaultSpacing;
                rowRect.y -= 2; //For some reason the bold title makes everything else lower

                //Value (CurrentValue)
                rowRect.width = defaultLabelWidth;
                EditorGUI.LabelField(rowRect, "Value");
                rowRect.x += rowRect.width;
                rowRect.width = fieldWidth;
                GUI.enabled = false; // Make the field read only
                EditorGUI.FloatField(rowRect, currentCharacterStat.Value);
                GUI.enabled = true;
                rowRect.x += rowRect.width + defaultSpacing;

                EditorGUI.BeginChangeCheck();

                //BaseValue
                rowRect.width = largeLabelWidth;
                EditorGUI.LabelField(rowRect, "BaseValue");
                rowRect.x += rowRect.width;
                rowRect.width = fieldWidth;
                currentCharacterStat.BaseValue = EditorGUI.FloatField(rowRect, currentCharacterStat.BaseValue);
                rowRect.x += rowRect.width + defaultSpacing;
                if (EditorGUI.EndChangeCheck()) {
                    EditorUtility.SetDirty(target);
                }

                //Text object field
                rowRect.width = fieldWidth + largeLabelWidth;
                ((StatList) element.serializedObject.targetObject).TextObjects[index] = (GameObject) EditorGUI.ObjectField(rowRect, ((StatList) element.serializedObject.targetObject).TextObjects[index], typeof(GameObject), true);
                if (currentTextObject != null) {
                    if (currentTextObject.GetComponent<Text>() == null && currentTextObject.GetComponent<TMP_Text>() == null) {
                        Debug.LogWarningFormat("Object doesn't have a Text or Text Mesh Pro component on it!");
                        currentTextObject = null;
                    }
                }
                
                //Reset on start checkbox
                EditorGUI.BeginChangeCheck();
                rowRect.x += defaultSpacing;
                rowRect.x += rowRect.width;
                rowRect.width = 15;
                currentCharacterStat.ResetOnStart = EditorGUI.Toggle(rowRect, currentCharacterStat.ResetOnStart);
                rowRect.x += rowRect.width + defaultSpacing;
                rowRect.width = 110;
                EditorGUI.LabelField(rowRect, "Reset On Start");
                rowRect.x += rowRect.width + defaultSpacing;
                if (EditorGUI.EndChangeCheck()) {
                    EditorUtility.SetDirty(target);
                }

                rowRect.x += fieldWidth + largeLabelWidth + 50;
                if (currentCharacterStat.StatModifiers.Count != 0) {
                    SerializedProperty modifiersExpanded = statList.serializedProperty.GetArrayElementAtIndex(index);

                    modifiersExpanded.isExpanded = EditorGUI.Foldout(rowRect, modifiersExpanded.isExpanded, "Modifiers");
                    if (currentCharacterStat.StatModifiers.Count == 0) {
                        modifiersExpanded.isExpanded = false;
                    }

                    if (modifiersExpanded.isExpanded) {
                        DrawModifiers(currentCharacterStat, rowRect);
                    }
                }
            };
        statList.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            var menu = new GenericMenu();

            //Debug.LogFormat("OnAddDropdownCallback() playerPrefStatPath = '{0}'",playerPrefStatPath);
            if (string.IsNullOrEmpty(playerPrefStatPath)) {
                Debug.LogFormat("Path to Stats was null or empty!");
                return;
            }

            var guids = AssetDatabase.FindAssets("", new[] {playerPrefStatPath});
            foreach (var guid in guids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                menu.AddItem(new GUIContent("Stats/" + Path.GetFileNameWithoutExtension(path)),
                    false,
                    clickHandler,
                    new WaveCreationParams() {Path = path});
            }

            menu.ShowAsContext();
        };
    }

    private struct WaveCreationParams {
        //public MobWave.WaveType Type;
        public string Path;
    }

    private void clickHandler(object targetStat) {
        StatList targetStatlist = (StatList) target;

        //var data = (WaveCreationParams)targetStat;
        var data = (WaveCreationParams) targetStat;
        CharacterStat tempStat = AssetDatabase.LoadAssetAtPath(data.Path, typeof(CharacterStat)) as CharacterStat;
        if (!targetStatlist.Stats.Contains(tempStat)) {
            targetStatlist.Stats.Add(tempStat);
            targetStatlist.TextObjects.Add(null);
        } else {
            Debug.LogWarningFormat("StatList already contains {0}!", tempStat.name);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawModifiers(CharacterStat inputStat, Rect rowRect) {
        for (int i = 0; i < inputStat.StatModifiers.Count; i++) {
            rowRect.x = defaultLabelWidth + defaultSpacing;
            rowRect.y += EditorGUIUtility.singleLineHeight;
            rowRect.width = largeLabelWidth;
            EditorGUI.LabelField(rowRect, inputStat.StatModifiers[i].Type.ToString());
            rowRect.x += rowRect.width + defaultSpacing;

            rowRect.width = fieldWidth;
            float oldValue = inputStat.StatModifiers[i].Value;
            inputStat.StatModifiers[i].Value = EditorGUI.FloatField(rowRect, inputStat.StatModifiers[i].Value);
            if (!Mathf.Approximately(inputStat.StatModifiers[i].Value, oldValue)) {
                inputStat.isDirty = true;
            }

            rowRect.x += rowRect.width + defaultSpacing;
            rowRect.width = defaultLabelWidth + 2 * largeLabelWidth;

            EditorGUI.LabelField(rowRect, "Source: " + (inputStat.StatModifiers[i].Source != null ? inputStat.StatModifiers[i].Source.ToString() : "None"));

            //inputStat.isDirty = true;
        }
    }

    void DropAreaGUI() {
        StatList statListTarget = (StatList) target;
        Event evt = Event.current;
        Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(drop_area, "Add Stat");

        switch (evt.type) {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!drop_area.Contains(evt.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform) {
                    DragAndDrop.AcceptDrag();

                    foreach (UnityEngine.Object dragged_object in DragAndDrop.objectReferences) {
                        if (dragged_object is CharacterStat) {
                            Debug.LogFormat("Dragged {0}!", dragged_object.name);
                            if (!statListTarget.Stats.Contains(dragged_object as CharacterStat)) {
                                if (statListTarget.InstantiateStatCopies) {
                                    CharacterStat tempObject = Instantiate(dragged_object) as CharacterStat;
                                    if (tempObject != null) {
                                        tempObject.name = dragged_object.name;
                                        statListTarget.Stats.Add(tempObject);
                                        statListTarget.TextObjects.Add(null);
                                    }
                                } else {
                                    statListTarget.Stats.Add(dragged_object as CharacterStat);
                                    statListTarget.TextObjects.Add(null);
                                }
                            }
                        } else {
                            Debug.LogWarningFormat("Can't drag that into Stats!");
                        }
                    }
                }

                break;
        }
    }

    void DrawOptions() {
        StatList targetStatlist = (StatList) target;
        if (!useDefault) {
            foldouts[0] = EditorGUILayout.Foldout(foldouts[0], "Options");
            if (foldouts[0]) {
                targetStatlist.InstantiateStatCopies = EditorGUILayout.Toggle("Instantiate Copies", targetStatlist.InstantiateStatCopies);
                useDefault = EditorGUILayout.Toggle("Use Default Inspector", useDefault);
                updateEveryFrame = EditorGUILayout.Toggle("Update inspector every frame", updateEveryFrame);
                string oldPath = playerPrefStatPath;
                playerPrefStatPath = EditorGUILayout.TextField("Path to Stats", playerPrefStatPath);
                if (!string.Equals(oldPath, playerPrefStatPath)) {
                    //Debug.LogFormat("Stat path changed! New path: {0}", playerPrefStatPath);
                    PlayerPrefs.SetString(playerPrefStatPathKey, playerPrefStatPath);
                }
            }
        } else {
            useDefault = EditorGUILayout.Toggle("Use Default Inspector", useDefault);
        }
    }

    public override void OnInspectorGUI() {
        DrawOptions();
        serializedObject.Update();
        if (useDefault) {
            base.OnInspectorGUI();
        } else {
            StatList statListTarget = (StatList) target;
            EditorGUI.BeginChangeCheck();
            DropAreaGUI();

            //Draw the list
            var statListFoldout = statList.serializedProperty;
            foldouts[1] = EditorGUILayout.Foldout(foldouts[1], "Stats");
            if (foldouts[1]) {
                statList.DoLayoutList();
            }

            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(target);
            }

            //SetDirty will force it to update every frame.
            if (updateEveryFrame) {
                EditorUtility.SetDirty(target);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}