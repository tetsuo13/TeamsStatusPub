# Teams Status Pub Changelog

## 1.6.2 (2024-12-03)

- Fixed rolled log files created with trailing sequence sometimes created on app startup ([#136](https://github.com/tetsuo13/TeamsStatusPub/issues/136))
- Prevent multiple About windows ([#129](https://github.com/tetsuo13/TeamsStatusPub/issues/129))

## 1.6.1 (2024-08-28)

- Refactored away service locator anti-pattern.
- Changed front-end from WinForms to Avalonia ([#120](https://github.com/tetsuo13/TeamsStatusPub/pull/120))
- Added support for dark mode in About window ([#75](https://github.com/tetsuo13/TeamsStatusPub/pull/75))

## 1.6.0 (2024-04-15)

- Added support for Microsoft Teams in addition to Microsoft Teams Classic ([#112](https://github.com/tetsuo13/TeamsStatusPub/issues/112))

## 1.5.0 (2024-02-21)

- Added CI build number to version reported in About window ([#105](https://github.com/tetsuo13/TeamsStatusPub/pull/105))
- Created installer rather than distributing executable ([#38](https://github.com/tetsuo13/TeamsStatusPub/issues/38))

## 1.4.0 (2023-11-30)

- .NET 8 ([#91](https://github.com/tetsuo13/TeamsStatusPub/pull/91))

## 1.3.1 (2023-05-29)

- Fixed crash on launch introduced in previous version ([#49](https://github.com/tetsuo13/TeamsStatusPub/issues/49))

## 1.3.0 (2023-05-22)

- Fixed minimized window showing in bottom-left of screen ([#31](https://github.com/tetsuo13/TeamsStatusPub/issues/31))
- Added runtime information to About dialog ([#41](https://github.com/tetsuo13/TeamsStatusPub/issues/41))

## 1.2.0 (2023-01-20)

- Bundled application icon, no longer distributed with release files.

## 1.1.0 (2022-12-05)

- .NET 7 ([#28](https://github.com/tetsuo13/TeamsStatusPub/pull/28))

## 1.0.1 (2022-10-19)

- Fixed screen sharing while on a call not counting as busy ([#8](https://github.com/tetsuo13/TeamsStatusPub/issues/8))

## 1.0.0 (2022-08-03)

- Initial release.
