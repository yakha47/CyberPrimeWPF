using System;
using System.IO;
using System.Media;
using System.Windows;

namespace CyberPrimeWPF
{
    public class AudioService
    {
        public void PlayGreeting()
        {
            try
            {
                // Look for audio file in multiple locations
                string[] possiblePaths = {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Audio", "Voice Greeting.wav"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Voice Greeting.wav"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CyberPrime", "Audio", "Voice Greeting.wav")
                };

                foreach (string path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        using (SoundPlayer player = new SoundPlayer(path))
                        {
                            player.Play(); // Play asynchronously
                        }
                        return;
                    }
                }

                // Silently fail - no audio file found
            }
            catch (Exception)
            {
                // Silently handle audio errors
            }
        }

        public void PlayNotification()
        {
            try
            {
                System.Media.SystemSounds.Beep.Play();
            }
            catch (Exception)
            {
                // Silently handle
            }
        }
    }
}