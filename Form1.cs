using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace layout_gen
{
    public partial class Form1 : Form
    {
       // string filepath = @"F:\workspace\study\layout_gen\activity_main.xml";

        BaseParser parser = null;

        string globalFilePath = "";
        string outputDir = "";
        public Form1()
        {
            InitializeComponent();
        }
         
        void browser() {
            DialogResult dr = this.openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                globalFilePath = this.openFileDialog1.FileName;
                if (globalFilePath.Contains("layout"))
                {
                    string temp = globalFilePath.Replace("res\\layout", "res\\values");
                    int startindex = 0;
                    startindex = temp.LastIndexOf(getFilename(globalFilePath));
                    temp = temp.Substring(0, startindex);
                    this.txtSaveFileDir.Text = temp;
                }
                this.txtFilePath.Text = globalFilePath;
                updateToVariables();
            }
        }
        void genFile()
        {
            parser = new LayoutParser(globalFilePath);
            parser.Parse();
            parser.setOutputPath(outputDir);
            parser.Save();
            MessageBox.Show("�㶨!");
        }
        string getFilename(string filepath) {
            string result = "";
            int startindex = filepath.LastIndexOf(@"\") + 1;
            result = filepath.Substring(startindex, filepath.Length - startindex);
            startindex = result.LastIndexOf(@".");
            result = result.Substring(0, startindex);
            return result;
        }
        private void selectDir()
        {
            DialogResult dr = this.folderBrowserDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                txtSaveFileDir.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Multiselect = true;
            openFileDialog1.Filter = "�����ļ�(*.xml)|*.xml";

            string dir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            txtSaveFileDir.Text = dir;
            updateToVariables();
        }

        void updateToVariables()
        {
           outputDir =  txtSaveFileDir.Text;
        }

        private void btn_browser_Click(object sender, EventArgs e)
        {
            browser();
        }

        private void btn_dir_Click(object sender, EventArgs e)
        {
            selectDir();
        }

       
        private void btn_save_Click(object sender, EventArgs e)
        {
            genFile();
        }
    }
}