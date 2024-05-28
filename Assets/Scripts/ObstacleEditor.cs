using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObstacleEditor : EditorWindow
{
    private ObstacleData obstacleData;
    GridManager gridManager;
    [MenuItem("Window/Obstacle Editor")]
    public static void ShowWindow()
    {
        GetWindow<ObstacleEditor>("Obstacle Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Obstacle Grid", EditorStyles.boldLabel);
        gridManager = FindObjectOfType<GridManager>();
        obstacleData = (ObstacleData)EditorGUILayout.ObjectField("Obstacle Data", obstacleData, typeof(ObstacleData), false);

        if (obstacleData != null)
        {
            for (int y = 0; y < 10; y++)
            {
                GUILayout.BeginHorizontal();
                for (int x = 0; x < 10; x++)
                {
                    
                    int index = y * 10 + x;
                    obstacleData.obstaclePositions[index] = GUILayout.Toggle(obstacleData.obstaclePositions[index], "");
                }
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Save"))
            {
                EditorUtility.SetDirty(obstacleData);
                AssetDatabase.SaveAssets();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Assign an ObstacleData ScriptableObject to edit.", MessageType.Info);
        }
    }
}