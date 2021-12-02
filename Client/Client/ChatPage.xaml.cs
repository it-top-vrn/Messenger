using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Client
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
        public ObservableCollection<string> Items { get; set; }

        User user = new User();
        TCPClient tcpClient = new TCPClient();
        public ChatPage(User _user, TCPClient _tcpClient, string _idDialog)
        {

            InitializeComponent();
            user = _user;
            tcpClient = _tcpClient;
            user.idDialog = _idDialog;

            var temp = new Dialog();

            QueryLib<string> req = new QueryLib<string>(user.idDialog, RequestType.GiveMeMessageList);
            string msg = JsonConvert.SerializeObject(req);

            tcpClient.SendMessage(msg);

            string msg1 = tcpClient.GetMessage();
            QueryLib<string> resp = new QueryLib<string>(JsonConvert.DeserializeObject<QueryLib<string>>(msg1));

            if (resp.rsType == ResponseType.RequestDenied)
            {
                _ = DisplayAlert("Ошибка", "Отказано.", "Ok");
            }
            else temp.Messages = JsonConvert.DeserializeObject<Dialog>(resp.Data).Messages;

            foreach (var entry in user.Dialogs)
            {
                var contact = entry.idDialog; // содержимое (доделать)
            }

        }

        private async void status_button_Clicked(object sender, EventArgs e)
        {
            var Page1 = new ChatList(user, tcpClient);

            await Navigation.PushAsync(Page1);
        }

        private void Send_button_Clicked(object sender, EventArgs e)
        {
            Chat_StackLayout.Children.Add(new Label { Text = Entry_chat.Text, BackgroundColor = Color.LightGreen });
            
            //Chat_StackLayout.Children.Add(new Frame { CornerRadius = 30, BindingContext = Entry_chat.Text, BackgroundColor = Color.LightGreen });
        }

        private async void settings_button_Clicked(object sender, EventArgs e)
        {
            var setPage = new Settings(user, tcpClient);

            await Navigation.PushAsync(setPage);
        }

        private async void contacts_button_Clicked(object sender, EventArgs e)
        {
            var secondPage = new Contacts(user, tcpClient);

            await Navigation.PushAsync(secondPage);
        }
    }
}
