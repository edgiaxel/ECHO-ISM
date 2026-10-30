# Change Logs — ECHO-ISM

## Stage 0 — Project audit and baseline

- Audited the initial working tree on 2026-10-30. The repository has no commits; the project files and `Docs/` are untracked, and `origin` points to `git@github.com-personal:edgiaxel/ECHO-ISM.git`.
- The initial audit snapshot lacked the C# project files, main scene, and run-main-scene setting. Axel added them afterward; the current checkout contains `ECHO-ISM.csproj`, `ECHO-ISM.sln`, and `Scenes/Main.tscn`, and `project.godot` selects Main and enables C#.
- Inspected `project.godot`: project name is ECHO-ISM, feature version is 4.7, renderer is GL Compatibility, and the .NET assembly name is configured. Godot reports `4.7.2.stable.mono.official.ed1daf0bf`.
- .NET SDK 10.0.403 and runtime 10.0.14 are installed under `/home/axel/.dotnet`; `dotnet` is not on the shell PATH by default. The Debian APT commands were unnecessary, and `sudo` requires an unavailable password. With `DOTNET_ROOT` and PATH set to that install, `dotnet build ECHO-ISM.sln` succeeded with 0 warnings and 0 errors.
- Godot headless editor opened and initialized the project successfully with the .NET environment set. A headless run of the configured Main scene completed five frames with exit code 0 and no output errors. Axel also confirmed opening the project in Godot and selecting `Main.tscn` as the run scene.
- An initial headless editor launch without the .NET PATH reported `dotnet: not found`, could not load `hostfxr`, and crashed; rerunning with the correct PATH succeeded. That first attempt also printed `Cannot convert string "vlines2" to type Pixmap`. No warnings or errors appeared in the successful build, editor initialization, or game run.
- Stage 0 is preserved as commit `fd2ef69`; the verified project and checklist/changelog updates are being committed and pushed as the Stage 0 completion revision.
