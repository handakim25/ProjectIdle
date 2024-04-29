using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Gust.Persistence
{
    [CustomEditor(typeof(PersistenceManager))]
    public class PersistenceManagerEditor : Editor
    {
        PersistenceManager _target;

        private void OnEnable()
        {
            _target = target as PersistenceManager;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);
            if(GUILayout.Button("Show Data"))
            {
                switch(_target.PersistenceType)
                {
                    case PersistenceType.PlayerPrefs:
                        PlayerPrefsViewerWindow.Open();
                        break;
                    case PersistenceType.File:
                        // @To-Do
                        // Open File and view data
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
