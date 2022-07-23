namespace System.Printing;

public class PrintJobTallComponents : PrintJob
{
    // set this to true to rasterize images to a file, then print images
    // set this to false to directly rasterize to the Graphics object (seems to be better quality output, but more complex code)
    private bool m_rasterizeToFile = false;

    private int m_currentPage = 0;
    private int m_currentDocIndex = 0;

    internal PrintJobTallComponents(Printer printer, string jobName)
        : base(printer, jobName)
    {
    }

    //protected override void OnDocumentPrintPage(object o, Sdp.PrintPageEventArgs e)
    //{
    //    throw new NotImplementedException();
    //    //System.Diagnostics.Debug.WriteLine(o.ToString());
    //    //if (m_rasterizeToFile)
    //    //{
    //    //    // RasterizeDocuments was called and did work, so use the base implementation
    //    //    base.OnDocumentPrintPage(o, e);
    //    //    return;
    //    //}

    //    //if (m_currentDocIndex >= SourceDocumentPaths.Count)
    //    //{
    //    //    // this shouldn't occur, but handle it anyway
    //    //    return;
    //    //}

    //    //using (var fs = new FileStream(SourceDocumentPaths[m_currentDocIndex], FileMode.Open, FileAccess.Read))
    //    //{

    //    //    var doc = new Document(fs);

    //    //    var page = doc.Pages[m_currentPage];

    //    //    e.Graphics.MultiplyTransform(GetPrintTransformation(PrintDocument, page.Width, page.Height, e.Graphics.VisibleClipBounds));

    //    //    // TODO:
    //    //    doc.Pages[m_currentPage].Draw(e.Graphics);
    //    //    e.Graphics.Flush();

    //    //    e.HasMorePages = false;
    //    //    if (m_currentPage < doc.Pages.Count - 1)
    //    //    {
    //    //        m_currentPage++;
    //    //        e.HasMorePages = true;
    //    //    }
    //    //    else if(m_currentDocIndex < SourceDocumentPaths.Count -1)
    //    //    {
    //    //        m_currentDocIndex++;
    //    //        m_currentPage = 0;
    //    //        e.HasMorePages = true;
    //    //    }
    //    //}
    //}

    protected override string[] RasterizeDocuments(string outputFolder, string outputDocumentName, params string[] inputFiles)
    {
        if (m_rasterizeToFile)
        {
            return DoRasterizeDocuments(outputDocumentName, outputDocumentName, inputFiles);
        }

        // nop - we'll handle printing directly in OnDocumentPrintPage
        return null;
    }

    private string[] DoRasterizeDocuments(string outputFolder, string outputDocumentName, params string[] inputFiles)
    {

        throw new NotImplementedException();
        //var destPage = 1;
        //var names = new List<string>();

        //foreach (var src in SourceDocumentPaths)
        //{
        //    using (var fs = new FileStream(src, FileMode.Open, FileAccess.Read))
        //    {
        //        var m_doc = new TallComponents.PDF.Rasterizer.Document(fs);

        //        for (int i = 0; i < m_doc.Pages.Count; i++)
        //        {
        //            var destName = Path.Combine(outputFolder, $"{outputDocumentName}_{destPage++}.tif");
        //            names.Add(destName);
        //            using (var f = File.Create(destName))
        //            {
        //                m_doc.Pages[i].ConvertToTiff(f, new ConvertToTiffOptions(200, TiffCompression.PackBits, false));
        //            }
        //        }
        //    }
        //}

        //return names.ToArray();
    }
}