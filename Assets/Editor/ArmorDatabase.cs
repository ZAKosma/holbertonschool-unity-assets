using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

public class ArmorDatabase : ItemDatabase<Armor>
{
    private ArmorFilterOptions filterOptions = new ArmorFilterOptions();
    private bool showFilterOptions = false; // Variable to toggle the collapsible menu

    public ArmorDatabase()
    {
        this.minSize = new Vector2(600, 400); // Set the minimum size of the window
    }
    
    [MenuItem("Window/Item Manager/Armor Database")]
    public static void ShowWindow()
    {
        GetWindow<ArmorDatabase>("Armor Database");
    }

    protected override bool IsValid(Armor item, out string issue)
    {
        issue = string.Empty;

        // Check for null or empty name
        if (string.IsNullOrEmpty(item.itemName))
        {
            issue = "Name is null or empty.";
            return false;
        }

        // Check for special characters in name
        if (item.itemName.Any(ch => !char.IsLetterOrDigit(ch) && ch != ' ' && ch != '-' && ch != '\''))
        {
            issue = "Name contains special characters.";
            return false;
        }
        
        if (duplicateNames.Contains(item.itemName))
        {
            issue = "Duplicate name.";
            return false;
        }

        // Add any more checks you need here.
        // ...

        return true;
    }

    protected override void DrawSelectedItemProperties(Armor item)
    {
        EditorGUI.BeginChangeCheck();

        // Icon Section - Placed above the name for a more aesthetic look
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace(); // Center the icon
        item.icon = (Sprite)EditorGUILayout.ObjectField(item.icon, typeof(Sprite), false, GUILayout.Height(64),
            GUILayout.Width(64));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        
        string issue;
        bool isValid = IsValid(item, out issue);

        GUIStyle textStyle = new GUIStyle(EditorStyles.largeLabel);
        if (!isValid)
        {
            textStyle.normal.textColor = Color.red;
        }

        // Name Section - Make the name prominent
        EditorGUILayout.LabelField("Name:", EditorStyles.boldLabel);
        item.itemName = EditorGUILayout.TextField(string.Empty, item.itemName, textStyle);
        
        // If invalid, display the issue
        if (!isValid)
        {
            EditorGUILayout.HelpBox(issue, MessageType.Error);
        }
        
        EditorGUILayout.Space(); // Add some space for better separation

        // Other General Properties
        item.description = EditorGUILayout.TextField("Description:", item.description);
        item.baseValue = EditorGUILayout.FloatField("Base Value:", item.baseValue);
        item.rarity = (Rarity)EditorGUILayout.EnumPopup("Rarity:", item.rarity);
        item.requiredLevel = EditorGUILayout.IntField("Required Level:", item.requiredLevel);
        item.equipSlot = (EquipSlot)EditorGUILayout.EnumPopup("Equip Slot:", item.equipSlot);

        // Weapon Specific Properties
        EditorGUILayout.Space();
        GUILayout.Label("Armor Properties:", EditorStyles.boldLabel);
        item.armorType = (ArmorType)EditorGUILayout.EnumPopup("Armor Type:", item.armorType);
        item.defensePower = EditorGUILayout.FloatField("Defense Power:", item.defensePower);
        item.resistance = EditorGUILayout.FloatField("Resistance:", item.resistance);
        item.weight = EditorGUILayout.FloatField("Weight:", item.weight);
        item.movementSpeedModifier = EditorGUILayout.FloatField("Movement Modifier:", item.movementSpeedModifier);

        // // Statistics and Information (example)
        // EditorGUILayout.Space();
        // GUILayout.Label("Weapon Statistics:", EditorStyles.boldLabel);
        // EditorGUILayout.LabelField($"Uniqueness Score: {CalculateUniquenessScore(item)}%");
        // EditorGUILayout.LabelField($"Damage Spectrum: {CalculateDamageSpectrum(item)}");

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(item);
            AssetDatabase.SaveAssets();
        }
    }

    protected override void InitializeDefaultFilterOptions()
    {
        filterOptions.armorTypeMask = (ArmorType)Enum.GetValues(typeof(ArmorType)).Cast<int>().Sum();
        filterOptions.rarityMask = (Rarity)Enum.GetValues(typeof(Rarity)).Cast<int>().Sum();
        filterOptions.equipSlotMask = (EquipSlot)Enum.GetValues(typeof(EquipSlot)).Cast<int>().Sum();

        // Load all weapons
        string[] guids = AssetDatabase.FindAssets("t:Armor");
        IEnumerable<Armor> armor = guids.Select(guid =>
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath<Armor>(path);
        });

        // Set max default values based on the weapons in the database
        filterOptions.maxDefensePower = armor.Max(a => a.defensePower);
        filterOptions.maxResistance = armor.Max(a => a.resistance);
        filterOptions.maxWeight = armor.Max(a => a.weight);
        filterOptions.maxMovementMod = armor.Max(a => a.movementSpeedModifier);
        filterOptions.maxBaseValue = armor.Max(a => a.baseValue);
        filterOptions.maxRequiredLevel = armor.Max(a => a.requiredLevel);
    }

    protected override IEnumerable<Armor> ApplyFilter(IEnumerable<Armor> items)
    {
// Clear any previously seen names and duplicates.
        namesSeen.Clear();
        duplicateNames.Clear();

        // Apply all filters
        var filteredArmor = items;
        
        // // Filtering based on the search string
        // if (!string.IsNullOrEmpty(searchString))
        // {
        //     filteredArmor = filteredArmor.Where(w => w.itemName.Contains(searchString, StringComparison.OrdinalIgnoreCase));
        // }
        // if (filterOptions.armorTypeMask != 0)
        //     filteredArmor = filteredArmor.Where(a => (filterOptions.armorTypeMask & (ArmorType)(1 << (int)a.armorType)) != 0);
        // if (filterOptions.minDefensePower != null)
        //     filteredArmor = filteredArmor.Where(a => a.defensePower >= filterOptions.minDefensePower.Value);
        // if (filterOptions.maxDefensePower != null)
        //     filteredArmor = filteredArmor.Where(a => a.defensePower <= filterOptions.maxDefensePower.Value);
        // if (filterOptions.minResistance != null)
        //     filteredArmor = filteredArmor.Where(a => a.resistance >= filterOptions.minResistance.Value);
        // if (filterOptions.maxResistance != null)
        //     filteredArmor = filteredArmor.Where(a => a.resistance <= filterOptions.maxResistance.Value);
        // if (filterOptions.minWeight != null)
        //     filteredArmor = filteredArmor.Where(a => a.weight >= filterOptions.minWeight.Value);
        // if (filterOptions.maxWeight != null)
        //     filteredArmor = filteredArmor.Where(a => a.weight <= filterOptions.maxWeight.Value);
        // if (filterOptions.minMovementMod != null)
        //     filteredArmor = filteredArmor.Where(a => a.movementSpeedModifier >= filterOptions.minMovementMod.Value);
        // if (filterOptions.maxMovementMod != null)
        //     filteredArmor = filteredArmor.Where(a => a.movementSpeedModifier <= filterOptions.maxMovementMod.Value);
        // if (filterOptions.rarityMask != 0)
        //     filteredArmor = filteredArmor.Where(w => (filterOptions.rarityMask & (Rarity)(1 << (int)w.rarity)) != 0);
        // if (filterOptions.minBaseValue != null)
        //     filteredArmor = filteredArmor.Where(w => w.baseValue >= filterOptions.minBaseValue.Value);
        // if (filterOptions.maxBaseValue != null)
        //     filteredArmor = filteredArmor.Where(w => w.baseValue <= filterOptions.maxBaseValue.Value);
        // if (filterOptions.minRequiredLevel != null)
        //     filteredArmor = filteredArmor.Where(w => w.requiredLevel >= filterOptions.minRequiredLevel.Value);
        // if (filterOptions.maxRequiredLevel != null)
        //     filteredArmor = filteredArmor.Where(w => w.requiredLevel <= filterOptions.maxRequiredLevel.Value);
        // if (filterOptions.equipSlotMask != 0)
        //     filteredArmor = filteredArmor.Where(w => (filterOptions.equipSlotMask & (EquipSlot)(1 << (int)w.equipSlot)) != 0);
        //
        // // Force enumeration to apply all filters
        // //filteredWeapons = filteredWeapons.ToList();
        //
        // // Checking for duplicate names
        // foreach (var armor in filteredArmor.Where(a => !namesSeen.Add(a.itemName)))
        // {
        //     duplicateNames.Add(armor.itemName);
        // }
    
        return filteredArmor;    
    }

    // private void OnGUI()
    // {
    //     // Creation Section
    //     GUILayout.Label("Create New Armor:", EditorStyles.boldLabel);
    //     if (GUILayout.Button("Open Armor Creation Window"))
    //     {
    //         ArmorCreation.ShowWindow();
    //     }
    //
    //     EditorGUILayout.Space();
    //
    //     // Editing Section
    //     scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
    //     string[] guids = AssetDatabase.FindAssets("t:Armor");
    //     foreach (string guid in guids)
    //     {
    //         string path = AssetDatabase.GUIDToAssetPath(guid);
    //         Armor armor = AssetDatabase.LoadAssetAtPath<Armor>(path);
    //         if (GUILayout.Button(armor.itemName))
    //         {
    //             selectedItem = armor;
    //             Selection.activeObject = armor;
    //         }
    //     }
    //     EditorGUILayout.EndScrollView();
    //
    //     // Selected Item Details
    //     if (selectedItem != null)
    //     {
    //         DrawSelectedArmorProperties(selectedItem);
    //     }
    // }

    private void DrawSelectedArmorProperties(Armor armor)
    {
        EditorGUI.BeginChangeCheck();
        armor.itemName = EditorGUILayout.TextField("Name:", armor.itemName);
        armor.icon = (Sprite)EditorGUILayout.ObjectField("Icon:", armor.icon, typeof(Sprite), false);
        armor.description = EditorGUILayout.TextField("Description:", armor.description);
        armor.baseValue = EditorGUILayout.FloatField("Base Value:", armor.baseValue);
        armor.rarity = (Rarity)EditorGUILayout.EnumPopup("Rarity:", armor.rarity);
        armor.requiredLevel = EditorGUILayout.IntField("Required Level:", armor.requiredLevel);
        armor.armorType = (ArmorType)EditorGUILayout.EnumPopup("Armor Type:", armor.armorType);
        armor.defensePower = EditorGUILayout.FloatField("Defense Power:", armor.defensePower);
        armor.resistance = EditorGUILayout.FloatField("Resistance:", armor.resistance);
        armor.weight = EditorGUILayout.FloatField("Weight:", armor.weight);
        armor.movementSpeedModifier = EditorGUILayout.FloatField("Movement Speed Modifier:", armor.movementSpeedModifier);

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(armor);
            AssetDatabase.SaveAssets();
        }
    }

    public override void ImportItemsFromCSV(string path)
    {
        HashSet<string> existingNames = new HashSet<string>();
        StreamReader reader = new StreamReader(File.OpenRead(path));
        reader.ReadLine(); // Skip the first line (header)

        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();
            string[] values = line.Split(',');
            
            // Check for duplicate items
            if (existingNames.Contains(values[0]))
            {
                Debug.LogWarning($"Duplicate item name found: {values[0]}");
                continue;
            }

            Armor armor = ScriptableObject.CreateInstance<Armor>();
            armor.itemName = values.Length > 0 ? values[0].Trim('"') : "Unnamed";
            
            // Check for missing fields
            if (values.Length < 12)
            {
                Debug.LogWarning($"Missing fields for item: {armor.itemName}");
            }
            
            armor.icon = values.Length > 1 ? AssetDatabase.LoadAssetAtPath<Sprite>(values[1].Trim('"')) : null;
            armor.armorType = values.Length > 2 ? (ArmorType)System.Enum.Parse(typeof(ArmorType), values[2]) : ArmorType.None;
            armor.defensePower = values.Length > 3 ? float.Parse(values[3]) : 0f;
            armor.resistance = values.Length > 4 ? float.Parse(values[4]) : 0f;
            armor.weight = values.Length > 5 ? float.Parse(values[5]) : 0f;
            armor.movementSpeedModifier = values.Length > 6 ? float.Parse(values[6]) : 0f;
            armor.baseValue = values.Length > 7 ? float.Parse(values[7]) : 0f;
            armor.rarity = values.Length > 8 ? (Rarity)System.Enum.Parse(typeof(Rarity), values[8]) : Rarity.Common;
            armor.requiredLevel = values.Length > 9 ? int.Parse(values[9]) : 0;
            armor.equipSlot = values.Length > 10 ? (EquipSlot)System.Enum.Parse(typeof(EquipSlot), values[10]) : EquipSlot.Null;
            armor.description = values.Length > 11 ? values[11].Trim('"') : "No description";

            existingNames.Add(armor.itemName);
            AssetDatabase.CreateAsset(armor, $"Assets/Items/Armors/{armor.itemName}.asset");
        }

        reader.Close();
        AssetDatabase.Refresh(); // Refresh the asset database to show new items
    }

    protected override void DrawFilterOptions()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);

        // Toggle collapsible filter options
        showFilterOptions = EditorGUILayout.Foldout(showFilterOptions, "Filter Options:", true, EditorStyles.foldoutHeader);
        if (showFilterOptions)
        {
            if (GUILayout.Button("Reset Filters"))
            {
                ResetFilterOptions();
            }
            
            filterOptions.armorTypeMask = (ArmorType)EditorGUILayout.EnumFlagsField("Armor Type:", filterOptions.armorTypeMask);
            filterOptions.rarityMask = (Rarity)EditorGUILayout.EnumFlagsField("Rarity:", filterOptions.rarityMask);
            filterOptions.equipSlotMask = (EquipSlot)EditorGUILayout.EnumFlagsField("Equip Slot:", filterOptions.equipSlotMask);
        
            EditorGUILayout.BeginHorizontal();
            filterOptions.minDefensePower = EditorGUILayout.FloatField("Min Attack Power:", filterOptions.minDefensePower ?? 0f);
            filterOptions.maxDefensePower = EditorGUILayout.FloatField("Max Attack Power:", filterOptions.maxDefensePower ?? 0f);
            EditorGUILayout.EndHorizontal();

            // Adding the rest of the filter options
            DrawRangeFilter("Resistance", ref filterOptions.minResistance, ref filterOptions.maxResistance);
            DrawRangeFilter("Durability", ref filterOptions.minWeight, ref filterOptions.maxWeight);
            DrawRangeFilter("Range", ref filterOptions.minMovementMod, ref filterOptions.maxMovementMod);
            DrawRangeFilter("Base Value", ref filterOptions.minBaseValue, ref filterOptions.maxBaseValue);
            DrawRangeFilter("Required Level", ref filterOptions.minRequiredLevel, ref filterOptions.maxRequiredLevel);
        }

        EditorGUILayout.EndVertical();
    }
    
    protected override void ResetFilterOptions()
    {
        filterOptions = new ArmorFilterOptions(); // Assuming FilterOptions has default values that include all objects
        InitializeDefaultFilterOptions(); // Resets the filters to default values
    }

    public override void ExportItemsToCSV(string savePath)
    {
        string[] guids = AssetDatabase.FindAssets("t:Armor");
        IEnumerable<Armor> items = guids.Select(guid =>
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath<Armor>(assetPath);
        });
    
        StringBuilder sb = new StringBuilder();

        // Writing the header
        sb.AppendLine("\"Item Name\",\"Icon Path\",\"Armor Type\",\"Defense Power\",\"Resistance\",\"Weight\",\"Movement Speed Modifier\",\"Base Value\",\"Rarity\",\"Required Level\",\"Equip Slot\",\"Description\"");

        foreach (Armor armor in items)
        {
            string iconPath = AssetDatabase.GetAssetPath(armor.icon);
            sb.AppendLine($"\"{armor.itemName}\",\"{iconPath}\",{armor.armorType},{armor.defensePower},{armor.resistance},{armor.weight},{armor.movementSpeedModifier},{armor.baseValue},{armor.rarity},{armor.requiredLevel},{armor.equipSlot},\"{armor.description}\"");
        }

        File.WriteAllText(savePath, sb.ToString());
    }
}
