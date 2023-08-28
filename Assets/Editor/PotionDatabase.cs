using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class PotionDatabase : EditorWindow
{
    private Vector2 scrollPosition;
    private Potion selectedItem;
    private string searchString = "";
    private float propertiesSectionWidth = 400f; // Default width for properties section

    private bool isResizingPropertiesSection; // New flag for resizing

    private Texture2D defaultIcon; // Declare a variable for the default icon

    [MenuItem("Window/Item Manager/Potion Database")]
    public static void ShowWindow()
    {
        GetWindow<PotionDatabase>("Potion Database");
    }

    private void OnEnable()
    {
        // Load the default icon from your Resources folder (or any other path)
        defaultIcon = Resources.Load<Texture2D>("DefaultIcon");
        InitializeDefaultFilterOptions();
    }

    private void OnGUI()
    {
        // Start Horizontal layout
        GUILayout.BeginHorizontal();

        // Potions List Section
        DrawPotionsList();

        // Draw divider
        DrawDividerAndHandle();

        // Properties Section
        GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(propertiesSectionWidth));
        DrawPropertiesSection();
        GUILayout.EndVertical();

        GUILayout.EndHorizontal(); // End Horizontal layout

        // Resize handle logic
        HandleDividerDrag();
    }

    private void DrawPotionsList()
    {
        float PotionsListWidth = position.width - propertiesSectionWidth - 5;
        GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(PotionsListWidth));

        // Top Left Options
        DrawTopLeftOptions();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, true);
        string[] guids = AssetDatabase.FindAssets("t:Potion");
        IEnumerable<Potion> Potions = guids.Select(guid =>
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath<Potion>(path);
        });
        Potions = ApplyFilter(Potions); // Apply the filter
        foreach (Potion Potion in Potions)
        {
            if (!string.IsNullOrEmpty(searchString) &&
                !Potion.itemName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                continue;

            Texture2D iconTexture =
                Potion.icon != null ? Potion.icon.texture : defaultIcon; // Use default icon if Potion's icon is null

            // Draw the elements
            Rect rect = EditorGUILayout.BeginHorizontal(GUI.skin.box);
            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                selectedItem = Potion;
                Selection.activeObject = Potion;
                Event.current.Use();
            }

            GUILayout.Box(iconTexture, GUILayout.Width(30), GUILayout.Height(30));
            GUILayout.Label(Potion.itemName);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        GUILayout.EndVertical();
    }


    private IEnumerable<Potion> ApplyFilter(IEnumerable<Potion> Potions)
    {
        if (filterOptions.potionEffectMask != 0)
            Potions = Potions.Where(w => (filterOptions.potionEffectMask & (PotionEffect)(1 << (int)w.potionEffect)) != 0);
        if (filterOptions.minEffectPower != null)
            Potions = Potions.Where(w => w.effectPower >= filterOptions.minEffectPower.Value);
        if (filterOptions.maxEffectPower != null)
            Potions = Potions.Where(w => w.effectPower <= filterOptions.maxEffectPower.Value);
        if (filterOptions.minDuration != null)
            Potions = Potions.Where(w => w.duration >= filterOptions.minDuration.Value);
        if (filterOptions.maxDuration != null)
            Potions = Potions.Where(w => w.duration <= filterOptions.maxDuration.Value);
        if (filterOptions.minCooldown != null)
            Potions = Potions.Where(w => w.cooldown >= filterOptions.minCooldown.Value);
        if (filterOptions.maxCooldown != null)
            Potions = Potions.Where(w => w.cooldown <= filterOptions.maxCooldown.Value);
        // if (filterOptions.isStackable != null)
        //     Potions = Potions.Where(w => w.isStackable == filterOptions.isStackable.Value);
        if (filterOptions.rarityMask != 0)
            Potions = Potions.Where(w => (filterOptions.rarityMask & (Rarity)(1 << (int)w.rarity)) != 0);
        if (filterOptions.minBaseValue != null)
            Potions = Potions.Where(w => w.baseValue >= filterOptions.minBaseValue.Value);
        if (filterOptions.maxBaseValue != null)
            Potions = Potions.Where(w => w.baseValue <= filterOptions.maxBaseValue.Value);
        if (filterOptions.minRequiredLevel != null)
            Potions = Potions.Where(w => w.requiredLevel >= filterOptions.minRequiredLevel.Value);
        if (filterOptions.maxRequiredLevel != null)
            Potions = Potions.Where(w => w.requiredLevel <= filterOptions.maxRequiredLevel.Value);
        // if (filterOptions.equipSlotMask != 0)
        //     Potions = Potions.Where(w => (filterOptions.equipSlotMask & (EquipSlot)(1 << (int)w.equipSlot)) != 0);

        // Add more filtering logic as needed

        return Potions;
    }


    private PotionFilterOptions filterOptions = new PotionFilterOptions();

    private bool showFilterOptions = false; // Variable to toggle the collapsible menu

    private void DrawFilterOptions()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);

        // Toggle collapsible filter options
        showFilterOptions = EditorGUILayout.Foldout(showFilterOptions, "Filter Options:", true, EditorStyles.foldoutHeader);
        if (showFilterOptions)
        {
            filterOptions.potionEffectMask = (PotionEffect)EditorGUILayout.EnumFlagsField("Potion Type:", filterOptions.potionEffectMask);
            filterOptions.rarityMask = (Rarity)EditorGUILayout.EnumFlagsField("Rarity:", filterOptions.rarityMask);
            // filterOptions.equipSlotMask = (EquipSlot)EditorGUILayout.EnumFlagsField("Equip Slot:", filterOptions.equipSlotMask);

            //filterOptions.isStackable = EditorGUILayout.Toggle("Is Stackable:", filterOptions.isStackable.Value);
            
            // Adding the rest of the filter options
            DrawRangeFilter("Effect Power", ref filterOptions.minEffectPower, ref filterOptions.maxEffectPower);
            DrawRangeFilter("Duration", ref filterOptions.minDuration, ref filterOptions.maxDuration);
            DrawRangeFilter("Cooldown", ref filterOptions.minCooldown, ref filterOptions.maxCooldown);
            DrawRangeFilter("Base Value", ref filterOptions.minBaseValue, ref filterOptions.maxBaseValue);
            DrawRangeFilter("Required Level", ref filterOptions.minRequiredLevel, ref filterOptions.maxRequiredLevel);
        }

        EditorGUILayout.EndVertical();
    }
    
    // Helper method to draw min-max range filters
    private void DrawRangeFilter(string label, ref float? minValue, ref float? maxValue)
    {
        EditorGUILayout.BeginHorizontal();
        minValue = EditorGUILayout.FloatField("Min " + label + ":", minValue ?? 0f);
        maxValue = EditorGUILayout.FloatField("Max " + label + ":", maxValue ?? 0f);
        EditorGUILayout.EndHorizontal();
    }
    private void DrawRangeFilter(string label, ref int? minValue, ref int? maxValue)
    {
        EditorGUILayout.BeginHorizontal();
        minValue = EditorGUILayout.IntField("Min " + label + ":", minValue ?? 0);
        maxValue = EditorGUILayout.IntField("Max " + label + ":", maxValue ?? 0);
        EditorGUILayout.EndHorizontal();
    }


    private void DrawTopLeftOptions()
    {
        DrawFilterOptions();
        // Create Potion
        GUILayout.Label("Create New Potion:", EditorStyles.boldLabel);
        if (GUILayout.Button("Open Potion Creation Window", GUILayout.Height(30)))
        {
            PotionCreation.ShowWindow();
        }

        // Search Bar
        searchString = EditorGUILayout.TextField("Search:", searchString, GUILayout.Height(20));

        // Sorting Options
        // EditorGUILayout.BeginHorizontal();
        // GUILayout.Label("Sort By:");
        // if (GUILayout.Button("Potion Type")) SortPotionsBy(Potion => Potion.PotionType);
        // if (GUILayout.Button("Equip Slot")) SortPotionsBy(Potion => Potion.equipSlot);
        // // Add more sorting options as needed
        // EditorGUILayout.EndHorizontal();

        // Import & Export
        if (GUILayout.Button("Export to CSV")) ExportPotionsToCSV();
        if (GUILayout.Button("Import from CSV")) ImportPotionsFromCSV();

        // Delete & Duplicate
        if (GUILayout.Button("Delete Selected Potion")) DeleteSelectedPotion();
        if (GUILayout.Button("Duplicate Selected Potion")) DuplicateSelectedPotion();
    }

    private void DrawPropertiesSection()
    {
        // Make the "Properties Section:" label bold
        GUILayout.Label("Properties Section:", EditorStyles.boldLabel);
        if (selectedItem != null)
        {
            // Increase spacing between properties for better layout
            EditorGUIUtility.labelWidth = 140;
            EditorGUILayout.Space();
            DrawSelectedPotionProperties(selectedItem);
        }
    }

    private void DrawDividerAndHandle()
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


    private void HandleDividerDrag()
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


    private void DrawSelectedPotionProperties(Potion Potion)
    {
        EditorGUI.BeginChangeCheck();

        // Icon Section - Placed above the name for a more aesthetic look
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace(); // Center the icon
        Potion.icon = (Sprite)EditorGUILayout.ObjectField(Potion.icon, typeof(Sprite), false, GUILayout.Height(64),
            GUILayout.Width(64));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        // Name Section - Make the name prominent
        EditorGUILayout.LabelField("Name:", EditorStyles.boldLabel);
        Potion.itemName = EditorGUILayout.TextField(string.Empty, Potion.itemName, EditorStyles.largeLabel);
        EditorGUILayout.Space(); // Add some space for better separation

        // Other General Properties
        Potion.description = EditorGUILayout.TextField("Description:", Potion.description);
        Potion.baseValue = EditorGUILayout.FloatField("Base Value:", Potion.baseValue);
        Potion.rarity = (Rarity)EditorGUILayout.EnumPopup("Rarity:", Potion.rarity);
        Potion.requiredLevel = EditorGUILayout.IntField("Required Level:", Potion.requiredLevel);
        Potion.equipSlot = (EquipSlot)EditorGUILayout.EnumPopup("Equip Slot:", Potion.equipSlot);

        // Potion Specific Properties
        EditorGUILayout.Space();
        GUILayout.Label("Potion Properties:", EditorStyles.boldLabel);
        Potion.potionEffect = (PotionEffect)EditorGUILayout.EnumPopup("Potion Type:", Potion.potionEffect);
        Potion.effectPower = EditorGUILayout.FloatField("Effect Power:", Potion.effectPower);
        Potion.duration = EditorGUILayout.FloatField("Attack Speed:", Potion.duration);
        Potion.cooldown = EditorGUILayout.FloatField("Durability:", Potion.cooldown);
        // Potion.isStackable = EditorGUILayout.Toggle("Is Stackable", Potion.isStackable);

        // Statistics and Information (example)
        EditorGUILayout.Space();
        GUILayout.Label("Potion Statistics:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Uniqueness Score: {CalculateUniquenessScore(Potion)}%");
        EditorGUILayout.LabelField($"Damage Spectrum: {CalculateDamageSpectrum(Potion)}");

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(Potion);
            AssetDatabase.SaveAssets();
        }
    }

    private float CalculateUniquenessScore(Potion Potion)
    {
        // Implement a method to calculate how unique the Potion is based on its properties
        // ...

        return 0f;
    }

    private string CalculateDamageSpectrum(Potion Potion)
    {
        // Implement a method to determine where the Potion falls on the damage spectrum
        // ...

        return "Medium";
    }

    private List<Potion> sortedPotions;

    private void DeleteSelectedPotion()
    {
        if (selectedItem != null)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(selectedItem));
            selectedItem = null;
            AssetDatabase.Refresh();
        }
    }

    private void DuplicateSelectedPotion()
    {
        if (selectedItem != null)
        {
            string path = AssetDatabase.GetAssetPath(selectedItem);
            AssetDatabase.CopyAsset(path, path.Replace(".asset", "_duplicate.asset"));
            AssetDatabase.Refresh();
        }
    }

    private void ExportPotionsToCSV()
    {
        // Your CSV exporting logic here
    }

    private void ImportPotionsFromCSV()
    {
        // Your CSV exporting logic here
    }
    
    private void InitializeDefaultFilterOptions()
    {
        filterOptions.potionEffectMask = (PotionEffect)Enum.GetValues(typeof(PotionEffect)).Cast<int>().Sum();
        filterOptions.rarityMask = (Rarity)Enum.GetValues(typeof(Rarity)).Cast<int>().Sum();
        // filterOptions.equipSlotMask = (EquipSlot)Enum.GetValues(typeof(EquipSlot)).Cast<int>().Sum();

        // Load all Potions
        string[] guids = AssetDatabase.FindAssets("t:Potion");
        IEnumerable<Potion> Potions = guids.Select(guid =>
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath<Potion>(path);
        });

        // Set max default values based on the Potions in the database
        filterOptions.maxEffectPower = Potions.Max(w => w.effectPower);
        filterOptions.maxDuration = Potions.Max(w => w.duration);
        filterOptions.maxCooldown = Potions.Max(w => w.cooldown);
        filterOptions.maxBaseValue = Potions.Max(w => w.baseValue);
        filterOptions.maxRequiredLevel = Potions.Max(w => w.requiredLevel);
    }

}