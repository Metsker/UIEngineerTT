using System;
using R3;
using UnityEngine.UIElements;

namespace _Project._Scripts.Shared.UI.MVVM
{
    public abstract class View<T> : VisualElement, IDisposable where T : ViewModel
    {
        public readonly T ViewModel;

        protected const string ContainerClass = "container";
        private const string ViewClass = "view";

        protected DisposableBag DisposableBag;

        protected View(T viewModel)
        {
            ViewModel = viewModel;
            
            AddToClassList(ViewClass);
            dataSource = viewModel;
        }

        public void Dispose()
        {
            DisposableBag.Dispose();
            ViewModel.Dispose();
        }
    }
}