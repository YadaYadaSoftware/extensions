namespace System.Printing;

public abstract class PrintJob : IDisposable
{
    public event EventHandler Started;
    public event EventHandler Completed;

    public const double DefaultPdfDensityScale = 2.0;

    private Printer m_printer;

    private NativePrintJob m_nativeJob;

    private int m_currentPage = 0;
    private int m_pagesToPrint = 0;
    private string[] m_printImages;

    //protected PrintDocument PrintDocument { get; private set; } = new PrintDocument();
    protected List<string> SourceDocumentPaths { get; private set; } = new List<string>();

    public bool IsStarted { get; private set; }
    public bool IsComplete { get; private set; }
    public string Status { get; protected set; }

    public PrintFitMode FitMode { get; set; }
    public PrintRotate Rotation { get; set; }
    public Action<Printer, PrintJob> PrePrintAction { get; set; }
    public Action<Printer, PrintJob> PostPrintAction { get; set; }

    public double PdfDensityScale { get; set; } = DefaultPdfDensityScale;



    internal PrintJob(Printer printer, string jobName)
    {
        throw new NotImplementedException();
        //m_printer = printer;
        //PrintDocument.PrintController = new Sdp.StandardPrintController();

        //if (jobName != null)
        //{
        //    PrintDocument.DocumentName = jobName;
        //}

        //var settings = PrintDocument.PrinterSettings;
        //settings.Copies = 1;

        //PrintDocument.PrintPage += OnDocumentPrintPage;

        //PrintDocument.PrinterSettings.PrinterName = m_printer.Name;

        //FitMode = PrintFitMode.Stretch;
    }

    public virtual void Dispose()
    {
        //if (PrintDocument != null)
        //{
        //    PrintDocument.Dispose();
        //    PrintDocument = null;
        //}

        if (m_printImages != null)
        {
            foreach (var file in m_printImages)
            {
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    // nothing to really do, we're just trying to clean up
                }
            }
            m_printImages = null;
        }
    }

    public PrintJob SetImage(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("File not found: " + path);
        }
        SourceDocumentPaths.Add(path);
        return this;
    }

    public PrintJob SetImage(params string[] paths)
    {
        foreach (var path in paths)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"File not found: {path}");
            }
        }

        SourceDocumentPaths.AddRange(paths);
        return this;
    }

    protected abstract string[] RasterizeDocuments(string outputFolder, string outputDocumentName, params string[] inputFiles);

    /// <summary>
    /// Rasterizes all documents to be printed to a series of image files that will in turn be sent to the print spooler
    /// </summary>
    /// <returns></returns>
    public PrintJob Enqueue()
    {
        // if we need to know when it is done (i.e. there's a post action) we need to hook up to the OS print spooler.  
        // The "AfterPrint" event of the PrintDocument is only "after it's sent to the spooler"
        var now = DateTime.Now;

        try
        {
            m_currentPage = 0;
            m_pagesToPrint = 0;

            m_printImages = RasterizeDocuments(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                Guid.NewGuid().ToString(),
                SourceDocumentPaths.ToArray());

            throw new NotImplementedException();

            //PrintDocument.DefaultPageSettings.PaperSize = new Sdp.PaperSize("4x6", 400, 600);
            //PrintDocument.DefaultPageSettings.Margins = new Sdp.Margins(0, 0, 0, 0);
            //PrintDocument.Print();
        }
        catch (Exception ex)
        {
            Status = ex.Message;
            return null;
        }

        return this;
    }

    //protected virtual void OnDocumentPrintPage(object o, Sdp.PrintPageEventArgs e)
    //{

    //    if (m_currentPage > 1) Debugger.Break();

    //    using (var img = Bitmap.FromFile(m_printImages[m_currentPage]))
    //    {
    //        e.Graphics.MultiplyTransform(GetPrintTransformation(PrintDocument, img.Width, img.Height, e.Graphics.VisibleClipBounds));
    //        e.Graphics.DrawImage(img, 0, 0);
    //        e.Graphics.Flush();

    //        if (m_currentPage < m_printImages.Length - 1)
    //        {
    //            m_currentPage++;
    //            e.HasMorePages = true;
    //        }
    //    }
    //}

    //protected static Matrix GetPrintTransformation(Sdp.PrintDocument document, double imageWidth, double imageHeight, RectangleF printableArea)
    //{
    //    Matrix transformation = new Matrix();

    //    var pdfRatio = imageWidth / imageHeight;

    //    //Sdp.PaperSize paperSize = document.PrinterSettings.PaperSizes[0];
    //    Sdp.PaperSize paperSize = new Sdp.PaperSize("4x6", 400, 600);

    //    float paperWidth = ((float)paperSize.Width * 72f) / 100f;
    //    float paperHeight = ((float)paperSize.Height * 72f) / 100f;

    //    // It is possible that a driver returns bogus data for the printable area. If that is
    //    // obviously the case, we are going to ignore the usePrintableArea settings (otherwise
    //    // we get invalid transforms).
    //    paperWidth = printableArea.Width;
    //    paperHeight = printableArea.Height;
    //    transformation.Translate(printableArea.Left, printableArea.Top);

    //    float paperRatio = paperWidth / paperHeight;

    //    var scaledPdfWidth = imageWidth;
    //    var scaledPdfHeight = imageHeight;

    //    bool rotate = false;
    //    if ((pdfRatio > 1 && paperRatio < 1) || (pdfRatio < 1 && paperRatio > 1))
    //    {
    //        // need to rotate
    //        rotate = true;
    //        var temp = scaledPdfWidth;
    //        scaledPdfWidth = scaledPdfHeight;
    //        scaledPdfHeight = temp;
    //        pdfRatio = 1 / pdfRatio;
    //    }

    //    var scale = 1d;
    //    if (pdfRatio > paperRatio)
    //        scale = paperWidth / scaledPdfWidth;
    //    else
    //        scale = paperHeight / scaledPdfHeight;

    //    scaledPdfWidth *= scale;
    //    scaledPdfHeight *= scale;

    //    // auto-rotate
    //    if (rotate)
    //    {
    //        transformation.Rotate(-90);
    //        transformation.Translate((float)-scaledPdfHeight, 0);
    //    }

    //    // scale
    //    transformation.Scale((float)scale, (float)scale);

    //    return transformation;
    //}

}