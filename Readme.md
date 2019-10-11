
# ScriptConsole: a multiplatform graphical C# REPL
## Introduction
**ScriptConsole** is a multiplatform C# REPL (Read-Eval-Print-Loop) with a graphical interface. It can be used to quickly test C# code snippets or to run scripts without creating a full application.

It is written using .NET Core, and is available for Mac, Windows and Linux. The GUI uses the [Avalonia Framework](http://avaloniaui.net/).

ScriptConsole is released under a GPLv3 license.

Along with the ScriptConsole executable, a library called ScriptConsoleLibrary is also available, which makes it possible to easily include a C# REPL into another Avalonia application.

## Installing ScriptConsole

### Standalone version
ScriptConsole does not have any dependencies, thus you essentially need to download and run the executable.
#### Windows
Download [`ScriptConsole-win-x64.exe`](https://github.com/arklumpus/ScriptConsole/releases/latest/download/ScriptConsole-win-x64.exe).
#### Linux
Download [`ScriptConsole-linux-x64`](https://github.com/arklumpus/ScriptConsole/releases/latest/download/ScriptConsole-linux-x64). You may need to mark the file as executable, e.g.:

	wget https://github.com/arklumpus/ScriptConsole/releases/latest/download/ScriptConsole-linux-x64
	chmod +x ScriptConsole-linux-x64
#### MacOS
Download [`ScriptConsole-mac-x64.dmg`](https://github.com/arklumpus/ScriptConsole/releases/latest/download/ScriptConsole-mac-x64.dmg). Open the file and drag the ScriptConsole program to the Applications folder. If you receive a complaint about the program being damaged when you try to open it, you may need to mark the file as executable. To do this, open a terminal and type:

	chmod +x /Applications/ScriptConsole.app/Contents/MacOs/ScriptConsole

And then retry.

### ScriptConsoleLibrary
To include the library in your project, you will need the [ScriptConsoleLibrary NuGet package](https://www.nuget.org/packages/ScriptConsoleLibrary/).

## Usage
ScriptConsole is based on the [Roslyn Scripting API](https://github.com/dotnet/roslyn/wiki/Scripting-API-Samples), thus all standard features of C# 8.0 are available; in addition to this, the following commands are available:

```Csharp
#load "Path\to\script.cs"
```
* Loads a C# script file and executes the commands therein. Please note that the backslashes (`\`) in the path to the file are **not** escaped. Also note that the line does not end with a semicolon.

```Csharp
#r "Name or Path\To\Assembly.dll"
```
* Loads an assembly based on the name or the path to the assembly file. Please note that the backslashes (`\`) in the path to the file are **not** escaped. Also note that the line does not end with a semicolon.
```Csharp
Ans
```
* Returns an object containing the result of the last evaluation. Please note that this is boxed into an _object_, thus it may have to be unboxed before using it. For example:
    ```Csharp
    > "test"
    < "test"
    > Ans.ToUpper();
    < CompilationErrorException: 'object' does not contain a definition for 'ToLower' and no extension method 'ToLower' accepting a first argument of type 'object' could be found (are you missing a using directive or an assembly reference?)
    ```
    While instead:
	```Csharp
	> "test"
    < "test"
    > ((string)Ans).ToUpper();
    < "TEST"
	```
```csharp
Str(object obj, string filter = null, bool showEverything = false, Type context = null)
Str<T>(string filter = null, bool showEverything = false, Type context = null)
Str(Type tp = null, string @namespace = null, string filter = null, bool showEverything = false, Type context = null)
```
* Shows a tree displaying the members of a certain `Type`. These overloads do essentially the same thing, but allow this method to be called in different ways; for example, `Str("test");`, `Str<string>();` and `Str(typeof(string));` will all result in the same output.
	* Arguments:
		* `object obj`: only used to determine its `Type`, e.g. `Str("foo");` and `Str("bar");` will result in the same output.
		* `string filter`: filter the output, only including members that contain this string
		* `showEverything`: include members that are not accessible from the current context (see below)
		* `Type context`: `Type` used to determine member accessibility. For example, if a member is declared as `protected` in a `Type` `T`, it will only be accessible if`context` is `T` or inherits from `T`.
		* `Type tp`: `Type` whose members are to be shown
		* `string @namespace`: used to show the members of a Namespace. For example, `Str(@namespace: "System")` will show the members of the `System` namespace.
```csharp
Clear()
```
* Clears the output console. The current status is preserved  (i.e. all variables declared, methods defined etc are still available). The command history is also preserved.
```csharp
Reset()
```
* Completely re-initialises the environment. This is effectively the same as closing and restarting the application. The command history is also erased.
### ScriptConsoleLibrary API
Once you have installed the NuGet package, you can include the control `ScriptConsoleControl` in your project.

To do this from the XAML designer, you will need to add an XML namespace declaration, and then include the control, e.g. :
```xml
<Window xmlns="https://github.com/avaloniaui"
...
xmlns:sc="clr-namespace:ScriptConsoleLibrary;assembly=ScriptConsoleLibrary"
...>
	<sc:ScriptConsoleControl Name="scc"></sc:ScriptConsoleControl>
</Window>
```
This will build the control using the default constructor (see below).
#### Constructors
```csharp
public ScriptConsoleControl()
```
* Default constructor when adding a `ScriptConsoleControl` through XAML; it loads automatically the following assembly references in the script context:
	```
	Microsoft.CodeAnalysis, Microsoft.CodeAnalysis.CSharp, Microsoft.CodeAnalysis.CSharp.Scripting, Microsoft.CodeAnalysis.Scripting, mscorlib, netstandard, ScriptConsoleLibrary, System.AppContext, System.Collections, System.Collections.Concurrent, System.Collections.Immutable, System.Collections.NonGeneric, System.Collections.Specialized, System.ComponentModel, System.ComponentModel.Primitives, System.ComponentModel.TypeConverter, System.Console, System.Diagnostics.Debug, System.Diagnostics.Tracing, System.Drawing.Primitives, System.Globalization, System.IO, System.IO.FileSystem, System.IO.MemoryMappedFiles, System.Linq, System.Linq.Expressions, System.Memory, System.ObjectModel, System.Private.CoreLib, System.Private.Uri, System.Private.Xml, System.Private.Xml.Linq, System.Reactive, System.Reflection, System.Reflection.Emit.ILGeneration, System.Reflection.Emit.Lightweight, System.Reflection.Metadata, System.Reflection.Primitives, System.Resources.ResourceManager, System.Runtime, System.Runtime.CompilerServices.Unsafe, System.Runtime.Extensions, System.Runtime.InteropServices, System.Runtime.InteropServices.RuntimeInformation, System.Runtime.Loader, System.Security.Cryptography.Algorithms, System.Security.Cryptography.Primitives, System.Text.Encoding.Extensions, System.Threading, System.Threading.Tasks, System.Threading.Tasks.Parallel, System.Threading.Thread, System.Threading.Timer, System.Xml.ReaderWriter, System.Xml.XDocument
	```
	This list is accessible as `ScriptConsoleControl.CoreLoadedAssemblies`.
```csharp
public ScriptConsoleControl(string[] preloadedAssemblies)
```
* Using this constructor, only the assemblies whose name (e.g. `System.Linq`) is contained in `preloadedAssemblies` will be loaded into the script context. Note that these assemblies must already be loaded in the current `AppDomain`. To load assemblies that are not in the current `AppDomain`, you need to use the `#r` directive (which can be invoked from code into the script context using the `RunCommand` method (see below).
#### Methods
```csharp
public void CleanUp()
```
* This method releases resources held by an instance of `ScriptConsoleControl`. This should be called every time a `ScriptConsoleControl` finishes its useful lifetime (e.g. when the containing window is closed), otherwise the garbage collector will not collect it. If you instantiate multiple `ScriptConsoleControl`s, you **must** call this method before a new instance is created, otherwise you will not be able to call it on the previous instance and you will not be able to release memory. Note that you **cannot have multiple `ScriptConsoleControl`s alive at the same time**!

```csharp
public object RunCommand(string command, bool addToOutput)
```
* This method invokes a command in the script context and returns the result of the command.
	* **Arguments**:
		* `string command`: the command to be invoked. It can also be a `#load` or `#r` directive (see above), which makes it possible to load into the script context an Assembly which is not loaded in the current `AppDomain`.
		* `bool addToOutput`: display the command and its output as if it had been entered by the user in the command line.
	* **Return value:** the result of the command, boxed as an object.

## Compiling ScriptConsole from source

To be able to compile ScriptConsole from source, you will need to install the [.NET Core 3.0 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.0) for your operating system.

You can use [Microsoft Visual Studio](https://visualstudio.microsoft.com/it/vs/) to compile the program. The following instructions will cover compiling ScriptConsole from the command line, instead.

First of all, you will need to download the ScriptConsole source code: [ScriptConsole.tar.gz](https://github.com/arklumpus/ScriptConsole/archive/v1.0.3.tar.gz) and extract it somewhere.

### Windows
Open a command-line window in the folder where you have extracted the source code, and type:

	Build <Target>

Where `<Target>` can be one of `Win-x64`, `Linux-x64` or `Mac-x64` depending on which platform you wish to generate executables for.

In the Release folder and in the appropriate subfolder for the target platform you selected, you will find the compiled program.

### macOS and Linux
Open a terminal in the folder where you have extracted the source code, and type:

	./Build.sh <Target>

Where `<Target>` can be one of `Win-x64`, `Linux-x64` or `Mac-x64` depending on which platform you wish to generate executables for.

In the Release folder and in the appropriate subfolder for the target platform you selected, you will find the compiled program.

If you receive an error about permissions being denied, try typing `chmod +x BuildAll.sh` first.