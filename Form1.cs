using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Threading;

namespace layout_gen
{
    public partial class Form1 : Form
    {
        delegate void myinvoke(string obj);
        delegate void myinvoke_bool(bool obj);
        BaseParser parser = null;

        string globalFilePath = "";
        string outputDir = "";
        System.Windows.Forms.ListView.ListViewItemCollection list_before, list_after;

        class backup_param
        {
            public string[] filenames;
            public string backup_path;
        }
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
                updateToControls();
            }
        }
        void genFile()
        {
            if (txtFilePath.Text.Length<=0 || openFileDialog1.FileNames.Length <= 0)
            {
                MessageBox.Show("����ѡ���ļ�");
                return;
            }
            if (txtSaveFileDir.Text.Length <= 0)
            {
                MessageBox.Show("����ѡ�񱣴�·��");
                return;
            }
            updateToVariables();

            backup_param temp_backup_param = new backup_param();
             temp_backup_param.filenames = this.openFileDialog1.FileNames;

            Thread thread = new Thread(new ParameterizedThreadStart(genFile_thread));
            thread.Start(temp_backup_param);
        }
        void genFile_thread(object obj)
        {
            backup_param temp_backup_param = (backup_param)obj;
            parser = new LayoutParser();

            this.BeginInvoke(new myinvoke(updateStatus), new object[] { "���ڴ���..." });
            this.Invoke(new myinvoke_bool(enableBtnGen), new object[] { false });
            foreach (string filename in this.openFileDialog1.FileNames)
            {
                parser.setFilePath(filename);
                parser.Parse();
                this.Invoke(new myinvoke(removeListBefore), new object[] { getFilename(filename) + ".xml" });
                this.Invoke(new myinvoke(addListAfter), new object[] { getFilename(filename) + ".xml" });
            }
            parser.setOutputPath(outputDir);
            parser.Save();
            parser = null;
            this.Invoke(new myinvoke_bool(enableBtnGen), new object[] { true });
            this.Invoke(new myinvoke(updateStatus), new object[] { "�������" });
            this.Invoke(new myinvoke(showMessageBox), new object[] { "�������" });
        }
     
        void removeListBefore(object obj) {
            string key = (string)obj;
            list_before.RemoveByKey(key);
        }
        void addListAfter(object obj)
        {
            string key = (string)obj;
            list_after.Add(key,key,0);
        }
        void enableBtnGen(bool enable) {
            btn_save.Enabled = enable;
        }
        void backup() {
            if (openFileDialog1.FileNames.Length <= 0)
            {
                showMessageBox("��ѡ���ļ�!");
                return;
            }
            string select_dir = "";
            DialogResult dr = this.folderBrowserDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
               // this.BeginInvoke(new myinvoke(updateStatus));
                select_dir = folderBrowserDialog1.SelectedPath;
                string backup_path = select_dir + "\\backup";
                string[] filenames = this.openFileDialog1.FileNames;
               
                backup_param temp_backup_param = new backup_param();
                temp_backup_param.filenames = filenames;
                temp_backup_param.backup_path = backup_path;

                Thread thread = new Thread(new ParameterizedThreadStart(backup_thread));
                thread.Start(temp_backup_param);
         
            }
           
        }
      
        void backup_thread(object obj)
        {
            this.BeginInvoke(new myinvoke(updateStatus),new object[]{"���ڱ���..."});
          

            backup_param temp_backup_param = (backup_param)obj;
            string[] filenames = temp_backup_param.filenames;
            string backup_path = temp_backup_param.backup_path;

            if (!Directory.Exists(backup_path))
            {
                Directory.CreateDirectory(backup_path);
            }
            foreach (string filename in this.openFileDialog1.FileNames)
            {
                File.Copy(filename, backup_path + "\\" + getFilename(filename) + ".xml", true);
            }
            this.Invoke(new myinvoke(updateStatus), new object[] { "�������" });
            this.Invoke(new myinvoke(showMessageBox), new object[] { "�������" });
        }
        void updateStatus(string msg)
        {
            toolStripStatusLabel1.Text = msg;
        }
        void showMessageBox(string msg)
        {
            MessageBox.Show(msg);
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
            localizeText();
            initListView(listView_before);
            initListView(listView_after);
            list_before = new ListView.ListViewItemCollection(listView_before);
            list_after = new ListView.ListViewItemCollection(listView_after);

            this.ContextMenuStrip = contextMenuStrip1;
            openFileDialog1.FileName = "";
            openFileDialog1.Multiselect = true;
            openFileDialog1.Filter = "�����ļ�(*.xml)|*.xml";

            string dir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            txtSaveFileDir.Text = dir;
            updateToVariables();
        }

        private void initListView(ListView listview)
        {
            listview.Columns.Add("�ļ�", 200);
            listview.Scrollable = true;
            listview.GridLines = true; //��ʾ�����
            listview.View = View.Details;//��ʾ���ϸ��
        }

        private void localizeText()
        {
            this.Text = "dimen.xml�ļ�������";
            this.label1.Text = "�����ļ�";
            this.label2.Text = "�����ļ���";
            this.btn_browser.Text = "���";
            this.btn_dir.Text = "ѡ���ļ���";
            this.btn_save.Text = "��ʼ����";
            this.toolStripStatusLabel1.Text = "����";
        
            this.groupBox1.Text = "�������ļ�";
            this.groupBox2.Text = "�Ѵ����ļ�";
        }

        void updateToVariables()
        {
           outputDir =  txtSaveFileDir.Text;
        }

        void updateToControls()
        {
            list_before.Clear();
            list_after.Clear();
            foreach (string filename in this.openFileDialog1.FileNames)
            {
               string temp_filename = getFilename(filename);
               temp_filename = temp_filename+".xml";
               list_before.Add(temp_filename.Trim(), temp_filename.Trim(), 0);     
            }
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

        private void bt_backup_Click(object sender, EventArgs e)
        {
            backup();
        }

        private void ��������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAboutDlg dlg = new FormAboutDlg();
            dlg.ShowDialog();
        }
      
    }
}