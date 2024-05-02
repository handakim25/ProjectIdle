using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

namespace Gust.UI
{
    /// <summary>
    /// UI의 기본 오브젝트를 정의
    /// </summary>
    public static class UIDefaultObject
    {
        private const float kWidth = 160f;
        private const float kThickHeight = 30f;
        private const float kThinHeight = 20f;
        private static Vector2 s_ThickElementSize = new(kWidth, kThickHeight);
        private static Vector2 s_ThinElementSize = new(kWidth, kThinHeight);

        private const int kFontSize = 24;

        /// <summary>
        /// RectTransform을 가진 GameObject를 생성합니다.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private static GameObject CreateUIElementRoot(string name, Vector2 size)
        {
            var go = new GameObject(name);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = size;
            return go;
        }

        /// <summary>
        /// UI Element Root에 자식으로 들어갈 GameObject를 생성합니다.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private static GameObject CreateUiObject(string name, GameObject parent, bool stretch = true)
        {
            var go = new GameObject(name, typeof(RectTransform));
            if (parent != null)
            {
                go.transform.SetParent(parent.transform, false);
                go.GetComponent<RectTransform>().Stretch();
            }
            return go;
        }

        /// <summary>
        /// Custom Button을 생성합니다.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// - Custom_Button(CustomButton)
        ///  - Background(Image)
        ///  - TextRoot(TextMeshProUGUI)
        /// </remarks>
        public static GameObject CreateCustomButton()
        {
            var buttonRoot = CreateUIElementRoot("Custom_Button", s_ThickElementSize);

            var background = CreateUiObject("Background", buttonRoot);
            var textRoot = CreateUiObject("TextRoot", buttonRoot);

            buttonRoot.AddComponent<CustomButton>();

            // Background
            background.AddComponent<Image>();

            // text
            var text = CreateUiObject("Text", textRoot);
            var textComponent = text.AddComponent<TextMeshProUGUI>();
            textComponent.color = Color.black;
            textComponent.text = "Button";
            textComponent.alignment = TextAlignmentOptions.Center;
            textComponent.fontSize = kFontSize;

            return buttonRoot;
        }

        /// <summary>
        /// Custom Toggle Button을 생성합니다.
        /// </summary>
        /// <returns></returns>
        /// <remarks
        /// - Custom_Button(CustomToggleButton)
        ///  - Background(Image)
        ///  - BackgroundSelected(Image)
        ///  - TextRoot
        ///   - Text(TextMeshProUGUI)
        /// </remarks>
        public static GameObject CreateCutstomToggleButton()
        {
            var buttonRoot = CreateUIElementRoot("Custom_Toggle_Button", s_ThickElementSize);

            var background = CreateUiObject("Background", buttonRoot);
            var backgroundSelected = CreateUiObject("BackgroundSelected", buttonRoot);
            var textRoot = CreateUiObject("TextRoot", buttonRoot);
            var text = CreateUiObject("Text", textRoot);

            // Background
            background.AddComponent<Image>();

            // background selected
            backgroundSelected.AddComponent<Image>();
            backgroundSelected.SetActive(false);

            // text
            var textComponent = text.AddComponent<TextMeshProUGUI>();
            textComponent.color = Color.black;
            textComponent.text = "Button";
            textComponent.alignment = TextAlignmentOptions.Center;
            textComponent.fontSize = kFontSize;

            var customToggleButton = buttonRoot.AddComponent<CustomToggleButton>();
            customToggleButton.NormalBackground = background;
            customToggleButton.SelectedBackground = backgroundSelected;

            return buttonRoot;
        }

        private static void StretchRectTransform(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
        }

        private static void Stretch(this RectTransform rectTransform)
        {
            StretchRectTransform(rectTransform);
        }

    }
}
