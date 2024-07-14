using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

Console.WriteLine("PDF Merge Tools v0.9b");
List<PdfDocument> sources = new List<PdfDocument>();
var outputPath = string.Empty;
foreach (string fn in args)
{
    if (fn.StartsWith("-o:"))
    {
        outputPath = fn.Substring(3);
        continue;
    }
    if (fn.Contains("*"))
    {
        var folder = Path.GetDirectoryName(fn);
        if (string.IsNullOrEmpty(folder)) folder = ".";
        foreach (var found in Directory.GetFiles(folder, Path.GetFileName(fn), SearchOption.AllDirectories))
        {
            Console.WriteLine($" * {found}");
            var srcPdf = PdfReader.Open(found, PdfDocumentOpenMode.Import);
            sources.Add(srcPdf);
        }
    }
    else if (!File.Exists(fn))
    {
        Console.WriteLine("Can't find " + fn);
        return;
    }
    else
    {
        Console.WriteLine(" * " + fn);
        var srcPdf = PdfReader.Open(fn, PdfDocumentOpenMode.Import);
        sources.Add(srcPdf);
    }
}
if (sources.Count == 0)
{
    Console.WriteLine("No pdf to merge");
    return;
}
var merged = new PdfDocument();
foreach (var pdf in sources)
{
    for (int i = 0; i < pdf.PageCount; i++)
    {
        merged.AddPage(pdf.Pages[i]);
    }
}
var mergedFilePath = $"{DateTime.Now:yyyyMMdd-HHmmss}.pdf";
if (!string.IsNullOrEmpty(outputPath))
{
    mergedFilePath = outputPath;
}
else
{
    Console.Write($"Output filename [default: {mergedFilePath}]: "); 
    var assignFilePath = Console.ReadLine();
    if (assignFilePath == null) return; // user press Ctrl+C or Ctrl-Z
    Console.WriteLine("assignFilePath: " + assignFilePath);
    if (!string.IsNullOrEmpty(assignFilePath))
    {
        if (!assignFilePath.EndsWith(".pdf")) assignFilePath += ".pdf";
        mergedFilePath = assignFilePath;
    }
}
Console.WriteLine($"Saving to {mergedFilePath}");
merged.Save(mergedFilePath);