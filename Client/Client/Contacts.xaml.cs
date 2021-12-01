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


        public Contacts(User _user)
        {
            InitializeComponent();
            user = _user;
        }

        private async void status_button_Clicked(object sender, EventArgs e)
        {
            var secondPage = new ChatList(user);

            await Navigation.PushAsync(secondPage);

            //запрос на загрузку списка контактов
        }


        private async void chat_button_Clicked(object sender, EventArgs e)
        {
            var secondPage = new ChatPage(user);

            await Navigation.PushAsync(secondPage);
        }

        private async void settings_button_Clicked(object sender, EventArgs e)
        {
            var secondPage = new Settings(user);

            await Navigation.PushAsync(secondPage);
        }

        private async void butoon_newContact_Clicked(object sender, EventArgs e)
        {

            var secondPage = new NewContact(user);

            await Navigation.PushAsync(secondPage);

        }
        private void ListContact_Focused(object sender, FocusEventArgs e)
        {

        }

    }
}