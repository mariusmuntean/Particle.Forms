using System;
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

        public ParticleCanvas()
        {
            _stopwatch = new Stopwatch();

            _rectParticle1 = new RectParticle(SKColors.Fuchsia,
                new SKPoint3(90, 90, 90),
                0.01f,
                90,
                new SKPoint3(0, 0, 0),
                new SKPoint(150, 30),
                new SKSize(20, 20));
            
            _rectParticle2 = new RectParticle(SKColors.Fuchsia,
                new SKPoint3(360, 360, 360),
                0.01f,
                90,
                new SKPoint3(0, 0, 0),
                new SKPoint(300, 30),
                new SKSize(20, 20));
        }

        public bool IsActive { get; set; }

        public void Start()
        {
            _stopwatch.Start();
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
            
            _rectParticle1.Update(canvas, _totalElapsedMillis);
            _rectParticle2.Update(canvas, _totalElapsedMillis);
        }
    }
}