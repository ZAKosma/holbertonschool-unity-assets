using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public abstract class ItemDatabase<T> : EditorWindow where T : BaseItem
{
    protected Vector2 scrollPosition;
    protected T selectedItem;
    protected string searchString = "";
    protected float propertiesSectionWidth = 400f; 
    protected bool isResizingPropertiesSection;
    
    protected Texture2D defaultIcon;
    
    protected HashSet<string> namesSeen = new HashSet<string>();
    protected HashSet<string> duplicateNames = new HashSet<string>();


    public ItemDatabase()
    {
        this.minSize = new Vector2(600, 400); 
    }
    
    protected virtual void DrawItemList()
    {
        float itemListWidth = position.width - propertiesSectionWidth - 5;
        GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(itemListWidth));

        // Top Left Options
        DrawTopLeftOptions();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, true);
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
        IEnumerable<T> items = guids.Select(guid =>
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        });
        int totalItems = items.Count();
        var filteredItems = ApplyFilter(items);
        int showingItems = filteredItems.Count();

        GUILayout.Label($"Showing {showingItems} out of {totalItems} items.");

        foreach (T item in filteredItems)
        {
            DrawItemInList(item);
        }

        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    protected abstract bool IsValid(T item, out string issue);

    protected virtual void DrawItemInList(T item)
    {
        Texture2D iconTexture = item.icon != null ? item.icon.texture : defaultIcon;
        
        Color defaultValue = GUI.backgroundColor;
        if (object.Equals(selectedItem, item))
        {
            GUI.backgroundColor = Color.blue;
        }
        
        string issue;
        bool isValid = IsValid(item, out issue);
        
        
        // Set the background color based on item validity and selection
        if (object.Equals(selectedItem, item))
        {
            GUI.backgroundColor = Color.blue;
        }
        else if (!isValid)
        {
            GUI.backgroundColor = Color.red;
        }

        
        Rect rect = EditorGUILayout.BeginHorizontal(GUI.skin.box);
        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
        {
            selectedItem = item;
            Selection.activeObject = item as UnityEngine.Object;
            Event.current.Use();
        }

        GUILayout.Box(iconTexture, GUILayout.Width(30), GUILayout.Height(30));
        GUILayout.Label(item.itemName);
        EditorGUILayout.EndHorizontal();


        GUI.backgroundColor = defaultValue;
    }

    protected virtual void DrawPropertiesSection()
    {
        // Make the "Properties Section:" label bold
        GUILayout.Label("Properties Section:", EditorStyles.boldLabel);
        if (selectedItem != null)
        {
            // Increase spacing between properties for better layout
            EditorGUIUtility.labelWidth = 140;
            EditorGUILayout.Space();
            DrawSelectedItemProperties(selectedItem);
        }
    }

    protected abstract void DrawSelectedItemProperties(T item);
    protected abstract void InitializeDefaultFilterOptions();
    
    protected abstract IEnumerable<T> ApplyFilter(IEnumerable<T> items);

    protected void DrawDividerAndHandle()
    {
        // Draw background color
        Rect rect = GUILayoutUtility.GetRect(5, 5, position.height, 5);
        EditorGUI.DrawRect(rect, new Color(0.6f, 0.6f, 0.6f));

        // Draw handle texture (using Unity's built-in texture)
        GUIStyle resizeHandleStyle = new GUIStyle();
        resizeHandleStyle.normal.background = EditorGUIUtility.Load("icons/d_AvatarBlendBackground.png") as Texture2D;
        GUI.Box(new Rect(rect.x, rect.y, rect.width, rect.height), "", resizeHandleStyle);

        EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeHorizontal);

        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
        {
            isResizingPropertiesSection = true;
        }
    }

    protected void HandleDividerDrag()
    {
        
        if (isResizingPropertiesSection)
        {
            propertiesSectionWidth -= Event.current.delta.x;
            // Ensure the properties section width is within bounds
            float maxPropertiesSectionWidth = position.width * 0.8f; // 80% of the window's width
            propertiesSectionWidth = Mathf.Clamp(propertiesSectionWidth, 200, maxPropertiesSectionWidth);
            Repaint();
        }

        if (Event.current.type == EventType.MouseUp)
        {
            isResizingPropertiesSection = false;
        }
    }

    protected void OnGUI()
    {
        GUILayout.BeginHorizontal();
        
        DrawItemList();
        
        DrawDividerAndHandle();

        GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(propertiesSectionWidth));
        DrawPropertiesSection();
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
        
        HandleDividerDrag();
    }
    
    private void OnEnable()
    { 
        defaultIcon = Resources.Load<Texture2D>("DefaultIcon"); 
        InitializeDefaultFilterOptions();
    }
    
    
    protected void DeleteSelectedItem()
    {
        if (selectedItem != null)
        {
            // Display confirmation dialog
            bool isConfirmed = EditorUtility.DisplayDialog(
                "Delete Asset",
                "Are you sure you want to delete the " + selectedItem.itemName +  " asset?",
                "Yes",
                "No"
            );

            // Only delete the asset if the user clicked 'Yes'
            if (isConfirmed)
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(selectedItem as UnityEngine.Object));
                selectedItem = null;
                AssetDatabase.Refresh();
            }
        }
    }

    protected void DuplicateSelectedItem()
    {
        if (selectedItem != null)
        {
            string path = AssetDatabase.GetAssetPath(selectedItem as UnityEngine.Object);
            AssetDatabase.CopyAsset(path, AssetDatabase.GenerateUniqueAssetPath(path.Replace(".asset", "_duplicate.asset")));
            AssetDatabase.Refresh();
        }
    }

    public abstract void ExportItemsToCSV(string path);

    public abstract void ImportItemsFromCSV(string path);

    protected void DrawTopLeftOptions()
    {
        GUILayout.Label("Database Admin Functions:", EditorStyles.boldLabel);
        if (GUILayout.Button("Export to CSV"))
        {
            // Show dialog for user to enter the CSV file name
            string fileName = EditorUtility.SaveFilePanel(
                "Save Weapons as CSV",
                "",
                "Weapons.csv",
                "csv");

            // If the user pressed the cancel button (fileName will be empty)
            if (string.IsNullOrEmpty(fileName)) return;

            ExportItemsToCSV(fileName);
        }
        if (GUILayout.Button("Import from CSV"))
        {
            string path = EditorUtility.OpenFilePanel("Select CSV File", "", "csv");
            if (path.Length != 0)
            {
                ImportItemsFromCSV(path);
            }
        }
        if (GUILayout.Button("Create New Item"))
        {
            if (typeof(T) == typeof(Weapon))
            {
                WeaponCreation.ShowWindow();
            }
            else if (typeof(T) == typeof(Armor))
            {
                ArmorCreation.ShowWindow();
            }
            else if (typeof(T) == typeof(Potion))
            {
                PotionCreation.ShowWindow();
            }
        }
        if (GUILayout.Button("Delete Selected Item")) DeleteSelectedItem();
        if (GUILayout.Button("Duplicate Selected Item")) DuplicateSelectedItem();
        
        // ... additional top-left options, like 'Create New Item' or 'Search' ...
        // Search
        EditorGUILayout.BeginHorizontal();
        searchString = EditorGUILayout.TextField("Search:", searchString, GUILayout.Height(20));
        if (GUILayout.Button("X", GUILayout.Width(20)))
        {
            searchString = ""; // Clear search string
        }
        EditorGUILayout.EndHorizontal();
        
        // Filter Options
        DrawFilterOptions();
    }

    protected abstract void DrawFilterOptions();
    protected abstract void ResetFilterOptions();
    
    // Helper method to draw min-max range filters
    protected void DrawRangeFilter(string label, ref float? minValue, ref float? maxValue)
    {
        EditorGUILayout.BeginHorizontal();
        minValue = EditorGUILayout.FloatField("Min " + label + ":", minValue ?? 0f);
        maxValue = EditorGUILayout.FloatField("Max " + label + ":", maxValue ?? 0f);
        EditorGUILayout.EndHorizontal();
    }
    protected void DrawRangeFilter(string label, ref int? minValue, ref int? maxValue)
    {
        EditorGUILayout.BeginHorizontal();
        minValue = EditorGUILayout.IntField("Min " + label + ":", minValue ?? 0);
        maxValue = EditorGUILayout.IntField("Max " + label + ":", maxValue ?? 0);
        EditorGUILayout.EndHorizontal();
    }
}