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
        public PersistenceType PersistenceType => persistenceType;

        private IPersistenceStrategy _persistanceStrategy;

        protected override void Init()
        {
            _persistanceStrategy = PersistenceStrategyFactory.Create(persistenceType);
        }

        /// <summary>
        /// 데이터를 저장한다. 데이터의 저장 방법은 설정된 방식에 따라 달라진다. e.g PlayerPrefs, File
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SaveData(string fileName, string data)
        {
            if(string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(data))
            {
                Debug.LogError($"{nameof(PersistenceManager)}: 입력값을 확인해주세요.");
                return false;
            }

            return _persistanceStrategy.Save(fileName, data);
        }

        public bool SaveData<T>(string fileName, T data)
        {
            return SaveData(fileName, JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// Data를 로드한다. 만약에 파일이 존재하지 않거나 파일을 읽는데 실패하면 null을 반환한다.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>읽어들인 데이터, 만약에 파일이 존재하지 않거나 파일을 읽는데 실패하면 null을 반환한다.</returns>
        public string LoadData(string fileName)
        {
            if(string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            return _persistanceStrategy.Load(fileName, out string data) ? data : null;
        }

        /// <summary>
        /// Data를 로드한다. 만약에 파일이 존재하지 않거나 파일을 읽는데 실패하면 default 값을 반환한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public T LoadData<T>(string fileName)
        {
            string data = LoadData(fileName);
            return string.IsNullOrEmpty(data) ? default : JsonConvert.DeserializeObject<T>(data);
        }

        /// <summary>
        /// Data 존재 확인
        /// </summary>
        /// <param name="fileName">확인할 Data 이름</param>
        /// <returns></returns>
        public bool HasFile(string fileName)
        {
            return _persistanceStrategy.IsFileExist(fileName);
        }
    }
}
