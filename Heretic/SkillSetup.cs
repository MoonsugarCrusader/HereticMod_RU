using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
                Addressables.LoadAssetAsync<SkillFamily>("RoR2/Base/Heretic/HereticSecondaryFamily.asset").WaitForCompletion().variants[0].skillDef = SkillCatalog.GetSkillDef(SkillCatalog.FindSkillIndexByName("LunarSecondaryReplacement"));
                Addressables.LoadAssetAsync<SkillFamily>("RoR2/Base/Heretic/HereticUtilityFamily.asset").WaitForCompletion().variants[0].skillDef = SkillCatalog.GetSkillDef(SkillCatalog.FindSkillIndexByName("LunarUtilityReplacement"));
                Addressables.LoadAssetAsync<SkillFamily>("RoR2/Base/Heretic/HereticSpecialFamily.asset").WaitForCompletion().variants[0].skillDef = SkillCatalog.GetSkillDef(SkillCatalog.FindSkillIndexByName("LunarDetonatorSpecialReplacement"));
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
