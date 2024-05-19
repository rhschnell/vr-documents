using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;

/// <summary>
/// This class is responsible for converting a PDF file to an image.
/// It is a class from github that uses GhostScript to convert the PDF.
/// https://gist.github.com/kalucky0/0eeb5bc21a2ee91c20fb6e3b4ce9373f
/// </summary>
class ConvertPDF
{
    /// <summary>
    /// This method converts a PDF file to an image.
    /// </summary>
    /// <param name="inputFile">path to input</param>
    /// <param name="outputFile">path to output</param>
    /// <param name="firstPage">first page number</param>
    /// <param name="lastPage">last page number</param>
    /// <param name="deviceFormat">type of image</param>
    /// <param name="width">width image</param>
    /// <param name="height">height image</param>
    public void Convert(string inputFile, string outputFile, int firstPage, int lastPage, string deviceFormat, int width, int height)
    {
        if (!File.Exists(inputFile))
        {
            return;
        }

        int intCounter;
        var sArgs = this.GetGeneratedArgs(inputFile, outputFile, firstPage, lastPage, deviceFormat, width, height);

        var intElementCount = sArgs.Length;
        var aAnsiArgs = new object[intElementCount];
        var aPtrArgs = new IntPtr[intElementCount];
        var aGCHandle = new GCHandle[intElementCount];

        for (intCounter = 0; intCounter < intElementCount; intCounter++)
        {
            aAnsiArgs[intCounter] = StringToAnsiZ(sArgs[intCounter]);
            aGCHandle[intCounter] = GCHandle.Alloc(aAnsiArgs[intCounter], GCHandleType.Pinned);
            aPtrArgs[intCounter] = aGCHandle[intCounter].AddrOfPinnedObject();
        }

        var gchandleArgs = GCHandle.Alloc(aPtrArgs, GCHandleType.Pinned);
        var intptrArgs = gchandleArgs.AddrOfPinnedObject();
        var intReturn = gsapi_new_instance(out var intGSInstanceHandle, this._objHandle);
        try
        {
            intReturn = gsapi_init_with_args(intGSInstanceHandle, intElementCount, intptrArgs);
        }
        catch (Exception)
        {
            // ignored
        }
        finally
        {
            for (intCounter = 0; intCounter < intReturn; intCounter++)
            {
                aGCHandle[intCounter].Free();
            }

            gchandleArgs.Free();
            gsapi_exit(intGSInstanceHandle);
            gsapi_delete_instance(intGSInstanceHandle);
        }
    }

    private static byte[] StringToAnsiZ(string str)
    {
        int intCounter;
        var intElementCount = str.Length;
        var aAnsi = new byte[intElementCount + 1];
        for (intCounter = 0; intCounter < intElementCount; intCounter++)
        {
            var bChar = (byte)str[intCounter];
            aAnsi[intCounter] = bChar;
        }

        aAnsi[intElementCount] = 0;
        return aAnsi;
    }

    private string[] GetGeneratedArgs(string inputFile, string outputFile, int firstPage, int lastPage, string deviceFormat, int width, int height)
    {
        this.OutputFormat = deviceFormat;
        this.ResolutionX = width;
        this.ResolutionY = height;

        var lstExtraArgs = new ArrayList();
        if (this.OutputFormat == "jpg" && this.JPEGQuality > 0 && this.JPEGQuality < 101)
        {
            lstExtraArgs.Add("-dJPEGQ=" + this.JPEGQuality);
        }

        if (this.Width > 0 && this.Height > 0)
        {
            lstExtraArgs.Add("-g" + this.Width + "x" + this.Height);
        }

        if (this.FitPage)
        {
            lstExtraArgs.Add("-dPDFFitPage");
        }

        if (this.ResolutionX > 0)
        {
            if (this.ResolutionY > 0)
            {
                lstExtraArgs.Add("-r" + this.ResolutionX + "x" + this.ResolutionY);
            }
            else
            {
                lstExtraArgs.Add("-r" + this.ResolutionX);
            }
        }

        var iFixedCount = 17;
        var iExtraArgsCount = lstExtraArgs.Count;
        var args = new string[iFixedCount + lstExtraArgs.Count];

        args[0] = "pdf2img";
        args[1] = "-dNOPAUSE";
        args[2] = "-dBATCH";
        args[3] = "-dPARANOIDSAFER";
        args[4] = "-sDEVICE=" + this.OutputFormat;
        args[5] = "-q";
        args[6] = "-dQUIET";
        args[7] = "-dNOPROMPT";
        args[8] = "-dMaxBitmap=500000000";
        args[9] = $"-dFirstPage={firstPage}";
        args[10] = $"-dLastPage={lastPage}";
        args[11] = "-dAlignToPixels=0";
        args[12] = "-dGridFitTT=0";
        args[13] = "-dTextAlphaBits=4";
        args[14] = "-dGraphicsAlphaBits=4";

        for (var i = 0; i < iExtraArgsCount; i++)
        {
            args[15 + i] = (string)lstExtraArgs[i];
        }

        args[15 + iExtraArgsCount] = $"-sOutputFile={outputFile}";
        args[16 + iExtraArgsCount] = $"{inputFile}";
        return args;
    }
#pragma warning disable SA1124 // Do not use regions
#pragma warning disable SA1204 // Static elements should appear before instance elements
#pragma warning disable SA1300 // Element should begin with upper-case letter
    #region GhostScript Import

    [DllImport("gsdll64.dll", EntryPoint = "gsapi_new_instance")]
    private static extern int gsapi_new_instance(out IntPtr pinstance, IntPtr caller_handle);

    [DllImport("gsdll64.dll", EntryPoint = "gsapi_init_with_args")]
    private static extern int gsapi_init_with_args(IntPtr instance, int argc, IntPtr argv);

    [DllImport("gsdll64.dll", EntryPoint = "gsapi_exit")]
    private static extern int gsapi_exit(IntPtr instance);

    [DllImport("gsdll64.dll", EntryPoint = "gsapi_delete_instance")]
    private static extern void gsapi_delete_instance(IntPtr instance);

    #endregion

    #region Variables

#pragma warning disable SA1201 // Elements should appear in the correct order
#pragma warning disable SA1309 // Field names should not begin with underscore
    private readonly IntPtr _objHandle;
#pragma warning restore SA1309 // Field names should not begin with underscore

    #endregion

    #region Proprieties

    private string OutputFormat { get; set; }

    private int Width { get; set; }

    private int Height { get; set; }

    private int ResolutionX { get; set; }

    private int ResolutionY { get; set; }

    private bool FitPage { get; set; }

    private int JPEGQuality { get; set; }

    #endregion

    #region Init

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertPDF"/> class.
    /// Constructor for the ConvertPDF class.
    /// </summary>
    /// <param name="objHandle">the handle</param>
    public ConvertPDF(IntPtr objHandle)
    {
        this._objHandle = objHandle;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertPDF"/> class.
    /// </summary>
    public ConvertPDF()
    {
        this._objHandle = IntPtr.Zero;
    }

    #endregion

#pragma warning restore SA1201 // Elements should appear in the correct order
#pragma warning restore SA1300 // Element should begin with upper-case letter
#pragma warning restore SA1204 // Static elements should appear before instance elements
#pragma warning restore SA1124 // Do not use regions
}