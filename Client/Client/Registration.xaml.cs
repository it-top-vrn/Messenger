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
        User user = new User();
        public Registration(User _user)
        {
            InitializeComponent();
            user = _user;
        }

        private async void button_registrationCancel_Clicked(object sender, EventArgs e)
        {
            var secondPage = new MainPage();

            await Navigation.PushAsync(secondPage);
        }

        private async void button_registration_Clicked(object sender, EventArgs e)
        {
            string login = entry_login.Text;
            string pass1 = entry_password.Text;
            string pass2 = entry_password2.Text;

            if (pass1 != pass2)
            {
                _ = DisplayAlert("Ошибка", "Проверьте ввод паролей", "Ok");
            }
            else if (login.Length == 0)
            {
                _ = DisplayAlert("Ошибка", "Введите данные, используя цифры и латинские бурвы", "Ok");
            }
            else
            {
                user.nickName = login;
                user.password = pass1;
                //запрос на получение id и роли

                var secondPage = new Contacts(user);

                _ = DisplayAlert("Регистрация", "Успешно!", "Ok");

                await Navigation.PushAsync(secondPage);
            }

        }
    }
}