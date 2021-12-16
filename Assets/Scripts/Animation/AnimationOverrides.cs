using System.Collections.Generic;
using UnityEngine;

public class AnimationOverrides : MonoBehaviour
{
    [SerializeField] private GameObject character = null;
    [SerializeField] private AnimationType[] animationTypeArray = null;

    private Dictionary<AnimationClip, AnimationType> animationTypeDictionaryByAnimation;
    private Dictionary<string, AnimationType> animationTypeDictionaryByCompositeKey;

    private void Start()
    {
        animationTypeDictionaryByAnimation = new Dictionary<AnimationClip, AnimationType>();

        foreach (AnimationType item in animationTypeArray)
        {
            animationTypeDictionaryByAnimation.Add(item.animationClip, item);
        }

        animationTypeDictionaryByCompositeKey = new Dictionary<string, AnimationType>();

        foreach (AnimationType item in animationTypeArray)
        {
            string key = item.animationName.ToString() + item.partVariantType.ToString();
            animationTypeDictionaryByCompositeKey.Add(key, item);
        }
    }

    public void ApplyCharacterCustomisationParameters(List<CharacterAttribute> characterAttributesList)
    {
        foreach (CharacterAttribute characterAttribute in characterAttributesList)
        {
            Animator currentAnimator = null;
            List<KeyValuePair<AnimationClip, AnimationClip>> animsKeyValuePairList = new List<KeyValuePair<AnimationClip, AnimationClip>>();

            string animatorAssetName = characterAttribute.characterPart.ToString();

            Animator[] animators = character.GetComponentsInChildren<Animator>();

            foreach (Animator animator in animators)
            {
                if (animator.name == animatorAssetName)
                {
                    currentAnimator = animator;
                    break;
                }
            }

            AnimatorOverrideController aoc = new AnimatorOverrideController(currentAnimator.runtimeAnimatorController);
            List<AnimationClip> animationClips = new List<AnimationClip>(aoc.animationClips);

            foreach (AnimationClip animationClip in animationClips)
            {
                AnimationType animationType;
                bool foundAnimation = animationTypeDictionaryByAnimation.TryGetValue(animationClip, out animationType);

                if (foundAnimation)
                {
                    string key = animationType.animationName.ToString() + characterAttribute.partVariantType.ToString();
                    AnimationType swapAnimationType;
                    bool foundSwapAnimation = animationTypeDictionaryByCompositeKey.TryGetValue(key, out swapAnimationType);

                    if (foundSwapAnimation)
                    {
                        AnimationClip swapAnimationClip = swapAnimationType.animationClip;
                        animsKeyValuePairList.Add(new KeyValuePair<AnimationClip, AnimationClip>(animationClip, swapAnimationClip));
                    }
                }
            }

            aoc.ApplyOverrides(animsKeyValuePairList);
            currentAnimator.runtimeAnimatorController = aoc;
        }
    }
}
