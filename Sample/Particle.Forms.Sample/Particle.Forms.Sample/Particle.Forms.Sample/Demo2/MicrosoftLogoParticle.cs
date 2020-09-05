using Particle.Forms.Particles;
using SkiaSharp;

namespace Particle.Forms.Sample.Demo2
{
    public class MicrosoftLogoParticle : ParticleBase
    {
        private SKPaint _background = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColor.Parse("#f3f3f3")
        };

        private SKPaint _r = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColor.Parse("#f35325")
        };

        private SKPaint _g = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColor.Parse("#81bc06")
        };

        private SKPaint _b = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColor.Parse("#05a6f0")
        };

        private SKPaint _y = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColor.Parse("#ffba08")
        };

        public MicrosoftLogoParticle(SKPoint3 rotationSpeed, float translationSpeed, float direction, SKPoint3 orientation, SKPoint position, SKSize size) : base(rotationSpeed, translationSpeed, direction, orientation, position, size)
        {
        }

        protected override void Draw(SKCanvas canvas)
        {
            var transformationMatrix = SKMatrix.CreateTranslation(Position.X - 11.5f, Position.Y - 11.5f);
            var scale = 6.0f / Size.Width;
            transformationMatrix = transformationMatrix.PostConcat(SKMatrix.CreateScale(scale, scale, Position.X, Position.Y));

            var backgroundPath = SKPath.ParseSvgPathData("M0 0h23v23H0z");
            backgroundPath.Transform(transformationMatrix);

            var redSquarePath = SKPath.ParseSvgPathData("M1 1h10v10H1z");
            redSquarePath.Transform(transformationMatrix);
            
            var greenSquarePath = SKPath.ParseSvgPathData("M12 1h10v10H12z");
            greenSquarePath.Transform(transformationMatrix);
            
            var blueSquarePath = SKPath.ParseSvgPathData("M1 12h10v10H1z");
            blueSquarePath.Transform(transformationMatrix);
            
            var yellowSquarePath = SKPath.ParseSvgPathData("M12 12h10v10H12z");
            yellowSquarePath.Transform(transformationMatrix);

            canvas.DrawPath(backgroundPath, _background);
            canvas.DrawPath(redSquarePath, _r);
            canvas.DrawPath(greenSquarePath, _g);
            canvas.DrawPath(blueSquarePath, _b);
            canvas.DrawPath(yellowSquarePath, _y);
        }
    }
}