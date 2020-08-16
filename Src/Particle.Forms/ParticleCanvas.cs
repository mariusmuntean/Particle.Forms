using System;
using System.Collections.Generic;
using System.Diagnostics;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Particle.Forms
{
    public class ParticleCanvas : SKCanvasView
    {
        private Stopwatch _stopwatch;
        private long _totalElapsedMillis;
        private RectParticle _rectParticle1;
        private RectParticle _rectParticle2;

        private RandomParticleGenerator _particleGenerator;
        private List<ParticleBase> _particles;

        public ParticleCanvas()
        {
            _stopwatch = new Stopwatch();

            _rectParticle1 = new RectParticle(SKColors.Fuchsia,
                new SKPoint3(90, 90, 90),
                100f,
                90,
                new SKPoint3(0, 0, 0),
                new SKPoint(150, 30),
                new SKSize(20, 20),
                0.0f);

            _rectParticle2 = new RectParticle(SKColors.Fuchsia.WithAlpha(128),
                new SKPoint3(360, 360, 360),
                150f,
                90,
                new SKPoint3(0, 0, 0),
                new SKPoint(300, 30),
                new SKSize(20, 20),
                3.0f);

            _particleGenerator = new RandomParticleGenerator();
        }

        public bool IsActive { get; set; }

        public void Start()
        {
            _stopwatch.Restart();
            IsActive = true;
            Device.StartTimer(TimeSpan.FromMilliseconds(16), () =>
            {
                _totalElapsedMillis = _stopwatch.ElapsedMilliseconds;
                this.InvalidateSurface();

                if (!IsActive)
                {
                    _totalElapsedMillis = 0;
                    _stopwatch.Stop();
                }

                return IsActive;
            });

            _particles = _particles ?? new List<ParticleBase>();
            _particles.AddRange(_particleGenerator.Generate(new[]
            {
                new SKPoint(this.CanvasSize.Width / 2, this.CanvasSize.Height / 2)
            }));
        }

        public void Stop()
        {
            IsActive = false;
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
                _rectParticle1.Update(canvas, _totalElapsedMillis);
                _rectParticle2.Update(canvas, _totalElapsedMillis);

                _particles?.ForEach(particle => particle.Update(canvas, _totalElapsedMillis));
            }
        }
    }
}