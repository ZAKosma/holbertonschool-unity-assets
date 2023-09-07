using System;
using System.Linq;

public class WeaponFilterOptions
{
    
    public WeaponType weaponTypeMask = (WeaponType)Enum.GetValues(typeof(WeaponType)).Cast<WeaponType>().Aggregate((a, b) => a | b);
    public Rarity rarityMask = (Rarity)Enum.GetValues(typeof(Rarity)).Cast<Rarity>().Aggregate((a, b) => a | b);
    public EquipSlot equipSlotMask = (EquipSlot)Enum.GetValues(typeof(EquipSlot)).Cast<EquipSlot>().Aggregate((a, b) => a | b);
    public float? minAttackPower = null;
    public float? maxAttackPower = null;
    public float? minAttackSpeed = null;
    public float? maxAttackSpeed = null;
    public float? minDurability = null;
    public float? maxDurability = null;
    public float? minRange = null;
    public float? maxRange = null;
    public float? minCriticalHitChance = null;
    public float? maxCriticalHitChance = null;
    public float? minBaseValue = null;
    public float? maxBaseValue = null;
    public int? minRequiredLevel = null;
    public int? maxRequiredLevel = null;
}

