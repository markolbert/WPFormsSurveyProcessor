using J4JSoftware.Logging;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace WPFormsSurveyProcessor;

public abstract class ExportBase<TEntity>
    where TEntity : class
{
    private int _rowNum;
    private IRow? _curRow;
    private int _colNum;
    private ICell? _curCell;
    private int _numCols;

    protected ExportBase(
        IJ4JLogger logger
    )
    {
        Logger = logger;
        Logger.SetLoggedType( GetType() );

        //Styles.SetDataFormat("EvenFixed0", "###0_);(###0)");
        //Styles.SetDataFormat( "EvenAccounting0", "#,##0_);(#,##0)" );
        //Styles.SetDataFormat("EvenAccounting2", "#,##0.00_);(#,##0.00)");
        //Styles.SetDataFormat( "EvenPercent0", "#,##0%_);(#,##0%)" );
        //Styles.SetDataFormat( "EvenShortDate", "mm/dd/yyyy" );

        //Styles.SetDataFormat("OddFixed0", "###0_);(###0)");
        //Styles.SetDataFormat("OddAccounting0", "#,##0_);(#,##0)");
        //Styles.SetDataFormat("OddAccounting2", "#,##0.00_);(#,##0.00)");
        //Styles.SetDataFormat("OddPercent0", "#,##0%_);(#,##0%)");
        //Styles.SetDataFormat("OddShortDate", "mm/dd/yyyy");
        //Styles.SetFillForegroundColor("OddFixed0", IndexedColors.PaleBlue);
        //Styles.SetFillForegroundColor("OddAccounting0", IndexedColors.PaleBlue);
        //Styles.SetFillForegroundColor( "OddAccounting2", IndexedColors.PaleBlue );
        //Styles.SetFillForegroundColor("OddPercent0", IndexedColors.PaleBlue);
        //Styles.SetFillForegroundColor("OddShortDate", IndexedColors.PaleBlue);
        
        //Styles.SetBottomBorder( "ColumnHeader", BorderStyle.Medium );
        //Styles.MakeBold( "ColumnHeader" );
        //Styles.SetHorizontalAlignment( "ColumnHeader", HorizontalAlignment.Center );
        //Styles.SetWordWrap( "ColumnHeader" );
        
        //Styles.SetFillForegroundColor( "OddRow", IndexedColors.PaleBlue);

        //Styles.SetHorizontalAlignment( "EvenCentered", HorizontalAlignment.Center );
        //Styles.SetHorizontalAlignment("OddCentered", HorizontalAlignment.Center);
        //Styles.SetFillForegroundColor("OddCentered", IndexedColors.PaleBlue);
    }

    protected IJ4JLogger Logger { get; }
    protected IWorkbook? Workbook { get; private set; }
    protected ISheet? Worksheet { get; private set; }
    protected CustomStyles? Styles { get; private set; }
    protected int EntityNumber { get; private set; }

    public bool Initialized { get; private set; }
    public string? SheetName { get; private set; }

    protected virtual void Initialize( IWorkbook workbook, string sheetName )
    {
        Workbook = workbook;
        SheetName = sheetName;

        var worksheet = workbook.GetSheet(SheetName);

        if (worksheet != null)
        {
            Logger.Warning<string>("Worksheet '{0}' already exists in workbook, removing it", SheetName);

            var idx = workbook.GetSheetIndex(Worksheet);
            workbook.RemoveSheetAt(idx);
        }

        Worksheet = workbook.CreateSheet(SheetName);

        Styles = new CustomStyles( Workbook );

        Initialized = true;
    }

    public bool ExportData()
    {
        if( !Initialized )
        {
            Logger.Error( "Exporter is not initialized" );
            return false;
        }

        if( !ExportHeader() )
            return false;

        EntityNumber = 0;

        foreach( var entity in GetEntities() )
        {
            ProcessEntity( entity );

            if( EntityNumber != 0 && EntityNumber % 500 == 0 )
                Logger.Information( "\t...exported {0:n0} transactions", EntityNumber );

            EntityNumber++;
        }

        Logger.Information("\t...exported {0:n0} transactions", EntityNumber - 1);

        return ExportFooter();
    }

    public int Row
    {
        get => _rowNum;

        set
        {
            if( value < 0 )
            {
                Logger.Error("Row number can't be < 0"  );
                value = 0;
            }

            _rowNum = value;

            _curRow = null;
            _curCell = null;
        }
    }

    public int Column
    {
        get => _colNum;

        set
        {
            if (value < 0)
            {
                Logger.Error("Column number can't be < 0");
                value = 0;
            }

            _colNum = value;

            _curRow = null;
            _curCell = null;
        }
    }

    public IRow? CurrentRow
    {
        get
        {
            if (Worksheet == null)
            {
                Logger.Error("Exporter is not initialized");
                return null;
            }

            if ( _curRow != null )
                return _curRow;

            _curRow = Worksheet.GetRow(_rowNum) ?? Worksheet.CreateRow(_rowNum);

            return _curRow;
        }
    }

    public ICell? CurrentCell
    {
        get
        {
            if (Worksheet == null)
            {
                Logger.Error("Exporter is not initialized");
                return null;
            }

            if ( _curCell != null )
                return _curCell;

            _curCell = CurrentRow!.GetCell(_colNum) ?? CurrentRow.CreateCell(_colNum);

            return _curCell;
        }
    }

    protected void SetPosition( int row, int col )
    {
        Row = row;
        Column = col;
    }

    protected void MoveRows( int rows = 1 )
    {
        if( Worksheet == null )
        {
            Logger.Error("Trying to move rows before initializing worksheet");
            return;
        }

        if( _rowNum + rows >= 0 )
            _rowNum += rows;

        _curRow = null;
        _curCell = null;
        _colNum = 0;

        _numCols = 1;
    }

    protected void MoveColumns( int columns = 1 )
    {
        if( CurrentRow == null )
        {
            Logger.Error( "Trying to move columns before initializing row" );
            return;
        }

        if( _colNum + columns >= 0 )
            _colNum += columns;

        _curCell = null;

        _numCols += columns;
    }

    protected void AutoSizeColumns( params int[] colNums )
    {
        if( Worksheet == null )
        {
            Logger.Error("Exporter is not initialized");
            return;
        }

        foreach( var colNum in colNums )
        {
            Worksheet.AutoSizeColumn(colNum);
        }
    }

    protected void AutoSizeColumns()
    {
        if (Worksheet == null)
        {
            Logger.Error("Exporter is not initialized");
            return;
        }

        for (var colNum = 0; colNum <= _numCols; colNum++)
        {
            Worksheet.AutoSizeColumn(colNum);
        }
    }

    protected void MergeCellsHorizontally( int horizontalRange )
    {
        if (Worksheet == null)
        {
            Logger.Error("Exporter is not initialized");
            return;
        }

        if ( CurrentCell == null )
        {
            Logger.Error("Trying to merge cells before moving to cell");
            return;
        }

        var mergedCells = new CellRangeAddress( CurrentCell.RowIndex,
                                                CurrentCell.RowIndex,
                                                CurrentCell.ColumnIndex,
                                                CurrentCell.ColumnIndex + horizontalRange );

        Worksheet.AddMergedRegion(mergedCells);

        RegionUtil.SetBorderBottom( 2, mergedCells, Worksheet );
    }

    protected void ApplyStyle( string? name )
    {
        if( string.IsNullOrEmpty( name ) )
            return;

        if (Worksheet == null)
        {
            Logger.Error("Exporter is not initialized");
            return;
        }

        if ( CurrentCell == null )
        {
            Logger.Error("Trying to format cell before moving to cell");
            return;
        }

        var styleInfo = Styles!.Contains( name ) ? Styles[ name ] : null;
        if( styleInfo == null )
            return;

        CurrentCell.CellStyle = styleInfo.CellStyle;
    }

    protected void SetCellValue<T>( T? value, string? style = null, int colsToMove = 1 )
    {
        if( CurrentCell == null )
        {
            Logger.Error("Trying to set value for undefined cell"  );
            return;
        }

        if( colsToMove < 0 )
            colsToMove = 1;

        MoveColumns(colsToMove);

        if( value == null )
        {
            ApplyStyle( style );
            return;
        }

        switch( value )
        {
            case string textValue:
                CurrentCell.SetCellValue( textValue );
                break;

            case double dblValue:
                CurrentCell.SetCellValue( dblValue );
                break;

            case int intValue:
                CurrentCell.SetCellValue( intValue );
                break;

            case bool boolValue:
                CurrentCell.SetCellValue( boolValue ? "Y" : "N" );
                break;

            case DateTime dtValue:
                CurrentCell.SetCellValue( dtValue );
                break;

            default:
                Logger.Error( "Unsupported value type '{0}'", typeof( T ) );
                break;
        }

        ApplyStyle( style );
    }

    protected virtual bool ExportHeader() => true;
    protected virtual bool ExportFooter() => true;

    protected abstract bool ProcessEntity( TEntity entity );
    protected abstract IEnumerable<TEntity> GetEntities();
}