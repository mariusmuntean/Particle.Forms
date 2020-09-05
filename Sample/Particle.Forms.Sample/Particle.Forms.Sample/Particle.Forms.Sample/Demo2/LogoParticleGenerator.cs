using System;
using System.Threading.Tasks;
using Particle.Forms.ParticleGenerators;
using Particle.Forms.Particles;
using SkiaSharp;

namespace Particle.Forms.Sample.Demo2
{
    public class LogoParticleGenerator : IParticleGenerator
    {
        public ParticleBase[] Generate(SKPoint[] startPositions, int amount = 25, SKColor[] colors = null)
        {
            var particles = new ParticleBase[amount];
            var rand = new Random();
            var startPosition = startPositions[rand.Next(startPositions.Length)];

            Parallel.For(0, amount, (i) =>
            {
                var newDouble = rand.NextDouble();
                particles[i] = (newDouble) switch
                {
                    _ when newDouble > 0.66d => new MicrosoftLogoParticle(
                        new SKPoint3(
                            (float) (rand.NextDouble() * 360.0f * 1.2),
                            (float) (rand.NextDouble() * 360.0f * 1.2),
                            (float) (rand.NextDouble() * 360.0f * 1.2)
                        ),
                        (float) (100.0f + rand.NextDouble() * 200),
                        (float) (45.0f + rand.NextDouble() * 90.0f),
                        new SKPoint3(
                            (float) (rand.NextDouble() * 360.0f),
                            (float) (rand.NextDouble() * 360.0f),
                            (float) (rand.NextDouble() * 360.0f)
                        ),
                        startPosition
                        ,
                        new SKSize(
                            (float) (4 + rand.NextDouble() * 5),
                            (float) (4 + rand.NextDouble() * 5)
                        )
                    ),
                    _ when newDouble > 0.33d => new GitHubLogoParticle(
                        new SKPoint3(
                            (float) (rand.NextDouble() * 360.0f * 1.2),
                            (float) (rand.NextDouble() * 360.0f * 1.2),
                            (float) (rand.NextDouble() * 360.0f * 1.2)
                        ),
                        (float) (100.0f + rand.NextDouble() * 200),
                        (float) (45.0f + rand.NextDouble() * 90.0f),
                        new SKPoint3(
                            (float) (rand.NextDouble() * 360.0f),
                            (float) (rand.NextDouble() * 360.0f),
                            (float) (rand.NextDouble() * 360.0f)
                        ),
                        startPosition
                        ,
                        new SKSize(
                            (float) (4 + rand.NextDouble() * 5),
                            (float) (4 + rand.NextDouble() * 5)
                        )
                    ),
                    _ => new GoogleLogoParticle(
                        new SKPoint3(
                            (float) (rand.NextDouble() * 360.0f * 1.2),
                            (float) (rand.NextDouble() * 360.0f * 1.2),
                            (float) (rand.NextDouble() * 360.0f * 1.2)
                        ),
                        (float) (100.0f + rand.NextDouble() * 200),
                        (float) (45.0f + rand.NextDouble() * 90.0f),
                        new SKPoint3(
                            (float) (rand.NextDouble() * 360.0f),
                            (float) (rand.NextDouble() * 360.0f),
                            (float) (rand.NextDouble() * 360.0f)
                        ),
                        startPosition
                        ,
                        new SKSize(
                            (float) (4 + rand.NextDouble() * 5),
                            (float) (4 + rand.NextDouble() * 5)
                        )
                    )
                };
            });

            return particles;
        }
    }
}