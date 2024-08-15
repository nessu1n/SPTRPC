# Discord RPC for SPT
A simple BepInEx plugin that displays basic Tarkov player / raid information on your Discord Profile

## Usage
Extract SPTRPC.dll and all of its dependencies into BepInEx/plugins

## Dependency references
[Rich Presence API](https://github.com/xhayper/RichPresenceAPI) - A BepInEx specific fix of the Discord RPC csharp library that no longer works due to Unity updates with piping.
[NativeNamedPipe](https://github.com/Lachee/unity-named-pipes/tree/1d1abc0bce88c89ba728907f2d338e65c72b74ef/UnityNamedPipe.Native) - A dependency for Rich Presence API which fixes aforementioned Unity Piping.
[DiscordRPC](https://github.com/Lachee/discord-rpc-csharp) - A dependency for Rich Presence API, the original Discord RPC csharp library.
[NewtonSoft.Json](https://github.com/JamesNK/Newtonsoft.Json) - A dependency for Discord RPC csharp
