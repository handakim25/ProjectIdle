using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

using Idle.Utility;

namespace Idle
{
    /// <summary>
    /// Addressable을 Wraping한다.
    /// </summary>
    public sealed class ResourceManager : MonoSingleton<ResourceManager>, IManager
    {
        public bool IsInit => Progress >= 1.0f;
        public float Progress => _progress;
        private float _progress = 0.0f;

        // public void LoadAsset<T>(string key, System.Action<T> callback)
        // {
        //     Addressables.LoadAssetAsync<T>(key).Completed += handle =>
        //     {
        //         callback?.Invoke(handle.Result);
        //     };
        // }

        private void Start()
        {
            StartCoroutine(WatInit());
        }

        private IEnumerator WatInit()
        {
            var initOp = Addressables.InitializeAsync();
            while(!initOp.IsDone)
            {
                _progress = initOp.PercentComplete;
                yield return null;
            }
            _progress = 1.0f;
        }

        // load scene
        public void LoadScene(string sceneName)
        {
            Addressables.LoadSceneAsync(sceneName);
        }
    }
}
