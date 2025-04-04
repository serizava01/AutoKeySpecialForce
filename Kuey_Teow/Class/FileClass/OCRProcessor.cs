using System;
using System.IO;
using Tesseract;
using System.Drawing;

public class OCRProcessor
{
    string LoadDLL = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");

    public string ExtractTextFromImage(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
            return "ไม่พบไฟล์ภาพ";

        string extractedText = "";

        try
        {
           
            using (var image = new Bitmap(imagePath))
            {
                using (var grayscale = ConvertToGrayscale(image))
                {
                    string tempFile = Path.Combine(Path.GetTempPath(), "processed_image.png");
                    grayscale.Save(tempFile);

                    
                    using (var engine = new TesseractEngine(LoadDLL, "eng", EngineMode.Default))
                    {
                        using (var img = Pix.LoadFromFile(tempFile))
                        {
                            engine.DefaultPageSegMode = PageSegMode.Auto;  
                            using (var page = engine.Process(img))
                            {
                                extractedText = page.GetText();
                            }
                        }
                    }

                    
                    File.Delete(tempFile);
                }
            }

            File.Delete(imagePath);
        }
        catch (Exception ex)
        {
            extractedText = "เกิดข้อผิดพลาด: " + ex.Message;
        }

        return extractedText;
    }

    
    private Bitmap ConvertToGrayscale(Bitmap original)
    {
        var grayscale = new Bitmap(original.Width, original.Height);
        for (int y = 0; y < original.Height; y++)
        {
            for (int x = 0; x < original.Width; x++)
            {
                Color pixelColor = original.GetPixel(x, y);
                int grayValue = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);
                grayscale.SetPixel(x, y, Color.FromArgb(grayValue, grayValue, grayValue));
            }
        }
        return grayscale;
    }
}
