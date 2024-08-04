﻿using AngleSharp.Dom;
using Blazing.Mvvm.Sample.WebApp.Client.Pages;
using Blazing.Mvvm.Sample.WebApp.Client.ViewModels;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Blazing.Mvvm.Tests.ComponentTests;

public class FormTests : ComponentTestBase
{
    private const string SubmitButtonSelector = "button[type=\"submit\"]";
    private const string ClearButtonSelector = "button[type=\"button\"]";

    private const string NameInputAriaLabel = "name";
    private const string EmailInputAriaLabel = "email";
    private const string PhoneNumberInputAriaLabel = "phone number";

    private const string ErrorSummaryAriaLabel = "validation summary";
    private const string NameValidationMessageAriaLabel = "name validation message";
    private const string EmailValidationMessageAriaLabel = "email validation message";
    private const string PhoneNumberValidationMessageAriaLabel = "phone number validation message";

    [Fact]
    public void GivenNameInputHasNoValue_WhenNameInputIsPopulatedWithValue_ThenNamePropertyShouldBeValue()
    {
        // Arrange
        const string expectedName = "Lorem Ipsum";

        var cut = RenderComponent<Form>();
        var nameInput = cut.FindByLabelText(NameInputAriaLabel);
        var cutViewModel = GetViewModel<EditContactViewModel>();

        // Act
        nameInput.Change(new ChangeEventArgs { Value = expectedName });

        // Assert
        using var _ = new AssertionScope();
        nameInput.GetAttribute(AttributeNames.Value).Should().Be(expectedName);
        cutViewModel.Contact.Name.Should().Be(expectedName);
    }

    [Fact]
    public void GivenEmailInputHasNoValue_WhenEmailInputIsPopulatedWithValue_ThenEmailPropertyShouldBeValue()
    {
        // Arrange
        const string expectedEmail = "lorem@ipsum.io";

        var cut = RenderComponent<Form>();
        var emailInput = cut.FindByLabelText(EmailInputAriaLabel);
        var cutViewModel = GetViewModel<EditContactViewModel>();

        // Act
        emailInput.Change(new ChangeEventArgs { Value = expectedEmail });

        // Assert
        using var _ = new AssertionScope();
        emailInput.GetAttribute(AttributeNames.Value).Should().Be(expectedEmail);
        cutViewModel.Contact.Email.Should().Be(expectedEmail);
    }

    [Fact]
    public void GivenPhoneNumberInputHasNoValue_WhenPhoneNumberInputIsPopulatedWithValue_ThenPhoneNumberPropertyShouldBeValue()
    {
        // Arrange
        const string expectedPhoneNumber = "+12345678";

        var cut = RenderComponent<Form>();
        var phoneNumberInput = cut.FindByLabelText(PhoneNumberInputAriaLabel);
        var cutViewModel = GetViewModel<EditContactViewModel>();

        // Act
        phoneNumberInput.Change(new ChangeEventArgs { Value = expectedPhoneNumber });

        // Assert
        using var _ = new AssertionScope();
        phoneNumberInput.GetAttribute(AttributeNames.Value).Should().Be(expectedPhoneNumber);
        cutViewModel.Contact.PhoneNumber.Should().Be(expectedPhoneNumber);
    }

    [Fact]
    public void GivenNameInputHasNoValue_WhenNamePropertyIsSetWithValue_ThenNameInputShouldHaveValue()
    {
        // Arrange
        const string expectedName = "Lorem Ipsum";

        var cut = RenderComponent<Form>();
        var cutViewModel = GetViewModel<EditContactViewModel>();

        // Act
        cutViewModel.Contact.Name = expectedName;

        // Assert
        cut.FindByLabelText(NameInputAriaLabel).GetAttribute(AttributeNames.Value).Should().Be(expectedName);
    }

    [Fact]
    public void GivenEmailInputHasNoValue_WhenEmailPropertyIsSetWithValue_ThenEmailInputShouldHaveValue()
    {
        // Arrange
        const string expectedEmail = "lorem@ipsum.io";

        var cut = RenderComponent<Form>();
        var cutViewModel = GetViewModel<EditContactViewModel>();

        // Act
        cutViewModel.Contact.Email = expectedEmail;

        // Assert
        cut.FindByLabelText(EmailInputAriaLabel).GetAttribute(AttributeNames.Value).Should().Be(expectedEmail);
    }

    [Fact]
    public void GivenPhoneNumberInputHasNoValue_WhenPhoneNumberPropertyIsSetWithValue_ThenPhoneNumberInputShouldHaveValue()
    {
        // Arrange
        const string expectedPhoneNumber = "+12345678";

        var cut = RenderComponent<Form>();
        var cutViewModel = GetViewModel<EditContactViewModel>();

        // Act
        cutViewModel.Contact.PhoneNumber = expectedPhoneNumber;

        // Assert
        cut.FindByLabelText(PhoneNumberInputAriaLabel).GetAttribute(AttributeNames.Value).Should().Be(expectedPhoneNumber);
    }

    [Fact]
    public void GivenFormIsValid_WhenSubmitButtonIsClicked_ThenFormShouldBeSubmitted()
    {
        // Arrange
        const string expectedLogMessage = "Form is valid and submitted!";

        var cut = RenderComponent<Form>();
        cut.FindByLabelText(NameInputAriaLabel).Change(new ChangeEventArgs { Value = "Lorem Ipsum" });
        cut.FindByLabelText(EmailInputAriaLabel).Change(new ChangeEventArgs { Value = "lorem@ipsum.io" });
        cut.FindByLabelText(PhoneNumberInputAriaLabel).Change(new ChangeEventArgs { Value = "+12345678" });
        var cutViewModel = GetViewModel<EditContactViewModel>();
        var loggerMock = GetMock<ILogger<EditContactViewModel>>();

        // Act
        cut.Find(SubmitButtonSelector).Click();

        // Assert
        using var _ = new AssertionScope();
        cutViewModel.Contact.HasErrors.Should().BeFalse();
        loggerMock.VerifyLog(LogLevel.Information, expectedLogMessage);
    }

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

        var cut = RenderComponent<Form>();
        var submitButton = cut.Find(SubmitButtonSelector);

        // Act
        submitButton.Click();

        // Assert
        using var _ = new AssertionScope();
        cut.FindByLabelText(ErrorSummaryAriaLabel).MarkupMatches(expectedErrorSummaryHtml);
        cut.FindByLabelText(NameValidationMessageAriaLabel).TextContent.Should().Be(expectedNameErrorMsg);
        cut.FindByLabelText(EmailValidationMessageAriaLabel).TextContent.Should().Be(expectedEmailErrorMsg);
        cut.FindByLabelText(PhoneNumberValidationMessageAriaLabel).TextContent.Should().Be(expectedPhoneNumberErrorMsg);
        submitButton.IsDisabled().Should().BeTrue();
    }

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

        var cut = RenderComponent<Form>();

        // Act
        cut.FindByLabelText(NameInputAriaLabel).Change(new ChangeEventArgs { Value = "L" });
        cut.FindByLabelText(EmailInputAriaLabel).Change(new ChangeEventArgs { Value = "l" });
        cut.FindByLabelText(PhoneNumberInputAriaLabel).Change(new ChangeEventArgs { Value = "+" });

        // Assert
        using var _ = new AssertionScope();
        cut.FindByLabelText(ErrorSummaryAriaLabel).MarkupMatches(expectedErrorSummaryHtml);
        cut.FindByLabelText(NameValidationMessageAriaLabel).TextContent.Should().Be(expectedNameErrorMsg);
        cut.FindByLabelText(EmailValidationMessageAriaLabel).TextContent.Should().Be(expectedEmailErrorMsg);
        cut.FindByLabelText(PhoneNumberValidationMessageAriaLabel).TextContent.Should().Be(expectedPhoneNumberErrorMsg);
        cut.Find(SubmitButtonSelector).IsDisabled().Should().BeTrue();
    }

    [Fact]
    public void GivenFormIsInvalid_WhenClearButtonIsClicked_ThenFormShouldBeCleared()
    {
        // Arrange
        var cut = RenderComponent<Form>();

        // Act
        cut.Find(ClearButtonSelector).Click();

        // Assert
        using var _ = new AssertionScope();
        cut.FindByLabelText(NameInputAriaLabel).GetAttribute(AttributeNames.Value).Should().BeNull();
        cut.FindByLabelText(EmailInputAriaLabel).GetAttribute(AttributeNames.Value).Should().BeNull();
        cut.FindByLabelText(PhoneNumberInputAriaLabel).GetAttribute(AttributeNames.Value).Should().BeNull();
        cut.Find(SubmitButtonSelector).IsEnabled().Should().BeTrue();
    }
}
