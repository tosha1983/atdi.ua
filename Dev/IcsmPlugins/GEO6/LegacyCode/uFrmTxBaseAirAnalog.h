//---------------------------------------------------------------------------

#ifndef uFrmTxBaseAirAnalogH
#define uFrmTxBaseAirAnalogH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include "uFrmTxBaseAir.h"
#include <Buttons.hpp>
#include <ComCtrls.hpp>
#include <ExtCtrls.hpp>
#include <DBGrids.hpp>
#include <Grids.hpp>
#include <DB.hpp>
#include <IBCustomDataSet.hpp>
#include <DBCtrls.hpp>
#include <Mask.hpp>
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
class TfrmTxBaseAirAnalog : public TfrmTxBaseAir
{
__published:	// IDE-managed Components
    TIBDataSet *ibdsRetranslate;
    TDataSource *dsRetranslate;
    TIntegerField *ibdsRetranslateID;
    TIBStringField *ibdsRetranslateADMINISTRATIONID;
    TIBStringField *ibdsRetranslateNAMESITE;
    TIBStringField *ibdsRetranslateNAMECHANNEL;
    TIBStringField *ibdsRetranslateTYPEREC_NAME;
    TIntegerField *ibdsRetranslateRELAYSTATION_ID;
    TIntegerField *ibdsRetranslateTYPERECEIVE_ID;
    void __fastcall dbcbTypeReciveChange(TObject *Sender);
    void __fastcall ibdsRetranslateAfterEdit(TDataSet *DataSet);
    void __fastcall dbcbTypeReciveDropDown(TObject *Sender);
    void __fastcall btnDeleteRelayClick(TObject *Sender);
private:	// User declarations
public:		// User declarations
    __fastcall TfrmTxBaseAirAnalog(TComponent* Owner, ILISBCTx *in_Tx);
protected:
    BEGIN_MESSAGE_MAP
        VCL_MESSAGE_HANDLER(WM_LIST_ELEMENT_SELECTED, Messages::TMessage, acceptListElementSelection)
    END_MESSAGE_MAP(TForm)
    virtual void __fastcall acceptListElementSelection(Messages::TMessage &Message);

    virtual void __fastcall TxDataLoad();
    virtual void __fastcall TxDataSave();
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmTxBaseAirAnalog *frmTxBaseAirAnalog;
//---------------------------------------------------------------------------
#endif
