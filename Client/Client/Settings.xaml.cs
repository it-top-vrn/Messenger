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
    public partial class Settings : ContentPage
    {
        User user = new User();
        public Settings(User _user)
        {
            InitializeComponent();
            user = _user;

            label_login.Text +=  " " + user.nickName;
            label_id.Text +=  " " + user.password;
        }
        private async void contacts_button_Clicked(object sender, EventArgs e)
        {
            var secondPage = new Contacts(user);

            await Navigation.PushAsync(secondPage);
        }

        private async void status_button_Clicked(object sender, EventArgs e)
        {
            var secondPage = new ChatList(user);

            await Navigation.PushAsync(secondPage);
        }

        private async void chat_button_Clicked(object sender, EventArgs e)
        {
            var secondPage = new ChatPage(user);

            await Navigation.PushAsync(secondPage);
        }
        private async void button_exit_Clicked(object sender, EventArgs e)
        {
            var secondPage = new MainPage();

            await Navigation.PushAsync(secondPage);
        }

        private async void button_changePassword_Clicked(object sender, EventArgs e)
        {
            var secondPage = new ChangePassword(user);

            await Navigation.PushAsync(secondPage);
        }
    }
}