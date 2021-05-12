using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class IFXAnimationEffectEditorSEND : EditorWindow
{

    IFXAnimEffect_SEND_MAIN[] InstancesOfSEND_Main;
    IFXAnimEffect_SEND_Module[] SENDModules;

    //////////////////////////////////////////////
    Vector2 scrollBar_MAIN_Pos;
    Vector2 scrollBar_Modules_Pos;

    /////////////////////////////////////////////

    [MenuItem("IFXAnimationEffect/IFXAnimationEffect Editor - SEND")]
    static void Init()
    {
        
        EditorWindow window = EditorWindow.GetWindow(typeof(IFXAnimationEffectEditorSEND));
        window.minSize = new Vector2(500,700);
        window.Show();
    }

    void OnEnable ()
    {
        //EditorWindow.GetWindow(typeof(IFXAnimationEffectEditor_SEND));
        InstancesOfSEND_Main = FindObjectsOfType(typeof(IFXAnimEffect_SEND_MAIN)) as IFXAnimEffect_SEND_MAIN[];
        SENDModules = FindObjectsOfType(typeof(IFXAnimEffect_SEND_Module)) as IFXAnimEffect_SEND_Module[];        
    }

    void OnGUI()
    {

        //the button area on the left stuff
        Rect SENDMAIN_Instances_Group_RECT = new Rect(5, 25, (this.position.width / 2) - 10, this.position.height - 100);
        GUILayout.BeginArea(SENDMAIN_Instances_Group_RECT);
        DisplaySENDModule_MAIN_Instances();
        GUILayout.EndArea();

        Rect SEND_Properties_Group_RECT = new Rect(SENDMAIN_Instances_Group_RECT.width + 10, 0, this.position.width - SENDMAIN_Instances_Group_RECT.width - 10, this.position.height);

        GUILayout.BeginArea(SEND_Properties_Group_RECT);
        DisplaySENDModuleProperties();
        GUILayout.EndArea();

    }

    private void DisplaySENDModule_MAIN_Instances()
    {
        scrollBar_MAIN_Pos = EditorGUILayout.BeginScrollView(scrollBar_MAIN_Pos);
        foreach (IFXAnimEffect_SEND_MAIN item in InstancesOfSEND_Main)
        {
            SerializedObject serializedObject = new SerializedObject(item);

            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.fontSize=20;
            labelStyle.normal.textColor = Color.white;

            EditorGUILayout.LabelField(item.gameObject.name, labelStyle);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("updateRate"), includeChildren: true);
            SerializedProperty SENDModulesArray = serializedObject.FindProperty("SEND_Modules");
            SerializedProperty SENDModulesArraySize = serializedObject.FindProperty("SEND_Modules" + ".Array.size");
            EditorGUILayout.PropertyField(SENDModulesArraySize, includeChildren: true);
            serializedObject.ApplyModifiedProperties();
            for (int i = 0; i < SENDModulesArray.arraySize; i++)
            {
                if (item.SEND_Modules[i] != null)
                {
                    item.SEND_Modules[i].custom_Name = EditorGUILayout.TextField("custom_Name", item.SEND_Modules[i].custom_Name);
                }

                EditorGUILayout.PropertyField(SENDModulesArray.GetArrayElementAtIndex(i), includeChildren: true);
                EditorGUILayout.Space();
            }

            // var serializedProperty = serializedObject.GetIterator();
            // while (serializedProperty.NextVisible(true))
            // {
            //     if (serializedProperty.name == "custom_Name" || serializedProperty.displayName == "Script" || serializedProperty.name == "AnimationEffectVariable") // this may cause missing items bug
            //     {
            //         //EditorGUILayout.PropertyField(serializedProperty, includeChildren: true);
            //     }
            //     else
            //     {
            //         EditorGUILayout.PropertyField(serializedProperty, includeChildren: true);
            //     }

            // }
            serializedObject.ApplyModifiedProperties();
        }
        EditorGUILayout.EndScrollView();
    }

    private void DisplaySENDModuleProperties()
    {
        scrollBar_Modules_Pos = EditorGUILayout.BeginScrollView(scrollBar_Modules_Pos);
        foreach (IFXAnimEffect_SEND_Module item in SENDModules)
        {
            SerializedObject serializedObject = new SerializedObject(item);
            GUIStyle moduleStyle = new GUIStyle();
            moduleStyle.fontSize = 20;
            moduleStyle.normal.textColor = Color.white;
            //moduleStyle.fontStyle = FontStyle.Bold;
            EditorGUILayout.LabelField(item.gameObject.name + " - " + item.moduleType, moduleStyle);


            item.custom_Name = EditorGUILayout.TextField("custom_Name", item.custom_Name);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("AnimationEffectVariable"), includeChildren: true);

            item.isExpanded = EditorGUILayout.InspectorTitlebar(item.isExpanded,item);
            if (item.isExpanded)
            {
                //Loop through all the serialised Properties 
                var serializedProperty = serializedObject.GetIterator();
                while (serializedProperty.NextVisible(true))
                {
                    if (serializedProperty.name == "custom_Name" || serializedProperty.displayName == "Script" || serializedProperty.name == "AnimationEffectVariable") // this may cause missing items bug
                    {
                        //EditorGUILayout.PropertyField(serializedProperty, includeChildren: true);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(serializedProperty, includeChildren: true);
                    }

                }
                //serializedObject.ApplyModifiedProperties();
            }
            serializedObject.ApplyModifiedProperties();


            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            //EditorGUILayout.PropertyField(item.AnimationEffectVariable.name);
        }
        EditorGUILayout.EndScrollView();
    }
}
// public class IFXAnimationEffectEditor_SEND : EditorWindow
// {
//     [MenuItem("IFXAnimationEffect/IFXAnimationEffect Editor SEND")]
//     static void Init()
//     {
//         EditorWindow window = EditorWindow.GetWindow(typeof(IFXAnimationEffectEditor_SEND));
//         window.minSize = new Vector2(500,700);
//         window.Show();
//     }
//     private void OnEnable() 
//     {
        
        
//     }
// }







//////////////////////////////////////////




public class IFXAnimationEffectEditorRECEIVE : EditorWindow
{
    IFXAnimEffect_RECEIVE_MAIN[] InstancesOfRECEIVE_Main;
    IFXAnimEffect_RECEIVE_Module[] RECEIVEModules;


    //////////////////////////////////////////////
    Vector2 scrollBar_MAIN_Pos;
    Vector2 scrollBar_Modules_Pos;

    /////////////////////////////////////////////

    [MenuItem("IFXAnimationEffect/IFXAnimationEffect Editor - RECEIVE")]
    static void Init()
    {
        
        EditorWindow window = EditorWindow.GetWindow(typeof(IFXAnimationEffectEditorRECEIVE));
        window.minSize = new Vector2(500,700);
        window.Show();
    }

    void OnEnable ()
    {
        //EditorWindow.GetWindow(typeof(IFXAnimationEffectEditor_SEND));
        InstancesOfRECEIVE_Main = FindObjectsOfType(typeof(IFXAnimEffect_RECEIVE_MAIN)) as IFXAnimEffect_RECEIVE_MAIN[];
        RECEIVEModules = FindObjectsOfType(typeof(IFXAnimEffect_RECEIVE_Module)) as IFXAnimEffect_RECEIVE_Module[];
        
    }

    void OnGUI()
    {

        //the button area on the left stuff
        Rect RECEIVEMAIN_Instances_Group_RECT = new Rect(5, 25, (this.position.width / 2) - 10, this.position.height - 100);
        GUILayout.BeginArea(RECEIVEMAIN_Instances_Group_RECT);
        DisplayRECEIVEModule_MAIN_Instances();
        GUILayout.EndArea();

        Rect RECEIVE_Properties_Group_RECT = new Rect(RECEIVEMAIN_Instances_Group_RECT.width + 10, 0, this.position.width - RECEIVEMAIN_Instances_Group_RECT.width - 10, this.position.height);

        GUILayout.BeginArea(RECEIVE_Properties_Group_RECT);
        DisplayRECEIVEModuleProperties();
        GUILayout.EndArea();

    }

    private void DisplayRECEIVEModule_MAIN_Instances()
    {
        scrollBar_MAIN_Pos = EditorGUILayout.BeginScrollView(scrollBar_MAIN_Pos);
        foreach (IFXAnimEffect_RECEIVE_MAIN item in InstancesOfRECEIVE_Main)
        {
            SerializedObject serializedObject = new SerializedObject(item);

            GUIStyle labelStyleH1 = new GUIStyle(EditorStyles.label);
            labelStyleH1.fontSize=20;
            labelStyleH1.normal.textColor = Color.white;
            GUIStyle labelStyleH2 = new GUIStyle(EditorStyles.label);
            labelStyleH2.fontSize=15;
            labelStyleH2.normal.textColor = Color.white;

            EditorGUILayout.LabelField(item.gameObject.name, labelStyleH1);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AnimationEffectVariable"));
            SerializedProperty RECEIVEModulesArray = serializedObject.FindProperty("RECEIVE_Modules");
            SerializedProperty RECEIVEModulesArraySize = serializedObject.FindProperty("RECEIVE_Modules" + ".Array.size");
            EditorGUILayout.PropertyField(RECEIVEModulesArraySize, includeChildren: true);
            serializedObject.ApplyModifiedProperties();
            for (int i = 0; i < RECEIVEModulesArray.arraySize; i++)
            {
                if (item.RECEIVE_Modules[i] != null)
                {
                    item.RECEIVE_Modules[i].custom_Name = EditorGUILayout.TextField("custom_Name", item.RECEIVE_Modules[i].custom_Name);
                }

                EditorGUILayout.PropertyField(RECEIVEModulesArray.GetArrayElementAtIndex(i), includeChildren: true);
                EditorGUILayout.Space();
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentValue"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("weight"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("inputSmoothing"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("valueLimiter"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("moreThan"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lessThan"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("useValueRangeLimit"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("valueRange"));

            EditorGUILayout.LabelField("Trigger Settings:", labelStyleH2);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("triggerThreshold"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("moreThan_Trigger"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lessThan_Trigger"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("whenNotEqualTo_Trigger"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("notEqualToValue"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("useTriggerValueRangeLimit"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("triggerValueRange"));
            


            // var serializedProperty = serializedObject.GetIterator();
            // while (serializedProperty.NextVisible(true))
            // {
            //     if (serializedProperty.name == "custom_Name" || serializedProperty.displayName == "Script" || serializedProperty.name == "RECEIVE_Modules") // this may cause missing items bug
            //     {
            //         //EditorGUILayout.PropertyField(serializedProperty, includeChildren: true);
            //     }
            //     else
            //     {
            //         EditorGUILayout.PropertyField(serializedProperty, includeChildren: true);
            //     }

            // }
            serializedObject.ApplyModifiedProperties();
        }
        EditorGUILayout.EndScrollView();
    }

    private void DisplayRECEIVEModuleProperties()
    {
        scrollBar_Modules_Pos = EditorGUILayout.BeginScrollView(scrollBar_Modules_Pos);
        foreach (IFXAnimEffect_RECEIVE_Module item in RECEIVEModules)
        {
            SerializedObject serializedObject = new SerializedObject(item);
            GUIStyle moduleStyle = new GUIStyle();
            moduleStyle.fontSize = 20;
            moduleStyle.normal.textColor = Color.white;
            //moduleStyle.fontStyle = FontStyle.Bold;
            EditorGUILayout.LabelField(item.gameObject.name + " - " + item.moduleType, moduleStyle);


            item.custom_Name = EditorGUILayout.TextField("custom_Name", item.custom_Name);

            //EditorGUILayout.PropertyField(serializedObject.FindProperty("AnimationEffectVariable"), includeChildren: true);

            item.isExpanded = EditorGUILayout.InspectorTitlebar(item.isExpanded,item);
            if (item.isExpanded)
            {
                //Loop through all the serialised Properties 
                var serializedProperty = serializedObject.GetIterator();
                while (serializedProperty.NextVisible(true))
                {
                    if (serializedProperty.name == "custom_Name" || serializedProperty.displayName == "Script") // this may cause missing items bug
                    {
                        //EditorGUILayout.PropertyField(serializedProperty, includeChildren: true);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(serializedProperty, includeChildren: true);
                    }

                }
                //serializedObject.ApplyModifiedProperties();
            }
            serializedObject.ApplyModifiedProperties();


            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            //EditorGUILayout.PropertyField(item.AnimationEffectVariable.name);
        }
        EditorGUILayout.EndScrollView();
    }
}
// public class IFXAnimationEffectEditor_RECEIVE : EditorWindow
// {
//     [MenuItem("IFXAnimationEffect/IFXAnimationEffect Editor SEND")]
//     static void Init()
//     {
//         EditorWindow window = EditorWindow.GetWindow(typeof(IFXAnimationEffectEditor_RECEIVE));
//         window.minSize = new Vector2(500,700);
//         window.Show();
//     }
//     private void OnEnable() 
//     {
        
        
//     }
// }

