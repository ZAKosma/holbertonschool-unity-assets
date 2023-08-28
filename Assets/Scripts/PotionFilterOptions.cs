using System;
using System.Linq;

public class PotionFilterOptions
{
    public PotionEffect potionEffectMask = (PotionEffect)Enum.GetValues(typeof(PotionEffect)).Cast<int>().Sum();
    public Rarity rarityMask = (Rarity)Enum.GetValues(typeof(Rarity)).Cast<int>().Sum();
    // public EquipSlot equipSlotMask = (EquipSlot)Enum.GetValues(typeof(EquipSlot)).Cast<int>().Sum();
    public float? minBaseValue = null;
    public float? maxBaseValue = null;
    public int? minRequiredLevel = null;
    public int? maxRequiredLevel = null;
    public float? minEffectPower = null;
    public float? maxEffectPower = null;
    public float? minDuration = null;
    public float? maxDuration = null;
    public float? minCooldown = null;
    public float? maxCooldown = null;
    // public bool? isStackable = null;
}

