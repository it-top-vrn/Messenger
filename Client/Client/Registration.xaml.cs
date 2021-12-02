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
    public partial class Registration : ContentPage
    {
        TCPClient tcpClient = new TCPClient();
        public Registration(TCPClient _tcpClient)
        {
            InitializeComponent();
            tcpClient = _tcpClient;
        }

        private async void button_registrationCancel_Clicked(object sender, EventArgs e)
        {
            var secondPage = new MainPage();

            await Navigation.PushAsync(secondPage);
        }

        private async void button_registration_Clicked(object sender, EventArgs e)
        {
            string _nickName = entry_login.Text;
            string pass1 = entry_password.Text;
            string pass2 = entry_password2.Text;

            if (pass1 != pass2)
            {
                _ = DisplayAlert("Ошибка", "Проверьте ввод паролей", "Ok");
            }
            else if (_nickName.Length == 0)
            {
                _ = DisplayAlert("Ошибка", "Введите данные, используя цифры и латинские бурвы", "Ok");
            }
            else
            {
                var user = new User { nickName = entry_login.Text, password = entry_password.Text };

                QueryLib<User> req = new QueryLib<User>(user, RequestType.Registration);
                string msg = JsonConvert.SerializeObject(req);

                tcpClient.SendMessage(msg);

                string msg1 = tcpClient.GetMessage();
                QueryLib<string> resp = JsonConvert.DeserializeObject<QueryLib<string>>(msg1);
                if (resp.rsType == ResponseType.RequestDenied)
                {
                    _ = DisplayAlert("Ошибка", "Отказано.", "Ok");
                }
                else user = JsonConvert.DeserializeObject<User>(resp.Data);

                var secondPage = new Contacts(user, tcpClient);

                _ = DisplayAlert("Регистрация", "Успешно!", "Ok");

                await Navigation.PushAsync(secondPage);
            }

        }
    }
}