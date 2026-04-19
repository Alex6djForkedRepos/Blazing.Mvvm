using System.ComponentModel;
using Blazing.Mvvm.ComponentModel;
using Blazing.Mvvm.Tests.Components.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace Blazing.Mvvm.Tests.Components.ViewModels;

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
