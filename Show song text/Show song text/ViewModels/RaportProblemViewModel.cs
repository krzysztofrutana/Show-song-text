using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ShowSongText.ViewModels
{
    public class RaportProblemViewModel : ViewModelBase
    {
        #region Property
        private string messageText;

        public string MessageText
        {
            get { return messageText; }
            set
            {
                messageText = value;
                OnPropertyChanged(nameof(MessageText));
            }
        }

        private string subjectMessage;

        public string SubjectMessage
        {
            get { return subjectMessage; }
            set
            {
                subjectMessage = value;
                OnPropertyChanged(nameof(SubjectMessage));
            }
        }
        #endregion

        #region Commands
        public ICommand SendMessageCommand { get; private set; }
        #endregion

        #region Constructor
        public RaportProblemViewModel()
        {
            SendMessageCommand = new Command(() => SendMessage());
        }
        #endregion

        #region Commands methods
        private void SendMessage()
        {
            Launcher.OpenAsync(new Uri($"mailto:krzysztofrutana@wp.pl?subject={SubjectMessage} candy&body={MessageText}"));
        }
        #endregion
    }
}
