#region License

// MIT License
//
// Copyright (c) 2018 Marcus Technical Services, Inc. http://www.marcusts.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT
// OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion License

#define HACK_XAMARIN_SELECTION_BUG
#define CALL_FORCE_STYLE
// #define ANIMATE #define HACK_BROKEN_BUTTON_STATE_BINDINGS

namespace SharedForms.Views.Controls
{
   #region Imports

   using Common.Interfaces;
   using Common.Utils;
   using PropertyChanged;
   using System;
   using System.Collections.Generic;
   using System.Collections.ObjectModel;
   using System.Diagnostics;
   using Xamarin.Forms;
   using XFShapeView;

   #endregion Imports

   public interface IGenericViewButtonBase<T> : IHaveButtonState, IDisposable
      where T : View
   {
      Command ButtonCommand { get; set; }

      string ButtonCommandBindingName { get; set; }

      IValueConverter ButtonCommandConverter { get; set; }

      object ButtonCommandConverterParameter { get; set; }

      object ButtonCommandSource { get; set; }

      Style SelectedButtonStyle { get; set; }

      Style DeselectedButtonStyle { get; set; }

      Style DisabledButtonStyle { get; set; }

      T InternalView { get; set; }

      double? CornerRadiusFixed { get; set; }

      double? CornerRadiusFactor { get; set; }

      bool CanSelect { get; set; }

      bool ToggleSelection { get; set; }
   }

   /// <summary>
   /// A button that takes a view to lay on top of it -- can be a label image, etc. NOTE that we need
   /// our own property change handling, since we have no view model. ISSUES:
   /// * On any setting or style change, we need to re-render !!!
   /// * Rendering needs to take the various appearance settings into account, especially radius,
   ///   background color, etc. NEEDS:
   /// * Tap effects -- to grow or shake or vibrate on tap -- ?
   /// * Border Thickness
   /// * Border Color
   /// * Shadow Properties
   /// </summary>
   [AddINotifyPropertyChangedInterface]
   public abstract class GenericViewButtonBase<T> : ShapeView, IGenericViewButtonBase<T>
      where T : View
   {
      public static readonly BindableProperty ButtonStateProperty =
         CreateGenericViewButtonBindableProperty
         (
            nameof(ButtonState),
            default(ButtonStates),
            BindingMode.TwoWay,
            (viewButton, oldVal, newVal) =>
            {
               viewButton.ButtonState = newVal;
               viewButton.HandleButtonStateChanged();
            }
         );

      public static readonly BindableProperty ButtonCommandProperty =
         CreateGenericViewButtonBindableProperty
         (
            nameof(ButtonCommand),
            default(Command),
            BindingMode.OneWay,
            (viewButton, oldVal, newVal) => { viewButton.ButtonCommand = newVal; }
         );

      public static readonly BindableProperty ButtonCommandBindingNameProperty =
         CreateGenericViewButtonBindableProperty
         (
            nameof(ButtonCommandBindingName),
            default(string),
            BindingMode.OneWay,
            (viewButton, oldVal, newVal) => { viewButton.ButtonCommandBindingName = newVal; }
         );

      public static readonly BindableProperty ButtonCommandConverterProperty =
         CreateGenericViewButtonBindableProperty
         (
            nameof(ButtonCommandConverter),
            default(IValueConverter),
            BindingMode.OneWay,
            (viewButton, oldVal, newVal) => { viewButton.ButtonCommandConverter = newVal; }
         );

      public static readonly BindableProperty ButtonCommandConverterParameterProperty =
         CreateGenericViewButtonBindableProperty
         (
            nameof(ButtonCommandConverterParameter),
            default(object),
            BindingMode.OneWay,
            (viewButton, oldVal, newVal) => { viewButton.ButtonCommandConverterParameter = newVal; }
         );

      public static readonly BindableProperty SelectedStyleProperty =
         CreateGenericViewButtonBindableProperty
         (
            nameof(SelectedButtonStyle),
            default(Style),
            BindingMode.OneWay,
            (viewButton, oldVal, newVal) => { viewButton.SelectedButtonStyle = newVal; }
         );

      public static readonly BindableProperty DeselectedStyleProperty =
         CreateGenericViewButtonBindableProperty
         (
            nameof(DeselectedButtonStyle),
            default(Style),
            BindingMode.OneWay,
            (viewButton, oldVal, newVal) => { viewButton.DeselectedButtonStyle = newVal; }
         );

      public static readonly BindableProperty DisabledStyleProperty =
         CreateGenericViewButtonBindableProperty
         (
            nameof(DisabledButtonStyle),
            default(Style),
            BindingMode.OneWay,
            (viewButton, oldVal, newVal) => { viewButton.DisabledButtonStyle = newVal; }
         );

      public static readonly BindableProperty CornerRadiusFixedProperty =
         CreateGenericViewButtonBindableProperty
         (
            nameof(CornerRadiusFixed),
            default(double?),
            BindingMode.OneWay,
            (viewButton, oldVal, newVal) => { viewButton.CornerRadiusFixed = newVal; }
         );

      public static readonly BindableProperty CornerRadiusFactorProperty =
         CreateGenericViewButtonBindableProperty
         (
            nameof(CornerRadiusFactor),
            default(double?),
            BindingMode.OneWay,
            (viewButton, oldVal, newVal) => { viewButton.CornerRadiusFactor = newVal; }
         );

      //---------------------------------------------------------------------------------------------------------------
      // VARIABLES
      //---------------------------------------------------------------------------------------------------------------

      private readonly TapGestureRecognizer _tapGesture = new TapGestureRecognizer();

      private Command _buttonCommand;

      private string _buttonCommandBindingName;

      private IValueConverter _buttonCommandConverter;

      private object _buttonCommandConverterParameter;

      private object _buttonCommandSource;

      private ButtonStates _buttonState;

      private double? _cornerRadiusFactor;

      private double? _cornerRadiusFixed;

      private Style _deselectedStyle;

      private Style _disabledStyle;

      private T _internalView;
      private volatile bool _internalViewEntered;

      private Style _selectedStyle;

      private int _selectionGroup;
      private volatile bool _tappedListenerEntered;
      private volatile bool _isReleasing;

      //---------------------------------------------------------------------------------------------------------------
      // CONSTRUCTOR
      //---------------------------------------------------------------------------------------------------------------

      protected GenericViewButtonBase
      (
         Color backColor = default(Color),
         double? buttonWidth = null,
         double? buttonHeight = null,
         double? cornerRadiusFixed = null,
         double? cornerRadiusFactor = null,
         double? borderWidth = null,
         Color borderColor = default(Color),
         string commandBindingPropertyName = default(string),
         IValueConverter commandBindingConverter = null,
         object commandBindingConverterParameter = null,
         object commandBindingSource = null,
         bool canSelect = true,
         int polygonSideCount = 0
      )
      {
         CanSelect = canSelect;

         if (backColor.IsAnEqualObjectTo(default(Color)))
         {
            backColor = Color.Transparent;
         }

         // TODO -is this used - ??? BackgroundColor = backColor;
         Color = backColor;

         _cornerRadiusFixed = cornerRadiusFixed;
         _cornerRadiusFactor = cornerRadiusFactor;

         SetCornerRadius();

         if (borderWidth.HasValue)
         {
            BorderWidth = Convert.ToSingle(borderWidth.GetValueOrDefault());
         }

         if (borderColor.IsNotAnEqualObjectTo(default(Color)))
         {
            BorderColor = borderColor;
         }

         // Set the fields so the properties do not react
         _buttonCommandBindingName = commandBindingPropertyName;
         _buttonCommandConverter = commandBindingConverter;
         _buttonCommandConverterParameter = commandBindingConverterParameter;
         _buttonCommandSource = commandBindingSource;

         SetUpCompleteViewButtonCommandBinding();

         IAmSelectedStatic += HandleStaticSelectionChanges;

         GestureRecognizers.Add(_tapGesture);
         _tapGesture.Tapped += HandleTapGestureTapped;

         if (buttonWidth.HasNoValue())
         {
            buttonWidth = FormsUtils.MAJOR_BUTTON_WIDTH;
         }

         if (buttonWidth.HasValue)
         {
            WidthRequest = buttonWidth.GetValueOrDefault();
         }

         if (buttonHeight.HasNoValue())
         {
            buttonHeight = FormsUtils.MAJOR_BUTTON_HEIGHT;
         }

         if (buttonHeight.HasValue)
         {
            HeightRequest = buttonHeight.GetValueOrDefault();
         }

         if ((buttonWidth.HasValue || buttonHeight.HasValue) && polygonSideCount > 0)
         {
            ShapeType = ShapeType.Path;
            Points = CreatePolygonPoints(polygonSideCount,
               buttonWidth.HasValue ? buttonWidth.GetValueOrDefault() : buttonHeight.GetValueOrDefault(),
               borderWidth.GetValueOrDefault());
         }
         else
         {
            ShapeType = ShapeType.Box;
         }

         SetStyle();
      }

      public event EventUtils.NoParamsDelegate ViewButtonPressed;

#if HACK_BROKEN_BUTTON_STATE_BINDINGS
      public event EventUtils.GenericDelegate<ButtonStates> ButtonStateChanged;
#endif

      //---------------------------------------------------------------------------------------------------------------
      // PROPERTIES (Public)
      //---------------------------------------------------------------------------------------------------------------

      public bool CanSelect { get; set; }

      public bool ToggleSelection { get; set; }

      public Command ButtonCommand
      {
         get => _buttonCommand;
         set
         {
            RemoveButtonCommandEventListener();

            _buttonCommand = value;

            if (ButtonCommand != null)
            {
               ButtonCommand.CanExecuteChanged += HandleButtonCommandCanExecuteChanged;

               // Force-fire the initial state
               ButtonCommand.ChangeCanExecute();
            }
         }
      }

      public string ButtonCommandBindingName
      {
         get => _buttonCommandBindingName;
         set
         {
            _buttonCommandBindingName = value;

            SetUpCompleteViewButtonCommandBinding();
         }
      }

      public IValueConverter ButtonCommandConverter
      {
         get => _buttonCommandConverter;
         set
         {
            _buttonCommandConverter = value;
            SetUpCompleteViewButtonCommandBinding();
         }
      }

      public object ButtonCommandConverterParameter
      {
         get => _buttonCommandConverterParameter;
         set
         {
            _buttonCommandConverterParameter = value;
            SetUpCompleteViewButtonCommandBinding();
         }
      }

      public object ButtonCommandSource
      {
         get => _buttonCommandSource;
         set
         {
            _buttonCommandSource = value;
            SetUpCompleteViewButtonCommandBinding();
         }
      }

      public Style DeselectedButtonStyle
      {
         get => _deselectedStyle;
         set
         {
            _deselectedStyle = value;
            SetStyle();
         }
      }

      public Style DisabledButtonStyle
      {
         get => _disabledStyle;
         set
         {
            _disabledStyle = value;
            SetStyle();
         }
      }

      public T InternalView
      {
         get => _internalView;
         set
         {
            if (_internalViewEntered || _isReleasing)
            {
               return;
            }

            _internalViewEntered = true;

            _internalView = value;

            //if (_internalView != null)
            //{
            try
            {
               Content = _internalView;

               AfterInternalViewSet();
            }
            catch (Exception e)
            {
               Debug.WriteLine("INTERNAL VIEW ASSIGNMENT ERROR ->" + e.Message + "<-");
            }
            //}

            _internalViewEntered = false;
         }
      }

      public ButtonStates ButtonState
      {
         get => (ButtonStates) GetValue(ButtonStateProperty);
         set => SetValue(ButtonStateProperty, value);
      }

      public event EventHandler<ButtonStates> ButtonStateChanged;

      public Style SelectedButtonStyle
      {
         get => _selectedStyle;
         set
         {
            _selectedStyle = value;
            SetStyle();
         }
      }

      /// <summary>
      /// Leave at 0 if multiple selection is OK
      /// </summary>
      public int SelectionGroup
      {
         get => _selectionGroup;
         set
         {
            _selectionGroup = value;
            BroadcastIfSelected();
         }
      }

      public double? CornerRadiusFixed
      {
         get => _cornerRadiusFixed;
         set
         {
            _cornerRadiusFixed = value;

            SetCornerRadius();
         }
      }

      public double? CornerRadiusFactor
      {
         get => _cornerRadiusFactor;
         set
         {
            _cornerRadiusFactor = value;

            SetCornerRadius();
         }
      }

      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }

      //---------------------------------------------------------------------------------------------------------------
      // EVENTS
      //---------------------------------------------------------------------------------------------------------------

      protected static event EventUtils.GenericDelegate<IGenericViewButtonBase<T>> IAmSelectedStatic;

      //---------------------------------------------------------------------------------------------------------------
      // METHODS - Protected
      //---------------------------------------------------------------------------------------------------------------

      protected virtual void AfterInternalViewSet()
      {
      }

      protected virtual void SetStyle()
      {
         Style newStyle;

         // Set the style based on being enabled/disabled
         if (ButtonState == ButtonStates.Disabled)
         {
            newStyle = DisabledButtonStyle ?? DeselectedButtonStyle;
         }
         else if (ButtonState == ButtonStates.Selected)
         {
            newStyle = SelectedButtonStyle ?? DeselectedButtonStyle;
         }
         else
         {
            newStyle = DeselectedButtonStyle;
         }

         if (newStyle != null && (Style == null || Style.IsNotAnEqualObjectTo(newStyle)))
         {
            Style = newStyle;

#if CALL_FORCE_STYLE
            // This library is not working well with styles, so forcing all settings manually
            this.ForceStyle(Style);
#endif
         }
      }

      private void SetUpCompleteViewButtonCommandBinding()
      {
         if (ButtonCommandBindingName.IsEmpty())
         {
            RemoveBinding(ButtonCommandProperty);
         }
         else
         {
            this.SetUpBinding
            (
               ButtonCommandProperty,
               ButtonCommandBindingName,
               BindingMode.OneWay,
               ButtonCommandConverter,
               ButtonCommandConverterParameter,
               null,
               ButtonCommandSource
            );
         }
      }

      private void SetCornerRadius()
      {
         if (CornerRadiusFactor.HasNoValue())
         {
            CornerRadiusFactor = FormsUtils.BUTTON_RADIUS_FACTOR;
         }

         if (CornerRadiusFactor.HasValue)
         {
            CornerRadius = Convert.ToSingle(Math.Min(Bounds.Width, Bounds.Height) * _cornerRadiusFactor);
         }
         else if (CornerRadiusFixed.HasValue)
         {
            CornerRadius = Convert.ToSingle(CornerRadiusFixed);
         }
         else
         {
            CornerRadius = Convert.ToSingle(FormsUtils.BUTTON_RADIUS_FACTOR);
         }
      }

      //---------------------------------------------------------------------------------------------------------------
      // EVENT HANDLERS
      //---------------------------------------------------------------------------------------------------------------

      private void BroadcastIfSelected()
      {
         if (ButtonState == ButtonStates.Selected && SelectionGroup > 0)
         {
            // Raise a static event to notify others in this selection group that they should be *deselected*
            IAmSelectedStatic?.Invoke(this);
         }
      }

      /// <summary>
      /// Set IsEnabled based on the command can execute
      /// </summary>
      private void HandleButtonCommandCanExecuteChanged(object sender, EventArgs e)
      {
         var newCanExecute = sender is Command senderAsCommand && senderAsCommand.CanExecute(this);

         IsEnabled = newCanExecute;

         // The control is not issuing a property change when we manually set IsEnabled, so handling
         // that case here. Cannot listen to property changes generally in this case.
         SetStyle();
      }

      /// <summary>
      /// Listens as an *instance* to a *static* event
      /// </summary>
      private void HandleStaticSelectionChanges(IGenericViewButtonBase<T> sender)
      {
         // Do not recur onto our own broadcast; also only respond to the same selection group.
         if (sender.SelectionGroup == SelectionGroup && !ReferenceEquals(sender, this) &&
             ButtonState == ButtonStates.Selected)
         {
            ButtonState = ButtonStates.Deselected;
         }
      }

      protected void HandleTapGestureTapped(object sender, EventArgs e)
      {
         if (_tappedListenerEntered || ButtonState == ButtonStates.Disabled)
         {
            return;
         }

         _tappedListenerEntered = true;

         if (CanSelect)
         {
            if (ToggleSelection)
            {
               ButtonState = ButtonState != ButtonStates.Selected ? ButtonStates.Selected : ButtonStates.Deselected;
            }
            else
            {
               ButtonState = ButtonStates.Selected;
            }
         }

         // If a command exists, fire it and reset our selected status to false; otherwise, leave the
         // selected state as it is.
         if (ButtonCommand != null)
         {
            Device.BeginInvokeOnMainThread
            (
#if ANIMATE
               async
#endif
               () =>
               {
#if ANIMATE
                     if (InternalView != null)
                     {
                        await InternalView.ScaleTo(0.95, 50, Easing.CubicOut);
                        await InternalView.ScaleTo(1, 50, Easing.CubicIn);
                     }
#endif

                  ButtonCommand.Execute(this);

                  // This means that we do not intend to maintain the button state.
                  if (SelectionGroup == 0)
                  {
                     // Revert the state to its default setting.
                     ButtonState = ButtonStates.Deselected;
                  }
               }
            );
         }

         _tappedListenerEntered = false;
      }

      protected override void OnSizeAllocated(double width, double height)
      {
         base.OnSizeAllocated(width, height);

         SetCornerRadius();
      }

      private void RemoveButtonCommandEventListener()
      {
         if (ButtonCommand != null)
         {
            ButtonCommand.CanExecuteChanged -= HandleButtonCommandCanExecuteChanged;
         }
      }

      private static Point DegreesToXY(float degrees, float radius, Point origin)
      {
         var xy = new Point();
         var radians = degrees * Math.PI / 180.0;

         xy.X = (int) (Math.Cos(radians) * radius + origin.X);
         xy.Y = (int) (Math.Sin(-radians) * radius + origin.Y);

         return xy;
      }

      private static Point[] CalculateVertices(int sides, int radius, int startingAngle, Point center)
      {
         if (sides < 3)
         {
            throw new ArgumentException("Polygon must have 3 sides or more.");
         }

         var points = new List<Point>();
         var step = 360.0f / sides;

         float angle = startingAngle; //starting angle
         for (double i = startingAngle; i < startingAngle + 360.0; i += step) //go in a full circle
         {
            points.Add(DegreesToXY(angle, radius, center)); //code snippet from above
            angle += step;
         }

         return points.ToArray();
      }

      private void HandleButtonStateChanged()
      {
         SetStyle();
         BroadcastIfSelected();
         ButtonStateChanged?.Invoke(this, _buttonState);
      }

      public static ObservableCollection<Point> CreatePolygonPoints(int sideCount, double widthHeight,
         double borderWidth)
      {
         var radius = (widthHeight / 2 - borderWidth).ToRoundedInt();

         return new ObservableCollection<Point>
         (
            CalculateVertices(sideCount, radius, 360 / sideCount / 2, new Point(radius, radius))
         );
      }

      //---------------------------------------------------------------------------------------------------------------
      // STATIC READ ONLY VARIABLES & METHODS
      //---------------------------------------------------------------------------------------------------------------

      public static Style CreateViewButtonStyle
      (
         Color backColor,
         double? borderWidth = null,
         Color borderColor = default(Color)
      )
      {
         return new Style(typeof(GenericViewButtonBase<T>))
         {
            Setters =
            {
               // Use the text color as the border color
               new Setter { Property = BorderColorProperty, Value = borderColor },
               new Setter { Property = BorderWidthProperty, Value = borderWidth.GetValueOrDefault() },

               // The deselected background is set for the underlying view, not the label
               new Setter { Property = ColorProperty, Value = backColor }
            }
         };
      }

      //---------------------------------------------------------------------------------------------------------------
      // BINDABLE PROPERTIES
      //---------------------------------------------------------------------------------------------------------------

      public static BindableProperty CreateGenericViewButtonBindableProperty<PropertyTypeT>
      (
         string localPropName,
         PropertyTypeT defaultVal = default(PropertyTypeT),
         BindingMode bindingMode = BindingMode.OneWay,
         Action<GenericViewButtonBase<T>, PropertyTypeT, PropertyTypeT> callbackAction = null
      )
      {
         return BindableUtils.CreateBindableProperty(localPropName, defaultVal, bindingMode, callbackAction);
      }

      //---------------------------------------------------------------------------------------------------------------
      // D I S P O S A L
      //---------------------------------------------------------------------------------------------------------------

      private void ReleaseUnmanagedResources()
      {
         _isReleasing = true;

         // Global static, so remove the handler
         IAmSelectedStatic -= HandleStaticSelectionChanges;

         _tapGesture.Tapped -= HandleTapGestureTapped;

         RemoveButtonCommandEventListener();
      }

      protected virtual void Dispose(bool disposing)
      {
         ReleaseUnmanagedResources();
         if (disposing)
         {
         }
      }

      ~GenericViewButtonBase()
      {
         Dispose(false);
      }
   }
}
