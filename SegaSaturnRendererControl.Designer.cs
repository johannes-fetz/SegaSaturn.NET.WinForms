namespace SegaSaturn.NET.WinForms
{
    partial class SegaSaturnRendererControl
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.openGlControl = new OpenGL.GlControl();
            this.SuspendLayout();
            // 
            // openGlControl
            // 
            this.openGlControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.openGlControl.ColorBits = ((uint)(24u));
            this.openGlControl.DepthBits = ((uint)(24u));
            this.openGlControl.Location = new System.Drawing.Point(0, 0);
            this.openGlControl.MultisampleBits = ((uint)(0u));
            this.openGlControl.Name = "openGlControl";
            this.openGlControl.Size = new System.Drawing.Size(320, 240);
            this.openGlControl.StencilBits = ((uint)(0u));
            this.openGlControl.TabIndex = 0;
            // 
            // SegaSaturnRendererControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.openGlControl);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(320, 256);
            this.Name = "SegaSaturnRendererControl";
            this.Size = new System.Drawing.Size(640, 480);
            this.ResumeLayout(false);

        }

        #endregion

        private OpenGL.GlControl openGlControl;
    }
}
