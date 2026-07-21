# Changelog

## [2.0.0](https://github.com/eQuantic/core-api-crud/compare/v1.9.1...v2.0.0) (2026-07-21)

### ⚠ BREAKING CHANGES

* repository/service contracts move to eQuantic.Core.Data v5 (two-argument
repositories); the DTO->entity filter/sort casting hooks are redesigned around ExpressionCast;
the CRUD client's paged-list query is built from the typed query builders.

### Features

* controllers option + migrate to eQuantic.Core.Data v5 ([fedd9aa](https://github.com/eQuantic/core-api-crud/commit/fedd9aaa000338af27fa883fa2fec958ea4d229b))
