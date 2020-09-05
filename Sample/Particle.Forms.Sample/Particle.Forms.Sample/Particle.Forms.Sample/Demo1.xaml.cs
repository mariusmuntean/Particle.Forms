using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Particle.Forms.Sample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Demo1 : ContentPage
    {
        public Demo1()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MyParticleCanvas.IsRunning = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MyParticleCanvas.IsRunning = false;
        }
    }
}