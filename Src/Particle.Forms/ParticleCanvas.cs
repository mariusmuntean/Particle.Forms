using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Particle.Forms
{
    public partial class ParticleCanvas : SKCanvasView
    {
        private Stopwatch _stopwatch;
        private long _totalElapsedMillis;
        private RandomParticleGenerator _particleGenerator;
        private List<ParticleBase> _particles;

        public ParticleCanvas()
        {
            _stopwatch = new Stopwatch();
            _particles ??= new List<ParticleBase>();
            _particleGenerator = new RandomParticleGenerator();

            // ToDo: SKPath GetPosition to create particle explosions  https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/curves/information#traversing-the-path
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            IsActive = true;
        }

        private void OnTouch(object sender, SKTouchEventArgs e)
        {
            // ToDo: detect the finger motion to decide if valid tap or not

            switch (e.ActionType)
            {
                case SKTouchAction.Entered:
                    break;
                case SKTouchAction.Pressed:
                    break;
                case SKTouchAction.Moved:
                    OnMoved(e);
                    break;
                case SKTouchAction.Released:
                    OnTapped(e);
                    break;
                case SKTouchAction.Cancelled:
                    break;
                case SKTouchAction.Exited:
                    break;
                case SKTouchAction.WheelChanged:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            e.Handled = true;
        }

        private void OnMoved(SKTouchEventArgs e)
        {
            if (!AddParticlesOnDrag)
            {
                return;
            }

            switch (DragParticleMoveType)
            {
                case ParticleMoveType.Fall:
                    _particles.AddRange(_particleGenerator.GenerateFallingParticles(
                        new[] {e.Location,},
                        (int) Math.Ceiling(DragParticleCount / 60.0d)
                    ));
                    break;
                case ParticleMoveType.Radiate:
                    _particles.AddRange(_particleGenerator.Generate(
                        new[] {e.Location,},
                        (int) Math.Ceiling(DragParticleCount / 60.0d)
                    ));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnTapped(SKTouchEventArgs e)
        {
            if (!AddParticlesOnTap)
            {
                return;
            }

            _particles.AddRange(_particleGenerator.Generate(
                new[] {e.Location},
                TapParticleCount
            ));
        }

        private void Start()
        {
            _stopwatch.Restart();
            Device.StartTimer(TimeSpan.FromMilliseconds(16), () =>
            {
                _totalElapsedMillis = _stopwatch.ElapsedMilliseconds;

                // Remove those out of view
                _particles.RemoveAll(particle => !SKRect.Create(SKPoint.Empty, CanvasSize).Contains(SKRect.Create(particle.Position, particle.Size)));

                // Add fresh ones if not specified otherwise
                if (HasFallingParticles)
                {
                    var startPositionCount = 9;
                    var startPointSpacing = CanvasSize.Width / startPositionCount;

                    _particles.AddRange(_particleGenerator.GenerateFallingParticles(
                        Enumerable.Range(1, startPositionCount).Select(i => new SKPoint(i * startPointSpacing, 0)).ToArray(),
                        (int) Math.Ceiling(FallingParticlesPerSecond / 60.0d)
                    ));
                }

                if (!IsActive)
                {
                    _totalElapsedMillis = 0;
                    _stopwatch.Stop();
                    _particles.Clear();
                }

                InvalidateSurface();

                return IsActive;
            });
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            // Let the event fire
            base.OnPaintSurface(e);

            var canvasSize = e.Info.Size;
            var canvas = e.Surface.Canvas;

            canvas.Clear();

            if (IsActive)
            {
                _particles?.ForEach(particle => particle.Update(canvas, _totalElapsedMillis));
            }
        }
    }
}