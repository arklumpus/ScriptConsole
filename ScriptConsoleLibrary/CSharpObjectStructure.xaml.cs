using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ScriptConsoleLibrary
{
    internal class CSharpObjectStructure : UserControl
    {
        public CSharpObjectStructure()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void Initialize(Type tp, string filter, bool showEverything, Type context)
        {
            this.FindControl<MyExpandable>("expander").ExpandedChanged += ExpandedChanged;

            InputTextBox headerBox;

            StackPanel headerContainer = new StackPanel() { Orientation = Avalonia.Layout.Orientation.Horizontal };
            headerContainer.Children.Add(new InputTextBox() { FontSize = 12, Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, Padding = new Thickness(0, 1.5, 0, 0), MyReadOnly = true, FontStyle = Avalonia.Media.FontStyle.Italic, Text = "Structure: " });
            headerContainer.Children.Add(headerBox = new InputTextBox() { FontSize = 12, Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, Padding = new Thickness(0, 1.5, 0, 0), MyReadOnly = true, FontStyle = Avalonia.Media.FontStyle.Italic, Foreground = Brush.Parse("#2B91BC"), FontWeight = Avalonia.Media.FontWeight.Bold });

            this.FindControl<MyExpandable>("expander").Header = headerContainer;

            headerBox.Text = tp.Name;
            ToolTip.SetTip(headerBox, tp.FullName);

            List<MemberInfo> memberList = new List<MemberInfo>();
            MemberInfo[] members = tp.GetMembers(BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);


            int propInd = 0;
            int step = Math.Max(1, members.Length / 100);

            foreach (System.Reflection.MemberInfo prop in members)
            {
                if (string.IsNullOrEmpty(filter) || prop.Name.ToLower().Contains(filter.ToLower()))
                {
                    memberList.Add(prop);
                }
                propInd++;
            }

            memberList = new List<System.Reflection.MemberInfo>(memberList.NaturalSort());

            bool hasMethods = false;
            bool hasProps = false;
            bool hasEvents = false;
            bool hasTypes = false;
            bool hasUnknown = false;

            for (int i = 0; i < memberList.Count; i++)
            {
                int index = i;

                if (memberList[i].MemberType == System.Reflection.MemberTypes.Constructor || memberList[i].MemberType == System.Reflection.MemberTypes.Method)
                {

                    bool isAccessible = false;

                    if (memberList[i].MemberType == System.Reflection.MemberTypes.Constructor)
                    {
                        ConstructorInfo info = (ConstructorInfo)memberList[i];

                        isAccessible = info.IsPublic || (context != null && ((info.IsPrivate && info.DeclaringType == context) || (info.IsFamily && info.DeclaringType.IsAssignableFrom(context)) || (info.IsAssembly && info.DeclaringType.Assembly == context.Assembly) || (info.IsFamilyOrAssembly && (info.DeclaringType.IsAssignableFrom(context) || info.DeclaringType.Assembly == context.Assembly))));
                    }
                    else
                    {
                        MethodInfo info = (MethodInfo)memberList[i];

                        isAccessible = info.IsPublic || (context != null && ((info.IsPrivate && info.DeclaringType == context) || (info.IsFamily && info.DeclaringType.IsAssignableFrom(context)) || (info.IsAssembly && info.DeclaringType.Assembly == context.Assembly) || (info.IsFamilyOrAssembly && (info.DeclaringType.IsAssignableFrom(context) || info.DeclaringType.Assembly == context.Assembly))));
                    }

                    if (showEverything || isAccessible)
                    {
                        MethodStructure str = new MethodStructure(memberList[index], isAccessible);

                        this.FindControl<ReducedExpander>("methodsExpander").ChildrenContainer.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                        Grid.SetRow(str, this.FindControl<ReducedExpander>("methodsExpander").ChildrenContainer.Children.Count);
                        this.FindControl<ReducedExpander>("methodsExpander").ChildrenContainer.Children.Add(str);

                        hasMethods = true;
                    }
                }
                else if (memberList[i].MemberType == System.Reflection.MemberTypes.Field || memberList[i].MemberType == System.Reflection.MemberTypes.Property)
                {
                    bool isAccessible = false;

                    if (memberList[i].MemberType == System.Reflection.MemberTypes.Field)
                    {
                        FieldInfo info = (FieldInfo)memberList[i];

                        isAccessible = info.IsPublic || (context != null && ((info.IsPrivate && info.DeclaringType == context) || (info.IsFamily && info.DeclaringType.IsAssignableFrom(context)) || (info.IsAssembly && info.DeclaringType.Assembly == context.Assembly) || (info.IsFamilyOrAssembly && (info.DeclaringType.IsAssignableFrom(context) || info.DeclaringType.Assembly == context.Assembly))));
                    }
                    else
                    {
                        PropertyInfo pInfo = (PropertyInfo)memberList[i];

                        if (pInfo.GetMethod != null)
                        {
                            MethodInfo info = pInfo.GetMethod;

                            isAccessible = info.IsPublic || (context != null && ((info.IsPrivate && info.DeclaringType == context) || (info.IsFamily && info.DeclaringType.IsAssignableFrom(context)) || (info.IsAssembly && info.DeclaringType.Assembly == context.Assembly) || (info.IsFamilyOrAssembly && (info.DeclaringType.IsAssignableFrom(context) || info.DeclaringType.Assembly == context.Assembly))));
                        }
                        else if (pInfo.SetMethod != null)
                        {
                            MethodInfo info = pInfo.SetMethod;

                            isAccessible = info.IsPublic || (context != null && ((info.IsPrivate && info.DeclaringType == context) || (info.IsFamily && info.DeclaringType.IsAssignableFrom(context)) || (info.IsAssembly && info.DeclaringType.Assembly == context.Assembly) || (info.IsFamilyOrAssembly && (info.DeclaringType.IsAssignableFrom(context) || info.DeclaringType.Assembly == context.Assembly))));
                        }
                        else
                        {
                            isAccessible = false;
                        }
                    }

                    if (showEverything || isAccessible)
                    {
                        PropertyStructure str = new PropertyStructure(memberList[index], isAccessible);

                        this.FindControl<ReducedExpander>("propsExpander").ChildrenContainer.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                        Grid.SetRow(str, this.FindControl<ReducedExpander>("propsExpander").ChildrenContainer.Children.Count);
                        this.FindControl<ReducedExpander>("propsExpander").ChildrenContainer.Children.Add(str);
                        hasProps = true;
                    }
                }
                else if (memberList[i].MemberType == System.Reflection.MemberTypes.TypeInfo || memberList[i].MemberType == System.Reflection.MemberTypes.NestedType)
                {
                    bool isAccessible = false;

                    TypeInfo info = (TypeInfo)memberList[i];

                    isAccessible = info.IsPublic || info.IsNestedPublic || (context != null && ((info.IsNestedPrivate && info.DeclaringType == context) || (info.IsNestedFamily && info.DeclaringType.IsAssignableFrom(context)) || (info.IsNestedAssembly && info.DeclaringType.Assembly == context.Assembly) || (info.IsNestedFamORAssem && (info.DeclaringType.IsAssignableFrom(context) || info.DeclaringType.Assembly == context.Assembly))));

                    if (showEverything || isAccessible)
                    {
                        TypeStructure str = new TypeStructure((System.Reflection.TypeInfo)memberList[index], isAccessible);

                        this.FindControl<ReducedExpander>("typesExpander").ChildrenContainer.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                        Grid.SetRow(str, this.FindControl<ReducedExpander>("typesExpander").ChildrenContainer.Children.Count);
                        this.FindControl<ReducedExpander>("typesExpander").ChildrenContainer.Children.Add(str);
                        hasTypes = true;
                    }
                }
                else if (memberList[i].MemberType == System.Reflection.MemberTypes.Event)
                {
                    bool isAccessible = false;


                    MethodInfo info = ((EventInfo)memberList[i]).AddMethod;

                    if (info != null)
                    {
                        isAccessible = info.IsPublic || (context != null && ((info.IsPrivate && info.DeclaringType == context) || (info.IsFamily && info.DeclaringType.IsAssignableFrom(context)) || (info.IsAssembly && info.DeclaringType.Assembly == context.Assembly) || (info.IsFamilyOrAssembly && (info.DeclaringType.IsAssignableFrom(context) || info.DeclaringType.Assembly == context.Assembly))));
                    }

                    if (showEverything || isAccessible)
                    {
                        EventStructure str = new EventStructure((System.Reflection.EventInfo)memberList[index], isAccessible);

                        this.FindControl<ReducedExpander>("eventsExpander").ChildrenContainer.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                        Grid.SetRow(str, this.FindControl<ReducedExpander>("eventsExpander").ChildrenContainer.Children.Count);
                        this.FindControl<ReducedExpander>("eventsExpander").ChildrenContainer.Children.Add(str);
                        hasEvents = true;
                    }
                }
                else
                {
                    UnknownStructure str = new UnknownStructure(memberList[index]);

                    this.FindControl<ReducedExpander>("unknownExpander").ChildrenContainer.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                    Grid.SetRow(str, this.FindControl<ReducedExpander>("unknownExpander").ChildrenContainer.Children.Count);
                    this.FindControl<ReducedExpander>("unknownExpander").ChildrenContainer.Children.Add(str);
                    hasUnknown = true;
                }
            }

            Grid contentGrid = this.FindControl<Grid>("contentGrid");

            if (!hasMethods)
            {
                contentGrid.Children.Remove(this.FindControl<ReducedExpander>("methodsExpander"));
            }
            if (!hasProps)
            {
                contentGrid.Children.Remove(this.FindControl<ReducedExpander>("propsExpander"));
            }
            if (!hasEvents)
            {
                contentGrid.Children.Remove(this.FindControl<ReducedExpander>("eventsExpander"));
            }
            if (!hasTypes)
            {
                contentGrid.Children.Remove(this.FindControl<ReducedExpander>("typesExpander"));
            }
            if (!hasUnknown)
            {
                contentGrid.Children.Remove(this.FindControl<ReducedExpander>("unknownExpander"));
            }
        }
        public CSharpObjectStructure(Type tp, string filter, bool showEverything, Type context)
        {
            InitializeComponent();

            Initialize(tp, filter, showEverything, context);
        }

        private void ExpandedChanged(object sender, BoolChangedEventArgs e)
        {
            this.FindControl<Grid>("contentGrid").IsVisible = e.NewValue;
        }
    }
}
