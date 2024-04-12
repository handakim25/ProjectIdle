using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gust.Utility
{
    /// <summary>
    /// MonoSingleton Class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary>
        /// Scene 이동 중에 파괴할 것이라면 이를 상속해서 false를 반환할 것
        /// </summary>
        protected virtual bool _dontDestroyOnLoad => true;
        private static T s_instance = null;
        private static bool s_isAppClosing = false;

        /// <summary>
        /// Instance를 반환한다. App Closing 중이면 null을 반환한다.
        /// 만약에 Instance가 없다면 새로 생성한다.
        /// Instance는 Awake에서 저장하므로 Awake 단계에서는 호출하지 않는다.
        /// </summary>
        public static T Instance
        {
            get
            {
                if(s_isAppClosing)
                {
                    return null;
                }
                if(s_instance == null)
                {
                    if((s_instance = FindObjectOfType<T>()) == null)
                    {
                        Debug.LogWarning($"No instance of {typeof(T).Name} found. Creating new instance.");
                        var go = new GameObject(typeof(T).Name);
                        s_instance = go.AddComponent<T>();
                    }
                }
                return s_instance;
            }
        }

        private void Awake()
        {
            if(s_instance == null)
            {
                s_instance = this as T;
                if(_dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(GetRootObject());
                }
            }
            else
            {
                // Already have instance, Destroy this object
                Destroy(gameObject);
            }
        }

        private GameObject GetRootObject()
        {
            Transform tr = transform;
            while(tr.parent != null)
            {
                tr = tr.parent;
            }
            return tr.gameObject;
        }

        private void OnApplicationQuit()
        {
            // 종료 시에는 instance를 반환하면 안 된다.
            s_instance = null;
            s_isAppClosing = true;
        }
    }
}