//---------------------------------------------------------------------------

#ifndef uFrmTxFxmH
#define uFrmTxFxmH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include "CustomMap.h"
#include "uFrmTxBaseAir.h"
#include "uLisObjectGrid.h"
#include <ActnList.hpp>
#include <Buttons.hpp>
#include <ComCtrls.hpp>
#include <DB.hpp>
#include <DBCtrls.hpp>
#include <DBGrids.hpp>
#include <ExtCtrls.hpp>
#include <Grids.hpp>
#include <IBCustomDataSet.hpp>
#include <IBDatabase.hpp>
#include <IBQuery.hpp>
#include <IBSQL.hpp>
#include <ImgList.hpp>
#include <Mask.hpp>
#include <Menus.hpp>
#include "NumericEdit.hpp"
//---------------------------------------------------------------------------
class TfrmTxFxm : public TfrmTxBaseAir
{
__published:	// IDE-managed Components
    TPanel *pnEmission;
    TLabel *lbFreq;
    TNumericEdit *edFreq;
    TPanel *pnFxm;
    TLabel *lbEmissionClass;
    TLabel *lbBw;
    TBitBtn *btEmissionClass;
    TDBEdit *edEmissionClass;
    TNumericEdit *edBw;
    TLabel *lbServ;
    TComboBox *cbServ;
    TIBDataSet *dsFxm;
    TDataSource *srcFxm;
    void __fastcall edFreqValueChange(TObject *Sender);
    void __fastcall edBwValueChange(TObject *Sender);
    void __fastcall cbServChange(TObject *Sender);
private:	// User declarations
public:		// User declarations
    __fastcall TfrmTxFxm(TComponent* Owner, ILISBCTx *in_Tx);
    virtual void __fastcall SetRadiationClass();
    typedef TfrmTxBaseAir Inherited;
protected:
    BEGIN_MESSAGE_MAP
        VCL_MESSAGE_HANDLER(WM_LIST_ELEMENT_SELECTED, Messages::TMessage, AcceptListElementSelection)
    END_MESSAGE_MAP(TForm)
    virtual void __fastcall AcceptListElementSelection(Messages::TMessage &Message);

    virtual void __fastcall TxDataLoad();
    virtual void __fastcall TxDataSave();
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmTxFxm *frmTxFxm;
//---------------------------------------------------------------------------
#endif
