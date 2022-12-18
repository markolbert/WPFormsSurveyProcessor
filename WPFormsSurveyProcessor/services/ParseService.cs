using J4JSoftware.DeusEx;
using J4JSoftware.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NPOI.XSSF.UserModel;
using WpFormsSurvey;

namespace WPFormsSurveyProcessor;

internal class ParseService : ServiceBase
{
    private readonly ExportFormInfo _formInfoExporter;

    public ParseService(
        Configuration config,
        ExportFormInfo formInfoExporter,
        IJ4JLogger logger
    )
        : base(config, logger)
    {
        _formInfoExporter = formInfoExporter;

        ValidateExcelFilePath();
    }

    private void ValidateExcelFilePath()
    {
        if( Path.GetInvalidPathChars().Length != 0 )
        {
            Logger.Warning<string>(
                "Excel output file path '{0}' contains invalid characters, changing output file to ExcelOutput.xlsx in the current directory",
                Configuration.ExcelFilePath );

            Configuration.ExcelFilePath = Path.Combine( Environment.CurrentDirectory, "ExcelOutput.xlsx" );
            return;
        }

        var extension = Path.GetExtension( Configuration.ExcelFilePath ).ToLower();
        if( extension != ".xlsx" )
        {
            Logger.Warning<string>("Unsupported or invalid file extension '{0}' changed to .xlsx", extension  );
            Configuration.ExcelFilePath = $"{Path.GetFileNameWithoutExtension(Configuration.ExcelFilePath)}{extension}";
        }

        //see if the file can be created where specified
        var fileDir = Path.GetDirectoryName( Configuration.ExcelFilePath ) ?? Environment.CurrentDirectory;
        var testFile = Path.Combine( fileDir, Guid.NewGuid().ToString() );

        var canCreate = false;

        try
        {
            using( File.Create( testFile ) )
            {
                File.Delete( testFile );
                canCreate = true;
            }
        }
        // ReSharper disable once EmptyGeneralCatchClause
        catch
        {
        }

        if( canCreate )
            return;

        Logger.Warning( "{0} is inaccessible, changing output file to ExcelOutput.xlsx in the current directory" );

        Configuration.ExcelFilePath = Path.Combine( Environment.CurrentDirectory, "ExcelOutput.xlsx" );
    }

    public override Task StartAsync( CancellationToken cancellationToken )
    {
        var parser = J4JDeusEx.ServiceProvider.GetRequiredService<WpFormsParser>();

        var forms = parser.ParseFile(Configuration.PostsFilePath);
        if (forms?.Table == null)
        {
            Logger.Error("Failed to parse WordPress posts file");
            return Task.CompletedTask;
        }

        var workbook = new XSSFWorkbook(XSSFWorkbookType.XLSX);

        _formInfoExporter.Initialize(workbook, forms.Table);

        _formInfoExporter.ExportData();

        using( var fileStream = File.Create( Configuration.ExcelFilePath ) )
        {
            workbook.Write(fileStream);
            fileStream.Close();
        }

        return Task.CompletedTask;
    }
}
