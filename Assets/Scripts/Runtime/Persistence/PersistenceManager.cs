using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

using Gust.Utility;
using System;

namespace Gust.Persistence
{
    // @To-Do
    // 직렬화 관련된 부분을 확장성 있게 수정할 수 있을 것 같다.
    // Unity ShaderGraph 관련 코드 참고할 것

    /// <summary>
    /// Save File을 저장하기 위한 Interface
    /// </summary>
    public interface IPersistence
    {
        string FileName { get; }
        /// <summary>
        /// string 형태로 저장한다.
        /// </summary>
        /// <returns></returns>
        string SaveDataToString();
        /// <summary>
        /// string 형태로 저장된 데이터를 불러온다.
        /// </summary>
        bool LoadDataFromString(string data);
    }

    /// <summary>
    /// 설정값을 저장하기 위한 Manager. 설정에 따라 PlayerPrefs나 File로 저장한다.
    /// </summary>
    public sealed class PersistenceManager : MonoSingleton<PersistenceManager>
    {
        // @To-Do
        // Game Save Data를 저장하는 기능 추가

        /// <summary>
        /// 저장 방식을 선택한다. Run-Time에서 변경할 수 없다.
        /// </summary>
        [Header("Save Settings")]
        [Tooltip("저장 방식을 선택한다.")]
        [SerializeField] private PersistenceType persistenceType = PersistenceType.PlayerPrefs;
        private IPersistenceStrategy _persistanceStrategy;

        private void Awake()
        {
            _persistanceStrategy = PersistenceStrategyFactory.Create(persistenceType);
        }

        /// <summary>
        /// 현재 데이터를 저장한다.
        /// </summary>
        /// <param name="data"></param>
        public bool SaveData(IPersistence data)
        {
            return SaveData(data.FileName, data.SaveDataToString());
        }

        private bool SaveData(string fileName, string data)
        {
            if(string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(data))
            {
                Debug.LogError("Failed to save data. fileName or data is empty.");
                return false;
            }

            if (_persistanceStrategy.Save(fileName, data))
            {
                return true;
            }
            else
            {
                Debug.LogError("Failed to save data to persistence strategy.");
                return false;
            }
        }

        public bool LoadData(IPersistence persistence)
        {
            if(LoadData(persistence.FileName, out string data))
            {
                return persistence.LoadDataFromString(data);
            }
            else
            {
                return false;
            }
        }

        private bool LoadData(string fileName, out string data)
        {
            if(string.IsNullOrEmpty(fileName))
            {
                Debug.LogError("Failed to load data. fileName is empty.");
                data = string.Empty;
                return false;
            }

            if (_persistanceStrategy.Load(fileName, out data))
            {
                return true;
            }
            else
            {
                Debug.LogError("Failed to load data from persistence strategy.");
                return false;
            }
        }
    }
}
