using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LXCustomTools
{
    [DefaultEvent("HelpRequest"), Designer("System.Windows.Forms.Design.FolderBrowserDialogDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), DefaultProperty("SelectedPath"), Description("提示用户选择文件夹。")]
    public partial class LXVistaFolderBrowserDialog : LXVistaFolderBrowser
    {
        private string _description;
        private bool _useDescriptionForTitle;
        private string _selectedPath;
        private Environment.SpecialFolder _rootFolder;

        /// <summary>
        /// 当用户点击对话框上的帮助按钮时发生。
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler HelpRequest
        {
            add
            {
                base.HelpRequest += value;
            }
            remove
            {
                base.HelpRequest -= value;
            }
        }

        /// <summary>
        /// 创建一个新的 <see cref="LXVistaFolderBrowserDialog" /> 类.
        /// </summary>
        public LXVistaFolderBrowserDialog()
        {
            if (!IsVistaFolderDialogSupported)
                _downlevelDialog = new FolderBrowserDialog();
            else
                Reset();
        }

        #region Public Properties
        /// <summary>
        /// 获取一个值，表明当前操作系统是否支持Vista风格的普通文件对话框。
        /// </summary>
        /// <value>
        /// <see langword="true" /> 在Windows Vista或更新的操作系统上；否则, <see langword="false" />.
        /// </value>
        [Browsable(false)]
        public static bool IsVistaFolderDialogSupported
        {
            get
            {
                return IsWindowsVistaOrLater;
            }
        }

        public static bool IsWindowsVistaOrLater
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= new Version(6, 0, 6000);
            }
        }

        public static bool IsWindowsXPOrLater
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= new Version(5, 1, 2600);
            }
        }

        /// <summary>
        /// 获取或设置对话框中树状视图控件上方显示的描述性文本，或Vista风格对话框中列表视图控件下方显示的描述性文本。
        /// </summary>
        /// <value>
        /// 要显示的描述。默认是一个空字符串（""）。
        /// </value>
        [Category("Folder Browsing"), DefaultValue(""), Localizable(true), Browsable(true), Description("在对话框中树状视图控件上方显示的描述性文本，或在Vista风格对话框中列表视图控件下方显示的描述性文本。")]
        public string Description
        {
            get
            {
                if( _downlevelDialog != null )
                    return _downlevelDialog.Description;
                return _description;
            }
            set
            {
                if( _downlevelDialog != null )
                    _downlevelDialog.Description = value;
                else
                    _description = value ?? String.Empty;
            }
        }
        /// <summary>
        /// 获取或设置浏览开始的根文件夹。如果使用Vista风格的对话框，此属性没有影响。
        /// </summary>
        /// <value>
        /// One of the <see cref="System.Environment.SpecialFolder" /> values. The default is Desktop.
        /// </value>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value assigned is not one of the <see cref="System.Environment.SpecialFolder" /> values.</exception>
        [Localizable(false), Description("浏览开始的根文件夹。如果使用Vista风格的对话框，这个属性就没有影响。"), Category("Folder Browsing"), Browsable(true), DefaultValue(typeof(System.Environment.SpecialFolder), "Desktop")]
        public System.Environment.SpecialFolder RootFolder
        {
            get
            {
                if( _downlevelDialog != null )
                    return _downlevelDialog.RootFolder;
                return _rootFolder;
            }
            set
            {
                if( _downlevelDialog != null )
                    _downlevelDialog.RootFolder = value;
                else
                    _rootFolder = value;
            }
        }

        /// <summary>
        /// 获取或设置用户选择的路径。
        /// </summary>
        /// <value>
        /// 对话框中首次选择的文件夹或用户最后选择的文件夹的路径。默认是一个空字符串（""）。
        /// </value>
        [Browsable(true), Editor("System.Windows.Forms.Design.SelectedPathEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor)), Description("用户选择的路径。"), DefaultValue(""), Localizable(true), Category("Folder Browsing")]
        public string SelectedPath
        {
            get
            {
                if( _downlevelDialog != null )
                    return _downlevelDialog.SelectedPath;
                return _selectedPath;
            }
            set
            {
                if( _downlevelDialog != null )
                    _downlevelDialog.SelectedPath = value;
                else
                    _selectedPath = value ?? string.Empty;
            }
        }

        private bool _showNewFolderButton;

        /// <summary>
        /// 获取或设置一个值，表示 "新建文件夹 "按钮是否出现在文件夹浏览器对话框中。
        /// 如果使用Vista风格的对话框，此属性没有影响；在这种情况下，新文件夹按钮总是被显示。
        /// </summary>
        /// <value>
        /// <see langword="true" /> 如果对话框中显示了 "新建文件夹 "按钮；否则, <see langword="false" />. The default is <see langword="true" />.
        /// </value>
        [Browsable(true), Localizable(false), Description("指示新文件夹按钮是否出现在文件夹浏览器对话框中的一个值。如果使用Vista风格的对话框，此属性没有影响；在这种情况下，新文件夹按钮总是被显示。"), DefaultValue(true), Category("Folder Browsing")]
        public bool ShowNewFolderButton
        {
            get
            {
                if( _downlevelDialog != null )
                    return _downlevelDialog.ShowNewFolderButton;
                return _showNewFolderButton;
            }
            set
            {
                if( _downlevelDialog != null )
                    _downlevelDialog.ShowNewFolderButton = value;
                else
                    _showNewFolderButton = value;
            }
        }


        /// <summary>
        /// 获取或设置一个值，表示是否使用 <see cref="Description" /> 属性的值。
        /// 作为Vista风格对话框的对话框标题。此属性对旧风格的对话框没有影响。
        /// </summary>
        /// <value><see langword="true" /> 表示将 <see cref="Description" /> 属性值作为对话框的标题; <see langword="false" />
        /// 表示将该值作为附加文本添加到对话框中。默认是 <see langword="false" />.</value>
        [Category("Folder Browsing"), DefaultValue(false), Description("表示是否使用描述属性的值作为Vista风格对话框的对话框标题的值。此属性对旧风格的对话框没有影响。")]
        public bool UseDescriptionForTitle
        {
            get { return _useDescriptionForTitle; }
            set { _useDescriptionForTitle = value; }
        }	

        #endregion
        
        #region Public Methods

        /// <summary>
        /// Resets all properties to their default values.
        /// </summary>
        public override void Reset()
        {
            _description = string.Empty;
            _useDescriptionForTitle = false;
            _selectedPath = string.Empty;
            _rootFolder = Environment.SpecialFolder.Desktop;
            _showNewFolderButton = true;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// 指定一个普通的对话框。
        /// </summary>
        /// <param name="hwndOwner">一个值，代表普通对话框的所有者窗口的句柄。</param>
        /// <returns><see langword="true" /> 如果该文件可以被打开；否则, <see langword="false" />.</returns>
        protected override bool RunDialog(IntPtr hwndOwner)
        {
            if( _downlevelDialog != null )
                return _downlevelDialog.ShowDialog(hwndOwner == IntPtr.Zero ? null : new WindowHandleWrapper(hwndOwner)) == DialogResult.OK;

            IFileDialog dialog = null;
            try
            {
                dialog = new NativeFileOpenDialog();
                SetDialogProperties(dialog);
                int result = dialog.Show(hwndOwner);
                if( result < 0 )
                {
                    if( (uint)result == (uint)HRESULT.ERROR_CANCELLED )
                        return false;
                    else
                        throw System.Runtime.InteropServices.Marshal.GetExceptionForHR(result);
                } 
                GetResult(dialog);
                return true;
            }
            finally
            {
                if( dialog != null )
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(dialog);
            }
        }

        #endregion

        #region Private Methods

        private void SetDialogProperties(IFileDialog dialog)
        {
            // Description
            if( !string.IsNullOrEmpty(_description) )
            {
                if( _useDescriptionForTitle )
                {
                    dialog.SetTitle(_description);
                }
                else
                {
                    IFileDialogCustomize customize = (IFileDialogCustomize)dialog;
                    customize.AddText(0, _description);
                }
            }

            dialog.SetOptions(NativeMethods.FOS.FOS_PICKFOLDERS | NativeMethods.FOS.FOS_FORCEFILESYSTEM | NativeMethods.FOS.FOS_FILEMUSTEXIST);

            if( !string.IsNullOrEmpty(_selectedPath) )
            {
                string parent = Path.GetDirectoryName(_selectedPath);
                if( parent == null || !Directory.Exists(parent) )
                {
                    dialog.SetFileName(_selectedPath);
                }
                else
                {
                    string folder = Path.GetFileName(_selectedPath);
                    dialog.SetFolder(NativeMethods.CreateItemFromParsingName(parent));
                    dialog.SetFileName(folder);
                }
            }
        }

        private void GetResult(IFileDialog dialog)
        {
            IShellItem item;
            dialog.GetResult(out item);
            item.GetDisplayName(NativeMethods.SIGDN.SIGDN_FILESYSPATH, out _selectedPath);
        }

        #endregion
    }
    class WindowHandleWrapper : IWin32Window
    {
        private IntPtr _handle;

        public WindowHandleWrapper(IntPtr handle)
        {
            _handle = handle;
        }

        #region IWin32Window Members

        public IntPtr Handle
        {
            get { return _handle; }
        }

        #endregion
    }

}
