using System;
using SkiaSharp;

namespace Particle.Forms.Particles
{
    public abstract class ParticleBase
    {
        protected SKPoint InitialPosition;
        protected SKPoint3 InitialOrientation;

        private long _absoluteElapsedMillisPrevious = 0;
        private long _internalAbsoluteMillis = 0;
        
        // Caching these matrices speeds up the Update() function a lot
        private SKMatrix44 _totalRotationMatrix = new SKMatrix44();
        private SKMatrix44 _xAxisRotationMatrix = new SKMatrix44();
        private SKMatrix44 _yAxisRotationMatrix = new SKMatrix44();
        private SKMatrix44 _zAxisRotationMatrix = new SKMatrix44();

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

        /// <summary>
        /// Updates the particle's position and orientation based on the given time
        /// </summary>
        /// <para>Call this method first and then <see cref="Paint"/></para>
        /// <para>This method doesn't need to be called on the UI/Main thread</para>
        /// <param name="absoluteElapsedMillis"></param>
        public virtual void Update(long absoluteElapsedMillis, SKSize scale)
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

            // New Scale
            TransformationMatrix = TransformationMatrix.PostConcat(SKMatrix.CreateScale(scale.Width, scale.Height));

            // New Orientation
            Orientation = InitialOrientation + new SKPoint3
            {
                X = _internalAbsoluteMillis * 0.001f * RotationSpeed.X,
                Y = _internalAbsoluteMillis * 0.001f * RotationSpeed.Y,
                Z = _internalAbsoluteMillis * 0.001f * RotationSpeed.Z
            };

            _totalRotationMatrix.SetIdentity();
            
            _xAxisRotationMatrix.SetRotationAboutDegrees(1, 0, 0, Orientation.X);
            _totalRotationMatrix.PostConcat(_xAxisRotationMatrix);
            
            _yAxisRotationMatrix.SetRotationAboutDegrees(0, 1, 0, Orientation.Y);
            _totalRotationMatrix.PostConcat(_yAxisRotationMatrix);
            
            _zAxisRotationMatrix.SetRotationAboutDegrees(0, 0, 1, Orientation.Z);
            _totalRotationMatrix.PostConcat(_zAxisRotationMatrix);
            
            TransformationMatrix = TransformationMatrix.PostConcat(_totalRotationMatrix.Matrix);

            // Translate back
            TransformationMatrix = TransformationMatrix.PostConcat(SKMatrix.CreateTranslation(Position.X, Position.Y));
            
        }

        /// <summary>
        /// Paints this particle on the given <see cref="SKCanvas"/> instance.
        /// </summary>
        /// <para>Call this method after you called <see cref="Update"/></para>
        /// <param name="canvas"></param>
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