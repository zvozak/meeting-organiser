using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MeetingOrganiserDesktopApp.View
{
    /// <summary>
    /// Interaction logic for EventEditorWindow.xaml
    /// </summary>
    public partial class EventEditorWindow : Window
    {
        public EventEditorWindow()
        {
            InitializeComponent();
            startDate.ValueChanged += new RoutedPropertyChangedEventHandler<object>(DateValuesChanged);
            deadlineForApplicationDate.ValueChanged += new RoutedPropertyChangedEventHandler<object>(DateValuesChanged);
            endDate.ValueChanged += new RoutedPropertyChangedEventHandler<object>(DateValuesChanged);
        }
        private void DateValuesChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            saveButton.IsEnabled = deadlineForApplicationDate.Value <= startDate.Value &&
                                   startDate.Value <= endDate.Value;
        }
    }
}
