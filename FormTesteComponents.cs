using System;
using System.Windows.Forms;

namespace AppTesteControlLibrary
{
    public partial class FormTesteComponents : Form
    {
        public FormTesteComponents()
        {
            InitializeComponent();

            closableTabBtn1.OpenFormEvent += AbrirForm;
        }

        private Form AbrirForm()
        {
            return new Form1();
        }
    }
}
