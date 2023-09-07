using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using UnityEngine;
using UnityEditor;

public class WeaponDatabase : ItemDatabase<Weapon>
{


    private WeaponFilterOptions filterOptions = new WeaponFilterOptions();

    private bool showFilterOptions = false; // Variable to toggle the collapsible menu
    
    private List<Weapon> sortedWeapons;

    
    public WeaponDatabase()
    {
        this.minSize = new Vector2(600, 400); // Set the minimum size of the window
    }

    
    [MenuItem("Window/Item Manager/Weapon Database")]
    public static void ShowWindow()
    {
        GetWindow<WeaponDatabase>("Weapon Database");
    }


    protected override IEnumerable<Weapon> ApplyFilter(IEnumerable<Weapon> weapons)
    {
        // Clear any previously seen names and duplicates.
        namesSeen.Clear();
        duplicateNames.Clear();

        // Apply all filters
        var filteredWeapons = weapons;
        
        // // Filtering based on the search string
        // if (!string.IsNullOrEmpty(searchString))
        // {
        //     filteredWeapons = filteredWeapons.Where(w => w.itemName.Contains(searchString, StringComparison.OrdinalIgnoreCase));
        // }
        // if (filterOptions.weaponTypeMask != 0)
        //     filteredWeapons = filteredWeapons.Where(w => (filterOptions.weaponTypeMask & (WeaponType)(1 << (int)w.weaponType)) != 0);
        // if (filterOptions.minAttackPower != null)
        //     filteredWeapons = filteredWeapons.Where(w => w.attackPower >= filterOptions.minAttackPower.Value);
        // if (filterOptions.maxAttackPower != null)
        //     filteredWeapons = filteredWeapons.Where(w => w.attackPower <= filterOptions.maxAttackPower.Value);
        // if (filterOptions.minAttackSpeed != null)
        //     filteredWeapons = filteredWeapons.Where(w => w.attackSpeed >= filterOptions.minAttackSpeed.Value);
        // if (filterOptions.maxAttackSpeed != null)
        //     filteredWeapons = filteredWeapons.Where(w => w.attackSpeed <= filterOptions.maxAttackSpeed.Value);
        // if (filterOptions.minDurability != null)
        //     filteredWeapons = filteredWeapons.Where(w => w.durability >= filterOptions.minDurability.Value);
        // if (filterOptions.maxDurability != null)
        //     filteredWeapons = filteredWeapons.Where(w => w.durability <= filterOptions.maxDurability.Value);
        // if (filterOptions.minRange != null)
        //     filteredWeapons = filteredWeapons.Where(w => w.range >= filterOptions.minRange.Value);
        // if (filterOptions.maxRange != null)
        //     filteredWeapons = filteredWeapons.Where(w => w.range <= filterOptions.maxRange.Value);
        // if (filterOptions.minCriticalHitChance != null)
        //     filteredWeapons = filteredWeapons.Where(w => w.criticalHitChance >= filterOptions.minCriticalHitChance.Value);
        // if (filterOptions.maxCriticalHitChance != null)
        //     filteredWeapons = filteredWeapons.Where(w => w.criticalHitChance <= filterOptions.maxCriticalHitChance.Value);
        // if (filterOptions.rarityMask != 0)
        //     filteredWeapons = filteredWeapons.Where(w => (filterOptions.rarityMask & (Rarity)(1 << (int)w.rarity)) != 0);
        // if (filterOptions.minBaseValue != null)
        //     filteredWeapons = filteredWeapons.Where(w => w.baseValue >= filterOptions.minBaseValue.Value);
        // if (filterOptions.maxBaseValue != null)
        //     filteredWeapons = filteredWeapons.Where(w => w.baseValue <= filterOptions.maxBaseValue.Value);
        // if (filterOptions.minRequiredLevel != null)
        //     filteredWeapons = filteredWeapons.Where(w => w.requiredLevel >= filterOptions.minRequiredLevel.Value);
        // if (filterOptions.maxRequiredLevel != null)
        //     filteredWeapons = filteredWeapons.Where(w => w.requiredLevel <= filterOptions.maxRequiredLevel.Value);
        // if (filterOptions.equipSlotMask != 0)
        //     filteredWeapons = filteredWeapons.Where(w => (filterOptions.equipSlotMask & (EquipSlot)(1 << (int)w.equipSlot)) != 0);
        //
        // // Force enumeration to apply all filters
        // //filteredWeapons = filteredWeapons.ToList();
        //
        // // Checking for duplicate names
        // foreach (var weapon in filteredWeapons.Where(weapon => !namesSeen.Add(weapon.itemName)))
        // {
        //     duplicateNames.Add(weapon.itemName);
        // }
    
        return filteredWeapons;
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
            
            filterOptions.weaponTypeMask = (WeaponType)EditorGUILayout.EnumFlagsField("Weapon Type:", filterOptions.weaponTypeMask);
            filterOptions.rarityMask = (Rarity)EditorGUILayout.EnumFlagsField("Rarity:", filterOptions.rarityMask);
            filterOptions.equipSlotMask = (EquipSlot)EditorGUILayout.EnumFlagsField("Equip Slot:", filterOptions.equipSlotMask);
        
            EditorGUILayout.BeginHorizontal();
            filterOptions.minAttackPower = EditorGUILayout.FloatField("Min Attack Power:", filterOptions.minAttackPower ?? 0f);
            filterOptions.maxAttackPower = EditorGUILayout.FloatField("Max Attack Power:", filterOptions.maxAttackPower ?? 0f);
            EditorGUILayout.EndHorizontal();

            // Adding the rest of the filter options
            DrawRangeFilter("Attack Speed", ref filterOptions.minAttackSpeed, ref filterOptions.maxAttackSpeed);
            DrawRangeFilter("Durability", ref filterOptions.minDurability, ref filterOptions.maxDurability);
            DrawRangeFilter("Range", ref filterOptions.minRange, ref filterOptions.maxRange);
            DrawRangeFilter("Critical Hit Chance", ref filterOptions.minCriticalHitChance, ref filterOptions.maxCriticalHitChance);
            DrawRangeFilter("Base Value", ref filterOptions.minBaseValue, ref filterOptions.maxBaseValue);
            DrawRangeFilter("Required Level", ref filterOptions.minRequiredLevel, ref filterOptions.maxRequiredLevel);
        }

        EditorGUILayout.EndVertical();
    }
    
    

    protected override bool IsValid(Weapon weapon, out string issue)
    {
        issue = string.Empty;

        // Check for null or empty name
        if (string.IsNullOrEmpty(weapon.itemName))
        {
            issue = "Name is null or empty.";
            return false;
        }

        // Check for special characters in name
        if (weapon.itemName.Any(ch => !char.IsLetterOrDigit(ch) && ch != ' ' && ch != '-' && ch != '\''))
        {
            issue = "Name contains special characters.";
            return false;
        }
        
        if (duplicateNames.Contains(weapon.itemName))
        {
            issue = "Duplicate name.";
            return false;
        }

        // Add any more checks you need here.
        // ...

        return true;
    }
    

    // private void DrawTopLeftOptions()
    // {
    //     // Database Admin Functions
    //     // GUILayout.Label("Database Admin Functions:", EditorStyles.boldLabel);
    //     // if (GUILayout.Button("Export to CSV")) ExportWeaponsToCSV();
    //     // if (GUILayout.Button("Import from CSV")) ImportWeaponsFromCSV();
    //     if (GUILayout.Button("Delete Selected Weapon")) DeleteSelectedWeapon();
    //     if (GUILayout.Button("Duplicate Selected Weapon")) DuplicateSelectedWeapon();
    //
    //     // Create New Weapon
    //     GUILayout.Label("Create New Weapon:", EditorStyles.boldLabel);
    //     if (GUILayout.Button("Open Weapon Creation Window", GUILayout.Height(30)))
    //     {
    //         WeaponCreation.ShowWindow();
    //     }
    //
    //     // Search
    //     EditorGUILayout.BeginHorizontal();
    //     searchString = EditorGUILayout.TextField("Search:", searchString, GUILayout.Height(20));
    //     if (GUILayout.Button("X", GUILayout.Width(20)))
    //     {
    //         searchString = ""; // Clear search string
    //     }
    //     EditorGUILayout.EndHorizontal();
    //     
    //     // Filter Options
    //     DrawFilterOptions();
    // }

    protected override void ResetFilterOptions()
    {
        filterOptions = new WeaponFilterOptions(); // Assuming FilterOptions has default values that include all objects
        InitializeDefaultFilterOptions(); // Resets the filters to default values
    }


    // protected override void DrawPropertiesSection()
    // {
    //     // Make the "Properties Section:" label bold
    //     GUILayout.Label("Properties Section:", EditorStyles.boldLabel);
    //     if (selectedItem != null)
    //     {
    //         // Increase spacing between properties for better layout
    //         EditorGUIUtility.labelWidth = 140;
    //         EditorGUILayout.Space();
    //         DrawSelectedWeaponProperties(selectedItem);
    //     }
    // }


    protected override void DrawSelectedItemProperties(Weapon weapon)
    {
        EditorGUI.BeginChangeCheck();

        // Icon Section - Placed above the name for a more aesthetic look
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace(); // Center the icon
        weapon.icon = (Sprite)EditorGUILayout.ObjectField(weapon.icon, typeof(Sprite), false, GUILayout.Height(64),
            GUILayout.Width(64));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        
        string issue;
        bool isValid = IsValid(weapon, out issue);

        GUIStyle textStyle = new GUIStyle(EditorStyles.largeLabel);
        if (!isValid)
        {
            textStyle.normal.textColor = Color.red;
        }

        // Name Section - Make the name prominent
        EditorGUILayout.LabelField("Name:", EditorStyles.boldLabel);
        weapon.itemName = EditorGUILayout.TextField(string.Empty, weapon.itemName, textStyle);
        
        // If invalid, display the issue
        if (!isValid)
        {
            EditorGUILayout.HelpBox(issue, MessageType.Error);
        }
        
        EditorGUILayout.Space(); // Add some space for better separation

        // Other General Properties
        weapon.description = EditorGUILayout.TextField("Description:", weapon.description);
        weapon.baseValue = EditorGUILayout.FloatField("Base Value:", weapon.baseValue);
        weapon.rarity = (Rarity)EditorGUILayout.EnumPopup("Rarity:", weapon.rarity);
        weapon.requiredLevel = EditorGUILayout.IntField("Required Level:", weapon.requiredLevel);
        weapon.equipSlot = (EquipSlot)EditorGUILayout.EnumPopup("Equip Slot:", weapon.equipSlot);

        // Weapon Specific Properties
        EditorGUILayout.Space();
        GUILayout.Label("Weapon Properties:", EditorStyles.boldLabel);
        weapon.weaponType = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type:", weapon.weaponType);
        weapon.attackPower = EditorGUILayout.FloatField("Attack Power:", weapon.attackPower);
        weapon.attackSpeed = EditorGUILayout.FloatField("Attack Speed:", weapon.attackSpeed);
        weapon.durability = EditorGUILayout.FloatField("Durability:", weapon.durability);
        weapon.range = EditorGUILayout.FloatField("Range:", weapon.range);
        weapon.criticalHitChance = EditorGUILayout.Slider("Critical Hit Chance:", weapon.criticalHitChance, 0f, 100f);

        // Statistics and Information (example)
        EditorGUILayout.Space();
        GUILayout.Label("Weapon Statistics:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Uniqueness Score: {CalculateUniquenessScore(weapon)}%");
        EditorGUILayout.LabelField($"Damage Spectrum: {CalculateDamageSpectrum(weapon)}");

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(weapon);
            AssetDatabase.SaveAssets();
        }
    }

    private Dictionary<WeaponType, int> CalculateWeaponTypeDistribution(IEnumerable<Weapon> weapons)
    {
        return weapons.GroupBy(w => w.weaponType)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private Weapon CalculateAverageStatsForRarity(IEnumerable<Weapon> weapons, Rarity rarity)
    {
        var filteredWeapons = weapons.Where(w => w.rarity == rarity);
        var count = filteredWeapons.Count();

        if (count == 0) return null; // Handle the case where no weapons match the given rarity.

        return new Weapon()
        {
            itemName = "Average " + rarity.ToString(),
            icon = null, // You may want to set an appropriate icon for this 'average' item.
            description = "An average representation of " + rarity.ToString() + " weapons.",
            baseValue = filteredWeapons.Average(w => w.baseValue),
            rarity = rarity,
            requiredLevel = (int)filteredWeapons.Average(w => w.requiredLevel),
            equipSlot = EquipSlot.Null, // This can be set to null, or you can calculate an appropriate value if desired.
            weaponType = WeaponType.Improvised, // This can be set to null, or you can calculate an appropriate value if desired.
            attackPower = filteredWeapons.Average(w => w.attackPower),
            attackSpeed = filteredWeapons.Average(w => w.attackSpeed),
            durability = filteredWeapons.Average(w => w.durability),
            range = filteredWeapons.Average(w => w.range),
            criticalHitChance = filteredWeapons.Average(w => w.criticalHitChance)
        };
    }

    private KeyValuePair<EquipSlot, int> MostCommonEquipSlot(IEnumerable<Weapon> weapons)
    {
        return weapons.GroupBy(w => w.equipSlot)
            .OrderByDescending(g => g.Count())
            .Select(g => new KeyValuePair<EquipSlot, int>(g.Key, g.Count()))
            .FirstOrDefault();
    }

    private KeyValuePair<WeaponType, int> MostCommonWeaponType(IEnumerable<Weapon> weapons)
    {
        return weapons.GroupBy(w => w.weaponType)
            .OrderByDescending(g => g.Count())
            .Select(g => new KeyValuePair<WeaponType, int>(g.Key, g.Count()))
            .FirstOrDefault();
    }

    
    
    private float CalculateWeaponEfficiency(Weapon weapon)
    {
        return weapon.baseValue != 0 ? weapon.attackPower / weapon.baseValue : 0;
    }


    private float CalculateUniquenessScore(Weapon weapon)
    {
        // We'll define factors with associated weights and add them to a score

        // Rarity weight
        float rarityWeight = (int)weapon.rarity;

        // Other factors with arbitrary weights
        float attackPowerWeight = weapon.attackPower * 0.2f;
        float attackSpeedWeight = weapon.attackSpeed * 0.15f;
        float durabilityWeight = weapon.durability * 0.1f;
        float rangeWeight = weapon.range * 0.05f;
        float criticalHitChanceWeight = weapon.criticalHitChance * 0.3f;

        // Sum up all the weighted factors
        float uniquenessScore = rarityWeight + attackPowerWeight + attackSpeedWeight +
                                durabilityWeight + rangeWeight + criticalHitChanceWeight;

        return uniquenessScore;
    }

    private string CalculateDamageSpectrum(Weapon weapon)
    {
        // We'll classify the weapon into "Low", "Medium", "High" damage categories
        // based on attack power. The thresholds can be adjusted.

        if (weapon.attackPower < 50)
        {
            return "Low";
        }
        else if (weapon.attackPower >= 50 && weapon.attackPower < 150)
        {
            return "Medium";
        }
        else
        {
            return "High";
        }
    }



    private void DeleteSelectedWeapon()
    {
        if (selectedItem != null)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(selectedItem));
            selectedItem = null;
            AssetDatabase.Refresh();
        }
    }

    private void DuplicateSelectedWeapon()
    {
        if (selectedItem != null)
        {
            string path = AssetDatabase.GetAssetPath(selectedItem);
            AssetDatabase.CopyAsset(path, path.Replace(".asset", "_duplicate.asset"));
            AssetDatabase.Refresh();
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

            Weapon weapon = ScriptableObject.CreateInstance<Weapon>();
            weapon.itemName = values.Length > 0 ? values[0].Trim('"') : "Unnamed";
            
            // Check for missing fields
            if (values.Length < 13)
            {
                Debug.LogWarning($"Missing fields for item: {weapon.itemName}");
            }
            
            weapon.icon = values.Length > 1 ? AssetDatabase.LoadAssetAtPath<Sprite>(values[1].Trim('"')) : null;
            weapon.weaponType = values.Length > 2 ? (WeaponType)System.Enum.Parse(typeof(WeaponType), values[2]) : WeaponType.None;
            weapon.attackPower = values.Length > 3 ? float.Parse(values[3]) : 0f;
            weapon.attackSpeed = values.Length > 4 ? float.Parse(values[4]) : 0f;
            weapon.durability = values.Length > 5 ? float.Parse(values[5]) : 0f;
            weapon.range = values.Length > 6 ? float.Parse(values[6]) : 0f;
            weapon.criticalHitChance = values.Length > 7 ? float.Parse(values[7]) : 0f;
            weapon.baseValue = values.Length > 8 ? float.Parse(values[8]) : 0f;
            weapon.rarity = values.Length > 9 ? (Rarity)System.Enum.Parse(typeof(Rarity), values[9]) : Rarity.Common;
            weapon.requiredLevel = values.Length > 10 ? int.Parse(values[10]) : 0;
            weapon.equipSlot = values.Length > 11 ? (EquipSlot)System.Enum.Parse(typeof(EquipSlot), values[11]) : EquipSlot.Null;
            weapon.description = values.Length > 12 ? values[12].Trim('"') : "No description";

            existingNames.Add(weapon.itemName);
            AssetDatabase.CreateAsset(weapon, $"Assets/Items/Weapons/{weapon.itemName}.asset");
        }

        reader.Close();
        AssetDatabase.Refresh(); // Refresh the asset database to show new items
    }

    public override void ExportItemsToCSV(string savePath)
    {
        string[] guids = AssetDatabase.FindAssets("t:Weapon");
        IEnumerable<Weapon> items = guids.Select(guid =>
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath<Weapon>(assetPath);
        });
    
        StringBuilder sb = new StringBuilder();

        // Writing the header
        sb.AppendLine("\"Item Name\",\"Icon Path\",\"Weapon Type\",\"Attack Power\",\"Attack Speed\",\"Durability\",\"Range\",\"Critical Hit Chance\",\"Base Value\",\"Rarity\",\"Required Level\",\"Equip Slot\",\"Description\"");

        foreach (Weapon weapon in items)
        {
            string iconPath = AssetDatabase.GetAssetPath(weapon.icon);
            sb.AppendLine($"\"{weapon.itemName}\",\"{iconPath}\",{weapon.weaponType},{weapon.attackPower},{weapon.attackSpeed},{weapon.durability},{weapon.range},{weapon.criticalHitChance},{weapon.baseValue},{weapon.rarity},{weapon.requiredLevel},{weapon.equipSlot},\"{weapon.description}\"");
        }

        File.WriteAllText(savePath, sb.ToString());
    }
    
    protected override void InitializeDefaultFilterOptions()
    {
        filterOptions.weaponTypeMask = (WeaponType)Enum.GetValues(typeof(WeaponType)).Cast<int>().Sum();
        filterOptions.rarityMask = (Rarity)Enum.GetValues(typeof(Rarity)).Cast<int>().Sum();
        filterOptions.equipSlotMask = (EquipSlot)Enum.GetValues(typeof(EquipSlot)).Cast<int>().Sum();

        // Load all weapons
        string[] guids = AssetDatabase.FindAssets("t:Weapon");
        IEnumerable<Weapon> weapons = guids.Select(guid =>
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath<Weapon>(path);
        });

        // Set max default values based on the weapons in the database
        filterOptions.maxAttackPower = weapons.Max(w => w.attackPower);
        filterOptions.maxAttackSpeed = weapons.Max(w => w.attackSpeed);
        filterOptions.maxDurability = weapons.Max(w => w.durability);
        filterOptions.maxRange = weapons.Max(w => w.range);
        filterOptions.maxCriticalHitChance = weapons.Max(w => w.criticalHitChance);
        filterOptions.maxBaseValue = weapons.Max(w => w.baseValue);
        filterOptions.maxRequiredLevel = weapons.Max(w => w.requiredLevel);
    }

}