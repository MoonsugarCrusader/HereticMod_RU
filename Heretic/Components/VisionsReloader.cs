using UnityEngine;
using RoR2;
using RoR2.Skills;

namespace HereticMod.Components
{
    public class VisionsReloader : MonoBehaviour
    {
        public static float graceDuration = 0.4f;    //Used when there's still stocks in the mag
        public static float baseDuration = 2f;

        private CharacterBody body;
        private SkillLocator skills;

        private float reloadStopwatch;
        private float delayStopwatch;

        private void Awake()
        {
            body = base.GetComponent<CharacterBody>();
            skills = base.GetComponent<SkillLocator>();

            reloadStopwatch = 0f;
            delayStopwatch = 0f;
        }

        private void FixedUpdate()
        {
            //Destroy itself when the player no longer has Visions
            if (body.bodyIndex != HereticPlugin.HereticBodyIndex && body.inventory && body.inventory.GetItemCount(RoR2Content.Items.LunarPrimaryReplacement) <= 0)
            {
                Destroy(this);
                return;
            }

            if (!skills.hasAuthority) return;

            if (skills.primary.stock < skills.primary.maxStock && skills.primary.skillDef == CharacterBody.CommonAssets.lunarPrimaryReplacementSkillDef)
            {
                if (skills.primary.stock <= 0) delayStopwatch = 0f;
                if (delayStopwatch > 0f)
                {
                    delayStopwatch -= Time.fixedDeltaTime;
                }
                else
                {
                    reloadStopwatch -= Time.fixedDeltaTime;
                    if (reloadStopwatch <= 0f)
                    {
                        skills.primary.stock = skills.primary.maxStock;
                    }
                }
            }
            else
            {
                reloadStopwatch = GetReloadStopwatch();
            }
        }

        private float GetStackMult()
        {
            float stackMult = 1f;
            if (body.inventory)
            {
                stackMult = Mathf.Max(1f, body.inventory.GetItemCount(RoR2Content.Items.LunarPrimaryReplacement));
            }
            return stackMult;
        }

        private float GetReloadStopwatch()
        {
            return (baseDuration / body.attackSpeed) * GetStackMult();
        }

        public void FireSkill()
        {
            delayStopwatch = graceDuration;  //Duration is already scaled to attack speed. InitialDelay is simply for inputs, and is ignored if the mag is empty.
            reloadStopwatch = GetReloadStopwatch();
        }
    }
}