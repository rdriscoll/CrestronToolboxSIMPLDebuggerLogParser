// License info and recommendations
//-----------------------------------------------------------------------
// <copyright file="Form1.cs" company="AVPlus Integration Pty Ltd">
//     {c} AV Plus Pty Ltd 2017.
//     http://www.avplus.net.au
//     20170611 Rod Driscoll
//     e: rdriscoll@avplus.net.au
//     m: +61 428 969 608
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//     
//     The above copyright notice and this permission notice shall be included in
//     all copies or substantial portions of the Software.
//     
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//     THE SOFTWARE.
//
//     For more details please refer to the LICENSE file located in the root folder 
//      of the project source code;
// </copyright>

namespace AVPlus.ToolboxLogParser
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;    
    
    public partial class Form1 : Form
    {
        private string filteredFile = String.Empty;
        private int idx = 0;
        private string str;
        private string[] rows;
        private FileDialog ofd = new OpenFileDialog();
        private List<CrestronDebuggerLine> lines = new List<CrestronDebuggerLine>();
        private List<string> sigs = new List<string>();
        private int prevWidth;
        private int prevHeight;

        public Form1()
        {
            InitializeComponent();
            prevWidth = this.Width;
            prevHeight = this.Height;
            cbTime.Checked = false;
            cbDate.Checked = false;
            cbSigName.Checked = true;
        }

        private void printFile()
        {
            StringBuilder sb = new StringBuilder();
            foreach (CrestronDebuggerLine l in lines)
            {
                if (checkedListBox1.GetItemCheckState(checkedListBox1.Items.IndexOf(l.Name)) == CheckState.Checked)
                {
                    if (cbTime.Checked)
                        sb.Append(l.Time + " - ");
                    if (cbDate.Checked)
                        sb.Append(l.Date);
                    if (cbTime.Checked || cbDate.Checked)
                        sb.Append(": ");
                    if (cbSigName.Checked)
                        sb.Append(l.Name + " -> ");
                    sb.Append(l.Data + "\n");
                }
            }
            sb.Replace(@"\x0D", "\x0D");
            sb.Replace(@"\x0A", "\x0A");
            filteredFile = sb.ToString();
            richTextBox1.Text = filteredFile;
            //lines = filteredFile.Split(new string[] { "\x0D\x0A" }, StringSplitOptions.None);
            rows = Regex.Split(filteredFile, "\x0D\x0A");
        }

        private void parseFile(string filename)
        {
            this.Text = filename;
            using (StreamReader reader = new StreamReader(filename))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Match m = Regex.Match(line, "(.*) - (.*): (.*) -> (.*)"); //"12:55:25 - 01-16-2015: MCU-01_Fusion_tx$ -> "
                    if (m.Groups[3].Value.Length > 0)
                    {
                        lines.Add(new CrestronDebuggerLine(m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value, m.Groups[4].Value));
                        if (!sigs.Contains(m.Groups[3].Value))
                        {
                            sigs.Add(m.Groups[3].Value);
                            checkedListBox1.Items.Add(m.Groups[3].Value, true);
                        }
                    }
                }
            }
            lines.Sort();
            printFile();
        }

        private void parseCcfFile(string filename)
        {
            this.Text = filename;
            Dictionary<string, string> IRData = new Dictionary<string, string>();
            using (StreamReader reader = new StreamReader(filename))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Match m = Regex.Match(line, @"^\[(.*)\]\s*(.*)$"); //[NAME]0000 0000\n\r"
                    if (m.Success)
                    {
                        IRData.Add(m.Groups[1].Value,m.Groups[2].Value);
                    }
                }
            }
            printFile();
        }

        private void ResizeControl(Control sender, int incHeight)
        {
            sender.SetBounds(sender.Location.X, sender.Location.Y + incHeight, sender.Width, sender.Height);
        }
        private void ResizeControl(Control sender, int incHeight, int incWidth)
        {
            sender.SetBounds(sender.Location.X, sender.Location.Y + incHeight, sender.Width + incWidth, sender.Height);
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            int incWidth = this.Width - prevWidth;
            int incHeight = this.Height - prevHeight;
            richTextBox1.SetBounds(richTextBox1.Location.X, richTextBox1.Location.Y, richTextBox1.Width + incWidth, richTextBox1.Height + incHeight);
            ResizeControl(checkedListBox1, incHeight, incWidth);
            ResizeControl(tbFilter, incHeight, incWidth);
            ResizeControl(cbSigName, incHeight);
            ResizeControl(cbTime, incHeight);
            ResizeControl(cbDate, incHeight);
            ResizeControl(btnSelectAll, incHeight);
            ResizeControl(btnClearAll, incHeight);
            ResizeControl(btnFilterShow, incHeight);
            ResizeControl(btnFilterHide, incHeight);
            ResizeControl(label1, incHeight);
            ResizeControl(label2, incHeight);
            prevWidth = this.Width;
            prevHeight = this.Height;
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                parseFile(ofd.FileName);
            }
        }

        private void cbDate_CheckedChanged(object sender, EventArgs e)
        {
            printFile();
        }

        private void cbTime_CheckedChanged(object sender, EventArgs e)
        {
            printFile();
        }

        private void cbSigName_CheckedChanged(object sender, EventArgs e)
        {
            printFile();
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            printFile();
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, true);
            }
            printFile();
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, false);
            }
            printFile();
        }

        private void btnFilterShow_Click(object sender, EventArgs e)
        {
             //Regex pattern = new Regex(".*" + + ".*");
             //bool isMatch = pattern.IsMatch("battle");


            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                Match m = Regex.Match(sigs[i], tbFilter.Text);
                //if()
                checkedListBox1.SetItemChecked(i, false);
            }
            printFile();
        }

    }
    public class CrestronDebuggerLine : IEquatable<CrestronDebuggerLine>, IComparable<CrestronDebuggerLine>
    {
        public string Name { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Data { get; set; }

        public CrestronDebuggerLine(string Time, string Date, string Name, string Data)
        {
            this.Name = Name;
            this.Date = Date;
            this.Time = Time;
            this.Data = Data;
        }
        public override string ToString()
        {
            return String.Format("{0} - {1}: {2} -> {3}", Time, Date, Name, Data); //"12:55:25 - 01-16-2015: MCU-01_Fusion_tx$ -> "
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            CrestronDebuggerLine objAsLine = obj as CrestronDebuggerLine;
            if (objAsLine == null) return false;
            else return Equals(objAsLine);
        }
        public int SortByNameAscending(string name1, string name2)
        {
            return name1.CompareTo(name2);
        }
        public int CompareTo(CrestronDebuggerLine compareLine)
        {
            if (compareLine == null)
                return 1;

            else
                return this.Name.CompareTo(compareLine.Name);
        }
        //public override int GetHashCode()
        //{
        //    return 0;
        //}
        public bool Equals(CrestronDebuggerLine other)
        {
            if (other == null) return false;
            return (this.Name.Equals(other.Name));
        }
        // Should also override == and != operators.
    }

}
