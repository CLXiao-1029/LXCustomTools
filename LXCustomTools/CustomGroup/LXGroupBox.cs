using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LXCustomTools
{
    public partial class LXGroupBox : GroupBox
    {
        private Color _BorderColor = Color.Black;
        private float _BorderSize = 1f;
        SmoothingMode _SmoothingMode = SmoothingMode.None;

        [Category("自定义属性"), Description("边框宽度")]
        public float BorderSize
        {
            get { return _BorderSize; }
            set
            {
                _BorderSize = value;
                this.Invalidate();
            }
        }

        [Category("自定义属性"), Description("边框颜色")]
        public Color BorderColor
        {
            get { return _BorderColor; }
            set
            {
                _BorderColor = value;
                this.Invalidate();
            }
        }

        [Category("自定义属性"), Description("获取或设置此边框呈现的画线质量。"),DefaultValue(SmoothingMode.None)]
        public SmoothingMode SmoothingMode
        {
            get { return _SmoothingMode; }
            set
            {
                _SmoothingMode = value == SmoothingMode.Invalid ? SmoothingMode.Default : value;
                this.Invalidate();
            }
        }

        public LXGroupBox()
        {
            InitializeComponent();
        }

        public LXGroupBox(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var vSize = e.Graphics.MeasureString(Text, Font);

            e.Graphics.Clear(this.BackColor);
            e.Graphics.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), 10, 1);

            Pen vPen = new Pen(this._BorderColor, _BorderSize); // 用属性颜色来画边框颜色

            e.Graphics.SmoothingMode = _SmoothingMode;

            e.Graphics.DrawLine(vPen, 1, vSize.Height / 2, 8, vSize.Height / 2);
            e.Graphics.DrawLine(vPen, vSize.Width + 8, vSize.Height / 2, this.Width - 2, vSize.Height / 2);
            e.Graphics.DrawLine(vPen, 1, vSize.Height / 2, 1, this.Height - 2);
            e.Graphics.DrawLine(vPen, 1, this.Height - 2, this.Width - 2, this.Height - 2);
            e.Graphics.DrawLine(vPen, this.Width - 2, vSize.Height / 2, this.Width - 2, this.Height - 2);
            //e.Graphics.DrawRectangle(vPen, 0, 0, this.Width - 1, this.Height - 1);
            vPen.Dispose();
        }
    }
}
