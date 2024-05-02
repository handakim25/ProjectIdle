using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Gust.UI
{
    public static class UICustomMenu
    {
        // 참고
        // https://github.com/Unity-Technologies/uGUI/blob/2019.1/UnityEditor.UI/UI/MenuOptions.cs
        // https://github.com/Unity-Technologies/uGUI/blob/2019.1/UnityEngine.UI/UI/Core/DefaultControls.cs

        /// <summary>
        /// 생성한 UI Element를 부모에 맞게 배치합니다.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="menuCommand"></param>
        private static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
        {
            // @TO-DO
            // 원래 UI Element처럼 부모가 없을 경우 Canvas를 생성하고 부모로 설정해주는 기능이 필요

            GameObjectUtility.SetParentAndAlign(element, menuCommand.context as GameObject);

            Undo.RegisterCreatedObjectUndo(element, "Create " + element.name);
            Selection.activeObject = element;
        }

        [MenuItem("GameObject/UI/Custom Button", false, 10)]
        static void CreateCustomButton(MenuCommand menuCommand)
        {
            GameObject go = UIDefaultObject.CreateCustomButton();

            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/UI/Custom Toggle Button", false, 10)]
        static void CreateToggleButton(MenuCommand menuCommand)
        {
            GameObject go = UIDefaultObject.CreateCutstomToggleButton();

            PlaceUIElementRoot(go, menuCommand);
        }
    }
}
