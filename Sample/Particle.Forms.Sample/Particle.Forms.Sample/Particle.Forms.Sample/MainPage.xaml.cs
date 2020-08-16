using System;
using Xamarin.Forms;

namespace Particle.Forms.Sample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            ActionButton.Text = "Start";
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            if (MyParticleCanvas.IsActive)
            {
                if (MyParticleCanvas.IsAddingParticles)
                {
                    MyParticleCanvas.StopAddingParticles();
                    ActionButton.Text = "Stop";
                }
                else
                {
                    MyParticleCanvas.Stop();
                    ActionButton.Text = "Start";
                }
            }
            else
            {
                MyParticleCanvas.Start();
                ActionButton.Text = "Stop adding";
            }
        }
    }
}