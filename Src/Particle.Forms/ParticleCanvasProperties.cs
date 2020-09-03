using Xamarin.Forms;

namespace Particle.Forms
{
    public partial class ParticleCanvas
    {
        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == IsActiveProperty.PropertyName && IsActive)
            {
                Start();
            }
            else if (propertyName == AddParticlesOnTapProperty.PropertyName
                     || propertyName == AddParticlesOnDragProperty.PropertyName)
            {
                if (AddParticlesOnTap || AddParticlesOnDrag)
                {
                    // Touch -= OnTouch;
                    _removeTouchHandler(OnTouch);
                    EnableTouchEvents = true;
                    // Touch += OnTouch;
                    _addTouchHandler(OnTouch);
                }
                else
                {
                    // Touch -= OnTouch;
                    _removeTouchHandler(OnTouch);
                    EnableTouchEvents = false;
                }
            }
        }

        public static readonly BindableProperty IsActiveProperty = BindableProperty.Create(
            nameof(IsActive),
            typeof(bool),
            typeof(ParticleCanvas),
            false
        );

        /// <summary>
        /// Whether or not the particles should be displayed.
        /// </summary>
        public bool IsActive
        {
            get => (bool) GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        public static readonly BindableProperty HasFallingParticlesProperty = BindableProperty.Create(
            nameof(FallingParticlesPerSecond),
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
    }
}