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
    public partial class Contacts : ContentPage
    {
        User user;
        TCPClient tcpClient;

        public Contacts(User _user, TCPClient _tcpClient)
        {
            InitializeComponent();
            user = _user;
            tcpClient = _tcpClient;

            QueryLib<string> req = new QueryLib<string>(user.nickName, RequestType.GiveMeContactList);
            string msg = JsonConvert.SerializeObject(req);

            tcpClient.SendMessage(msg);

            string msg1 = tcpClient.GetMessage();
            QueryLib<string> resp = new QueryLib<string>(JsonConvert.DeserializeObject<QueryLib<string>>(msg1));
            if (resp.rsType == ResponseType.RequestDenied)
            {
                _ = DisplayAlert("Ошибка", "Отказано.", "Ok");
            }
            else user.Contacts = JsonConvert.DeserializeObject<User>(resp.Data).Contacts;

            foreach (var entry in user.Contacts)
             {
                 var contact = entry.nickName; // содержимое доделать, вывод имен и айдишников диалогов
             }
        }

        private async void status_button_Clicked(object sender, EventArgs e)
        {
            var secondPage = new ChatList(user, tcpClient);

            await Navigation.PushAsync(secondPage);
        }


        private async void chat_button_Clicked(object sender, EventArgs e)
        {
            var secondPage = new ChatPage(user, tcpClient, user.idDialog);

            await Navigation.PushAsync(secondPage);
        }

        private async void settings_button_Clicked(object sender, EventArgs e)
        {
            var secondPage = new Settings(user, tcpClient);

            await Navigation.PushAsync(secondPage);
        }

        private async void butoon_newContact_Clicked(object sender, EventArgs e)
        {

            var secondPage = new NewContact(user, tcpClient);

            await Navigation.PushAsync(secondPage);

        }
        private void ListContact_Focused(object sender, FocusEventArgs e)
        {

        }
    }
}