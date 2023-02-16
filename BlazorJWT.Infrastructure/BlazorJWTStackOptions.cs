using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.Cognito;

namespace BlazorJWT.Infrastructure;

public class BlazorJWTStackOptions
{
    public string StackObjectNamePrefix { get; set; } = "BlazorJWT-";

    public string UserPoolName { get; set; } = "UserPool";

    public bool SelfSignUpEnabled { get; set; } = true;

    public string DomainName { get; set; } = string.Empty;
    public ICertificate? Certificate { get; set; }

    public string UserPoolClientName { get; set; } = "UserPoolClient";

    public UserPoolClientIdentityProvider[] SupportedIdentityProviders { get; set; } =
        { UserPoolClientIdentityProvider.COGNITO };

    public OAuthScope[] OAuthScopes { get; set; } = new[]
        { OAuthScope.EMAIL, OAuthScope.OPENID, OAuthScope.COGNITO_ADMIN, OAuthScope.PROFILE };
    
    public string[] OAuthCallbackUrls { get; set; } = Array.Empty<string>();
    public string[] OAuthSignOutUrls { get; set; } = Array.Empty<string>();

    public string IdentityPoolName { get; set; } = "IdentityPool";
    public bool AllowUnauthenticatedIdentities { get; set; } = false;

    internal string EffectiveUserPoolName => $"{StackObjectNamePrefix}{UserPoolName}";
    internal string EffectiveUserPoolClientName => $"{StackObjectNamePrefix}{UserPoolClientName}";
    internal string EffectiveIdentityPoolName => $"{StackObjectNamePrefix}{IdentityPoolName}";
}