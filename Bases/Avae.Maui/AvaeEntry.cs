using Avae.Abstractions;
using Microsoft.Maui.Controls.Shapes;
using System.ComponentModel;

namespace Avae.Maui
{
    public class AvaeEntry : ContentView
    {
        PathGeometry? geometry = new PathGeometryConverter().ConvertFromInvariantString("M14,7 A7,7 0 0,0 0,7 M0,7 A7,7 0 1,0 14,7 M7,3l0,5 M7,9l0,2") as PathGeometry;

        public static BindableProperty ColumnNameProperty = BindableProperty.Create(
                nameof(ColumnName), typeof(string), typeof(AvaeEntry));
        public string ColumnName
        {
            get => (string)GetValue(ColumnNameProperty);
            set => SetValue(ColumnNameProperty, value);
        }

        public static BindableProperty TextProperty = BindableProperty.Create(
                nameof(Text), typeof(string), typeof(AvaeEntry),
                defaultBindingMode: BindingMode.TwoWay);
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static BindableProperty UnderlineColorProperty = BindableProperty.Create(
                nameof(UnderlineColor), typeof(Color), typeof(AvaeEntry), Colors.Black);
        public Color UnderlineColor
        {
            get => (Color)GetValue(UnderlineColorProperty);
            set => SetValue(UnderlineColorProperty, value);
        }

        public static BindableProperty UnderlineThicknessProperty = BindableProperty.Create(
                nameof(UnderlineThickness), typeof(int), typeof(AvaeEntry), 0);
        public int UnderlineThickness
        {
            get => (int)GetValue(UnderlineThicknessProperty);
            set => SetValue(UnderlineThicknessProperty, value);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            this.ControlTemplate = new ControlTemplate(() =>
            {
                var grid = new Grid() { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto), } };
                var border = new Border
                {
                    Stroke = null,
                    StrokeThickness = 0
                };
                var entry = new Entry();
                entry.SetBinding(Entry.TextProperty, new Binding(nameof(Text), source: this));
                border.Content = entry;
                grid.Children.Add(border);
                //entry.SetBinding(Entry.TextColorProperty, new Binding(nameof(TextColor), source: this));
                //entry.SetBinding(Entry.PlaceholderProperty, new Binding(nameof(Placeholder), source: this));
                //entry.SetBinding(Entry.PlaceholderColorProperty, new Binding(nameof(PlaceholderColor), source: this));
                var underline = new Border
                {
                    Margin = new Thickness(0, 0, -50, 0),
                    Stroke = null,
                    StrokeThickness = 0
                };

                if (!string.IsNullOrWhiteSpace(ColumnName) &&
                    BindingContext is IDataErrorInfo errorInfo)
                {
                    if(BindingContext is IViewModelErrorInfo viewModel)
                    {
                        this.PropertyChanged += (s, e) =>
                        {
                            if (e.PropertyName == nameof(Text))
                            {
                                viewModel.RaiseErrorChanged();
                            }
                        };
                    }

                    var binding = new Binding($"[{ColumnName}]", source: errorInfo, converter: new NullConverter());

                    border.Triggers.Add(new DataTrigger(typeof(Border))
                    {
                        Binding = binding,
                        Setters =
                        {
                            new Setter()
                            {
                                Property = Border.StrokeProperty,
                                Value = Colors.Red
                            },
                            new Setter()
                            {
                                Property = Border.StrokeThicknessProperty,
                                Value = 0.5
                            }
                        },
                        Value = false
                    });

                    underline.Triggers.Add(new DataTrigger(typeof(Border))
                    {
                        Binding = binding,
                        Setters =
                        {
                            new Setter()
                            {
                                Property = Border.ContentProperty,
                                Value = new Microsoft.Maui.Controls.Shapes.Path
                                {
                                    Data =geometry,
                                    Fill = Colors.Transparent,
                                    Stroke = Colors.Red,
                                    StrokeThickness = 2,
                                    WidthRequest = 20,
                                    HeightRequest = 20
                                }
                            },
                            new Setter()
                            {
                                Property = ToolTipProperties.TextProperty,
                                Value = new Binding($"[{ColumnName}]", source: errorInfo)
                            }
                        },
                        Value = false
                    });
                }
                Grid.SetColumn(underline, 1);
                grid.Children.Add(underline);
                return grid;
            });
        }
    }
}
