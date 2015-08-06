using System;
using System.Windows.Forms;

namespace aes256pad
{
    public partial class PasswordForm : Form
    {
        public string Password { get; private set; }

        public PasswordForm(bool decrypt)
        {
            InitializeComponent();

            if (decrypt)
            {
                lblFirst.Text = "Decryption password:";
                lblSecond.Visible = false;
                txtPassword2.Visible = false;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var password1 = txtPassword1.Text;
            var password2 = txtPassword2.Text;
            if (txtPassword2.Visible && !password1.Equals(password2))
            {
                MessageBox.Show("Passwords do not match!", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            if (password1.Length == 0)
            {
                MessageBox.Show("Password can't be empty!", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            Password = password1;
            Close();
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Password = null;
            Close();
        }
    }
}
