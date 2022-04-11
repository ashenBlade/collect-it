using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CollectIt.API.React;

public class JwtOptions
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string Key { get; set; }
    public int LifeTime { get; set; }

    public SymmetricSecurityKey SymmetricSecurityKey =>
        new(Encoding.ASCII.GetBytes(Key));
}