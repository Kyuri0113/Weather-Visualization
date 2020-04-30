using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Rain : MonoBehaviour
{
    public Camera Camera;

    public AudioClip RainAudioLight;
    public AudioClip RainAudioMedium;
    public AudioClip RainAudioHeavy;
    public AudioClip ThunderstormAudio;
    public AudioMixerGroup RainAudioMixer;

    [Range(0.0f, 1.0f)]
    public float RainIntensity = 0.0f;

    public ParticleSystem RainFallParticleSystem;
    public ParticleSystem RainExplosionParticleSystem;
    public ParticleSystem RainMistParticleSystem;

    [Range(0.0f, 1.0f)]
    public float RainMistThreshold = 0.5f;

    protected LoopingAudioSource audioSourceRainLight;
    protected LoopingAudioSource audioSourceRainMedium;
    protected LoopingAudioSource audioSourceRainHeavy;
    protected LoopingAudioSource audioSourceThunderstorm;
    protected LoopingAudioSource audioSourceRainCurrent;

    protected Material rainMaterial;
    protected Material rainExplosionMaterial;
    protected Material rainMistMaterial;

    private float lastRainIntensityValue = -1.0f;

    public float RainHeight = 25.0f;
    public float RainForwardOffset = -7.0f;
    public float RainMistHeight = 3.0f;

    private void CheckForRainChange()
    {
        if (lastRainIntensityValue != RainIntensity) {
            lastRainIntensityValue = RainIntensity;
            if (RainIntensity == 0.0f) {
                if (audioSourceRainCurrent != null) {
                    audioSourceRainCurrent.Stop();
                    audioSourceRainCurrent = null;
                }
                    
                if (RainFallParticleSystem != null) {
                    ParticleSystem.EmissionModule e = RainFallParticleSystem.emission;
                    e.enabled = false;
                    RainFallParticleSystem.Stop();
                }
        
                if (RainMistParticleSystem != null) {
                    ParticleSystem.EmissionModule e = RainMistParticleSystem.emission;
                    e.enabled = false;
                    RainMistParticleSystem.Stop();
                }
            } else {
                LoopingAudioSource newSource;
                
                if (RainIntensity == 1.0f) {
                    newSource = audioSourceThunderstorm;
                } else if (RainIntensity >= 0.67f) {
                    newSource = audioSourceRainHeavy;
                } else if (RainIntensity >= 0.33f) {
                    newSource = audioSourceRainMedium;
                } else {
                    newSource = audioSourceRainLight;
                }
                
                if (audioSourceRainCurrent != newSource) {
                    if (audioSourceRainCurrent != null) {
                        audioSourceRainCurrent.Stop();
                    }
                    audioSourceRainCurrent = newSource;
                    audioSourceRainCurrent.Play(1.0f);
                }

                if (RainFallParticleSystem != null) {
                    ParticleSystem.EmissionModule e = RainFallParticleSystem.emission;
                    e.enabled = RainFallParticleSystem.GetComponent<Renderer>().enabled = true;
                    if (!RainFallParticleSystem.isPlaying) {
                        RainFallParticleSystem.Play();
                    }
                    ParticleSystem.MinMaxCurve rate = e.rateOverTime;
                    rate.mode = ParticleSystemCurveMode.Constant;
                    rate.constantMin = rate.constantMax = RainFallEmissionRate();
                    e.rateOverTime = rate;
                }

                if (RainMistParticleSystem != null) {
                    ParticleSystem.EmissionModule e = RainMistParticleSystem.emission;
                    e.enabled = RainMistParticleSystem.GetComponent<Renderer>().enabled = true;
                    if (!RainMistParticleSystem.isPlaying) {
                        RainMistParticleSystem.Play();
                    }
                    float emissionRate;
                    if (RainIntensity < RainMistThreshold) {
                        emissionRate = 0.0f;
                    } else {
                        emissionRate = MistEmissionRate();
                    }
                    ParticleSystem.MinMaxCurve rate = e.rateOverTime;
                    rate.mode = ParticleSystemCurveMode.Constant;
                    rate.constantMin = rate.constantMax = emissionRate;
                    e.rateOverTime = rate;
                }
            }
        }
    }

    private void UpdateRain()
    {
        if (RainFallParticleSystem != null) {
            var s = RainFallParticleSystem.shape;
            s.shapeType = ParticleSystemShapeType.ConeVolume;
            RainFallParticleSystem.transform.position = Camera.transform.position;
            RainFallParticleSystem.transform.Translate(0.0f, RainHeight, RainForwardOffset);
            RainFallParticleSystem.transform.rotation = Quaternion.Euler(0.0f, Camera.transform.rotation.eulerAngles.y, 0.0f);
            if (RainMistParticleSystem != null) {
                var s2 = RainMistParticleSystem.shape;
                s2.shapeType = ParticleSystemShapeType.Hemisphere;
                Vector3 pos = Camera.transform.position;
                pos.y += RainMistHeight;
                RainMistParticleSystem.transform.position = pos;
            }
        }
    }

    protected virtual void Start() {
        if (Camera == null) {
            Camera = Camera.main;
        }
        audioSourceRainLight = new LoopingAudioSource(this, RainAudioLight, RainAudioMixer);
        audioSourceRainMedium = new LoopingAudioSource(this, RainAudioMedium, RainAudioMixer);
        audioSourceRainHeavy = new LoopingAudioSource(this, RainAudioHeavy, RainAudioMixer);
        audioSourceThunderstorm = new LoopingAudioSource(this, ThunderstormAudio, RainAudioMixer);

        if (RainFallParticleSystem != null) {
            ParticleSystem.EmissionModule e = RainFallParticleSystem.emission;
            e.enabled = false;
            Renderer rainRenderer = RainFallParticleSystem.GetComponent<Renderer>();
            rainRenderer.enabled = false;
            rainMaterial = new Material(rainRenderer.material);
            rainMaterial.EnableKeyword("SOFTPARTICLES_OFF");
            rainRenderer.material = rainMaterial;
        }

        if (RainExplosionParticleSystem != null) {
            ParticleSystem.EmissionModule e = RainExplosionParticleSystem.emission;
            e.enabled = false;
            Renderer rainRenderer = RainExplosionParticleSystem.GetComponent<Renderer>();
            rainExplosionMaterial = new Material(rainRenderer.material);
            rainExplosionMaterial.EnableKeyword("SOFTPARTICLES_OFF");
            rainRenderer.material = rainExplosionMaterial;
        }

        if (RainMistParticleSystem != null) {
            ParticleSystem.EmissionModule e = RainMistParticleSystem.emission;
            e.enabled = false;
            Renderer rainRenderer = RainMistParticleSystem.GetComponent<Renderer>();
            rainRenderer.enabled = false;
            rainMistMaterial = new Material(rainRenderer.material);
            if (UseRainMistSoftParticles) {
                rainMistMaterial.EnableKeyword("SOFTPARTICLES_ON");
            } else {
                rainMistMaterial.EnableKeyword("SOFTPARTICLES_OFF");
            }
            rainRenderer.material = rainMistMaterial;
        }
    }

    protected virtual void Update()
    {
        CheckForRainChange();
        audioSourceRainLight.Update();
        audioSourceRainMedium.Update();
        audioSourceRainHeavy.Update();
        audioSourceThunderstorm.Update();
        UpdateRain();
    }

    protected virtual float RainFallEmissionRate()
    {
        return (RainFallParticleSystem.main.maxParticles / RainFallParticleSystem.main.startLifetime.constant) * RainIntensity * 10;
    }

    protected virtual float MistEmissionRate()
    {
        return (RainMistParticleSystem.main.maxParticles / RainMistParticleSystem.main.startLifetime.constant) * RainIntensity * RainIntensity;
    }

    protected virtual bool UseRainMistSoftParticles
    {
        get
        {
            return true;
        }
    }
}

public class LoopingAudioSource
{
    public AudioSource AudioSource { get; private set; }
    public float TargetVolume { get; private set; }

    public LoopingAudioSource(MonoBehaviour script, AudioClip clip, AudioMixerGroup mixer)
    {
        AudioSource = script.gameObject.AddComponent<AudioSource>();

        if (mixer != null) {
            AudioSource.outputAudioMixerGroup = mixer;
        }

        AudioSource.loop = true;
        AudioSource.clip = clip;
        AudioSource.playOnAwake = false;
        AudioSource.volume = 0.0f;
        AudioSource.Stop();
        TargetVolume = 1.0f;
    }

    public void Play(float targetVolume)
    {
        if (!AudioSource.isPlaying) {
            AudioSource.volume = 0.0f;
            AudioSource.Play();
        }
        TargetVolume = targetVolume;
    }

    public void Stop()
    {
        TargetVolume = 0.0f;
    }

    public void Update()
    {
        if (AudioSource.isPlaying && (AudioSource.volume = Mathf.Lerp(AudioSource.volume, TargetVolume, Time.deltaTime)) == 0.0f) {
            AudioSource.Stop();
        }
    }
}
