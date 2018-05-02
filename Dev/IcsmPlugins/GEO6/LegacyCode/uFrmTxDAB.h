//---------------------------------------------------------------------------

#ifndef uFrmTxDABH
#define uFrmTxDABH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include "uFrmTxBaseAirDigital.h"
#include <Buttons.hpp>
#include <ComCtrls.hpp>
#include <ExtCtrls.hpp>
#include <DBGrids.hpp>
#include <Grids.hpp>
#include <DB.hpp>
#include <IBCustomDataSet.hpp>
#include <DBCtrls.hpp>
#include <Mask.hpp>
#include <math.h>
#include <IBQuery.hpp>
#include <OleCtnrs.hpp>
#include <ActnList.hpp>
#include <Menus.hpp>
#include <ImgList.hpp>
#include "xlcClasses.hpp"
#include "xlEngine.hpp"
#include "xlReport.hpp"
#include <IBSQL.hpp>
#include "NumericEdit.hpp"
#include <ToolWin.hpp>
#include "CustomMap.h"
#include "uLisObjectGrid.h"
#include <IBDatabase.hpp>
//---------------------------------------------------------------------------
class TfrmTxDAB : public TfrmTxBaseAirDigital
{
__published:	// IDE-managed Components
    TPanel *pnlForDAB;
    TLabel *lblFreqCentre;
    TNumericEdit *edtFreqCentre;
    TLabel *lblRangeFreq;
    TDBEdit *edtFreqFrom;
    TButton *btnBlock;
    TDBEdit *edtFreqTo;
    TLabel *Label2;
    TIBDataSet *ibdsDAB;
    TDataSource *dsDAB;
    TIntegerField *ibdsDABID;
    TIntegerField *ibdsDABALLOTMENTBLOCKDAB_ID;
    TFloatField *ibdsDABBD_FREQFROM;
    TFloatField *ibdsDABBD_FREQTO;
    TIBStringField *ibdsDABBLOCK_NAME;
    TDBEdit *edtBlockDAB;
    TLabel *Label3;
    TLabel *lblClassRadiation;
    TEdit *edtClassRadiationVideo;
    TDBEdit *edtSoundEmissionPrimary;
    TBitBtn *btnSoundEmissionPrimary;
    void __fastcall FormCreate(TObject *Sender);
    void __fastcall btnBlockClick(TObject *Sender);
    void __fastcall ibdsDABAfterEdit(TDataSet *DataSet);
    void __fastcall btnSoundEmissionPrimaryClick(TObject *Sender);
private:	// User declarations
public:		// User declarations
    __fastcall TfrmTxDAB(TComponent* Owner, ILISBCTx *in_Tx);
protected:
    BEGIN_MESSAGE_MAP
        VCL_MESSAGE_HANDLER(WM_LIST_ELEMENT_SELECTED, Messages::TMessage, acceptListElementSelection)
    END_MESSAGE_MAP(TForm)
    virtual void __fastcall acceptListElementSelection(Messages::TMessage &Message);
    virtual   void __fastcall TfrmTxDAB::TxDataLoad();
    virtual  void __fastcall TfrmTxDAB::TxDataSave();
    virtual void __fastcall SetRadiationClass();
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmTxDAB *frmTxDAB;
//---------------------------------------------------------------------------
#endif
