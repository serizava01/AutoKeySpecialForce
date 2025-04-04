using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

public class ScreenCapture
{
    private Rectangle selectionRectangle;
    private Point startPoint;
    private bool isSelecting = false;
    private Form overlayForm;
    private Bitmap screenshot;

    public string CaptureSelectedArea()
    {

        screenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        using (Graphics graphics = Graphics.FromImage(screenshot))
        {
            graphics.CopyFromScreen(Point.Empty, Point.Empty, screenshot.Size);
        }


        overlayForm = new Form
        {
            FormBorderStyle = FormBorderStyle.None,
            Bounds = Screen.PrimaryScreen.Bounds,
            BackColor = Color.White,
            Opacity = 0.3,
            TopMost = true,
            Cursor = Cursors.Cross,

        };

        overlayForm.MouseDown += Overlay_MouseDown;
        overlayForm.MouseMove += Overlay_MouseMove;
        overlayForm.MouseUp += Overlay_MouseUp;
        overlayForm.Paint += Overlay_Paint;
        overlayForm.ShowDialog();


        if (selectionRectangle.Width > 0 && selectionRectangle.Height > 0)
        {
            using (Bitmap croppedImage = screenshot.Clone(selectionRectangle, screenshot.PixelFormat))
            {
                Bitmap processedImage = ConvertToGrayscale(croppedImage);
                string tempPath = Path.Combine(Path.GetTempPath(), "ocr_temp.png");
                processedImage.Save(tempPath, ImageFormat.Png);
                processedImage.Dispose();
                return tempPath;
            }
        }

        return null;
    }

    private void Overlay_MouseDown(object sender, MouseEventArgs e)
    {
        isSelecting = true;
        startPoint = e.Location;
    }

    private void Overlay_MouseMove(object sender, MouseEventArgs e)
    {
        if (isSelecting)
        {
            selectionRectangle = new Rectangle(
                Math.Min(startPoint.X, e.X),
                Math.Min(startPoint.Y, e.Y),
                Math.Abs(e.X - startPoint.X),
                Math.Abs(e.Y - startPoint.Y)
            );

            overlayForm.Invalidate();
        }
    }

    private void Overlay_MouseUp(object sender, MouseEventArgs e)
    {
        isSelecting = false;
        overlayForm.Close();
    }

    private void Overlay_Paint(object sender, PaintEventArgs e)
    {
        if (isSelecting)
        {
            using (Pen pen = new Pen(Color.Red, 2))
            {
                e.Graphics.DrawRectangle(pen, selectionRectangle);
            }
        }
    }

    /// <summary>
    /// OCR Setting
    /// </summary>
    private Bitmap ConvertToGrayscale(Bitmap source)
    {
        Bitmap grayscaleBitmap = new Bitmap(source.Width, source.Height);
        using (Graphics g = Graphics.FromImage(grayscaleBitmap))
        {
            ColorMatrix colorMatrix = new ColorMatrix(new float[][]
            {
                new float[] {0.3f, 0.3f, 0.3f, 0, 0},
                new float[] {0.59f, 0.59f, 0.59f, 0, 0},
                new float[] {0.11f, 0.11f, 0.11f, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1}
            });

            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(colorMatrix);

            g.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height),
                0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attributes);
        }
        return grayscaleBitmap;
    }
}
