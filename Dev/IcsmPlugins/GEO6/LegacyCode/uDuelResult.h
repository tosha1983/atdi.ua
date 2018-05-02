//---------------------------------------------------------------------------

#ifndef uDuelResultH
#define uDuelResultH
//---------------------------------------------------------------------------

#include <Classes.hpp>
#include <Controls.hpp>
#include <ExtCtrls.hpp>
#include <Forms.hpp>
#include <Graphics.hpp>
#include <Grids.hpp>
#include <StdCtrls.hpp>

#include "uDiagramPanel.h"
#include "uProfileView.h"

#include <memory>

//---------------------------------------------------------------------------

class TfrmDuelResult : public TForm
{
__published:	// IDE-managed Components
    TPanel *panData;
    TPanel *panGraph;
    TLabel *lblA;
    TLabel *lblB;
    TStringGrid *grdPoints;
    TLabel *lblEminA;
    TLabel *lblEminB;
    TLabel *lblEa;
    TLabel *lblEb;
    TLabel *lblAData;
    TLabel *lblBData;
    TLabel *lblEminAData;
    TLabel *lblEminBData;
    TSplitter *Splitter1;
    TPanel *panRelief;
    TfmProfileView *fmProfileView;
    void __fastcall FormDeactivate(TObject *Sender);
    void __fastcall FormKeyDown(TObject *Sender, WORD &Key, TShiftState Shift);
    void __fastcall panDataResize(TObject *Sender);
    void __fastcall grdPointsDrawCell(TObject *Sender, int ACol, int ARow, TRect &Rect, TGridDrawState State);
    void __fastcall panGraphResize(TObject *Sender);
    void __fastcall OnDuelResultResize(TObject *Sender);
    void __fastcall panGraphMouseMove(TObject *Sender, TShiftState Shift, int X, int Y);
private:
    void __fastcall pdpA_DoMouseMove(int X, int Y);
    void __fastcall pdpA_OnMouseMove(TObject *Sender, TShiftState Shift, int X, int Y);
public:		// User declarations
    __fastcall TfrmDuelResult(TComponent* Owner, bool tt);
    TPolarDiagramPanel* pdpA;
    TPolarDiagramPanel* pdpB;
    double dist03;
    bool ttAMType;
    COnMouseMove Import_OnMouseMove;
    void __fastcall External_OnMouseMove(int X, int Y);
    void __fastcall setProfileViewWidth();
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmDuelResult *frmDuelResult;
//---------------------------------------------------------------------------
#endif

