using Blazing.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace Blazing.Mvvm.Tests.Assets.ViewModels;

public interface ITransientTestViewModel : IViewModelBase;

public interface IScopedTestViewModel : IViewModelBase;

public interface ISingletonTestViewModel : IViewModelBase;

public sealed class TestViewModel : ViewModelBase;

[ViewModelDefinition<ITransientTestViewModel>]
[ViewModelDefinition(Lifetime = ServiceLifetime.Transient)]
public sealed class TransientTestViewModel : ViewModelBase, ITransientTestViewModel;

[ViewModelDefinition(Key = nameof(TransientKeyedTestViewModel), Lifetime = ServiceLifetime.Transient)]
public sealed class TransientKeyedTestViewModel : ViewModelBase;

[ViewModelDefinition<IScopedTestViewModel>(Lifetime = ServiceLifetime.Scoped)]
[ViewModelDefinition(Lifetime = ServiceLifetime.Scoped)]
public sealed class ScopedTestViewModel : ViewModelBase, IScopedTestViewModel;

[ViewModelDefinition<ISingletonTestViewModel>(Lifetime = ServiceLifetime.Singleton)]
[ViewModelDefinition<ISingletonTestViewModel>(Key = nameof(SingletonTestViewModel), Lifetime = ServiceLifetime.Singleton)]
public sealed class SingletonTestViewModel : ViewModelBase, ISingletonTestViewModel;

[ViewModelDefinition(Lifetime = ServiceLifetime.Singleton)]
public sealed class CounterViewModel : ViewModelBase;
