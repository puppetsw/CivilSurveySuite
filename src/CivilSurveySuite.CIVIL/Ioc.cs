using System.Windows;
using CivilSurveySuite.ACAD;
using CivilSurveySuite.CIVIL.Services;
using CivilSurveySuite.Shared.Services.Interfaces;
using CivilSurveySuite.UI.Services.Implementation;
using CivilSurveySuite.UI.ViewModels;
using CivilSurveySuite.UI.Views;
using SimpleInjector;

namespace CivilSurveySuite.CIVIL
{
    /// <summary>
    /// Civil 3D IOC Container
    /// </summary>
    public static class Ioc
    {
        public static Container Default { get; } = new Container();

        public static void RegisterServices()
        {
            // CIVIL SERVICES
            Default.Register<ICivilSelectService, CivilSelectService>();
            Default.Register<ICogoPointService, CogoPointService>();

            Default.Register<IConnectLineworkService, ConnectLineworkService>();
            Default.Register<ICogoPointSurfaceReportService, CogoPointSurfaceReportService>();
            Default.Register<ICogoPointReplaceDuplicateService, CogoPointReplaceDuplicateService>();

            // DIALOGS
            Default.Register<IOpenFileDialogService, OpenFileDialogService>();
            Default.Register<ISaveFileDialogService, SaveFileDialogService>();
            Default.Register<IFolderBrowserDialogService, FolderBrowserDialogService>();

            // VIEWS AND VIEWMODELS
            Default.Register<SelectSurfaceView>();
            Default.Register<SelectSurfaceViewModel>();
            Default.Register<SelectPointGroupView>();
            Default.Register<SelectPointGroupViewModel>();
            Default.Register<SelectAlignmentView>();
            Default.Register<SelectAlignmentViewModel>();
            Default.Register<CogoPointMoveLabelView>();
            Default.Register<CogoPointMoveLabelViewModel>();
            Default.Register<ConnectLineworkView>();
            Default.Register<ConnectLineworkViewModel>();
            Default.Register<CogoPointEditorView>();
            Default.Register<CogoPointEditorViewModel>();
            Default.Register<CogoPointSurfaceReportView>();
            Default.Register<CogoPointSurfaceReportViewModel>();
            Default.Register<CogoPointReplaceDuplicateView>();
            Default.Register<CogoPointReplaceDuplicateViewModel>();

            Default.Verify(VerificationOption.VerifyAndDiagnose);
            AcadApp.Logger.Info("Civil3D Services registered successfully.");
        }

        public static Window GetRequiredView<TView>() where TView : Window
        {
            AcadApp.Logger.Info($"Creating instance of {typeof(TView)}");
            return Default.GetInstance<TView>();
        }
    }
}
