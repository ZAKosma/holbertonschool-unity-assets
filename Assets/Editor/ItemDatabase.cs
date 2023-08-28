using System.Collections.Generic;
using System.Linq;
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
        items = ApplyFilter(items);
        int showingItems = items.Count();

        GUILayout.Label($"Showing {showingItems} out of {totalItems} items.");

        foreach (T item in items)
        {
            DrawItemInList(item);
        }

        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    protected virtual void DrawItemInList(T item)
    {
        Texture2D iconTexture = item.icon != null ? item.icon.texture : defaultIcon;
        Rect rect = EditorGUILayout.BeginHorizontal(GUI.skin.box);
        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
        {
            selectedItem = item;
            Selection.activeObject = item as UnityEngine.Object;
            Event.current.Use();
        }

        GUILayout.Box(iconTexture, GUILayout.Width(30), GUILayout.Height(30));
        GUILayout.Label(item.name);
        EditorGUILayout.EndHorizontal();
    }

    protected abstract void DrawPropertiesSection();
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

    private void OnGUI()
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
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(selectedItem as UnityEngine.Object));
            selectedItem = null;
            AssetDatabase.Refresh();
        }
    }

    protected void DuplicateSelectedItem()
    {
        if (selectedItem != null)
        {
            string path = AssetDatabase.GetAssetPath(selectedItem as UnityEngine.Object);
            AssetDatabase.CopyAsset(path, path.Replace(".asset", "_duplicate.asset"));
            AssetDatabase.Refresh();
        }
    }

    protected abstract void ExportItemsToCSV();

    protected abstract void ImportItemsFromCSV();

    protected void DrawTopLeftOptions()
    {
        GUILayout.Label("Database Admin Functions:", EditorStyles.boldLabel);
        if (GUILayout.Button("Export to CSV")) ExportItemsToCSV();
        if (GUILayout.Button("Import from CSV")) ImportItemsFromCSV();
        if (GUILayout.Button("Delete Selected Item")) DeleteSelectedItem();
        if (GUILayout.Button("Duplicate Selected Item")) DuplicateSelectedItem();
        
        // ... additional top-left options, like 'Create New Item' or 'Search' ...
    }
}