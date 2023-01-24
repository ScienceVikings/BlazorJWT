// See https://aka.ms/new-console-template for more information

// GOOD EXAMPLE WITH TESTS
// https://github.com/aws-samples/aws-cdk-examples/tree/master/csharp/lambda-cron/src/LambdaCron.Tests

using Amazon.CDK;
using Amazon.CDK.AWS.CertificateManager;
using BlazorJWT.Infrastructure;
using BlazorJWT.Web.Infrastructure;

// https://docs.aws.amazon.com/cdk/v2/guide/testing.html
var certArn = "arn:aws:acm:us-east-1:190546235283:certificate/7ceb483f-6deb-4a54-b849-084c488b6b80";
var app = new App();
var rootStack = new Stack(app,"BlazorJWT-Root",new StackProps()
{
    StackName = "BlazorJWT"
});

var blazorStack = new BlazorJWTStack(rootStack, new BlazorJWTStackOptions()
{
    DomainName = "blazor-jwt-login.sciencevikinglabs.com", //The site we want to give to cognito (we call this to login)
    Certificate = Certificate.FromCertificateArn(rootStack, "BlazorJWT-Certificate", certArn),
    //The site that Cognito calls back to
    OAuthCallbackUrls = new[] { "https://localhost:7166/login-callback", "https://blazor-jwt.sciencevikinglabs.com/login-callback" },
    OAuthSignOutUrls = new [] { "https://localhost:7166/logout-callback", "https://blazor-jwt.sciencevikinglabs.com/logout-callback" }
});

await blazorStack.BuildStack();

var siteStack = new StaticSiteStack(rootStack, new StaticSiteStackOptions()
{
    DomainName = "blazor-jwt.sciencevikinglabs.com",
    CertificateArn = certArn,
    BucketName = "blazor-jwt-site"
});

await siteStack.BuildStack();

app.Synth();

// https://www.spotify.com/us/family/join/invite/6A5x7x9az8CXY7y/