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
    public partial class frmChanePassword : Form
    {
        public frmChanePassword()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string oldPass=AppParams.serverConfig.AppPass;
           
            if (!AppUntil.GetMD5(txtOldPass.Text).Equals(oldPass))
            {
                txtOldPass.SelectionStart = 0;
                txtOldPass.SelectionLength = txtOldPass.Text.Length;    
                lblNotice.Text = "Mật khẩu cũ không đúng!";
                return;
            }
            if (txtNewPass.Text == "")
            {
                lblNotice.Text = "Vui lòng nhập mật khẩu mới!";
                return;
            }
            if (!txtConfirmPass.Text.Equals(txtNewPass.Text))
            {
                txtConfirmPass.SelectionStart = 0;
                txtConfirmPass.SelectionLength = txtConfirmPass.Text.Length;    
                lblNotice.Text = "Xác lại mật khẩu mới không đúng!";
                return;
            }
            AppParams.serverConfig.AppPass = AppUntil.GetMD5(txtNewPass.Text);
            if (AppParams.serverConfig.Write() == false)
            {
                lblNotice.Text = "Đổi mật khẩu không thành công!";
                AppParams.serverConfig.AppPass = oldPass;
                return;
            }
            this.Close();
        }
        
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

      
    }
}