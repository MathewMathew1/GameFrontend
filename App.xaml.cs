namespace BoardGameFrontend
{
    using AutoMapper;
    using System.Windows;

    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AutoMapperConfig.Initialize();
        }
    }
}
