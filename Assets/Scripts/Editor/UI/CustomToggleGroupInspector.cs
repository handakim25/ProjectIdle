using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Gust.UI
{
    [CustomEditor(typeof(CustomToggleGroup))]
    public class CustomToggleGroupInspector : Editor
    {
        private CustomToggleGroup _toggleGroup;
        /// <summary>
        /// 하위 객체들 중 CustomToggleButton을 캐싱합니다.
        /// </summary>
        private CustomToggleButton[] _toggleButtonCache;
        private int _registeredCount;

        private void OnEnable()
        {
            _toggleGroup = target as CustomToggleGroup;
            CacheToggleInfo();
            EditorApplication.hierarchyChanged += CacheToggleInfo;
        }

        private void OnDisable()
        {
            EditorApplication.hierarchyChanged -= CacheToggleInfo;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);

            GUILayout.Label($"{_registeredCount} Toggle Buttons are Registered");

            if(GUILayout.Button("Register Toggle Buttons in Children"))
            {
                _registeredCount = _toggleButtonCache.Length; // 어차피 전부 등록하므로 개수는 동일해진다.
                foreach(var toggleButton in _toggleButtonCache)
                {
                    var toggleSo = new SerializedObject(toggleButton);
                    var toggleProp = toggleSo.FindProperty("_toggleGroup");
                    if(toggleProp.objectReferenceValue != _toggleGroup)
                    {
                        toggleProp.objectReferenceValue = _toggleGroup;
                        toggleSo.ApplyModifiedProperties();
                    }
                }
            }
        }

        private void CacheToggleInfo()
        {
            _toggleButtonCache = _toggleGroup.GetComponentsInChildren<CustomToggleButton>();
            _registeredCount = 0;

            foreach (var toggleButton in _toggleButtonCache)
            {
                var toggleSo = new SerializedObject(toggleButton);
                var toggleProp = toggleSo.FindProperty("_toggleGroup");
                if (toggleProp.objectReferenceValue == _toggleGroup)
                {
                    _registeredCount++;
                }
            }
        }
    }
}
