using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Client
{
    public partial class MainPage : ContentPage
    {
        User user = new User();
        TCPClient tcpClient = new TCPClient();

        public MainPage()
        {
            InitializeComponent();

            tcpClient.Connect();
        }

        private async void button_registration_Clicked(object sender, EventArgs e)
        {
            var mainPage = new Registration(user, tcpClient);

            await Navigation.PushAsync(mainPage);
        }

        private async void button_login_Clicked(object sender, EventArgs e)
        {
            if (entry_login.Text != "" && entry_password.Text != "")
            {
                string _nickName = entry_login.Text;
                string _password = entry_password.Text;

                user = new User { nickName = _nickName, password = _password };

                QueryLib<string> req = new QueryLib<string>(JsonConvert.SerializeObject(user), RequestType.Authorization);
                string msg = JsonConvert.SerializeObject(req);

                tcpClient.SendMessage(msg);


                string msg1 = tcpClient.GetMessage();
                QueryLib<string> resp = JsonConvert.DeserializeObject<QueryLib<string>>(msg1);
                if (resp.rsType == ResponseType.RequestDenied)
                {
                    _ = DisplayAlert("Ошибка", "Отказано.", "Ok");
                } else user = JsonConvert.DeserializeObject<User>(resp.Data);

                var secondPage = new Contacts(user, tcpClient);

                _ = DisplayAlert("Авторизация", "Успешно!", "Ok");

                await Navigation.PushAsync(secondPage);
            }
            else _ = DisplayAlert("Ошибка", "Неверный логин или пароль.", "Ok");

        }
    }

    // надо прописать запросы на загрузку json доков со списками контактов, диалогов, сообщений
    // чтобы не все вместе загружалось, а что-то конкретное, в зависимости от того, что нажмет пользователь
    // и при изменении содержимого отправлять запрос, чтобы эти изменения вносились и в БД также

}

