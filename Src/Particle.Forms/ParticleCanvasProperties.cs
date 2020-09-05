using System.Linq;
using Particle.Forms.ParticleGenerators;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Particle.Forms
{
    public partial class ParticleCanvas
    {
        private void DisableTouchInput()
        {
            _removeTouchHandler(OnTouch);
            EnableTouchEvents = false;
        }

        private void EnableTouchInput()
        {
            _removeTouchHandler(OnTouch);
            EnableTouchEvents = true;
            _addTouchHandler(OnTouch);
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == IsActiveProperty.PropertyName)
            {
                if (IsActive)
                {
                    EnableTouchInput();
                    if (IsRunning)
                    {
                        StartMainAnimation();
                    }
                }
                else
                {
                    DisableTouchInput();
                    StopMainAnimation();

                    IsRunning = false;
                }
            }
            else if (propertyName == AddParticlesOnTapProperty.PropertyName
                     || propertyName == AddParticlesOnDragProperty.PropertyName)
            {
                if (AddParticlesOnTap || AddParticlesOnDrag)
                {
                    EnableTouchInput();
                }
                else
                {
                    DisableTouchInput();
                }
            }
            else if(propertyName == IsRunningProperty.PropertyName && IsActive)
            {
                if (IsRunning)
                {
                    StartMainAnimation();
                }
                else
                {
                    PauseMainAnimation();
                }
            }
        }

        public static readonly BindableProperty IsActiveProperty = BindableProperty.Create(
            nameof(IsActive),
            typeof(bool),
            typeof(ParticleCanvas),
            true
        );

        /// <summary>
        /// Whether or not the control is displaying particles.
        /// </summary>
        public bool IsActive
        {
            get => (bool) GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }
        
        public static readonly BindableProperty IsRunningProperty = BindableProperty.Create(
            nameof(IsRunning),
            typeof(bool),
            typeof(ParticleCanvas),
            true
        );

        /// <summary>
        /// Whether or not the control is animating particles
        /// </summary>
        public bool IsRunning
        {
            get => (bool) GetValue(IsRunningProperty);
            set => SetValue(IsRunningProperty, value);
        }

        public static readonly BindableProperty HasFallingParticlesProperty = BindableProperty.Create(
            nameof(HasFallingParticles),
            typeof(bool),
            typeof(ParticleCanvas),
            false
        );

        /// <summary>
        /// Whether or not falling particles should be shown.
        /// </summary>
        public bool HasFallingParticles
        {
            get => (bool) GetValue(HasFallingParticlesProperty);
            set => SetValue(HasFallingParticlesProperty, value);
        }

        public static readonly BindableProperty FallingParticlesPerSecondProperty = BindableProperty.Create(
            nameof(FallingParticlesPerSecond),
            typeof(int),
            typeof(ParticleCanvas),
            60
        );

        /// <summary>
        /// Amount of new particles to be added every second when <see cref="HasFallingParticles"/> is true.
        /// </summary>
        public int FallingParticlesPerSecond
        {
            get => (int) GetValue(FallingParticlesPerSecondProperty);
            set => SetValue(FallingParticlesPerSecondProperty, value);
        }

        public static readonly BindableProperty AddParticlesOnTapProperty = BindableProperty.Create(
            nameof(AddParticlesOnTap),
            typeof(bool),
            typeof(ParticleCanvas),
            false
        );

        /// <summary>
        /// Whether or not to add particles on tap.
        /// </summary>
        public bool AddParticlesOnTap
        {
            get => (bool) GetValue(AddParticlesOnTapProperty);
            set => SetValue(AddParticlesOnTapProperty, value);
        }

        public static readonly BindableProperty TapParticleCountProperty = BindableProperty.Create(
            nameof(TapParticleCount),
            typeof(int),
            typeof(ParticleCanvas),
            30
        );

        /// <summary>
        /// Amount of particles to add on tap when <see cref="AddParticlesOnTap"/> is true.
        /// </summary>
        public int TapParticleCount
        {
            get => (int) GetValue(TapParticleCountProperty);
            set => SetValue(TapParticleCountProperty, value);
        }


        public static readonly BindableProperty AddParticlesOnDragProperty = BindableProperty.Create(
            nameof(AddParticlesOnDrag),
            typeof(bool),
            typeof(ParticleCanvas),
            false
        );

        /// <summary>
        /// Whether or not to add particles on drag.
        /// </summary>
        public bool AddParticlesOnDrag
        {
            get => (bool) GetValue(AddParticlesOnDragProperty);
            set => SetValue(AddParticlesOnDragProperty, value);
        }

        public static readonly BindableProperty DragParticleCountProperty = BindableProperty.Create(
            nameof(DragParticleCount),
            typeof(int),
            typeof(ParticleCanvas),
            60
        );

        /// <summary>
        /// Amount of particles to add on drag when <see cref="AddParticlesOnDrag"/> is true.
        /// </summary>
        public int DragParticleCount
        {
            get => (int) GetValue(DragParticleCountProperty);
            set => SetValue(DragParticleCountProperty, value);
        }

        public static readonly BindableProperty DragParticleMoveTypeProperty = BindableProperty.Create(
            nameof(DragParticleMoveType),
            typeof(ParticleMoveType),
            typeof(ParticleCanvas),
            ParticleMoveType.Fall
        );

        /// <summary>
        /// particle movement type while dragging
        /// </summary>
        public ParticleMoveType DragParticleMoveType
        {
            get => (ParticleMoveType) GetValue(DragParticleMoveTypeProperty);
            set => SetValue(DragParticleMoveTypeProperty, value);
        }

        
        private SKColor[] _convertedConfettiColors = RandomParticleGenerator.DefaultColors;
        
        public static readonly BindableProperty ConfettiColorsProperty = BindableProperty.Create(
            nameof(ConfettiColors),
            typeof(Color[]),
            typeof(ParticleCanvas),
            RandomParticleGenerator.DefaultColors.Select(skColor => skColor.ToFormsColor()).ToArray(), 
            propertyChanged: (bindable, value, newValue) =>
            {
                if (bindable is ParticleCanvas me && newValue is Color[] newConfettiColors)
                {
                    me._convertedConfettiColors = newConfettiColors.Select(newColor => newColor.ToSKColor()).ToArray();
                }
            }
        );

        public Color[] ConfettiColors
        {
            get => (Color[]) GetValue(ConfettiColorsProperty);
            set => SetValue(ConfettiColorsProperty, value);
        }
    }
}