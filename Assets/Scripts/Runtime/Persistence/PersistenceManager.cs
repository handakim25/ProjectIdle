using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

using Gust.Utility;

namespace Gust.Persistence
{
    public interface IPersistence
    {
        string FileName { get; }
        /// <summary>
        /// string 형태로 저장한다.
        /// </summary>
        /// <returns></returns>
        string Save();
        /// <summary>
        /// string 형태로 저장된 데이터를 불러온다.s
        /// </summary>
        bool Load(string data);
    }

    /// <summary>
    /// 설정값을 저장하기 위한 Manager. 설정에 따라 PlayerPrefs나 File로 저장한다.
    /// </summary>
    public sealed class PersistenceManager : MonoSingleton<PersistenceManager>
    {
        // @To-Do
        // Game Save Data를 저장하는 기능 추가

        public enum PersistenceType
        {
            PlayerPrefs,
            File,
        }

        [Tooltip("저장 방식을 선택한다.")]
        [SerializeField] private PersistenceType persistenceType = PersistenceType.PlayerPrefs;

        private const string _saveDataKey = "SaveData";

        private Dictionary<string, IPersistence> _persistenceDict = new Dictionary<string, IPersistence>();

        public void Register(IPersistence persistence)
        {
            if (_persistenceDict.ContainsKey(persistence.FileName))
            {
                Debug.LogWarning($"Already registered {persistence.FileName}");
                return;
            }

            _persistenceDict.Add(persistence.FileName, persistence);
        }

        public void Unregister(IPersistence persistence)
        {
            if (!_persistenceDict.ContainsKey(persistence.FileName))
            {
                Debug.LogWarning($"Not registered {persistence.FileName}");
                return;
            }

            _persistenceDict.Remove(persistence.FileName);
        }

        public void Save()
        {
            var saveData = new Dictionary<string, string>();

            foreach (var persistence in _persistenceDict.Values)
            {
                saveData.Add(persistence.FileName, persistence.Save());
            }

            PlayerPrefs.SetString(_saveDataKey, JsonConvert.SerializeObject(saveData));
            PlayerPrefs.Save();
        }

        public void Load()
        {
            if (!PlayerPrefs.HasKey(_saveDataKey))
            {
                Debug.LogWarning("No save data");
                return;
            }

            var saveData = JsonConvert.DeserializeObject<Dictionary<string, string>>(PlayerPrefs.GetString(_saveDataKey));

            foreach (var persistence in _persistenceDict.Values)
            {
                if (saveData.ContainsKey(persistence.FileName))
                {
                    persistence.Load(saveData[persistence.FileName]);
                }
            }
        }
    }
}
