using UnityEngine;

namespace SoraKobo.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Music")]
        public AudioSource musicSource;
        public AudioClip[] backgroundTracks;

        [Header("SFX")]
        public AudioSource sfxSource;
        public AudioClip[] blockPlaceSFX;
        public AudioClip[] blockRemoveSFX;
        public AudioClip[] interactSFX;
        public AudioClip[] footstepSFX;
        public AudioClip   jumpSFX;

        [Header("Piano Notes")]
        public AudioClip[] pianoNotes; // C4..C5 (8 notes)

        [Header("Settings")]
        [Range(0f, 1f)] public float musicVolume = 0.4f;
        [Range(0f, 1f)] public float sfxVolume   = 0.8f;

        void Awake()
        {
            Instance = this;
            if (musicSource != null) musicSource.volume = musicVolume;
            if (sfxSource   != null) sfxSource.volume   = sfxVolume;
        }

        void Start() => PlayNextTrack();

        void Update()
        {
            // Guard: backgroundTracks may be null or empty until ProceduralAudioGenerator runs
            if (backgroundTracks == null || backgroundTracks.Length == 0) return;
            if (musicSource != null && !musicSource.isPlaying)
                PlayNextTrack();
        }

        // ── Music ─────────────────────────────────────────────────────────

        private int _trackIndex = 0;

        void PlayNextTrack()
        {
            if (backgroundTracks == null || backgroundTracks.Length == 0) return;
            _trackIndex = (_trackIndex + 1) % backgroundTracks.Length;
            PlayMusic(backgroundTracks[_trackIndex]);
        }

        public void PlayMusic(AudioClip clip)
        {
            if (musicSource == null || clip == null) return;
            musicSource.clip = clip;
            musicSource.Play();
        }

        public void SetMusicVolume(float v)
        {
            musicVolume = v;
            if (musicSource != null) musicSource.volume = v;
        }

        // ── SFX ───────────────────────────────────────────────────────────

        public void PlayBlockPlace()  => PlayRandom(blockPlaceSFX);
        public void PlayBlockRemove() => PlayRandom(blockRemoveSFX);
        public void PlayInteract()    => PlayRandom(interactSFX);
        public void PlayFootstep()    => PlayRandom(footstepSFX);

        public void PlayJump()
        {
            if (sfxSource != null && jumpSFX != null)
                sfxSource.PlayOneShot(jumpSFX, sfxVolume);
        }

        public void PlayNote(int index)
        {
            if (pianoNotes == null || index < 0 || index >= pianoNotes.Length) return;
            sfxSource?.PlayOneShot(pianoNotes[index], sfxVolume);
        }

        void PlayRandom(AudioClip[] clips)
        {
            if (sfxSource == null || clips == null || clips.Length == 0) return;
            var clip = clips[Random.Range(0, clips.Length)];
            if (clip != null) sfxSource.PlayOneShot(clip, sfxVolume);
        }

        public void SetSFXVolume(float v)
        {
            sfxVolume = v;
            if (sfxSource != null) sfxSource.volume = v;
        }

        // ── Procedural tone generation ────────────────────────────────────

        public AudioClip GenerateTone(float frequency, float duration = 0.3f)
        {
            int sr      = 44100;
            int samples = Mathf.RoundToInt(sr * duration);
            var clip    = AudioClip.Create("tone", samples, 1, sr, false);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t   = (float)i / sr;
                float env = Mathf.Clamp01(1f - t / duration);
                data[i]   = Mathf.Sin(2 * Mathf.PI * frequency * t) * env * 0.5f;
            }
            clip.SetData(data, 0);
            return clip;
        }

        public void GeneratePianoNotes()
        {
            float[] freqs = { 261.63f, 293.66f, 329.63f, 349.23f,
                              392.00f, 440.00f, 493.88f, 523.25f };
            pianoNotes = new AudioClip[freqs.Length];
            for (int i = 0; i < freqs.Length; i++)
                pianoNotes[i] = GenerateTone(freqs[i]);
        }
    }
}
