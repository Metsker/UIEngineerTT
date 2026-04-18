using UnityEngine;
using UnityEngine.UIElements;

namespace _Project._Scripts.Shared.UI.Manipulators {
    public class DragManipulator : PointerManipulator
    {
        private bool _isDragging;
        private Vector2 _offset;

        public DragManipulator()
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
        }
        
        protected override void RegisterCallbacksOnTarget() {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        protected override void UnregisterCallbacksFromTarget() {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        }

        private void OnPointerDown(PointerDownEvent evt) {
            if (!CanStartManipulation(evt) || _isDragging) return;
            
            target.BringToFront();
            target.CapturePointer(evt.pointerId);
            evt.StopPropagation();
        }

        private void OnPointerMove(PointerMoveEvent evt) {
            if (!target.HasPointerCapture(evt.pointerId)) return;
            if (!_isDragging)
            {
                _offset = evt.localPosition;
                _isDragging = true;
            }
            Vector3 delta = evt.localPosition - (Vector3) _offset;
            Translate pos = target.style.translate.value;
            target.style.translate = new Vector3(pos.x.value, pos.y.value) + delta;
            evt.StopPropagation();
        }

        private void OnPointerUp(PointerUpEvent evt) {
            if (!CanStopManipulation(evt)) return;
            
            _isDragging = false;
            target.ReleasePointer(evt.pointerId);
            evt.StopPropagation();
        }
    }
}