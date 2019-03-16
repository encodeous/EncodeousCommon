using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EncodeousCommon.Miscellaneous.ModalDialog;

namespace EncodeousCommon.Demo.ModalDialog
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://encodeous.github.io/");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int backgroundBrightness = -50;
            DialogForm modalDialogForm = new DialogForm();
            if (textBox1.Text != "")
            {
                modalDialogForm.label1.Text = textBox1.Text;
            }
            DialogCreator modalDialogCreator = new DialogCreator("DesktopName",modalDialogForm, backgroundBrightness);
            modalDialogCreator.CreateDialog();
        }
    }
}
