using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Gust.Persistence
{
    public class PlayerPrefsViewerWindow : EditorWindow
    {
        public int windowWidth = 400;

        [MenuItem("Tools/PlayerPrefs Viewer")]
        public static void Open()
        {
            PlayerPrefsViewerWindow window = GetWindow<PlayerPrefsViewerWindow>();
            window.titleContent = new GUIContent("PlayerPrefs Viewer");
            window.minSize = new Vector2(window.windowWidth, 400);
            window.Show();
        }

        string[] keys;

        // https://stackoverflow.com/questions/5142349/declare-a-const-array
        // array는 객체이므로 상수로 선언할 수 없다고 하는 것 같다.
        readonly string[] unityDefinedKeys = new string[]
        {
            "UnityGraphicsQuality",
            "unity.player_session_count",
            "unity.player_sessionid",
            "unity.cloud_userid",
        };

        private void OnGUI()
        {
            if(GUILayout.Button("Refresh"))
            {
                keys = GetAllPrefsKeys();
            }

            foreach(var key in keys)
            {
                EditorGUILayout.LabelField(key, PlayerPrefs.GetString(key));
            }
        }

        private void OnEnable()
        {
            keys = GetAllPrefsKeys();
        }

        private string[] GetAllPrefsKeys()
        {
            // https://docs.unity3d.com/2020.1/Documentation/ScriptReference/PlayerPrefs.html
            // On Windows, PlayerPrefs are stored in the registry under HKCU\Software\[company name]\[product name] key,
            // where company and product names are the names set up in Project Settings.

            // Editor의 경우는 Unity/UnityEditor/CompanyName/ProductName
            string regKeyPath = $"Software\\Unity\\UnityEditor\\{PlayerSettings.companyName}\\{PlayerSettings.productName}";
            var regKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(regKeyPath);
            if(regKey == null)
            {
                return new string[0];
            }

            string[] playerPrefs = regKey.GetValueNames();
            // Key 뒤에 _h 붙어 있어서 해당 값 제거
            return playerPrefs.Select(key => key[..key.LastIndexOf("_h")]).ToArray();
        }
    }
}
