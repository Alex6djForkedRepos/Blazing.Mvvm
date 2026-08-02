using AngleSharp.Dom;
using Bunit;
using Blazing.Mvvm.Tests.Components.ViewModels;
using Blazing.Mvvm.Tests.Components.Views;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Blazing.Mvvm.Tests.ComponentTests;

public class HexTranslateTests : ComponentTestBase
{
    private const string AsciiInputAriaLabel = "ascii input";
    private const string HexInputAriaLabel = "hex input";

    private const string SendAsciiButtonSelector = "#send-ascii";
    private const string SendHexButtonSelector = "#send-hex";

    /// <summary>
    /// Initializes a new instance of the <see cref="HexTranslateTests"/> class and registers required services and view models.
    /// </summary>
    public HexTranslateTests()
    {
        Services.AddSingleton<IMessenger>(_ => WeakReferenceMessenger.Default);
        Services.AddKeyedSingleton<HexTranslateViewModel>(nameof(HexTranslateViewModel));
        Services.AddSingleton<HexEntryViewModel>();
        Services.AddSingleton<TextEntryViewModel>();
    }

    /// <summary>
    /// Verifies that the Send Ascii button is disabled when the ASCII input is invalid.
    /// </summary>
    /// <param name="input">The ASCII input value to test.</param>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenComponentRendered_WhenAsciiInputInvalid_ThenSendAsciiButtonShouldBeDisabled(string input)
    {
        // Arrange
        var cut = Render<HexTranslate>();

        // Act
        cut.FindByLabelText(AsciiInputAriaLabel).Input(new ChangeEventArgs { Value = input });

        // Assert
        cut.Find(SendAsciiButtonSelector).IsDisabled().ShouldBeTrue();
    }

    /// <summary>
    /// Verifies that the Send Hex button is disabled when the Hex input is invalid.
    /// </summary>
    /// <param name="input">The Hex input value to test.</param>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenComponentRendered_WhenHexInputInvalid_ThenSendHexButtonShouldBeDisabled(string input)
    {
        // Arrange
        var cut = Render<HexTranslate>();

        // Act
        cut.FindByLabelText(HexInputAriaLabel).Input(new ChangeEventArgs { Value = input });

        // Assert
        cut.Find(SendHexButtonSelector).IsDisabled().ShouldBeTrue();
    }

    /// <summary>
    /// Verifies that clicking the Send Ascii button sends a message to convert ASCII to Hex and updates the UI and view models.
    /// </summary>
    [Fact]
    public void GivenAsciiInputValid_WhenSendAsciiButtonClicked_ThenShouldSendConvertAsciiToHexMessage()
    {
        // Arrange
        const string input = "some text here";
        const string expectedHex = "736F6D6520746578742068657265";

        var cut = Render<HexTranslate>();
        var textEntryViewModel = GetViewModel<TextEntryViewModel>();
        var hexEntryViewModel = GetViewModel<HexEntryViewModel>();
        var asciiTextInput = cut.FindByLabelText(AsciiInputAriaLabel);

        asciiTextInput.Input(new ChangeEventArgs { Value = input });

        // Act
        cut.Find(SendAsciiButtonSelector).Click();

        // Assert
        cut.FindByLabelText(HexInputAriaLabel).GetAttribute(AttributeNames.Value).ShouldBe(expectedHex);
        hexEntryViewModel.HexText.ShouldBe(expectedHex);
        asciiTextInput.GetAttribute(AttributeNames.Value).ShouldBe(input);
        textEntryViewModel.AsciiText.ShouldBe(input);
    }

    /// <summary>
    /// Verifies that clicking the Send Hex button sends a message to convert Hex to ASCII and updates the UI and view models.
    /// </summary>
    [Fact]
    public void GivenHexInputValid_WhenSendHexButtonClicked_ThenShouldSendConvertHexToAsciiMessage()
    {
        // Arrange
        const string input = "736F6D6520746578742068657265";
        const string expectedAscii = "some text here";

        var cut = Render<HexTranslate>();
        var hexEntryViewModel = GetViewModel<HexEntryViewModel>();
        var textEntryViewModel = GetViewModel<TextEntryViewModel>();
        var hexTextInput = cut.FindByLabelText(HexInputAriaLabel);

        hexTextInput.Input(new ChangeEventArgs { Value = input });

        // Act
        cut.Find(SendHexButtonSelector).Click();

        // Assert
        cut.FindByLabelText(AsciiInputAriaLabel).GetAttribute(AttributeNames.Value).ShouldBe(expectedAscii);
        textEntryViewModel.AsciiText.ShouldBe(expectedAscii);
        hexTextInput.GetAttribute(AttributeNames.Value).ShouldBe(input);
        hexEntryViewModel.HexText.ShouldBe(input);
    }

    /// <summary>
    /// Verifies that clicking the clear button clears both input fields and resets the view models.
    /// </summary>
    [Fact]
    public void GivenInputsHaveValues_WhenClearButtonClicked_ThenShouldClearInputs()
    {
        // Arrange
        const string clearButtonSelector = "#reset-child-inputs";
        const string asciiInput = "some text here";
        const string hexInput = "736F6D6520746578742068657265";

        var cut = Render<HexTranslate>();
        var hexEntryViewModel = GetViewModel<HexEntryViewModel>();
        var textEntryViewModel = GetViewModel<TextEntryViewModel>();

        textEntryViewModel.AsciiText = asciiInput;
        hexEntryViewModel.HexText = hexInput;

        // Act
        cut.Find(clearButtonSelector).Click();

        // Assert
        textEntryViewModel.AsciiText.ShouldBeEmpty();
        hexEntryViewModel.HexText.ShouldBeEmpty();
        cut.FindByLabelText(HexInputAriaLabel).GetAttribute(AttributeNames.Value).ShouldBeEmpty();
        cut.FindByLabelText(AsciiInputAriaLabel).GetAttribute(AttributeNames.Value).ShouldBeEmpty();
    }
}
