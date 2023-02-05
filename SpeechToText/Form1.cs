using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio;
using NAudio.Wave;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Speech.Synthesis;
using System.Threading;
using System.Diagnostics;
using System.Security;

namespace SpeechToText
{
    

    public partial class Form1 : Form
    {
        WaveIn waveIn;
        WaveFileWriter writer;
        string outputFilename = "demo.wav";
        bool ON = false;
        int speechSpeed;
        public Form1()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ON == false)
            {
                waveIn = new WaveIn();
                waveIn.DeviceNumber = 0;
                waveIn.DataAvailable += waveIn_DataAvailable;
                waveIn.RecordingStopped += new EventHandler<NAudio.Wave.StoppedEventArgs>(waveIn_RecordingStopped);
                waveIn.WaveFormat = new WaveFormat(16000, 1);
                writer = new WaveFileWriter(outputFilename, waveIn.WaveFormat);
                label2.Text = "Идет запись...";
                button1.Text = "Стоп";
                waveIn.StartRecording();
                ON = true;
                
            }
            else
            {
                waveIn.StopRecording();
                label2.Text = "";
                ON = false;
                button1.Text = "Запись";
            }
        }
        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            writer.WriteData(e.Buffer, 0, e.BytesRecorded);
        }


        void waveIn_RecordingStopped(object sender, EventArgs e)
        {
            waveIn.Dispose();
            waveIn = null;
            writer.Close();
            writer = null;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            WebRequest request = WebRequest.Create("https://www.google.com/speech-api/v2/recognize?output=json&lang=ru-RU&key=AIzaSyBOti4mM-6x9WDnZIjIeyEU21OpBXqWBgw");
            //
            request.Method = "POST";
            byte[] byteArray = File.ReadAllBytes(outputFilename);
            request.ContentType = "audio/l16; rate=16000"; //"16000";
            request.ContentLength = byteArray.Length;
            request.GetRequestStream().Write(byteArray, 0, byteArray.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            

            string strtrs = reader.ReadToEnd();
            var rg = new Regex(@"transcript" + '"' + ":" + '"' + "([A-Z, А-Я, a-z,а-я, ,0-9]*)");
            var result = rg.Match(strtrs).Groups[1].Value;
            label3.Text = result;
            
            reader.Close();
            response.Close();
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Для начала нажмите кнопку Записи, затем кнопку Распознать, для получения произнесенного текста");
        }
    }
}
