using System.Collections.ObjectModel;
using Avae.ViewModels.Tests.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Avae.ViewModels.Tests;

public class NavigableViewModelBaseTests
{
    private static Router CreateRouter()
    {
        var services = new ServiceCollection();
        var configuration = Substitute.For<IIocConfiguration>();
        configuration.GetContextFor(Arg.Any<string>(), Arg.Any<NavigableContext>())
            .Returns(callInfo => new FakeViewFor());
        services.AddSingleton(configuration);
        return new Router(services.BuildServiceProvider());
    }

    private static ObservableCollection<NavigableView> TwoNavigablesWithOwnViewModels(
        out NavigableView<FakeViewModel> first,
        out NavigableView<OtherFakeViewModel> second)
    {
        first = new NavigableView<FakeViewModel>(new FakeViewModel(), "First");
        second = new NavigableView<OtherFakeViewModel>(new OtherFakeViewModel(), "Second");
        return [first, second];
    }

    [Fact]
    public void Navigables_is_populated_from_GetNavigables_and_then_cached()
    {
        var navigables = TwoNavigablesWithOwnViewModels(out _, out _);
        var sut = TestNavigableViewModel.Create(CreateRouter(), navigables, initialize: false);

        Assert.Same(navigables, sut.Navigables);
        Assert.Same(sut.Navigables, sut.Navigables); // same instance on repeated access
    }

    [Fact]
    public void Constructing_with_initialize_true_selects_the_first_navigable_and_navigates_to_it()
    {
        var navigables = TwoNavigablesWithOwnViewModels(out var first, out _);
        var sut = TestNavigableViewModel.Create(CreateRouter(), navigables, initialize: true);

        Assert.Same(first, sut.SelectedNavigable);
        Assert.NotNull(sut.CurrentView);
        Assert.Contains(nameof(sut.SelectedNavigable), sut.NotifiedProperties);
        Assert.Contains(nameof(sut.CurrentView), sut.NotifiedProperties);
    }

    [Fact]
    public void Constructing_with_initialize_false_does_not_select_a_navigable()
    {
        var navigables = TwoNavigablesWithOwnViewModels(out _, out _);
        var sut = TestNavigableViewModel.Create(CreateRouter(), navigables, initialize: false);

        Assert.Null(sut.SelectedNavigable);
        Assert.Null(sut.CurrentView);
    }

    [Fact]
    public void Selecting_a_different_navigable_updates_CurrentView_and_raises_CurrentViewChanged()
    {
        var navigables = TwoNavigablesWithOwnViewModels(out var first, out var second);
        var sut = TestNavigableViewModel.Create(CreateRouter(), navigables, initialize: true);
        var firstView = sut.CurrentView;

        IViewFor? raised = null;
        sut.CurrentViewChanged += (_, view) => raised = view;

        sut.SelectedNavigable = second;

        Assert.Same(second, sut.SelectedNavigable);
        Assert.NotNull(sut.CurrentView);
        Assert.NotSame(firstView, sut.CurrentView);
        Assert.Same(sut.CurrentView, raised);
    }

    [Fact]
    public void Reselecting_a_previously_visited_navigable_reuses_the_cached_view()
    {
        var navigables = TwoNavigablesWithOwnViewModels(out var first, out var second);
        var sut = TestNavigableViewModel.Create(CreateRouter(), navigables, initialize: true);
        var firstView = sut.CurrentView;

        sut.SelectedNavigable = second;
        var secondView = sut.CurrentView;
        Assert.NotSame(firstView, secondView);

        sut.SelectedNavigable = first;

        Assert.Same(firstView, sut.CurrentView);
    }

    [Fact]
    public void Setting_SelectedNavigable_to_null_is_a_no_op_for_navigation()
    {
        var navigables = TwoNavigablesWithOwnViewModels(out _, out _);
        var sut = TestNavigableViewModel.Create(CreateRouter(), navigables, initialize: false);

        sut.SelectedNavigable = null;

        Assert.Null(sut.SelectedNavigable);
        Assert.Null(sut.CurrentView);
    }

    [Fact]
    public void GoBack_after_navigating_restores_the_previous_navigable_and_its_cached_view()
    {
        var navigables = TwoNavigablesWithOwnViewModels(out var first, out var second);
        var sut = TestNavigableViewModel.Create(CreateRouter(), navigables, initialize: true);
        var firstView = sut.CurrentView;
        sut.SelectedNavigable = second;

        sut.GoBack();

        Assert.Same(first, sut.SelectedNavigable);
        Assert.Same(firstView, sut.CurrentView);
    }

    [Fact]
    public void GoBack_raises_RaiseCanExecutesChanged()
    {
        var navigables = TwoNavigablesWithOwnViewModels(out _, out var second);
        var sut = TestNavigableViewModel.Create(CreateRouter(), navigables, initialize: true);
        sut.SelectedNavigable = second;
        var countBeforeGoBack = sut.RaiseCanExecutesChangedCallCount;

        sut.GoBack();

        Assert.True(sut.RaiseCanExecutesChangedCallCount > countBeforeGoBack);
    }
}
