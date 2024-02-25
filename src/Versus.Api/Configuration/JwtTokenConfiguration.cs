namespace Versus.Api.Configuration;

public class JwtTokenConfiguration
{
    public string Key { get; set; }
    public string Audience { get; set; }
    public string Issuer { get; set; }
}
