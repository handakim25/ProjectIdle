using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AddressableAssets;

namespace Gust
{
    using Audio;

    /// <summary>
    /// Sound Manager Tester 객체
    /// IMGUI를 이용해서 SoundManager의 기능을 테스트한다.
    /// </summary>
    public class SoundManagerTester : MonoBehaviour
    {
        SoundManager manager;

        private GUIStyle testButtonStyle;

        public AssetReferenceT<AudioClip> _bgmAssetReference;

        private void Start()
        {
            manager = SoundManager.Instance;
            if(manager == null)
            {
                Debug.LogError($"SoundManager is null");
            }
        }

        private void Update()
        {
            Debug.Log($"CurVolume: {manager.CurVolume}");
        }

        private int _buttonWidth = 400;
        private int _buttonHeight = 128;

        private void OnGUI()
        {
            // Start에서 초기화하면 에러가 발생한다.
            testButtonStyle ??= new GUIStyle(GUI.skin.button)
            {
                fontSize = 40,
            };

            using (var verticlaScope = new GUILayout.VerticalScope(GUI.skin.box))
            {
                GUILayout.Space(10);
                if(GUILayout.Button("Play BGM",  testButtonStyle, GUILayout.Width(_buttonWidth), GUILayout.Height(_buttonHeight)))
                {
                    manager.PlayBGM("bgm_test");
                }
                if(GUILayout.Button("Stop BGM",  testButtonStyle, GUILayout.Width(_buttonWidth), GUILayout.Height(_buttonHeight)))
                {
                    manager.StopBGM();
                }
                GUILayout.Space(10);
                if(GUILayout.Button("Fade Out",  testButtonStyle, GUILayout.Width(_buttonWidth), GUILayout.Height(_buttonHeight)))
                {
                    manager.FadeOut(duration: 1.0f);
                }
                if(GUILayout.Button("Fade In",  testButtonStyle, GUILayout.Width(_buttonWidth), GUILayout.Height(_buttonHeight)))
                {
                    manager.FadeIn("bgm_test", 1.0f);
                }
                if(GUILayout.Button("Fade To",  testButtonStyle, GUILayout.Width(_buttonWidth), GUILayout.Height(_buttonHeight)))
                {
                    manager.FadeTo("bgm_test", 1.0f);
                }
            }

        }
    }
}
