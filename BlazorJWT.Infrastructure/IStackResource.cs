using Amazon.CDK;

namespace BlazorJWT.Infrastructure;

internal interface IStackResource
{
    Task BuildStack();
}