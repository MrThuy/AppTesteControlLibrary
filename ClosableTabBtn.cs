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
    class TaPageInfo
    {
        TabPage tab;
        String  title;
    }

    [ToolboxBitmap(typeof(TabControl))]
    public class ClosableTabBtn : TabControl
    {

        public ClosableTabBtn() : base()
        {

            Padding = new Point(12, 4);

            MouseDown += _MouseDown;
            Selecting += _Selecting;
            HandleCreated += _HandleCreated;
            //Multiline = true;
            //AllowDrop = true;
        }

        private Dictionary<Button, TabPage> dicButtons = new Dictionary<Button, TabPage>();

        private bool blnShow = true;
        private Image imgImage;
        private bool bDragingOver;
        private Point _pt = new Point(0, 0);

        public event EventHandler AfterCloseClick;
        public event CancelEventHandler CloseClick;

        [Browsable(true)]
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("Show / Hide Close Button(s)")]
        public bool ShowButtonClose
        {
            get { return blnShow; }

            set
            {
                blnShow = value;

                foreach (var btn in dicButtons.Keys)

                    btn.Visible = blnShow;

                Repos();
            }
        }

        [Browsable(true)]
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Close Image")]
        public Image TabPageImage
        {
            get { return imgImage; }
            set { imgImage = value; }
        }

        //Evento ao clicar nas setas de navegação quando tem muitas abas
        const int WM_HSCROLL = 0x0114;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_HSCROLL)
            {
                Repos();
                //System.Diagnostics.Debug.WriteLine($"WndProc {DateTime.Now.ToString("ss.fff")}");
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            Repos();
        }

        private Size btnSize(Rectangle rtCurrent)
        {
            return new Size(18,18);
        }

        private Point btnLocation(Rectangle rtCurrent)
        {
            return new Point(rtCurrent.X + rtCurrent.Width - 19,
                             rtCurrent.Y + (rtCurrent.Height/2) - (18/2) );
        }


        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            TabPage tpCurrent = (TabPage)e.Control;

            string _Title = tpCurrent.Text.Trim();

            Rectangle rctCurrent =this.GetTabRect(this.TabPages.IndexOf(tpCurrent));

            Button btnClose = new Button();

            btnClose.Image      = Properties.Resources.Close_10;
            btnClose.ImageAlign = ContentAlignment.MiddleCenter;
            btnClose.TextAlign  = ContentAlignment.MiddleLeft;
            btnClose.Size       = btnSize(rctCurrent);
            btnClose.Location   = btnLocation(rctCurrent);

            //System.Diagnostics.Debug.WriteLine($"OnControlAdded Size:{btnClose.Size} Location {btnClose.Location} {DateTime.Now.ToString("ss.fff")}");

            SetParent(btnClose.Handle, this.Handle);

            btnClose.Click += OnCloseClick;

            dicButtons.Add(btnClose, tpCurrent );

            tpCurrent.Text = _Title + "    ";
        }

        protected override void OnLayout(LayoutEventArgs lea)
        {
            base.OnLayout(lea);
            Repos();
        }

        protected virtual void OnCloseClick(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                Button btnClose = (Button)sender;
                TabPage tpCurrent = dicButtons[btnClose];

                CancelEventArgs cea = new CancelEventArgs();

                CloseClick?.Invoke(sender, cea);

                if (!cea.Cancel)
                {
                    if (TabPages.Count > 1)
                    {
                        TabPages.Remove(tpCurrent);

                        btnClose.Dispose();
                        Repos();
                        AfterCloseClick?.Invoke(sender, e);
                    };
                }
            }
        }

        protected virtual void OnAddClick(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                NewPage(sender);
            }
        }

        public void Repos()
        {
            foreach (var but in dicButtons)
                Repos(but.Value);
        }

        public void Repos(TabPage tpCurrent)
        {
            if (bDragingOver) return;

            Button btnClose = GetButton(tpCurrent);

            if (btnClose != null)
            {
                {
                    int tpIndex = TabPages.IndexOf(tpCurrent);

                    if (tpIndex >= 0)
                    {
                        bool _LastPage = (tpCurrent == TabPages[TabCount - 1]);

                        if (_LastPage)
                        {
                            btnClose.Image = Properties.Resources.Add;
                            btnClose.Click -= OnCloseClick;
                            btnClose.Click -= OnAddClick;
                            btnClose.Click += OnAddClick;
                            tpCurrent.Text = "";
                        }

                        Rectangle rctCurrent = GetTabRect(tpIndex);

                        if (SelectedTab == tpCurrent)
                        {

                            btnClose.Size = btnSize(rctCurrent);
                            btnClose.Location = btnLocation(rctCurrent);
                        }

                        else
                        {
                            btnClose.Size = new Size(rctCurrent.Height - 3,
                                                     rctCurrent.Height - 2);

                            btnClose.Location = new Point(rctCurrent.X +
                                                          rctCurrent.Width - rctCurrent.Height - 1,
                                                          rctCurrent.Y + 1);
                        }

                        //btnClose.Visible = ShowButtonClose;
                        btnClose.Visible = (rctCurrent.Contains(_pt) || (SelectedIndex == tpIndex) || _LastPage);
                        btnClose.BringToFront();

                    }
                }
            }
        }

        protected Button GetButton(TabPage tpCurrent)
        {
            return (from item in dicButtons
                    where item.Value == tpCurrent
                    select item.Key).FirstOrDefault();
        }

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
            }
        }

        public TabPage NewPage(object sender, string title = "Escolha uma tela")
        {
            var lastIndex = TabCount - 1;

            TabPages.Insert(lastIndex, title + lastIndex );
            SelectedIndex = lastIndex;
            TabPages[lastIndex].UseVisualStyleBackColor = true;
            return TabPages[lastIndex];
        }
        public TabPage InsertaPage(TabPage tab)
        {
            var lastIndex = TabCount - 1;

            TabPages.Insert(lastIndex, tab );
            SelectedIndex = lastIndex;
            TabPages[lastIndex].UseVisualStyleBackColor = true;
            return TabPages[lastIndex];
        }

        //protected override void OnMouseMove(MouseEventArgs e)
        //{
        //    base.OnMouseMove(e);
        //    if (e.Button == MouseButtons.Left)
        //    {
        //        DoDragDrop( SelectedTab, DragDropEffects.All);
        //        Repos();
        //    }
        //}

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);

            if (!(drgevent.Data.GetDataPresent(typeof(TabPage))))
                drgevent.Effect = DragDropEffects.None;
            //drgevent.Effect = DragDropEffects.Move;
            else
            {
                TabPage DropTab = (TabPage)(drgevent.Data.GetData(typeof(TabPage)));

                Point pt = new Point(drgevent.X, drgevent.Y);
                //We need client coordinates.
                pt = PointToClient(pt);

                TabPage hover_tab = GetTabPageByTab(pt);
                int drop_location_index = FindIndex(hover_tab);

                //if (drop_location_index == FindIndex(DropTab))
                //    drgevent.Effect = DragDropEffects.None;
                //else
                if (drop_location_index >= TabCount - 1)
                    drgevent.Effect = DragDropEffects.None;
                else if (drop_location_index < 0)
                    drgevent.Effect = DragDropEffects.None;
                else
                    drgevent.Effect = DragDropEffects.Move;
            }
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);
            TabPage DropTab = (TabPage)(drgevent.Data.GetData(typeof(TabPage)));
            Point pt = new Point(drgevent.X, drgevent.Y);
            //We need client coordinates.
            pt = PointToClient(pt);

            TabPage hover_tab = GetTabPageByTab(pt);
            int drop_location_index = FindIndex(hover_tab);

            if (drop_location_index == FindIndex(DropTab))
                drgevent.Effect = DragDropEffects.None;
            else if (drop_location_index >= TabCount - 1)
                drgevent.Effect = DragDropEffects.None;
            else if (drop_location_index < 0)
                drgevent.Effect = DragDropEffects.None;
            else
            {
                //TabPages.move


                //TabPages.Remove(DropTab);
                //DropTab.TabIndex = 1;

                int index_src = TabPages.IndexOf(DropTab);
                int index_dst = TabPages.IndexOf(hover_tab);
                TabPages[index_dst] = DropTab;

                //for (int i = index_src; i < TabPages.Count; i++)
                //{
                //    if (TabPages[i] == page)
                //        return i;
                //}

                TabPages[index_src] = hover_tab;
                Refresh();

                //Button btnClose = GetButton(DropTab);
                //btnClose.PerformClick();
                ////btnClose.Dispose();
                //TabPages.Insert(drop_location_index, DropTab);
                Repos();
            }
        }

        //protected override void OnDragOver(DragEventArgs e)
        //{
        //    base.OnDragOver(e);

        //    bDragingOver = true;
        //    try
        //    {
        //        Point pt = new Point(e.X, e.Y);
        //        //We need client coordinates.
        //        pt = PointToClient(pt);

        //        //Get the tab we are hovering over.
        //        TabPage hover_tab = GetTabPageByTab(pt);

        //        //Make sure we are on a tab.
        //        if (hover_tab != null)
        //        {
        //            //Make sure there is a TabPage being dragged.
        //            if (e.Data.GetDataPresent(typeof(TabPage)))
        //            {
        //                e.Effect = DragDropEffects.Move;
        //                TabPage drag_tab = (TabPage)e.Data.GetData(typeof(TabPage));
        //                int item_drag_index = FindIndex(drag_tab);
        //                int drop_location_index = FindIndex(hover_tab);

        //                //Don't do anything if we are hovering over ourself.
        //                if (item_drag_index != drop_location_index)
        //                {

        //                    System.Collections.ArrayList pages = new System.Collections.ArrayList();
        //                    //Put all tab pages into an array.
        //                    for (int i = 0; i < TabPages.Count; i++)
        //                    {
        //                        //Except the one we are dragging.
        //                        if (i != item_drag_index)
        //                            pages.Add(TabPages[i]);
        //                    }

        //                    //Now put the one we are dragging it at the proper location.
        //                    pages.Insert(drop_location_index, drag_tab);

        //                    //Make them all go away for a nanosec.
        //                    TabPages.Clear();

        //                    //Add them all back in.
        //                    TabPages.AddRange((TabPage[])pages.ToArray(typeof(TabPage)));

        //                    //Make sure the drag tab is selected.
        //                    SelectedTab = drag_tab;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            e.Effect = DragDropEffects.None;
        //        }
        //    }
        //    finally
        //    {
        //        bDragingOver = false;
        //    }

        //}

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            Point pt = new Point(e.X, e.Y);
            TabPage tp = GetTabPageByTab(pt);

            if (e.Button == MouseButtons.Left)
            {
                //Point pt = new Point(e.X, e.Y);
                //TabPage tp = GetTabPageByTab(pt);

                if (tp != null)
                {
                    DoDragDrop(tp, DragDropEffects.All);
                }
            }
        }

        private TabPage GetTabPageByTab(Point pt)
        {
            for (int i = 0; i < TabPages.Count; i++)
            {
                if (GetTabRect(i).Contains(pt))
                {
                    return TabPages[i];
                }
            }
            return null;
        }


        private int FindIndex(TabPage page)
        {
            for (int i = 0; i < TabPages.Count; i++)
            {
                if (TabPages[i] == page)
                    return i;
            }
            return -1;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            _pt = new Point(e.X, e.Y);

            for (int i = 0; i < TabPages.Count-1; i++)
            {
                GetButton(TabPages[i]).Visible =
                (GetTabRect(i).Contains(_pt) || (SelectedIndex == i));
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            _pt = new Point(-1, -1);

            for (int i = 0; i < TabPages.Count - 1; i++)
            {
                GetButton(TabPages[i]).Visible = (SelectedIndex == i);
            }
        }

    }

}
