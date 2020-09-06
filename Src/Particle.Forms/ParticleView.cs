using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Particle.Forms.ParticleGenerators;
using Particle.Forms.Particles;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Particle.Forms
{
    /// <summary>
    /// Displays particles. Supports particles falling from the top edge to the bottom edge or radiating from a touch point.
    /// </summary>
    public partial class ParticleView : ContentView
    {
        // Constants
        private const string ParticleAnimationName = "MainParticleAnimation";

        // Vars used for indirection 
        private Func<bool> _getEnableTouchEvents;
        private Action<bool> _setEnableTouchEvents;

        private Action<EventHandler<SKTouchEventArgs>> _addTouchHandler;
        private Action<EventHandler<SKTouchEventArgs>> _removeTouchHandler;

        private Action _invalidateSurface;
        private Func<SKSize> _getCanvasSize;

        // Normal vars
        private Stopwatch _stopwatch;
        private long _totalElapsedMillis;
        private List<ParticleBase> _particles;
        private readonly object _particleLock = new object();
        private int _fallingParticlesPerFrame;

        public ParticleView()
        {
            Init();

            // ToDo: SKPath GetPosition to create particle explosions  https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/curves/information#traversing-the-path
        }

        private void Init()
        {
            _stopwatch = new Stopwatch();
            _totalElapsedMillis = 0L;
            lock (_particleLock)
            {
                _particles ??= new List<ParticleBase>();
            }
            TouchParticleGenerator = new SimpleParticleGenerator();
            FallingParticleGenerator = new FallingParticleGenerator();

            _fallingParticlesPerFrame = (int)Math.Ceiling(FallingParticlesPerSecond / 60.0f);

            // SKCanvasView is not fast enough on Android, so let's use an SKGLView 😄
            // Since there isn't a common interface for both SKCanvasView and SKGLView we're using a bit of indirection
            if (UseSKGLView)
            {
                var skglView = new SKGLView();

                _getCanvasSize = () => skglView.CanvasSize;

                _invalidateSurface = () => skglView.InvalidateSurface();

                _getEnableTouchEvents = () => skglView.EnableTouchEvents;
                _setEnableTouchEvents = enableTouchEvents => skglView.EnableTouchEvents = enableTouchEvents;

                _addTouchHandler = touchHandler => skglView.Touch += touchHandler;
                _removeTouchHandler = touchHandler => skglView.Touch -= touchHandler;

                skglView.PaintSurface += SkglViewOnPaintSurface;
                Content = skglView;
            }
            else
            {
                var skCanvasView = new SKCanvasView();

                _getCanvasSize = () => skCanvasView.CanvasSize;

                _invalidateSurface = () => skCanvasView.InvalidateSurface();

                _getEnableTouchEvents = () => skCanvasView.EnableTouchEvents;
                _setEnableTouchEvents = enableTouchEvents => skCanvasView.EnableTouchEvents = enableTouchEvents;

                _addTouchHandler = touchHandler => skCanvasView.Touch += touchHandler;
                _removeTouchHandler = touchHandler => skCanvasView.Touch -= touchHandler;

                skCanvasView.PaintSurface += OnPaintSurface;
                Content = skCanvasView;
            }
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            StartMainAnimation();
        }

        public bool EnableTouchEvents
        {
            get => _getEnableTouchEvents();
            set => _setEnableTouchEvents(value);
        }

        public IParticleGenerator TouchParticleGenerator { get; set; }

        public IParticleGenerator FallingParticleGenerator { get; set; }

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
                        _particles.AddRange(FallingParticleGenerator.Generate(
                            new[] { e.Location, },
                            (int)Math.Ceiling(DragParticleCount / 60.0d),
                            _convertedConfettiColors
                        ));
                        break;
                    case ParticleMoveType.Radiate:
                        _particles.AddRange(TouchParticleGenerator.Generate(
                            new[] { e.Location, },
                            (int)Math.Ceiling(DragParticleCount / 60.0d),
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
                _particles.AddRange(TouchParticleGenerator.Generate(
                    new[] { e.Location },
                    TapParticleCount,
                    _convertedConfettiColors
                ));
            }
        }

        private void StartMainAnimation()
        {

            _stopwatch.Start();
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

                    var newFallingParticles = FallingParticleGenerator.Generate(
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
                var scale = new SKSize((float)(canvasSize.Width / this.Width), (float)(canvasSize.Height / this.Height));
                foreach (var particle in _particles)
                {
                    particle.Update(_totalElapsedMillis, scale);
                }

                GC.Collect(0, GCCollectionMode.Optimized, false);
                // Console.WriteLine($"Compute duration = {_stopwatch.ElapsedMilliseconds - _totalElapsedMillis}");

                Device.BeginInvokeOnMainThread(() => _invalidateSurface());

            });
            anim.Commit(this, ParticleAnimationName, length: 1000u, repeat: () => IsActive);
        }

        private void StopMainAnimation()
        {
            this.AbortAnimation(ParticleAnimationName);
            _totalElapsedMillis = 0;
            _stopwatch.Reset();
            lock (_particleLock)
            {
                _particles.Clear();
            }

            _invalidateSurface();
        }

        private void PauseMainAnimation()
        {
            this.AbortAnimation(ParticleAnimationName);
            _stopwatch.Stop();
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

        SKPaint _fpsPaint = new SKPaint()
        {
            Color = SKColors.LightGreen,
            TextSize = 32,
            Style = SKPaintStyle.Fill
        };

        private void OnPaint(SKCanvas canvas)
        {
            canvas.Clear();

            var canvasSize = CanvasSize;
            var scale = canvasSize.Width / this.Width;
            _fpsPaint.TextSize = (float)(32.0f * scale);
            var fps = 1000.0f / (_stopwatch.ElapsedMilliseconds - _surfacePaintDuration);

            _surfacePaintDuration = _stopwatch.ElapsedMilliseconds;

            if (IsActive)
            {
                lock (_particleLock)
                {
                    foreach (var particle in _particles)
                    {
                        particle.Paint(canvas);
                    }
                }
            }

            canvas.DrawText($"{fps} FPS",
                canvasSize.Width * 0.1f,
                canvasSize.Height * 0.1f,
                _fpsPaint);

            // Console.WriteLine($"Surface paint duration: {_stopwatch.ElapsedMilliseconds - _surfacePaintDuration}ms");
        }
    }
}