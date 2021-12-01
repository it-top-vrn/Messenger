using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Client
{
    public partial class MainPage : ContentPage
    {
        User user = new User();
        public MainPage()
        {
            InitializeComponent();

        }

        private async void button_registration_Clicked(object sender, EventArgs e)
        {
            var mainPage = new Registration(user);

            await Navigation.PushAsync(mainPage);
        }

        private async void button_login_Clicked(object sender, EventArgs e)
        {
            if (entry_login.Text != "" && entry_password.Text != "")
            {
                user.nickName = entry_login.Text;
                user.password = entry_password.Text;

                //получение ответа для аутентификации if( ... )

                //загрузка айдишника и роли

                var secondPage = new Contacts(user);

                _ = DisplayAlert("Авторизация", "Успешно!", "Ok");

                await Navigation.PushAsync(secondPage);
            }
            else _ = DisplayAlert("Ошибка", "Неверный логин или пароль.", "Ok");

        }
    }

    // надо прописать запросы на загрузку xml доков со списками контактов, диалогов, сообщений
    // чтобы не все вместе загружалось, а что-то конкретное, в зависимости от того, что нажмет пользователь
    // и при изменении содержимого отправлять запрос, чтобы эти изменения вносились и в БД также

}

