using HereticMod.Components;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace HereticMod
{
    //This modifies the Lunar Skills to function properly at 0 stacks of their corresponding item.
    public class ModifyLunarSkillDefs
    {
        private static bool initialized = false;

        public static void Init()
        {
            if (initialized) return;
            initialized = true;
            
            //SKILLSLOT.CHARACTERBODY.INVENTORY NEEDS TO BE NULLCHECKED
            SetupPrimary();
            SetupSecondary();
            SetupUtility();
            SetupSpecial();
        }

        private static void SetupPrimary()
        {
            //Replace the vanilla cooldown with a reload system.
            if (HereticPlugin.visionsAttackSpeed)
            {
                On.EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.OnEnter += (orig, self) =>
                {
                    orig(self);

                    VisionsReloader visRel = self.GetComponent<VisionsReloader>();
                    if (visRel) visRel.FireSkill();
                };
                HereticPlugin.HereticBodyObject.AddComponent<VisionsReloader>();
                RoR2.CharacterBody.onBodyInventoryChangedGlobal += AddVisionsReloader;

                On.RoR2.Skills.LunarPrimaryReplacementSkill.GetRechargeInterval += (orig, self, skillSlot) =>
                {
                    return 0f;
                };

                On.RoR2.Skills.LunarPrimaryReplacementSkill.GetRechargeStock += (orig, self, skillSlot) =>
                {
                    return 0;
                };
            }
            else //At 0 stacks of the item, behave like there is 1 stack.
            {
                On.RoR2.Skills.LunarPrimaryReplacementSkill.GetRechargeInterval += (orig, self, skillSlot) =>
                {
                    float interval = self.baseRechargeInterval;
                    if (skillSlot && skillSlot.characterBody && skillSlot.characterBody.inventory)
                    {
                        interval = Mathf.Max(orig(self, skillSlot), interval);
                    }
                    return interval;
                };
            }

            On.RoR2.Skills.LunarPrimaryReplacementSkill.GetMaxStock += (orig, self, skillSlot) =>
            {
                int maxStock = self.baseMaxStock;
                if (skillSlot && skillSlot.characterBody && skillSlot.characterBody.inventory)
                {
                    maxStock = Math.Max(orig(self, skillSlot), maxStock);
                }
                return maxStock;
            };
        }

        private static void AddVisionsReloader(CharacterBody body)
        {
            if (body.bodyIndex != HereticPlugin.HereticBodyIndex && body.inventory && body.inventory.GetItemCount(RoR2Content.Items.LunarPrimaryReplacement.itemIndex) > 0)
            {
                VisionsReloader visRel = body.GetComponent<VisionsReloader>();
                if (!visRel) body.gameObject.AddComponent<VisionsReloader>();
            }
        }

        private static void SetupSecondary()
        {
            On.RoR2.Skills.LunarSecondaryReplacementSkill.GetRechargeInterval += (orig, self, skillSlot) =>
            {
                float interval = self.baseRechargeInterval;
                if (skillSlot && skillSlot.characterBody && skillSlot.characterBody.inventory)
                {
                    interval = Mathf.Max(orig(self, skillSlot), interval);
                }
                return interval;
            };

            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(MoveType.After,
                         x => x.MatchLdsfld(typeof(RoR2.RoR2Content.Items), "LunarSecondaryReplacement"),
                         x => x.MatchCallvirt<RoR2.Inventory>("GetItemCount")
                         );
                c.EmitDelegate<Func<int, int>>(itemCount =>
                {
                    if (itemCount <= 0)
                    {
                        itemCount = 1;
                    }

                    return itemCount;
                });
            };
        }

        private static void SetupUtility()
        {
            IL.EntityStates.GhostUtilitySkillState.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(MoveType.After,
                         x => x.MatchLdsfld(typeof(RoR2.RoR2Content.Items), "LunarUtilityReplacement"),
                         x => x.MatchCallvirt<RoR2.Inventory>("GetItemCount")
                         );
                c.EmitDelegate<Func<int, int>>(itemCount =>
               {
                   if (itemCount <= 0) itemCount = 1;
                   return itemCount;
               });
            };
        }

        //Spaghetti code
        private static void SetupSpecial()
        {
            On.RoR2.Skills.LunarDetonatorSkill.GetRechargeInterval += (orig, self, skillSlot) =>
            {
                float interval = self.baseRechargeInterval;
                if (skillSlot && skillSlot.characterBody && skillSlot.characterBody.inventory)
                {
                    interval = Mathf.Max(orig(self, skillSlot), interval);
                }
                return interval;
            };

            //Incredibly jank.
            On.RoR2.Skills.LunarDetonatorSkill.OnAssigned += (orig, self, skillSlot) =>
            {
                if (skillSlot && skillSlot.characterBody && skillSlot.characterBody.skillLocator && skillSlot.characterBody.skillLocator.allSkills == null)
                {
                    AssignLunarDetonator ald = skillSlot.characterBody.gameObject.GetComponent<AssignLunarDetonator>();
                    if (!ald) ald = skillSlot.characterBody.gameObject.AddComponent<AssignLunarDetonator>();
                    ald.cb = skillSlot.characterBody;
                    ald.skill = self;
                    ald.skillSlot = skillSlot;
                    return null;
                }
                else
                {
                    return orig(self, skillSlot);
                }
            };

            //Fixes a nullref that sometimes shows up.
            On.RoR2.Skills.LunarDetonatorSkill.OnUnassigned += (orig, self, skillSlot) =>
            {
                if (skillSlot && (RoR2.Skills.LunarDetonatorSkill.InstanceData)skillSlot.skillInstanceData != null)
                {
                    orig(self, skillSlot);
                }
            };

            //On.RoR2.GlobalEventManager.OnHitEnemy += ApplyRuin;
        }

        /*private static void ApplyRuin(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
            if (NetworkServer.active && !damageInfo.rejected && victim && damageInfo.procCoefficient > 0f)
            {
                if (damageInfo.attacker)
                {
                    CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                    if (attackerBody && attackerBody.bodyIndex == HereticPlugin.HereticBodyIndex)
                    {
                        if ((!attackerBody.inventory || attackerBody.inventory.GetItemCount(RoR2Content.Items.LunarSpecialReplacement) <= 0) && attackerBody.skillLocator && attackerBody.skillLocator.special.skillDef == CharacterBody.CommonAssets.lunarSpecialReplacementSkillDef)
                        {
                            CharacterBody victimBody = victim.GetComponent<CharacterBody>();
                            if (victimBody)
                            {
                                if (Util.CheckRoll(100f * damageInfo.procCoefficient, attackerBody.master))
                                {
                                    Debug.Log("Override code");
                                    victimBody.AddTimedBuff(RoR2Content.Buffs.LunarDetonationCharge, 10f);
                                }
                            }
                        }
                    }
                }
            }
        }*/
    }
}
