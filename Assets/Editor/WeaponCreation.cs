using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WeaponCreation : BaseItemCreation<Weapon>
{
    // Weapon specific properties
    private WeaponType weaponType;
    private float attackPower;
    private float attackSpeed;
    private float durability;
    private float range;
    private float criticalHitChance;
    private EquipSlot equipSlot;

    [MenuItem("Window/Item Manager/Weapon Creation")]
    public static void ShowWindow()
    {
        GetWindow<WeaponCreation>("Weapon Creation");
    }

    private void OnGUI()
    {
        DrawCommonFields();

        // Weapon specific fields
        GUILayout.Label("Weapon Properties:", EditorStyles.boldLabel);
        weaponType = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type:", weaponType);
        attackPower = EditorGUILayout.FloatField("Attack Power:", attackPower);
        attackSpeed = EditorGUILayout.FloatField("Attack Speed:", attackSpeed);
        durability = EditorGUILayout.FloatField("Durability:", durability);
        range = EditorGUILayout.FloatField("Range:", range);
        criticalHitChance = EditorGUILayout.FloatField("Critical Hit Chance:", criticalHitChance);
        equipSlot = (EquipSlot)EditorGUILayout.EnumPopup("Equip Slot:", equipSlot);

        if (GUILayout.Button("Create Weapon")) CreateWeapon();
    }

    private void CreateWeapon()
    {
        // Gather missing fields
        List<string> missingFields = new List<string>();
        if (string.IsNullOrEmpty(itemName)) missingFields.Add("Name");
        if (icon == null) missingFields.Add("Icon");
        if (string.IsNullOrEmpty(description)) missingFields.Add("Description");
        // Add checks for other fields if required

        string missingFieldsMessage = missingFields.Count > 0 ? 
            "\nMissing Properties: " + string.Join(", ", missingFields) + "\nAre you sure you want to proceed?" 
            : "";

        // Confirmation dialog
        if (EditorUtility.DisplayDialog(
            "Confirm Weapon Creation",
            "Are you sure you want to create this weapon with the following details?\n" +
            "Name: " + itemName + "\n" +
            "Description: " + description + "\n" +
            // Add other properties as desired
            missingFieldsMessage,
            "Yes", "No"))
        {
            Weapon newItem = CreateInstance<Weapon>();
            newItem.itemName = itemName;
            newItem.icon = icon; // Set icon
            newItem.description = description; // Set description
            newItem.baseValue = baseValue;
            newItem.rarity = rarity;
            newItem.requiredLevel = requiredLevel; // Set required level
            newItem.weaponType = weaponType;
            newItem.attackPower = attackPower;
            newItem.attackSpeed = attackSpeed;
            newItem.durability = durability;
            newItem.range = range;
            newItem.criticalHitChance = criticalHitChance;
            newItem.equipSlot = equipSlot;

            string folderPath = "Assets/Items/Weapons/";

            // Create the directory if it doesn't exist
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                System.IO.Directory.CreateDirectory(Application.dataPath + folderPath.Substring("Assets".Length));
                AssetDatabase.Refresh();
            }

            string fullPath = folderPath + itemName + ".asset";
            AssetDatabase.CreateAsset(newItem, fullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}