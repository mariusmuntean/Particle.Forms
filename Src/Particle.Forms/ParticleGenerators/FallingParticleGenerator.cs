using System;
using Particle.Forms.Particles;
using SkiaSharp;

namespace Particle.Forms.ParticleGenerators
{
    public class FallingParticleGenerator : IParticleGenerator
    {
        private readonly RandomParticleGenerator _randomParticleGenerator = new RandomParticleGenerator();
        private readonly Random _rand;

        public FallingParticleGenerator()
        {
            _rand = new Random();
        }

        public ParticleBase[] Generate(SKPoint[] startPositions, int amount = 25, SKColor[] colors = null)
        {
            var particles = new ParticleBase[amount];
            for (int i = 0; i < amount; i++)
            {
                particles[i] = _randomParticleGenerator.GetRandomParticle(startPositions,
                    new[]
                    {
                        new DirectionRange(45, 135)
                    }, amount, _rand, i, colors ?? RandomParticleGenerator.DefaultColors);
            }

            return particles;
        }
    }
}