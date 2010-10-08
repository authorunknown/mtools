using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using href.Utils;
using System.Threading;
using System.IO;

namespace EncodingTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("en-us");
            InitializeComponent();
        }

 
        
        private void button1_Click(object sender, EventArgs e)
        {
            this.listView1.Items.Clear();
            Encoding encForStream = EncodingTools.GetMostEfficientEncodingForStream(testText.Text);
            Encoding encForMime = EncodingTools.GetMostEfficientEncoding(testText.Text);


            ListViewItem encItem = null;
            /*
            
            encItem = GetListItemForEncoding(encForMime);
            encItem.ImageKey = "text_ok";
            this.listView1.Items.Add(encItem);
           */
            
            
            


            Encoding[] encodings = EncodingTools.DetectOutgoingEncodings(testText.Text, EncodingTools.AllEncodings, true);
            foreach (Encoding encoding in encodings)
            {

                encItem = GetListItemForEncoding(encoding);

                if (encoding == encForStream)
                    encItem.StateImageIndex = 2;
                
                if (encoding == encForMime)
                    encItem.ImageKey = "text_ok";
                    

                
                this.listView1.Items.Add(encItem);
            }
            Encoding encodingDefault = EncodingTools.GetMostEfficientEncoding(testText.Text);

            
        }

        private ListViewItem GetListItemForEncoding(Encoding encoding)
        {
            byte[] encodedBytes = encoding.GetBytes(this.testText.Text);

            /* test if all bytes are kept through encoding
            byte[] unicodeBytes = Encoding.Unicode.GetBytes(this.testText.Text);

            
            string decoded = encoding.GetString(encodedBytes);
            byte[] unicodeAfterEncodingByte = Encoding.Unicode.GetBytes(decoded);



            int errors = (int)System.Math.Abs(unicodeBytes.Length - unicodeAfterEncodingByte.Length);
            for(int i=0;i<System.Math.Min(unicodeBytes.Length, unicodeAfterEncodingByte.Length);i++)
            {
                if (unicodeBytes[i] != unicodeAfterEncodingByte[i])
                    errors++;
            }
            */

            ListViewItem encItem = new ListViewItem(encoding.EncodingName);
            encItem.SubItems.Add(encoding.CodePage.ToString());
            encItem.SubItems.Add(encoding.BodyName);
            encItem.SubItems.Add(encodedBytes.Length.ToString());
            encItem.Tag = encoding;
          //  encItem.SubItems.Add(errors.ToString());
            return encItem;
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count != 1)
                return;

            EncodingTestForm testForm = new EncodingTestForm(this.listView1.SelectedItems[0].Tag as Encoding, this.testText.Text);
            testForm.ShowDialog(this);
        }


        /// <summary>
        /// Save the given text to a stream
        /// </summary>
        /// <param name="text">text to save</param>
        /// <param name="path">path to the file</param>
        private void SaveToStream(string text, string path)
        {
            // this is all... detect the encoding
            Encoding enc = EncodingTools.GetMostEfficientEncodingForStream(text);
            // then safe
            using (StreamWriter sw = new StreamWriter(path, false, enc))
                sw.Write(text);
        }

        /// <summary>
        /// Creeates a dummy email
        /// </summary>
        /// <param name="text">body text of the email</param>
        /// <param name="path">path to the new email (should have the extension .eml)</param>
        private void SaveToAsEmail(string text, string path)
        {
            // this is all... detect the encoding
            Encoding enc = EncodingTools.GetMostEfficientEncoding(text);
            // then safe
            using (StreamWriter sw = new StreamWriter(path, false, Encoding.ASCII))
            {
                sw.WriteLine("Subject: test");
                sw.WriteLine("Transfer-Encoding: 7bit");
                sw.WriteLine("Content-Type: text/plain;\r\n\tcharset=\"{0}\"", enc.BodyName);
                sw.WriteLine("Content-Transfer-Encoding: base64"); // should be QP
                sw.WriteLine();
                sw.Write(Convert.ToBase64String(enc.GetBytes(text),Base64FormattingOptions.InsertLineBreaks));
            }
        }


        private void OpenTextFileTest()
        {
            // read the complete file into a string
            string content = EncodingTools.ReadTextFile(@"C:\test\txt");

            // create a StreamReader with the guessed best encoding
            using (StreamReader sr = EncodingTools.OpenTextFile(@"C:\test\txt"))
            {
                string fileContent = sr.ReadToEnd();
            }

            // create a streamReader from a stream
            using (MemoryStream ms = new MemoryStream(
                Encoding.GetEncoding("windows-1252").GetBytes("Some umlauts: öäüß")))
            {
                using (StreamReader sr = EncodingTools.OpenTextStream(ms))
                {
                    string fileContent = sr.ReadToEnd();
                }
            }
        }
    }
}
