//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uDuelResult.h"
#include "uParams.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uProfileView"
#pragma resource "*.dfm"
TfrmDuelResult *frmDuelResult;
static char *duelColNamesTtAM[] = {"Точка", "Е зав", "ЗВ", "Е вик", "Е мін"};
static char *duelColNames[] = {"Точка", "Е зав", "ЗВ", "D ант", "Е вик", "Е мін", "Тип зав"};
static int duelColCount = 7;
static int duelColCountTtAM = 5;
//---------------------------------------------------------------------------
__fastcall TfrmDuelResult::TfrmDuelResult(TComponent* Owner, bool tt)
    : TForm(Owner)
{
    dist03 = 0.0;
    ttAMType = tt;
    pdpA = new TPolarDiagramPanel(this);
    pdpA->Parent = panGraph;
    pdpA->showMode = smDuel;
    pdpA->Align = alNone;
    pdpA->OnMouseMove = pdpA_OnMouseMove;
    pdpA->OnResize = OnDuelResultResize;

    pdpA->fmProfileView = fmProfileView;

    pdpB = new TPolarDiagramPanel(Application);
    pdpB->Parent = panGraph;
    pdpB->showMode = smLine;
    pdpB->Visible = false;
    /*
    int minDim = panGraph->Width < panGraph->Height ? panGraph->Width : panGraph->Height;
    pdpA->Width = minDim / 2;
    pdpA->Height = minDim / 2;
    pdpB->Width = minDim / 2;
    pdpB->Height = minDim / 2;
    pdpA->Left = panGraph->Width / 2 - pdpA->Width;
    pdpB->Left = panGraph->Width / 2;
    */
    /*
    pdpA->Width = panGraph->ClientWidth / 2 - 1;
    pdpA->Height = panGraph->ClientHeight / 2 - 1;
    pdpB->Width = panGraph->ClientWidth / 2 - 1;
    pdpB->Height = panGraph->ClientHeight / 2 - 1;
    pdpA->Left = panGraph->ClientWidth / 2 - pdpA->Width;
    pdpB->Left = panGraph->ClientWidth / 2;
    pdpA->Top = panGraph->ClientHeight / 2 - pdpA->Height;
    pdpB->Top = panGraph->ClientHeight / 2;
    */

    if(ttAMType)
    {
        grdPoints->ColCount = duelColCountTtAM;
        for (int i = 0; i < duelColCountTtAM; i++)
            grdPoints->Cells[i][0] = duelColNamesTtAM[i];
        grdPoints->RowCount = 5;
        panData->Height = 135;
        grdPoints->Height = 88;
        grdPoints->Top = 42;
    }
    else
    {
        grdPoints->ColCount = duelColCount;
        for (int i = 0; i < duelColCount; i++)
            grdPoints->Cells[i][0] = duelColNames[i];
        grdPoints->RowCount = 17;
        panData->Height = 135 + (grdPoints->DefaultRowHeight+1) * 12;
        grdPoints->Height = 88 + (grdPoints->DefaultRowHeight+1) * 12;
        grdPoints->Top = 42;
        panRelief->Align = alClient;
        fmProfileView->Height = panRelief->Height - 2;
    }
    grdPoints->Cells[0][1] = "А дальня";
    grdPoints->Cells[0][2] = "А ближня";
    grdPoints->Cells[0][3] = "В ближня";
    grdPoints->Cells[0][4] = "В дальня";

    lblAData->Font->Color = clMaroon;
    lblA->Font->Style = lblA->Font->Style << fsBold;
    lblBData->Font->Color = clMaroon;
    lblB->Font->Style = lblB->Font->Style << fsBold;
    lblEminAData->Font->Color = clMaroon;
    lblEminBData->Font->Color = clMaroon;
}
//---------------------------------------------------------------------------
void __fastcall TfrmDuelResult::FormDeactivate(TObject *Sender)
{
    Close();
}
//---------------------------------------------------------------------------
void __fastcall TfrmDuelResult::FormKeyDown(TObject *Sender, WORD &Key,
      TShiftState Shift)
{
    if (Key == VK_ESCAPE && Shift == TShiftState())
        Close();
    if (Key == VK_RETURN && Shift == TShiftState())
        Close();
}
//---------------------------------------------------------------------------

void __fastcall TfrmDuelResult::panDataResize(TObject *Sender)
{
    grdPoints->DefaultColWidth = grdPoints->Width / grdPoints->ColCount - 1;
}
//---------------------------------------------------------------------------

void __fastcall TfrmDuelResult::grdPointsDrawCell(TObject *Sender,
      int ACol, int ARow, TRect &Rect, TGridDrawState State)
{
    TStringGrid *sg = dynamic_cast<TStringGrid*>(Sender);
    if (!sg)
        return;

    AnsiString text = Trim(sg->Cells[ACol][ARow]);

    int xOffset = 1;

    sg->Canvas->Font->Style = sg->Canvas->Font->Style >> fsBold;
    if(ttAMType)
    {
        if (ACol == 1 ||
            ACol == 2 ||
            ACol == 3 ||
            ACol == 4)
        {
            //  right align
            xOffset = Rect.Right - Rect.left - sg->Canvas->TextWidth(text) - 2;
        }
        if (ARow == 0)
            sg->Canvas->Font->Style = sg->Canvas->Font->Style << fsBold;
        if (
            ACol == 0 ||
            ARow == 0)
        {
            //  center align
            xOffset = (Rect.Right - Rect.left - sg->Canvas->TextWidth(text)) / 2;
        }
    }
    else
    {
        if (ACol == 1 ||
            ACol == 2 ||
            ACol == 3 ||
            ACol == 4 ||
            ACol == 5)
        {
            //  right align
            xOffset = Rect.Right - Rect.left - sg->Canvas->TextWidth(text) - 2;
        }
        if (ARow == 0)
            sg->Canvas->Font->Style = sg->Canvas->Font->Style << fsBold;
        if (
            ACol == 0 ||
            ACol == 6 ||
            ARow == 0)
        {
            //  center align
            xOffset = (Rect.Right - Rect.left - sg->Canvas->TextWidth(text)) / 2;
        }
    }
    sg->Canvas->TextRect(Rect, Rect.left + xOffset, Rect.Top + 1, text);
}
//---------------------------------------------------------------------------

void __fastcall TfrmDuelResult::panGraphResize(TObject *Sender)
{
    pdpA->Invalidate();
}
//---------------------------------------------------------------------------
void __fastcall TfrmDuelResult::OnDuelResultResize(TObject *Sender)
{
    setProfileViewWidth();
    #ifdef _DEBUG
        MessageBeep(0xFFFFFFFF);
    #endif
}

void __fastcall TfrmDuelResult::setProfileViewWidth()
{
    //fmProfileView->Width = dist03 / pdpA->norma;
    fmProfileView->Width = pdpA->Width;
    fmProfileView->Left = (fmProfileView->Parent->Width - fmProfileView->Width) / 2;
}
//---------------------------------------------------------------------------

void __fastcall TfrmDuelResult::pdpA_DoMouseMove(int X, int Y)
{
    if ( pdpA )
        pdpA->DrawMarker(X);
}
//---------------------------------------------------------------------------

void __fastcall TfrmDuelResult::pdpA_OnMouseMove(TObject *Sender, TShiftState Shift, int X, int Y)
{
    pdpA_DoMouseMove(X, Y);

    if ( Import_OnMouseMove )
        Import_OnMouseMove(X, Y);
}
//---------------------------------------------------------------------------

void __fastcall TfrmDuelResult::panGraphMouseMove(TObject *Sender, TShiftState Shift, int X, int Y)
{
/*
    pdpA_DoMouseMove(X, Y);
*/
}
//---------------------------------------------------------------------------

void __fastcall TfrmDuelResult::External_OnMouseMove(int X, int Y)
{
    pdpA_DoMouseMove(X, Y);
}
//---------------------------------------------------------------------------

