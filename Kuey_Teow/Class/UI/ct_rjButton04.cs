using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GlassButtonExample
{
    public class GlassButton : Button
    {
        public GlassButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.BackColor = Color.Transparent;
            this.ForeColor = Color.White;
            this.Padding = new Padding(0);
            this.Margin = new Padding(0);
            this.Size = new Size(100, 40);
            this.Paint += new PaintEventHandler(GlassButton_Paint);
        }

        private void GlassButton_Paint(object sender, PaintEventArgs e)
        {
            // สร้างกราฟิกส์สำหรับปุ่ม
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // สร้างเส้นขอบและพื้นหลัง
            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
            using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.FromArgb(100, Color.White), Color.FromArgb(50, Color.White), LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, rect);
            }

            // สร้างเอฟเฟกต์กระจก
            Rectangle glassRect = new Rectangle(0, 0, this.Width, this.Height / 2);
            using (LinearGradientBrush glassBrush = new LinearGradientBrush(glassRect, Color.FromArgb(100, Color.White), Color.FromArgb(0, Color.White), LinearGradientMode.Vertical))
            {
                g.FillRectangle(glassBrush, glassRect);
            }

            // วาดข้อความ
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            g.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), rect, sf);
        }
    }

    
}