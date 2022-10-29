using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace HereticMod
{
    class SkillSetup
    {
        private static bool initialized = false;

        public static void Init()
        {
            if (initialized) return;
            initialized = true;

            SkillDef squawkSkill = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Heretic/HereticDefaultAbility.asset").WaitForCompletion();//InterruptPriority.Skill

            SkillLocator hereticSkills = HereticPlugin.HereticBodyObject.GetComponent<SkillLocator>();
            hereticSkills.passiveSkill.enabled = true;
            hereticSkills.passiveSkill.skillNameToken = "HERETIC_DEFAULT_SKILL_NAME";
            hereticSkills.passiveSkill.skillDescriptionToken = "MOFFEINHERETIC_PASSIVE_DESCRIPTION";
            hereticSkills.passiveSkill.icon = squawkSkill.icon;

            FixGenericSkills(hereticSkills);

            SkillCatalog.skillsDefined.CallWhenAvailable(delegate
            {
                Addressables.LoadAssetAsync<SkillFamily>("RoR2/Base/Heretic/HereticPrimaryFamily.asset").WaitForCompletion().variants[0].skillDef = SkillCatalog.GetSkillDef(SkillCatalog.FindSkillIndexByName("LunarPrimaryReplacement"));

                SkillDef lunarSecondary = SkillCatalog.GetSkillDef(SkillCatalog.FindSkillIndexByName("LunarSecondaryReplacement"));
                Addressables.LoadAssetAsync<SkillFamily>("RoR2/Base/Heretic/HereticSecondaryFamily.asset").WaitForCompletion().variants[0].skillDef = lunarSecondary;

                Addressables.LoadAssetAsync<SkillFamily>("RoR2/Base/Heretic/HereticUtilityFamily.asset").WaitForCompletion().variants[0].skillDef = SkillCatalog.GetSkillDef(SkillCatalog.FindSkillIndexByName("LunarUtilityReplacement"));

                SkillDef lunarSpecial = SkillCatalog.GetSkillDef(SkillCatalog.FindSkillIndexByName("LunarDetonatorSpecialReplacement"));
                Addressables.LoadAssetAsync<SkillFamily>("RoR2/Base/Heretic/HereticSpecialFamily.asset").WaitForCompletion().variants[0].skillDef = lunarSpecial;

                if (HereticPlugin.fixTypos)
                {
                    lunarSecondary.skillDescriptionToken = "MOFFEINHERETIC_SKILL_LUNAR_SECONDARY_REPLACEMENT_DESCRIPTION";
                    lunarSpecial.skillDescriptionToken = "MOFFEINHERETIC_SKILL_LUNAR_SPECIAL_REPLACEMENT_DESCRIPTION";
                }


                On.EntityStates.Heretic.SpawnState.OnEnter += (orig, self) =>
                {
                    orig(self);
                    if (NetworkServer.active && self.characterBody)
                    {
                        self.characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, EntityStates.Heretic.SpawnState.duration + 0.5f);
                    }
                };
            });
        }

        private static void FixGenericSkills(SkillLocator skillLocator)
        {
            SkillFamily utilityFamily = skillLocator.utility.skillFamily;
            SkillFamily specialFamily = skillLocator.special.skillFamily;

            GenericSkill slot3 = skillLocator.special;
            GenericSkill slot4 = skillLocator.utility;

            skillLocator.utility = slot3;
            skillLocator.utility._skillFamily = utilityFamily;

            skillLocator.special = slot4;
            skillLocator.special._skillFamily = specialFamily;
        }
    }
}
