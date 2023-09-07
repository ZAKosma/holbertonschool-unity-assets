using UnityEngine;
using UnityEditor;
using System.IO;

public class DatabasesManager : EditorWindow
{
    private Vector2 scrollPosition;

    [MenuItem("Window/Item Manager/Databases Manager")]
    public static void ShowWindow()
    {
        GetWindow<DatabasesManager>("Databases Manager");
    }

    private void OnGUI()
    {
        // Title
        EditorGUILayout.LabelField("Item Database Manager", EditorStyles.boldLabel);
        
        // Description
        EditorGUILayout.HelpBox("Welcome to the Item Database Manager! Here, you can access and manage different databases related to weapons, potions, armors, and other items.", MessageType.Info);

        // Information about overall data
        GUILayout.Label("Overall Statistics:", EditorStyles.boldLabel);
        GUILayout.Label($"Total Items: {AssetDatabase.FindAssets("t:BaseItem").Length}");
        GUILayout.Label($"Total Weapons: {AssetDatabase.FindAssets("t:Weapon").Length}");
        GUILayout.Label($"Total Potions: {AssetDatabase.FindAssets("t:Potion").Length}");
        GUILayout.Label($"Total Armors: {AssetDatabase.FindAssets("t:Armor").Length}");

        EditorGUILayout.Space();
        if (GUILayout.Button("Import Weapons from CSV"))
        {
            string path = EditorUtility.OpenFilePanel("Select CSV File", "", "csv");
            if (path.Length != 0)
            {
                WeaponDatabase w = new WeaponDatabase();
                w.ImportItemsFromCSV(path);
            }
        }
        if (GUILayout.Button("Import Potions from CSV"))
        {
            string path = EditorUtility.OpenFilePanel("Select CSV File", "", "csv");
            if (path.Length != 0)
            {
                PotionDatabase p = new PotionDatabase();
                p.ImportItemsFromCSV(path);
            }
        }

        if (GUILayout.Button("Import Armor from CSV"))
        {
            string path = EditorUtility.OpenFilePanel("Select CSV File", "", "csv");
            if (path.Length != 0)
            {
                ArmorDatabase a = new ArmorDatabase();
                a.ImportItemsFromCSV(path);
            }
        }
        
        // Separator
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        // Buttons to access other windows
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        if (GUILayout.Button("Weapon Database")) WeaponDatabase.ShowWindow();
        if (GUILayout.Button("Potion Database")) PotionDatabase.ShowWindow();
        if (GUILayout.Button("Armor Database")) ArmorDatabase.ShowWindow();
        // if (GUILayout.Button("Other Items")) OtherItemsDatabase.ShowWindow();
        // if (GUILayout.Button("Item Creation")) ItemCreation.ShowWindow();
        if (GUILayout.Button("Statistics Page")) StatisticsPage.ShowWindow();

        EditorGUILayout.EndScrollView();
    }
    
}