using System;
using Xamarin.Forms;

namespace Particle.Forms.Sample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Demo1Btn_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Demo1());
        }

        private void Demo2Btn_OnClicked(object sender, EventArgs e)
        {
        }
    }
}