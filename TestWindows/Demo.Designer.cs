using LXCustomTools;

namespace TestWindows
{
    partial class Demo
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lxCustomTextBox1 = new LXCustomTools.LXCustomTextBox(this.components);
            this.lxImageButton1 = new LXCustomTools.LXImageButton(this.components);
            this.SuspendLayout();
            // 
            // lxCustomTextBox1
            // 
            this.lxCustomTextBox1.BorderColor = System.Drawing.Color.Red;
            this.lxCustomTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lxCustomTextBox1.Location = new System.Drawing.Point(103, 35);
            this.lxCustomTextBox1.Multiline = true;
            this.lxCustomTextBox1.Name = "lxCustomTextBox1";
            this.lxCustomTextBox1.Size = new System.Drawing.Size(301, 210);
            this.lxCustomTextBox1.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.lxCustomTextBox1.TabIndex = 0;
            // 
            // lxImageButton1
            // 
            this.lxImageButton1.DownImage = null;
            this.lxImageButton1.EnterImage = null;
            this.lxImageButton1.FlatAppearance.BorderSize = 0;
            this.lxImageButton1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.lxImageButton1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.lxImageButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lxImageButton1.Location = new System.Drawing.Point(13, 25);
            this.lxImageButton1.Name = "lxImageButton1";
            this.lxImageButton1.NormalImage = null;
            this.lxImageButton1.OpenAutoSize = false;
            this.lxImageButton1.OpenGraphicsPath = false;
            this.lxImageButton1.OpenTransparent = false;
            this.lxImageButton1.Size = new System.Drawing.Size(75, 23);
            this.lxImageButton1.TabIndex = 1;
            this.lxImageButton1.Text = "lxImageButton1";
            this.lxImageButton1.UseVisualStyleBackColor = true;
            // 
            // Demo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 304);
            this.Controls.Add(this.lxImageButton1);
            this.Controls.Add(this.lxCustomTextBox1);
            this.Name = "Demo";
            this.Text = "Demo";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private LXCustomTextBox lxCustomTextBox1;
        private LXImageButton lxImageButton1;
        #endregion
    }
}

