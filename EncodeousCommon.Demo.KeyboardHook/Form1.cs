using EncodeousCommon.Input.Keyboard.KeyboardHook;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EncodeousCommon.Demo.KeyboardHook
{
    public partial class Form1 : Form
    {
        KeyboardHookWrapper KHW = new KeyboardHookWrapper();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            KHW.KeyDown += OnKeyDown;
        }

        public void OnKeyDown(KeyboardArgs e)
        {
            e.Handled = true;
        }
    }
}
