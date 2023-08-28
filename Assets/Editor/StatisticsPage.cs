using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class StatisticsPage : EditorWindow
{
    private Vector2 scrollPosition;
    
    private bool totalCountFoldout;
    private bool averagesFoldout;
    private bool averagePerRarityFoldout;
    private bool mostCommonFoldout;


    [MenuItem("Window/Item Manager/Statistics Page")]
    public static void ShowWindow()
    {
        GetWindow<StatisticsPage>("Statistics Page");
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        DrawSection("General Item Statistics", DrawGeneralItemStatistics);
        EditorGUILayout.Separator();
        DrawSection("Weapon Statistics", DrawWeaponStatistics, new Color(0.8f, 0, 0));
        EditorGUILayout.Separator();
        DrawSection("Armor Statistics", DrawArmorStatistics, new Color(0, 0, 0.8f));
        EditorGUILayout.Separator();
        DrawSection("Potion Statistics", DrawPotionStatistics, new Color(0, 0.8f, 0));

        EditorGUILayout.EndScrollView();
    }
    
    private void DrawSection(string title, Action drawMethod, Color? sectionColor = null)
    {
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            normal = { textColor = sectionColor ?? Color.black }
        };

        GUILayout.Label(title, headerStyle);
        EditorGUILayout.BeginVertical("Box");
        if (sectionColor != null) GUI.backgroundColor = sectionColor.Value;
        drawMethod();
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
        GUI.backgroundColor = Color.white; // Reset color
    }

    
    private void DrawGeneralItemStatistics()
    {
        DrawLabel("Total Items", FindAssetsByType<BaseItem>().Count);
        DrawLabel("Total Weapons", FindAssetsByType<Weapon>().Count);
        DrawLabel("Total Armors", FindAssetsByType<Armor>().Count);
        DrawLabel("Total Potions", FindAssetsByType<Potion>().Count);
    }
    private void DrawLabel(string label, int value)
    {
        GUILayout.Label($"{label}: {value}");
    }
    
    private void DrawAverageStatsTable(Dictionary<Rarity, Weapon> averageStats)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Rarity", GUILayout.Width(100));
        GUILayout.Label("AP", GUILayout.Width(50));
        GUILayout.Label("Durability", GUILayout.Width(80));
        GUILayout.Label("Range", GUILayout.Width(50));
        GUILayout.Label("Critical Chance", GUILayout.Width(120));
        EditorGUILayout.EndHorizontal();

        foreach (var entry in averageStats)
        {
            var rarity = entry.Key;
            var weapon = entry.Value;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(rarity.ToString(), GUILayout.Width(100));
            GUILayout.Label(weapon.attackPower.ToString(), GUILayout.Width(50));
            GUILayout.Label(weapon.durability.ToString(), GUILayout.Width(80));
            GUILayout.Label(weapon.range.ToString(), GUILayout.Width(50));
            GUILayout.Label(weapon.criticalHitChance.ToString() + "%", GUILayout.Width(120));
            EditorGUILayout.EndHorizontal();
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

    private void DrawWeaponStatistics()
{
    var weapons = FindAssetsByType<Weapon>();
    Dictionary<WeaponType, int> weaponTypeCount = CalculateWeaponTypeDistribution(weapons);
    float totalAttackPower = 0, totalDurability = 0, totalRange = 0, totalCriticalHitChance = 0;

    foreach (var weapon in weapons)
    {
        totalAttackPower += weapon.attackPower;
        totalDurability += weapon.durability;
        totalRange += weapon.range;
        totalCriticalHitChance += weapon.criticalHitChance;
    }
    
    totalCountFoldout = EditorGUILayout.Foldout(totalCountFoldout, "Total Count");
    if (totalCountFoldout)
    {
        foreach (var type in weaponTypeCount)
        {
            GUILayout.Label($"Total {type.Key}: {type.Value}");
        }
    }
    
    // Averages foldout section
    averagesFoldout = EditorGUILayout.Foldout(averagesFoldout, "Averages");
    if (averagesFoldout)
    {
        GUILayout.Label($"Average Attack Power: {totalAttackPower / weapons.Count}");
        GUILayout.Label($"Average Durability: {totalDurability / weapons.Count}");
        GUILayout.Label($"Average Range: {totalRange / weapons.Count}");
        GUILayout.Label($"Average Critical Hit Chance: {totalCriticalHitChance / weapons.Count}");
    }
    
    // Collect average stats for each rarity
    Dictionary<Rarity, Weapon> averageStats = new Dictionary<Rarity, Weapon>();
    foreach (Rarity rarity in Enum.GetValues(typeof(Rarity)))
    {
        var avgWeapon = CalculateAverageStatsForRarity(weapons, rarity);
        if (avgWeapon != null)
        {
            averageStats[rarity] = avgWeapon;
        }
    }
    // Average per rarity foldout section
    averagePerRarityFoldout = EditorGUILayout.Foldout(averagePerRarityFoldout, "Average Per Rarity");
    if (averagePerRarityFoldout)
    {
        // Draw the table
        DrawAverageStatsTable(averageStats);
    }
    
    
    mostCommonFoldout = EditorGUILayout.Foldout(mostCommonFoldout, "Most Common");
    if (mostCommonFoldout)
    {
        var mostCommonEquipSlot = MostCommonEquipSlot(weapons);
        GUILayout.Label($"Most Common Equip Slot: {mostCommonEquipSlot.Key} - {mostCommonEquipSlot.Value} instances");

        var mostCommonWeaponType = MostCommonWeaponType(weapons);
        GUILayout.Label(
            $"Most Common Weapon Type: {mostCommonWeaponType.Key} - {mostCommonWeaponType.Value} instances");
    }
}

private void DrawArmorStatistics()
{
    var armors = FindAssetsByType<Armor>();
    Dictionary<ArmorType, int> armorTypeCount = new Dictionary<ArmorType, int>();
    float totalDefensePower = 0, totalResistance = 0, totalWeight = 0;

    foreach (var armor in armors)
    {
        if (!armorTypeCount.ContainsKey(armor.armorType))
            armorTypeCount[armor.armorType] = 0;
        armorTypeCount[armor.armorType]++;
        totalDefensePower += armor.defensePower;
        totalResistance += armor.resistance;
        totalWeight += armor.weight;
    }

    foreach (var type in armorTypeCount)
    {
        GUILayout.Label($"Total {type.Key}: {type.Value}");
    }

    GUILayout.Label($"Average Defense Power: {totalDefensePower / armors.Count}");
    GUILayout.Label($"Average Resistance: {totalResistance / armors.Count}");
    GUILayout.Label($"Average Weight: {totalWeight / armors.Count}");
}

private void DrawPotionStatistics()
{
    var potions = FindAssetsByType<Potion>();
    Dictionary<PotionEffect, int> potionEffectCount = new Dictionary<PotionEffect, int>();
    float totalEffectPower = 0, totalDuration = 0, totalCooldown = 0;

    foreach (var potion in potions)
    {
        if (!potionEffectCount.ContainsKey(potion.potionEffect))
            potionEffectCount[potion.potionEffect] = 0;
        potionEffectCount[potion.potionEffect]++;
        totalEffectPower += potion.effectPower;
        totalDuration += potion.duration;
        totalCooldown += potion.cooldown;
    }

    foreach (var effect in potionEffectCount)
    {
        GUILayout.Label($"Total {effect.Key}: {effect.Value}");
    }

    GUILayout.Label($"Average Effect Power: {totalEffectPower / potions.Count}");
    GUILayout.Label($"Average Duration: {totalDuration / potions.Count}");
    GUILayout.Label($"Average Cooldown: {totalCooldown / potions.Count}");
}


private List<T> FindAssetsByType<T>() where T : UnityEngine.Object
{
    List<T> assets = new List<T>();
    string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
    foreach (string guid in guids)
    {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        T asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset != null)
        {
            assets.Add(asset);
        }
    }
    return assets;
}
}
