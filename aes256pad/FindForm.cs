using System;
using System.Windows.Forms;

namespace aes256pad
{
    public partial class FindForm : Form
    {
        public string SearchString { get; private set; }

        public FindForm(string searchString = "")
        {
            InitializeComponent();

            SearchString = searchString;
            txtSearchString.Text = searchString;
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            SearchString = txtSearchString.Text;
            Close();
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SearchString = String.Empty;
            Close();
        }
    }
}
