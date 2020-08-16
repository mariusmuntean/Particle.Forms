using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Particle.Forms
{
    public class ParticleCanvas : SKCanvasView
    {
        private Stopwatch _stopwatch;
        private long _totalElapsedMillis;
        private RandomParticleGenerator _particleGenerator;
        private List<ParticleBase> _particles;

        public ParticleCanvas()
        {
            _stopwatch = new Stopwatch();

            _particleGenerator = new RandomParticleGenerator();
        }

        public static readonly BindableProperty ParticlesPerSecondProperty = BindableProperty.Create(
            nameof(ParticlesPerSecond),
            typeof(int),
            typeof(ParticleCanvas),
            1
        );

        public int ParticlesPerSecond
        {
            get => (int) GetValue(ParticlesPerSecondProperty);
            set => SetValue(ParticlesPerSecondProperty, value);
        }

        public bool IsActive { get; set; }
        public bool IsAddingParticles { get; set; }

        public void Start()
        {
            _stopwatch.Restart();
            IsActive = true;
            IsAddingParticles = true;
            Device.StartTimer(TimeSpan.FromMilliseconds(16), () =>
            {
                _totalElapsedMillis = _stopwatch.ElapsedMilliseconds;

                _particles ??= new List<ParticleBase>();

                // Remove those out of view
                _particles.RemoveAll(particle => !SKRect.Create(SKPoint.Empty, this.CanvasSize).Contains(SKRect.Create(particle.Position, particle.Size)));

                // Add fresh ones if not specified otherwise
                if (IsAddingParticles)
                {
                    var startPositionCount = 9;
                    var startPointSpacing = this.CanvasSize.Width / startPositionCount;

                    _particles.AddRange(_particleGenerator.GenerateFallingParticles(
                        Enumerable.Range(1, startPositionCount).Select(i => new SKPoint(i * startPointSpacing, 0)).ToArray(),
                        ParticlesPerSecond
                    ));
                }
                
                if (!IsActive)
                {
                    _totalElapsedMillis = 0;
                    _stopwatch.Stop();
                    _particles.Clear();
                }

                this.InvalidateSurface();

                return IsActive;
            });
        }

        public void Stop()
        {
            IsActive = false;
        }

        public void StopAddingParticles()
        {
            IsAddingParticles = false;
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