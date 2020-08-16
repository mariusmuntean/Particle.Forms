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

        private void Button_OnClicked(object sender, EventArgs e)
        {
            if (MyParticleCanvas.IsActive)
            {
                MyParticleCanvas.Stop();
            }
            else
            {
                MyParticleCanvas.Start();
            }
        }
    }
}