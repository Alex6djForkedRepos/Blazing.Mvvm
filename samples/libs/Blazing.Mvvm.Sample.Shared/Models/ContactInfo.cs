using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;


/// <summary>
/// Represents contact information with validation for name, email, and phone number.
/// </summary>
namespace Blazing.Mvvm.Sample.Shared.Models;


/// <summary>
/// Represents contact information for a person, including name, email, and phone number.
/// </summary>
public class ContactInfo : ObservableValidator
{
    private string? _name;

    /// <summary>
    /// Gets or sets the name of the contact.
    /// </summary>
    /// <remarks>
    /// The name must be between 2 and 100 characters and can only contain letters, spaces, apostrophes, and hyphens.
    /// </remarks>
    [Required]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "The {0} field must have a length between {2} and {1}.")]
    [RegularExpression(@"^[a-zA-Z\s'-]+$", ErrorMessage = "The {0} field contains invalid characters. Only letters, spaces, apostrophes, and hyphens are allowed.")]
    public string? Name
    {
        get => _name;
        set => SetProperty(ref _name, value, true);
    }


    private string? _email;

    /// <summary>
    /// Gets or sets the email address of the contact.
    /// </summary>
    [Required]
    [EmailAddress]
    public string? Email
    {
        get => _email;
        set => SetProperty(ref _email, value, true);
    }


    private string? _phoneNumber;

    /// <summary>
    /// Gets or sets the phone number of the contact.
    /// </summary>
    [Required]
    [Phone]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber
    {
        get => _phoneNumber;
        set => SetProperty(ref _phoneNumber, value, true);
    }
}
