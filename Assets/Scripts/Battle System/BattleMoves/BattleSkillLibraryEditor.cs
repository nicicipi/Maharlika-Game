using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BattleSkillLibrary))]
public class BattleSkillLibraryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BattleSkillLibrary library = (BattleSkillLibrary)target;

        if (GUILayout.Button("Load All Battle Skills"))
        {
            library.LoadAllBattleSkills();

            EditorUtility.SetDirty(library);
            AssetDatabase.SaveAssets();
        }
    }
}