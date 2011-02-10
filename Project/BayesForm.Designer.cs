namespace Project
{
    partial class BayesForm
    {
        private System.Windows.Forms.GroupBox envInfo;

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
            this.envInfo = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // envInfo
            // 
            this.envInfo.Location = new System.Drawing.Point(827, 10);
            this.envInfo.Name = "envInfo";
            this.envInfo.Size = new System.Drawing.Size(245, 705);
            this.envInfo.TabIndex = 0;
            this.envInfo.TabStop = false;
            this.envInfo.Text = "Environment Info";
            // 
            // BayesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 737);
            this.Controls.Add(this.envInfo);
            this.Name = "BayesForm";
            this.Text = "BayesForm";
            this.ResumeLayout(false);

        }

        #endregion
    }
}