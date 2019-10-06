using Avalonia.Controls;
using Avalonia.Threading;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ScriptConsoleLibrary
{
    public class DoNotAddOutputLine { }
    public class ResetType : DoNotAddOutputLine { }

    public class GlobalsContainer
    {
        public static OutputConsoleWriter _____ConsoleWriter;

        internal static StackPanel consoleContainer;

        internal static ScriptConsoleControl cSharpConsole;

        public static object Ans { get; set; } = null;

        public static void Str(object obj, string filter = null, bool showEverything = false, Type context = null)
        {
            Str(obj.GetType(), filter: filter, showEverything: showEverything, context: context);
        }

        public static void Str<T>(string filter = null, bool showEverything = false, Type context = null)
        {
            Str(typeof(T), filter: filter, showEverything: showEverything, context: context);
        }

        public static void Str(Type tp = null, string @namespace = null, string filter = null, bool showEverything = false, Type context = null)
        {
            if (tp != null)
            {
                CSharpObjectStructure str = new CSharpObjectStructure(tp, filter, showEverything, context);

                consoleContainer.Children.Insert(consoleContainer.Children.Count - 1, str);
            }
            else
            {
                if (string.IsNullOrEmpty(@namespace))
                {
                    consoleContainer.Children.Insert(consoleContainer.Children.Count - 1, new CSharpOutputLine(null, true));
                }
                else
                {
                    consoleContainer.Children.Insert(consoleContainer.Children.Count - 1, new NamespaceStructure(@namespace, filter, showEverything, context, cSharpConsole));
                }
            }
        }


        public static DoNotAddOutputLine Clear()
        {
            cSharpConsole.FindControl<StackPanel>("consoleContainer").Children.Clear();
            cSharpConsole.FindControl<StackPanel>("consoleContainer").Children.Add(cSharpConsole.FindControl<ConsoleInputLine>("inputLine"));
            return new DoNotAddOutputLine();
        }

        public static ResetType Reset()
        {
            return new ResetType();
        }
    }

    public class OutputConsoleWriter : TextWriter
    {
        StackPanel consoleContainer;

        public OutputConsoleWriter(StackPanel ConsoleContainer)
        {
            consoleContainer = ConsoleContainer;
        }

        public async override void WriteLine(object obj)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                consoleContainer.Children.Insert(consoleContainer.Children.Count - 1, new CSharpOutputLine(obj, true));
            });
        }

        public override void WriteLine()
        {
            
        }

        public async override void WriteLine(string value)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                consoleContainer.Children.Insert(consoleContainer.Children.Count - 1, new CSharpOutputLine(value, true));
            });
        }

        public async override void Write(string value)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                consoleContainer.Children.Insert(consoleContainer.Children.Count - 1, new CSharpOutputLine(value, true));
            });
        }

        public async override void Write(char value)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                consoleContainer.Children.Insert(consoleContainer.Children.Count - 1, new CSharpOutputLine(value + "", true));
            });
        }

        public override void Write(object obj)
        {
            WriteLine(obj);
        }

        public override Encoding Encoding
        {
            get
            {
                return Encoding.Default;
            }
        }
    }


    public class AssemblyLoadException : Exception
    {
        public object Assembly { get; }

        public AssemblyLoadException(string message, Exception innerException, object assembly) : base(message, innerException)
        {
            this.Assembly = assembly;
        }
    }

    public class AssemblyNameParseException : Exception
    {
        public string Line { get; }

        public AssemblyNameParseException(string message, Exception innerException, string line) : base(message, innerException)
        {
            this.Line = line;
        }
    }
}
