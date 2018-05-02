//---------------------------------------------------------------------------

#ifndef uOtherTerrSrvcH
#define uOtherTerrSrvcH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include "uBaseObjForm.h"
#include <ActnList.hpp>
#include <Buttons.hpp>
#include <ExtCtrls.hpp>
#include <IBDatabase.hpp>
#include <DB.hpp>
#include <IBCustomDataSet.hpp>
#include <DBCtrls.hpp>
#include <Mask.hpp>
#include <Grids.hpp>
//---------------------------------------------------------------------------
class TfrmOtherTerrSrvc : public TfrmBaseObjForm
{
__published:	// IDE-managed Components
    TLabel *Label1;
    TLabel *Label2;
    TDBEdit *DBEdit2;
    TLabel *Label3;
    TDBEdit *DBEdit3;
    TLabel *Label4;
    TDBEdit *DBEdit4;
    TLabel *Label5;
    TDBEdit *DBEdit5;
    TLabel *Label6;
    TDBEdit *DBEdit6;
    TLabel *Label7;
    TDBEdit *DBEdit7;
    TLabel *Label8;
    TDBEdit *DBEdit8;
    TLabel *Label9;
    TLabel *Label10;
    TDBEdit *DBEdit10;
    TLabel *Label11;
    TDBEdit *DBEdit11;
    TLabel *Label12;
    TDBEdit *DBEdit12;
    TLabel *Label13;
    TDBEdit *DBEdit13;
    TLabel *Label14;
    TLabel *Label15;
    TDBEdit *DBEdit15;
    TLabel *Label16;
    TDBEdit *DBEdit16;
    TLabel *Label17;
    TDBEdit *DBEdit17;
    TGroupBox *gbGeneral;
    TGroupBox *gbTransmissionSite;
    TGroupBox *gbReceptionSite;
    TGroupBox *gbSystem;
    TLabel *Label18;
    TDBEdit *DBEdit18;
    TLabel *Label19;
    TDBEdit *DBEdit19;
    TLabel *Label20;
    TDBEdit *DBEdit20;
    TLabel *Label21;
    TDBEdit *DBEdit21;
    TLabel *Label22;
    TDBEdit *DBEdit22;
    TLabel *Label23;
    TDBEdit *DBEdit23;
    TLabel *Label24;
    TDBEdit *DBEdit24;
    TGroupBox *gbAnt;
    TLabel *Label25;
    TDBEdit *DBEdit25;
    TLabel *Label26;
    TLabel *Label27;
    TDBEdit *DBEdit27;
    TLabel *Label28;
    TLabel *Label29;
    TDBEdit *DBEdit29;
    TLabel *Label30;
    TDBEdit *DBEdit30;
    TLabel *Label31;
    TDBEdit *DBEdit31;
    TLabel *Label32;
    TDBEdit *DBEdit32;
    TLabel *Label33;
    TDBEdit *edAzm;
    TLabel *Label34;
    TDBEdit *edAzmStart;
    TLabel *Label35;
    TDBEdit *edAzmEnd;
    TCheckBox *chRotating;
    TGroupBox *gbEah;
    TGroupBox *gbOther;
    TStringGrid *sgEah;
    TLabel *Label36;
    TLabel *Label37;
    TDBEdit *DBEdit36;
    TLabel *Label38;
    TDBEdit *DBEdit37;
    TDBCheckBox *DBCheckBox1;
    TDBCheckBox *DBCheckBox2;
    TLabel *Label39;
    TDBEdit *DBEdit38;
    TLabel *Label40;
    TDBEdit *DBEdit39;
    TDBComboBox *cbAdm;
    TDBComboBox *cbTxCtry;
    TDBComboBox *cbRxCtry;
    TDBComboBox *DBComboBox1;
    TDBComboBox *DBComboBox2;
    TButton *btAllMax;
    void __fastcall dstObjNewRecord(TDataSet *DataSet);
    void __fastcall chRotatingClick(TObject *Sender);
    void __fastcall dstObjAfterOpen(TDataSet *DataSet);
    void __fastcall sgEahDrawCell(TObject *Sender, int ACol, int ARow,
          TRect &Rect, TGridDrawState State);
    void __fastcall sgEahSetEditText(TObject *Sender, int ACol, int ARow,
          const AnsiString Value);
    void __fastcall sgEahExit(TObject *Sender);
    void __fastcall btAllMaxClick(TObject *Sender);
    void __fastcall CoordFieldGetText(TField *Sender, AnsiString &Text,
          bool DisplayText);
    void __fastcall CoordFieldSetText(TField *Sender,
          const AnsiString Text);
private:	// User declarations
public:		// User declarations
    __fastcall TfrmOtherTerrSrvc(TComponent* Owner);
    void __fastcall RefreshEah();
    bool eahChanged;
    void __fastcall SaveData(); // overriden
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmOtherTerrSrvc *frmOtherTerrSrvc;
//---------------------------------------------------------------------------
#endif
