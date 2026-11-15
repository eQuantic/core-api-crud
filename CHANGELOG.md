# Changelog

## [4.0.0](https://github.com/eQuantic/core-api-crud/compare/v3.0.0...v4.0.0) (2026-11-15)

### ⚠ BREAKING CHANGES

* users get core-api 6.0 and core-ddd 7.0 through these packages. The ServiceRegistry overload of AddEndpointModules is gone (it now extends IServiceCollection), Lamar no longer comes through them, and eQuantic.Core.Application no longer brings an IoC container.

### Features

* move to core-api 6.0 and core-ddd 7.0 ([#11](https://github.com/eQuantic/core-api-crud/issues/11)) ([b1eb157](https://github.com/eQuantic/core-api-crud/commit/b1eb157427ab88605b938fa9b4cac3c55e6066d1)), closes [#10](https://github.com/eQuantic/core-api-crud/issues/10)

## [3.0.0](https://github.com/eQuantic/core-api-crud/compare/v2.0.0...v3.0.0) (2026-11-14)

### ⚠ BREAKING CHANGES

* core-api-crud now depends on core-api 5.0, which crosses several majors: IEntity lives in eQuantic.Core.DataModel (core-api 4.0), EntityBase<TKey> compares by identity instead of by reference (core-ddd 5.0), and the repositories come from eQuantic.Core.Data 7.0, which changes what some existing calls do, such as a duplicate key surfacing as DuplicateKeyException at commit (core-ddd 6.0). Applications get all of them through core-api-crud.

### Features

* move to core-api 5.0 and publish through trusted publishing ([#9](https://github.com/eQuantic/core-api-crud/issues/9)) ([7f4fe2a](https://github.com/eQuantic/core-api-crud/commit/7f4fe2ac8431e90e3d27beecdbfb5bc432991731))

## [2.0.0](https://github.com/eQuantic/core-api-crud/compare/v1.9.1...v2.0.0) (2026-07-21)

### ⚠ BREAKING CHANGES

* repository/service contracts move to eQuantic.Core.Data v5 (two-argument
repositories); the DTO->entity filter/sort casting hooks are redesigned around ExpressionCast;
the CRUD client's paged-list query is built from the typed query builders.

### Features

* controllers option + migrate to eQuantic.Core.Data v5 ([fedd9aa](https://github.com/eQuantic/core-api-crud/commit/fedd9aaa000338af27fa883fa2fec958ea4d229b))
