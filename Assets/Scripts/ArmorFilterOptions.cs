using System;
using System.Linq;

public class ArmorFilterOptions
{
    
    public ArmorType armorTypeMask = (ArmorType)Enum.GetValues(typeof(ArmorType)).Cast<ArmorType>().Aggregate((a, b) => a | b);
    public Rarity rarityMask = (Rarity)Enum.GetValues(typeof(Rarity)).Cast<Rarity>().Aggregate((a, b) => a | b);
    public EquipSlot equipSlotMask = (EquipSlot)Enum.GetValues(typeof(EquipSlot)).Cast<EquipSlot>().Aggregate((a, b) => a | b);
    public float? minDefensePower = null;
    public float? maxDefensePower = null;
    public float? minResistance = null;
    public float? maxResistance = null;
    public float? minWeight = null;
    public float? maxWeight = null;
    public float? minMovementMod = null;
    public float? maxMovementMod = null;
    public float? minBaseValue = null;
    public float? maxBaseValue = null;
    public int? minRequiredLevel = null;
    public int? maxRequiredLevel = null;
}

