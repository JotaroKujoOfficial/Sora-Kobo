using UnityEngine;

namespace SoraKobo.Audio
{
    /// <summary>
    /// Generates all game sound effects procedurally if no audio clips are provided.
    /// This ensures the game has audio even without external audio assets.
    /// </summary>
    public class ProceduralAudioGenerator : MonoBehaviour
    {
        void Start()
        {
            var manager = AudioManager.Instance;
            if (manager == null) return;

            // Generate piano notes if needed
            if (manager.pianoNotes == null || manager.pianoNotes.Length == 0)
                manager.GeneratePianoNotes();

            // Generate block place SFX
            if (manager.blockPlaceSFX == null || manager.blockPlaceSFX.Length == 0)
                manager.blockPlaceSFX = new AudioClip[] { GenerateBlockPlace() };

            // Generate block remove SFX
            if (manager.blockRemoveSFX == null || manager.blockRemoveSFX.Length == 0)
                manager.blockRemoveSFX = new AudioClip[] { GenerateBlockRemove() };

            // Generate interact SFX
            if (manager.interactSFX == null || manager.interactSFX.Length == 0)
                manager.interactSFX = new AudioClip[] { GenerateInteract() };

            // Generate footstep SFX
            if (manager.footstepSFX == null || manager.footstepSFX.Length == 0)
                manager.footstepSFX = GenerateFootsteps();

            // Generate jump SFX
            if (manager.jumpSFX == null)
                manager.jumpSFX = GenerateJump();

            // Generate background music
            if (manager.backgroundTracks == null || manager.backgroundTracks.Length == 0)
                manager.backgroundTracks = new AudioClip[] { GenerateBackgroundMusic() };
        }

        AudioClip GenerateBlockPlace()
        {
            int sr = 22050; float dur = 0.08f;
            int n = Mathf.RoundToInt(sr * dur);
            float[] data = new float[n];
            for (int i = 0; i < n; i++)
            {
                float t = (float)i / sr;
                float env = 1f - t / dur;
                data[i] = Mathf.Sin(2 * Mathf.PI * 800f * t) * env * 0.3f;
            }
            var c = AudioClip.Create("block_place", n, 1, sr, false);
            c.SetData(data, 0); return c;
        }

        AudioClip GenerateBlockRemove()
        {
            int sr = 22050; float dur = 0.1f;
            int n = Mathf.RoundToInt(sr * dur);
            float[] data = new float[n];
            for (int i = 0; i < n; i++)
            {
                float t = (float)i / sr;
                float freq = Mathf.Lerp(600f, 200f, t / dur);
                float env = 1f - t / dur;
                data[i] = Mathf.Sin(2 * Mathf.PI * freq * t) * env * 0.25f;
            }
            var c = AudioClip.Create("block_remove", n, 1, sr, false);
            c.SetData(data, 0); return c;
        }

        AudioClip GenerateInteract()
        {
            int sr = 22050; float dur = 0.15f;
            int n = Mathf.RoundToInt(sr * dur);
            float[] data = new float[n];
            float[] freqs = { 523f, 659f };
            for (int i = 0; i < n; i++)
            {
                float t = (float)i / sr;
                float env = Mathf.Clamp01(1f - t / dur);
                int fi = t < dur * 0.5f ? 0 : 1;
                data[i] = Mathf.Sin(2 * Mathf.PI * freqs[fi] * t) * env * 0.3f;
            }
            var c = AudioClip.Create("interact", n, 1, sr, false);
            c.SetData(data, 0); return c;
        }

        AudioClip[] GenerateFootsteps()
        {
            var clips = new AudioClip[3];
            float[] baseFreqs = { 120f, 100f, 140f };
            for (int k = 0; k < 3; k++)
            {
                int sr = 22050; float dur = 0.06f;
                int n = Mathf.RoundToInt(sr * dur);
                float[] data = new float[n];
                for (int i = 0; i < n; i++)
                {
                    float t = (float)i / sr;
                    float env = 1f - t / dur;
                    float noise = Random.Range(-1f, 1f) * 0.5f;
                    data[i] = (Mathf.Sin(2 * Mathf.PI * baseFreqs[k] * t) + noise) * env * 0.15f;
                }
                var c = AudioClip.Create("footstep_" + k, n, 1, sr, false);
                c.SetData(data, 0);
                clips[k] = c;
            }
            return clips;
        }

        AudioClip GenerateJump()
        {
            int sr = 22050; float dur = 0.2f;
            int n = Mathf.RoundToInt(sr * dur);
            float[] data = new float[n];
            for (int i = 0; i < n; i++)
            {
                float t = (float)i / sr;
                float freq = Mathf.Lerp(300f, 700f, t / dur);
                float env = 1f - t / dur;
                data[i] = Mathf.Sin(2 * Mathf.PI * freq * t) * env * 0.35f;
            }
            var c = AudioClip.Create("jump", n, 1, sr, false);
            c.SetData(data, 0); return c;
        }

        AudioClip GenerateBackgroundMusic()
        {
            int sr = 22050;
            float dur = 30f; // 30 second ambient loop
            int n = Mathf.RoundToInt(sr * dur);
            float[] data = new float[n];

            // Simple pentatonic ambient: C E G A C
            float[] melody = { 261.63f, 329.63f, 392.00f, 440.00f, 523.25f };
            float noteDur = 1.5f;
            int noteLen = Mathf.RoundToInt(sr * noteDur);

            for (int i = 0; i < n; i++)
            {
                float t = (float)i / sr;
                int noteIdx = (int)(t / noteDur) % melody.Length;
                float noteT = (t % noteDur) / noteDur;
                float env = Mathf.Clamp01(noteT < 0.1f ? noteT / 0.1f : 1f - (noteT - 0.1f) / 0.9f);

                // Soft pad tone
                float sample  = Mathf.Sin(2 * Mathf.PI * melody[noteIdx]       * t) * 0.3f;
                sample += Mathf.Sin(2 * Mathf.PI * melody[noteIdx] * 2f * t) * 0.1f;
                sample += Mathf.Sin(2 * Mathf.PI * melody[noteIdx] * 0.5f * t) * 0.15f;

                // Add gentle bass
                float bassNote = melody[noteIdx] * 0.25f;
                sample += Mathf.Sin(2 * Mathf.PI * bassNote * t) * 0.2f;

                data[i] = sample * env * 0.4f;
            }

            var c = AudioClip.Create("bgm_ambient", n, 1, sr, false);
            c.SetData(data, 0);
            return c;
        }
    }
}
