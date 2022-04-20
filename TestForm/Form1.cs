using System;
using System.Globalization;
using System.Windows.Forms;
using ioCode.Serialization;

namespace TestForm
{
    public partial class Form1 : Form
    {
        private SampleObject m_sampleObject = new SampleObject();

        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            propertyGrid1.SelectedObject = m_sampleObject;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = m_sampleObject = new SampleObject();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog()
            {
                Title = "Load File",
                Filter = "Xml Files (*.xml)|*.xml|Bin Files (*.bin)|*.bin|All Files (*.*)|*.*",
                Multiselect = false
            })
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    SampleObject sampleObject;
                    string sExtension = System.IO.Path.GetExtension(dlg.FileName).ToLower(CultureInfo.InvariantCulture);
                    if (sExtension == ".bin")
                    {
                        sampleObject = BinSerializer<SampleObject>.ReadFile(dlg.FileName);
                    }
                    else // same as xml
                    {
                        sampleObject = XmlSerializer<SampleObject>.ReadFile(dlg.FileName);
                    }

                    if (sampleObject == null)
                    {
                        MessageBox.Show(this
                            , "The file could not be loaded."
                            , "Load Error"
                            , MessageBoxButtons.OK
                            , MessageBoxIcon.Error
                            );

                        return;
                    }

                    propertyGrid1.SelectedObject = m_sampleObject = sampleObject;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog()
            {
                Title = "Save to file",
                Filter = "Xml Files (*.xml)|*.xml|Bin Files (*.bin)|*.bin|All Files (*.*)|*.*",
            })
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    bool isSuccess;
                    string sExtension = System.IO.Path.GetExtension(dlg.FileName).ToLower(CultureInfo.InvariantCulture);
                    if (sExtension == ".bin")
                    {
                        isSuccess = BinSerializer<SampleObject>.WriteFile(dlg.FileName, m_sampleObject);
                    }
                    else // same as xml
                    {
                        isSuccess = XmlSerializer<SampleObject>.WriteFile(dlg.FileName, m_sampleObject);
                    }

                    if (!isSuccess)
                    {
                        MessageBox.Show(this
                            , "The file could not be saved."
                            , "Save Error"
                            , MessageBoxButtons.OK
                            , MessageBoxIcon.Error
                            );
                    }
                }
            }
        }
    }
}
