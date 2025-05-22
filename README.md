# ShowLogLauncher

A WPF SDK and application suite for automation integration with kSA systems.

## Projects

- `ShowLogSDK`: The core automation SDK (NuGet package-ready)
- `ShowLogLauncher`: Test harness for SDK
- `ShowLogLauncher_Installer`: Wix installer project for the test harness

## Features

- Modular SDK
- Per-process monitoring and control
- Integrated MVVM Toolkit
- Status indicators and live updates

## Getting Started

1. Clone this repository.
2. Open the solution in Visual Studio 2022 or later.
3. Goto Extensions-> Manage Extensions and install the following: 
   [FireGiant Heatware for VS2022.] Needed to support the WiX installer project.
4. Build the solution.
5. Set `ShowLogLauncher` as project to run on startup and run to try out the SDK.

## License

MIT (or your preferred license)

## Repository

Hosted at: [GitLab](https://github.com/k-SpaceAssociates/ShowLog.git)