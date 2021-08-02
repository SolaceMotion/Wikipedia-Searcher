using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net.Http;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace WinFormsWikipedia
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async Task<string> CallWikipedia(string text)
        {
            string responseBody = "";
            try
            {
                string url = $"https://en.wikipedia.org/wiki/ {text}";
                using HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(url);
                responseBody = await response.Content.ReadAsStringAsync();
                
            } catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }

            return responseBody;

        }

        private async Task<long> GetDocument()
        {
            // Stopwatch object
            Stopwatch watch = new Stopwatch();

            if (textBox1.Text == "")
            {
                textBox1.PlaceholderText = "Search query cannot be empty.";
                return 0;
            }

            watch.Start();

            string document = await CallWikipedia(textBox1.Text);
            watch.Stop();
            HtmlDocument htmlDoc = new HtmlDocument();
            
            htmlDoc.LoadHtml(document);
            var content = htmlDoc.GetElementbyId("bodyContent");
            string input = content.InnerText;
            input = input.Replace("\n", "").Replace("\t", "");
            string output = "";
            string[] parts = input.Split('\n');

            foreach (var part in parts)
            {
                output += part.Trim();
            }
            richTextBox1.Text = output;

            // Return API call execution time
            return watch.ElapsedMilliseconds;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var timeElapsed = await GetDocument();
            textBox1.Text = "";

            if (timeElapsed != 0)
            {
                textBox1.PlaceholderText = $"Time elapsed: {timeElapsed}ms";
            }
            
        }

    }
}
