using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace EatWork.Mobile.Utils
{
    /// <summary>
    /// The Title view
    /// </summary>
    [Preserve(AllMembers = true)]
    public class TitleView : Grid
    {
        #region Bindable Properties

        /// <summary>
        /// Gets or sets the LeadingViewProperty, and it is a bindable property.
        /// </summary>
        public static readonly BindableProperty LeadingViewProperty = BindableProperty.Create(nameof(LeadingView), typeof(View), typeof(TitleView), new ContentView(), BindingMode.Default, null, OnLeadingViewPropertyChanged);

        /// <summary>
        /// Gets or sets the TrailingViewProperty, and it is a bindable property.
        /// </summary>
        public static readonly BindableProperty TrailingViewProperty = BindableProperty.Create(nameof(TrailingView), typeof(View), typeof(TitleView), new ContentView(), BindingMode.Default, null, OnTrailingViewPropertyChanged);

        /// <summary>
        /// Gets or sets the ContentProperty, and it is a bindable property.
        /// </summary>
        public static readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(Content), typeof(View), typeof(TitleView), new ContentView(), BindingMode.TwoWay, null, OnContentPropertyChanged);

        /// <summary>
        /// Gets or sets the SepatatorProperty, and it is a bindable property.
        /// </summary>
        public static readonly BindableProperty SeparatorProperty = BindableProperty.Create(nameof(Separator), typeof(BoxView), typeof(TitleView), new BoxView(), BindingMode.Default, null, OnSeparatorPropertyChanged);

        /// <summary>
        /// Gets or sets the TitleProperty, and it is a bindable property.
        /// </summary>
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(TitleView), string.Empty, BindingMode.TwoWay, null, OnTitlePropertyChanged);

        /// <summary>
        /// Gets or sets the TitleProperty, and it is a bindable property.
        /// </summary>
        public static readonly BindableProperty TitleColorProperty = BindableProperty.Create(nameof(TitleColor), typeof(Color), typeof(TitleView), null, BindingMode.TwoWay, null, OnTitleColorPropertyChanged);

        /// <summary>
        /// Gets or sets the TitleProperty, and it is a bindable property.
        /// </summary>
        public static readonly BindableProperty SeparatorColorProperty = BindableProperty.Create(nameof(SeparatorColor), typeof(Color), typeof(TitleView), null, BindingMode.Default, null, OnSeparatorColorPropertyChanged);

        /// <summary>
        /// Gets or sets the FontFamilyProperty, and it is a bindable property.
        /// </summary>
        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(TitleView), string.Empty, BindingMode.Default, null, OnFontFamilyPropertyChanged);

        /// <summary>
        /// Gets or sets the FontAttributesProperty, and it is a bindable property.
        /// </summary>
        public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(nameof(FontAttributes), typeof(FontAttributes), typeof(TitleView), FontAttributes.None, BindingMode.Default, null, OnFontAttributesPropertyChanged);

        /// <summary>
        /// Gets or sets the FontSizeProperty, and it is a bindable property.
        /// </summary>
        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(TitleView), 16d, BindingMode.Default, null, OnFontSizePropertyChanged);

        #endregion Bindable Properties

        #region variables

        /// <summary>
        /// Gets or sets the title label.
        /// </summary>
        private Label titleLabel;

        #endregion variables

        #region Constructor

        public TitleView()
        {
            this.titleLabel = new Label() { Text = string.Empty };

            this.ColumnSpacing = 0;
            this.RowSpacing = 8;
            this.Padding = new Thickness(0, 8, 1, 0);

            this.ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = 4 },
                //new ColumnDefinition(),
                //new ColumnDefinition(),
                //new ColumnDefinition(),
                new ColumnDefinition() { Width = GridLength.Auto },
                new ColumnDefinition() { Width = GridLength.Star },
                new ColumnDefinition() { Width = GridLength.Auto },
                new ColumnDefinition { Width = 1 },
            };

            this.RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = 1 }
            };

            //var boxView = new BoxView { Color = (Color)Application.Current.Resources["Gray-200"] };
            Separator = new BoxView { Color = (Color)Application.Current.Resources["Transparent"] };

            //Children.Add(this.LeadingView, 1, 0);
            Children.Add(this.Content, 0, 0);
            SetColumnSpan(this.Content, 3);
            Children.Add(this.TrailingView, 3, 0);
            Children.Add(Separator, 0, 1);
            SetColumnSpan(Separator, 5);

            //SfGradientView gradientView = new SfGradientView();
            //SfLinearGradientBrush linearGradientBrush = new SfLinearGradientBrush();
            //linearGradientBrush.GradientStops = new Syncfusion.XForms.Graphics.GradientStopCollection()
            //{
            //    new SfGradientStop(){Color = (Color)Application.Current.Resources["PrimaryColor"], Offset=0.0},
            //    new SfGradientStop(){Color = (Color)Application.Current.Resources["PrimaryGradient"], Offset=1.0},
            //};

            //gradientView.BackgroundBrush = linearGradientBrush;
            //this.Children.Add(gradientView);
        }

        #endregion Constructor

        #region Public Properties

        /// <summary>
        /// Gets or sets the LeadingView.
        /// </summary>
        public View LeadingView
        {
            get { return (View)GetValue(LeadingViewProperty); }
            set { this.SetValue(LeadingViewProperty, value); }
        }

        /// <summary>
        /// Gets or sets the TrailingView.
        /// </summary>
        public View TrailingView
        {
            get { return (View)GetValue(TrailingViewProperty); }
            set { this.SetValue(TrailingViewProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Content.
        /// </summary>
        public View Content
        {
            get { return (View)GetValue(ContentProperty); }
            set { this.SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Content.
        /// </summary>
        private BoxView Separator
        {
            get { return (BoxView)GetValue(SeparatorProperty); }
            set { this.SetValue(SeparatorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        public Color TitleColor
        {
            get { return (Color)GetValue(TitleColorProperty); }
            set { this.SetValue(TitleColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Title. SeparatorColor
        /// </summary>
        public Color SeparatorColor
        {
            get { return (Color)GetValue(SeparatorColorProperty); }
            set { this.SetValue(SeparatorColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the FontFamily.
        /// </summary>
        public string FontFamily
        {
            get { return (string)GetValue(FontFamilyProperty); }
            set { this.SetValue(FontFamilyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the FontAttributes.
        /// </summary>
        public FontAttributes FontAttributes
        {
            get { return (FontAttributes)GetValue(FontAttributesProperty); }
            set { this.SetValue(FontAttributesProperty, value); }
        }

        /// <summary>
        /// Gets or sets the FontSize.
        /// </summary>
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { this.SetValue(FontSizeProperty, value); }
        }

        #endregion Public Properties

        #region Methods

        /// <summary>
        /// Invoked when the leading view is changed.
        /// </summary>
        /// <param name="bindable">The TitleView</param>
        /// <param name="oldValue">The old value</param>
        /// <param name="newValue">The new value</param>
        private static void OnLeadingViewPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var titleView = bindable as TitleView;
            var newView = (View)newValue;

            var title = titleView.titleLabel;

            newView.HorizontalOptions = LayoutOptions.Start;
            titleView.Children.Add(newView, 1, 0);

            //update content
            if (!string.IsNullOrEmpty(titleView.Title))
            {
                titleView.Children.Remove(titleView.titleLabel);
                title.HorizontalOptions = LayoutOptions.Start;
                titleView.Children.Add(title, 2, 0);
            }
        }

        /// <summary>
        /// Invoked when the trailing view is changed.
        /// </summary>
        /// <param name="bindable">The TitleView</param>
        /// <param name="oldValue">The old value</param>
        /// <param name="newValue">The new value</param>
        private static void OnTrailingViewPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var titleView = bindable as TitleView;
            var newView = (View)newValue;
            newView.HorizontalOptions = LayoutOptions.End;
            titleView.Children.Add(newView, 3, 0);
        }

        /// <summary>
        /// Invoked when the Content is changed.
        /// </summary>
        /// <param name="bindable">The TitleView</param>
        /// <param name="oldValue">The old value</param>
        /// <param name="newValue">The new value</param>
        private static void OnContentPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var titleView = bindable as TitleView;
            var newView = (View)newValue;

            if (!string.IsNullOrEmpty(titleView.Title))
            {
                titleView.Children.Remove(titleView.titleLabel);
            }
            newView.HorizontalOptions = LayoutOptions.Start;
            titleView.Children.Add(newView, 2, 0);

            //titleView.Children.Add(newView, 1, 0);
        }

        /// <summary>
        /// Invoked when the Separator is changed.
        /// </summary>
        /// <param name="bindable">The TitleView</param>
        /// <param name="oldValue">The old value</param>
        /// <param name="newValue">The new value</param>
        private static void OnSeparatorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var titleView = bindable as TitleView;
            var newView = (BoxView)newValue;

            if (titleView.Separator != null)
            {
                titleView.Children.Remove(titleView.Separator);
            }

            //newView.HorizontalOptions = LayoutOptions.Start;
            //titleView.Children.Add(newView, 2, 0);
            //newView = new BoxView { Color = (Color)Application.Current.Resources["Transparent"] };

            newView.Color = (Color)Application.Current.Resources["Transparent"];
            titleView.Children.Add(newView, 0, 1);
            SetColumnSpan(newView, 5);
        }

        /// <summary>
        /// Invoked when the Title is changed.
        /// </summary>
        /// <param name="bindable">The TitleView</param>
        /// <param name="oldValue">The old value</param>
        /// <param name="newValue">The new value</param>
        private static void OnTitlePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var titleView = bindable as TitleView;
            var newText = (string)newValue;

            if (!string.IsNullOrEmpty(newText))
            {
                titleView.titleLabel = new Label
                {
                    Text = newText,
                    TextColor = (Color)Application.Current.Resources["Gray-900"],
                    //FontSize = 16,
                    FontSize = 18,
                    //Margin = new Thickness(0, 8),
                    Margin = new Thickness(24, 8, 0, 8),
                    FontFamily = Device.RuntimePlatform == Device.Android
                            ? "Montserrat-Medium.ttf#Montserrat-Medium"
                            : Device.RuntimePlatform == Device.iOS
                                ? "Montserrat-Medium"
                                : "Assets/Montserrat-Medium.ttf#Montserrat-Medium",
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    LineBreakMode = LineBreakMode.TailTruncation,
                };

                if (Device.RuntimePlatform == Device.Android)
                {
                    titleView.titleLabel.LineHeight = 1.5;
                }

                titleView.Children.Remove(titleView.Content);
                //titleView.Children.Add(titleView.titleLabel, 2, 0);
                titleView.Children.Add(titleView.titleLabel, 0, 0);
                SetColumnSpan(titleView.titleLabel, 3);
            }
        }

        /// <summary>
        /// Invoked when the Title is changed.
        /// </summary>
        /// <param name="bindable">The TitleView</param>
        /// <param name="oldValue">The old value</param>
        /// <param name="newValue">The new value</param>
        private static void OnTitleColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var titleView = bindable as TitleView;
            var newText = (Color)newValue;

            titleView.titleLabel.TextColor = newText;
        }

        /// <summary>
        /// Invoked when the Title is changed.
        /// </summary>
        /// <param name="bindable">The TitleView</param>
        /// <param name="oldValue">The old value</param>
        /// <param name="newValue">The new value</param>
        private static void OnSeparatorColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var titleView = bindable as TitleView;
            var newText = (Color)newValue;

            if (titleView.Separator != null)
            {
                titleView.Children.Remove(titleView.Separator);
            }

            var boxView = new BoxView() { Color = newText };
            titleView.Children.Add(boxView, 0, 1);
            SetColumnSpan(boxView, 5);

            //titleView.titleLabel.TextColor = newText;
        }

        /// <summary>
        /// Invoked when the FontFamily is changed.
        /// </summary>
        /// <param name="bindable">The TitleView</param>
        /// <param name="oldValue">The old value</param>
        /// <param name="newValue">The new value</param>
        private static void OnFontFamilyPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var titleView = bindable as TitleView;

            if (titleView.titleLabel != null)
            {
                titleView.titleLabel.FontFamily = (string)newValue;
            }
        }

        /// <summary>
        /// Invoked when the FontAttributes is changed.
        /// </summary>
        /// <param name="bindable">The TitleView</param>
        /// <param name="oldValue">The old value</param>
        /// <param name="newValue">The new value</param>
        private static void OnFontAttributesPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var titleView = bindable as TitleView;

            if (titleView.titleLabel != null)
            {
                titleView.titleLabel.FontAttributes = (FontAttributes)newValue;
            }
        }

        /// <summary>
        /// Invoked when the FontSize is changed.
        /// </summary>
        /// <param name="bindable">The TitleView</param>
        /// <param name="oldValue">The old value</param>
        /// <param name="newValue">The new value</param>
        private static void OnFontSizePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var titleView = bindable as TitleView;

            if (titleView.titleLabel != null)
            {
                titleView.titleLabel.FontSize = (double)newValue;
            }
        }

        #endregion Methods
    }
}