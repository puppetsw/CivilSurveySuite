using System.Windows;
using _3DS_CivilSurveySuite.ACAD.Services;
using _3DS_CivilSurveySuite.Shared.Services.Interfaces;
using _3DS_CivilSurveySuite.UI.Logger;
using _3DS_CivilSurveySuite.UI.Services.Implementation;
using _3DS_CivilSurveySuite.UI.ViewModels;
using _3DS_CivilSurveySuite.UI.Views;
using SimpleInjector;

namespace _3DS_CivilSurveySuite.ACAD
{
    /// <summary>
    /// AutoCAD IOC Container.
    /// </summary>
    public static class Ioc
    {
        public static Container Default { get; } = new Container();

        public static void RegisterServices()
        {
            // Logger
            Default.Register<ILogWriter, LogWriter>();
            Default.Register<ILogger, Logger>();

            // ACAD Services
            Default.Register<IProcessService, ProcessService>();
            Default.Register<ITraverseService, TraverseService>(Lifestyle.Singleton);
            Default.Register<IMessageBoxService, MessageBoxService>();
            Default.Register<IRasterImageService, RasterImageService>();
            Default.Register<IBlockService, BlockService>();

            // Dialog Services
            Default.Register<IOpenFileDialogService, OpenFileDialogService>();
            Default.Register<ISaveFileDialogService, SaveFileDialogService>();
            Default.Register<IFolderBrowserDialogService, FolderBrowserDialogService>();

            // Views
            Default.Register<AngleCalculatorView>();
            Default.Register<ImageManagerView>();
            Default.Register<TraverseAngleView>();
            Default.Register<TraverseView>();
            // Input dialog view.
            Default.Register<InputDialogView>();

            // ViewModels
            Default.Register<AngleCalculatorViewModel>();
            Default.Register<ImageManagerViewModel>();
            Default.Register<TraverseViewModel>();
            Default.Register<TraverseAngleViewModel>();

            Default.Verify(VerificationOption.VerifyAndDiagnose);
        }

        public static TView GetRequiredView<TView>() where TView : Window
        {
            AcadApp.Logger?.Info($"New instance of {typeof(TView)} requested");
            return Default.GetInstance<TView>();
        }
    }
}
