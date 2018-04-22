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

namespace MvvmAntipattern.Forms.Common.Navigation
{
   #region Imports

   using System;
   using SharedForms.Common.Generators;
   using SharedForms.Common.Interfaces;
   using SharedForms.Common.Navigation;
   using SharedForms.Models.Samples;
   using SharedForms.ViewModels;
   using SharedForms.ViewModels.Samples;
   using SharedForms.Views.Pages;
   using ViewModels;
   using Views.Pages;
   using Xamarin.Forms;
   using Xamarin.Forms.Internals;

   #endregion

   /// <summary>
   /// A controller to manage which views and view models are shown for a given state
   /// </summary>
   /// <remarks>
   /// I avoid using an interface here because we inherently share IStateMachineBase with the StateMachineBase.
   /// The DI Container Resolve needs a consistent interface as a reference, and cannot benefit from additional, competitive interfaces.
   /// </remarks>
   public class FormsStateMachine : StateMachineBase
   {
      public const string NO_APP_STATE = "None";
      public const string AUTO_SIGN_IN_APP_STATE = "AttemptAutoSignIn";
      public const string ABOUT_APP_STATE = "About";
      public const string PREFERENCES_APP_STATE = "Preferences";
      public const string NO_ANIMAL_APP_STATE = "No Animal";
      public const string CAT_ANIMAL_APP_STATE = "Cat";
      public const string BIRD_ANIMAL_APP_STATE = "Bird";
      public const string DOG_ANIMAL_APP_STATE = "Dog";

      public static readonly string[] APP_STATES =
      {
         NO_APP_STATE,
         AUTO_SIGN_IN_APP_STATE,
         NO_ANIMAL_APP_STATE,
         CAT_ANIMAL_APP_STATE,
         BIRD_ANIMAL_APP_STATE,
         DOG_ANIMAL_APP_STATE,
         ABOUT_APP_STATE,
         PREFERENCES_APP_STATE
      };

      public override string AppStartUpState => AUTO_SIGN_IN_APP_STATE;

      public override IMenuNavigationState[] MenuItems => 
         new IMenuNavigationState[]
         {
            new MenuNavigationState(GetMenuOrderFromAppState(ABOUT_APP_STATE), ABOUT_APP_STATE, "About", ABOUT_APP_STATE),
            new MenuNavigationState(GetMenuOrderFromAppState(BIRD_ANIMAL_APP_STATE), BIRD_ANIMAL_APP_STATE, "ANIMALS", BIRD_ANIMAL_APP_STATE),
            new MenuNavigationState(GetMenuOrderFromAppState(CAT_ANIMAL_APP_STATE), CAT_ANIMAL_APP_STATE, "ANIMALS", CAT_ANIMAL_APP_STATE),
            new MenuNavigationState(GetMenuOrderFromAppState(DOG_ANIMAL_APP_STATE), DOG_ANIMAL_APP_STATE, "ANIMALS", DOG_ANIMAL_APP_STATE),
            new MenuNavigationState(GetMenuOrderFromAppState(NO_ANIMAL_APP_STATE), NO_ANIMAL_APP_STATE, "ANIMALS", NO_ANIMAL_APP_STATE),
            new MenuNavigationState(GetMenuOrderFromAppState(ABOUT_APP_STATE), PREFERENCES_APP_STATE, "Preferences", PREFERENCES_APP_STATE),
         };

      public override void GoToLandingPage(bool preventStackPush = true)
      {
         GoToAppState<NoPayload>(NO_ANIMAL_APP_STATE, null, preventStackPush);
      }

      protected override void RespondToAppStateChange<PayloadT>(string newState, PayloadT payload, bool preventStackPush)
      {
         var titleStr = payload is IMenuNavigationState pageAsNavState ? pageAsNavState.ViewTitle : "";

         switch (newState)
         {
            case ABOUT_APP_STATE:
               CheckAgainstLastPage(typeof(DummyPage), () => new DummyPage(), () => new AboutViewModel(this) {PageTitle = titleStr}, preventStackPush);
               break;

            case PREFERENCES_APP_STATE:
               CheckAgainstLastPage(typeof(DummyPage), () => new DummyPage(), () => new PreferencesViewModel(this) { PageTitle = titleStr }, preventStackPush);
               break;

            case CAT_ANIMAL_APP_STATE:
               CheckAgainstLastPage(typeof(AnimalStage), () => new AnimalStage(), () => new CatViewModel(this, new CatData()) { PageTitle = titleStr }, preventStackPush);
               break;

            case BIRD_ANIMAL_APP_STATE:
               CheckAgainstLastPage(typeof(AnimalStage), () => new AnimalStage(), () => new BirdViewModel(this, new BirdData()) { PageTitle = titleStr }, preventStackPush);
               break;

            case DOG_ANIMAL_APP_STATE:
               CheckAgainstLastPage(typeof(AnimalStage), () => new AnimalStage(), () => new DogViewModel(this, new DogData()) { PageTitle = titleStr }, preventStackPush);
               break;

            default:
               //NO_ANIMAL_APP_STATE:
               CheckAgainstLastPage(typeof(AnimalStage), () => new AnimalStage(), () => new NoAnimalViewModel(this, null) { PageTitle = titleStr }, true);
               break;
         }
      }

      private void AttemptAutoSignIn()
      {
         // Assuming true; also, prevent stack push so we don't go back into this state, as it is "finished"
         GoToAppState<NoPayload>(NO_ANIMAL_APP_STATE, null, true);
      }

      public static int GetMenuOrderFromAppState(string appState)
      {
         return APP_STATES.IndexOf(appState);
      }
   }
}
