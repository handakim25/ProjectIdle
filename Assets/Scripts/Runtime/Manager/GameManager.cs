using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

using Gust.Utility;
using Gust.Audio;
using Gust.Persistence;

namespace Gust
{
    public sealed class GameManager : MonoSingleton<GameManager>, IManager
    {
        public bool IsInit => Progress >= 1.0f;

        public float Progress => 1.0f;
        [SerializeField] private string _gameSettingFileName = "GameSetting.json";
        private GameSetting _setting;
        public GameSetting GameSetting => _setting;

        private void Start()
        {
            var LoadSceneController = FindObjectOfType<LoadSceneController>();
            if(LoadSceneController != null)
            {
                // Wait other Managers
                LoadSceneController.OnComplete += OnCompleteHandler;
            }
            else
            {
                OnCompleteHandler();
            }
        }

        private void OnCompleteHandler()
        {
            // Load Game Settings
            _setting = LoadGameSetting();

            // Apply Game Settings
            SoundManager.Instance.LoadSetting(_setting);
        }

        /// <summary>
        /// 게임 설정을 불러온다. 만약에 기존 설정이 없을 경우 새로운 설정을 생성하고 저장한다.
        /// </summary>
        /// <remarks>
        /// Persistence Manager는 Awake에서 초기화가 되기 때문에 적어도 Start에서 호출해야 한다.
        /// </remarks>
        private GameSetting LoadGameSetting()
        {
            if(PersistenceManager.Instance.HasFile(_gameSettingFileName))
            {
                return PersistenceManager.Instance.LoadData<GameSetting>(_gameSettingFileName) ?? new GameSetting();
            }
            else
            {
                var setting = new GameSetting();
                PersistenceManager.Instance.SaveData(_gameSettingFileName, setting);
                return setting;
            }
        }

        public void SaveGameSetting()
        {
            PersistenceManager.Instance.SaveData(_gameSettingFileName, _setting);
        }
    }
}