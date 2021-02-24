using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CrazyGames.Logires.Utils
{
    public enum CurrentState
    {
        Start,
        Change,
        End
    }

    public class InputHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IInitializePotentialDragHandler, IPointerUpHandler
    {
        public delegate void MoveHandler(Vector2 delta, CurrentState state);
        public event MoveHandler OnMoved;
        public delegate void ZoomHandler(float delta, Vector2 point);
        public event ZoomHandler OnZoomed;

        private Vector2 _movingInput = Vector2.zero;
        private FixedSizedQueue<Vector2> _movingAcceleration = new FixedSizedQueue<Vector2>(5);
        private float _zoomInput = 0;

        public Vector2 MovingDirection
        {
            get
            {
                var temp = _movingInput;
                _movingInput = Vector2.zero;
                return temp;
            }
        }
        public float ZoomDelta
        {
            get
            {
                var temp = -_zoomInput;
                _zoomInput = 0;
                return temp;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (Input.touchCount == 1 || Input.GetMouseButton(0))
            {
                _movingInput = Vector2.zero;
                OnMoved?.Invoke(MovingDirection, CurrentState.Start);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (Input.touchCount == 1 || Input.touchCount == 0)
            {
                _movingInput = eventData.delta;
                _movingAcceleration.Enqueue(_movingInput);

                OnMoved?.Invoke(MovingDirection, CurrentState.Change);
            }
            else if (Input.touchCount == 2)
            {
                _movingInput = eventData.delta;
                _movingAcceleration.Enqueue(_movingInput);

                OnMoved?.Invoke(MovingDirection, CurrentState.Change);

                var distance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position) /
                               Vector2.Distance(Input.touches[0].position - Input.touches[0].deltaPosition, Input.touches[1].position - Input.touches[1].deltaPosition);

                _zoomInput = distance - 1;

                OnZoomed?.Invoke(ZoomDelta, Vector2.Lerp(Input.touches[0].position, Input.touches[1].position, 0.5f));
            }
        }
        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            int curTouches = Input.touchCount - 1;
            if (curTouches <= 0)
            {
                _movingInput = eventData.delta;

                int count = _movingAcceleration.Count;
                for (int i = 0; i < count; i++)
                {
                    _movingAcceleration.TryDequeue(out var moving);
                    _movingInput += moving;
                }
                
                OnMoved?.Invoke(MovingDirection, CurrentState.End);
            }
        }
    }
}