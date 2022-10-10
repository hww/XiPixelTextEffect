
using NaughtyAttributes;
using System.Collections;

using UnityEngine;
using XiCore.UnityExtensions;
using XiSound;
using XiSound.Events;

namespace XiPixelTextEffect
{
    public class PixelText : MonoBehaviour
    {
        const float INFINITIVE_LIVE_TIME = 100f;
        public enum EAnmiation { None, AnimateToVisible, AnimateToInvisible, MakeVisible, MakeInvisible }

        [System.Serializable]
        public struct AnimationSetings
        {
            public float duration;
            public float timeOffset;
            public Gradient colorCurve;
            public Vector2 timeOffsetByXY;
            public AudioEvent sound;
        }

        [System.Serializable]
        public struct TileState
        {
            public Vector3 position;
            public float rotation;
            public Vector3 axisOfRotation;

            public static TileState Lerp(TileState a, TileState b, float k)
            {
                return new TileState() {
                    position = Vector3.Lerp(a.position, b.position, k),
                    rotation = Mathf.Lerp(a.rotation, b.rotation, k),
                    axisOfRotation = Vector3.Lerp(a.axisOfRotation, b.axisOfRotation, k)
                };
            }
        }

        [Header("ParticleText")]
        public new ParticleSystem particleSystem;
        public PixelText slave;

        [Header("The sungle tile dimentions")]
        public Vector3 tileSize = new Vector3(1f, 1f, 0);
        public float particleSize = 0.95f;

        [Header("Random Position")]
        public float randomRotation = 360f;
        public float randomMagnitude = 2f;    // Direction from center of text to the hiddenTile magnifyed by thos
        public float randomRadius = 3f;       // Inside this radius
        public Vector3 fixedOffset;

        [Header("Animation")]
        public AnimationSetings showAnimation;
        public AnimationSetings hideAnimation;

        [Header("Animation State")]
        [ReadOnly] public EAnmiation animationState;
        [ReadOnly] public Gradient animationColorCurve;
        [ReadOnly] public float animationDuration;
        [ReadOnly] public float animationTime;
        [ReadOnly] public float animationStartTime;
        [ReadOnly] public float animationSpeed;
        [ReadOnly] public bool animationReversed;
        [ReadOnly] public Vector2 animationTimeOffsetByXY;
        [ReadOnly] public AudioEvent animationSound;

        private string text = "PixelText";
        private int totalTilesNumber;
        private TileState[] visibleState;
        private TileState[] hidenState;
        private float[] timeShift;
        private ParticleSystem.Particle[] particles;
        private PixelTextBuilder virtualScreen;

        private bool doRebuildText;

        public bool IsAnimating => animationState != EAnmiation.None;

        private void OnValidate()
        {
            doRebuildText = true;
        }

        private void OnEnable()
        {
            animationTime = 0;
            particleSystem.Clear();
            doRebuildText = true;
        }

        public void SetText(string str)
        {
            text = str;
            doRebuildText = true;
            slave?.SetText(str);
        }

        public void Animate(EAnmiation animation, float speed = 1)
        {
            enabled = true;
            animationState = animation;
            switch (animationState)
            {
                case EAnmiation.None:
                    break;
                case EAnmiation.AnimateToVisible:
                    SetAnimationState_Internal(showAnimation, speed, showAnimation.timeOffset);
                    if (animationSound != null) SoundSystem.Play(animationSound);
                    break;
                case EAnmiation.AnimateToInvisible:
                    SetAnimationState_Internal(hideAnimation, speed, hideAnimation.timeOffset, true);
                    if (animationSound != null) SoundSystem.Play(animationSound);
                    break;
                case EAnmiation.MakeVisible:
                    SetAnimationState_Internal(showAnimation, speed, -showAnimation.duration);
                    break;
                case EAnmiation.MakeInvisible:
                    SetAnimationState_Internal(hideAnimation, speed, -hideAnimation.duration, true);
                    break;
            }
            slave?.Animate(animation, speed);
        }

        private void SetAnimationState_Internal(AnimationSetings animation, float speed = 1, float timeOffset = 0, bool reversed = false)
        {
            animationSpeed = speed;
            animationDuration = animation.duration;
            animationColorCurve = animation.colorCurve;
            animationStartTime = Time.time + timeOffset;
            animationTimeOffsetByXY = animation.timeOffsetByXY;
            animationSound = animation.sound;
            animationReversed = reversed;
            animationTime = Time.time - animationStartTime;
        }

        [Button()]
        public void Show()
        { 
            Animate(EAnmiation.AnimateToVisible);
        }

        [Button()]
        public void Hide()
        {
            Animate(EAnmiation.AnimateToInvisible);
        }

        /// <summary>
        ///     Render particles with given animation 
        /// </summary>
        /// <param name="time"></param>
        private void Update()
        {
            if (doRebuildText)
                RebuildText();
            
            if (animationState == EAnmiation.None)
                return;

            animationTime = Time.time - animationStartTime;
            int numParticlesAlive = particleSystem.GetParticles(particles);
            particleSystem.Emit(totalTilesNumber - numParticlesAlive);
            numParticlesAlive = particleSystem.GetParticles(particles);

            int particlesAnimated = 0;

            for (var i = 0; i < totalTilesNumber; i++)
            {
                var nTime = Mathf.Clamp01((animationTime - timeShift[i]) / animationDuration);
                if (animationReversed) nTime = 1.0f-nTime;

                if (nTime == 0)
                {
                    particles[i].startSize = 0;
                    particles[i].remainingLifetime = INFINITIVE_LIVE_TIME;
                }
                else 
                {
                    if (nTime < 1.0) particlesAnimated++;
                    TileState state = TileState.Lerp(hidenState[i], visibleState[i], nTime);
                    particles[i].position = state.position;
                    particles[i].rotation = state.rotation;
                    particles[i].axisOfRotation = state.axisOfRotation;
                    particles[i].startSize = particleSize;
                    particles[i].startColor = animationColorCurve.Evaluate(nTime);
                    particles[i].remainingLifetime = INFINITIVE_LIVE_TIME;
                }
            }
            // For unused particles
            for (var i = totalTilesNumber; i < numParticlesAlive; i++)
            {
                particles[i].remainingLifetime = 0f;
                particles[i].startSize = 0;
            }

            particleSystem.SetParticles(particles);

            if (Time.time >= animationStartTime && particlesAnimated == 0)
            {
                animationState = EAnmiation.None;
                enabled = false;
            }
        }




        // Rebuild text message
        private void RebuildText()
        {
            doRebuildText = false;

            // Generate particles screen
            virtualScreen = new PixelTextBuilder(text.Length * 16);
            virtualScreen.PrintStringCentered(text, transform.position, tileSize);

            totalTilesNumber = virtualScreen.TilesCount;

            // Create final (visible) position
            if (visibleState == null || visibleState.Length < totalTilesNumber)
                visibleState = new TileState[totalTilesNumber];
            if (hidenState == null || hidenState.Length < totalTilesNumber)
                hidenState = new TileState[totalTilesNumber];

            // Create final (timeshift) position
            timeShift = new float[totalTilesNumber];

            var txtSize = virtualScreen.Bounds.size;

            for (var i = 0; i < totalTilesNumber; i++)
            {
                var worldPos = virtualScreen.tilePositions[i];
                var localPos = worldPos - virtualScreen.Bounds.min;

                // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

                TileState visibleTile;
                visibleTile.position = worldPos;
                visibleTile.rotation = 0;
                visibleTile.axisOfRotation = transform.forward;
                visibleState[i] = visibleTile;

                // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

                TileState hiddenTile;
                // 1 - Time shift per pizel   
                timeShift[i] = ((float)localPos.x / (float)txtSize.x) * animationTimeOffsetByXY.x +
                               ((float)localPos.y / (float)txtSize.y) * animationTimeOffsetByXY.y;

                // 2 - Hidden position
                hiddenTile.position = (worldPos * randomMagnitude)
                              + (Random.insideUnitSphere * randomRadius).WithZ(0)
                              + fixedOffset;
                // 3 - Hiddlen rotation
                hiddenTile.rotation = Random.Range(0f, randomRotation);                               
                hiddenTile.axisOfRotation = Random.onUnitSphere;
                
                hidenState[i] = hiddenTile;
            }
            // Check if the particles supports this amount of elements
            if (particleSystem.main.maxParticles < totalTilesNumber)
            {
                var main = particleSystem.main;
                main.maxParticles = totalTilesNumber;
            }

            // Prepair buffer for particles
            if (particles == null || particles.Length < totalTilesNumber)
                particles = new ParticleSystem.Particle[totalTilesNumber];
        }

        private void OnDrawGizmosSelected()
        {
            if (hidenState != null && visibleState != null)
            {
                for (var i=0; i<totalTilesNumber; i++)
                    Gizmos.DrawLine(hidenState[i].position, visibleState[i].position);
            }

        }
    }
}
