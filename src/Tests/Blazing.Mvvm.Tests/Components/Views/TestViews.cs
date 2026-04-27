using Blazing.Mvvm.Components;
using Blazing.Mvvm.Tests.Components.ViewModels;
using Microsoft.AspNetCore.Components;

namespace Blazing.Mvvm.Tests.Components.Views;

[ViewModelKey(nameof(SingletonTestViewModel))]
[Route("/singleton")]
internal class SingletonTestView : MvvmComponentBase<ISingletonTestViewModel>
{ }

[ViewModelKey(nameof(SingletonKeyedTestViewModel))]
[Route("/singleton-keyed")]
internal class SingletonKeyedTestView : MvvmComponentBase<SingletonKeyedTestViewModel>
{ }
