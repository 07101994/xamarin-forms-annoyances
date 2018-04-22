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

namespace MvvmAntipattern.Forms.ViewModels
{
   using System.ComponentModel;
   using System.Runtime.CompilerServices;
   using PropertyChanged;
   using SharedForms.Common.Interfaces;
   using SharedForms.Common.Interfaces.Samples;
   using SharedForms.Common.Navigation;
   using SharedForms.Models.Samples;
   using SharedForms.ViewModels;
   using Xamarin.Forms;

   public interface IAnimalViewModelBase : IAnimal, INotifyPropertyChanged, IPageViewModelBase
   {
   }

   /// <summary>
   /// The base abstract class for all IAnimal implementors
   /// </summary>
   [AddINotifyPropertyChangedInterface]
   [DoNotNotify]
   public abstract class AnimalViewModelBase : PageViewModelBase, IAnimalViewModelBase
   {
      private bool _IAmBig;
      private readonly IAnimalDataBase _animalData;

      protected AnimalViewModelBase(IStateMachineBase stateMachine, IAnimalDataBase animalData) : base(stateMachine)
      {
         _animalData = animalData;

         // These commands are not currently used
         MakeNoiseCommand = new Command(() => { });
         MoveCommand = new Command(() => { });
      }

      public bool IAmBig
      {
         get => _IAmBig;

         set
         {
            _IAmBig = value;

            // Ask the properties to refresh.
            // No way to do this through Fody Weavers.
            OnPropertyChanged(nameof(LikeToEat));
            OnPropertyChanged(nameof(MyImageSource));
         }
      }

      /// <summary>
      /// AnimalData could be null...
      /// </summary>
      public virtual string MyImageSource => _animalData?.GetAnimalImageSource(this);

      public abstract string WhatAmI { get; }

      public abstract string LikeToEat { get; }

      public Command MakeNoiseCommand { get; }

      public Command MoveCommand { get; }

      public event PropertyChangedEventHandler PropertyChanged;

      protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
   }
}
