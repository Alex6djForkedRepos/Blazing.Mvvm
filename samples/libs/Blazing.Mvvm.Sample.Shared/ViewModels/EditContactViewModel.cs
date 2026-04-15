using Blazing.Mvvm.ComponentModel;
using Blazing.Mvvm.Sample.Shared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Threading;

namespace Blazing.Mvvm.Sample.Shared.ViewModels;

/// <summary>
/// ViewModel for editing contact information, providing form validation, clear, and save functionality.
/// </summary>
public sealed partial class EditContactViewModel : ViewModelBase
{
    private readonly ILogger<EditContactViewModel> _logger;

    /// <summary>
    /// Gets or sets the contact information being edited.
    /// </summary>
    [ObservableProperty]
    private ContactInfo _contact = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="EditContactViewModel"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for logging events.</param>
    public EditContactViewModel(ILogger<EditContactViewModel> logger)
    {
        _logger = logger;
        Contact.PropertyChanged += ContactOnPropertyChanged;
    }

    /// <summary>
    /// Clears the contact form by resetting the <see cref="Contact"/> property.
    /// </summary>
    [RelayCommand]
    private void ClearForm()
        => Contact = new ContactInfo();

    /// <summary>
    /// Saves the contact form and logs a message if the form is valid.
    /// </summary>
    [RelayCommand]
    private void Save()
        => _logger.LogInformation("Form is valid and submitted!");

    /// <summary>
    /// Handles the <see cref="INotifyPropertyChanged.PropertyChanged"/> event for the <see cref="Contact"/> property.
    /// Notifies the view of state changes.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void ContactOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        => NotifyStateChanged();

    /// <summary>
    /// Disposes the ViewModel and detaches event handlers.
    /// </summary>
    /// <param name="disposing">Indicates whether the method is called from Dispose.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _logger.LogInformation("Disposing {VMName}.", GetType().Name);
            Contact.PropertyChanged -= ContactOnPropertyChanged;
        }

        base.Dispose(disposing);
    }
}
