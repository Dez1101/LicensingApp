namespace LicensingApp.Models;

public class CryptlexLoginModel
{
    public string AccountId { get; set; } // It seems this might be optional, as "string or null" is mentioned.
    public string Email { get; set; }
    public string Password { get; set; }
}

