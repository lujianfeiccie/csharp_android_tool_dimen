using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace layout_gen
{
    public partial class FormAboutDlg : Form
    {
        public FormAboutDlg()
        {
            InitializeComponent();
        }

        private void FormAboutDlg_Load(object sender, EventArgs e)
        {
            this.Text = "关于";
            this.label1.Text = "作者：陆键霏 (C)无敌软件工作室 2013";
        }
    }
}
