using System;
using System.Windows.Forms;

namespace aes256pad
{
    public partial class MainForm : Form
    {
        private const string _programVersion = "AES256PAD 1.0";
        private AES256IO _io;
        private FileStatus _fs;
        private string _lastSearchedString = String.Empty;

        #region "Enums"

        private enum SaveRequest
        {
            Save,
            SaveAs,
            ForceSave,
        }

        private enum SaveResult
        {
            None,
            Saved,
            NotChanged,
            Cancelled,
            Failed
        }
        #endregion

        #region "Constructor"

        public MainForm(string filepath = null)
        {
            InitializeComponent();


            OpenNewFile();
            if (filepath != null)
            {
                OpenFile(filepath);
            }
            UpdateStatusBar();
        }

        #endregion

        #region "Events Handling"

        private void tsNew_Click(object sender, EventArgs e)
        {
            CloseFile();
            UpdateStatusBar();
        }

        private void tsOpen_Click(object sender, EventArgs e)
        {
            OpenFile();
            UpdateStatusBar();
        }

        private void tsSave_Click(object sender, EventArgs e)
        {
            var saveResult = SaveFile(SaveRequest.Save);
            UpdateStatusBar(saveResult);
        }

        private void tsForceSave_Click(object sender, EventArgs e)
        {
            var saveResult = SaveFile(SaveRequest.ForceSave);
            UpdateStatusBar(saveResult);
        }

        private void tsSaveAs_Click(object sender, EventArgs e)
        {
            var saveResult = SaveFile(SaveRequest.SaveAs);
            UpdateStatusBar(saveResult);
        }

        private void tsExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void tsFind_Click(object sender, EventArgs e)
        {
            Find();
        }

        private void tsFindNext_Click(object sender, EventArgs e)
        {
            FindNext();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            AskSaveFile();
        }

        private void txtContent_TextChanged(object sender, EventArgs e)
        {
            _fs.IsEdited = true;
            UpdateStatusBar();
        }

        #endregion

        #region "Features Implementation"

        private void UpdateStatusBar(SaveResult fileSaved = SaveResult.None)
        {
            if (fileSaved == SaveResult.None)
            {
                if (_fs.IsNew)
                {
                    if (_fs.IsEdited)
                    {
                        ssBar.Items["tsInfo"].Text = "New file";
                    }
                    else
                    {
                        ssBar.Items["tsInfo"].Text = "Editing new file";
                    }
                }
                else
                {
                    if (_fs.IsEdited)
                    {
                        ssBar.Items["tsInfo"].Text = string.Format("Editing {0}", _fs.FilePath);
                    }
                    else
                    {
                        ssBar.Items["tsInfo"].Text = string.Format("Opened {0} @ {1} {2}", _fs.FilePath, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString());
                    }
                }
            }
            else
            {
                switch (fileSaved)
                {
                        case SaveResult.Saved:
                            ssBar.Items["tsInfo"].Text = string.Format("Saved {0} @ {1} {2}", _fs.FilePath, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString());
                            break;
                        case SaveResult.NotChanged:
                            ssBar.Items["tsInfo"].Text = string.Format("Not saved because no changes detected. Use CTRL+X to force save to disk.");
                            break;
                        case SaveResult.Cancelled:
                            ssBar.Items["tsInfo"].Text = string.Format("Not saved because cancelled by user.");
                            break;
                        case SaveResult.Failed:
                            ssBar.Items["tsInfo"].Text = string.Format("Not saved because an error occured. Check permissions on file/directory and available disk space.");
                            break;
                }
            }
            Text = String.Format("{0}{1} - {2}", _fs.IsEdited ? "*" : "",
                                          _fs.IsNew ? "New" : _fs.FilePath, _programVersion);
        }

        private void OpenFile()
        {
            // Close current file first
            CloseFile();

            string filepath = String.Empty;
            if (TryBrowseFileToOpen(false, ref filepath))
            {
                OpenFile(filepath);
            }
        }

        private void OpenFile(string filepath)
        {
            try
            {
                var pForm = new PasswordForm(true);
                if (pForm.ShowDialog() == DialogResult.OK)
                {
                    var content = _io.Read(filepath, pForm.Password);
                    txtContent.Text = content;
                    _fs.FilePath = filepath;
                    _fs.IsEdited = false;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, e.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CloseFile()
        {
            AskSaveFile();
            OpenNewFile();
        }

        private void OpenNewFile()
        {
            txtContent.Text = String.Empty;
            _lastSearchedString = String.Empty; // empty to avoid leaking "secure data" in the search box
            _io = new AES256IO();
            _fs = new FileStatus();
        }

        private void AskSaveFile()
        {
            if (!_fs.IsEdited)
            {
                return;
            }

            if (!_fs.IsNew || MessageBox.Show("Do you want to save the current file?", "Save file?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SaveFile();
            }
        }

        private SaveResult SaveFile(SaveRequest saveAction = SaveRequest.Save)
        {
            try
            {
                if (_fs.IsNew || saveAction == SaveRequest.SaveAs)
                {
                    // Ask where to save new file
                    string filepath = String.Empty;
                    if (TryBrowseFileToOpen(true, ref filepath))
                    {
                        var pForm = new PasswordForm(false);
                        if (pForm.ShowDialog() == DialogResult.OK)
                        {
                            _io.Write(filepath, txtContent.Text, pForm.Password);
                            _fs.FilePath = filepath;
                            _fs.IsEdited = false;
                            return SaveResult.Saved;
                        }
                    }
                    return SaveResult.Cancelled;
                }
                
                // Save existing file
                if (_io.Write(txtContent.Text, saveAction == SaveRequest.ForceSave))
                {
                    _fs.IsEdited = false;
                    return SaveResult.Saved;
                }
                return SaveResult.NotChanged;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, e.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return SaveResult.Failed;
        }

        private void Find()
        {
            var fForm = new FindForm(_lastSearchedString);
            if (fForm.ShowDialog() == DialogResult.OK)
            {
                _lastSearchedString = fForm.SearchString;
                FindNext();
            }
        }

        private void FindNext()
        {
            if (string.IsNullOrEmpty(_lastSearchedString))
            {
                Find();
            }
            else
            {
                try
                {
                    if (txtContent.CanFocus)
                    {
                        txtContent.Focus();
                        var startIndex = txtContent.SelectionStart + txtContent.SelectionLength;
                        var content = txtContent.Text;
                        var findIndex = content.IndexOf(_lastSearchedString, startIndex, StringComparison.InvariantCultureIgnoreCase);
                        if (findIndex < 0)
                        {
                            findIndex = content.IndexOf(_lastSearchedString, StringComparison.InvariantCultureIgnoreCase);
                        }
                        if (findIndex < 0)
                        {
                            MessageBox.Show(string.Format("string '{0}' not found", _lastSearchedString), "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            txtContent.SelectionStart = findIndex;
                            txtContent.ScrollToCaret();
                            txtContent.SelectionLength = _lastSearchedString.Length;
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, e.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool TryBrowseFileToOpen(bool saveDialog, ref string filepath)
        {
            FileDialog ofd;
            if (saveDialog)
            {
                ofd = new SaveFileDialog();
            }
            else
            {
                ofd = new OpenFileDialog()
                {
                    Multiselect = false
                };
            }
            ofd.CheckFileExists = false;
            ofd.CheckPathExists = true;
            ofd.Filter = @"AES 256 Encrypted File|*.aes256;*.*";
            ofd.AddExtension = true;
            ofd.FileName = filepath;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filepath = ofd.FileName;
                return true;
            }
            filepath = null;
            return false;
        }

        #endregion

    }
}
