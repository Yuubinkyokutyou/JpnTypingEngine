#region

using UnityEngine;

#endregion

namespace JpnTypingEngine
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        protected virtual bool DestroyTargetGameObject => false;

        public static T I { get; private set; }

        private void Awake()
        {
            if (I == null)
            {
                I = this as T;
                I.Init();
                return;
            }

            if (DestroyTargetGameObject)
                Destroy(gameObject);
            else
                Destroy(this);
        }

        private void OnDestroy()
        {
            if (I == this) I = null;
            OnRelease();
        }

        /// <summary>
        ///     Singletonが有効か
        /// </summary>
        public static bool IsValid()
        {
            return I != null;
        }

        /// <summary>
        ///     派生クラス用のAwake
        /// </summary>
        protected virtual void Init()
        {
        }

        /// <summary>
        ///     派生クラス用のOnDestroy
        /// </summary>
        protected virtual void OnRelease()
        {
        }
    }
}