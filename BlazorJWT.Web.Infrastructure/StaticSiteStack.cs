using Amazon.CDK;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.CloudFront;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.S3;
using BlazorJWT.Infrastructure;
using Constructs;

namespace BlazorJWT.Web.Infrastructure;

public class StaticSiteStackOptions
{
    public string BucketName { get; set; } = string.Empty;
    public string DomainName { get; set; } = string.Empty;
    public string CertificateArn { get; set; } = string.Empty;
}

public class StaticSiteStack : NestedStack,IStackResource
{
    private readonly StaticSiteStackOptions _staticSiteStackOptions;

    public StaticSiteStack(Construct scope, StaticSiteStackOptions staticSiteStackOptions) : base(scope, "StaticSite", null)
    {
        _staticSiteStackOptions = staticSiteStackOptions;
    }

    public Task BuildStack()
    {
        Console.WriteLine("Options");
        Console.WriteLine(_staticSiteStackOptions.DomainName);
        Console.WriteLine(_staticSiteStackOptions.BucketName);
        var cloudFrontOriginAccessIdentity = new OriginAccessIdentity(this, "OriginAccessIdentity",
            new OriginAccessIdentityProps() { Comment = "Access S3 bucket content only through CloudFront" });
        
        var bucketProps = new BucketProps()
        {
            AccessControl = BucketAccessControl.PRIVATE,
            BucketName = _staticSiteStackOptions.BucketName,
            PublicReadAccess = false,
            BlockPublicAccess = BlockPublicAccess.BLOCK_ALL,
            RemovalPolicy = RemovalPolicy.DESTROY
        };
        var bucket = new Bucket(this, $"BlazorJWT-Bucket-{_staticSiteStackOptions.BucketName}", bucketProps);

        bucket.AddToResourcePolicy(new PolicyStatement(new PolicyStatementProps()
        {
            Effect = Effect.ALLOW,
            Actions = new[] { "s3:GetObject" },
            Resources = new[] { $"arn:aws:s3:::{bucket.BucketName}/*" },
            Principals = new IPrincipal[]
            {
                new CanonicalUserPrincipal(cloudFrontOriginAccessIdentity
                    .CloudFrontOriginAccessIdentityS3CanonicalUserId)
            }
        }));
        var cert = Certificate.FromCertificateArn(this,"Certificate",_staticSiteStackOptions.CertificateArn);
       
        var dist = new Distribution(this, "CloudFrontDistribution", new DistributionProps()
        {
            Certificate = cert,
            DefaultBehavior = new BehaviorOptions()
            {
                AllowedMethods = AllowedMethods.ALLOW_GET_HEAD_OPTIONS,
                CachePolicy = CachePolicy.CACHING_DISABLED,
                OriginRequestPolicy = OriginRequestPolicy.CORS_S3_ORIGIN
            },
            DefaultRootObject = "/index.html",
            Enabled = true,
            PriceClass = PriceClass.PRICE_CLASS_100,
            ErrorResponses = new IErrorResponse[]{
                new ErrorResponse()
                {
                    HttpStatus = 404,
                    ResponseHttpStatus = 404,
                    ResponsePagePath = "/not-found",
                    Ttl = Duration.Seconds(0)
                },
                new ErrorResponse()
                {
                    HttpStatus = 500,
                    ResponseHttpStatus = 500,
                    ResponsePagePath = "/server-error",
                    Ttl = Duration.Seconds(0)
                },
                new ErrorResponse()
                {
                    HttpStatus = 403,
                    ResponseHttpStatus = 403,
                    ResponsePagePath = "/forbidden",
                    Ttl = Duration.Seconds(0)
                }
            }
        });
        
        return Task.CompletedTask;
    }
}