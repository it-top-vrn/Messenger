using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Client
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
        public ObservableCollection<string> Items { get; set; }

        User user = new User();
        public ChatPage(User _user)
        {

            InitializeComponent();
            user = _user;

            //запрос на загрузку сообщений диалога

        }

        private async void status_button_Clicked(object sender, EventArgs e)
        {
            var Page1 = new ChatList(user);

            await Navigation.PushAsync(Page1);
        }

        private void Send_button_Clicked(object sender, EventArgs e)
        {
            Chat_StackLayout.Children.Add(new Label { Text = Entry_chat.Text, BackgroundColor = Color.LightGreen });
            
            //Chat_StackLayout.Children.Add(new Frame { CornerRadius = 30, BindingContext = Entry_chat.Text, BackgroundColor = Color.LightGreen });
        }

        private async void settings_button_Clicked(object sender, EventArgs e)
        {
            var setPage = new Settings(user);

            await Navigation.PushAsync(setPage);
        }

        private async void contacts_button_Clicked(object sender, EventArgs e)
        {
            var secondPage = new Contacts(user);

            await Navigation.PushAsync(secondPage);
        }
    }
}
