#nullable disable
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Avae.Abstractions
{
    /// <summary>
    /// This class is used to manage the pages in the application.
    /// </summary>
    public abstract partial class PagesViewModelBase : ObservableObject, IViewModelBase
    {
        /// <summary>
        /// A dictionary to store the context for each page.
        /// </summary>
        private readonly Dictionary<Type, IContextFor> dico = [];

        /// <summary>
        /// The currently selected page in the menu.
        /// </summary>
        [ObservableProperty]
        private IContextFor _currentPage = null!;

        /// <summary>
        /// The currently selected page in the menu.
        /// </summary>
        [ObservableProperty]
        private PageViewModelBase _selectedPage;

        /// <summary>
        /// The router used to navigate between pages.
        /// </summary>
        protected Router _router;

        public PagesViewModelBase(Router router, bool initialize = true)
        {
            _router = router;

            // Subscribe to the router's current view model changed event
            _router.CurrentViewModelChanged += OnRouterCurrentViewModelChanged;

            if (initialize)
            {
                var page = Pages.FirstOrDefault();
                if (CurrentPage == null && page != null)
                {
                    OnRouterCurrentViewModelChanged(_router.Create<IViewModelBase>(page.ViewModelType, true, page.ViewModelParameters));
                }
            }
        }

        /// <summary>
        /// This method is called when the current view model changes in the router.
        /// </summary>
        /// <param name="vm"></param>
        private void OnRouterCurrentViewModelChanged(IViewModelBase vm)
        {
            var m = Pages.FirstOrDefault(mi => mi.ViewModelType == vm.GetType());// ?? throw new InvalidOperationException($"The view model {vm.GetType().Name} is not registered in the pages list.");
            if (m == null)
                return;

            if (m.ViewModelType == SelectedPage?.ViewModelType)
            {
                AddOrUpdate();
                return;
            }

            AddOrUpdate();

            //Debug.WriteLine(_router.Current);

            // If the page is not already in the dictionary, add it
            void AddOrUpdate()
            {
                if (dico.TryGetValue(m.ViewModelType, out var context))
                {
                    CurrentPage = context;                    
                }
                else
                {
                    var configuration = SimpleProvider.GetService<IIocConfiguration>();
                    var page = configuration!.GetContextFor(vm.GetType().Name, m.ViewParameters);
                    //Avoid binding error due to propagating context
                    if (page != null)
                        page.DataContext = null;
                    CurrentPage = page;
                    dico[m.ViewModelType] = CurrentPage;
                }

                SelectedPage = m;

                if (m.SetDataContext)
                    CurrentPage.DataContext = vm;
            }
        }


        /// <summary>
        /// The list of pages to be displayed in the menu.
        /// </summary>
        public abstract ObservableCollection<PageViewModelBase> Pages { get; }

        /// <summary>
        /// This method is called when the selected page changes.
        /// </summary>
        /// <param name="value"></param>
        partial void OnSelectedPageChanged(PageViewModelBase value)
        {
            if (value == null)
                return;

            if (dico.TryGetValue(value.ViewModelType, out var context))
            {
                CurrentPage = context;
            }
            else
            {
                _router.GoTo<IViewModelBase>(value.ViewModelType, value.ViewModelParameters);
            }
        }

        /// <summary>
        /// This method is called when the user scrolls the menu.
        /// </summary>
        /// <param name="delta"></param>
        [RelayCommand]
        public void OnScrollChanged(object delta)
        {
            if (delta is (double x, double y))
            {
                var index = (int)y;

                if (index < 0)
                    index = 0;
                else if (index >= Pages.Count)
                    index = Pages.Count - 1;

                SelectedPage = Pages[index];
            }
        }
    }
}
