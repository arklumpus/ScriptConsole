using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Avalonia.Threading;

namespace ScriptConsoleLibrary
{
    internal class NamespaceStructureExpander : UserControl
    {
        public NamespaceStructureExpander()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public NamespaceStructureExpander(string @namespace, string filter, bool showEverything, Type context, ScriptConsoleControl consoleContext)
        {
            InitializeComponent();

            bool built = false;

            ((InputTextBox)((StackPanel)this.FindControl<ReducedExpander>("expander").Header).Children[1]).Text = @namespace;

            this.FindControl<ReducedExpander>("expander")._expander.ExpandedChanged += (s, e) =>
            {
                if (e.NewValue)
                {
                    if (!built)
                    {
                        try
                        {

                            List<Type> types = new List<Type>();

                            Dictionary<string, Assembly> myAssemblies = Assembly.GetExecutingAssembly().MyGetReferencedAssembliesRecursive();



                            consoleContext.lastTask = consoleContext.lastTask.Result.ContinueWithAsync("System.AppDomain.CurrentDomain.GetAssemblies()");
                            consoleContext.lastTask.Wait();

                            Assembly[] assemblies = (Assembly[])consoleContext.lastTask.Result.ReturnValue;



                            for (int i = 0; i < assemblies.Length; i++)
                            {

                                try
                                {
                                    types.AddRange(assemblies[i].GetTypes());
                                }
                                catch { }

                            }

                            List<string> childrenNamespaces = new List<string>();
                            List<Type> relevantTypes = new List<Type>();

                            for (int i = 0; i < types.Count; i++)
                            {
                                if (types[i].Namespace == @namespace && (string.IsNullOrEmpty(filter) || types[i].Name.ToLower().Contains(filter.ToLower())))
                                {
                                    relevantTypes.Add(types[i]);
                                }
                                else if (!string.IsNullOrEmpty(types[i].Namespace) && (types[i].Namespace.StartsWith(@namespace + ".") && types[i].Namespace.Length > @namespace.Length + 1 && (string.IsNullOrEmpty(filter) || (types[i].Name.ToLower().Contains(filter.ToLower()) || types[i].Namespace.ToLower().Contains(filter.ToLower())))))
                                {
                                    string childNamespaceName = types[i].Namespace;

                                    if (childNamespaceName.Substring(@namespace.Length + 1).Contains("."))
                                    {
                                        childNamespaceName = childNamespaceName.Substring(0, childNamespaceName.IndexOf(".", @namespace.Length + 1));
                                    }
                                    if (!childrenNamespaces.Contains(childNamespaceName))
                                    {
                                        childrenNamespaces.Add(childNamespaceName);
                                    }
                                }
                            }

                            int rowIndex = 0;

                            childrenNamespaces.Sort();

                            for (int i = 0; i < childrenNamespaces.Count; i++)
                            {
                                this.FindControl<ReducedExpander>("expander").ChildrenContainer.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });

                                NamespaceStructureExpander str = new NamespaceStructureExpander(childrenNamespaces[i], filter, showEverything, context, consoleContext);
                                Grid.SetColumn(str, 1);
                                Grid.SetRow(str, rowIndex);
                                this.FindControl<ReducedExpander>("expander").ChildrenContainer.Children.Add(str);
                                rowIndex++;
                            }

                            List<MemberInfo> sortedTypes = new List<MemberInfo>(from el in relevantTypes orderby el.Name ascending select el.GetTypeInfo());

                            for (int i = 0; i < sortedTypes.Count; i++)
                            {

                                TypeInfo info = (TypeInfo)sortedTypes[i];

                                bool isAccessible = info.IsPublic || info.IsNestedPublic || (context != null && ((info.IsNestedPrivate && info.DeclaringType == context) || (info.IsNestedFamily && info.DeclaringType.IsAssignableFrom(context)) || (info.IsNestedAssembly && info.DeclaringType.Assembly == context.Assembly) || (info.IsNestedFamORAssem && (info.DeclaringType.IsAssignableFrom(context) || info.DeclaringType.Assembly == context.Assembly))));

                                if (showEverything || isAccessible)
                                {
                                    this.FindControl<ReducedExpander>("expander").ChildrenContainer.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });
                                    TypeStructure str = new TypeStructure(info, isAccessible);
                                    Grid.SetColumn(str, 1);
                                    Grid.SetRow(str, rowIndex);
                                    this.FindControl<ReducedExpander>("expander").ChildrenContainer.Children.Add(str);
                                    rowIndex++;
                                }
                            }

                            built = true;
                        }
                        catch (Exception ex)
                        {
                            this.FindControl<ReducedExpander>("expander").ChildrenContainer.RowDefinitions.Clear();
                            this.FindControl<ReducedExpander>("expander").ChildrenContainer.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                            this.FindControl<ReducedExpander>("expander").ChildrenContainer.Children.Add(new ErrorLine(ex));
                        }
                    }
                }
            };
        }
    }
}
