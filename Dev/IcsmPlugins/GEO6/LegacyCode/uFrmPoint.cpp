//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uFrmPoint.h"
#include "uLayoutMngr.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TfrmPoint *frmPoint;
//---------------------------------------------------------------------------
__fastcall TfrmPoint::TfrmPoint(TComponent* Owner)
    : TForm(Owner)
{

}
//---------------------------------------------------------------------------
void __fastcall TfrmPoint::FormCreate(TObject *Sender)
{
    LayoutManager.loadLayout(this);
}
//---------------------------------------------------------------------------

void __fastcall TfrmPoint::FormResize(TObject *Sender)
{
    int firstColWidth = 150;
    int lift = 16;
    stringGrid->DefaultColWidth = (stringGrid->Width - firstColWidth) / (stringGrid->ColCount - 1) - 1;
    stringGrid->ColWidths[0] = firstColWidth - lift;

    int deficit = stringGrid->Width - (stringGrid->DefaultColWidth + 1) * (stringGrid->ColCount - 1) - (stringGrid->ColWidths[0] + lift);
    //Label1->Caption = "deficit = " + IntToStr(deficit);
    for (int i = 0; (i < stringGrid->ColCount / 2) && (deficit > 0); i++, deficit--)
        stringGrid->ColWidths[i * 2 + 1] = stringGrid->DefaultColWidth + 1;
    for (int i = 1; (i < stringGrid->ColCount / 2) && (deficit > 0); i++, deficit--)
        stringGrid->ColWidths[i * 2] = stringGrid->DefaultColWidth + 1;
}
//---------------------------------------------------------------------------

//  преобразование цифрового индекса колонки в буквенный
AnsiString __fastcall TfrmPoint::nToAz(int n)
{
    static const int base = 'Z' - 'A' + 1;
    AnsiString res;
    int len = 1;
    int val = n;
    while (val /= base)
        len++;
    res.SetLength(len);
    val = n;
    while (len > 0) {
        res[len--] = 'A' + val % base;
        val /= base;
        //  коррекция. без неё ненулевые разряды начнутся с 'B'
        val--;
    }
    return res;
}
//---------------------------------------------------------------------------

void __fastcall TfrmPoint::stringGridDrawCell(TObject *Sender, int ACol,
      int ARow, TRect &Rect, TGridDrawState State)
{
    TStringGrid *sg = dynamic_cast<TStringGrid*>(Sender);
    if (Sender == NULL)
        return;

    AnsiString text = Trim(sg->Cells[ACol][ARow]);
    int xOffset = 1;

    if ( (sg->RowCount > 0) && (unwantedRows.find(ARow) != unwantedRows.end()) && (ACol > 0) )
    {
        sg->Canvas->Brush->Color = clRed;
        sg->Canvas->Font->Color = clWhite;
    }

    if ( ( maxInterferenceRow > -1 ) && (sg->RowCount > 0) && (ARow == maxInterferenceRow) && (ACol > 0) )
        sg->Canvas->Font->Style = TFontStyles()<< fsBold;

    if (ARow == 0 || ACol == stringGrid->ColCount - 1 || ACol == stringGrid->ColCount - 2 || ACol == stringGrid->ColCount - 3)
    {
        //  center align
        xOffset = (Rect.Right - Rect.left - sg->Canvas->TextWidth(text)) / 2;
        //sg->Canvas->Brush->Color = clBtnFace;
    } else if (ACol > 0) {
        //  right align
        xOffset = Rect.Right - Rect.left - sg->Canvas->TextWidth(text) - 2 ;
    }

    sg->Canvas->TextRect(Rect, Rect.left + xOffset, Rect.Top + 1, text);

}
//---------------------------------------------------------------------------

void __fastcall TfrmPoint::FormDestroy(TObject *Sender)
{
    LayoutManager.saveLayout(this);    
}
//---------------------------------------------------------------------------

void __fastcall TfrmPoint::btToExcelClick(TObject *Sender)
{
    Variant excel = CreateOleObject("Excel.Application");

    excel.OlePropertyGet("Application").OlePropertySet<int>("SheetsInNewWorkbook", 1);
    excel.OlePropertyGet("Workbooks").OleProcedure("Add");

    excel.OlePropertyGet<WideString>("Range", "A1").OleProcedure("Select");
    excel.OlePropertyGet("ActiveCell").OlePropertySet<WideString>("FormulaR1C1", Caption);

    excel.OlePropertyGet<WideString>("Rows", "2:2").OleProcedure("Select");
    excel.OlePropertyGet("Selection").OlePropertyGet("Font").OlePropertySet<VARIANT_BOOL>("Bold", true);

    for ( int i = 0; i < stringGrid->ColCount; i++ )
    try {
        AnsiString colRef = nToAz(i);
        excel.OlePropertyGet<WideString>("Range", colRef + '2').OleProcedure("Select");
        excel.OlePropertyGet("ActiveCell").OlePropertySet<WideString>("FormulaR1C1", stringGrid->Cells[i][0]);
    } catch (...) {
    }


    for ( int j = 2; j < stringGrid->RowCount; j++ )
    try {
        for ( int i = 0; i < stringGrid->ColCount; i++ )
        {
            AnsiString colRef = nToAz(i);
            excel.OlePropertyGet<WideString>("Range", colRef + IntToStr(j+1)).OleProcedure("Select");
            excel.OlePropertyGet("ActiveCell").OlePropertySet<WideString>("FormulaR1C1", stringGrid->Cells[i][j]);
        }
    } catch (...) {
    }

    excel.OlePropertySet<VARIANT_BOOL>("Visible", true);
    excel = Unassigned;
}
//---------------------------------------------------------------------------

