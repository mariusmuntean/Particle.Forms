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

        public SKMatrix TransformationMatrix { get; private set; }


        public virtual void Update(long absoluteElapsedMillis)
        {
            // Determine elapsed time since this particles was created
            var elapsedMillis = absoluteElapsedMillis - _absoluteElapsedMillisPrevious;
            if (_absoluteElapsedMillisPrevious == 0)
            {
                _absoluteElapsedMillisPrevious = absoluteElapsedMillis;
                return;
            }

            _internalAbsoluteMillis += elapsedMillis;
            _absoluteElapsedMillisPrevious = absoluteElapsedMillis;

            // Traversed distance = speed x time
            var dist = TranslationSpeed * _internalAbsoluteMillis * 0.001;

            // New position
            var deg2radFactor = 0.0174533;
            var angle = Direction * deg2radFactor;
            Position = InitialPosition + new SKPoint
            {
                X = (float) (dist * Math.Cos(angle)),
                Y = (float) (dist * Math.Sin(angle))
            };

            TransformationMatrix = SKMatrix.CreateTranslation(-Position.X, -Position.Y);


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
            TransformationMatrix = TransformationMatrix.PostConcat(matrix44.Matrix);
            TransformationMatrix = TransformationMatrix.PostConcat(SKMatrix.CreateRotationDegrees(Orientation.Z, 0, 0)); // SKMatrix44 is kinda slow, so I'm avoiding it to rotate around the Z-axis
            
            // Translate back
            TransformationMatrix = TransformationMatrix.PostConcat(SKMatrix.CreateTranslation(Position.X, Position.Y));

            matrix44.Dispose();
        }

        public void Paint(SKCanvas canvas)
        {
            canvas.Save();

            canvas.SetMatrix(TransformationMatrix);

            Draw(canvas);

            canvas.Restore();
        }

        protected abstract void Draw(SKCanvas canvas);
    }
}