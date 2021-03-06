using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace ConnectLab
{
    public partial class frmPassword : Form
    {
        public frmPassword()
        {
            InitializeComponent();
            
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
          
            if (!AppUntil.GetMD5(txtPass.Text).Equals(AppParams.serverConfig.AppPass))
            {
                lblNotice.Text = "Sai mật khẩu!";
                txtPass.SelectionStart = 0;
                txtPass.SelectionLength = txtPass.Text.Length;              
                return;
            }
            frmConfig frm = new frmConfig();
            frm.Show();
            this.Close();
        }
        
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}