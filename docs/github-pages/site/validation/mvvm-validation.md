# MVVM Validation

Blazing.Mvvm includes [`MvvmObservableValidator`](xref:Blazing.Mvvm.Components.MvvmObservableValidator), which connects Blazor `EditForm` scenarios to `ObservableValidator` from CommunityToolkit.Mvvm.

## Define a model with validation attributes

Create a type that derives from `ObservableValidator` and applies standard validation attributes:

```csharp
public class ContactInfo : ObservableValidator
{
    private string? _name;

    [Required]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "The {0} field must have a length between {2} and {1}.")]
    [RegularExpression(@"^[a-zA-Z\s'-]+$", ErrorMessage = "The {0} field contains invalid characters. Only letters, spaces, apostrophes, and hyphens are allowed.")]
    public string? Name
    {
        get => _name;
        set => SetProperty(ref _name, value, true);
    }

    private string? _email;

    [Required]
    [EmailAddress]
    public string? Email
    {
        get => _email;
        set => SetProperty(ref _email, value, true);
    }

    private string? _phoneNumber;

    [Required]
    [Phone]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber
    {
        get => _phoneNumber;
        set => SetProperty(ref _phoneNumber, value, true);
    }
}
```

## Expose the validated object from the ViewModel

Keep the validated object on the ViewModel and trigger UI refreshes when its state changes:

```csharp
public sealed partial class EditContactViewModel : ViewModelBase
{
    private readonly ILogger<EditContactViewModel> _logger;

    [ObservableProperty]
    private ContactInfo _contact = new();

    public EditContactViewModel(ILogger<EditContactViewModel> logger)
    {
        _logger = logger;
        Contact.PropertyChanged += ContactOnPropertyChanged;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Contact.PropertyChanged -= ContactOnPropertyChanged;
        }

        base.Dispose(disposing);
    }

    [RelayCommand]
    private void ClearForm()
        => Contact = new ContactInfo();

    [RelayCommand]
    private void Save()
        => _logger.LogInformation("Form is valid and submitted!");

    private void ContactOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        => NotifyStateChanged();
}
```

## Use `MvvmObservableValidator` in the view

Wire the form in Razor:

```razor
@page "/form"
@inherits MvvmComponentBase<EditContactViewModel>

<EditForm Model="ViewModel.Contact" FormName="EditContact" OnValidSubmit="ViewModel.SaveCommand.Execute">
    <MvvmObservableValidator />
    <ValidationSummary />

    <div class="row g-3">
        <div class="col-12">
            <label class="form-label">Name:</label>
            <InputText aria-label="name" @bind-Value="ViewModel.Contact.Name" class="form-control" placeholder="Some Name"/>
            <ValidationMessage For="() => ViewModel.Contact.Name" />
        </div>

        <div class="col-12">
            <label class="form-label">Email:</label>
            <InputText aria-label="email" @bind-Value="ViewModel.Contact.Email" class="form-control" placeholder="user@domain.tld"/>
            <ValidationMessage For="() => ViewModel.Contact.Email" />
        </div>

        <div class="col-12">
            <label class="form-label">Phone Number:</label>
            <InputText aria-label="phone number" @bind-Value="ViewModel.Contact.PhoneNumber" class="form-control" placeholder="555-1212"/>
            <ValidationMessage For="() => ViewModel.Contact.PhoneNumber" />
        </div>
    </div>

    <hr class="my-4">

    <div class="row">
        <button class="btn btn-primary btn-lg col"
                type="submit"
                disabled="@ViewModel.Contact.HasErrors">
            Save
        </button>
        <button class="btn btn-secondary btn-lg col"
                type="button"
                @onclick="ViewModel.ClearFormCommand.Execute">
            Clear Form
        </button>
    </div>
</EditForm>
```

## When to use this approach

Use [`MvvmObservableValidator`](xref:Blazing.Mvvm.Components.MvvmObservableValidator) when:

- the form state belongs in a ViewModel
- you want attribute-based validation through CommunityToolkit.Mvvm
- you need Blazor validation UI without custom glue code in every component

## Related topics

- [View Models](../configuration/view-models.md)
- [Getting Started](../getting-started/quick-start.md)
