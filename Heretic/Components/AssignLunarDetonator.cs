using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace HereticMod.Components
{
    public class AssignLunarDetonator : MonoBehaviour
    {
        public GenericSkill skillSlot;
        public LunarDetonatorSkill skill;
        public CharacterBody cb;

        public void FixedUpdate()
        {
            if (skillSlot.skillInstanceData == null)
            {
                if (cb && cb.skillLocator && cb.skillLocator.allSkills != null)
                {
                    skill.OnAssigned(skillSlot);
                    Destroy(this);
                    return;
                }
            }
            else
            {
                Destroy(this);
                return;
            }
        }
    }
}
