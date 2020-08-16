using System;
using SkiaSharp;

namespace Particle.Forms
{
    public abstract class ParticleBase
    {
        protected SKPoint InitialPosition;
        protected SKPoint3 InitialOrientation;

        private long _absoluteElapsedMillisPrevious = 0;
        private long _internalAbsoluteMillis = 0;

        public ParticleBase(SKPoint3 rotationSpeed, float translationSpeed, float direction, SKPoint3 orientation, SKPoint position, SKSize size)
        {
            RotationSpeed = rotationSpeed;
            TranslationSpeed = translationSpeed;
            Direction = direction;

            Orientation = orientation;
            InitialOrientation = orientation;

            Position = position;
            InitialPosition = position;

            Size = size;
        }

        public SKSize Size { get; set; }
        public SKPoint Position { get; set; }
        public SKPoint3 Orientation { get; set; }

        public float Direction { get; set; }
        public float TranslationSpeed { get; set; }

        public SKPoint3 RotationSpeed { get; set; }


        public virtual void Update(SKCanvas canvas, long absoluteElapsedMillis)
        {
            var elapsedMillis = absoluteElapsedMillis - _absoluteElapsedMillisPrevious;
            if (_absoluteElapsedMillisPrevious == 0)
            {
                _absoluteElapsedMillisPrevious = absoluteElapsedMillis;
                return;
            }

            _internalAbsoluteMillis += elapsedMillis;
            _absoluteElapsedMillisPrevious = absoluteElapsedMillis;

            canvas.Save();

            // Traversed distance speed x time
            var dist = TranslationSpeed * _internalAbsoluteMillis * 0.001;

            // New position
            var deg2radFactor = 0.0174533;
            var angle = Direction * deg2radFactor;
            Position = InitialPosition + new SKPoint
            {
                X = (float) (dist * Math.Cos(angle)),
                Y = (float) (dist * Math.Sin(angle))
            };

            var matrix = SKMatrix.CreateTranslation(-Position.X, -Position.Y);

            // New Orientation
            Orientation = InitialOrientation + new SKPoint3
            {
                X = _internalAbsoluteMillis * 0.001f * RotationSpeed.X,
                Y = _internalAbsoluteMillis * 0.001f * RotationSpeed.Y,
                Z = _internalAbsoluteMillis * 0.001f * RotationSpeed.Z
            };

            var matrix44 = SKMatrix44.CreateIdentity();
            matrix44.PostConcat(SKMatrix44.CreateRotationDegrees(1, 0, 0, Orientation.X));
            matrix44.PostConcat(SKMatrix44.CreateRotationDegrees(0, 1, 0, Orientation.Y));
            matrix44.PostConcat(SKMatrix44.CreateRotationDegrees(0, 0, 1, Orientation.Z));

            // Apply transforms
            matrix = matrix.PostConcat(matrix44.Matrix);
            matrix = matrix.PostConcat(SKMatrix.CreateTranslation(Position.X, Position.Y));
            canvas.SetMatrix(matrix);

            Draw(canvas);

            canvas.Restore();
        }

        protected abstract void Draw(SKCanvas canvas);
    }

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

    public class EllipseParticle : RectParticle
    {
        public EllipseParticle(SKColor color,
            SKPoint3 rotationSpeed,
            float translationSpeed,
            float direction,
            SKPoint3 orientation,
            SKPoint position,
            SKSize size,
            float blurFactor) : base(color, rotationSpeed, translationSpeed, direction, orientation, position, size, blurFactor)
        {
        }

        protected override void Draw(SKCanvas canvas)
        {
            canvas.DrawOval(Position.X, Position.Y, Size.Width / 2, Size.Height / 2, Paint);
        }
    }
}