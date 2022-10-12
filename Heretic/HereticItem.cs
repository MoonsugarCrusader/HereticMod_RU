using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.Networking;

namespace HereticMod
{
    class HereticItem
    {
        public static ItemDef HereticStatBonusItem;

        private static bool initialized = false;
        public static void Init()
        {
            if (initialized) return;
            initialized = true;

            HereticStatBonusItem = ScriptableObject.CreateInstance<ItemDef>();
            HereticStatBonusItem.canRemove = false;
            HereticStatBonusItem.name = "MoffeinHereticStatBonusItem";
            HereticStatBonusItem.deprecatedTier = ItemTier.NoTier;
            HereticStatBonusItem.descriptionToken = "MOFFEINHERETIC_STATBONUSITEM_DESC";
            HereticStatBonusItem.nameToken = "MOFFEINHERETIC_STATBONUSITEM_NAME";
            HereticStatBonusItem.pickupToken = "MOFFEINHERETIC_STATBONUSITEM_PICKUP";
            HereticStatBonusItem.hidden = false;
            HereticStatBonusItem.pickupIconSprite = Assets.assetBundle.LoadAsset<Sprite>("texHeresyItemIcon");
            HereticStatBonusItem.tags = new[]
            {
                ItemTag.WorldUnique,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotSteal,
                ItemTag.CannotDuplicate
            };
            ItemDisplayRule[] idr = new ItemDisplayRule[0];
            ItemAPI.Add(new CustomItem(HereticStatBonusItem, idr));

            RecalculateStatsAPI.GetStatCoefficients += (sender, args) =>
            {
                if (sender.inventory && sender.inventory.GetItemCount(HereticItem.HereticStatBonusItem) > 0)
                {
                    bool shieldOnly = sender.HasBuff(RoR2Content.Buffs.AffixLunar) || sender.inventory.GetItemCount(RoR2Content.Items.ShieldOnly) > 0;
                    if (!shieldOnly)
                    {
                        float adjustedLevel = sender.level - 1f;
                        args.baseRegenAdd -= sender.baseRegen + sender.levelRegen * adjustedLevel;//Negate base regen.
                        args.baseRegenAdd -= (6f + 1.2f * adjustedLevel) * (sender.HasBuff(RoR2Content.Buffs.TonicBuff) ? 0.3333333333f : 1f);//Set negative health regen. Lower the health degeneration while Tonic is active due to the regen multiplier.
                    }

                    args.healthMultAdd += 3f;
                    args.damageMultAdd += 0.5f;
                }
            };

            if (HereticPlugin.giveHereticItem) On.RoR2.CharacterMaster.OnInventoryChanged += GiveHereticItem;
        }

        private static void GiveHereticItem(On.RoR2.CharacterMaster.orig_OnInventoryChanged orig, CharacterMaster self)
        {
            bool isHeretic = false;
            CharacterBody cb = self.GetBody();
            if (cb && cb.bodyIndex == HereticPlugin.HereticBodyIndex) isHeretic = true;

            orig(self);

            if (NetworkServer.active
                && self.inventory
                && self.inventory.GetItemCount(RoR2Content.Items.LunarPrimaryReplacement.itemIndex) > 0
                && self.inventory.GetItemCount(RoR2Content.Items.LunarSecondaryReplacement.itemIndex) > 0
                && self.inventory.GetItemCount(RoR2Content.Items.LunarSpecialReplacement.itemIndex) > 0
                && self.inventory.GetItemCount(RoR2Content.Items.LunarUtilityReplacement.itemIndex) > 0
                && self.inventory.GetItemCount(HereticItem.HereticStatBonusItem) <= 0)
            {
                self.inventory.GiveItem(HereticItem.HereticStatBonusItem);

                if (isHeretic)
                {
                    EffectManager.SimpleEffect(EntityStates.Heretic.SpawnState.effectPrefab, cb.corePosition, Quaternion.identity, true);
                }
            }
        }
    }
}
