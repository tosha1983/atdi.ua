//---------------------------------------------------------------------------

#ifndef uFrmPointH
#define uFrmPointH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ExtCtrls.hpp>
#include <Grids.hpp>
#include <Buttons.hpp>

#include <set>
//---------------------------------------------------------------------------
class TfrmPoint : public TForm
{
__published:	// IDE-managed Components
    TPanel *pnButtons;
    TStringGrid *stringGrid;
    TPanel *pnlCpReq;
    TLabel *lblCPNumber;
    TLabel *lblCPDistance;
    TLabel *lblCPESum;
    TSpeedButton *btToExcel;
    TPanel *pnSum;
    TLabel *Label1;
    TLabel *lbEu;
    TLabel *Label3;
    TLabel *lbEuIntfr;
    TLabel *Label5;
    TLabel *lbDiff;
    TLabel *Label7;
    TLabel *lbSumIntfr;
    TLabel *Label2;
    TLabel *lbSumSelected;
    void __fastcall FormCreate(TObject *Sender);
    void __fastcall FormResize(TObject *Sender);
    void __fastcall stringGridDrawCell(TObject *Sender, int ACol, int ARow,
          TRect &Rect, TGridDrawState State);
    void __fastcall FormDestroy(TObject *Sender);
    void __fastcall btToExcelClick(TObject *Sender);
private:
    AnsiString __fastcall nToAz(int n);
public:		// User declarations
    __fastcall TfrmPoint(TComponent* Owner);
    int maxInterferenceRow;
    std::set<int> unwantedRows;
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmPoint *frmPoint;
//---------------------------------------------------------------------------
#endif
