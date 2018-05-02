//---------------------------------------------------------------------------

#ifndef uTable36H
#define uTable36H
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <Grids.hpp>
#include <ValEdit.hpp>
#include <ExtCtrls.hpp>
#include <LISBCTxServer_TLB.h>

//---------------------------------------------------------------------------

typedef enum Table36type
{
  t36EPRH = 0,  // Ё»ћ по азимутам, дЅ
  t36EPRV = 1,
  t36HEFF = 2,  // эфф. высота по азимутам, м
  t36GAIN_H = 3,  // усиление антенны по аз.(H пол€р.), дЅ
  t36HEFF2 = 4,
  t36DISCR_H = 5,  // ослабление антены в заданном азимуте (H пол€р.)
  t36GAIN_V = 6,  //  усиление антенны по азимутам (V пол€р.), дЅ
  t36DISCR_V = 7,  //  ослабление антены по азимутам (V пол€р.), дЅ
  t36DegradationSector = 8
} Table36type;


class TfrmTable36 : public TForm
{
__published:	// IDE-managed Components
    TStringGrid *sgTable36;
    TShape *Shape1;
    TLabel *Label1;
    TShape *Shape2;
    TLabel *Label2;
    TStringGrid *sgTable36old;
    TButton *btnSaveNew;
    void __fastcall FormCreate(TObject *Sender);
    void __fastcall FormClose(TObject *Sender, TCloseAction &Action);
    void __fastcall FormDeactivate(TObject *Sender);
    void __fastcall sgTable36SetEditText(TObject *Sender, int ACol, int ARow, const AnsiString Value);
    void __fastcall FormPaint(TObject *Sender);
    void __fastcall btnSaveNewClick(TObject *Sender);
    void __fastcall sgTable36DrawCell(TObject *Sender, int ACol, int ARow, TRect &Rect, TGridDrawState State);
    void __fastcall FormKeyDown(TObject *Sender, WORD &Key, TShiftState Shift);
    void __fastcall sgTable36KeyPress(TObject *Sender, char &Key);
    void __fastcall sgTable36KeyDown(TObject *Sender, WORD &Key, TShiftState Shift);
private:	// User declarations
//    void *GetValue, *SetValue;
    bool data_changes;
    TForm* frmTx;
public:		// User declarations
    ILISBCTx *Tx;
    Table36type type;
    __fastcall TfrmTable36(TComponent* Owner, Table36type, ILISBCTx *in_Tx);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmTable36 *frmTable36;
//---------------------------------------------------------------------------
#endif
