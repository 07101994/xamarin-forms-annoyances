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

// #define PREVENT_PAGE_CHANGE

namespace IOCAntipattern.Forms
{
   #region Imports

   using System;
   using System.Diagnostics;
   using System.Threading.Tasks;
   using Autofac;
   using Xamarin.Forms;

   #endregion

   public partial class App : Application
   {
      public App()
      {
         // GlobalVariables.Add(typeof(IMainViewModel), () => new MainViewModel());

         InitializeComponent();

         var firstAccessedMainViewModel = AppContainer.Container.Resolve<IMainViewModel>();
         var secondAccessedMainViewModel = AppContainer.Container.Resolve<IMainViewModel>();
         var areEqual = ReferenceEquals(firstAccessedMainViewModel, secondAccessedMainViewModel);

         // Injects the second view model because it was added to the container last.
         var whoKnowsWhatThisIs = AppContainer.Container.Resolve<ClassWithConstructors>();

         // MainPage = new MainPage() { BindingContext = GlobalVariables[typeof(IMainViewModel)] };
         var mainPage = new MainPage { BindingContext = AppContainer.Container.Resolve<IMainViewModel>() };
         MainPage = mainPage;

         Device.BeginInvokeOnMainThread
            (
               async () => 
               {
                  await Task.Delay(5000);
                  var secondPage = new SecondPage();
                  Debug.WriteLine("About to assign the main page to the second page.");
                  MainPage = secondPage;
                  Debug.WriteLine("Finished assigning the main page to the second page.");

#if !PREVENT_PAGE_CHANGE
                  await Task.Delay(5000);
                  Debug.WriteLine("About to assign the main page to the original main page.");
                  MainPage = mainPage;
                  Debug.WriteLine("Finished assigning the main page to the original main page.");
#endif

                  // Force clean-up
                  secondPage = null;
                  GC.Collect();
               });
      }

      // public static Dictionary<Type, Func<object>> GlobalVariables = new Dictionary<Type, Func<object>>();

      public static IContainer IOCContainer { get; set; }

      protected override void OnStart()
      {
         // Handle when your app starts
      }

      protected override void OnSleep()
      {
         // Handle when your app sleeps
      }

      protected override void OnResume()
      {
         // Handle when your app resumes
      }
   }
}