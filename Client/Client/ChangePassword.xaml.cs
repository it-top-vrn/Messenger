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
    public partial class ChangePassword : ContentPage
    {
        User user = new User();
        TCPClient tcpClient = new TCPClient();
        public ChangePassword(User _user, TCPClient _tcpClient)
        {
            InitializeComponent();
            user = _user;
            tcpClient = _tcpClient;
        }

        private async void button_changePassword_Clicked(object sender, EventArgs e)
        {
            string oldPass = entry_oldPass.Text;
            string pass1 = entry_newPass.Text;
            string pass2 = entry_newPass2.Text;

            if (oldPass == user.password)
            {
                if (pass1 != pass2)
                {
                    _ = DisplayAlert("Ошибка", "Пароли не совпадают, попробуйте еще раз.", "Ok");
                }
                else
                {
                    user.password = pass1;

                    QueryLib<string> req = new QueryLib<string>(JsonConvert.SerializeObject(user), RequestType.ChangePassword);
                    string msg = JsonConvert.SerializeObject(req);

                    tcpClient.SendMessage(msg);

                    var secondPage = new Contacts(user, tcpClient);

                    _ = DisplayAlert("Смена пароля", "Успешно!", "Ok");

                    await Navigation.PushAsync(secondPage);
                }
            }
            else
            {
                _ = DisplayAlert("Ошибка", "Неверный пароль, попробуйте еще раз.", "Ok");
            }
        }

        private void button_changeCancel_Clicked(object sender, EventArgs e)
        {

        }
    }
}