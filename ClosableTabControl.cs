using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppTesteControlLibrary
{

    [ToolboxBitmap(typeof(TabControl))]
    public class ClosableTabControl : TabControl
    {

        public ClosableTabControl() : base()
        {
            Padding = new Point(12, 4);
            DrawMode = TabDrawMode.OwnerDrawFixed;

            DrawItem += _DrawItem;
            MouseDown += _MouseDown;
            Selecting += _Selecting;
            HandleCreated += _HandleCreated;
        }

        private Dictionary<Button, TabPage> dicButtons = new Dictionary<Button, TabPage>();


        //private bool blnShow = true;
        //private Image imgImage;

        public event CancelEventHandler CloseClick;

        //[Browsable(true)]
        //[DefaultValue(true)]
        //[Category("Behavior")]
        //[Description("Show / Hide Close Button(s)")]
        //public bool ShowButtonClose
        //{

        //	get
        //	{

        //		return blnShow;

        //	}

        //	set
        //	{

        //		blnShow = value;

        //		foreach (var btn in dicButtons.Keys)

        //			btn.Visible = blnShow;

        //		Repos();

        //	}

        //}

        //[Browsable(true)]
        //[DefaultValue(true)]
        //[Category("Appearance")]
        //[Description("Close Image")]
        //public Image TabPageImage
        //{

        //	get
        //	{

        //		return imgImage;

        //	}

        //	set
        //	{

        //		imgImage = value;

        //	}

        //}



        //protected override void OnCreateControl()
        //{
        //    base.OnCreateControl();
        //    Repos();
        //}


        //      [DllImport("user32.dll", SetLastError = true)]
        //      private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        //protected override void OnControlAdded(ControlEventArgs e)
        //{

        //    base.OnControlAdded(e);

        //    MessageBox.Show($"OnControlAdded - {TabCount}");

        //    TabPage tpCurrent = (TabPage)e.Control;

        //    Rectangle rtCurrent =
        //       this.GetTabRect(this.TabPages.IndexOf(tpCurrent));

        //    Button btnClose = new Button();

        //    btnClose.Image = Properties.Resources.Close;

        //    btnClose.ImageAlign = ContentAlignment.MiddleRight;
        //    btnClose.TextAlign = ContentAlignment.MiddleLeft;

        //    btnClose.Size = new Size(rtCurrent.Height - 1,
        //       rtCurrent.Height - 1);
        //    btnClose.Location = new Point(rtCurrent.X + rtCurrent.Width -
        //       rtCurrent.Height - 1, rtCurrent.Y + 1);

        //    SetParent(btnClose.Handle, this.Handle);

        //    btnClose.Click += OnCloseClick;

        //    dicButtons.Add(btnClose, tpCurrent);

        //}

        //protected override void OnLayout(LayoutEventArgs lea)
        //{
        //    base.OnLayout(lea);
        //    Repos();
        //}

        //      protected virtual void OnCloseClick(object sender, EventArgs e)
        //      {

        //          if (!DesignMode)
        //          {

        //              Button btnClose = (Button)sender;
        //              TabPage tpCurrent = dicButtons[btnClose];

        //              CancelEventArgs cea = new CancelEventArgs();

        //              CloseClick?.Invoke(sender, cea);

        //              if (!cea.Cancel)
        //              {

        //                  if (TabPages.Count > 1)
        //                  {

        //                      TabPages.Remove(tpCurrent);

        //                      btnClose.Dispose();
        //                      Repos();

        //                  }

        //                  else

        //                      MessageBox.Show("Must Have At Least 1 Tab Page");

        //              }

        //          }

        //      }

        //      public void Repos()
        //      {

        //          foreach (var but in dicButtons)

        //              Repos(but.Value);

        //      }

        //      public void Repos(TabPage tpCurrent)
        //      {

        //          Button btnClose = CloseButton(tpCurrent);

        //          if (btnClose != null)
        //          {

        //              int tpIndex = TabPages.IndexOf(tpCurrent);

        //              if (tpIndex >= 0)
        //              {

        //                  Rectangle rctCurrent = GetTabRect(tpIndex);

        //                  if (SelectedTab == tpCurrent)
        //                  {

        //                      btnClose.Size = new Size(rctCurrent.Height - 1,
        //                         rctCurrent.Height - 1);
        //                      btnClose.Location = new Point(rctCurrent.X +
        //                         rctCurrent.Width - rctCurrent.Height,
        //                         rctCurrent.Y + 1);

        //                  }

        //                  else
        //                  {

        //                      btnClose.Size = new Size(rctCurrent.Height - 3,
        //                         rctCurrent.Height - 2);
        //                      btnClose.Location = new Point(rctCurrent.X +
        //                         rctCurrent.Width - rctCurrent.Height - 1,
        //                         rctCurrent.Y + 1);

        //                  }

        //                  btnClose.Visible = ShowButtonClose;
        //                  btnClose.BringToFront();
        //              }

        //          }

        //      }

        //      protected Button CloseButton(TabPage tpCurrent)
        //{
        //	return (from item in dicButtons
        //			where item.Value == tpCurrent
        //			select item.Key).FirstOrDefault();
        //}

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        private const int TCM_SETMINTABWIDTH = 0x1300 + 49;

        private void _HandleCreated(object sender, EventArgs e)
        {
            SendMessage(Handle, TCM_SETMINTABWIDTH, IntPtr.Zero, (IntPtr)16);
        }

        private void _Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == TabCount - 1)
                e.Cancel = true;
        }

        private void _MouseDown(object sender, MouseEventArgs e)
        {
            var lastIndex = TabCount - 1;
            if (GetTabRect(lastIndex).Contains(e.Location))
            {
                NewPage(sender);
                //TabPages.Insert(lastIndex, $"New Tab {TabCount}");
                //SelectedIndex = lastIndex;
                //TabPages[lastIndex].UseVisualStyleBackColor = true;
            }
            else
            {
                for (var i = 0; i < TabPages.Count; i++)
                {
                    var tabRect = GetTabRect(i);
                    tabRect.Inflate(-1, -2);
                    var closeImage = Properties.Resources.Close_1;
                    var imageRect = new Rectangle(
                        tabRect.Right + 1 - closeImage.Width, tabRect.Y,
                        closeImage.Width, closeImage.Height);
                    if (imageRect.Contains(e.Location))
                    {
                        TabPages.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void _DrawItem(object sender, DrawItemEventArgs e)
        {
            var tabPage = TabPages[e.Index];
            var tabRect = GetTabRect(e.Index);

            if (e.Index == TabCount - 1)
            {
                tabRect.Inflate(-4, -4);
                var addImage = Properties.Resources.Add;
                e.Graphics.DrawImage(addImage,
                    tabRect.X, tabRect.Y,
                    addImage.Width - 2, addImage.Height - 2);
            }
            else
            {
                tabRect.Inflate(-1, -2);
                var closeImage = Properties.Resources.Close_1;

                e.Graphics.DrawImage(closeImage,
                    tabRect.Right + 1 - closeImage.Width, tabRect.Y,
                    closeImage.Width, closeImage.Height);

                TextRenderer.DrawText(e.Graphics, tabPage.Text, tabPage.Font,
                    tabRect, tabPage.ForeColor, TextFormatFlags.Left);
            }
        }

        public TabPage NewPage(object sender, string title = "Escolha uma tela")
        {
            var lastIndex = TabCount - 1;

            TabPages.Insert(lastIndex, title);
            SelectedIndex = lastIndex;
            TabPages[lastIndex].UseVisualStyleBackColor = true;
            return TabPages[lastIndex];
        }
    }

}
