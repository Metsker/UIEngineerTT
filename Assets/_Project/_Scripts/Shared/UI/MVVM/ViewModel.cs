using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using R3;
using UnityEngine.UIElements;

namespace _Project._Scripts.Shared.UI.MVVM
{
    public abstract class ViewModel : INotifyBindablePropertyChanged, IDisposable
    {
        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged = delegate { };
        
        private DisposableBag _disposableBag;
        
        protected void BindProperty<T>(string propertyName, Observable<T> observable)
        {
            observable
                .Subscribe(this, (_, sender) => Publish(sender, propertyName))
                .AddTo(ref _disposableBag);
        }
        
        protected void BindProperties<T>(IEnumerable<string> propertyNames, Observable<T> observable)
        {
            foreach (var propertyName in propertyNames) 
                BindProperty(propertyName, observable);
        }
        
        protected void BindProperties<T>(string propertyName, IEnumerable<Observable<T>> observables)
        {
            foreach (Observable<T> observable in observables) 
                BindProperty(propertyName, observable);
        }

        protected void Set<T>(ref T prop, T value, [CallerMemberName] string caller = null)
        {
            prop = value;
            Publish(caller);
        }

        protected void Publish(object sender = null, [CallerMemberName] string caller = null) =>
            propertyChanged.Invoke(sender ?? this, new BindablePropertyChangedEventArgs(caller));

        public void Dispose() =>
            _disposableBag.Dispose();
    }
}