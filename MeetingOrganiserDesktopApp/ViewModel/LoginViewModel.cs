using MeetingOrganiserDesktopApp.Model;
using MeetingOrganiserDesktopApp.Persistence;
using System;
using System.Windows.Controls;

namespace MeetingOrganiserDesktopApp.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private IMeetingApplicationModel model;

        public DelegateCommand ExitCommand { get; private set; }

        public DelegateCommand LoginCommand { get; private set; }

        public String UserName { get; set; }
        public String OrganisationName { get; set; }

        public event EventHandler ExitApplication;

        public event EventHandler LoginSuccess;

        public event EventHandler LoginFailed;

        public LoginViewModel(IMeetingApplicationModel model)
        { 
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            this.model = model;
            UserName = String.Empty;
            OrganisationName = String.Empty;

            ExitCommand = new DelegateCommand(param => OnExitApplication());

            LoginCommand = new DelegateCommand(param => LoginAsync(param as PasswordBox));
        }

        private async void LoginAsync(PasswordBox passwordBox)
        {
            if (passwordBox == null)
                return;

            try
            {
                Boolean result = await model.LoginAsync(UserName, passwordBox.Password, OrganisationName);

                if (result)
                {
                    await model.LoadAsync(OrganisationName);
                    OnLoginSuccess();
                }
                else
                {
                    OnLoginFailed();
                }
            }
            catch (PersistenceUnavailableException)
            {
                OnMessageApplication("There is no connection with the provider.");
            }
        }

        private void OnLoginSuccess()
        {
            if (LoginSuccess != null)
                LoginSuccess(this, EventArgs.Empty);
        }

        private void OnExitApplication()
        {
            if (ExitApplication != null)
                ExitApplication(this, EventArgs.Empty);
        }

        private void OnLoginFailed()
        {
            if (LoginFailed != null)
                LoginFailed(this, EventArgs.Empty);
        }

    }
}
