using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class GlowingButton : Button
{
    private Timer glowTimer;
    private float glowX = 0;
    private float glowY = 0;
    private bool movingRight = true;
    private bool movingDown = true;

    public GlowingButton()
    {
        this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
        this.Font = new Font("Consolas", 18, FontStyle.Bold);
        this.ForeColor = Color.White;
        this.BackColor = Color.Black;
        this.FlatStyle = FlatStyle.Flat;
        this.FlatAppearance.BorderSize = 2;
        this.FlatAppearance.BorderColor = Color.DarkGray;
        this.Size = new Size(200, 70); // ขนาดของปุ่ม

        // ตั้งค่า Timer ให้แสงเคลื่อนที่
        glowTimer = new Timer();
        glowTimer.Interval = 30; // กำหนดความเร็วของการเคลื่อนที่
        glowTimer.Tick += (s, e) =>
        {
            if (movingRight)
                glowX += 2; // เลื่อนไปทางขวา
            else
                glowX -= 2; // เลื่อนไปทางซ้าย

            if (movingDown)
                glowY += 2; // เลื่อนไปด้านล่าง
            else
                glowY -= 2; // เลื่อนไปด้านบน

            // เปลี่ยนทิศทางเมื่อถึงขอบของปุ่ม
            if (glowX > this.Width - 10) movingRight = false;
            if (glowX < 0) movingRight = true;

            if (glowY > this.Height - 10) movingDown = false;
            if (glowY < 0) movingDown = true;

            this.Invalidate(); // รีเฟรชปุ่มเพื่อแสดงผล
        };
        glowTimer.Start();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        Rectangle rect = this.ClientRectangle;

        // วาดพื้นหลังปุ่ม
        using (Brush bgBrush = new SolidBrush(Color.FromArgb(50, Color.Gray)))
        {
            g.FillRectangle(bgBrush, rect);
        }

        // วาดแสงที่เคลื่อนที่จากซ้ายไปขวาและบนไปล่าง
        using (Pen glowPen = new Pen(Color.FromArgb(100, Color.Cyan), 2))
        {
            glowPen.Alignment = PenAlignment.Center;
            g.DrawLine(glowPen, new PointF(glowX, glowY), new PointF(glowX + 20, glowY + 20)); // เคลื่อนที่ในแนวทแยง
        }

        // วาดข้อความในปุ่ม
        TextRenderer.DrawText(g, this.Text, this.Font, rect, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }
}
