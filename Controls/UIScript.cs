using System;
using System.Drawing;
using System.Windows.Forms;

namespace Controls
{
    public partial class UIScript : Form
    {
        public UIScript()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Up trzcionka
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtSizeUp_Click(object sender, EventArgs e)
        {
            editor.Font = new Font(editor.Font.Name, editor.Font.Size + 4);
        }

        /// <summary>
        /// Down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtSizeDown_Click(object sender, EventArgs e)
        {
            if(editor.Font.Size > 5)
                editor.Font = new Font(editor.Font.Name, editor.Font.Size - 4);
        }
    }
}
