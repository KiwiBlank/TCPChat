<p align="center">
  <h1 align="center">KiwiBlank's TCPChat</h3>
  <h4 align="center">Console Chat System using .NET</h3>
</p>
<p align="center">
    <img src="https://user-images.githubusercontent.com/24278929/93818607-e22bd100-fc5a-11ea-96f2-864c0bc24f81.gif" />
</p>


## Features
- Client-Server Chat System
- Customizable Username & Text Color
- Cross-Platform Support with .NET Core (Windows, macOS, Linux)
- Hybrid Message Encryption with RSA and AES
- No External Dependencies

## Build & Installation

### Prerequisites
#### **NOTE: .NET Core is not required when running the included binaries.**
- .NET Core 3.1 [(Download)](https://dotnet.microsoft.com/download/dotnet-core/3.1)

### Building
To build the solution, run
```bat
dotnet build
```
in the main directory.

Or, alternatively run `publish.bat` to build a ReadyToRun and self contained executable.

## Usage

### Server
1. Run TCPChat-Server
2. Edit the `serverconfig.json` to your liking.
3. Run TCPChat-Server
4. Type `/server` and follow the steps.

### Client
1. Run TCPChat-Client
2. Edit the `clientconfig.json` to your liking. [(See Available Colors Here)](https://docs.microsoft.com/en-us/dotnet/api/system.consolecolor?view=netcore-3.1)
3. Run TCPChat-Client
4. Type `/connect` and follow the steps.
