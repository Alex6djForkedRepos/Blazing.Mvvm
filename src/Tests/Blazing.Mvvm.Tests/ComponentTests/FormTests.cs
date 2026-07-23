using AngleSharp.Dom;
using Bunit;
using Blazing.Mvvm.Tests.Components.ViewModels;
using Blazing.Mvvm.Tests.Components.Views;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blazing.Mvvm.Tests.ComponentTests;

public class FormTests : ComponentTestBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FormTests"/> class and registers the <see cref="EditContactViewModel"/>.
    /// </summary>
    public FormTests()
    {
        Services.AddSingleton<EditContactViewModel>();
    }

    private const string SubmitButtonSelector = "button[type=\"submit\"]";
    private const string ClearButtonSelector = "button[type=\"button\"]";

    private const string NameInputAriaLabel = "name";
    private const string EmailInputAriaLabel = "email";
    private const string PhoneNumberInputAriaLabel = "phone number";

    private const string ErrorSummaryAriaLabel = "validation summary";
    private const string NameValidationMessageAriaLabel = "name validation message";
    private const string EmailValidationMessageAriaLabel = "email validation message";
    private const string PhoneNumberValidationMessageAriaLabel = "phone number validation message";

    /// <summary>
    /// Verifies that when the name input is populated, the Name property in the view model is updated accordingly.
    /// </summary>
    [Fact]
    public void GivenNameInputHasNoValue_WhenNameInputIsPopulatedWithValue_ThenNamePropertyShouldBeValue()
    {
        // Arrange
        const string expectedName = "Lorem Ipsum";

        var cut = Render<Form>();
        var nameInput = cut.FindByLabelText(NameInputAriaLabel);
        var cutViewModel = GetViewModel<EditContactViewModel>();

        // Act
        nameInput.Change(new ChangeEventArgs { Value = expectedName });

        // Assert
        nameInput.GetAttribute(AttributeNames.Value).ShouldBe(expectedName);
        cutViewModel.Contact.Name.ShouldBe(expectedName);
    }

    /// <summary>
    /// Verifies that when the email input is populated, the Email property in the view model is updated accordingly.
    /// </summary>
    [Fact]
    public void GivenEmailInputHasNoValue_WhenEmailInputIsPopulatedWithValue_ThenEmailPropertyShouldBeValue()
    {
        // Arrange
        const string expectedEmail = "lorem@ipsum.io";

        var cut = Render<Form>();
        var emailInput = cut.FindByLabelText(EmailInputAriaLabel);
        var cutViewModel = GetViewModel<EditContactViewModel>();

        // Act
        emailInput.Change(new ChangeEventArgs { Value = expectedEmail });

        // Assert
        emailInput.GetAttribute(AttributeNames.Value).ShouldBe(expectedEmail);
        cutViewModel.Contact.Email.ShouldBe(expectedEmail);
    }

    /// <summary>
    /// Verifies that when the phone number input is populated, the PhoneNumber property in the view model is updated accordingly.
    /// </summary>
    [Fact]
    public void GivenPhoneNumberInputHasNoValue_WhenPhoneNumberInputIsPopulatedWithValue_ThenPhoneNumberPropertyShouldBeValue()
    {
        // Arrange
        const string expectedPhoneNumber = "+12345678";

        var cut = Render<Form>();
        var phoneNumberInput = cut.FindByLabelText(PhoneNumberInputAriaLabel);
        var cutViewModel = GetViewModel<EditContactViewModel>();

        // Act
        phoneNumberInput.Change(new ChangeEventArgs { Value = expectedPhoneNumber });

        // Assert
        phoneNumberInput.GetAttribute(AttributeNames.Value).ShouldBe(expectedPhoneNumber);
        cutViewModel.Contact.PhoneNumber.ShouldBe(expectedPhoneNumber);
    }

    /// <summary>
    /// Verifies that when the Name property is set in the view model, the name input is updated accordingly.
    /// </summary>
    [Fact]
    public void GivenNameInputHasNoValue_WhenNamePropertyIsSetWithValue_ThenNameInputShouldHaveValue()
    {
        // Arrange
        const string expectedName = "Lorem Ipsum";

        var cut = Render<Form>();
        var cutViewModel = GetViewModel<EditContactViewModel>();

        // Act
        cutViewModel.Contact.Name = expectedName;

        // Assert
        cut.FindByLabelText(NameInputAriaLabel).GetAttribute(AttributeNames.Value).ShouldBe(expectedName);
    }

    /// <summary>
    /// Verifies that when the Email property is set in the view model, the email input is updated accordingly.
    /// </summary>
    [Fact]
    public void GivenEmailInputHasNoValue_WhenEmailPropertyIsSetWithValue_ThenEmailInputShouldHaveValue()
    {
        // Arrange
        const string expectedEmail = "lorem@ipsum.io";

        var cut = Render<Form>();
        var cutViewModel = GetViewModel<EditContactViewModel>();

        // Act
        cutViewModel.Contact.Email = expectedEmail;

        // Assert
        cut.FindByLabelText(EmailInputAriaLabel).GetAttribute(AttributeNames.Value).ShouldBe(expectedEmail);
    }

    /// <summary>
    /// Verifies that when the PhoneNumber property is set in the view model, the phone number input is updated accordingly.
    /// </summary>
    [Fact]
    public void GivenPhoneNumberInputHasNoValue_WhenPhoneNumberPropertyIsSetWithValue_ThenPhoneNumberInputShouldHaveValue()
    {
        // Arrange
        const string expectedPhoneNumber = "+12345678";

        var cut = Render<Form>();
        var cutViewModel = GetViewModel<EditContactViewModel>();

        // Act
        cutViewModel.Contact.PhoneNumber = expectedPhoneNumber;

        // Assert
        cut.FindByLabelText(PhoneNumberInputAriaLabel).GetAttribute(AttributeNames.Value).ShouldBe(expectedPhoneNumber);
    }

    /// <summary>
    /// Verifies that when the form is valid and the submit button is clicked, the form is submitted and the appropriate log message is written.
    /// </summary>
    [Fact]
    public void GivenFormIsValid_WhenSubmitButtonIsClicked_ThenFormShouldBeSubmitted()
    {
        // Arrange
        const string expectedLogMessage = "Form is valid and submitted!";
        Services.AddSingleton(_ => GetMock<ILogger<EditContactViewModel>>().Object);

        var cut = Render<Form>();
        cut.FindByLabelText(NameInputAriaLabel).Change(new ChangeEventArgs { Value = "Lorem Ipsum" });
        cut.FindByLabelText(EmailInputAriaLabel).Change(new ChangeEventArgs { Value = "lorem@ipsum.io" });
        cut.FindByLabelText(PhoneNumberInputAriaLabel).Change(new ChangeEventArgs { Value = "+12345678" });
        var cutViewModel = GetViewModel<EditContactViewModel>();
        var loggerMock = GetMock<ILogger<EditContactViewModel>>();

        // Act
        cut.Find(SubmitButtonSelector).Click();

        // Assert
        cutViewModel.Contact.HasErrors.ShouldBeFalse();
        loggerMock.VerifyLog(LogLevel.Information, expectedLogMessage);
    }

    /// <summary>
    /// Verifies that when form inputs are empty and the submit button is clicked, the button is disabled and error messages are displayed.
    /// </summary>
    [Fact]
    public void GivenFormInputsAreEmpty_WhenSubmitButtonIsClicked_ThenSubmitButtonIsDisabledAndErrorMessagesDisplayed()
    {
        // Arrange
        const string expectedNameErrorMsg = "The Name field is required.";
        const string expectedEmailErrorMsg = "The Email field is required.";
        const string expectedPhoneNumberErrorMsg = "The Phone Number field is required.";
        const string expectedErrorSummaryHtml = $"""

            <ul diff:ignoreAttributes>
                <li diff:ignoreAttributes>{expectedNameErrorMsg}</li>
                <li diff:ignoreAttributes>{expectedEmailErrorMsg}</li>
                <li diff:ignoreAttributes>{expectedPhoneNumberErrorMsg}</li>
            </ul>
            """;

        var cut = Render<Form>();
        var submitButton = cut.Find(SubmitButtonSelector);

        // Act
        submitButton.Click();

        // Assert
        cut.FindByLabelText(ErrorSummaryAriaLabel).MarkupMatches(expectedErrorSummaryHtml);
        cut.FindByLabelText(NameValidationMessageAriaLabel).TextContent.ShouldBe(expectedNameErrorMsg);
        cut.FindByLabelText(EmailValidationMessageAriaLabel).TextContent.ShouldBe(expectedEmailErrorMsg);
        cut.FindByLabelText(PhoneNumberValidationMessageAriaLabel).TextContent.ShouldBe(expectedPhoneNumberErrorMsg);
        submitButton.IsDisabled().ShouldBeTrue();
    }

    /// <summary>
    /// Verifies that when invalid inputs are entered, the submit button is disabled and error messages are displayed.
    /// </summary>
    [Fact]
    public void GivenFormInputsAreEmpty_WhenInvalidInputsEntered_ThenSubmitButtonIsDisabledAndErrorMessagesDisplayed()
    {
        // Arrange
        const string expectedNameErrorMsg = "The Name field must have a length between 2 and 100.";
        const string expectedEmailErrorMsg = "The Email field is not a valid e-mail address.";
        const string expectedPhoneNumberErrorMsg = "The Phone Number field is not a valid phone number.";
        const string expectedErrorSummaryHtml = $"""

            <ul diff:ignoreAttributes>
                <li diff:ignoreAttributes>{expectedNameErrorMsg}</li>
                <li diff:ignoreAttributes>{expectedEmailErrorMsg}</li>
                <li diff:ignoreAttributes>{expectedPhoneNumberErrorMsg}</li>
            </ul>
            """;

        var cut = Render<Form>();

        // Act
        cut.FindByLabelText(NameInputAriaLabel).Change(new ChangeEventArgs { Value = "L" });
        cut.FindByLabelText(EmailInputAriaLabel).Change(new ChangeEventArgs { Value = "l" });
        cut.FindByLabelText(PhoneNumberInputAriaLabel).Change(new ChangeEventArgs { Value = "+" });

        // Assert
        cut.FindByLabelText(ErrorSummaryAriaLabel).MarkupMatches(expectedErrorSummaryHtml);
        cut.FindByLabelText(NameValidationMessageAriaLabel).TextContent.ShouldBe(expectedNameErrorMsg);
        cut.FindByLabelText(EmailValidationMessageAriaLabel).TextContent.ShouldBe(expectedEmailErrorMsg);
        cut.FindByLabelText(PhoneNumberValidationMessageAriaLabel).TextContent.ShouldBe(expectedPhoneNumberErrorMsg);
        cut.Find(SubmitButtonSelector).IsDisabled().ShouldBeTrue();
    }

    /// <summary>
    /// Verifies that when the clear button is clicked, the form is cleared and the submit button is enabled.
    /// </summary>
    [Fact]
    public void GivenFormIsInvalid_WhenClearButtonIsClicked_ThenFormShouldBeCleared()
    {
        // Arrange
        var cut = Render<Form>();

        // Act
        cut.Find(ClearButtonSelector).Click();

        // Assert
        cut.FindByLabelText(NameInputAriaLabel).GetAttribute(AttributeNames.Value).ShouldBeNull();
        cut.FindByLabelText(EmailInputAriaLabel).GetAttribute(AttributeNames.Value).ShouldBeNull();
        cut.FindByLabelText(PhoneNumberInputAriaLabel).GetAttribute(AttributeNames.Value).ShouldBeNull();
        cut.Find(SubmitButtonSelector).IsEnabled().ShouldBeTrue();
    }
}
