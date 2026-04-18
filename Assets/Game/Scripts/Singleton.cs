using UnityEngine;

namespace Game
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        public static T Instance;

        protected virtual void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"Dublicate singleton instance of {this.GetType().FullName}");
                Destroy(gameObject);
            }
            else
            {
                Instance = this as T;
            }
        }
    }
}
