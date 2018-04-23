namespace MvvmAntipattern.Forms.Common.Container
{
   #region Imports

   using Autofac;
   using Navigation;
   using SharedForms.Common.Generators;
   using SharedForms.Common.Navigation;
   using SharedForms.Common.Utils;
   using SharedForms.Views.SubViews;
   using SharedGlobals.Container;

   #endregion

   public class FormsContainerSetup : ContainerSetup
   {
      //------------------------------------------------------------------------------------------
      protected override void RegisterDependencies(ContainerBuilder containerBuilder)
      {
         base.RegisterDependencies(containerBuilder);

         // This is a single injection of a global state machine. It is not intended to be replaced during the life of the app.
         containerBuilder.RegisterType<FormsStateMachine>().As<IStateMachineBase>().SingleInstance();

         // This is a single injection of a global menu system. It is not intended to be replaced during the life of the app.
         containerBuilder.RegisterType<MainMenu>().As<IMainMenu>().SingleInstance();
      }
   }
}
