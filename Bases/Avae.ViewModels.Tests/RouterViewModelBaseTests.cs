using Avae.ViewModels.Tests.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Avae.ViewModels.Tests;

public class RouterViewModelBaseTests
{
    private static Router CreateRouter()
    {
        var services = new ServiceCollection();
        services.AddSingleton(Substitute.For<IIocConfiguration>());
        return new Router(services.BuildServiceProvider());
    }

    [Fact]
    public void CanGoBack_and_CanGoForward_reflect_the_router_state()
    {
        var router = CreateRouter();
        var sut = new TestRouterViewModel(router);

        Assert.False(sut.CanGoBack());
        Assert.False(sut.CanGoForward());

        router.AddHistory(new FakeViewModel());
        router.AddHistory(new FakeViewModel());

        Assert.True(sut.CanGoBack());
        Assert.False(sut.CanGoForward());
    }

    [Fact]
    public void GoBack_navigates_the_router_and_notifies_OnViewModelChanged()
    {
        var router = CreateRouter();
        var sut = new TestRouterViewModel(router);
        var vm1 = new FakeViewModel();
        var vm2 = new FakeViewModel();
        router.AddHistory(vm1);
        router.AddHistory(vm2);

        sut.GoBack();

        Assert.Same(vm1, sut.LastViewModelChanged);
        Assert.Same(vm1, router.Current);
        Assert.Equal(1, sut.RaiseCanExecutesChangedCallCount);
    }

    [Fact]
    public void GoBack_does_nothing_when_there_is_no_history_to_go_back_to()
    {
        var router = CreateRouter();
        var sut = new TestRouterViewModel(router);

        sut.GoBack();

        Assert.Null(sut.LastViewModelChanged);
        Assert.Equal(0, sut.RaiseCanExecutesChangedCallCount);
    }

    [Fact]
    public void GoForward_navigates_the_router_and_notifies_OnViewModelChanged()
    {
        var router = CreateRouter();
        var sut = new TestRouterViewModel(router);
        var vm1 = new FakeViewModel();
        var vm2 = new FakeViewModel();
        router.AddHistory(vm1);
        router.AddHistory(vm2);
        router.Back();

        sut.GoForward();

        Assert.Same(vm2, sut.LastViewModelChanged);
        Assert.Same(vm2, router.Current);
        Assert.Equal(1, sut.RaiseCanExecutesChangedCallCount);
    }

    [Fact]
    public void GoForward_does_nothing_when_there_is_no_forward_history()
    {
        var router = CreateRouter();
        var sut = new TestRouterViewModel(router);
        router.AddHistory(new FakeViewModel());

        sut.GoForward();

        Assert.Null(sut.LastViewModelChanged);
        Assert.Equal(0, sut.RaiseCanExecutesChangedCallCount);
    }
}
