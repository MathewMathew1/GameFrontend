using System;
using System.IO;
using System.Windows;
using BoardGameFrontend.Windows;

namespace BoardGameFrontend
{
    public partial class InstructionsWindow : FullScreenWindow
    {
        public InstructionsWindow()
        {
            InitializeComponent();
            LoadPdf();
        }

        private void LoadPdf()
        {
            // Use a relative path based on the application’s directory
            var pdfPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pdfs", "Boardgame_CoF.pdf");

            if (File.Exists(pdfPath))
            {
                PdfWebViewer.Source = new Uri(pdfPath);
            }
            else
            {
                MessageBox.Show("PDF file not found at: " + pdfPath);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Owner?.Show(); // Show MainWindow again
        }
    }
}