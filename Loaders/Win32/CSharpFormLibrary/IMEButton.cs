using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CSharpFormLibrary
{
    public class IMEButton : System.Windows.Forms.Button
    {
        private const int MA_ACTIVATE = 1;
        private const int MA_ACTIVATEANDEAT = 2;
        private const int MA_NOACTIVATE = 0x0003;
        private const int MA_NOACTIVATEANDEAT = 0x0004;
        
        private UInt64 m_AppHWnd;
        private bool m_wasMouseDown = false;

        private Color m_colorTop;
        private Color m_colorMiddle;
        private Color m_colorBottom;
        private Color m_colorBorder;
        private Color m_colorText;

        private ButtonBorderStyle m_buttonBorderStyle;

        public IMEButton()
        {
            this.SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
            UpdateAppearance();
        }

        public UInt64 AppHWnd
        {
            get { return m_AppHWnd; }
            set { m_AppHWnd = value; }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle = 0x00000004; //WS_EX_NOPARENTNOTIFY
                return cp;
            }
        }

        private void UpdateAppearance()
        {
            if (m_wasMouseDown)
            {
                m_colorTop = Color.DimGray;
                m_colorMiddle = Color.DimGray;
                m_colorBottom = Color.Black;
                m_colorBorder = Color.Gray;
                m_colorText = Color.White;
                m_buttonBorderStyle = ButtonBorderStyle.Inset;
            }
            else
            {
                m_colorTop = Color.LightGray;
                m_colorMiddle = Color.DimGray;
                m_colorBottom = Color.Black;
                m_colorBorder = Color.Gray;
                m_colorText = Color.White;
                m_buttonBorderStyle = ButtonBorderStyle.Outset;
            }

        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            SizeF sizeF =
                new SizeF(
                    (float)ClientRectangle.Width,
                    (float)ClientRectangle.Height / 2.0f);

            RectangleF rectF = new RectangleF(new PointF(0.0f, 0.0f), sizeF);
            Brush b = new LinearGradientBrush(
                rectF, m_colorTop, m_colorMiddle,
                    LinearGradientMode.Vertical);
            pe.Graphics.FillRectangle(b, rectF);

            rectF = new RectangleF(
                new PointF(0.0f, sizeF.Height), sizeF);
            b = new System.Drawing.SolidBrush(m_colorBottom);
            pe.Graphics.FillRectangle(b, rectF);

            b = new SolidBrush(m_colorText);
            sizeF = pe.Graphics.MeasureString(Text, Font);
            PointF pt = new PointF(
                (Width - sizeF.Width) / 2.0f, (Height - sizeF.Height) / 2.0f);
            pe.Graphics.DrawString(Text, Font, b, pt);

            b.Dispose();
            ControlPaint.DrawBorder(
                pe.Graphics, ClientRectangle,
                    m_colorBorder, m_buttonBorderStyle);
        }

        protected void MyOnMouseDown()
        {
            m_wasMouseDown = true;
            UpdateAppearance();
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            if (m_wasMouseDown)
            {
                m_wasMouseDown = false;
                UpdateAppearance();
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == (Int32)UtilFuncs.WindowsMessage.WM_MOUSEACTIVATE)
            {
                m.Result = (IntPtr)MA_NOACTIVATEANDEAT;
                MyOnMouseDown();
            }
        }
    }
}
