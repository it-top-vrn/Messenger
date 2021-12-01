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
    public partial class ChatList : ContentPage
    {
        User user = new User();
        public ChatList(User _user)
        {
            InitializeComponent();
            user = _user;

            //запрос на загрузку списка диалогов
        }

        private async void chat_button_Clicked(object sender, EventArgs e)
        {
            var secondPage = new ChatPage(user);

            await Navigation.PushAsync(secondPage);
        }

        private async void ListView_Focused(object sender, FocusEventArgs e)
        {
            var secondPage = new ChatPage(user);

            await Navigation.PushAsync(secondPage);
        }

        private async void settings_button_Clicked(object sender, EventArgs e)
        {
            var setPage = new Settings(user);

            await Navigation.PushAsync(setPage);
        }

        private async void contacts_button_Clicked(object sender, EventArgs e)
        {
            var contactPage = new Contacts(user);

            await Navigation.PushAsync(contactPage);
        }
    }
}