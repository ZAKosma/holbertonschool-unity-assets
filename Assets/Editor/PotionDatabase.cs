using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

public class PotionDatabase : ItemDatabase<Potion>
{
    private PotionFilterOptions filterOptions = new PotionFilterOptions();

    private bool showFilterOptions = false; // Variable to toggle the collapsible menu
    
    private List<Potion> sortedPotions;


    [MenuItem("Window/Item Manager/Potion Database")]
    public static void ShowWindow()
    {
        GetWindow<PotionDatabase>("Potion Database");
    }


    protected override IEnumerable<Potion> ApplyFilter(IEnumerable<Potion> Potions)
    {
        // if (filterOptions.potionEffectMask != 0)
        //     Potions = Potions.Where(w => (filterOptions.potionEffectMask & (PotionEffect)(1 << (int)w.potionEffect)) != 0);
        // if (filterOptions.minEffectPower != null)
        //     Potions = Potions.Where(w => w.effectPower >= filterOptions.minEffectPower.Value);
        // if (filterOptions.maxEffectPower != null)
        //     Potions = Potions.Where(w => w.effectPower <= filterOptions.maxEffectPower.Value);
        // if (filterOptions.minDuration != null)
        //     Potions = Potions.Where(w => w.duration >= filterOptions.minDuration.Value);
        // if (filterOptions.maxDuration != null)
        //     Potions = Potions.Where(w => w.duration <= filterOptions.maxDuration.Value);
        // if (filterOptions.minCooldown != null)
        //     Potions = Potions.Where(w => w.cooldown >= filterOptions.minCooldown.Value);
        // if (filterOptions.maxCooldown != null)
        //     Potions = Potions.Where(w => w.cooldown <= filterOptions.maxCooldown.Value);
        // // if (filterOptions.isStackable != null)
        // //     Potions = Potions.Where(w => w.isStackable == filterOptions.isStackable.Value);
        // if (filterOptions.rarityMask != 0)
        //     Potions = Potions.Where(w => (filterOptions.rarityMask & (Rarity)(1 << (int)w.rarity)) != 0);
        // if (filterOptions.minBaseValue != null)
        //     Potions = Potions.Where(w => w.baseValue >= filterOptions.minBaseValue.Value);
        // if (filterOptions.maxBaseValue != null)
        //     Potions = Potions.Where(w => w.baseValue <= filterOptions.maxBaseValue.Value);
        // if (filterOptions.minRequiredLevel != null)
        //     Potions = Potions.Where(w => w.requiredLevel >= filterOptions.minRequiredLevel.Value);
        // if (filterOptions.maxRequiredLevel != null)
        //     Potions = Potions.Where(w => w.requiredLevel <= filterOptions.maxRequiredLevel.Value);
        // // if (filterOptions.equipSlotMask != 0)
        // //     Potions = Potions.Where(w => (filterOptions.equipSlotMask & (EquipSlot)(1 << (int)w.equipSlot)) != 0);
        //
        // // Add more filtering logic as needed

        return Potions;
    }




    protected override void DrawFilterOptions()
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

    protected override void ResetFilterOptions()
    {
        filterOptions = new PotionFilterOptions(); // Assuming FilterOptions has default values that include all objects
        InitializeDefaultFilterOptions(); // Resets the filters to default values    }
    }

    protected override bool IsValid(Potion item, out string issue)
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

    protected override void DrawSelectedItemProperties(Potion item)
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

        // Potion Specific Properties
        EditorGUILayout.Space();
        GUILayout.Label("Potion Properties:", EditorStyles.boldLabel);
        item.potionEffect = (PotionEffect)EditorGUILayout.EnumPopup("Potion Effect:", item.potionEffect);
        item.effectPower = EditorGUILayout.FloatField("Effect Power:", item.effectPower);
        item.duration = EditorGUILayout.FloatField("Duration:", item.duration);
        item.cooldown = EditorGUILayout.FloatField("Cooldown:", item.cooldown);
        item.isStackable = EditorGUILayout.Toggle("Is Stackable:", item.isStackable);

        // Statistics and Information (example)
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

    public override void ExportItemsToCSV(string savePath)
    {
        string[] guids = AssetDatabase.FindAssets("t:Potion");
        IEnumerable<Potion> items = guids.Select(guid =>
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath<Potion>(assetPath);
        });
    
        StringBuilder sb = new StringBuilder();

        // Writing the header
        sb.AppendLine("\"Item Name\",\"Icon Path\",\"Potion Effect\",\"Effect Power\",\"Duration\",\"Cooldown\",\"IsStackable\",\"Base Value\",\"Rarity\",\"Required Level\",\"Description\"");

        foreach (Potion p in items)
        {
            string iconPath = AssetDatabase.GetAssetPath(p.icon);
            sb.AppendLine($"\"{p.itemName}\",\"{iconPath}\",{p.potionEffect},{p.effectPower},{p.duration},{p.cooldown},{p.isStackable},{p.baseValue},{p.rarity},{p.requiredLevel},\"{p.description}\"");
        }

        File.WriteAllText(savePath, sb.ToString());
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

            Potion potion = ScriptableObject.CreateInstance<Potion>();
            potion.itemName = values.Length > 0 ? values[0].Trim('"') : "Unnamed";
            
            // Check for missing fields
            if (values.Length < 11)
            {
                Debug.LogWarning($"Missing fields for item: {potion.itemName} (total count " + values.Length + ".)");
            }
            
            potion.icon = values.Length > 1 ? AssetDatabase.LoadAssetAtPath<Sprite>(values[1].Trim('"')) : null;
            potion.potionEffect = values.Length > 2 ? (PotionEffect)System.Enum.Parse(typeof(PotionEffect), values[2]) : PotionEffect.None;
            potion.effectPower = values.Length > 3 ? float.Parse(values[3]) : 0f;
            potion.duration = values.Length > 4 ? float.Parse(values[4]) : 0f;
            potion.cooldown = values.Length > 5 ? float.Parse(values[5]) : 0f;
            potion.isStackable = values.Length > 6 && bool.Parse(values[6]);
            potion.baseValue = values.Length > 7 ? float.Parse(values[7]) : 0f;
            potion.rarity = values.Length > 8 ? (Rarity)System.Enum.Parse(typeof(Rarity), values[8]) : Rarity.Common;
            potion.requiredLevel = values.Length > 9 ? int.Parse(values[9]) : 0;
            potion.description = values.Length > 10 ? values[10].Trim('"') : "No description";

            potion.equipSlot = EquipSlot.Null;

            existingNames.Add(potion.itemName);
            AssetDatabase.CreateAsset(potion, $"Assets/Items/Potions/{potion.itemName}.asset");
        }

        reader.Close();
        AssetDatabase.Refresh(); // Refresh the asset database to show new items
    }

    protected override void InitializeDefaultFilterOptions()
    {
        filterOptions.potionEffectMask = (PotionEffect)Enum.GetValues(typeof(PotionEffect)).Cast<int>().Sum();
        filterOptions.rarityMask = (Rarity)Enum.GetValues(typeof(Rarity)).Cast<int>().Sum();

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