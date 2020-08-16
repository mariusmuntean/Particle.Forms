using System;
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


            for (var i = 0; i < amount; i++)
            {
                particles[i] = GetRandomParticle(startPositions, amount, rand, i);
            }

            return particles;
        }

        private RectParticle GetRandomParticle(SKPoint[] startPositions, int amount, Random rand, int i)
        {
            return (rand.NextDouble() > 0.5d) switch
            {
                true => new RectParticle(
                    _defaultColors[rand.Next(0, _defaultColors.Length)],
                    new SKPoint3(
                        (float) (rand.NextDouble() * 360.0f * 2.0f),
                        (float) (rand.NextDouble() * 360.0f * 2.0f),
                        (float) (rand.NextDouble() * 360.0f * 2.0f)
                    ),
                    (float) (50.0f + rand.NextDouble() * 200),
                    (float) (rand.NextDouble() * 360),
                    new SKPoint3(
                        (float) (rand.NextDouble() * 360.0f),
                        (float) (rand.NextDouble() * 360.0f),
                        (float) (rand.NextDouble() * 360.0f)
                    ),
                    startPositions[rand.Next(startPositions.Length)],
                    new SKSize(
                        (float) (2 + rand.NextDouble() * 20),
                        (float) (2 + rand.NextDouble() * 20)
                    ),
                    (amount - i - 1) / amount * 6
                ),
                false => new EllipseParticle(
                    _defaultColors[rand.Next(0, _defaultColors.Length)],
                    new SKPoint3(
                        (float) (rand.NextDouble() * 360.0f * 2.0f),
                        (float) (rand.NextDouble() * 360.0f * 2.0f),
                        (float) (rand.NextDouble() * 360.0f * 2.0f)
                    ),
                    (float) (50.0f + rand.NextDouble() * 200),
                    (float) (rand.NextDouble() * 360),
                    new SKPoint3(
                        (float) (rand.NextDouble() * 360.0f),
                        (float) (rand.NextDouble() * 360.0f),
                        (float) (rand.NextDouble() * 360.0f)
                    ),
                    startPositions[rand.Next(startPositions.Length)],
                    new SKSize(
                        (float) (2 + rand.NextDouble() * 20),
                        (float) (2 + rand.NextDouble() * 20)
                    ),
                    (amount - i - 1) / amount * 6
                )
            };
        }
    }
}