using System;
using R3;
using UnityEngine.UIElements;

namespace _Project._Scripts.Shared.UI.MVVM
{
    public abstract class View<T> : VisualElement, IDisposable where T : ViewModel
    {
        protected readonly T ViewModel;
        
        public const string ViewUSS = "view";
        
        protected DisposableBag DisposableBag;

        protected View(T viewModel)
        {
            ViewModel = viewModel;
            
            AddToClassList(ViewUSS);
            dataSource = viewModel;
        }

        public void Dispose()
        {
            DisposableBag.Dispose();
            ViewModel.Dispose();
        }
    }
}