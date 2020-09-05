using Particle.Forms.Particles;
using SkiaSharp;

namespace Particle.Forms.ParticleGenerators
{
    public interface IParticleGenerator
    {
        ParticleBase[] Generate(SKPoint[] startPositions, int amount = 25, SKColor[] colors = null);
    }
}