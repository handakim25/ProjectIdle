using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gust.Persistence
{
    // @Note
    // 1. 추상화해야할 부분이 2곳이 있다. 첫번째는 저장 위치(Local, Pref, Network)이고 두번째는 데이터 형식이다.
    // 2. Network를 지원하기 위해서는 비동기 기능이 필요. Network를 지원하기 위해서는 또한 다른 Network Manger하고 연동하는 방법을 결정해야 한다.

    /// <summary>
    /// Save File을 저장하기 위한 Interface
    /// 데이터 형식은 Manager에서 결정한다.
    /// </summary>
    public interface IPersistenceStrategy
    {
        bool Save(string fileName, string data);
        bool Load(string fileName, out string data);
    }

    public enum PersistenceType
    {
        PlayerPrefs,
        File,
    }

    public static class PersistenceStrategyFactory
    {
        public static IPersistenceStrategy Create(PersistenceType persistenceType)
        {
            return persistenceType switch
            {
                PersistenceType.PlayerPrefs => new PlayerPrefsPersistenceStrategy(),
                PersistenceType.File => new FilePersistenceStrategy(),
                _ => null,
            };
        }
    }

    public class PlayerPrefsPersistenceStrategy : IPersistenceStrategy
    {
        public bool Save(string fileName, string data)
        {
            PlayerPrefs.SetString(fileName, data);
            return true;
        }

        public bool Load(string fileName, out string data)
        {
            data = PlayerPrefs.GetString(fileName);
            return true;
        }
    }

    public class FilePersistenceStrategy : IPersistenceStrategy
    {
        public bool Load(string fileName, out string data)
        {
            data = string.Empty;

            try
            {
                if (!System.IO.File.Exists(fileName))
                {
                    return false;
                }

                data = System.IO.File.ReadAllText(fileName);
                return true; // Add this line to return a value
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        public bool Save(string fileName, string data)
        {
            try
            {
                System.IO.File.WriteAllText(fileName, data);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }
    }
}
