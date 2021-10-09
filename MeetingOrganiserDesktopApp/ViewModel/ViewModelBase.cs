using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MeetingOrganiserDesktopApp.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<MessageEventArgs> MessageApplication;

        protected ViewModelBase() { }

        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnMessageApplication(String message)
        {
            if (MessageApplication != null)
                MessageApplication(this, new MessageEventArgs(message));
        }
    }
}
