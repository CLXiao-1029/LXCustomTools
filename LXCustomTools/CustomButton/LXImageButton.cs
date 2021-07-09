using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LXCustomTools
{
    public partial class LXImageButton : Button
    {
        public enum ButtonStateEx
        {
            Normal,
            Enter,
            Down,
        }

        bool openTrans = false;
        [Category("自定义属性"), Description("设置透明颜色鼠标穿透")]
        public bool OpenTransparent
        {
            get { return openTrans; }
            set { 
                openTrans = value;
                this.Invalidate();
            }
        }

        Bitmap _normalImage;
        Bitmap _enterImage;
        Bitmap _downImage;
        [Category("自定义属性"), Description("正常状态的图片")]
        public Bitmap NormalImage
        {
            get { return _normalImage; }
            set
            {
                _normalImage = value;
                if (_normalImage == null)
                {
                    if (BackgroundImage != null)
                        _normalImage = new Bitmap(BackgroundImage);
                }
                else
                {
                    BackgroundImage = _normalImage;
                }

                this.Invalidate();
            }
        }
        [Category("自定义属性"), Description("进入状态的图片")]
        public Bitmap EnterImage
        {
            get { return _enterImage; }
            set
            {
                _enterImage = value;
                if (_enterImage == null)
                    _enterImage = _normalImage;

                this.Invalidate();
            }
        }
        [Category("自定义属性"), Description("按下状态的图片")]
        public Bitmap DownImage
        {
            get { return _downImage; }
            set
            {
                _downImage = value;
                if (_downImage == null)
                    _downImage = _normalImage;

                this.Invalidate();
            }
        }

        ButtonStateEx stateEx = ButtonStateEx.Normal;


        public LXImageButton()
        {
            InitializeComponent();
            Text = "";
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseDownBackColor = Color.Transparent;
            FlatAppearance.MouseOverBackColor = Color.Transparent;
        }

        public LXImageButton(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            Text = "";
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseDownBackColor = Color.Transparent;
            FlatAppearance.MouseOverBackColor = Color.Transparent;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            stateEx = ButtonStateEx.Enter;
            base.OnMouseEnter(e);
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            if (stateEx == ButtonStateEx.Enter)
            {
                stateEx = ButtonStateEx.Down;
                BackgroundImage = _downImage;
            }
            base.OnMouseDown(mevent);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            stateEx = ButtonStateEx.Normal;
            BackgroundImage = _normalImage;
            base.OnMouseLeave(e);
        }

        protected override void OnMouseMove(MouseEventArgs mevent)
        {
            int posX = mevent.X;
            int posY = mevent.Y;
            if (openTrans)
            {
                if (_enterImage.Width - 1 < posX)
                    posX = _enterImage.Width - 1;
                if (_enterImage.Height - 1  < posY)
                    posY = _enterImage.Height -1;
                Color curColor = _enterImage.GetPixel(posX, posY);
                if (curColor.ToArgb() == 0)
                {
                    stateEx = ButtonStateEx.Normal;
                    BackgroundImage = _normalImage;
                }
                else
                {
                    stateEx = ButtonStateEx.Enter;
                    BackgroundImage = _enterImage;
                }
            }
            base.OnMouseMove(mevent);
        }

        protected override void OnClick(EventArgs e)
        {
            if (stateEx == ButtonStateEx.Normal)
            {
                return;
            }
            base.OnClick(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (stateEx != ButtonStateEx.Enter)
            {
                return;
            }
            base.OnMouseClick(e);
        }

    }
}
