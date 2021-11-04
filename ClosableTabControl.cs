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
            //System.Diagnostics.Debug.WriteLine($"_MouseDown {SelectedIndex} : {DateTime.Now.ToString("ss.fff")}");

            var lastIndex = TabCount - 1;
            if (GetTabRect(lastIndex).Contains(e.Location))
            {
                NewPage(sender);
            }
            else
            {
                for (var i = 0; i < TabPages.Count; i++)
                {
                    var tabRect = GetTabRect(i);
                    tabRect.Inflate(-1, -2);
                    var closeImage = Properties.Resources.Close;
                    var imageRect = new Rectangle(
                        tabRect.Right + 1 - closeImage.Width, tabRect.Y,
                        closeImage.Width, closeImage.Height);
                    if (imageRect.Contains(e.Location))
                    {
                        System.Diagnostics.Debug.WriteLine($"Antes Remover {i} : {SelectedIndex} : {DateTime.Now.ToString("ss.fff")}");
                        TabPages.RemoveAt(i);
                        System.Diagnostics.Debug.WriteLine($"Depois Remover {i} : {SelectedIndex} : {DateTime.Now.ToString("ss.fff")}");
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
                //System.Diagnostics.Debug.WriteLine($"_DrawItem TabIndex {TabPages.IndexOf(tabPage)} {DateTime.Now.ToString("ss.fff")} {sender} {e}");

                tabRect.Inflate(-1, -2);

                if (e.State == DrawItemState.Selected)
                {
                    e.Graphics.FillRectangle(new SolidBrush(SystemColors.Control), e.Bounds);

                    TextRenderer.DrawText(e.Graphics, $"{tabPage.Text}",
                        new Font(tabPage.Font, FontStyle.Bold),
                        tabRect, tabPage.ForeColor,
                        TextFormatFlags.Left);
                }
                else
                {
                    e.Graphics.FillRectangle(new SolidBrush(SystemColors.ButtonShadow), e.Bounds);

                    TextRenderer.DrawText(e.Graphics, $"{tabPage.Text}",
                        new Font(tabPage.Font, FontStyle.Regular),
                        tabRect, tabPage.ForeColor,
                        TextFormatFlags.Left);
                }

                var closeImage = Properties.Resources.Close;

                e.Graphics.DrawImage(closeImage,
                    tabRect.Right + 4 - closeImage.Width, tabRect.Y + 3,
                    closeImage.Width - 6, closeImage.Height - 6);
            }
        }

        public TabPage NewPage(object sender, string title = "Escolha uma tela")
        {
            var lastIndex = TabCount - 1;

            TabPages.Insert(lastIndex, title);
            SelectedIndex = lastIndex;
            TabPages[lastIndex].UseVisualStyleBackColor = true;


            //TabPages[lastIndex].MouseEnter += _MouseEnter;
            //TabPages[lastIndex].MouseHover += _MouseHover;
            //TabPages[lastIndex].MouseLeave += _MouseLeave;

            return TabPages[lastIndex];
        }
    }

}
