namespace Controls
{
    partial class UIScript
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.editor = new System.Windows.Forms.RichTextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.BtSizeUp = new System.Windows.Forms.ToolStripButton();
            this.BtSizeDown = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // editor
            // 
            this.editor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editor.Location = new System.Drawing.Point(0, 25);
            this.editor.Name = "editor";
            this.editor.Size = new System.Drawing.Size(385, 355);
            this.editor.TabIndex = 0;
            this.editor.Text = "";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BtSizeUp,
            this.BtSizeDown});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(385, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // BtSizeUp
            // 
            this.BtSizeUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BtSizeUp.Image = global::Controls.Zasoby.Alarm_Arrow_Up_icon;
            this.BtSizeUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BtSizeUp.Name = "BtSizeUp";
            this.BtSizeUp.Size = new System.Drawing.Size(23, 22);
            this.BtSizeUp.Text = "toolStripButton1";
            this.BtSizeUp.Click += new System.EventHandler(this.BtSizeUp_Click);
            // 
            // BtSizeDown
            // 
            this.BtSizeDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BtSizeDown.Image = global::Controls.Zasoby.Alarm_Arrow_Down_icon;
            this.BtSizeDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BtSizeDown.Name = "BtSizeDown";
            this.BtSizeDown.Size = new System.Drawing.Size(23, 22);
            this.BtSizeDown.Text = "toolStripButton2";
            this.BtSizeDown.Click += new System.EventHandler(this.BtSizeDown_Click);
            // 
            // UIScript
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(385, 380);
            this.Controls.Add(this.editor);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "UIScript";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "UIScript";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.RichTextBox editor;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton BtSizeUp;
        private System.Windows.Forms.ToolStripButton BtSizeDown;

     
    }
}