using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using DG.Tweening;

namespace Gust.UI
{
    /// <summary>
    /// 버튼 클래스. 클릭을 하면 PressedScale로 스케일이 변경되고, 클릭을 떼면 원래 스케일로 돌아온다.
    /// 비활성화가 됬을 경우 Animation을 중단하고 원래 스케일로 돌아온다.
    /// </summary>
    [ExecuteAlways]
    public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
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
        public UnityEvent PointerUp;
        [Tooltip("Pointer Click Event")]
        public UnityEvent PointerClick;

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
                Debug.Log($"OnPointerClick: {name}");
            }
#endif
            PointerClick?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
#if UNITY_EDITOR
            if (_shouldLog)
            {
                Debug.Log($"OnPointerDown: {name}");

            }
#endif
            // Pressed 상태로 스케일 변경
            transform.DOScale(OrignalScale * _pressedScale, _tweenDuration)
                .SetEase(_tweenEaseType)
                .SetUpdate(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
#if UNITY_EDITOR
            if (_shouldLog)
            {
                Debug.Log($"OnPointerUp: {name}");
            }
#endif    
            PointerUp?.Invoke();

            if (DOTween.IsTweening(transform))
            {
                transform.DOKill();
            }
            transform.localScale = OrignalScale;
        }
    }
}
