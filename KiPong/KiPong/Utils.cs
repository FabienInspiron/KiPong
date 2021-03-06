#define _MICROSOFT_SPEECH
#if MICROSOFT_SPEECH
using Microsoft.Speech;
using Microsoft.Speech.Synthetis;
#else
using System.Speech;
using System.Speech.Synthesis;
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Microsoft.DirectX.DirectSound;
using System.Media;

namespace KiPong
{
    public static class Utils
    {
        public static SpeechSynthesizer OldSpeech;
        public static SoundPlayer sound;

        public static void DrawRectangle(SpriteBatch sb, GraphicsDevice gd, Rectangle r, Color c)
        {
            Texture2D rectTitle = new Texture2D(gd, r.Width, r.Height);
            Color[] dataTitle = new Color[r.Width * r.Height];
            for (int i = 0; i < dataTitle.Length; ++i) dataTitle[i] = c;
            rectTitle.SetData(dataTitle);

            // dessin du fond
            sb.Draw(rectTitle, new Vector2(r.X, r.Y), Color.White);
        }

        public static void DrawStringAtCenter(SpriteBatch sb, SpriteFont font, Rectangle r, String text, Color color)
        {
            sb.DrawString(font, text, new Vector2((r.Width - font.MeasureString(text).X) / 2 + r.X, (r.Height - font.MeasureString(text).Y) / 2 + r.Y), color);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Amplitude"></param>
        /// <param name="Frequency"></param>
        /// <param name="Duration">En miliseconde</param>
        public static void Beep(int Amplitude, int Frequency, int Duration)
        {
            double A = ((Amplitude * 32768.0) / 1000.0) - 1.0;
            double DeltaFT = (2 * Math.PI * Frequency) / 44100.0;
            int Samples = (44100 * Duration) / 1000;
            Console.WriteLine(Frequency);
            int Bytes = Samples * 4;
            int[] Hdr = new int[] { 0x46464952, 0x24 + Bytes, 0x45564157, 0x20746d66, 0x10, 0x20001, 0xac44, 0x2b110, 0x100004, 0x61746164, Bytes };

            using (MemoryStream MS = new MemoryStream(0x24 + Bytes))
            {

                using (BinaryWriter BW = new BinaryWriter(MS))
                {

                    int length = Hdr.Length - 1;

                    for (int I = 0; I <= length; I++)
                    {
                        BW.Write(Hdr[I]);
                    }
                    int sample = Samples - 1;
                    for (int T = 0; T <= sample; T++)
                    {
                        short Sample = (short)Math.Round((double)(A * Math.Sin(DeltaFT * T)));
                        BW.Write(Sample);
                        BW.Write(Sample);
                    }
                    BW.Flush();
                    MS.Seek(0L, SeekOrigin.Begin);
                    using (SoundPlayer SP = new SoundPlayer(MS))
                    {
                        SP.Play();
                    }

                }

            }
        }

            // Méthode permettant de lancer la synthese vocale asynchrone
        //
        public static void SpeechAsynchroneOld(String texte)
        {
            SpeechSynthesizer s = new SpeechSynthesizer();
            PromptBuilder builder = new PromptBuilder(new System.Globalization.CultureInfo("fr-fr"));
            builder.AppendText(texte);
            s.SpeakAsync(builder);
        }

        // Méthode permettant de lancer la synthese vocale asynchrone
        //
        public static void SpeechAsynchrone(String texte)
        {
            if (OldSpeech != null) OldSpeech.Pause();

            SpeechSynthesizer s = new SpeechSynthesizer();
            PromptBuilder builder = new PromptBuilder(new System.Globalization.CultureInfo("fr-fr"));
            builder.AppendText(texte);
            s.SpeakAsync(builder);
            OldSpeech = s;
        }

        // Méthode permettant de lancer la synchronisation de maniere synchrone
        //
        public static void SpeechSynchrone(String text)
        {
            SpeechSynthesizer s = new SpeechSynthesizer();
            String voix = "ScanSoft Virginie_Dri40_16kHz";
            s.SelectVoice(voix);
            s.Speak(text);
        }

        // Arreter l'execution du speech
        public static void SpeechStop()
        {
            OldSpeech.Pause();
        }

        // Jouer le son definit par le chemin path
        public static void PlaySong(String path)
        {
            sound = new SoundPlayer(path);
            try
            {
                sound.Play();
            } 
            catch (InvalidOperationException e)
            {
                Console.Write("Erreur de lecture du song : " + e.Message);
            }
        }

        public static void StopSong()
        {
            if(sound != null) sound.Stop();
        }
    }
}
