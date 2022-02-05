using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LXCustomTools
{
    public partial class ListViewEx : ListView
    {
        #region 操作结构、导入和常量
        /// <summary>
        /// 消息结构体 WM_NOTIFY
        /// </summary>
        private struct NMHDR
        {
            public IntPtr hwndFrom;
            public Int32 idFrom;
            public Int32 code;
        }


        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wPar, IntPtr lPar);
        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int len, ref int[] order);

        // ListView 消息
        private const int LVM_FIRST = 0x1000;
        private const int LVM_GETCOLUMNORDERARRAY = (LVM_FIRST + 59);

        // 将中止编辑的Windows消息
        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;
        private const int WM_SIZE = 0x05;
        private const int WM_NOTIFY = 0x4E;

        private const int HDN_FIRST = -300;
        private const int HDN_BEGINDRAG = (HDN_FIRST - 10);
        private const int HDN_ITEMCHANGINGA = (HDN_FIRST - 0);
        private const int HDN_ITEMCHANGINGW = (HDN_FIRST - 20);
        #endregion

        public event SubItemEventHandler SubItemClicked;
        public event SubItemEventHandler SubItemBeginEditing;
        public event SubItemEndEditingEventHandler SubItemEndEditing;

        public ListViewEx()
        {
            InitializeComponent();

            base.FullRowSelect = true;
            base.View = View.Details;
            base.AllowColumnReorder = true;
        }

        public ListViewEx(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        private bool _doubleClickActivation = true;
        /// <summary>
        /// 是否需要双击才能开始编辑单元格 ?
        /// </summary>
        public bool DoubleClickActivation
        {
            get { return _doubleClickActivation; }
            set { _doubleClickActivation = value; }
        }

        /// <summary>
        /// 检索列出现的顺序
        /// </summary>
        /// <returns>列索引的当前显示顺序</returns>
        public int[] GetColumnOrder()
        {
            IntPtr lPar = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)) * Columns.Count);

            IntPtr res = SendMessage(Handle, LVM_GETCOLUMNORDERARRAY, new IntPtr(Columns.Count), lPar);
            if (res.ToInt32() == 0) // Something went wrong
            {
                Marshal.FreeHGlobal(lPar);
                return null;
            }

            int[] order = new int[Columns.Count];
            Marshal.Copy(lPar, order, 0, Columns.Count);

            Marshal.FreeHGlobal(lPar);

            return order;
        }
        /// <summary>
		/// 找到ListViewItem和子项索引在位置  (x,y)
		/// </summary>
		/// <param name="x">相对于ListView</param>
		/// <param name="y">相对于ListView</param>
		/// <param name="item">位置项目 (x,y)</param>
		/// <returns>SubItem index</returns>
		public int GetSubItemAt(int x, int y, out ListViewItem item)
        {
            item = this.GetItemAt(x, y);

            if (item != null)
            {
                int[] order = GetColumnOrder();
                Rectangle lviBounds;
                int subItemX;

                lviBounds = item.GetBounds(ItemBoundsPortion.Entire);
                subItemX = lviBounds.Left;
                for (int i = 0; i < order.Length; i++)
                {
                    ColumnHeader h = this.Columns[order[i]];
                    if (x < subItemX + h.Width)
                    {
                        return h.Index;
                    }
                    subItemX += h.Width;
                }
            }

            return -1;
        }


        /// <summary>
        /// 获取子项的边界
        /// </summary>
        /// <param name="Item">目标 ListViewItem</param>
        /// <param name="SubItem">目标SubItem 索引</param>
        /// <returns>返回相对于 ListView 的子项边界</returns>
        public Rectangle GetSubItemBounds(ListViewItem Item, int SubItem)
        {
            int[] order = GetColumnOrder();

            Rectangle subItemRect = Rectangle.Empty;
            if (SubItem >= order.Length)
                throw new IndexOutOfRangeException("SubItem " + SubItem + " out of range");

            if (Item == null)
                throw new ArgumentNullException("Item");

            Rectangle lviBounds = Item.GetBounds(ItemBoundsPortion.Entire);
            int subItemX = lviBounds.Left;

            ColumnHeader col;
            int i;
            for (i = 0; i < order.Length; i++)
            {
                col = this.Columns[order[i]];
                if (col.Index == SubItem)
                    break;
                subItemX += col.Width;
            }
            subItemRect = new Rectangle(subItemX, lviBounds.Top, this.Columns[order[i]].Width, lviBounds.Height);
            return subItemRect;
        }


        protected override void WndProc(ref Message msg)
        {
            switch (msg.Msg)
            {
                // Look	for	WM_VSCROLL,WM_HSCROLL or WM_SIZE messages.
                case WM_VSCROLL:
                case WM_HSCROLL:
                case WM_SIZE:
                    EndEditing(false);
                    break;
                case WM_NOTIFY:
                    // Look for WM_NOTIFY of events that might also change the
                    // editor's position/size: Column reordering or resizing
                    NMHDR h = (NMHDR)Marshal.PtrToStructure(msg.LParam, typeof(NMHDR));
                    if (h.code == HDN_BEGINDRAG ||
                        h.code == HDN_ITEMCHANGINGA ||
                        h.code == HDN_ITEMCHANGINGW)
                        EndEditing(false);
                    break;
            }

            base.WndProc(ref msg);
        }

        #region 根据DoubleClickActivation属性初始化编辑 
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (DoubleClickActivation)
            {
                return;
            }

            EditSubitemAt(new Point(e.X, e.Y));
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);

            if (!DoubleClickActivation)
            {
                return;
            }

            Point pt = this.PointToClient(Cursor.Position);

            EditSubitemAt(pt);
        }

        ///<summary>
        /// Fire SubItemClicked
        ///</summary>
        ///<param name="p">Point of click/doubleclick</param>
        private void EditSubitemAt(Point p)
        {
            ListViewItem item;
            int idx = GetSubItemAt(p.X, p.Y, out item);
            if (idx >= 0)
            {
                OnSubItemClicked(new SubItemEventArgs(item, idx));
            }
        }

        #endregion

        #region In-place editing functions
        // The control performing the actual editing
        private Control _editingControl;
        // The LVI being edited
        private ListViewItem _editItem;
        // The SubItem being edited
        private int _editSubItem;

        protected void OnSubItemBeginEditing(SubItemEventArgs e)
        {
            if (SubItemBeginEditing != null)
                SubItemBeginEditing(this, e);
        }
        protected void OnSubItemEndEditing(SubItemEndEditingEventArgs e)
        {
            if (SubItemEndEditing != null)
                SubItemEndEditing(this, e);
        }
        protected void OnSubItemClicked(SubItemEventArgs e)
        {
            if (SubItemClicked != null)
                SubItemClicked(this, e);
        }

        /// <summary>
		/// 开始对给定单元格进行就地编辑
		/// </summary>
		/// <param name="c">控件用作单元格编辑器</param>
		/// <param name="Item">预编辑项</param>
		/// <param name="SubItem">预编辑子项索引</param>
		public void StartEditing(Control c, ListViewItem Item, int SubItem)
        {
            OnSubItemBeginEditing(new SubItemEventArgs(Item, SubItem));

            Rectangle rcSubItem = GetSubItemBounds(Item, SubItem);

            if (rcSubItem.X < 0)
            {
                rcSubItem.Width += rcSubItem.X;
                rcSubItem.X = 0;
            }
            if (rcSubItem.X + rcSubItem.Width > this.Width)
            {
                rcSubItem.Width = this.Width - rcSubItem.Left;
            }

            // 子项边界是相对于ListView的位置的! 
            rcSubItem.Offset(Left, Top);

            Point origin = new Point(0, 0);
            Point lvOrigin = this.Parent.PointToScreen(origin);
            if (c.Parent == null)
                throw new IndexOutOfRangeException("ListView SubItem EditBox  are not in the panel.");

            Point ctlOrigin = c.Parent.PointToScreen(origin);
            rcSubItem.Offset(lvOrigin.X - ctlOrigin.X, lvOrigin.Y - ctlOrigin.Y);

            // Position and show editor
            c.Bounds = rcSubItem;
            if (SubItem >= Item.SubItems.Count)
                c.Text = default;
            else
                c.Text = Item.SubItems[SubItem]?.Text;
            c.Visible = true;
            c.BringToFront();
            c.Focus();

            _editingControl = c;
            _editingControl.Leave += new EventHandler(_editControl_Leave);
            _editingControl.KeyPress += new KeyPressEventHandler(_editControl_KeyPress);

            _editItem = Item;
            _editSubItem = SubItem;
        }


        private void _editControl_Leave(object sender, EventArgs e)
        {
            // cell editor losing focus
            EndEditing(true);
        }

        private void _editControl_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char)(int)Keys.Escape:
                    {
                        EndEditing(false);
                        break;
                    }

                case (char)(int)Keys.Enter:
                    {
                        EndEditing(true);
                        break;
                    }
            }
        }

        /// <summary>
        /// 接受或放弃单元格编辑器控件的当前值 
        /// </summary>
        /// <param name="AcceptChanges">使用_editingControl的文本作为新的子项文本或丢弃更改? </param>
        public void EndEditing(bool AcceptChanges)
        {
            if (_editingControl == null)
                return;

            SubItemEndEditingEventArgs e = new SubItemEndEditingEventArgs(
                _editItem,      // The item being edited
                _editSubItem,   // The subitem index being edited
                AcceptChanges ?
                    _editingControl.Text :  // Use editControl text if changes are accepted
                    _editItem.SubItems[_editSubItem].Text,  // or the original subitem's text, if changes are discarded
                !AcceptChanges  // Cancel?
            );

            OnSubItemEndEditing(e);
            if (_editSubItem >= _editItem.SubItems.Count)
                _editItem.SubItems.Add(e.DisplayText);
            else
                _editItem.SubItems[_editSubItem].Text = e.DisplayText;

            _editingControl.Leave -= new EventHandler(_editControl_Leave);
            _editingControl.KeyPress -= new KeyPressEventHandler(_editControl_KeyPress);

            _editingControl.Visible = false;

            _editingControl = null;
            _editItem = null;
            _editSubItem = -1;
        }
        #endregion
    }

    /// <summary>
    /// Event Handler for SubItem events
    /// </summary>
    public delegate void SubItemEventHandler(object sender, SubItemEventArgs e);
    /// <summary>
    /// Event Handler for SubItemEndEditing events
    /// </summary>
    public delegate void SubItemEndEditingEventHandler(object sender, SubItemEndEditingEventArgs e);
    /// <summary>
    /// Event Args for SubItemClicked event
    /// </summary>
    public class SubItemEventArgs : EventArgs
    {
        public SubItemEventArgs(ListViewItem item, int subItem)
        {
            _subItemIndex = subItem;
            _item = item;
        }
        private int _subItemIndex = -1;
        private ListViewItem _item = null;
        public int SubItem
        {
            get { return _subItemIndex; }
        }
        public ListViewItem Item
        {
            get { return _item; }
        }
    }


    /// <summary>
    /// Event Args for SubItemEndEditingClicked event
    /// </summary>
    public class SubItemEndEditingEventArgs : SubItemEventArgs
    {
        private string _text = string.Empty;
        private bool _cancel = true;

        public SubItemEndEditingEventArgs(ListViewItem item, int subItem, string display, bool cancel) :
            base(item, subItem)
        {
            _text = display;
            _cancel = cancel;
        }
        public string DisplayText
        {
            get { return _text; }
            set { _text = value; }
        }
        public bool Cancel
        {
            get { return _cancel; }
            set { _cancel = value; }
        }
    }
}
