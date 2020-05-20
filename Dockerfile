FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY [".", "StackoverflowGetFanaticBadge/"]
RUN dotnet restore "StackoverflowGetFanaticBadge/StackoverflowGetFanaticBadge.csproj"
COPY . .
WORKDIR "/src/StackoverflowGetFanaticBadge"
RUN dotnet build "StackoverflowGetFanaticBadge.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StackoverflowGetFanaticBadge.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StackoverflowGetFanaticBadge.dll"]