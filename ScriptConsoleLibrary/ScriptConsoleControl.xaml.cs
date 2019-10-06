using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ScriptConsoleLibrary
{
    public class ScriptConsoleControl : UserControl
    {
        internal static FontFamily RobotoMono;

        public static string[] CoreLoadedAssemblies = new string[] { "System.Private.CoreLib", "ScriptConsoleLibrary", "System.Runtime", "netstandard", "System.ObjectModel", "System.Collections", "System.ComponentModel.Primitives", "System.Threading", "System.Runtime.InteropServices.RuntimeInformation", "System.Runtime.Extensions", "System.Linq", "System.Globalization", "System.Private.Uri", "System.Collections.NonGeneric", "System.Xml.XDocument", "System.Private.Xml.Linq", "System.Private.Xml", "System.Runtime.InteropServices", "System.Threading.Thread", "System.Memory", "System.Security.Cryptography.Algorithms", "System.Text.Encoding.Extensions", "System.Reactive", "System.ComponentModel", "System.Diagnostics.Debug", "System.Threading.Timer", "System.Resources.ResourceManager", "System.ComponentModel.TypeConverter", "System.Xml.ReaderWriter", "System.Collections.Specialized", "System.Drawing.Primitives", "System.Linq.Expressions", "System.Collections.Concurrent", "System.Reflection.Emit.ILGeneration", "System.Reflection.Primitives", "System.Reflection.Emit.Lightweight", "System.Reflection", "System.IO", "mscorlib", "Microsoft.CodeAnalysis.Scripting", "System.Collections.Immutable", "Microsoft.CodeAnalysis.CSharp.Scripting", "Microsoft.CodeAnalysis", "Microsoft.CodeAnalysis.CSharp", "System.Runtime.Loader", "System.Security.Cryptography.Primitives", "System.Reflection.Metadata", "System.IO.FileSystem", "System.AppContext", "System.Threading.Tasks.Parallel", "System.Diagnostics.Tracing", "System.Runtime.CompilerServices.Unsafe", "System.IO.MemoryMappedFiles", "System.Threading.Tasks", "System.Console" };

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            this.FindControl<ConsoleInputLine>("inputLine").EnterPressed = InputLine_EnterPressed;
            this.FindControl<ConsoleInputLine>("inputLine").HistoryRequested = InputLine_HistoryRequested;
        }

        public ScriptConsoleControl()
        {
            InitializeComponent();
            Initialize();
        }

        public ScriptConsoleControl(string[] preloadedAssemblies)
        {
            InitializeComponent();
            Initialize(preloadedAssemblies);
        }

        private string[] CustomLoadedAssemblies = null;

        public void CleanUp()
        {
            GlobalsContainer.Ans = null;
            GlobalsContainer.consoleContainer = null;
            GlobalsContainer.cSharpConsole = null;
            GlobalsContainer._____ConsoleWriter = null;
            System.Console.SetOut(new System.IO.StreamWriter(System.Console.OpenStandardOutput()) { AutoFlush = true });
            this.globalsContainer = null;
            this.history = null;
            this.lastTask = null;
            GC.Collect();
        }

        private void Initialize(string[] preloadedAssemblies = null)
        {
            CustomLoadedAssemblies = preloadedAssemblies;

            RobotoMono = FontFamily.Parse("resm:ScriptConsoleLibrary.Fonts.?assembly=ScriptConsoleLibrary#Roboto Mono");

            GlobalsContainer.consoleContainer = this.FindControl<StackPanel>("consoleContainer");
            GlobalsContainer._____ConsoleWriter = new OutputConsoleWriter(this.FindControl<StackPanel>("consoleContainer"));
            GlobalsContainer.Ans = null;
            GlobalsContainer.cSharpConsole = this;
            globalsContainer = new GlobalsContainer();
            lastTask = CSharpScript.RunAsync("System.Console.SetOut(_____ConsoleWriter);", null, globalsContainer);
            lastTask.Wait();

            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                bool toBeLoaded = false;

                try
                {
                    if (preloadedAssemblies == null)
                    {
                        if (CoreLoadedAssemblies.Contains(asm.GetName().Name))
                        {
                            toBeLoaded = true;
                        }
                    }
                    else
                    {
                        if (preloadedAssemblies.Contains(asm.GetName().Name))
                        {
                            toBeLoaded = true;
                        }
                    }
                }
                catch
                {

                }

                if (toBeLoaded)
                {

                    try
                    {
                        lastTask = lastTask.Result.ContinueWithAsync("#r \"" + asm.FullName + "\"");
                        lastTask.Wait();
                    }
                    catch
                    {
                        try
                        {
                            lastTask = lastTask.Result.ContinueWithAsync("#r \"" + asm.Location + "\"");
                            lastTask.Wait();
                        }
                        catch (Exception ex)
                        {
                            Exception reportedEx;
                            try
                            {
                                reportedEx = new AssemblyLoadException("An error occurred while loading assembly " + asm.GetName().Name + ": its types will be available during structure exploration but not in code.", ex, asm);
                            }
                            catch
                            {
                                reportedEx = new AssemblyLoadException("An error occurred while loading an assembly.", ex, asm);
                            }

                            this.FindControl<StackPanel>("consoleContainer").Children.Insert(this.FindControl<StackPanel>("consoleContainer").Children.Count - 1, new ErrorLine(reportedEx, true));
                        }
                    }
                }
            }
        }

        internal List<string> history = new List<string>();

        internal Task<ScriptState<object>> lastTask = null;

        internal GlobalsContainer globalsContainer;

        private void InputLine_EnterPressed(object sender, EventArgs e)
        {
            if (this.FindControl<ConsoleInputLine>("inputLine").Command == "")
            {
                return;
            }

            this.FindControl<ConsoleInputLine>("inputLine").ResetHistoryIndex();

            string command = this.FindControl<ConsoleInputLine>("inputLine").Command;

            this.FindControl<ConsoleInputLine>("inputLine").Command = "";

            if (history.Count == 0 || history[history.Count - 1] != command)
            {
                history.Add(command);
            }

            this.FindControl<StackPanel>("consoleContainer").Children.Insert(this.FindControl<StackPanel>("consoleContainer").Children.Count - 1, new HistoryLine(command));

            bool toBeCollected = false;

            try
            {
                Task<ScriptState<object>> evalTask;

                if (lastTask == null)
                {
                    evalTask = CSharpScript.RunAsync(command);
                }
                else
                {
                    evalTask = lastTask.Result.ContinueWithAsync(command);
                }

                evalTask.Wait();

                lastTask = evalTask;

                if (!(evalTask.Result.ReturnValue is DoNotAddOutputLine) && command != "Clear();")
                {
                    GlobalsContainer.Ans = evalTask.Result.ReturnValue;
                    if (!(evalTask.Result.ReturnValue is Type))
                    {
                        this.FindControl<StackPanel>("consoleContainer").Children.Insert(this.FindControl<StackPanel>("consoleContainer").Children.Count - 1, new CSharpOutputLine(evalTask.Result.ReturnValue, false));
                    }
                    else
                    {
                        this.FindControl<StackPanel>("consoleContainer").Children.Insert(this.FindControl<StackPanel>("consoleContainer").Children.Count - 1, new CSharpObjectStructure((Type)evalTask.Result.ReturnValue, "", false, null));
                    }
                }

                if (evalTask.Result.ReturnValue is ResetType || command == "Reset();")
                {
                    ___Reset();
                    toBeCollected = true;
                }

                if (command.Contains("#r"))
                {
                    string[] commandLines = command.Replace("\r", "").Split('\n');
                    foreach (string commandLine in commandLines)
                    {
                        string cmdLine = commandLine.TrimStart(' ', '\t');
                        if (cmdLine.StartsWith("#r"))
                        {
                            try
                            {
                                string assemblyName = cmdLine.Substring(cmdLine.IndexOf("\"") + 1);
                                assemblyName = assemblyName.Substring(0, assemblyName.LastIndexOf("\""));
                                assemblyName = assemblyName.TrimStart(' ', '\t').TrimEnd(' ', '\t');

                                try
                                {
                                    Assembly.LoadFrom(assemblyName);
                                }
                                catch
                                {
                                    try
                                    {
                                        Assembly.LoadWithPartialName(assemblyName);
                                    }
                                    catch (Exception ex)
                                    {
                                        Exception reportedEx = new AssemblyLoadException("An error occurred while loading assembly " + assemblyName + ": its types will be available in code but not during structure exploration.", ex, assemblyName);
                                        this.FindControl<StackPanel>("consoleContainer").Children.Insert(this.FindControl<StackPanel>("consoleContainer").Children.Count - 1, new ErrorLine(reportedEx, true));
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                Exception reportedEx = new AssemblyNameParseException("An error occurred while trying to parse an assembly name from a preprocessor directive: if it was a valid assembly, its types will be available in code but not during structure exploration.", ex, cmdLine);
                                this.FindControl<StackPanel>("consoleContainer").Children.Insert(this.FindControl<StackPanel>("consoleContainer").Children.Count - 1, new ErrorLine(reportedEx, true));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GlobalsContainer.Ans = ex;
                this.FindControl<StackPanel>("consoleContainer").Children.Insert(this.FindControl<StackPanel>("consoleContainer").Children.Count - 1, new ErrorLine(ex, false));
            }


            new Thread(() =>
            {
                Thread.Sleep(10);
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    this.FindControl<ScrollViewer>("scroller").Offset = new Vector(this.FindControl<ScrollViewer>("scroller").Offset.X, double.MaxValue);
                });

                if (toBeCollected)
                {
                    GC.Collect();
                }

            }).Start();
        }

        private void InputLine_HistoryRequested(object sender, HistoryRequestEventArgs e)
        {
            this.FindControl<ConsoleInputLine>("inputLine").historyIndex = Math.Max(this.FindControl<ConsoleInputLine>("inputLine").historyIndex, -history.Count);

            if (e.RequestedIndex < 0 && history.Count >= 1)
            {
                int reqInd = e.RequestedIndex;
                if (reqInd == -1 && history[history.Count - 1] != this.FindControl<ConsoleInputLine>("inputLine").Command && e.Direction < 0)
                {
                    history.Add(this.FindControl<ConsoleInputLine>("inputLine").Command);
                }

                while ((history[Math.Min(history.Count - 1, Math.Max(0, history.Count + reqInd))] == "" || history[Math.Min(history.Count - 1, Math.Max(0, history.Count + reqInd))] == this.FindControl<ConsoleInputLine>("inputLine").Command && history.Count + reqInd + e.Direction >= 0) && history.Count + reqInd + e.Direction < history.Count)
                {
                    reqInd += e.Direction;
                }

                this.FindControl<ConsoleInputLine>("inputLine").Command = history[Math.Min(history.Count - 1, Math.Max(0, history.Count + reqInd))];
                this.FindControl<ConsoleInputLine>("inputLine").FindControl<InputTextBox>("commandBox").CaretIndex = this.FindControl<ConsoleInputLine>("inputLine").Command.Length;
            }
        }

        private void ___Reset()
        {
            this.FindControl<StackPanel>("consoleContainer").Children.Clear();
            this.FindControl<StackPanel>("consoleContainer").Children.Add(this.FindControl<ConsoleInputLine>("inputLine"));
            this.lastTask = null;
            GlobalsContainer.consoleContainer = this.FindControl<StackPanel>("consoleContainer");
            GlobalsContainer._____ConsoleWriter = new OutputConsoleWriter(this.FindControl<StackPanel>("consoleContainer"));
            GlobalsContainer.Ans = null;
            GlobalsContainer.cSharpConsole = this;
            this.globalsContainer = new GlobalsContainer();
            this.lastTask = CSharpScript.RunAsync("System.Console.SetOut(_____ConsoleWriter);", null, this.globalsContainer);
            this.lastTask.Wait();

            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                bool toBeLoaded = false;

                try
                {
                    if (CustomLoadedAssemblies == null)
                    {
                        if (CoreLoadedAssemblies.Contains(asm.GetName().Name))
                        {
                            toBeLoaded = true;
                        }
                    }
                    else
                    {
                        if (CustomLoadedAssemblies.Contains(asm.GetName().Name))
                        {
                            toBeLoaded = true;
                        }
                    }
                }
                catch
                {

                }

                if (toBeLoaded)
                {
                    try
                    {
                        this.lastTask = this.lastTask.Result.ContinueWithAsync("#r \"" + asm.FullName + "\"");
                        this.lastTask.Wait();
                    }
                    catch
                    {
                        try
                        {
                            this.lastTask = this.lastTask.Result.ContinueWithAsync("#r \"" + asm.Location + "\"");
                            this.lastTask.Wait();
                        }
                        catch (Exception ex)
                        {
                            Exception reportedEx;
                            try
                            {
                                reportedEx = new AssemblyLoadException("An error occurred while loading assembly " + asm.GetName().Name + ": its types will be available during structure exploration but not in code.", ex, asm);
                            }
                            catch
                            {
                                reportedEx = new AssemblyLoadException("An error occurred while loading an assembly.", ex, asm);
                            }

                            this.FindControl<StackPanel>("consoleContainer").Children.Insert(this.FindControl<StackPanel>("consoleContainer").Children.Count - 1, new ErrorLine(reportedEx, true));
                        }
                    }
                }
            }

            this.history.Clear();
        }

        public object RunCommand(string command, bool addToOutput)
        {
            if (command == "")
            {
                return null;
            }

            if (addToOutput)
            {
                this.FindControl<StackPanel>("consoleContainer").Children.Insert(this.FindControl<StackPanel>("consoleContainer").Children.Count - 1, new HistoryLine(command));
            }

            bool toBeCollected = false;

            try
            {
                Task<ScriptState<object>> evalTask;

                if (lastTask == null)
                {
                    evalTask = CSharpScript.RunAsync(command);
                }
                else
                {
                    evalTask = lastTask.Result.ContinueWithAsync(command);
                }

                evalTask.Wait();

                lastTask = evalTask;

                if (!(evalTask.Result.ReturnValue is DoNotAddOutputLine) && command != "Clear();")
                {
                    if (addToOutput)
                    {
                        GlobalsContainer.Ans = evalTask.Result.ReturnValue;
                        if (!(evalTask.Result.ReturnValue is Type))
                        {
                            this.FindControl<StackPanel>("consoleContainer").Children.Insert(this.FindControl<StackPanel>("consoleContainer").Children.Count - 1, new CSharpOutputLine(evalTask.Result.ReturnValue, false));
                        }
                        else
                        {
                            this.FindControl<StackPanel>("consoleContainer").Children.Insert(this.FindControl<StackPanel>("consoleContainer").Children.Count - 1, new CSharpObjectStructure((Type)evalTask.Result.ReturnValue, "", false, null));
                        }
                    }
                }

                if (evalTask.Result.ReturnValue is ResetType || command == "Reset();")
                {
                    ___Reset();
                    toBeCollected = true;
                }

                if (command.Contains("#r"))
                {
                    string[] commandLines = command.Replace("\r", "").Split('\n');
                    foreach (string commandLine in commandLines)
                    {
                        string cmdLine = commandLine.TrimStart(' ', '\t');
                        if (cmdLine.StartsWith("#r"))
                        {
                            try
                            {
                                string assemblyName = cmdLine.Substring(cmdLine.IndexOf("\"") + 1);
                                assemblyName = assemblyName.Substring(0, assemblyName.LastIndexOf("\""));
                                assemblyName = assemblyName.TrimStart(' ', '\t').TrimEnd(' ', '\t');

                                try
                                {
                                    Assembly.LoadFrom(assemblyName);
                                }
                                catch
                                {
                                    try
                                    {
                                        Assembly.LoadWithPartialName(assemblyName);
                                    }
                                    catch (Exception ex)
                                    {
                                        Exception reportedEx = new AssemblyLoadException("An error occurred while loading assembly " + assemblyName + ": its types will be available in code but not during structure exploration.", ex, assemblyName);
                                        this.FindControl<StackPanel>("consoleContainer").Children.Insert(this.FindControl<StackPanel>("consoleContainer").Children.Count - 1, new ErrorLine(reportedEx, true));
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                Exception reportedEx = new AssemblyNameParseException("An error occurred while trying to parse an assembly name from a preprocessor directive: if it was a valid assembly, its types will be available in code but not during structure exploration.", ex, cmdLine);
                                this.FindControl<StackPanel>("consoleContainer").Children.Insert(this.FindControl<StackPanel>("consoleContainer").Children.Count - 1, new ErrorLine(reportedEx, true));
                            }
                        }
                    }
                }

                new Thread(() =>
                {
                    Thread.Sleep(10);

                    if (addToOutput)
                    {
                        Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            this.FindControl<ScrollViewer>("scroller").Offset = new Vector(this.FindControl<ScrollViewer>("scroller").Offset.X, double.MaxValue);
                        });
                    }

                    if (toBeCollected)
                    {
                        GC.Collect();
                    }

                }).Start();

                return evalTask.Result.ReturnValue;
            }
            catch (Exception ex)
            {
                if (addToOutput)
                {
                    GlobalsContainer.Ans = ex;
                    this.FindControl<StackPanel>("consoleContainer").Children.Insert(this.FindControl<StackPanel>("consoleContainer").Children.Count - 1, new ErrorLine(ex, false));

                    new Thread(() =>
                    {
                        Thread.Sleep(10);

                        if (addToOutput)
                        {
                            Dispatcher.UIThread.InvokeAsync(() =>
                            {
                                this.FindControl<ScrollViewer>("scroller").Offset = new Vector(this.FindControl<ScrollViewer>("scroller").Offset.X, double.MaxValue);
                            });
                        }

                        if (toBeCollected)
                        {
                            GC.Collect();
                        }

                    }).Start();
                }
                throw ex;
            }
        }
    }
}
