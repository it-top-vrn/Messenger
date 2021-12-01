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
        public ChangePassword(User _user)
        {
            InitializeComponent();
            user = _user;
        }

        private async void button_changePassword_Clicked(object sender, EventArgs e)
        {
            string oldPass = entry_oldPass.Text;
            string pass1 = entry_newPass.Text;
            string pass2 = entry_newPass2.Text;

            if (oldPass != "") //отправить запрос на аутентификацию
            {
                if (pass1 != pass2)
                {
                    _ = DisplayAlert("Ошибка", "Пароли не совпадают, попробуйте еще раз.", "Ok");
                }
                else
                {
                    user.password = pass1;

                    //отправить запрос на смену пароля

                    var secondPage = new Contacts(user);

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