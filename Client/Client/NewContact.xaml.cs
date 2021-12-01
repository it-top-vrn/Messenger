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
    public partial class NewContact : ContentPage
    {
        User user = new User();
        public NewContact(User _user)
        {
            InitializeComponent();
            user = _user;
        }
      
        private async void butoon_cancel_Clicked(object sender, EventArgs e)
        {
            var secondPage = new Contacts(user);

            await Navigation.PushAsync(secondPage);
        }

        private void butoon_ADDContact_Clicked(object sender, EventArgs e)
        {
            User user1 = new User();

            user1.nickName = entry_name.Text;

            user.contacts.Add(user1);
        }
    }
}
