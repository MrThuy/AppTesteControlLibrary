
namespace AppTesteControlLibrary
{
    partial class FormTesteComponents
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
            this.closableTabBtn1 = new AppTesteControlLibrary.ClosableTabBtn();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.closableTabBtn1.SuspendLayout();
            this.SuspendLayout();
            // 
            // closableTabBtn1
            // 
            this.closableTabBtn1.AllowDrop = true;
            this.closableTabBtn1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.closableTabBtn1.Controls.Add(this.tabPage6);
            this.closableTabBtn1.Controls.Add(this.tabPage3);
            this.closableTabBtn1.Controls.Add(this.tabPage4);
            this.closableTabBtn1.Location = new System.Drawing.Point(12, 12);
            this.closableTabBtn1.Name = "closableTabBtn1";
            this.closableTabBtn1.Padding = new System.Drawing.Point(12, 4);
            this.closableTabBtn1.SelectedIndex = 0;
            this.closableTabBtn1.Size = new System.Drawing.Size(662, 203);
            this.closableTabBtn1.TabIndex = 0;
            // 
            // tabPage6
            // 
            this.tabPage6.Location = new System.Drawing.Point(4, 26);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(654, 150);
            this.tabPage6.TabIndex = 0;
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 26);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(654, 150);
            this.tabPage3.TabIndex = 8;
            this.tabPage3.Text = "tabPage3    ";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 26);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(654, 173);
            this.tabPage4.TabIndex = 9;
            this.tabPage4.Text = "tabPage4    ";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // FormTesteComponents
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 300);
            this.Controls.Add(this.closableTabBtn1);
            this.Name = "FormTesteComponents";
            this.Text = "Form1";
            this.closableTabBtn1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ClosableTabBtn closableTabBtn1;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
    }
}