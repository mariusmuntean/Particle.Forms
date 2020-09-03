using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Particle.Forms
{
    public partial class ParticleCanvas : ContentView
    {
        // Vars used for indirection 
        private SKGLView _skglView;
        private SKCanvasView _skCanvasView;

        private Func<bool> _getEnableTouchEvents;
        private Action<bool> _setEnableTouchEvents;

        private Action<EventHandler<SKTouchEventArgs>> _addTouchHandler;
        private Action<EventHandler<SKTouchEventArgs>> _removeTouchHandler;

        private Action _invalidateSurface;
        private Func<SKSize> _getCanvasSize;

        // Normal vars
        private Stopwatch _stopwatch;
        private long _totalElapsedMillis;
        private RandomParticleGenerator _particleGenerator;
        private List<ParticleBase> _particles;
        private object _particleLock = new object();

        public ParticleCanvas()
        {
            _stopwatch = new Stopwatch();
            _particles ??= new List<ParticleBase>();
            _particleGenerator = new RandomParticleGenerator();

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
                    TapParticleCount
                ));
            }
        }

        private void Start()
        {
            _stopwatch.Restart();
            Device.StartTimer(TimeSpan.FromMilliseconds(16), () =>
            {
                _totalElapsedMillis = _stopwatch.ElapsedMilliseconds;

                lock (_particleLock)
                {
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
                }

                _invalidateSurface();

                return IsActive;
            });
        }

        private void SkglViewOnPaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            OnPaint(e.Surface.Canvas);
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            OnPaint(e.Surface.Canvas);
        }

        private void OnPaint(SKCanvas canvas)
        {
            canvas.Clear();

            if (IsActive)
            {
                lock (_particleLock)
                {
                    _particles?.ForEach(particle => particle.Update(canvas, _totalElapsedMillis));
                }
            }
        }
    }
}