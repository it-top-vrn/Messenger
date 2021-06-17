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
        public List<Chat> Chat1 { get; set; }

        public ObservableCollection<string> Items { get; set; }

        public ChatPage()
        {

            InitializeComponent();

            Chat1 = new List<Chat>
            {
                new Chat {name = "Max", message = "" },
          };

        }



        private async void status_button_Clicked(object sender, EventArgs e)
        {
            var Page1 = new MainPage();

            await Navigation.PushAsync(Page1);
        }

        private void Send_button_Clicked(object sender, EventArgs e)
        {
            //    Chat_StackLayout.Children.Add(new Frame { CornerRadius = 30 });


            Chat_StackLayout.Children.Add(new Label { Text = Entry_chat.Text, BackgroundColor = Color.LightGreen });


            //  StackLayout stackLayout = new StackLayout() { Children = { Chat_boxviev } };
            //   Content = Chat_StackLayout;


        }

        private async void settings_button_Clicked(object sender, EventArgs e)
        {
            var setPage = new Settings();

            await Navigation.PushAsync(setPage);
        }

        private async void contacts_button_Clicked(object sender, EventArgs e)
        {
            var secondPage = new Contacts();

            await Navigation.PushAsync(secondPage);
        }
    }
}