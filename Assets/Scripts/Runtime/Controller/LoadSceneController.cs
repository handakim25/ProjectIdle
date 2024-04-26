using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;

namespace Gust
{
    /// <summary>
    /// Load Scene에서는 Manager들을 초기화하고 Play Scene으로 넘어간다.
    /// 만약에 해당 Scene으로 되돌아 올 경우 Manager들의 Reference가 유지 되지 않는 것에 유의한다.
    /// </summary>
    public class LoadSceneController : MonoBehaviour
    {
        [Tooltip("모든 Manager들이 초기화가 완료되면 넘어갈 Scene")]
        [SerializeField] private string _playSceneName = "PlayScene";

        /// <summary>
        /// Load Scene Callback. 0.0f ~ 1.0f
        /// UI에서 Progress Bar 표시하기 위해 사용
        /// </summary>
        public event System.Action<float> OnProgress;
        private float _progress = 0.0f;
        
        public event System.Action OnComplete;

        private void Start()
        {
            // manager들을 어떻게 찾을 것인지는 아직 최적화를 못 하겠다. 추후에 수정이 필요하다.
            var managers = FindObjectsOfType<MonoBehaviour>().OfType<IManager>();
            
            // Wait Managers Initialize and load Play Scene
            StartCoroutine(WaitManagersInitAndLoadScene(managers, _playSceneName));
        }

        private IEnumerator WaitManagersInitAndLoadScene(IEnumerable<IManager> managers, string sceneName)
        {
            while(!managers.All(manager => manager.IsInit))
            {
                _progress = managers.Average(manager => manager.Progress);
                Debug.Log($"Progress: {_progress}");
                OnProgress?.Invoke(_progress);
                yield return null;
            }

            OnComplete?.Invoke();
            ResourceManager.Instance.LoadScene(sceneName);
        }
    }
}
