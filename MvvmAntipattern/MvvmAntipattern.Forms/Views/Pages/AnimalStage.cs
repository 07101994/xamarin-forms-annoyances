#region License
// MIT License
// 
// Copyright (c) 2018 
// Marcus Technical Services, Inc.
// http://www.marcusts.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion  

namespace MvvmAntipattern.Forms.Views.Pages
{
   using SharedForms.Common.Utils;
   using SharedForms.Views.Pages;
   using ViewModels;
   using Xamarin.Forms;
   using Xamarin.Forms.Plugin.CustomToggleButton;

   public class AnimalStage : MenuNavPageBase<IAnimalViewModelBase>
   {
      protected override View ConstructPageView()
      {
         var stackLayout = FormsUtils.GetExpandingStackLayout();
         stackLayout.VerticalOptions = LayoutOptions.Start;

         var whatLabel = new Label
         {
            Text = "I Am A:",
            Margin = new Thickness(0, 10, 0, 0),
            HorizontalTextAlignment = TextAlignment.Center,
            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
            TextColor = Color.Gray
         };
         stackLayout.Children.Add(whatLabel);
         var whatAmILabel = FormsUtils.GetSimpleLabel(labelBindingPropertyName: nameof(IAnimalViewModelBase.WhatAmI), fontNamedSize:NamedSize.Large, fontAttributes:FontAttributes.Bold);
         whatAmILabel.HorizontalOptions = LayoutOptions.Center;
         stackLayout.Children.Add(whatAmILabel);

         var eatLabel = new Label
         {
            Text = "I Like To Eat:",
            Margin = new Thickness(0, 10, 0, 0),
            HorizontalTextAlignment = TextAlignment.Center,
            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
            TextColor = Color.Gray
         };
         stackLayout.Children.Add(eatLabel);
         var likeToEatLabel = FormsUtils.GetSimpleLabel(labelBindingPropertyName: nameof(IAnimalViewModelBase.LikeToEat), fontNamedSize: NamedSize.Large, fontAttributes: FontAttributes.Bold);
         likeToEatLabel.HorizontalOptions = LayoutOptions.Center;
         stackLayout.Children.Add(likeToEatLabel);

         var bigLabel = new Label
         {
            Text = "I Am Big:",
            Margin = new Thickness(0, 10, 0, 0),
            HorizontalOptions = LayoutOptions.Center,
            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
            TextColor = Color.Gray
         };
         stackLayout.Children.Add(bigLabel);

         var iAmBigSwitch = 
            new CustomToggleButton
               {
                  HorizontalOptions = LayoutOptions.Center,
                  WidthRequest = 75,
                  HeightRequest = 30,
                  CheckedImage = ImageSource.FromFile("checked.png"),
                  UnCheckedImage = ImageSource.FromFile("unchecked.png")
               };
         iAmBigSwitch.SetUpBinding(CustomToggleButton.CheckedProperty, nameof(IAnimalViewModelBase.IAmBig), BindingMode.TwoWay);
         stackLayout.Children.Add(iAmBigSwitch);

         var imageLabel = 
            new Label
               {
                  Text = "I Look Like This:",
                  Margin = new Thickness(0, 10, 0, 0),
                  HorizontalOptions = LayoutOptions.Center,
                  FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                  TextColor = Color.Gray
            };
         stackLayout.Children.Add(imageLabel);
         var myImage = FormsUtils.GetImage("", 100, 100);
         myImage.SetUpBinding(Image.SourceProperty, nameof(IAnimalViewModelBase.MyImageSource));
         myImage.HorizontalOptions = LayoutOptions.Center;
         stackLayout.Children.Add(myImage);

         var moveButton = new Button { Text = "Move", HorizontalOptions = LayoutOptions.Center};
         moveButton.SetUpBinding(Button.CommandProperty, nameof(IAnimalViewModelBase.MoveCommand));
         stackLayout.Children.Add(moveButton);

         var makeNoiseButton = new Button { Text = "Make Noise", HorizontalOptions = LayoutOptions.Center };
         moveButton.SetUpBinding(Button.CommandProperty, nameof(IAnimalViewModelBase.MakeNoiseCommand));
         stackLayout.Children.Add(makeNoiseButton);

         return stackLayout;
      }
   }
}
