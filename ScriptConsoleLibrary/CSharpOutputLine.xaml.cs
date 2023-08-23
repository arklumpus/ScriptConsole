using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ScriptConsoleLibrary
{
    internal class CSharpOutputLine : UserControl
    {
        public CSharpOutputLine()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }


        public CSharpOutputLine(dynamic output, bool log)
        {
            InitializeComponent();

            if (log)
            {
                this.FindControl<Canvas>("logIcon").IsVisible = true;
            }
            else
            {
                this.FindControl<Canvas>("outputIcon").IsVisible = true;
            }

            dynamic outObj = output;

            Control elem = contentElement(outObj, log);

            Grid.SetRow(elem, 1);
            Grid.SetColumn(elem, 1);
            this.FindControl<Grid>("outputGrid").Children.Add(elem);
        }

        public static Control contentElement(dynamic outObj, bool log = false)
        {
            if (outObj is string)
            {
                InputTextBox outputBox = new InputTextBox() { Foreground = new SolidColorBrush(Color.FromRgb(196, 26, 22)), Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, MyReadOnly = true, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, Padding = new Thickness(0, 1.5, 0, 0), Text = log ? outObj : (JsonConvert.SerializeObject(outObj)) };
                return outputBox;
            }
            else if (outObj is char)
            {
                string serialized = JsonConvert.SerializeObject(outObj.ToString());
                InputTextBox outputBox = new InputTextBox() { Foreground = new SolidColorBrush(Color.FromRgb(196, 26, 22)), Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, MyReadOnly = true, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, Padding = new Thickness(0, 1.5, 0, 0), Text = "'" + serialized.Substring(1, serialized.Length - 2) + "'" };
                return outputBox;
            }
            else if (outObj is long || outObj is int || outObj is double || outObj is decimal || outObj is ulong || outObj is uint || outObj is short || outObj is ushort || outObj is byte || outObj is sbyte || outObj is float)
            {
                InputTextBox outputBox = new InputTextBox() { Foreground = new SolidColorBrush(Color.FromRgb(28, 0, 207)), Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, MyReadOnly = true, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, Padding = new Thickness(0, 1.5, 0, 0), Text = outObj.ToString(System.Globalization.CultureInfo.InvariantCulture) };

                return outputBox;
            }
            else if (outObj is bool)
            {
                InputTextBox outputBox = new InputTextBox() { Foreground = new SolidColorBrush(Color.FromRgb(28, 0, 207)), Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, MyReadOnly = true, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, Padding = new Thickness(0, 1.5, 0, 0), Text = outObj.ToString().ToLower() };

                return outputBox;
            }
            else if (Object.ReferenceEquals(outObj, null))
            {
                InputTextBox outputBox = new InputTextBox() { Foreground = new SolidColorBrush(Color.FromRgb(151, 151, 151)), Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, MyReadOnly = true, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, Padding = new Thickness(0, 1.5, 0, 0), Text = "null" };

                return outputBox;
            }
            else if (isIEnumerable(outObj))
            {
                string enumerableType = "";

                foreach (Type interf in ((Type)outObj.GetType()).GetInterfaces())
                {
                    if (interf.IsGenericType && interf.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    {
                        foreach (Type tp in interf.GetGenericArguments())
                        {
                            enumerableType += tp.Name + ", ";
                        }
                    }
                }

                if (enumerableType.Length > 2)
                {
                    enumerableType = enumerableType.Substring(0, enumerableType.Length - 2);
                }

                Grid contentGrid = new Grid();
                contentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                contentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                contentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                contentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                string contentText = "";

                int count = 0;

                foreach (dynamic dyn in outObj)
                {
                    contentText += getShortString(dyn) + ", ";
                    count++;
                    if (count >= 10)
                    {
                        break;
                    }
                }

                if (contentText.Length > 2)
                {
                    contentText = contentText.Substring(0, contentText.Length - 2);
                }

                int itemCount = -1;

                try
                {
                    itemCount = outObj.Count;
                }
                catch
                {
                    try
                    {
                        itemCount = outObj.Length;
                    }
                    catch
                    {
                        try
                        {
                            itemCount = outObj.Count();
                        }
                        catch
                        {

                        }
                    }
                }

                if (itemCount >= 10 || (itemCount < 0 && count >= 10))
                {
                    contentText += "…";
                }


                InputTextBox contentBox = new InputTextBox() { Foreground = new SolidColorBrush(Color.FromRgb(151, 151, 151)), Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, MyReadOnly = true, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, Padding = new Thickness(0, 1.5, 0, 0), Text = "[" + contentText + "]" };
                Grid.SetColumn(contentBox, 1);
                contentGrid.Children.Add(contentBox);


                MyExpandable exp = new MyExpandable();

                InputTextBox outputBox = new InputTextBox() { FontSize = 12, Foreground = new SolidColorBrush(Colors.Black), Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, MyReadOnly = true, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, Padding = new Thickness(0, 1.2, 0, 0), Text = outObj.GetType().Name + " : IEnumerable<" + enumerableType + ">[" + (itemCount >= 0 ? itemCount.ToString() : "") + "]: " };

                exp.Header = outputBox;
                contentGrid.Children.Add(exp);

                Grid arrayContentGrid = new Grid() { Margin = new Thickness(20, 0, 0, 0), IsVisible = false };
                arrayContentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                arrayContentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                arrayContentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

                count = 0;

                foreach (dynamic dyn in outObj)
                {
                    int i = count;
                    arrayContentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                    InputTextBox index = new InputTextBox() { FontSize = 12, Foreground = new SolidColorBrush(Color.FromRgb(163, 73, 164)), Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, MyReadOnly = true, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right, Padding = new Thickness(0, 1.5, 0, 0), Text = i.ToString() };
                    Grid.SetRow(index, i);
                    arrayContentGrid.Children.Add(index);

                    TextBlock colon = new TextBlock() { FontSize = 12, Foreground = new SolidColorBrush(Colors.Black), Background = null, FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, Margin = new Thickness(0, 1.5, 3, 0), Text = ":" };
                    Grid.SetRow(colon, i);
                    Grid.SetColumn(colon, 1);
                    arrayContentGrid.Children.Add(colon);

                    Control value = contentElement(dyn);

                    Grid.SetRow(value, i);
                    Grid.SetColumn(value, 2);
                    arrayContentGrid.Children.Add(value);

                    count++;
                }

                Grid.SetRow(arrayContentGrid, 1);

                contentGrid.Children.Add(arrayContentGrid);

                exp.ExpandedChanged += (s, e) =>
                {
                    arrayContentGrid.IsVisible = e.NewValue;
                    contentBox.IsVisible = !e.NewValue;
                };

                return contentGrid;
            }
            else if (outObj is Enum)
            {
                StackPanel tbr = new StackPanel() { Orientation = Avalonia.Layout.Orientation.Horizontal };
                InputTextBox bx = new InputTextBox() { Foreground = new SolidColorBrush(Color.FromRgb(0x2b, 0x91, 0xbc)), Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, MyReadOnly = true, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, Padding = new Thickness(0, 1.5, 0, 0), Text = outObj.GetType().Name, FontStyle = FontStyle.Italic };
                ToolTip.SetTip(bx, outObj.GetType().FullName);
                tbr.Children.Add(bx);
                tbr.Children.Add(new InputTextBox() { Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, MyReadOnly = true, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, Padding = new Thickness(0, 1.5, 0, 0), Text = ".", Margin = new Thickness(0, 0, 0, 0) });
                tbr.Children.Add(new InputTextBox() { Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, MyReadOnly = true, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, Padding = new Thickness(0, 1.5, 0, 0), Text = outObj.ToString(), Margin = new Thickness(0, 0, 0, 0) });
                return tbr;
            }
            else
            {
                List<System.Reflection.MemberInfo> propertyList = new List<System.Reflection.MemberInfo>();

                List<string> propertyNames = new List<string>();

                foreach (System.Reflection.MemberInfo prop in ((Type)outObj.GetType()).GetMembers())
                {
                    if (prop.MemberType == System.Reflection.MemberTypes.Property || prop.MemberType == System.Reflection.MemberTypes.Field)
                    {
                        propertyList.Add(prop);
                        propertyNames.Add(prop.Name);
                    }
                }

                Grid contentGrid = new Grid();
                contentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                contentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                contentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                contentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                
                string contentText = "";

                bool moreThan10Props = (from el in propertyList where (el.MemberType == MemberTypes.Field && !((FieldInfo)el).IsStatic) || (el.MemberType == MemberTypes.Property && !((((PropertyInfo)el).GetMethod != null && ((PropertyInfo)el).GetMethod.IsStatic) || (((PropertyInfo)el).SetMethod != null && ((PropertyInfo)el).SetMethod.IsStatic))) select el).Count() > 10;

                int nonStaticCount = 0;

                for (int i = 0; i < propertyList.Count && nonStaticCount < 10; i++)
                {
                    if (propertyList[i].MemberType == System.Reflection.MemberTypes.Property)
                    {
                        if (((PropertyInfo)propertyList[i]).PropertyType != typeof(System.Reflection.MethodBase) && !((((PropertyInfo)propertyList[i]).GetMethod != null && ((PropertyInfo)propertyList[i]).GetMethod.IsStatic) || (((PropertyInfo)propertyList[i]).SetMethod != null && ((PropertyInfo)propertyList[i]).SetMethod.IsStatic)))
                        {
                            contentText += propertyNames[i] + ": " + getShortString(((PropertyInfo)propertyList[i]).GetValue(outObj)) + ", ";
                            nonStaticCount++;
                        }
                    }
                    else if (propertyList[i].MemberType == MemberTypes.Field)
                    {
                        if (((FieldInfo)propertyList[i]).FieldType != typeof(System.Reflection.MethodBase) && !((FieldInfo)propertyList[i]).IsStatic)
                        {
                            contentText += propertyNames[i] + ": " + getShortString(((FieldInfo)propertyList[i]).GetValue(outObj)) + ", ";
                            nonStaticCount++;
                        }
                    }
                }

                if (contentText.Length > 2)
                {
                    contentText = contentText.Substring(0, contentText.Length - 2);
                }

                if (moreThan10Props)
                {
                    contentText += "…";
                }


                InputTextBox contentBox = new InputTextBox() { Foreground = new SolidColorBrush(Color.FromRgb(151, 151, 151)), Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, MyReadOnly = true, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, Padding = new Thickness(0, 1.5, 0, 0), FontStyle = FontStyle.Italic, Text = "{" + contentText + "}" };
                Grid.SetColumn(contentBox, 1);
                contentGrid.Children.Add(contentBox);


                MyExpandable exp = new MyExpandable();

                InputTextBox outputBox = new InputTextBox() { FontSize = 12, Foreground = new SolidColorBrush(Colors.Black), Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, MyReadOnly = true, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, Padding = new Thickness(0, 1.2, 0, 0), FontStyle = FontStyle.Italic, Text = outObj.GetType().Name + " " };



                exp.Header = outputBox;
                contentGrid.Children.Add(exp);

                Grid arrayContentGrid = new Grid() { Margin = new Thickness(20, 0, 0, 0), IsVisible = false };
                arrayContentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                arrayContentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                arrayContentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });



                Grid.SetRow(arrayContentGrid, 1);

                contentGrid.Children.Add(arrayContentGrid);

                bool contentBuilt = false;

                exp.ExpandedChanged += (s, e) =>
                {
                    if (e.NewValue)
                    {
                        if (!contentBuilt)
                        {
                            contentBuilt = true;

                            try
                            {
                                int rowIndex = 0;

                                for (int i = 0; i < propertyList.Count; i++)
                                {
                                    Type fType = propertyList[i].MemberType == MemberTypes.Field ? ((FieldInfo)propertyList[i]).FieldType : ((PropertyInfo)propertyList[i]).PropertyType;

                                    bool isStatic = (propertyList[i].MemberType == MemberTypes.Field && ((FieldInfo)propertyList[i]).IsStatic);

                                    if (propertyList[i].MemberType == MemberTypes.Property)
                                    {
                                        isStatic = (((PropertyInfo)propertyList[i]).GetMethod != null && ((PropertyInfo)propertyList[i]).GetMethod.IsStatic) || (((PropertyInfo)propertyList[i]).SetMethod != null && ((PropertyInfo)propertyList[i]).SetMethod.IsStatic);
                                    }

                                    if (fType != typeof(System.Reflection.MethodBase) && !isStatic)
                                    {
                                        arrayContentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                                        InputTextBox index = new InputTextBox() { FontSize = 12, Foreground = new SolidColorBrush(Color.FromRgb(163, 73, 164)), Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, MyReadOnly = true, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right, Padding = new Thickness(0, 1.5, 0, 0), Text = propertyNames[i] };
                                        Grid.SetRow(index, rowIndex);
                                        arrayContentGrid.Children.Add(index);

                                        TextBlock colon = new TextBlock() { FontSize = 12, Foreground = new SolidColorBrush(Colors.Black), Background = null, FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, Margin = new Thickness(0, 1.5, 0, 0), Text = ":" };
                                        Grid.SetRow(colon, rowIndex);
                                        Grid.SetColumn(colon, 1);
                                        arrayContentGrid.Children.Add(colon);

                                        try
                                        {
                                            Control value = contentElement(propertyList[i].MemberType == MemberTypes.Field ? ((FieldInfo)propertyList[i]).GetValue(outObj) : ((PropertyInfo)propertyList[i]).GetValue(outObj));

                                            Grid.SetRow(value, rowIndex);
                                            Grid.SetColumn(value, 2);
                                            arrayContentGrid.Children.Add(value);
                                        }
                                        catch (Exception ex)
                                        {
                                            Control value = new ErrorLine(ex);
                                            Grid.SetRow(value, rowIndex);
                                            Grid.SetColumn(value, 2);
                                            arrayContentGrid.Children.Add(value);
                                        }
                                        rowIndex++;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                arrayContentGrid.Children.Clear();
                                arrayContentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                                Control errVal = new ErrorLine(ex);
                                arrayContentGrid.ColumnDefinitions.Clear();
                                arrayContentGrid.Children.Add(errVal);
                            }
                        }

                        arrayContentGrid.IsVisible = true;
                        contentBox.IsVisible = false;
                    }
                    else
                    {
                        arrayContentGrid.IsVisible = false;
                        contentBox.IsVisible = true;
                    }
                };

                return contentGrid;
            }
        }

        public static string getShortString(dynamic outObj)
        {
            if (outObj is string)
            {
                return JsonConvert.SerializeObject(outObj.ToString());
            }
            else if (outObj is char)
            {
                return "'" + outObj + "'";
            }
            else if (outObj is long || outObj is int || outObj is double || outObj is decimal || outObj is ulong || outObj is uint || outObj is short || outObj is ushort || outObj is byte || outObj is sbyte)
            {
                return outObj.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (outObj is bool)
            {
                return outObj.ToString().ToLower();
            }
            else if (Object.ReferenceEquals(outObj, null))
            {
                return "null";
            }
            else if (isIEnumerable(outObj))
            {
                string enumerableType = "";

                foreach (Type interf in ((Type)outObj.GetType()).GetInterfaces())
                {
                    if (interf.IsGenericType && interf.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    {
                        foreach (Type tp in interf.GetGenericArguments())
                        {
                            enumerableType += tp.Name + ", ";
                        }
                    }
                }

                if (enumerableType.Length > 2)
                {
                    enumerableType = enumerableType.Substring(0, enumerableType.Length - 2);
                }

                int itemCount = -1;

                try
                {
                    itemCount = outObj.Count;
                }
                catch
                {
                    try
                    {
                        itemCount = outObj.Length;
                    }
                    catch
                    {
                        try
                        {
                            itemCount = outObj.Count();
                        }
                        catch
                        {

                        }
                    }
                }

                return outObj.GetType().Name + " : IEnumerable<" + enumerableType + ">[" + (itemCount >= 0 ? itemCount.ToString() : "") + "]";
            }
            else if (outObj is Enum)
            {
                return outObj.GetType().Name + "." + outObj.ToString();
            }
            else
            {
                return outObj.GetType().Name;
            }
        }

        public static bool isNotRecognized(dynamic outObj)
        {
            if (outObj is string)
            {
                return false;
            }
            else if (outObj is char)
            {
                return false;
            }
            else if (outObj is long || outObj is int || outObj is double || outObj is decimal || outObj is ulong || outObj is uint || outObj is short || outObj is ushort || outObj is byte || outObj is sbyte)
            {
                return false;
            }
            else if (outObj is bool)
            {
                return false;
            }
            else if (Object.ReferenceEquals(outObj, null))
            {
                return false;
            }
            else if (isIEnumerable(outObj))
            {
                return false;
            }
            else if (outObj is Enum)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool isIEnumerable(dynamic obj)
        {
            var enumerable = obj as System.Collections.IEnumerable;

            if (enumerable != null)
            {
                return true;
            }
            return false;
        }
    }
}
