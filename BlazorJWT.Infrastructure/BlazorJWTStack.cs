using Amazon.CDK;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.Cognito;
using Constructs;

namespace BlazorJWT.Infrastructure;

public class BlazorJWTStack : NestedStack, IStackResource
{
    private readonly BlazorJWTStackOptions _options;

    public BlazorJWTStack(Construct scope, BlazorJWTStackOptions options) : base(scope, "BlazorJWT-Cognito",
        new NestedStackProps()
        {
            Description = "Cognito"
        })
    {
        _options = options;
    }

    public async Task BuildStack()
    {
        var userPoolProps = new UserPoolProps()
        {
            RemovalPolicy = RemovalPolicy.RETAIN,
            UserPoolName = _options.EffectiveUserPoolName,
            SelfSignUpEnabled = _options.SelfSignUpEnabled,
            SignInAliases = new SignInAliases()
            {
                Email = true,
                Username = false
            },
            UserVerification = new UserVerificationConfig()
            {
                EmailStyle = VerificationEmailStyle.LINK
            },
            Email = UserPoolEmail.WithCognito("noreply@sciencevikinglabs.com")
        };
        var userPool = new UserPool(this, _options.EffectiveUserPoolName, userPoolProps);

        if (_options.Certificate != null)
        {
            var userPoolDomainProps = new UserPoolDomainProps()
            {
                UserPool = userPool,
                CustomDomain = new CustomDomainOptions()
                {
                    DomainName = _options.DomainName,
                    Certificate = _options.Certificate
                }
            };
            var userPoolDomain = new UserPoolDomain(this, $"{_options.StackObjectNamePrefix}UserPoolDomain",
                userPoolDomainProps);
        }

        var userPoolClientProps = new UserPoolClientProps()
        {
            UserPool = userPool,
            UserPoolClientName = _options.EffectiveUserPoolClientName,
            GenerateSecret = false,
            SupportedIdentityProviders = _options.SupportedIdentityProviders,
            OAuth = new OAuthSettings()
            {
                Scopes = _options.OAuthScopes,
                Flows = new OAuthFlows() { ImplicitCodeGrant = true },
                CallbackUrls = _options.OAuthCallbackUrls
            },
            EnableTokenRevocation = true,
            AuthFlows = new AuthFlow()
            {
                UserPassword = true
                
            },
            PreventUserExistenceErrors = true
        };
        var userPoolAppClient = new UserPoolClient(this, _options.EffectiveUserPoolClientName, userPoolClientProps);

        var cognitoIdentityProviderProps = new CfnIdentityPool.CognitoIdentityProviderProperty()
        {
            ClientId = userPoolAppClient.UserPoolClientId,
            ProviderName = userPool.UserPoolProviderName
        };
        var identityPoolProps = new CfnIdentityPoolProps()
        {
            AllowClassicFlow = false,
            AllowUnauthenticatedIdentities = _options.AllowUnauthenticatedIdentities,
            IdentityPoolName = _options.EffectiveIdentityPoolName,
            CognitoIdentityProviders = new[] { cognitoIdentityProviderProps }
        };
        var identityPool = new CfnIdentityPool(this, _options.EffectiveIdentityPoolName, identityPoolProps);

        await Task.CompletedTask;
    }
}