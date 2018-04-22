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

#define CALL_FORCE_STYLE

namespace SharedForms.Views.Controls
{
   #region Imports

   using Common.Interfaces;
   using Common.Utils;
   using System;
   using System.Diagnostics;
   using Xamarin.Forms;

   #endregion Imports

   public interface ILabelButton : IGenericViewButtonBase<Label>
   {
      string LabelBindingName { get; set; }

      IValueConverter LabelBindingConverter { get; set; }

      object LabelBindingConverterParameter { get; set; }

      Style SelectedLabelStyle { get; set; }

      Style DeselectedLabelStyle { get; set; }

      Style DisabledLabelStyle { get; set; }
   }

   public class LabelButton : GenericViewButtonBase<Label>, ILabelButton
   {
      public static readonly BindableProperty LabelButtonBindingNameProperty =
         CreateLabelButtonBindableProperty
         (
            nameof(LabelBindingName),
            default(string),
            BindingMode.OneWay,
            (labelButton, oldVal, newVal) => { labelButton.LabelBindingName = newVal; }
         );

      public static readonly BindableProperty LabelButtonConverterProperty =
         CreateLabelButtonBindableProperty
         (
            nameof(LabelBindingConverter),
            default(IValueConverter),
            BindingMode.OneWay,
            (labelButton, oldVal, newVal) => { labelButton.LabelBindingConverter = newVal; }
         );

      public static readonly BindableProperty LabelButtonConverterParameterProperty =
         CreateLabelButtonBindableProperty
         (
            nameof(LabelBindingConverterParameter),
            default(object),
            BindingMode.OneWay,
            (labelButton, oldVal, newVal) => { labelButton.LabelBindingConverterParameter = newVal; }
         );

      public static readonly BindableProperty SelectedLabelStyleProperty =
         CreateLabelButtonBindableProperty
         (
            nameof(SelectedLabelStyle),
            default(Style),
            BindingMode.OneWay,
            (labelButton, oldVal, newVal) => { labelButton.SelectedLabelStyle = newVal; }
         );

      public static readonly BindableProperty DeselectedLabelStyleProperty =
         CreateLabelButtonBindableProperty
         (
            nameof(DeselectedLabelStyle),
            default(Style),
            BindingMode.OneWay,
            (labelButton, oldVal, newVal) => { labelButton.DeselectedLabelStyle = newVal; }
         );

      public static readonly BindableProperty DisabledLabelStyleProperty =
         CreateLabelButtonBindableProperty
         (
            nameof(DisabledLabelStyle),
            default(Style),
            BindingMode.OneWay,
            (labelButton, oldVal, newVal) => { labelButton.DisabledLabelStyle = newVal; }
         );

      private Style _deselectedLabelButtonStyle;

      private Style _disabledLabelButtonStyle;
      //---------------------------------------------------------------------------------------------------------------
      // VARIABLES
      //---------------------------------------------------------------------------------------------------------------

      private IValueConverter _labelBindingConverter;
      private object _labelBindingConverterParameter;
      private string _labelBindingName;
      private Style _selectedLabelButtonStyle;

      //---------------------------------------------------------------------------------------------------------------
      // CONSTRUCTOR
      //---------------------------------------------------------------------------------------------------------------

      /// <summary>
      /// These parameters are for the *deselected* state, but are generally shared with the other states.
      /// </summary>
      public LabelButton
      (
         string labelText = default(string),
         double fontSize = default(double),
         FontAttributes fontAttributes = default(FontAttributes),
         Color fontColor = default(Color),
         TextAlignment horizontalAlignment = TextAlignment.Center,
         TextAlignment verticalAlignment = TextAlignment.Center,
         LineBreakMode lineBreaks = LineBreakMode.TailTruncation,
         string labelBindingPropertyName = default(string),
         IValueConverter labelBindingConverter = null,
         object labelBindingConverterParameter = null,
         Color backColor = default(Color),
         double? buttonWidth = null,
         double? buttonHeight = null,
         double? cornerRadius = null,
         double? cornerRadiusFactor = null,
         double? borderWidth = null,
         Color borderColor = default(Color),
         string commandBindingPropertyName = default(string),
         IValueConverter commandBindingConverter = null,
         object commandBindingConverterParameter = null,
         object commandBindingSource = null,
         bool canSelect = true
      )
         : base
         (
            backColor,
            buttonWidth,
            buttonHeight,
            cornerRadius,
            cornerRadiusFactor,
            borderWidth,
            borderColor,
            commandBindingPropertyName,
            commandBindingConverter,
            commandBindingConverterParameter,
            commandBindingSource,
            canSelect
         )
      {
         if (fontColor.IsAnEqualObjectTo(default(Color)))
         {
            fontColor = Color.Black;
         }

         var calculatedFontSize =
            fontSize.IsNotEmpty()
               ? fontSize
               : FormsUtils.DEFAULT_TEXT_SIZE;

         // Set up the label with the deselected defaults -- it's easy because it just calls control utils
         InternalView =
            new Label
            {
               Text = labelText,
               HorizontalOptions = LayoutOptions.FillAndExpand,
               VerticalOptions = LayoutOptions.CenterAndExpand,
               HorizontalTextAlignment = horizontalAlignment,
               VerticalTextAlignment = verticalAlignment,
               LineBreakMode = lineBreaks,
               TextColor = fontColor,
               FontSize = Convert.ToSingle(calculatedFontSize),
               InputTransparent = true
            };

         if (fontAttributes.IsNotAnEqualObjectTo(default(FontAttributes)))
         {
            InternalView.FontAttributes = fontAttributes;
         }

         // Set up the label text binding (if provided)
         if (labelBindingPropertyName.IsNotEmpty())
         {
            InternalView.SetUpBinding(Label.TextProperty, labelBindingPropertyName, BindingMode.OneWay,
               labelBindingConverter, labelBindingConverterParameter);
         }
         else
         {
            Debug.WriteLine("LABEL BUTTON: Cannot set up label binding!");
         }

         // Set the fields so the properties do not react
         _labelBindingName = labelBindingPropertyName;
         _labelBindingConverter = labelBindingConverter;
         _labelBindingConverterParameter = labelBindingConverterParameter;

         // The label always has a transparent background
         BackgroundColor = Color.Transparent;
         InputTransparent = false;

         // Force-refresh the label styles; this will configure the label properly
         SetStyle();
      }

      //---------------------------------------------------------------------------------------------------------------
      // PROPERTIES - Public
      //---------------------------------------------------------------------------------------------------------------

      public IValueConverter LabelBindingConverter
      {
         get => _labelBindingConverter;
         set
         {
            _labelBindingConverter = value;
            SetUpCompleteLabelButtonBinding();
         }
      }

      public string LabelBindingName
      {
         get => _labelBindingName;
         set
         {
            _labelBindingName = value;

            SetUpCompleteLabelButtonBinding();
         }
      }

      public object LabelBindingConverterParameter
      {
         get => _labelBindingConverterParameter;
         set
         {
            _labelBindingConverterParameter = value;
            SetUpCompleteLabelButtonBinding();
         }
      }

      public Style SelectedLabelStyle
      {
         get => _selectedLabelButtonStyle;
         set
         {
            _selectedLabelButtonStyle = value;
            SetStyle();
         }
      }

      public Style DeselectedLabelStyle
      {
         get => _deselectedLabelButtonStyle;
         set
         {
            _deselectedLabelButtonStyle = value;
            SetStyle();
         }
      }

      public Style DisabledLabelStyle
      {
         get => _disabledLabelButtonStyle;
         set
         {
            _disabledLabelButtonStyle = value;
            SetStyle();
         }
      }

      //---------------------------------------------------------------------------------------------------------------
      // METHODS - Protected
      //---------------------------------------------------------------------------------------------------------------

      protected override void AfterInternalViewSet()
      {
         base.AfterInternalViewSet();

         // If the view gets set after the bindings, those bindings must be set up now.
         SetUpCompleteLabelButtonBinding();
      }

      protected override void SetStyle()
      {
         base.SetStyle();

         if (InternalView == null)
         {
            return;
         }

         Style newStyle = null;

         // Set the style based on being enabled/disabled
         if (ButtonState == ButtonStates.Disabled)
         {
            newStyle = DisabledLabelStyle ?? DeselectedLabelStyle;
         }
         else if (ButtonState == ButtonStates.Selected)
         {
            newStyle = SelectedLabelStyle ?? DeselectedLabelStyle;
         }
         else
         {
            newStyle = DeselectedLabelStyle;
         }

         if (newStyle != null && (InternalView.Style == null || InternalView.Style.IsNotAnEqualObjectTo(newStyle)))
         {
            InternalView.Style = newStyle;

#if CALL_FORCE_STYLE
            // This library is not working well with styles, so forcing all settings manually
            InternalView.ForceStyle(newStyle);
#endif
         }
      }

      //---------------------------------------------------------------------------------------------------------------
      // METHODS - Private
      //---------------------------------------------------------------------------------------------------------------

      private void SetUpCompleteLabelButtonBinding()
      {
         if (LabelBindingName.IsEmpty())
         {
            InternalView?.RemoveBinding(Label.TextProperty);
         }
         else
         {
            InternalView?.SetUpBinding(Label.TextProperty, LabelBindingName, BindingMode.OneWay,
               LabelBindingConverter, LabelBindingConverterParameter);
         }
      }

      //---------------------------------------------------------------------------------------------------------------
      // STATIC READ ONLY VARIABLES & METHODS
      //---------------------------------------------------------------------------------------------------------------

      public static Style CreateLabelStyle
      (
         Color textColor,
         double fontSize,
         FontAttributes fontAttributes = FontAttributes.None
      )
      {
         return new Style(typeof(Label))
         {
            Setters =
            {
               // The text color is now the background color -- should be white
               new Setter { Property = Label.TextColorProperty, Value = textColor },

               // The label is always transparent
               new Setter { Property = BackgroundColorProperty, Value = Color.Transparent },

               new Setter { Property = Label.FontAttributesProperty, Value = fontAttributes },
               new Setter { Property = Label.FontSizeProperty, Value = fontSize }
            }
         };
      }

      //---------------------------------------------------------------------------------------------------------------
      // BINDABLE PROPERTIES
      //---------------------------------------------------------------------------------------------------------------

      public static BindableProperty CreateLabelButtonBindableProperty<PropertyTypeT>
      (
         string localPropName,
         PropertyTypeT defaultVal = default(PropertyTypeT),
         BindingMode bindingMode = BindingMode.OneWay,
         Action<LabelButton, PropertyTypeT, PropertyTypeT> callbackAction = null
      )
      {
         return BindableUtils.CreateBindableProperty(localPropName, defaultVal, bindingMode, callbackAction);
      }
   }
}
