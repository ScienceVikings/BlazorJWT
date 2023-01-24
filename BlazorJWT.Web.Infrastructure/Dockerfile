FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /code
COPY . .
RUN dotnet build BlazorJWT.sln -c Release

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS deploy
RUN apt-get update && apt-get install -yqq curl
RUN curl -sL -o node-setup.sh https://deb.nodesource.com/setup_16.x
RUN chmod +x node-setup.sh && ./node-setup.sh
RUN apt-get update && apt-get install -yqq nodejs
RUN npm install -g aws-cdk
WORKDIR /app
COPY --from=build /code/BlazorJWT.Web.Infrastructure/cdk.json .
COPY --from=build /code/BlazorJWT.Web.Infrastructure/bin/Release/net6.0/ .
COPY --from=build /code/BlazorJWT.Web.Infrastructure/entrypoint.sh .
RUN chmod +x ./entrypoint.sh
ENTRYPOINT ["./entrypoint.sh"]

FROM deploy AS cli
ENTRYPOINT ["/bin/bash"]