using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MeetingOrganiserDesktopApp.Model;
using MeetingOrganiserDesktopApp.Persistence;
using MeetingOrganiserDesktopApp.View;
using MeetingOrganiserDesktopApp.ViewModel;

namespace MeetingOrganiserDesktopApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IMeetingApplicationModel model;
        private LoginViewModel loginViewModel;
        private LoginWindow loginView;
        private MainViewModel mainViewModel;
        private MainWindow mainView;
        private EventEditorWindow eventEditorView;
        private VenueEditorWindow venueEditorView;
        private MemberEditorWindow memberEditorView;
        private GuestListWindow guestListView;

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
            Exit += new ExitEventHandler(App_Exit);
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            IConfigurationRoot configuration = configBuilder.Build();

            model = new MeetingApplicationModel(new MeetingApplicationServicePersistence(configuration["BaseAddress"]));

            loginViewModel = new LoginViewModel(model);
            loginViewModel.ExitApplication += new EventHandler(ViewModel_ExitApplication);
            loginViewModel.LoginSuccess += new EventHandler(ViewModel_LoginSuccess);
            loginViewModel.LoginFailed += new EventHandler(ViewModel_LoginFailed);

            loginView = new LoginWindow();
            loginView.DataContext = loginViewModel;
            loginView.Show();
        }

        public async void App_Exit(object sender, ExitEventArgs e)
        {
            if (model.IsUserLoggedIn)
            {
                await model.LogoutAsync();
            }
        }
        private void ViewModel_GuestListCreated(object sender, EventArgs e)
        {
            guestListView = new GuestListWindow();
            guestListView.DataContext = mainViewModel;
            guestListView.Show();
        }

        private void ViewModel_GuestListClose(object sender, EventArgs e)
        {
            guestListView.Close();
        }


        private void ViewModel_LoginSuccess(object sender, EventArgs e)
        {
            

            mainViewModel = new MainViewModel(model);
            mainViewModel.MessageApplication += new EventHandler<MessageEventArgs>(ViewModel_MessageApplication);
            mainViewModel.GuestListQuery += new EventHandler<EventEventArgs>(ViewModel_GuestListQuery);
            mainViewModel.GuestListCreated += new EventHandler<EventEventArgs>(ViewModel_GuestListCreated);
            mainViewModel.EventEditingStarted += new EventHandler(MainViewModel_EventEditingStarted);
            mainViewModel.EventEditingFinished += new EventHandler(MainViewModel_EventEditingFinished);
            mainViewModel.VenueEditingStarted += new EventHandler(MainViewModel_VenueEditingStarted);
            mainViewModel.VenueEditingFinished += new EventHandler(MainViewModel_VenueEditingFinished);
            mainViewModel.ImageEditingStarted += new EventHandler<VenueEventArgs>(MainViewModel_ImageEditingStarted);
            mainViewModel.ExitApplication += new EventHandler(ViewModel_ExitApplication);

            mainView = new MainWindow();
            mainView.DataContext = mainViewModel;
            mainView.Show();

            loginView.Close();
        }

        private void ViewModel_GuestListQuery(object sender, EventEventArgs e)
        {
            model.CreateGuestList(e.EventId);
        }

        private void ViewModel_LoginFailed(object sender, EventArgs e)
        {
            MessageBox.Show("Login failed!", "Meeting organiser", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void ViewModel_MessageApplication(object sender, MessageEventArgs e)
        {
            MessageBox.Show(e.Message, "Meeting organiser", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void MainViewModel_MemberEditingStarted(object sender, EventArgs e)
        {
            memberEditorView = new MemberEditorWindow();
            memberEditorView.DataContext = mainViewModel;
            memberEditorView.Show();
        }

        private void MainViewModel_MemberEditingFinished(object sender, EventArgs e)
        {
            memberEditorView.Close();
        }

        private void MainViewModel_EventEditingStarted(object sender, EventArgs e)
        {
            eventEditorView = new EventEditorWindow();
            eventEditorView.DataContext = mainViewModel;
            eventEditorView.Show();
        }

        private void MainViewModel_EventEditingFinished(object sender, EventArgs e)
        {
            if (eventEditorView != null)
                eventEditorView.Close();
        }

        private void MainViewModel_VenueEditingStarted(object sender, EventArgs e)
        {
            venueEditorView = new VenueEditorWindow();
            venueEditorView.DataContext = mainViewModel;
            venueEditorView.Show();
        }

        private void MainViewModel_VenueEditingFinished(object sender, EventArgs e)
        {
            venueEditorView.Close();
        }

        private void MainViewModel_ImageEditingStarted(object sender, VenueEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.CheckFileExists = true;
                dialog.Filter = "Image formats|*.jpg;*.jpeg;*.bmp;*.tif;*.gif;*.png;";
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                Boolean? result = dialog.ShowDialog();

                if (result == true)
                {
                    model.CreateImage(e.EventId, e.VenueId,
                                       ImageHandler.OpenAndResize(dialog.FileName, 100),
                                       ImageHandler.OpenAndResize(dialog.FileName, 600));
                }
            }
            catch { }
        }

        private void ViewModel_ExitApplication(object sender, System.EventArgs e)
        {
            Shutdown();
        }
    }
}
