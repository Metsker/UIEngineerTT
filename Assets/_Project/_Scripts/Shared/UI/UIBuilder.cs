using System;
using _Project._Scripts.Shared.Data;
using _Project._Scripts.Shared.UI.MVVM;
using R3;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Project._Scripts.Shared.UI
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class UIBuilder : MonoBehaviour
    {
        [Inject] private StyleSheetRepository _styleSheets;

        private Container _sceneContainer;

        private void Awake()
        {
            UIDocument uiDocument = GetComponent<UIDocument>();
            VisualElement root = uiDocument.rootVisualElement;
            _sceneContainer = gameObject.scene.GetSceneContainer();
            
            root.styleSheets.Add(_styleSheets.global);
            
            Build(root, _styleSheets);
        }

        protected TView AttachView<TView, TViewModel>(
            Func<TViewModel, TView> factory, VisualElement parent, StyleSheet styleSheet)
        
            where TView : View<TViewModel> where TViewModel : ViewModel
        {
            TViewModel viewModel = _sceneContainer.Construct<TViewModel>();

            TView view = AttachElement(factory(viewModel), parent, styleSheet);
            view.AddTo(this);
            
            return view;
        }

        protected static T AttachElement<T>(T element, VisualElement parent, StyleSheet styleSheet) where T : VisualElement
        {
            element.styleSheets.Add(styleSheet);
            parent.Add(element);
            return element;
        }

        protected abstract void Build(VisualElement root, StyleSheetRepository styleSheets);
    }
}