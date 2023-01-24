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
        
        var cloudFrontProps = new CloudFrontWebDistributionProps()
        {
            ViewerCertificate = ViewerCertificate.FromAcmCertificate(cert,new ViewerCertificateOptions()
            {
                Aliases = new []{_staticSiteStackOptions.DomainName},
                SslMethod = SSLMethod.SNI
            }),
            DefaultRootObject = "/index.html",
            Enabled = true,
            HttpVersion = HttpVersion.HTTP1_1,
            EnableIpV6 = false,
            PriceClass = PriceClass.PRICE_CLASS_ALL,
            OriginConfigs = new ISourceConfiguration[]{new SourceConfiguration()
            {
                Behaviors = new IBehavior[]{new Behavior()
                {
                    AllowedMethods = CloudFrontAllowedMethods.GET_HEAD_OPTIONS,
                    ViewerProtocolPolicy = ViewerProtocolPolicy.REDIRECT_TO_HTTPS,
                    IsDefaultBehavior = true,
                    ForwardedValues = new CfnDistribution.ForwardedValuesProperty()
                    {
                        Headers = new []{"Origin"},
                        QueryString = false
                    },
                    DefaultTtl = Duration.Seconds(0),
                    MinTtl = Duration.Seconds(0),
                    MaxTtl = Duration.Seconds(0)
                    // * * * I think if we make another behavior with the _framework/* path to have no cache it may work a bit better?
                }},
                S3OriginSource = new S3OriginConfig()
                {
                    OriginAccessIdentity = cloudFrontOriginAccessIdentity,
                    S3BucketSource = bucket
                }
            }},
            ErrorConfigurations = new CfnDistribution.ICustomErrorResponseProperty[]
            {
                new CfnDistribution.CustomErrorResponseProperty()
                {
                    ErrorCode = 404,
                    ResponseCode = 200,
                    ResponsePagePath = "/index.html"
                },
                new CfnDistribution.CustomErrorResponseProperty()
                {
                    ErrorCode = 403,
                    ResponseCode = 200,
                    ResponsePagePath = "/index.html"
                }
            }
        };
        
        var cloudFront = new CloudFrontWebDistribution(this, "CloudFrontWebDistribution", cloudFrontProps);
        
        return Task.CompletedTask;
    }
}