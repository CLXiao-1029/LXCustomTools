using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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
            Move,
        }

        bool graphicsPathOld = false;

        bool openGraphicsPath = false;
        [Category("自定义属性"), Description("开启图形路径绘制")]
        public bool OpenGraphicsPath
        {
            get { return openGraphicsPath; }
            set
            {
                openGraphicsPath = value;
                if (_bgImage != null && openGraphicsPath && !graphicsPathOld)
                {
                    ControlTrans(this, _bgImage);
                }
                this.Invalidate();
            }
        }

        bool openTrans = false;
        [Category("自定义属性"), Description("设置透明颜色鼠标穿透")]
        public bool OpenTransparent
        {
            get { return openTrans; }
            set
            {
                openTrans = value;
                this.Invalidate();
            }
        }

        bool openAutoSize = false;
        [Category("自定义属性"), Description("设置自动大小")]
        public bool OpenAutoSize
        {
            get { return openAutoSize; }
            set
            {
                openAutoSize = value;
                ChangeSize();
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
                    BgImage = _normalImage;
                ChangeSize();

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
                ChangeSize();

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
                ChangeSize();
                this.Invalidate();
            }
        }

        ButtonStateEx _stateEx = ButtonStateEx.Normal;
        ButtonStateEx StateEx
        {
            get { return _stateEx; }
            set
            {
                if (_stateEx != value)
                {
                    _stateEx = value;
                }
            }
        }

        Bitmap _bgImage = null;
        Bitmap BgImage
        {
            get
            {
                return _bgImage;
            }
            set
            {
                if (_bgImage != value)
                {
                    _bgImage = value;
                    BackgroundImage = _bgImage;
                    if (openGraphicsPath)
                        ControlTrans(this, _bgImage);
                }
            }
        }

        public LXImageButton()
        {
            InitializeComponent();
            Text = "";
            ChangeFlatAppearance();
            FlatStyle = FlatStyle.Flat;
        }

        public LXImageButton(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            Text = "";
            ChangeFlatAppearance();
            FlatStyle = FlatStyle.Flat;
        }

        void ChangeFlatAppearance()
        {
            FlatAppearance.BorderSize = 0;
            FlatAppearance.BorderColor = Color.FromArgb(0, 0, 0, 0);
            FlatAppearance.MouseDownBackColor = Color.Transparent;
            FlatAppearance.MouseOverBackColor = Color.Transparent;
        }

        void ChangeSize()
        {
            if (openAutoSize)
            {
                Size = NormalImage?.Size ?? EnterImage?.Size ?? DownImage?.Size ?? new Size(96, 32);
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                //WS_EX_TRANSPARENT 

                cp.ExStyle |= 0x00000020;

                return cp;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            StateEx = ButtonStateEx.Enter;
            base.OnMouseEnter(e);
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            if (StateEx == ButtonStateEx.Enter)
            {
                StateEx = ButtonStateEx.Down;
                BgImage = _downImage;
            }
            base.OnMouseDown(mevent);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            StateEx = ButtonStateEx.Normal;
            BgImage = _normalImage;
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
                if (_enterImage.Height - 1 < posY)
                    posY = _enterImage.Height - 1;

                posX = posX < 0 ? 0 : posX;
                posY = posY < 0 ? 0 : posY;

                Color curColor = _enterImage.GetPixel(posX, posY);
                if (curColor.ToArgb() == 0)
                {
                    StateEx = ButtonStateEx.Normal;
                    BgImage = _normalImage;
                }
                else if (!StateEx.HasFlag(ButtonStateEx.Move))
                {
                    //StateEx = ButtonStateEx.Enter;
                    StateEx = ButtonStateEx.Move & ButtonStateEx.Enter;
                    BgImage = _enterImage;
                }
            }
            base.OnMouseMove(mevent);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (!StateEx.HasFlag(ButtonStateEx.Enter) && !StateEx.HasFlag(ButtonStateEx.Down))
            {
                return;
            }
            base.OnMouseClick(e);
        }

        protected override void OnClick(EventArgs e)
        {
            if (!StateEx.HasFlag(ButtonStateEx.Enter) && !StateEx.HasFlag(ButtonStateEx.Down))
            {
                return;
            }
            base.OnClick(e);
        }

#if(DEBUG)//需要开启不安全代码编译
        private unsafe static GraphicsPath subGraphicsPath(Image img)
        {
            if (img == null) return null;

            // 建立GraphicsPath, 给我们的位图路径计算使用   
            GraphicsPath g = new GraphicsPath(FillMode.Alternate);

            Bitmap bitmap = new Bitmap(img);

            int width = bitmap.Width;
            int height = bitmap.Height;
            BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            byte* p = (byte*)bmData.Scan0;
            int offset = bmData.Stride - width * 3;
            int p0, p1, p2;         // 记录左上角0，0座标的颜色值  
            p0 = p[0];
            p1 = p[1];
            p2 = p[2];

            int start = -1;
            // 行座标 ( Y col )   
            for (int Y = 0; Y < height; Y++)
            {
                // 列座标 ( X row )   
                for (int X = 0; X < width; X++)
                {
                    if (start == -1 && (p[0] != p0 || p[1] != p1 || p[2] != p2))     //如果 之前的点没有不透明 且 不透明   
                    {
                        start = X;                            //记录这个点  
                    }
                    else if (start > -1 && (p[0] == p0 && p[1] == p1 && p[2] == p2))      //如果 之前的点是不透明 且 透明  
                    {
                        g.AddRectangle(new Rectangle(start, Y, X - start, 1));    //添加之前的矩形到  
                        start = -1;
                    }

                    if (X == width - 1 && start > -1)        //如果 之前的点是不透明 且 是最后一个点  
                    {
                        g.AddRectangle(new Rectangle(start, Y, X - start + 1, 1));      //添加之前的矩形到  
                        start = -1;
                    }
                    //if (p[0] != p0 || p[1] != p1 || p[2] != p2)  
                    //    g.AddRectangle(new Rectangle(X, Y, 1, 1));  
                    p += 3;                                   //下一个内存地址  
                }
                p += offset;
            }
            bitmap.UnlockBits(bmData);
            bitmap.Dispose();
            // 返回计算出来的不透明图片路径   
            return g;
        }

        //Image lastImg =null;
        List<Image> images = new List<Image>();
        /// <summary>  
        /// 调用此函数后使图片透明  
        /// </summary>  
        /// <param name="control">需要处理的控件</param>  
        /// <param name="img">控件的背景或图片，如PictureBox.Image  
        ///   或PictureBox.BackgroundImage</param>  
        public void ControlTrans(Control control, Image img)
        {
            if (images.Contains(img))
                return;
            images.Add(img);
            //lastImg = img;
            GraphicsPath g;
            g = subGraphicsPath(img);
            if (g == null)
                return;
            control.Region = new Region(g);
            graphicsPathOld = true;
        }
#endif
    }
}
