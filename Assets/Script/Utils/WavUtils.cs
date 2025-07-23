using System;
using System.IO;
using System.Text;
using UnityEngine;

public class WavUtils
{
    static public AudioClip TrimClip(AudioClip originalClip, int positionSamples)
    {
        if (positionSamples <= 0 || positionSamples > originalClip.samples)
        {
            Debug.LogWarning("Invalid positionSamples value for trimming.");
            return originalClip;
        }

        int channels = originalClip.channels;
        float[] fullData = new float[originalClip.samples * channels];
        originalClip.GetData(fullData, 0);

        float[] trimmedData = new float[positionSamples * channels];
        Array.Copy(fullData, trimmedData, trimmedData.Length);

        AudioClip trimmedClip = AudioClip.Create("TrimmedClip", positionSamples, channels, originalClip.frequency, false);
        trimmedClip.SetData(trimmedData, 0);

        return trimmedClip;
    }

    static public byte[] ConvertClipToWav(AudioClip clip)
    {
        using (var stream = new MemoryStream())
        {
            float[] samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);

            int byteCount = samples.Length * 2; // 16-bit PCM = 2 bytes/sample
            WriteWavHeader(stream, clip.channels, clip.frequency, byteCount);

            byte[] byteBuffer = new byte[byteCount];
            int offset = 0;
            for (int i = 0; i < samples.Length; i++)
            {
                short s = (short)Mathf.Clamp(samples[i] * short.MaxValue, short.MinValue, short.MaxValue);
                byteBuffer[offset++] = (byte)(s & 0xFF);
                byteBuffer[offset++] = (byte)((s >> 8) & 0xFF);
            }

            stream.Write(byteBuffer, 0, byteBuffer.Length);
            return stream.ToArray();
        }
    }

    static private void WriteWavHeader(Stream stream, int channels, int sampleRate, int byteCount)
    {
        int blockAlign = channels * 2;
        int byteRate = sampleRate * blockAlign;

        using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, true))
        {
            writer.Write(Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(36 + byteCount);
            writer.Write(Encoding.ASCII.GetBytes("WAVE"));
            writer.Write(Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16);
            writer.Write((short)1); // PCM
            writer.Write((short)channels);
            writer.Write(sampleRate);
            writer.Write(byteRate);
            writer.Write((short)blockAlign);
            writer.Write((short)16); // Bits per sample
            writer.Write(Encoding.ASCII.GetBytes("data"));
            writer.Write(byteCount);
        }
    }
}
