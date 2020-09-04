using System;
using System.Threading.Tasks;
using SkiaSharp;

namespace Particle.Forms
{
    public class RandomParticleGenerator
    {
        private readonly SKColor[] _defaultColors =
        {
            SKColors.DodgerBlue,
            SKColors.CornflowerBlue,
            SKColors.PaleVioletRed,
            SKColors.LightPink,
            SKColors.Goldenrod,
            SKColors.Gold,
            SKColors.Red
        };

        public ParticleBase[] Generate(SKPoint[] startPositions, int amount = 25)
        {
            var particles = new ParticleBase[amount];
            var rand = new Random();

            Parallel.For(0, amount, (i) =>
            {
                particles[i] = GetRandomParticle(startPositions, new[] {new DirectionRange(0, 360),}, amount, rand, i);
            });

            return particles;
        }
        
        public ParticleBase[] GenerateFallingParticles(SKPoint[] startPositions, int amount = 25)
        {
            var particles = new ParticleBase[amount];
            var rand = new Random();
            Parallel.For(0, amount, (i) =>
            {
                particles[i] = GetRandomParticle(startPositions, new[] {new DirectionRange(45, 135)}, amount, rand, i);
            });

            return particles;
        }

        private RectParticle GetRandomParticle(SKPoint[] startPositions, DirectionRange[] directionRanges, int amount, Random rand, int i)
        {
            var directionRange = directionRanges[rand.Next(directionRanges.Length)];
            var direction = (float) (directionRange.MinAngle + rand.NextDouble() * (directionRange.MaxAngle - directionRange.MinAngle));

            var startPositionIndex = rand.Next(startPositions.Length);
            var startPosition = startPositions[startPositionIndex];
            
            return (rand.NextDouble() > 0.45d) switch
            {
                true => new RectParticle(
                    _defaultColors[rand.Next(0, _defaultColors.Length)],
                    new SKPoint3(
                        (float) (rand.NextDouble() * 360.0f * 1.2),
                        (float) (rand.NextDouble() * 360.0f * 1.2),
                        (float) (rand.NextDouble() * 360.0f * 1.2)
                    ),
                    (float) (50.0f + rand.NextDouble() * 200),
                    direction,
                    new SKPoint3(
                        (float) (rand.NextDouble() * 360.0f),
                        (float) (rand.NextDouble() * 360.0f),
                        (float) (rand.NextDouble() * 360.0f)
                    ),
                    startPosition,
                    new SKSize(
                        (float) (6 + rand.NextDouble() * 16),
                        (float) (6 + rand.NextDouble() * 16)
                    ),
                    (amount - i - 1) / amount * 6
                ),
                false => new EllipseParticle(
                    _defaultColors[rand.Next(0, _defaultColors.Length)],
                    new SKPoint3(
                        (float) (rand.NextDouble() * 360.0f),
                        (float) (rand.NextDouble() * 360.0f),
                        (float) (rand.NextDouble() * 360.0f)
                    ),
                    (float) (50.0f + rand.NextDouble() * 200),
                    direction,
                    new SKPoint3(
                        (float) (rand.NextDouble() * 360.0f),
                        (float) (rand.NextDouble() * 360.0f),
                        (float) (rand.NextDouble() * 360.0f)
                    ),
                    startPosition,
                    new SKSize(
                        (float) (2 + rand.NextDouble() * 12),
                        (float) (2 + rand.NextDouble() * 12)
                    ),
                    (amount - i - 1) / amount * 6
                )
            };
        }
    }
}