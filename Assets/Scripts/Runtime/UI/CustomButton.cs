using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using DG.Tweening;
using System;

namespace Gust.UI
{
    /// <summary>
    /// 버튼 클래스. 클릭을 하면 PressedScale로 스케일이 변경되고, 클릭을 떼면 원래 스케일로 돌아온다.
    /// 비활성화가 됬을 경우 Animation을 중단하고 원래 스케일로 돌아온다.
    /// </summary>
    [ExecuteAlways]
    public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler
    {
        [Header("Button Animation")]
        [Tooltip("Press Scale")]
        [SerializeField] private float _pressedScale = 0.9f;
        protected Vector3 OrignalScale
        {
            private set;
            get;
        }

        [Tooltip("Press 등의 Tween Animation 시간")]
        [SerializeField] private float _tweenDuration = 0.1f;
        [Tooltip("Press 시의 Tween Type")]
        [SerializeField] private Ease _tweenEaseType = Ease.OutCubic;

        [Header("Button Event")]
        [Tooltip("Pointer Up Event")]
        public UnityEvent PointerUpEvent;
        [Tooltip("Pointer Click Event")]
        public UnityEvent PointerClickEvent;
        public UnityEvent PointerDownEvent;
        public UnityEvent PointerPressEvent;
        [Tooltip("Pointer Down 후 Press Event를 발생시키기 위한 Delay 시간, 그 이전에 마우스를 뗄 경우 Press Event가 발생하지 않는다.")]
        [SerializeField] private float _pressStartDelay = 0.5f;
        [Tooltip("Press Event를 발생시키는 간격")]
        [SerializeField] private float _repeatInterval = 0.1f;

        /// <summary>
        /// Pointer가 Button 위에 올라가 있는지 여부
        /// </summary>
        private bool _isHover = false;
        Coroutine _pressedCoroutine;

#if UNITY_EDITOR
        [SerializeField] private bool _shouldLog = false;
#endif

        protected virtual void Awake()
        {
            OrignalScale = transform.localScale;
        }

        private void OnDisable()
        {
            if (DOTween.IsTweening(transform))
            {
                transform.DOKill();
            }
            transform.localScale = OrignalScale;

            if(_pressedCoroutine != null)
            {
                StopCoroutine(_pressedCoroutine);
                _pressedCoroutine = null;
            }
        }

        // @Memo
        // OnPointerDown -> OnPointerUp -> OnPointerClick
        // 언제 애니메이션을 재생할 지 헷갈리므로
        // 애니메이션을 사용할 때는 OnPointerDown에서 애니메이션을 시작하고, OnPointerUp에서 애니메이션을 취소하기로 정한다.

        // Button 밖에서 PointerUp이 됬을 경우에는 Click이 발생하지 않는다. Event 처리는 Click에서 처리할 것

        public virtual void OnPointerClick(PointerEventData eventData)
        {
#if UNITY_EDITOR
            if (_shouldLog)
            {
                Debug.Log($"OnPointerClick: {name}, Time : {Time.time}");
            }
#endif
            PointerClickEvent?.Invoke();

            if(_pressedCoroutine != null)
            {
                StopCoroutine(_pressedCoroutine);
                _pressedCoroutine = null;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
#if UNITY_EDITOR
            if (_shouldLog)
            {
                Debug.Log($"OnPointerDown: {name}, Time : {Time.time}");
            }
#endif
            PointerDownEvent?.Invoke();

            // Pressed 상태로 스케일 변경
            transform.DOScale(OrignalScale * _pressedScale, _tweenDuration)
                .SetEase(_tweenEaseType)
                .SetUpdate(true);

            _pressedCoroutine = StartCoroutine(CheckPressedCoroutine());
        }

        /// <summary>
        /// Pressed 상태에서 일정 시간 동안 PointerPressEvent를 발생시킨다.
        /// Pointer가 Button 위에 올라가 있는 동안에만 발생한다.
        /// </summary>
        private IEnumerator CheckPressedCoroutine()
        {
            float repeatInterval = _repeatInterval;

            // Click 상황으로 가면 Press Event를 발생시키지 않는다.
            yield return new WaitForSeconds(_pressStartDelay);

            while (true)
            {
                if(_isHover)
                {
                    PointerPressEvent?.Invoke();
                    // @To-Do : Update Repeat Interval using damping
                }

                yield return new WaitForSeconds(repeatInterval);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
#if UNITY_EDITOR
            if (_shouldLog)
            {
                Debug.Log($"OnPointerUp: {name}, Time : {Time.time}");
            }
#endif    
            PointerUpEvent?.Invoke();

            if (DOTween.IsTweening(transform))
            {
                transform.DOKill();
            }
            transform.localScale = OrignalScale;
        }

        // Press Event를 Hover 상태에서만 발생하기 위해서 사용

        public void OnPointerEnter(PointerEventData eventData)
        {
#if UNITY_EDITOR
            if (_shouldLog)
            {
                Debug.Log($"OnPointerEnter: {name}, Time : {Time.time}");
            }
#endif
            _isHover = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
#if UNITY_EDITOR
            if (_shouldLog)
            {
                Debug.Log($"OnPointerEnter: {name}, Time : {Time.time}");
            }
#endif
            _isHover = false;
        }

        // OnDrag가 없으면 마우스 포인터가 밖으로 나갔을 때 OnPointerUp이 호출된다.
        // reference: https://issuetracker.unity3d.com/issues/onpointerup-is-called-when-dragging-mouse-from-the-object-which-is-a-child-of-an-inputfield

        public void OnDrag(PointerEventData eventData)
        {
#if UNITY_EDITOR
            if (_shouldLog)
            {
                Debug.Log($"OnDrag: {name}, Time : {Time.time}");
            }
#endif
        }
    }
}
