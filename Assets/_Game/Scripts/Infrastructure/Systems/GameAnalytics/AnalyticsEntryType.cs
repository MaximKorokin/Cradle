namespace Assets._Game.Scripts.Infrastructure.Systems.GameAnalytics
{
    public enum AnalyticsEntryType
    {
        None = 0,

        HealApply = 10,
        DamageApply = 20,
        CriticalDamageApply = 22,
        ExperienceGain = 50,

        Item = 1000,
        ItemUse = 1010,
        ItemEquip = 1020,
        ItemUnequip = 1030,
        ItemAppear = 1040,
        ItemDisappear = 1050,
        ItemEnchantSucces = 1060,
        ItemEnchantFail = 1070,
        ItemCraft = 1080,
        ItemTransfer = 1090,
        ItemDestroy = 1100,
        ItemDrop = 1110,
        ItemBuy = 1120,
        ItemSell = 1130,

        Entity = 2000,
        EntitySpawn = 2010,
        EntityDespawn = 2020,
        EntityDeath = 2030,
        EntityRevive = 2040,

        StatusEffect = 3000,
        StatusEffectStart = 3010,
        StatusEffectEnd = 3020,
        StatusEffectCancel = 3020,
    }
}
