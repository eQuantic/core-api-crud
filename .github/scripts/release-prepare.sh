#!/usr/bin/env bash
# semantic-release prepare step: stamp the computed version and pack all packages.
# Runs on ubuntu-latest (GNU sed). Invoked as: release-prepare.sh <version>
set -euo pipefail

version="$1"

# Keep the repo's default version in sync with the release
# (committed back to the branch by @semantic-release/git).
sed -i "s|<Version>[^<]*</Version>|<Version>${version}</Version>|" src/Directory.Build.props

rm -rf artifacts/packages

# Build the whole solution once (all target frameworks) so every packable project's outputs
# exist, then pack with --no-build — packing per-project with an implicit build skips a TFM
# in this mixed net8/net10 + net8-only-sample layout (NU5026).
dotnet build eQuantic.Core.Api.Crud.sln -c Release \
  -p:Version="${version}" \
  -p:ContinuousIntegrationBuild=true

for project in src/*/*.csproj; do
  dotnet pack "$project" -c Release --no-build \
    -p:Version="${version}" \
    -o artifacts/packages
done
