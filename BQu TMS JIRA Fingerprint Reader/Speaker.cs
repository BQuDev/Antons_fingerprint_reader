using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Synthesis;
using System.Windows;

namespace BQu_TMS_JIRA_Fingerprint_Reader
{
    public class Speaker
    {
        SpeechSynthesizer reader; //declare the object 
        public Speaker()
        {
            reader = new SpeechSynthesizer(); //create new object 
        }

        public void speachThis(string text)
        {
            reader.Dispose();
            reader = new SpeechSynthesizer();
            reader.SelectVoiceByHints(VoiceGender.Male);
            reader.SpeakAsync(text);
            reader.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(reader_SpeakCompleted); 
        }

        //event handler 
        void reader_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            //speak completed
        } 

        public void stopSpeaking()
        {
            if (reader != null)
            {
                reader.Dispose(); 
            }
        }

        //PAUSE 
        public void pauseSpeak()
        {
            if (reader != null)
            {
                if (reader.State == SynthesizerState.Speaking)
                {
                    reader.Pause();
                }
            }
        }

        //RESUME 
        public void ResumeSpeak()
        {
            if (reader != null)
            {
                if (reader.State == SynthesizerState.Paused)
                {
                    reader.Resume();
                }
            }
        } 
    }


}
