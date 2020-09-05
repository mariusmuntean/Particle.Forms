using SkiaSharp;

namespace Particle.Forms.Particles
{
    public class EllipseParticle : RectParticle
    {
        public EllipseParticle(SKColor color,
            SKPoint3 rotationSpeed,
            float translationSpeed,
            float direction,
            SKPoint3 orientation,
            SKPoint position,
            SKSize size,
            float blurFactor) : base(color, rotationSpeed, translationSpeed, direction, orientation, position, size, blurFactor)
        {
        }

        protected override void Draw(SKCanvas canvas)
        {
            canvas.DrawOval(Position.X, Position.Y, Size.Width / 2, Size.Height / 2, Paint);
        }
    }
}