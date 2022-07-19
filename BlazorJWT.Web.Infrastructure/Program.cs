// See https://aka.ms/new-console-template for more information

// GOOD EXAMPLE WITH TESTS
// https://github.com/aws-samples/aws-cdk-examples/tree/master/csharp/lambda-cron/src/LambdaCron.Tests

using Amazon.CDK;
using Amazon.CDK.AWS.CertificateManager;
using BlazorJWT.Infrastructure;

// https://docs.aws.amazon.com/cdk/v2/guide/testing.html
var app = new App();
var rootStack = new Stack(app,"BlazorJWT-Root",new StackProps()
{
    StackName = "BlazorJWT"
});

var blazorStack = new BlazorJWTStack(rootStack, new BlazorJWTStackOptions()
{
    DomainName = "blazor-jwt-login.sciencevikinglabs.com",
    Certificate = Certificate.FromCertificateArn(rootStack, "BlazorJWT-Certificate",
        "arn:aws:acm:us-east-1:190546235283:certificate/7ceb483f-6deb-4a54-b849-084c488b6b80"),
    OAuthCallbackUrls = new[]
        { "https://localhost:7166/login-callback", "https://blazor-jwt.sciencevikinglabs.com/login-callback" }
});

await blazorStack.BuildStack();

app.Synth();