namespace BlazorJWT.Models;

public class JwtToken
{
    public DateTime ExpiresOn { get; set; } = DateTime.MinValue;
    public string IdToken { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    
    private int _expiresIn;
    public int ExpiresIn
    {
        get => _expiresIn;
        set
        {
            _expiresIn = value;
            ExpiresOn = DateTime.Now + TimeSpan.FromSeconds(_expiresIn);
        }
    }

    public bool IsLoggedIn => DateTime.Now <= ExpiresOn && !string.IsNullOrWhiteSpace(IdToken);

}