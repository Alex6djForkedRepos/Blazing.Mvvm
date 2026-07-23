using Blazing.Mvvm.Tests.Infrastructure.Fakes.ViewModels;
using Blazing.Mvvm.Tests.Infrastructure.Fakes.Views;
using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace Blazing.Mvvm.Tests.ComponentTests;

/// <summary>
/// Component tests for AsyncRelayCommand integration with Blazor components.
/// Tests verify that button states update correctly when IsRunning changes (GitHub Issue #65).
/// </summary>
public class AsyncRelayCommandComponentTests : ComponentTestBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommandComponentTests"/> class
    /// and registers the test ViewModel.
    /// </summary>
    public AsyncRelayCommandComponentTests()
    {
        Services.AddSingleton(_ => CreateInstance<AsyncCommandTestViewModel>(true));
    }

    /// <summary>
    /// Verifies that button becomes disabled while command is executing and re-enabled when complete.
    /// This is the core scenario from GitHub Issue #65.
    /// </summary>
    [Fact]
    public async Task WhenCommandExecutes_ThenButtonDisablesAndReEnables()
    {
        // Arrange
        var cut = Render<AsyncCommandTestView>();
        var button = cut.Find("#load-data");
        var viewModel = GetViewModel<AsyncCommandTestViewModel>();

        // Assert - Initial state
        button.HasAttribute("disabled").ShouldBeFalse("Button should be enabled initially");

        // Act
        button.Click();

        // Assert - During execution
        cut.WaitForAssertion(() =>
            button.HasAttribute("disabled").ShouldBeTrue("Button should be disabled while command is running"));

        // Assert - After execution
        await viewModel.LoadDataCommand.ExecutionTask.ShouldNotBeNull();

        cut.WaitForAssertion(() =>
            button.HasAttribute("disabled").ShouldBeFalse("Button should be re-enabled after command completes"));
    }

    /// <summary>
    /// Verifies that IsRunning state is reflected in the UI.
    /// </summary>
    [Fact]
    public async Task WhenCommandExecutes_ThenIsRunningStateIsDisplayed()
    {
        // Arrange
        var cut = Render<AsyncCommandTestView>();
        var viewModel = GetViewModel<AsyncCommandTestViewModel>();

        // Act
        cut.Find("#load-data").Click();

        // Assert - During execution
        cut.WaitForAssertion(() =>
            cut.Find("#is-running").TextContent.ShouldContain(
                "True",
                customMessage: "IsRunning should be true while executing"));

        // Assert - After execution
        await viewModel.LoadDataCommand.ExecutionTask.ShouldNotBeNull();

        cut.WaitForAssertion(() =>
            cut.Find("#is-running").TextContent.ShouldContain(
                "False",
                customMessage: "IsRunning should be false after completion"));
    }

    /// <summary>
    /// Verifies that result updates are shown in the UI after command completion.
    /// </summary>
    [Fact]
    public async Task WhenCommandCompletes_ThenResultIsUpdatedInUI()
    {
        // Arrange
        var cut = Render<AsyncCommandTestView>();
        var viewModel = GetViewModel<AsyncCommandTestViewModel>();

        // Act
        cut.Find("#load-data").Click();
        await viewModel.LoadDataCommand.ExecutionTask.ShouldNotBeNull();

        // Assert
        cut.WaitForAssertion(() =>
        {
            cut.Find("#result").TextContent.ShouldContain("Data loaded");
            cut.Find("#execution-count").TextContent.ShouldContain("1");
        });
    }

    /// <summary>
    /// Verifies that parameterized commands work correctly.
    /// </summary>
    [Fact]
    public async Task WhenParameterizedCommandExecutes_ThenParameterIsProcessed()
    {
        // Arrange
        var cut = Render<AsyncCommandTestView>();
        var viewModel = GetViewModel<AsyncCommandTestViewModel>();

        // Act
        cut.Find("#process-data").Click();
        await viewModel.ProcessCommand.ExecutionTask.ShouldNotBeNull();

        // Assert
        cut.WaitForAssertion(() =>
            cut.Find("#result").TextContent.ShouldContain("Processed: TestInput"));
    }

    /// <summary>
    /// Verifies that multiple sequential command executions work correctly.
    /// </summary>
    [Fact]
    public async Task WhenCommandExecutedMultipleTimes_ThenEachExecutionUpdatesUI()
    {
        // Arrange
        var cut = Render<AsyncCommandTestView>();
        var button = cut.Find("#fast-command");
        var viewModel = GetViewModel<AsyncCommandTestViewModel>();

        // Act - Execute three times
        for (int i = 0; i < 3; i++)
        {
            button.Click();
            await viewModel.FastCommand.ExecutionTask.ShouldNotBeNull();
            cut.WaitForAssertion(() => button.HasAttribute("disabled").ShouldBeFalse());
        }

        // Assert
        cut.WaitForAssertion(() =>
        {
            cut.Find("#execution-count").TextContent.ShouldContain("3");
            cut.Find("#result").TextContent.ShouldContain("Fast command completed");
        });
    }

    /// <summary>
    /// Verifies that CanExecute state is properly displayed in the UI.
    /// </summary>
    [Fact]
    public async Task WhenCommandIsRunning_ThenCanExecuteStateIsDisplayed()
    {
        // Arrange
        var cut = Render<AsyncCommandTestView>();
        var viewModel = GetViewModel<AsyncCommandTestViewModel>();

        // Act
        cut.Find("#slow-command").Click();

        // Assert - During execution
        cut.WaitForAssertion(() =>
        {
            viewModel.SlowCommand.IsRunning.ShouldBeTrue();
            cut.Find("#load-data-enabled").TextContent.ShouldContain("True");
        });

        // Assert - After execution
        await viewModel.SlowCommand.ExecutionTask.ShouldNotBeNull();

        cut.WaitForAssertion(() =>
            cut.Find("#load-data-enabled").TextContent.ShouldContain("True"));
    }

    /// <summary>
    /// Verifies that UI updates don't block command execution.
    /// </summary>
    [Fact]
    public async Task WhenCommandExecutes_ThenUIUpdatesAreNonBlocking()
    {
        // Arrange
        var cut = Render<AsyncCommandTestView>();
        var viewModel = GetViewModel<AsyncCommandTestViewModel>();

        // Act
        cut.Find("#load-data").Click();
        cut.WaitForState(() => viewModel.LoadDataCommand.IsRunning);

        Action accessViewModel = () =>
        {
            _ = viewModel.Result;
            _ = viewModel.IsProcessing;
            _ = viewModel.ExecutionCount;
        };

        // Assert
        Should.NotThrow(accessViewModel, "ViewModel should be accessible while command is running");
        await viewModel.LoadDataCommand.ExecutionTask.ShouldNotBeNull();
    }

    /// <summary>
    /// Verifies that different commands can have different IsRunning states.
    /// </summary>
    [Fact]
    public async Task WhenMultipleCommandsExist_ThenEachHasIndependentIsRunningState()
    {
        // Arrange
        var cut = Render<AsyncCommandTestView>();
        var viewModel = GetViewModel<AsyncCommandTestViewModel>();

        // Act
        cut.Find("#slow-command").Click();
        cut.WaitForState(() => viewModel.SlowCommand.IsRunning);

        // Assert
        viewModel.SlowCommand.IsRunning.ShouldBeTrue("Slow command should be running");
        viewModel.LoadDataCommand.IsRunning.ShouldBeFalse("Load command should not be running");
        await viewModel.SlowCommand.ExecutionTask.ShouldNotBeNull();
    }

    /// <summary>
    /// Verifies that the component lifecycle works with async commands.
    /// </summary>
    [Fact]
    public async Task WhenComponentRendersAndDisposed_ThenNoExceptionsOccur()
    {
        // Arrange
        var cut = Render<AsyncCommandTestView>();
        var viewModel = GetViewModel<AsyncCommandTestViewModel>();

        // Act
        cut.Find("#slow-command").Click();
        cut.WaitForState(() => viewModel.SlowCommand.IsRunning);

        Action disposeAction = () => viewModel.Dispose();

        // Assert
        Should.NotThrow(disposeAction);
        await viewModel.SlowCommand.ExecutionTask.ShouldNotBeNull();
    }

    /// <summary>
    /// Verifies that rapid button clicks while command is running don't queue multiple executions.
    /// Due to default non-concurrent behavior, subsequent clicks should be ignored while executing.
    /// </summary>
    [Fact]
    public async Task WhenButtonClickedRapidly_ThenButtonIsDisabledWhileRunning()
    {
        // Arrange
        var cut = Render<AsyncCommandTestView>();
        var button = cut.Find("#slow-command");
        var viewModel = GetViewModel<AsyncCommandTestViewModel>();

        // Act
        button.Click();

        // Assert - During execution
        cut.WaitForAssertion(() =>
        {
            button.HasAttribute("disabled").ShouldBeTrue("Button should be disabled while command is running");
            viewModel.SlowCommand.CanExecute(null).ShouldBeFalse("CanExecute should be false while command is running");
        });

        // Assert - After execution
        await viewModel.SlowCommand.ExecutionTask.ShouldNotBeNull();

        cut.WaitForAssertion(() =>
            button.HasAttribute("disabled").ShouldBeFalse("Button should be re-enabled after command completes"));
    }

    /// <summary>
    /// Verifies that button state reflects CanExecute correctly throughout execution.
    /// </summary>
    [Fact]
    public async Task WhenCommandExecutes_ThenButtonEnabledStateMatchesCanExecute()
    {
        // Arrange
        var cut = Render<AsyncCommandTestView>();
        var button = cut.Find("#load-data");
        var viewModel = GetViewModel<AsyncCommandTestViewModel>();

        // Act & Assert - Before execution
        button.HasAttribute("disabled").ShouldBeFalse();
        viewModel.LoadDataCommand.CanExecute(null).ShouldBeTrue();

        // Execute
        button.Click();

        // During execution
        cut.WaitForAssertion(() =>
        {
            button.HasAttribute("disabled").ShouldBeTrue();
            viewModel.LoadDataCommand.IsRunning.ShouldBeTrue();
            viewModel.LoadDataCommand.CanExecute(null).ShouldBeFalse();
        });

        // After execution
        await viewModel.LoadDataCommand.ExecutionTask.ShouldNotBeNull();

        cut.WaitForAssertion(() =>
        {
            button.HasAttribute("disabled").ShouldBeFalse();
            viewModel.LoadDataCommand.IsRunning.ShouldBeFalse();
            viewModel.LoadDataCommand.CanExecute(null).ShouldBeTrue();
        });
    }
}
