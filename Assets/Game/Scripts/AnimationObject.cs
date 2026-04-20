using System;
using UnityEngine;

namespace Game
{
    public sealed class AnimationObject : MonoBehaviour
    {
        public event Action<AnimationObject> OnDestroy; 
            
        [SerializeField] private Animator animator;
        
        public void Play(string animationName, float speed = 1f)
        {
            animator.speed = speed;
            animator.Play(animationName);
        }
        
        public void Play(int animationHash, float speed = 1f)
        {
            animator.speed = speed;
            animator.Play(animationHash);
        }
        
        public void Destroy()
        {
            OnDestroy?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
