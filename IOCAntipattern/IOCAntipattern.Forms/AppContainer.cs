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

namespace IOCAntipattern.Forms
{
   using Autofac;

   public static class AppContainer
   {
      static AppContainer()
      {
         var containerBuilder = new ContainerBuilder();
         containerBuilder.RegisterType<ParentViewModel>().As<IParentViewModel>().SingleInstance();
         containerBuilder.RegisterType<ChildViewModel>().As<IChildViewModel>();

         // Maintains only a single instance, but might cause problems with disposal
         //containerBuilder.RegisterType<MainViewModel>().As<IMainViewModel>().SingleInstance();
         //containerBuilder.RegisterType<SecondViewModel>().As<ISecondViewModel>().SingleInstance();

         // Normal usage
         //containerBuilder.RegisterType<MainViewModel>().As<IMainViewModel>();
         //containerBuilder.RegisterType<SecondViewModel>().As<ISecondViewModel>();

         // Revised to ensure that there is an *single* alignment between the view and the view model.
         containerBuilder.RegisterType<MainViewModel>().As<IMainViewModel>().InstancePerDependency();
         containerBuilder.RegisterType<SecondViewModel>().As<ISecondViewModel>().InstancePerDependency();

         containerBuilder.RegisterType<FirstPossibleInjectedClass>().As<IGeneralInjectable>();
         containerBuilder.RegisterType<FirstPossibleInjectedClass>().As<IOtherwiseInjectable>();
         containerBuilder.RegisterType<SecondPossibleInjectedClass>().As<IGeneralInjectable>();
         containerBuilder.RegisterType<ClassWithConstructors>();
         Container = containerBuilder.Build();
      }

      public static IContainer Container { get; set; }
   }
}