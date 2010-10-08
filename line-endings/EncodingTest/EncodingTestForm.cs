using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using href.Utils;

namespace EncodingTest
{
    public partial class EncodingTestForm : Form
    {
        private Encoding m_Encoding;
        private string m_TestText;

        public EncodingTestForm(Encoding enc, string testText)
        {
            InitializeComponent();
            this.m_Encoding = enc;
            this.m_TestText = testText;
            this.DoTest();
        }

        private void DoTest()
        {
         

            if ((this.m_TestText == null) || (this.m_TestText.Length == 0))
                return;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                byte[] encoded = this.m_Encoding.GetBytes(this.m_TestText);
                // preamble?
                byte[] preamble = this.m_Encoding.GetPreamble();

                // Make sure a preamble was returned 
                // and is large enough to containa BOM.
                if (preamble.Length >= 2)
                {
                    ms.Write(preamble, 0, preamble.Length);
                }

                ms.Write(encoded, 0, encoded.Length);

                ms.Position = 0;
                // read it using standard text reader
                System.IO.StreamReader tr = new System.IO.StreamReader(ms, true);
                

                this.streamReader.Text = tr.ReadToEnd();
                this.label1.Text = String.Format("StreamReader: {0} / {1}", tr.CurrentEncoding.EncodingName, tr.CurrentEncoding.BodyName);

                // now the improved test
                ms.Position = 0;
                Encoding targetEncoding;
                byte[] rawData = ms.ToArray();
                try
                {
                    targetEncoding = EncodingTools.DetectInputCodepage(rawData);
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    targetEncoding = Encoding.Default;
                }
                this.detected.Text = targetEncoding.GetString(rawData);
                this.label2.Text = String.Format("EncodingTools.DetectInputCodepage: {0} / {1}", targetEncoding.EncodingName, targetEncoding.BodyName);
            }

        }


    }
}