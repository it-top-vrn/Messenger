using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Client
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatList : ContentPage
    {
        User user = new User();
        TCPClient tcpClient = new TCPClient();
        public ChatList(User _user, TCPClient _tcpClient)
        {
            InitializeComponent();
            user = _user;
            tcpClient = _tcpClient;

            QueryLib<string> req = new QueryLib<string>(user.nickName, RequestType.GiveMeChatList);
            string msg = JsonConvert.SerializeObject(req);

            tcpClient.SendMessage(msg);

            string msg1 = tcpClient.GetMessage();
            QueryLib<string> resp = new QueryLib<string>(JsonConvert.DeserializeObject<QueryLib<string>>(msg1));

            if (resp.rsType == ResponseType.RequestDenied)
            {
                _ = DisplayAlert("Ошибка", "Отказано.", "Ok");
            }
            else user.Dialogs = JsonConvert.DeserializeObject<User>(resp.Data).Dialogs;

            foreach (var entry in user.Dialogs)
            {
                var contact = entry.idDialog; // содержимое (доделать)
            }
        }

        private async void chat_button_Clicked(object sender, EventArgs e)
        {
            string idDialog = "unfinded";
            //idDialog = label_idDialog;

            var secondPage = new ChatPage(user, tcpClient, idDialog);

            await Navigation.PushAsync(secondPage);
        }

        private async void ListView_Focused(object sender, FocusEventArgs e)
        {
            string idDialog = "unfinded";
            //idDialog = label_idDialog;

            var secondPage = new ChatPage(user, tcpClient, idDialog);

            await Navigation.PushAsync(secondPage);
        }

        private async void settings_button_Clicked(object sender, EventArgs e)
        {
            var setPage = new Settings(user, tcpClient);

            await Navigation.PushAsync(setPage);
        }

        private async void contacts_button_Clicked(object sender, EventArgs e)
        {
            var contactPage = new Contacts(user, tcpClient);

            await Navigation.PushAsync(contactPage);
        }
    }
}