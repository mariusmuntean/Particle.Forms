using System;
using System.Threading.Tasks;
using Particle.Forms.Particles;
using SkiaSharp;

namespace Particle.Forms.ParticleGenerators
{
    public class FallingParticleGenerator : IParticleGenerator
    {
        private readonly RandomParticleGenerator _randomParticleGenerator = new RandomParticleGenerator();

        public ParticleBase[] Generate(SKPoint[] startPositions, int amount = 25, SKColor[] colors = null)
        {
            var particles = new ParticleBase[amount];
            var rand = new Random();
            Parallel.For(0, amount, (i) =>
            {
                particles[i] = _randomParticleGenerator.GetRandomParticle(startPositions,
                    new[]
                    {
                        new DirectionRange(45, 135)
                    }, amount, rand, i, colors ?? RandomParticleGenerator.DefaultColors);
            });

            return particles;
        }
    }
}