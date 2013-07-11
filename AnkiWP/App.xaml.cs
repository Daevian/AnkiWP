using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Diagnostics;
using AnkiWP.ViewModel;
using Windows.Storage;
using Windows.System;

namespace AnkiWP
{
    public partial class App : Application
    {
        private static Model.Collection s_collection;
        private static Database s_database;
        private static AnkiViewModel s_viewModel;
        private static Scheduler s_scheduler;

        public static Database Database
        {
            get { return s_database; }
        }

        public static AnkiViewModel ViewModel
        {
            get { return s_viewModel; }
        }

        public static Model.Collection Collection
        {
            get { return App.s_collection; }
        }

        public static Scheduler Scheduler
        {
            get { return App.s_scheduler; }
        }

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

            s_collection = new Model.Collection();
            s_viewModel = new AnkiViewModel(s_collection);
            s_scheduler = new Scheduler();

            Furigana.Install();
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private async void Application_Launching(object sender, LaunchingEventArgs e)
        {

            var tempDatabaseFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Data/collection.anki2"));
            await tempDatabaseFile.CopyAsync(ApplicationData.Current.LocalFolder, "collection.anki2", NameCollisionOption.ReplaceExisting);


            s_database = new Database(Database.DB_PATH);
            await s_database.Load(s_collection);
           // await s_database.Commit();

            

            //try
            //{
            //    StorageFolder packageLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
            //    StorageFolder certificateFolder = await packageLocation.GetFolderAsync("Data");
            //    StorageFile certificate = await certificateFolder.GetFileAsync("ankiweb.cer");

            //    await Launcher.LaunchFileAsync(certificate);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message.ToString());
            //}

            //RemoteServer remoteServer = new RemoteServer(string.Empty);
            //var hostKey = await remoteServer.HostKey("daeviann@live.com", "TestingTesting");

            //HttpSyncer test = new HttpSyncer(await HttpSyncer.CreateHttpConnection());
            //await test.Request("download", null, 0);
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            //m_database.Close();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            //m_database.Close();
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Debug.WriteLine(e.Exception.Message);
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Debug.WriteLine(e.ExceptionObject.Message);
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new TransitionFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}