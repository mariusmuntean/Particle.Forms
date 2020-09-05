using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Particle.Forms
{
    public partial class ParticleCanvas : ContentView
    {
        private const string ParticleAnimationName = "MainParticleAnimation";

        // Vars used for indirection 
        private readonly SKGLView _skglView;
        private readonly SKCanvasView _skCanvasView;

        private readonly Func<bool> _getEnableTouchEvents;
        private readonly Action<bool> _setEnableTouchEvents;

        private readonly Action<EventHandler<SKTouchEventArgs>> _addTouchHandler;
        private readonly Action<EventHandler<SKTouchEventArgs>> _removeTouchHandler;

        private readonly Action _invalidateSurface;
        private readonly Func<SKSize> _getCanvasSize;

        // Normal vars
        private readonly Stopwatch _stopwatch;
        private long _totalElapsedMillis;
        private readonly RandomParticleGenerator _particleGenerator;
        private readonly List<ParticleBase> _particles;
        private readonly object _particleLock = new object();
        private int _fallingParticlesPerFrame;

        public ParticleCanvas()
        {
            _stopwatch = new Stopwatch();
            _particles ??= new List<ParticleBase>();
            _particleGenerator = new RandomParticleGenerator();

            _fallingParticlesPerFrame = (int) Math.Ceiling(FallingParticlesPerSecond / 60.0f);

            // SKCanvasView is not fast enough on Android, so let's use an SKGLView 😄
            // Since there isn't a common interface for both SKCanvasView and SKGLView we're using a bit of indirection
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                {
                    _skglView = new SKGLView();

                    _getCanvasSize = () => _skglView.CanvasSize;

                    _invalidateSurface = () => _skglView.InvalidateSurface();

                    _getEnableTouchEvents = () => _skglView.EnableTouchEvents;
                    _setEnableTouchEvents = enableTouchEvents => _skglView.EnableTouchEvents = enableTouchEvents;

                    _addTouchHandler = touchHandler => _skglView.Touch += touchHandler;
                    _removeTouchHandler = touchHandler => _skglView.Touch -= touchHandler;

                    _skglView.PaintSurface += SkglViewOnPaintSurface;
                    Content = _skglView;
                    break;
                }
                default:
                {
                    _skCanvasView = new SKCanvasView();

                    _getCanvasSize = () => _skCanvasView.CanvasSize;

                    _invalidateSurface = () => _skCanvasView.InvalidateSurface();

                    _getEnableTouchEvents = () => _skCanvasView.EnableTouchEvents;
                    _setEnableTouchEvents = enableTouchEvents => _skCanvasView.EnableTouchEvents = enableTouchEvents;

                    _addTouchHandler = touchHandler => _skCanvasView.Touch += touchHandler;
                    _removeTouchHandler = touchHandler => _skCanvasView.Touch -= touchHandler;

                    _skCanvasView.PaintSurface += OnPaintSurface;
                    Content = _skCanvasView;
                    break;
                }
            }


            // ToDo: SKPath GetPosition to create particle explosions  https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/curves/information#traversing-the-path
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            IsActive = true;
        }

        public bool EnableTouchEvents
        {
            get => _getEnableTouchEvents();
            set => _setEnableTouchEvents(value);
        }

        public SKSize CanvasSize => _getCanvasSize();

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

            lock (_particleLock)
            {
                switch (DragParticleMoveType)
                {
                    case ParticleMoveType.Fall:
                        _particles.AddRange(_particleGenerator.GenerateFallingParticles(
                            new[] {e.Location,},
                            (int) Math.Ceiling(DragParticleCount / 60.0d),
                            _convertedConfettiColors
                        ));
                        break;
                    case ParticleMoveType.Radiate:
                        _particles.AddRange(_particleGenerator.Generate(
                            new[] {e.Location,},
                            (int) Math.Ceiling(DragParticleCount / 60.0d),
                            _convertedConfettiColors
                        ));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void OnTapped(SKTouchEventArgs e)
        {
            if (!AddParticlesOnTap)
            {
                return;
            }

            lock (_particleLock)
            {
                _particles.AddRange(_particleGenerator.Generate(
                    new[] {e.Location},
                    TapParticleCount,
                    _convertedConfettiColors
                ));
            }
        }

        private void StartMainAnimation()
        {
            _stopwatch.Restart();
            var anim = new Animation(d =>
            {
                // Console.WriteLine($"Timer interval: {_stopwatch.ElapsedMilliseconds - _totalElapsedMillis}ms");
                _totalElapsedMillis = _stopwatch.ElapsedMilliseconds;

                var canvasSize = CanvasSize;

                // Remove those out of view
                lock (_particleLock)
                {
                    _particles.RemoveAll(particle => !SKRect.Create(SKPoint.Empty, canvasSize).Contains(SKRect.Create(particle.Position, particle.Size)));
                }

                // Add fresh ones if not specified otherwise
                if (HasFallingParticles)
                {
                    var startPositionCount = 9;
                    var startPointSpacing = canvasSize.Width / startPositionCount;

                    var newFallingParticles = _particleGenerator.GenerateFallingParticles(
                        Enumerable.Range(1, startPositionCount).Select(i => new SKPoint(i * startPointSpacing, 0)).ToArray(),
                        _fallingParticlesPerFrame,
                        _convertedConfettiColors
                    );
                    lock (_particleLock)
                    {
                        _particles.AddRange(newFallingParticles);
                    }
                }

                // Update the current particles
                var scale = new SKSize((float) (canvasSize.Width / this.Width), (float) (canvasSize.Height / this.Height));
                _particles.ForEach(particle => particle.Update(_totalElapsedMillis, scale));


                GC.Collect(0, GCCollectionMode.Optimized, false);
                // Console.WriteLine($"Compute duration = {_stopwatch.ElapsedMilliseconds - _totalElapsedMillis}");

                Device.BeginInvokeOnMainThread(() => _invalidateSurface());
            });
            anim.Commit(this, ParticleAnimationName, repeat: () => IsActive);
        }

        private void StopMainAnimation()
        {
            this.AbortAnimation(ParticleAnimationName);
            _totalElapsedMillis = 0;
            _stopwatch.Stop();
            lock (_particleLock)
            {
                _particles.Clear();
            }
        }

        private void SkglViewOnPaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            OnPaint(e.Surface.Canvas);
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            OnPaint(e.Surface.Canvas);
        }

        private long _surfacePaintDuration = 0L;

        private void OnPaint(SKCanvas canvas)
        {
            _surfacePaintDuration = _stopwatch.ElapsedMilliseconds;
            canvas.Clear();

            if (IsActive)
            {
                lock (_particleLock)
                {
                    _particles?.ForEach(particle => particle.Paint(canvas));
                }
            }

            // Console.WriteLine($"Surface paint duration: {_stopwatch.ElapsedMilliseconds - _surfacePaintDuration}ms");
        }
    }
}