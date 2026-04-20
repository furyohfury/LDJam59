using System;
using UnityEngine;

namespace Game
{
    public sealed class AnimationObject : MonoBehaviour
    {
        public event Action<AnimationObject> OnDestroy;

        public void Awake()
        {
            if (TryGetComponent(out AudioSource audioSource))
            {
                audioSource.volume = VFXManager.Instance.Volume;
            }
        }

        public void Destroy()
        {
            OnDestroy?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
