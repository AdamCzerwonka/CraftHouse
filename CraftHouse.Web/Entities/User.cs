using System.ComponentModel.DataAnnotations;

namespace CraftHouse.Web.Entities;

public class User : EntityBase
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    [Required]
    public string TelephoneNumber { get; set; } = null!;

    [Required]
    public string City { get; set; } = null!;

    [Required]
    public string PostalCode { get; set; } = null!;

    [Required]
    public string AddressLine { get; set; } = null!;

    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public UserType UserType { get; set; } = UserType.Standard;

    [Required]
    public string PasswordHash { get; set; } = null!;

    [Required]
    public string PasswordSalt { get; set; } = null!;

    public ICollection<Order> Orders { get; set; } = null!;
}

public enum UserType
{
    Standard,
    Worker,
    Administrator
}