﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XICSM.ICSControlClient.Forms
{
    public partial class WaitForm : Form
    {
        public WaitForm()
        {
            InitializeComponent();
        }
        public void SetMessage(string message)
        {
            lblMain.Text = message;
            lblMain.Refresh();
        }
    }
}
