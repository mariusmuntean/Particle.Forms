using SkiaSharp;

namespace Particle.Forms
{
    public class RectParticle : ParticleBase
    {
        protected readonly SKPaint Paint;
        private float _blurFactor;

        public RectParticle(SKColor color,
            SKPoint3 rotationSpeed,
            float translationSpeed,
            float direction,
            SKPoint3 orientation,
            SKPoint position,
            SKSize size, float blurFactor)
            : base(rotationSpeed, translationSpeed, direction, orientation, position, size)
        {
            _blurFactor = blurFactor;

            Paint = new SKPaint
            {
                IsAntialias = true,
                Color = color,
                Style = SKPaintStyle.Fill,
                ImageFilter = SKImageFilter.CreateBlur(blurFactor, blurFactor)
            };
        }

        public SKColor Color
        {
            get => Paint.Color;
            set => Paint.Color = value;
        }

        public float BlurFactor
        {
            get => _blurFactor;
            set
            {
                _blurFactor = value;
                Paint.ImageFilter = SKImageFilter.CreateBlur(_blurFactor, _blurFactor);
            }
        }

        protected override void Draw(SKCanvas canvas)
        {
            canvas.DrawRect(Position.X - Size.Width / 2, Position.Y - Size.Height / 2, Size.Width, Size.Height, Paint);
        }
    }
}