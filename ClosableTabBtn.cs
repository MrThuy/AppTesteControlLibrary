using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AppTesteControlLibrary
{
    public delegate Form OpenFormEventHandler();

    //class TaPageInfo
    //{
    //    TabPage tab;
    //    String  title;
    //}

    [ToolboxBitmap(typeof(TabControl))]
    public class ClosableTabBtn : TabControl
    {
        public ClosableTabBtn() : base()
        {

            Padding = new Point(12, 4);

            MouseDown += _MouseDown;
            Selecting += _Selecting;
            HandleCreated += _HandleCreated;
            AllowDrop = true;
            //Multiline = true;
        }

        private Dictionary<Button, TabPage> dicButtons = new Dictionary<Button, TabPage>();

        private bool blnShow = true;
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

        //private Image imgImage;
        //[Browsable(true)]
        //[DefaultValue(true)]
        //[Category("Appearance")]
        //[Description("Close Image")]
        //public Image TabPageImage
        //{
        //    get { return imgImage; }
        //    set { imgImage = value; }
        //}
        
        //Evento ao clicar nas setas de navegação quando tem muitas abas
        const int WM_HSCROLL = 0x0114;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_HSCROLL)
            {
                Repos();
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            Repos();
        }

        private Size btnSize(Rectangle rtCurrent)
        {
            return new Size(19,19);
        }

        private Point btnLocation(Rectangle rct)
        {
            return new Point(rct.X + rct.Width - 23,
                             rct.Y + (rct.Height/2) - (18/2) );
        }


        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            TabPage tpCurrent = (TabPage)e.Control;

            var _btn = GetButton(tpCurrent);
            if (_btn != null)
            {
                dicButtons.Remove(_btn);
                _btn.Dispose();
            }

            if (TabPages.Count == 1)
            {
                Rectangle rctCurrent = this.GetTabRect(this.TabPages.IndexOf(tpCurrent));

                Button btnAdd = new Button();
                btnAdd.FlatStyle = FlatStyle.Flat;
                btnAdd.FlatAppearance.BorderSize = 0;

                btnAdd.Image = Properties.Resources.Add;
                btnAdd.ImageAlign = ContentAlignment.MiddleCenter;
                btnAdd.TextAlign = ContentAlignment.MiddleLeft;
                btnAdd.Size = new Size(19,18);
                btnAdd.Location = btnLocation(rctCurrent);

                SetParent(btnAdd.Handle, this.Handle);

                btnAdd.Click += OnAddClick;

                dicButtons.Add(btnAdd, tpCurrent);

                tpCurrent.Text = "";
            }
            else {
                int index_Location = TabPages.IndexOf(tpCurrent);
                bool is_LastPage = index_Location == (TabPages.Count - 1);

                if (is_LastPage)
                {
                    var _LastPage = TabPages[TabPages.Count - 2];
                    TabPages[TabPages.Count - 2] = tpCurrent;
                    TabPages[TabPages.Count - 1] = _LastPage;
                }   
                Rectangle rctCurrent = this.GetTabRect(index_Location);

                Button btnClose = new RoundButton();
                btnClose.FlatStyle = FlatStyle.Flat;
                btnClose.FlatAppearance.BorderSize = 0;

                btnClose.Image = Properties.Resources.Close;
                btnClose.ImageAlign = ContentAlignment.MiddleCenter;
                btnClose.TextAlign = ContentAlignment.MiddleLeft;
                btnClose.Size = btnSize(rctCurrent);
                btnClose.Location = new Point(rctCurrent.X + rctCurrent.Width - 21,
                                              rctCurrent.Y + 1);

                //System.Diagnostics.Debug.WriteLine($"OnControlAdded Size:{btnClose.Size} Location {btnClose.Location} {DateTime.Now.ToString("ss.fff")}");

                SetParent(btnClose.Handle, this.Handle);

                btnClose.Click += OnCloseClick;
                btnClose.MouseEnter += OnMouseEnterBtn;

                dicButtons.Add(btnClose, tpCurrent);

                tpCurrent.Text = tpCurrent.Text.Trim() + "    ";
            }
        }

        private void OnMouseEnterBtn(object sender, EventArgs e)
        {            
            Button btnClose = (Button)sender;
            btnClose.Image = Properties.Resources.Close;
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);

            var _btn = GetButton((TabPage)e.Control);
            if (_btn != null) { 
                dicButtons.Remove(_btn);
                _btn.Dispose();
            }
            Repos();
        }

        protected override void OnLayout(LayoutEventArgs lea)
        {
            base.OnLayout(lea);
            Repos();
        }

        public event CancelEventHandler CloseClickEvent;
        public event EventHandler AfterCloseClickEvent;
        protected virtual void OnCloseClick(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                Button btnClose = (Button)sender;
                TabPage tpCurrent = dicButtons[btnClose];

                CancelEventArgs cea = new CancelEventArgs();

                CloseClickEvent?.Invoke(sender, cea);

                if (!cea.Cancel)
                {
                    if (TabPages.Count > 1)
                    {
                        TabPages.Remove(tpCurrent);
                        dicButtons.Remove(btnClose);
                        btnClose.Dispose();
                        Repos();
                        AfterCloseClickEvent?.Invoke(sender, e);
                    };
                }
            }
        }

        protected virtual void OnAddClick(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                NewPage();
            }
        }

        public void Repos()
        {
            foreach (var btn in dicButtons)
                Repos(btn.Value);
        }

        public void Repos(TabPage tpCurrent)
        {
            Button btn = GetButton(tpCurrent);

            if (btn != null)
            {
                int tpIndex = TabPages.IndexOf(tpCurrent);

                if (tpIndex >= 0)
                {
                    Rectangle rctCurrent = GetTabRect(tpIndex);

                    bool _LastPage = (tpCurrent == TabPages[TabCount - 1]);

                    if (_LastPage)
                    {
                        btn.Location = btnLocation(rctCurrent);
                    }
                    else
                    {
                        btn.Location = new Point(rctCurrent.X +
                                                        rctCurrent.Width - 21,
                                                        rctCurrent.Y + 1);
                    }
                        
                    btn.Visible = ShowButtonClose;
                    btn.BringToFront();
                    HoverBtn(tpCurrent);
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
                NewPage();
            }
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);

            if (!(drgevent.Data.GetDataPresent(typeof(TabPage))))
                drgevent.Effect = DragDropEffects.None;
            else
            {
                TabPage DropTab = (TabPage)(drgevent.Data.GetData(typeof(TabPage)));

                Point pt = new Point(drgevent.X, drgevent.Y);

                pt = PointToClient(pt);

                TabPage hover_tab = GetTabPageByTab(pt);
                if (hover_tab == null)
                {
                    drgevent.Effect = DragDropEffects.None;
                    return;
                }

                int drop_location_index = TabPages.IndexOf(hover_tab);
                int src_location_index = TabPages.IndexOf(DropTab);

                if (drop_location_index == src_location_index)
                    drgevent.Effect = DragDropEffects.None;
                else if (drop_location_index >= TabCount - 1)
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

            if (drgevent.Effect == DragDropEffects.Move)
            {
                TabPage DropTab = (TabPage)(drgevent.Data.GetData(typeof(TabPage)));

                Point pt = new Point(drgevent.X, drgevent.Y);

                pt = PointToClient(pt);

                TabPage hover_tab = GetTabPageByTab(pt);
                int drop_location_index = TabPages.IndexOf(hover_tab);
                int src_location_index = TabPages.IndexOf(DropTab);
                var pages = new System.Collections.ArrayList();

                //Put all tab pages into an array.
                for (int i = 0; i < TabPages.Count; i++)
                {
                    //Except the one we are dragging.
                    if (i != src_location_index)
                        pages.Add(TabPages[i]);
                }

                //Now put the one we are dragging it at the proper location.
                pages.Insert(drop_location_index, DropTab);

                for (int i = 0; i < TabPages.Count; i++)
                {
                    TabPages[i] = (TabPage)pages[i];
                }

                SelectedIndex = drop_location_index;

                Repos();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            Point pt = new Point(e.X, e.Y);
            TabPage tp = GetTabPageByTab(pt);

            if (e.Button == MouseButtons.Left)
            {
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

        private Point _MousePosition = new(0, 0);
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            _MousePosition = new Point(e.X, e.Y);
            HoverBtn();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _MousePosition = new Point(-1, -1);
            HoverBtn();
        }

        private void HoverBtn()
        {
            if (!ShowButtonClose) return;

            for (int i = 0; i < TabPages.Count - 1; i++)
            {
                HoverBtn(TabPages[i]);
            }
        }

        private void HoverBtn(TabPage page)
        {
            if (!ShowButtonClose) return;
            var btn = GetButton(page);
            var index = TabPages.IndexOf(page);
            var tabRect = GetTabRect(index);

            if (index == (TabPages.Count - 1))
            {
                btn.Image = Properties.Resources.Add;
            } 
            else if (tabRect.Contains(_MousePosition) || (SelectedIndex == index))
            {
                btn.Image = Properties.Resources.Close;
            }
            else
            {
                btn.Image = null;
            }            
        }

        public event OpenFormEventHandler OpenFormEvent;
        public void NewPage(Form aForm = null)
        {
            if (aForm == null)
            {
                aForm = OpenFormEvent?.Invoke();
            }

            if (aForm == null)
                NewBlankPage();
            else
               OpenFormInNewPage(aForm);
        }

        public TabPage NewBlankPage()
        {
            var lastIndex = TabCount - 1;

            TabPages.Insert(lastIndex, $"TabPage { lastIndex + 1}");
            SelectedIndex = lastIndex;
            TabPages[lastIndex].UseVisualStyleBackColor = true;
            return TabPages[lastIndex];
        }

        public TabPage InsertPage(TabPage tab)
        {
            var lastIndex = TabCount - 1;

            TabPages.Insert(lastIndex, tab);
            SelectedIndex = lastIndex;
            TabPages[lastIndex].UseVisualStyleBackColor = true;
            return TabPages[lastIndex];
        }

        public Form OpenFormInPage(Form aForm, TabPage aPage = null)
        {
            if (aForm == null) return aForm;
            if (aPage == null) aPage = TabPages[SelectedIndex];

            //TopLevel for form is set to false
            aForm.TopLevel = false;
            aForm.Dock = DockStyle.Fill;
            aForm.FormBorderStyle = FormBorderStyle.None;
            aPage.Text = aForm.Text + "    ";
            aPage.Controls.Clear();
            aPage.Controls.Add(aForm);            
            aForm.Show();
            Refresh();
            Repos();
            return aForm;
        }

        public Form OpenFormInNewPage(Form aForm)
        {
            return OpenFormInPage(aForm, NewBlankPage());            
        }
    }
}
