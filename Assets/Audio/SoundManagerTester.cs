using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gust
{
    using Audio;

    /// <summary>
    /// Sound Manager Tester 객체
    /// </summary>
    public class SoundManagerTester : MonoBehaviour
    {
        SoundManager manager;

        private GUIStyle testButtonStyle;

        private void Start()
        {
            manager = SoundManager.Instance;

            testButtonStyle = new GUIStyle(GUI.skin.button);
            testButtonStyle.fontSize = 24;
        }

        public int startX = 10;
        public int startY = 10;

        public int width = 400;
        public int height = 128;

        public int interval = 10;

        private void OnGUI()
        {
            using (var verticlaScope = new GUILayout.VerticalScope(GUI.skin.box))
            {
                GUILayout.Space(10);
                if(GUILayout.Button("Play BGM", testButtonStyle, GUILayout.Width(400), GUILayout.Height(128)))
                {
                    manager.PlayBGM("bgm_test");
                }
                if(GUILayout.Button("Stop BGM", testButtonStyle, GUILayout.Width(400), GUILayout.Height(128)))
                {
                    manager.StopBGM();
                }
            }

        }
    }
}
