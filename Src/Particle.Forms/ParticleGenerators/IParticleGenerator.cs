using Particle.Forms.Particles;
using SkiaSharp;

namespace Particle.Forms.ParticleGenerators
{
    /// <summary>
    /// Common contract for all classes that generate instances of (subclasses of) <see cref="ParticleBase"/>
    /// </summary>
    public interface IParticleGenerator
    {
        /// <summary>
        /// Generates the requested amount of <see cref="ParticleBase"/> instances , randomly distributed at the specified positions.
        /// <para>Each <see cref="ParticleBase"/> is randomly assigned one of the provided colors.
        /// It is up to the specific <see cref="ParticleBase"/> subclass to decide how to use that color.</para>
        /// </summary>
        /// <param name="startPositions">The positions where the particles should be created</param>
        /// <param name="amount">The amount of particles to generate</param>
        /// <param name="colors">The colors to be assigned</param>
        /// <returns></returns>
        ParticleBase[] Generate(SKPoint[] startPositions, int amount = 25, SKColor[] colors = null);
    }
}