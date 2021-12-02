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
    public partial class NewContact : ContentPage
    {
        User user = new User();
        TCPClient tcpClient = new TCPClient();
        public NewContact(User _user, TCPClient _tcpClient)
        {
            InitializeComponent();
            user = _user;
            tcpClient = _tcpClient;
        }
      
        private async void butoon_cancel_Clicked(object sender, EventArgs e)
        {
            var secondPage = new Contacts(user, tcpClient);

            await Navigation.PushAsync(secondPage);
        }

        private async void butoon_ADDContact_Clicked(object sender, EventArgs e)
        {
            string _nickName = entry_name.Text;

            QueryLib<string> req = new QueryLib<string>(_nickName, RequestType.AddNewContact);
            string msg = JsonConvert.SerializeObject(req);

            tcpClient.SendMessage(msg);

            string msg1 = tcpClient.GetMessage();
            QueryLib<string> resp = JsonConvert.DeserializeObject<QueryLib<string>>(msg1);
            if (resp.rsType == ResponseType.RequestDenied)
            {
                _ = DisplayAlert("Ошибка", "Отказано.", "Ok");
            }
            else {
                User temp = new User { nickName = _nickName };
                user.Contacts.Add(temp);
            }

            var secondPage = new Contacts(user, tcpClient);

            _ = DisplayAlert("Добавление контакта", "Успешно!", "Ok");

            await Navigation.PushAsync(secondPage);
        }
    }
}
