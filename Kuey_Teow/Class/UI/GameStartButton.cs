using System;
using System.Drawing;
using System.Windows.Forms;

public class GameStartButton : UserControl
{
    private bool isHovered = false;
    private bool isClicked = false; // ตัวแปรตรวจสอบการคลิก

    public GameStartButton()
    {
        this.Size = new Size(250, 80);
        this.Cursor = Cursors.Hand;
        this.DoubleBuffered = true;

        // ใช้ MouseEnter เปลี่ยนสีเฉยๆ
        this.MouseEnter += (s, e) => { isHovered = true; this.Invalidate(); };
        this.MouseLeave += (s, e) => { isHovered = false; this.Invalidate(); };

        // ต้องกดปุ่มก่อนถึงจะทำงาน
        this.MouseDown += (s, e) => { isClicked = true; };
        this.MouseClick += GameStartButton_Click;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

        Color borderColor = Color.Black;
        Color fillColor = isHovered ? Color.MidnightBlue : Color.Blue;
        Color textColor = Color.White;

        using (SolidBrush brush = new SolidBrush(fillColor))
        {
            g.FillRectangle(brush, 5, 5, this.Width - 10, this.Height - 10);
        }

        using (Pen pen = new Pen(borderColor, 5))
        {
            g.DrawRectangle(pen, 5, 5, this.Width - 10, this.Height - 10);
        }

        using (Font font = new Font("Arial", 20, FontStyle.Bold))
        using (SolidBrush textBrush = new SolidBrush(textColor))
        {
            StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("GAME START", font, textBrush, new Rectangle(5, 5, this.Width - 10, this.Height - 10), sf);
        }
    }

    private void GameStartButton_Click(object sender, MouseEventArgs e)
    {
        if (isClicked) // ตรวจสอบก่อนว่ามีการคลิกจริง
        {
            //MessageBox.Show("เกมเริ่มต้นแล้ว!", "Start Game", MessageBoxButtons.OK, MessageBoxIcon.Information);
            isClicked = false; // รีเซ็ตค่า
        }
    }
}
