using Avae.ViewModels.Tests.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Avae.ViewModels.Tests;

public class RouterTests
{
    private static Router CreateRouter(out IIocConfiguration configuration, Action<IServiceCollection>? configure = null)
    {
        var services = new ServiceCollection();
        var config = Substitute.For<IIocConfiguration>();
        services.AddSingleton(config);
        configure?.Invoke(services);
        configuration = config;
        return new Router(services.BuildServiceProvider());
    }

    [Fact]
    public void New_router_has_no_history_and_cannot_navigate()
    {
        var router = CreateRouter(out _);

        Assert.False(router.CanGoBack);
        Assert.False(router.CanGoForward);
        Assert.Null(router.Current);
    }

    [Fact]
    public void AddHistory_makes_the_item_current()
    {
        var router = CreateRouter(out _);
        var vm1 = new FakeViewModel();

        router.AddHistory(vm1);

        Assert.Same(vm1, router.Current);
        Assert.False(router.CanGoBack);
        Assert.False(router.CanGoForward);
    }

    [Fact]
    public void AddHistory_of_a_second_item_enables_going_back()
    {
        var router = CreateRouter(out _);
        router.AddHistory(new FakeViewModel());
        var vm2 = new FakeViewModel();

        router.AddHistory(vm2);

        Assert.Same(vm2, router.Current);
        Assert.True(router.CanGoBack);
        Assert.False(router.CanGoForward);
    }

    [Fact]
    public void Back_moves_to_the_previous_item_and_raises_CurrentViewModelChanged()
    {
        var router = CreateRouter(out _);
        var vm1 = new FakeViewModel();
        var vm2 = new FakeViewModel();
        router.AddHistory(vm1);
        router.AddHistory(vm2);

        IViewModelBase? raised = null;
        router.CurrentViewModelChanged += vm => raised = vm;

        var result = router.Back();

        Assert.Same(vm1, result);
        Assert.Same(vm1, router.Current);
        Assert.Same(vm1, raised);
        Assert.True(router.CanGoForward);
        Assert.False(router.CanGoBack);
    }

    [Fact]
    public void Back_returns_null_and_raises_nothing_when_there_is_nothing_to_go_back_to()
    {
        var router = CreateRouter(out _);
        router.AddHistory(new FakeViewModel());

        var raised = false;
        router.CurrentViewModelChanged += _ => raised = true;

        var result = router.Back();

        Assert.Null(result);
        Assert.False(raised);
    }

    [Fact]
    public void Forward_moves_to_the_next_item_after_going_back()
    {
        var router = CreateRouter(out _);
        var vm1 = new FakeViewModel();
        var vm2 = new FakeViewModel();
        router.AddHistory(vm1);
        router.AddHistory(vm2);
        router.Back();

        var result = router.Forward();

        Assert.Same(vm2, result);
        Assert.Same(vm2, router.Current);
        Assert.False(router.CanGoForward);
        Assert.True(router.CanGoBack);
    }

    [Fact]
    public void Forward_returns_null_when_there_is_nothing_to_go_forward_to()
    {
        var router = CreateRouter(out _);
        router.AddHistory(new FakeViewModel());

        Assert.Null(router.Forward());
    }

    [Fact]
    public void AddHistory_after_going_back_discards_the_stale_forward_history()
    {
        var router = CreateRouter(out _);
        var vm1 = new FakeViewModel();
        var vm2 = new FakeViewModel();
        var vm3 = new FakeViewModel();
        router.AddHistory(vm1);
        router.AddHistory(vm2);
        router.Back();

        router.AddHistory(vm3);

        Assert.Same(vm3, router.Current);
        Assert.False(router.CanGoForward);
        Assert.True(router.CanGoBack);

        router.Back();
        Assert.Same(vm1, router.Current);
    }

    [Fact]
    public void AddHistory_trims_the_oldest_entry_once_the_max_history_size_is_exceeded()
    {
        var router = CreateRouter(out _);
        var items = Enumerable.Range(0, 25).Select(_ => new FakeViewModel()).ToArray();

        foreach (var item in items)
            router.AddHistory(item);

        Assert.Same(items[^1], router.Current);

        var backCount = 0;
        while (router.CanGoBack)
        {
            router.Back();
            backCount++;
        }

        // 25 additions into a 20-item ring buffer trims the oldest 5, leaving 20 entries
        // (indices 5..24), so going all the way back takes 19 steps and lands on items[5].
        Assert.Equal(19, backCount);
        Assert.Same(items[5], router.Current);
    }

    [Fact]
    public void EraseHistory_clears_current_and_navigation_state()
    {
        var router = CreateRouter(out _);
        router.AddHistory(new FakeViewModel());
        router.AddHistory(new FakeViewModel());

        router.EraseHistory();

        Assert.Null(router.Current);
        Assert.False(router.CanGoBack);
        Assert.False(router.CanGoForward);
    }

    [Fact]
    public void GoTo_with_an_existing_viewmodel_resolves_the_view_and_assigns_context()
    {
        var viewFor = new FakeViewFor();
        var router = CreateRouter(out var configuration);
        configuration.GetContextFor(nameof(FakeViewModel), Arg.Any<NavigableContext>()).Returns(viewFor);

        var viewModel = new FakeViewModel();
        var result = router.GoTo(viewModel);

        Assert.Same(viewFor, result);
        Assert.Same(viewModel, viewFor.Context);
        Assert.Same(viewModel, router.Current);
    }

    [Fact]
    public void GoTo_raises_CurrentViewModelChanged_with_the_new_viewmodel()
    {
        var router = CreateRouter(out var configuration);
        configuration.GetContextFor(Arg.Any<string>(), Arg.Any<NavigableContext>()).Returns(new FakeViewFor());

        IViewModelBase? raised = null;
        router.CurrentViewModelChanged += vm => raised = vm;

        var viewModel = new FakeViewModel();
        router.GoTo(viewModel);

        Assert.Same(viewModel, raised);
    }

    [Fact]
    public void GoTo_throws_when_no_view_is_registered_for_the_viewmodel()
    {
        var router = CreateRouter(out var configuration);
        configuration.GetContextFor(Arg.Any<string>(), Arg.Any<NavigableContext>()).Returns((IViewFor?)null);

        Assert.Throws<NotImplementedException>(() => router.GoTo(new FakeViewModel()));
    }

    [Fact]
    public void GoTo_by_type_resolves_the_viewmodel_from_the_service_provider()
    {
        var viewFor = new FakeViewFor();
        var router = CreateRouter(out var configuration, services => services.AddTransient<FakeViewModel>());
        configuration.GetContextFor(nameof(FakeViewModel), Arg.Any<NavigableContext>()).Returns(viewFor);

        var result = router.GoTo(typeof(FakeViewModel), out var viewModel);

        Assert.IsType<FakeViewModel>(viewModel);
        Assert.Same(viewFor, result);
        Assert.Same(viewModel, router.Current);
    }

    [Fact]
    public void GoTo_by_type_without_an_out_parameter_still_navigates()
    {
        var viewFor = new FakeViewFor();
        var router = CreateRouter(out var configuration, services => services.AddTransient<FakeViewModel>());
        configuration.GetContextFor(nameof(FakeViewModel), Arg.Any<NavigableContext>()).Returns(viewFor);

        var result = router.GoTo(typeof(FakeViewModel));

        Assert.Same(viewFor, result);
        Assert.IsType<FakeViewModel>(router.Current);
    }
}
