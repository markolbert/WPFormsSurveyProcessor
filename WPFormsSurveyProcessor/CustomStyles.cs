﻿using System.Collections.ObjectModel;
using NPOI.SS.UserModel;

namespace WPFormsSurveyProcessor;

public class CustomStyles : KeyedCollection<string, CustomStyles.Style>
{
    public record Style( string Name, ICellStyle CellStyle );

    public CustomStyles( 
        IWorkbook workbook 
        )
        : base( StringComparer.OrdinalIgnoreCase )
    {
        Workbook = workbook;
    }

    protected override string GetKeyForItem( Style item ) => item.Name;

    public IWorkbook Workbook { get; }

    public Style SetDataFormat( string name, string formatString )
    {
        var retVal = GetCreateStyle( name );
        retVal.CellStyle.DataFormat = Workbook.CreateDataFormat().GetFormat( formatString );

        return retVal;
    }

    private Style GetCreateStyle( string name )
    {
        if( Contains( name ))
            return this[ name ];

        var retVal = new Style(name, Workbook.CreateCellStyle());
        Add( retVal );

        return retVal;
    }

    public Style SetFillForegroundColor(
        string name,
        IndexedColors color,
        FillPattern pattern = FillPattern.SolidForeground
    )
    {
        var retVal = GetCreateStyle( name );
        retVal.CellStyle.FillForegroundColor = color.Index;
        retVal.CellStyle.FillPattern = pattern;

        return retVal;
    }

    public Style SetFillBackgroundColor(
        string name,
        IndexedColors color,
        FillPattern pattern = FillPattern.SolidForeground
    )
    {
        var retVal = GetCreateStyle( name );
        retVal.CellStyle.FillBackgroundColor = color.Index;
        retVal.CellStyle.FillPattern = pattern;

        return retVal;
    }

    public Style SetBottomBorder( string name, BorderStyle borderStyle )
    {
        var retVal = GetCreateStyle(name);
        retVal.CellStyle.BorderBottom = borderStyle;

        return retVal;
    }

    public Style MakeBold( string name )
    {
        var retVal = GetCreateStyle(name);

        var font = Workbook.CreateFont();
        font.IsBold = true;
        retVal.CellStyle.SetFont( font );

        return retVal;
    }

    public Style SetHorizontalAlignment( string name, HorizontalAlignment alignment )
    {
        var retVal = GetCreateStyle(name);
        retVal.CellStyle.Alignment = alignment;

        return retVal;
    }

    public Style SetWordWrap(string name, bool wrapText = true )
    {
        var retVal = GetCreateStyle(name);
        retVal.CellStyle.WrapText = wrapText;

        return retVal;
    }
}