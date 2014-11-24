# Loadrix #
_Loadrix_ is a system for loading content and code for games. Which is a super
abstract concept, because in a lot of ways it's a super abstract tool. Loadrix
is borne out of some of my game development adventures, as the core of a mod
system that I've carried with me from game project to game project. It
supports the general idea of "load that thing, whatever it is" in an
extensible and configurable way; creating a mod system that allows content
overriding is as simple as arranging your resources as it expects and using
a LoadrixMultiContext as your game's base loader.

Loadrix is released under the MIT license.

## Architecture ##
Loadrix is implemented as a set of assemblies:

- **Loadrix.Core** is a Portable Class Library that contains the required
  interfaces and high-level implementations that are used by consuming
  applications.
- **Loadrix.ExtensionCompiler** is a desktop class library (designed for .NET
  and Mono) that implements dynamic compilation via `CSharpCodeProvider` for
  a specified source tree.
- **Loadrix.MonoGame** is a shared library (so as to not require different
  project files for every MonoGame platform) that integrates the MonoGame
  ContentManager into Loadrix.

All parts of Loadrix have at least the "happy path" tested; if you run into
edge cases (which almost certainly exist), pull requests for additional tests
are welcome!

## Future Work ##
- ExtensionCompiler could read csproj files instead of being compiled strictly
  with the settings provided by the calling application.