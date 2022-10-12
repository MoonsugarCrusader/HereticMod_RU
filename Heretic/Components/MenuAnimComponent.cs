using UnityEngine;
using RoR2;
using System.Collections;
using EntityStates.Heretic;

namespace HereticMod.Components
{
    public class MenuAnimComponent : MonoBehaviour
    {
        /*internal void OnEnable()
        {
            if (base.gameObject && base.transform.parent && base.gameObject.transform.parent.gameObject && base.gameObject.transform.parent.gameObject.name == "CharacterPad")
            {
                base.StartCoroutine(this.SelectAnim());
            }
        }*/

        public void Play()
        {
            Animator animator = base.gameObject.GetComponent<Animator>();
            EffectManager.SimpleEffect(SpawnState.effectPrefab, base.gameObject.transform.position, Quaternion.identity, false);
            PlayAnimation("Body", "Spawn", "Spawn.playbackRate", SpawnState.duration, animator);
        }

        //No timed events, no need for ienumerator
        /*private IEnumerator SelectAnim()
        {
            Animator animator = base.gameObject.GetComponent<Animator>();
            EffectManager.SimpleEffect(SpawnState.effectPrefab, base.gameObject.transform.position, Quaternion.identity, false);
            PlayAnimation("Body", "Spawn", "Spawn.playbackRate", SpawnState.duration, animator);
            yield break;
        }*/

        private void PlayAnimation(string layerName, string animationStateName, string playbackRateParam, float duration, Animator animator)
        {
            int layerIndex = animator.GetLayerIndex(layerName);
            animator.SetFloat(playbackRateParam, 1f);
            animator.PlayInFixedTime(animationStateName, layerIndex, 0f);
            animator.Update(0f);
            float length = animator.GetCurrentAnimatorStateInfo(layerIndex).length;
            animator.SetFloat(playbackRateParam, length / duration);
        }
    }
}
