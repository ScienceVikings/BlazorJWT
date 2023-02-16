# BlazorJWT

Automation IAM user should be able to assume `cdk*` roles for CDK deployment.

NEEDS THIS for the `[Authorize]` attribute to work
```
<AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)">
    <NotAuthorized>
        <p>Sorry, you're not authorized to reach this page.</p>
        <p>You may need to log in as a different user.</p>
    </NotAuthorized>
</AuthorizeRouteView>
```

NOTED FILES IN EXAMPLE

* `_Imports.razor`
* `Program.cs`