using System.Collections.ObjectModel;

namespace Avae.ViewModels.Tests.TestDoubles;

/// <summary>
/// Concrete <see cref="NavigableViewModelBase"/> used to test navigation/history/caching behavior.
/// </summary>
/// <remarks>
/// <see cref="NavigableViewModelBase"/>'s own constructor can, when <c>initialize</c> is
/// <see langword="true"/>, synchronously select the first navigable item - which calls the abstract
/// <see cref="GetNavigables"/> *before* any field assigned in this type's constructor body has run
/// (base constructors always execute before derived constructor bodies). A normal constructor
/// parameter captured into a field would therefore be observed as not-yet-assigned the first time
/// <see cref="GetNavigables"/> is invoked. <see cref="Create"/> works around this by handing the
/// navigables collection to the instance via a <see cref="ThreadStaticAttribute"/> slot that is
/// populated immediately before construction and read by <see cref="GetNavigables"/> for as long as
/// it is set, falling back to the (by-then-assigned) field afterwards.
/// </remarks>
public class TestNavigableViewModel : NavigableViewModelBase
{
    [ThreadStatic]
    private static ObservableCollection<NavigableView>? _pending;

    public static TestNavigableViewModel Create(Router router, ObservableCollection<NavigableView> navigables, bool initialize = true)
    {
        _pending = navigables;
        try
        {
            return new TestNavigableViewModel(router, initialize);
        }
        finally
        {
            _pending = null;
        }
    }

    private readonly ObservableCollection<NavigableView> _navigables;

    private TestNavigableViewModel(Router router, bool initialize)
        : base(router, initialize)
    {
        _navigables = _pending ?? [];
    }

    public List<string> NotifiedProperties { get; } = [];

    public int RaiseCanExecutesChangedCallCount { get; private set; }

    protected override ObservableCollection<NavigableView> GetNavigables() => _pending ?? _navigables;

    protected override void NotifyPropertyChanged(string propertyName)
    {
        NotifiedProperties.Add(propertyName);
    }

    protected override void RaiseCanExecutesChanged()
    {
        RaiseCanExecutesChangedCallCount++;
    }
}
