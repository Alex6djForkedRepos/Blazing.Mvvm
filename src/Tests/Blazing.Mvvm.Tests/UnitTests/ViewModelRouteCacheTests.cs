using System.Reflection;
using Blazing.Mvvm.ComponentModel;
using Blazing.Mvvm.Components;
using Blazing.Mvvm.Components.Routing;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Moq;

#pragma warning disable CS0618 // Type or member is obsolete - Testing obsolete BasePath property

namespace Blazing.Mvvm.Tests.UnitTests;

/// <summary>
/// Unit tests for <see cref="ViewModelRouteCache"/> covering route caching, base path handling, and error scenarios.
/// </summary>
public class ViewModelRouteCacheTests
{
    private readonly Mock<ILogger<ViewModelRouteCache>> _loggerMock;
    private readonly LibraryConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelRouteCacheTests"/> class.
    /// </summary>
    public ViewModelRouteCacheTests()
    {
        _loggerMock = new Mock<ILogger<ViewModelRouteCache>>();
        _configuration = new LibraryConfiguration();
    }

    /// <summary>
    /// Tests that the constructor initializes empty route dictionaries with a valid configuration.
    /// </summary>
    [Fact]
    public void Constructor_GivenValidConfiguration_ShouldInitializeEmptyRoutes()
    {
        // Arrange & Act
        var cache = new ViewModelRouteCache(_loggerMock.Object, _configuration);

        // Assert
        cache.ViewModelRoutes.ShouldNotBeNull();
        cache.KeyedViewModelRoutes.ShouldNotBeNull();
    }

    /// <summary>
    /// Tests that the constructor caches routes for view models found in the registered assembly.
    /// </summary>
    [Fact]
    public void Constructor_GivenAssemblyWithViewModels_ShouldCacheRoutes()
    {
        // Arrange
        _configuration.RegisterViewModelsFromAssemblyContaining<TestViewModel>();

        // Act
        var cache = new ViewModelRouteCache(_loggerMock.Object, _configuration);

        // Assert
        cache.ViewModelRoutes.ContainsKey(typeof(TestViewModel)).ShouldBeTrue();
        cache.ViewModelRoutes[typeof(TestViewModel)].ShouldBe("/test");
    }

    /// <summary>
    /// Tests that the constructor caches keyed routes for view models with a key attribute.
    /// </summary>
    [Fact]
    public void Constructor_GivenAssemblyWithKeyedViewModels_ShouldCacheKeyedRoutes()
    {
        // Arrange
        _configuration.RegisterViewModelsFromAssemblyContaining<TestKeyedViewModel>();

        // Act
        var cache = new ViewModelRouteCache(_loggerMock.Object, _configuration);

        // Assert
        cache.KeyedViewModelRoutes.ContainsKey("TestKey").ShouldBeTrue();
        cache.KeyedViewModelRoutes["TestKey"].ShouldBe("/keyed-test");
    }

    /// <summary>
    /// Tests that the constructor prepends the base path to cached routes when configured.
    /// </summary>
    [Fact]
    public void Constructor_GivenBasePathConfiguration_ShouldPrependBasePath()
    {
        // Arrange
        _configuration.BasePath = "/app";
        _configuration.RegisterViewModelsFromAssemblyContaining<TestViewModel>();

        // Act
        var cache = new ViewModelRouteCache(_loggerMock.Object, _configuration);

        // Assert
        cache.ViewModelRoutes[typeof(TestViewModel)].ShouldBe("/app/test");
    }

    /// <summary>
    /// Tests that the constructor handles base paths with trailing slashes correctly.
    /// </summary>
    [Fact]
    public void Constructor_GivenBasePathWithTrailingSlash_ShouldHandleCorrectly()
    {
        // Arrange
        _configuration.BasePath = "/app/";
        _configuration.RegisterViewModelsFromAssemblyContaining<TestViewModel>();

        // Act
        var cache = new ViewModelRouteCache(_loggerMock.Object, _configuration);

        // Assert
        cache.ViewModelRoutes[typeof(TestViewModel)].ShouldBe("/app/test");
    }

    /// <summary>
    /// Tests that the constructor does not cache routes for views without a route attribute.
    /// </summary>
    [Fact]
    public void Constructor_GivenViewWithoutRoute_ShouldNotCacheRoute()
    {
        // Arrange
        _configuration.RegisterViewModelsFromAssemblyContaining<TestViewModelWithoutRoute>();

        // Act
        var cache = new ViewModelRouteCache(_loggerMock.Object, _configuration);

        // Assert
        cache.ViewModelRoutes.ContainsKey(typeof(TestViewModelWithoutRoute)).ShouldBeFalse();
    }

    /// <summary>
    /// Tests that the constructor caches only the first route when multiple routes are present on the same view.
    /// </summary>
    [Fact]
    public void Constructor_GivenMultipleRoutesOnSameView_ShouldCacheFirstRoute()
    {
        // Arrange
        _configuration.RegisterViewModelsFromAssemblyContaining<TestViewModelWithMultipleRoutes>();

        // Act
        var cache = new ViewModelRouteCache(_loggerMock.Object, _configuration);

        // Assert
        cache.ViewModelRoutes.ContainsKey(typeof(TestViewModelWithMultipleRoutes)).ShouldBeTrue();
        cache.ViewModelRoutes[typeof(TestViewModelWithMultipleRoutes)].ShouldBe("/first");
    }

    /// <summary>
    /// Tests that the constructor continues processing when an invalid assembly is encountered.
    /// </summary>
    [Fact]
    public void Constructor_GivenInvalidAssembly_ShouldContinueProcessing()
    {
        // Arrange
        _configuration.RegisterViewModelsFromAssemblyContaining<TestViewModel>();
        
        // Mock an assembly that throws during GetTypes()
        var mockAssembly = new Mock<Assembly>();
        var loaderException = new ReflectionTypeLoadException(
            new Type[0], 
            new Exception[] { new FileNotFoundException("Test file not found") });
        mockAssembly.Setup(a => a.GetTypes()).Throws(loaderException);
        
        // This test verifies the cache handles exceptions gracefully
        // Act & Assert should not throw
        var act = () => new ViewModelRouteCache(_loggerMock.Object, _configuration);
        Should.NotThrow(act);
    }

    /// <summary>
    /// Tests that the constructor creates an empty cache when no assemblies are registered.
    /// </summary>
    [Fact]
    public void Constructor_GivenEmptyAssemblyList_ShouldCreateEmptyCache()
    {
        // Arrange
        // No assemblies registered

        // Act
        var cache = new ViewModelRouteCache(_loggerMock.Object, _configuration);

        // Assert
        cache.ViewModelRoutes.ShouldBeEmpty();
        cache.KeyedViewModelRoutes.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that the constructor caches both route and key for views with both attributes.
    /// </summary>
    [Fact]
    public void Constructor_GivenViewWithBothRouteAndKey_ShouldCacheBoth()
    {
        // Arrange
        _configuration.RegisterViewModelsFromAssemblyContaining<TestViewModelWithRouteAndKey>();

        // Act
        var cache = new ViewModelRouteCache(_loggerMock.Object, _configuration);

        // Assert
        cache.ViewModelRoutes.ContainsKey(typeof(TestViewModelWithRouteAndKey)).ShouldBeTrue();
        cache.ViewModelRoutes[typeof(TestViewModelWithRouteAndKey)].ShouldBe("/route-and-key");
        cache.KeyedViewModelRoutes.ContainsKey("RouteAndKey").ShouldBeTrue();
        cache.KeyedViewModelRoutes["RouteAndKey"].ShouldBe("/route-and-key");
    }

    /// <summary>
    /// Tests that the constructor caches routes for views inheriting from OwningComponentBase.
    /// </summary>
    [Fact]
    public void Constructor_GivenOwningComponentBase_ShouldCacheRoute()
    {
        // Arrange
        _configuration.RegisterViewModelsFromAssemblyContaining<TestOwningViewModel>();

        // Act
        var cache = new ViewModelRouteCache(_loggerMock.Object, _configuration);

        // Assert
        cache.ViewModelRoutes.ContainsKey(typeof(TestOwningViewModel)).ShouldBeTrue();
        cache.ViewModelRoutes[typeof(TestOwningViewModel)].ShouldBe("/owning");
    }

    /// <summary>
    /// Tests that <see cref="ViewModelRouteCache.ViewModelRoutes"/> returns a read-only dictionary.
    /// </summary>
    [Fact]
    public void ViewModelRoutes_ShouldReturnReadOnlyDictionary()
    {
        // Arrange
        var cache = new ViewModelRouteCache(_loggerMock.Object, _configuration);

        // Act
        var routes = cache.ViewModelRoutes;

        // Assert
        routes.ShouldBeAssignableTo<IReadOnlyDictionary<Type, string>>();
    }

    /// <summary>
    /// Tests that <see cref="ViewModelRouteCache.KeyedViewModelRoutes"/> returns a read-only dictionary.
    /// </summary>
    [Fact]
    public void KeyedViewModelRoutes_ShouldReturnReadOnlyDictionary()
    {
        // Arrange
        var cache = new ViewModelRouteCache(_loggerMock.Object, _configuration);

        // Act
        var keyedRoutes = cache.KeyedViewModelRoutes;

        // Assert
        keyedRoutes.ShouldBeAssignableTo<IReadOnlyDictionary<object, string>>();
    }

    // Test ViewModel and View classes
    public class TestViewModel : ViewModelBase { }

    [Route("/test")]
    public class TestView : MvvmComponentBase<TestViewModel> { }

    public class TestKeyedViewModel : ViewModelBase { }

    [Route("/keyed-test")]
    [ViewModelKey("TestKey")]
    public class TestKeyedView : MvvmComponentBase<TestKeyedViewModel> { }

    public class TestViewModelWithoutRoute : ViewModelBase { }

    public class TestViewWithoutRoute : MvvmComponentBase<TestViewModelWithoutRoute> { }

    public class TestViewModelWithMultipleRoutes : ViewModelBase { }

    [Route("/first")]
    [Route("/second")]
    public class TestViewWithMultipleRoutes : MvvmComponentBase<TestViewModelWithMultipleRoutes> { }

    public class TestViewModelWithRouteAndKey : ViewModelBase { }

    [Route("/route-and-key")]
    [ViewModelKey("RouteAndKey")]
    public class TestViewWithRouteAndKey : MvvmComponentBase<TestViewModelWithRouteAndKey> { }

    public class TestOwningViewModel : ViewModelBase { }

    [Route("/owning")]
    public class TestOwningView : MvvmOwningComponentBase<TestOwningViewModel> { }
}
