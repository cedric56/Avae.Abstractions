namespace Avae.Abstractions.Bases
{
    public abstract class RouterViewModelBase
    {
        protected Router _router;
        public RouterViewModelBase(Router router)
        {
            _router = router;
        }

        public virtual void GoBack()
        {
            if (CanGoBack())
            {
                var viewModel = _router.Back()!;
                OnViewModelChanged(viewModel);
            }
        }

        public bool CanGoBack()
        {
            return _router.CanGoBack;
        }

        public virtual void GoForward()
        {
            if (CanGoForward())
            {
                var viewModel = _router.Forward()!;
                OnViewModelChanged(viewModel);
            }
        }

        public bool CanGoForward()
        {
            return _router.CanGoForward;
        }

        protected virtual void OnViewModelChanged(IViewModelBase viewModel)
        {
            RaiseCanExecutesChanged();
        }

        protected abstract void RaiseCanExecutesChanged();
    }
}
