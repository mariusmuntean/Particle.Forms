using System;

namespace Particle.Forms.ParticleRequester
{
    public class RateBasedParticleRequester
    {
        private double _accumulatedParticlesToRequest = 0.0f;

        public int AmountToRequest(float desiredParticlesPerSecond, uint animationRateMillis)
        {
            var particlesToRequest = desiredParticlesPerSecond * (animationRateMillis * 0.001f);

            _accumulatedParticlesToRequest += particlesToRequest;

            if (_accumulatedParticlesToRequest >= 1.0f)
            {
                var integerPart = Math.Floor(_accumulatedParticlesToRequest);

                _accumulatedParticlesToRequest = Math.Max(_accumulatedParticlesToRequest - integerPart, 0);

                return (int) integerPart;
            }
            else
            {
                return 0;
            }
        }

        public void Reset()
        {
            _accumulatedParticlesToRequest = 0;
        }
    }
}