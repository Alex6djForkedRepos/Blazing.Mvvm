using Blazing.Mvvm.Analyzers.Analyzers;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Blazing.Mvvm.Analyzers.Tests.CSharpAnalyzerVerifier<
    Blazing.Mvvm.Analyzers.Analyzers.MessengerRegistrationLifetimeAnalyzer>;

namespace Blazing.Mvvm.Analyzers.Tests.AnalyzerTests;

/// <summary>
/// Unit tests for <see cref="MessengerRegistrationLifetimeAnalyzer"/>
/// </summary>
public class MessengerRegistrationLifetimeAnalyzerTests
{
    [Fact]
    public async Task EmptyCode_NoDiagnostic()
    {
        const string test = "";
        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task MessengerRegisterInConstructor_WithoutUnregister_ReportsDiagnostic()
    {
        const string test = @"
using Blazing.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace TestNamespace
{
    public class TestViewModel : ViewModelBase
    {
        public {|#0:TestViewModel|}()
        {
            WeakReferenceMessenger.Default.Register<MyMessage>(this, HandleMessage);
        }

        private void HandleMessage(object recipient, MyMessage message)
        {
        }
    }

    public class MyMessage { }
}";

        var expected = new DiagnosticResult(DiagnosticDescriptors.MessengerRegistrationLeakPossible)
            .WithLocation(0)
            .WithArguments("TestViewModel");

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task MessengerRegisterInConstructor_WithUnregisterInDisposeOverride_NoDiagnostic()
    {
        const string test = @"
using Blazing.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace TestNamespace
{
    public class TestViewModel : ViewModelBase
    {
        public TestViewModel()
        {
            WeakReferenceMessenger.Default.Register<MyMessage>(this, HandleMessage);
        }

        private void HandleMessage(object recipient, MyMessage message)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                WeakReferenceMessenger.Default.Unregister<MyMessage>(this);
            }

            base.Dispose(disposing);
        }
    }

    public class MyMessage { }
}";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task RecipientViewModelBase_WithOnActivated_NoDiagnostic()
    {
        const string test = @"
using Blazing.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace TestNamespace
{
    public class TestViewModel : RecipientViewModelBase
    {
        protected override void OnActivated()
        {
            Messenger.Register<MyMessage>(this, HandleMessage);
        }

        private void HandleMessage(object recipient, MyMessage message)
        {
        }
    }

    public class MyMessage { }
}";

        // OnActivated pattern handles cleanup automatically
        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task MessengerRegisterInMethod_WithoutUnregister_ReportsDiagnostic()
    {
        const string test = @"
using Blazing.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace TestNamespace
{
    public class TestViewModel : ViewModelBase
    {
        public void {|#0:Subscribe|}()
        {
            WeakReferenceMessenger.Default.Register<MyMessage>(this, HandleMessage);
        }

        private void HandleMessage(object recipient, MyMessage message)
        {
        }
    }

    public class MyMessage { }
}";

        var expected = new DiagnosticResult(DiagnosticDescriptors.MessengerRegistrationLeakPossible)
            .WithLocation(0)
            .WithArguments("Subscribe");

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task DisposeOverrideWithoutUnregister_StillReportsDiagnostic()
    {
        const string test = @"
using Blazing.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace TestNamespace
{
    public class TestViewModel : ViewModelBase
    {
        public {|#0:TestViewModel|}()
        {
            WeakReferenceMessenger.Default.Register<MyMessage>(this, HandleMessage);
        }

        private void HandleMessage(object recipient, MyMessage message)
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }

    public class MyMessage { }
}";

        // A Dispose override without explicit unregister is still incomplete cleanup.
        var expected = new DiagnosticResult(DiagnosticDescriptors.MessengerRegistrationLeakPossible)
            .WithLocation(0)
            .WithArguments("TestViewModel");

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task MultipleMessengerRegistrations_WithUnregister_NoDiagnostic()
    {
        const string test = @"
using Blazing.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace TestNamespace
{
    public class TestViewModel : ViewModelBase
    {
        public TestViewModel()
        {
            WeakReferenceMessenger.Default.Register<FirstMessage>(this, HandleFirst);
            WeakReferenceMessenger.Default.Register<SecondMessage>(this, HandleSecond);
        }

        private void HandleFirst(object recipient, FirstMessage message) { }
        private void HandleSecond(object recipient, SecondMessage message) { }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                WeakReferenceMessenger.Default.UnregisterAll(this);
            }

            base.Dispose(disposing);
        }
    }

    public class FirstMessage { }
    public class SecondMessage { }
}";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task ViewModelWithoutMessenger_NoDiagnostic()
    {
        const string test = @"
using Blazing.Mvvm.ComponentModel;

namespace TestNamespace
{
    public class TestViewModel : ViewModelBase
    {
        public string Name { get; set; }
    }
}";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

}
