// using UnityEngine;
// using UnityEditor;
// using System.IO;
//
// public class DatabasesManager : EditorWindow
// {
//     private Vector2 scrollPosition;
//
//     [MenuItem("Window/Item Manager/Databases Manager")]
//     public static void ShowWindow()
//     {
//         GetWindow<DatabasesManager>("Databases Manager");
//     }
//
//     private void OnGUI()
//     {
//         // Title
//         EditorGUILayout.LabelField("Item Database Manager", EditorStyles.boldLabel);
//         
//         // Description
//         EditorGUILayout.HelpBox("Welcome to the Item Database Manager! Here, you can access and manage different databases related to weapons, potions, armors, and other items.", MessageType.Info);
//
//         // Information about overall data
//         GUILayout.Label("Overall Statistics:", EditorStyles.boldLabel);
//         GUILayout.Label($"Total Items: {AssetDatabase.FindAssets("t:BaseItem").Length}");
//         GUILayout.Label($"Total Weapons: {AssetDatabase.FindAssets("t:Weapon").Length}");
//         GUILayout.Label($"Total Potions: {AssetDatabase.FindAssets("t:Potion").Length}");
//         GUILayout.Label($"Total Armors: {AssetDatabase.FindAssets("t:Armor").Length}");
//
//         EditorGUILayout.Space();
//         if (GUILayout.Button("Import Weapons from CSV"))
//         {
//             string path = EditorUtility.OpenFilePanel("Select CSV File", "", "csv");
//             if (path.Length != 0)
//             {
//                 ImportWeaponsFromCSV(path);
//             }
//         }
//         if (GUILayout.Button("Import Potions from CSV"))
//         {
//             string path = EditorUtility.OpenFilePanel("Select CSV File", "", "csv");
//             if (path.Length != 0)
//             {
//                 ImportPotionsFromCSV(path);
//             }
//         }
//
//         if (GUILayout.Button("Import Armor from CSV"))
//         {
//             string path = EditorUtility.OpenFilePanel("Select CSV File", "", "csv");
//             if (path.Length != 0)
//             {
//                 ImportArmorFromCSV(path);
//             }
//         }
//         
//         // Separator
//         EditorGUILayout.Space();
//         EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
//         EditorGUILayout.Space();
//
//         // Buttons to access other windows
//         scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
//
//         if (GUILayout.Button("Weapon Database")) WeaponDatabase.ShowWindow();
//         if (GUILayout.Button("Potion Database")) PotionDatabase.ShowWindow();
//         if (GUILayout.Button("Armor Database")) ArmorDatabase.ShowWindow();
//         // if (GUILayout.Button("Other Items")) OtherItemsDatabase.ShowWindow();
//         // if (GUILayout.Button("Item Creation")) ItemCreation.ShowWindow();
//         if (GUILayout.Button("Statistics Page")) StatisticsPage.ShowWindow();
//
//         EditorGUILayout.EndScrollView();
//     }
//     
//     private void ImportWeaponsFromCSV(string path)
//     {
//         StreamReader reader = new StreamReader(File.OpenRead(path));
//         reader.ReadLine(); // Skip the first line (header)
//
//         while (!reader.EndOfStream)
//         {
//             string line = reader.ReadLine();
//             string[] values = line.Split(',');
//
//             Weapon weapon = ScriptableObject.CreateInstance<Weapon>();
//             weapon.itemName = values[0].Trim('"');
//             weapon.weaponType = (WeaponType)System.Enum.Parse(typeof(WeaponType), values[1]);
//             weapon.attackPower = float.Parse(values[2]);
//             weapon.attackSpeed = float.Parse(values[3]);
//             weapon.durability = float.Parse(values[4]);
//             weapon.range = float.Parse(values[5]);
//             weapon.criticalHitChance = float.Parse(values[6]);
//             weapon.baseValue = float.Parse(values[7]);
//             weapon.rarity = (Rarity)System.Enum.Parse(typeof(Rarity), values[8]);
//             weapon.requiredLevel = int.Parse(values[9]);
//             weapon.equipSlot = (EquipSlot)System.Enum.Parse(typeof(EquipSlot), values[10]);
//             weapon.description = values[11].Trim('"');
//
//             AssetDatabase.CreateAsset(weapon, $"Assets/Items/Weapons/{weapon.itemName}.asset");
//         }
//
//         reader.Close();
//         AssetDatabase.Refresh(); // Refresh the asset database to show new items
//     }
//     private void ImportPotionsFromCSV(string path)
//     {
//         StreamReader reader = new StreamReader(File.OpenRead(path));
//         reader.ReadLine(); // Skip the first line (header)
//
//         while (!reader.EndOfStream)
//         {
//             string line = reader.ReadLine();
//             string[] values = line.Split(',');
//
//             Potion potion = ScriptableObject.CreateInstance<Potion>();
//             potion.itemName = values[0].Trim('"');
//             potion.potionEffect = (PotionEffect)System.Enum.Parse(typeof(PotionEffect), values[1]);
//             potion.effectPower = float.Parse(values[2]);
//             potion.duration = float.Parse(values[3]);
//             potion.cooldown = float.Parse(values[4]);
//             potion.isStackable = bool.Parse(values[5]);
//             potion.baseValue = float.Parse(values[6]);
//             potion.rarity = (Rarity)System.Enum.Parse(typeof(Rarity), values[7]);
//             potion.requiredLevel = int.Parse(values[8]);
//             potion.equipSlot = (EquipSlot)System.Enum.Parse(typeof(EquipSlot), values[9]);
//             potion.description = values[10].Trim('"');
//
//             AssetDatabase.CreateAsset(potion, $"Assets/Items/Potions/{potion.itemName}.asset");
//         }
//
//         reader.Close();
//         AssetDatabase.Refresh(); // Refresh the asset database to show new items
//     }
//     private void ImportArmorFromCSV(string path)
//     {
//         StreamReader reader = new StreamReader(File.OpenRead(path));
//         reader.ReadLine(); // Skip the first line (header)
//
//         while (!reader.EndOfStream)
//         {
//             string line = reader.ReadLine();
//             string[] values = line.Split(',');
//
//             Armor armor = ScriptableObject.CreateInstance<Armor>();
//             armor.itemName = values[0].Trim('"');
//             armor.armorType = (ArmorType)System.Enum.Parse(typeof(ArmorType), values[1]);
//             armor.defensePower = float.Parse(values[2]);
//             armor.resistance = float.Parse(values[3]);
//             armor.weight = float.Parse(values[4]);
//             armor.movementSpeedModifier = float.Parse(values[5]);
//             armor.baseValue = float.Parse(values[6]);
//             armor.rarity = (Rarity)System.Enum.Parse(typeof(Rarity), values[7]);
//             armor.requiredLevel = int.Parse(values[8]);
//             armor.equipSlot = (EquipSlot)System.Enum.Parse(typeof(EquipSlot), values[9]);
//             armor.description = values[10].Trim('"');
//
//             AssetDatabase.CreateAsset(armor, $"Assets/Items/Armors/{armor.itemName}.asset");
//         }
//
//         reader.Close();
//         AssetDatabase.Refresh(); // Refresh the asset database to show new items
//     }
// }