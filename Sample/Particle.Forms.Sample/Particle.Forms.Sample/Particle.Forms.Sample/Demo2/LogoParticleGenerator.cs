using System;
using Particle.Forms.ParticleGenerators;
using Particle.Forms.Particles;
using SkiaSharp;

namespace Particle.Forms.Sample.Demo2
{
    public class LogoParticleGenerator : IParticleGenerator
    {
        private readonly Random _rand;

        public LogoParticleGenerator()
        {
            _rand = new Random();
        }

        public ParticleBase[] Generate(SKPoint[] startPositions, int amount = 25, SKColor[] colors = null)
        {
            var particles = new ParticleBase[amount];
            var startPosition = startPositions[_rand.Next(startPositions.Length)];

            for (int i = 0; i < amount; i++)
            {
                var newDouble = _rand.NextDouble();
                particles[i] = (newDouble) switch
                {
                    _ when newDouble > 0.66d => new MicrosoftLogoParticle(
                        new SKPoint3(
                            (float)(_rand.NextDouble() * 360.0f * 1.2),
                            (float)(_rand.NextDouble() * 360.0f * 1.2),
                            (float)(_rand.NextDouble() * 360.0f * 1.2)
                        ),
                        (float)(100.0f + _rand.NextDouble() * 200),
                        (float)(45.0f + _rand.NextDouble() * 90.0f),
                        new SKPoint3(
                            (float)(_rand.NextDouble() * 360.0f),
                            (float)(_rand.NextDouble() * 360.0f),
                            (float)(_rand.NextDouble() * 360.0f)
                        ),
                        startPosition
                        ,
                        new SKSize(
                            (float)(4 + _rand.NextDouble() * 5),
                            (float)(4 + _rand.NextDouble() * 5)
                        )
                    ),
                    _ when newDouble > 0.33d => new GitHubLogoParticle(
                        new SKPoint3(
                            (float)(_rand.NextDouble() * 360.0f * 1.2),
                            (float)(_rand.NextDouble() * 360.0f * 1.2),
                            (float)(_rand.NextDouble() * 360.0f * 1.2)
                        ),
                        (float)(100.0f + _rand.NextDouble() * 200),
                        (float)(45.0f + _rand.NextDouble() * 90.0f),
                        new SKPoint3(
                            (float)(_rand.NextDouble() * 360.0f),
                            (float)(_rand.NextDouble() * 360.0f),
                            (float)(_rand.NextDouble() * 360.0f)
                        ),
                        startPosition
                        ,
                        new SKSize(
                            (float)(4 + _rand.NextDouble() * 5),
                            (float)(4 + _rand.NextDouble() * 5)
                        )
                    ),
                    _ => new GoogleLogoParticle(
                        new SKPoint3(
                            (float)(_rand.NextDouble() * 360.0f * 1.2),
                            (float)(_rand.NextDouble() * 360.0f * 1.2),
                            (float)(_rand.NextDouble() * 360.0f * 1.2)
                        ),
                        (float)(100.0f + _rand.NextDouble() * 200),
                        (float)(45.0f + _rand.NextDouble() * 90.0f),
                        new SKPoint3(
                            (float)(_rand.NextDouble() * 360.0f),
                            (float)(_rand.NextDouble() * 360.0f),
                            (float)(_rand.NextDouble() * 360.0f)
                        ),
                        startPosition
                        ,
                        new SKSize(
                            (float)(4 + _rand.NextDouble() * 5),
                            (float)(4 + _rand.NextDouble() * 5)
                        )
                    )
                };
            };

            return particles;
        }
    }
}