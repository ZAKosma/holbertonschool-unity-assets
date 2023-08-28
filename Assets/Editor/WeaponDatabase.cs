using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class WeaponDatabase : ItemDatabase<Weapon>
{

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
        // Filtering based on the search string
        if (!string.IsNullOrEmpty(searchString))
        {
            weapons = weapons.Where(w => w.itemName.Contains(searchString, StringComparison.OrdinalIgnoreCase));
        }
        if (filterOptions.weaponTypeMask != 0)
            weapons = weapons.Where(w => (filterOptions.weaponTypeMask & (WeaponType)(1 << (int)w.weaponType)) != 0);
        if (filterOptions.minAttackPower != null)
            weapons = weapons.Where(w => w.attackPower >= filterOptions.minAttackPower.Value);
        if (filterOptions.maxAttackPower != null)
            weapons = weapons.Where(w => w.attackPower <= filterOptions.maxAttackPower.Value);
        if (filterOptions.minAttackSpeed != null)
            weapons = weapons.Where(w => w.attackSpeed >= filterOptions.minAttackSpeed.Value);
        if (filterOptions.maxAttackSpeed != null)
            weapons = weapons.Where(w => w.attackSpeed <= filterOptions.maxAttackSpeed.Value);
        if (filterOptions.minDurability != null)
            weapons = weapons.Where(w => w.durability >= filterOptions.minDurability.Value);
        if (filterOptions.maxDurability != null)
            weapons = weapons.Where(w => w.durability <= filterOptions.maxDurability.Value);
        if (filterOptions.minRange != null)
            weapons = weapons.Where(w => w.range >= filterOptions.minRange.Value);
        if (filterOptions.maxRange != null)
            weapons = weapons.Where(w => w.range <= filterOptions.maxRange.Value);
        if (filterOptions.minCriticalHitChance != null)
            weapons = weapons.Where(w => w.criticalHitChance >= filterOptions.minCriticalHitChance.Value);
        if (filterOptions.maxCriticalHitChance != null)
            weapons = weapons.Where(w => w.criticalHitChance <= filterOptions.maxCriticalHitChance.Value);
        if (filterOptions.rarityMask != 0)
            weapons = weapons.Where(w => (filterOptions.rarityMask & (Rarity)(1 << (int)w.rarity)) != 0);
        if (filterOptions.minBaseValue != null)
            weapons = weapons.Where(w => w.baseValue >= filterOptions.minBaseValue.Value);
        if (filterOptions.maxBaseValue != null)
            weapons = weapons.Where(w => w.baseValue <= filterOptions.maxBaseValue.Value);
        if (filterOptions.minRequiredLevel != null)
            weapons = weapons.Where(w => w.requiredLevel >= filterOptions.minRequiredLevel.Value);
        if (filterOptions.maxRequiredLevel != null)
            weapons = weapons.Where(w => w.requiredLevel <= filterOptions.maxRequiredLevel.Value);
        if (filterOptions.equipSlotMask != 0)
            weapons = weapons.Where(w => (filterOptions.equipSlotMask & (EquipSlot)(1 << (int)w.equipSlot)) != 0);

        // Add more filtering logic as needed

        return weapons;
    }


    private WeaponFilterOptions filterOptions = new WeaponFilterOptions();

    private bool showFilterOptions = false; // Variable to toggle the collapsible menu

    private void DrawFilterOptions()
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


    protected override void ExportItemsToCSV()
    {
        throw new NotImplementedException();
    }

    protected override void ImportItemsFromCSV()
    {
        throw new NotImplementedException();
    }

    private void DrawTopLeftOptions()
    {
        // Database Admin Functions
        GUILayout.Label("Database Admin Functions:", EditorStyles.boldLabel);
        if (GUILayout.Button("Export to CSV")) ExportWeaponsToCSV();
        if (GUILayout.Button("Import from CSV")) ImportWeaponsFromCSV();
        if (GUILayout.Button("Delete Selected Weapon")) DeleteSelectedWeapon();
        if (GUILayout.Button("Duplicate Selected Weapon")) DuplicateSelectedWeapon();

        // Create New Weapon
        GUILayout.Label("Create New Weapon:", EditorStyles.boldLabel);
        if (GUILayout.Button("Open Weapon Creation Window", GUILayout.Height(30)))
        {
            WeaponCreation.ShowWindow();
        }

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

    private void ResetFilterOptions()
    {
        filterOptions = new WeaponFilterOptions(); // Assuming FilterOptions has default values that include all objects
        InitializeDefaultFilterOptions(); // Resets the filters to default values
    }


    protected override void DrawPropertiesSection()
    {
        // Make the "Properties Section:" label bold
        GUILayout.Label("Properties Section:", EditorStyles.boldLabel);
        if (selectedItem != null)
        {
            // Increase spacing between properties for better layout
            EditorGUIUtility.labelWidth = 140;
            EditorGUILayout.Space();
            DrawSelectedWeaponProperties(selectedItem);
        }
    }


    private void DrawSelectedWeaponProperties(Weapon weapon)
    {
        EditorGUI.BeginChangeCheck();

        // Icon Section - Placed above the name for a more aesthetic look
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace(); // Center the icon
        weapon.icon = (Sprite)EditorGUILayout.ObjectField(weapon.icon, typeof(Sprite), false, GUILayout.Height(64),
            GUILayout.Width(64));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        // Name Section - Make the name prominent
        EditorGUILayout.LabelField("Name:", EditorStyles.boldLabel);
        weapon.itemName = EditorGUILayout.TextField(string.Empty, weapon.itemName, EditorStyles.largeLabel);
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


    private List<Weapon> sortedWeapons;

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

    private void ExportWeaponsToCSV()
    {
        // Your CSV exporting logic here
    }

    private void ImportWeaponsFromCSV()
    {
        // Your CSV exporting logic here
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