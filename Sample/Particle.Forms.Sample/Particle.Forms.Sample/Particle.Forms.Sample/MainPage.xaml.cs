using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Particle.Forms.Sample
{
    public partial class MainPage : ContentPage
    {
        private const string TitleAnimationName = "TitleAnimation";

        private readonly Color[] _titleColors =
        {
            Color.DodgerBlue,
            Color.Gold,
            Color.CornflowerBlue,
            Color.PaleVioletRed,
            Color.LawnGreen,
            Color.LightPink,
            Color.DarkTurquoise,
            Color.Goldenrod,
            Color.DarkViolet,
            Color.Red
        };

        private uint _totalAnimationDuration = 3000;
        private Animation _parentAnimation;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _parentAnimation ??= GetTitleAnimation();
            _parentAnimation.Commit(this, TitleAnimationName, length: _totalAnimationDuration, easing: Easing.CubicOut, repeat: () => true);

            MyParticleCanvas.IsRunning = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            this.AbortAnimation(TitleAnimationName);

            MyParticleCanvas.IsRunning = false;
        }

        private async void Demo1Btn_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Demo1());
        }

        private void Demo2Btn_OnClicked(object sender, EventArgs e)
        {
        }

        private Animation GetTitleAnimation()
        {
            var animations = new List<Animation>(_titleColors.Length);

            for (var i = 0; i < _titleColors.Length; i++)
            {
                var currentColor = _titleColors[i];
                var nextColor = _titleColors[(i + 1) % _titleColors.Length];

                animations.Add(new Animation(d =>
                    {
                        var r = currentColor.R + (d * (nextColor.R - currentColor.R));
                        var g = currentColor.G + (d * (nextColor.G - currentColor.G));
                        var b = currentColor.B + (d * (nextColor.B - currentColor.B));
                        var h = currentColor.Hue + (d * (nextColor.Hue - currentColor.Hue));
                        var s = currentColor.Saturation + (d * (nextColor.Saturation - currentColor.Saturation));
                        var l = currentColor.Luminosity + (d * (nextColor.Luminosity - currentColor.Luminosity));

                        TitleLabel.TextColor = new Color(r, g, b).WithHue(h).WithLuminosity(l).WithSaturation(s);
                    }
                ));
            }

            var parentAnimation = new Animation();
            var childAnimationDuration = ((float) _totalAnimationDuration) / animations.Count;
            for (var i = 0; i < animations.Count; i++)
            {
                var currentAnimation = animations[i];
                parentAnimation.Add(
                    (i * childAnimationDuration) / _totalAnimationDuration,
                    ((i + 1) * childAnimationDuration) / _totalAnimationDuration,
                    currentAnimation);
            }

            return parentAnimation;
        }
    }
}