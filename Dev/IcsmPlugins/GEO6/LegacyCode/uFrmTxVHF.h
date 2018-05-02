//---------------------------------------------------------------------------

#ifndef uFrmTxVHFH
#define uFrmTxVHFH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include "uFrmTxBaseAirAnalog.h"
#include <Buttons.hpp>
#include <ComCtrls.hpp>
#include <ExtCtrls.hpp>
#include <DB.hpp>
#include <DBGrids.hpp>
#include <Grids.hpp>
#include <IBCustomDataSet.hpp>
#include <DBCtrls.hpp>
#include <IBQuery.hpp>
#include <Mask.hpp>
#include <math.h>
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
class TfrmTxVHF : public TfrmTxBaseAirAnalog
{
__published:	// IDE-managed Components
        TPanel *pnlForVHF;
        TLabel *lblFreq;
    TNumericEdit *edtFreq;
    TLabel *lbMonoStereo;
    TComboBox *cbMonoStereo;
        TLabel *lblSystemCast;
        TLabel *lblClassRadiation;
    TComboBox *cbxTypeSysName;
    TEdit *edtClassRadiationVideo;
        TDBEdit *edtSoundEmissionPrimary;
        TBitBtn *btnSoundEmissionPrimary;
    void __fastcall cbMonoStereoChange(TObject *Sender);
    void __fastcall FormCreate(TObject *Sender);
    void __fastcall btnSystemVHFCastClick(TObject *Sender);
    void __fastcall cbxTypeSysNameChange(TObject *Sender);
    void __fastcall edtFreqValueChange(TObject *Sender);
    void __fastcall actIntoBaseExecute(TObject *Sender);
        void __fastcall btnSoundEmissionPrimaryClick(TObject *Sender);
private:	// User declarations
public:		// User declarations
    __fastcall TfrmTxVHF(TComponent* Owner, ILISBCTx *in_Tx);
protected:
    BEGIN_MESSAGE_MAP
        VCL_MESSAGE_HANDLER(WM_LIST_ELEMENT_SELECTED, Messages::TMessage, acceptListElementSelection)
    END_MESSAGE_MAP(TForm)
    virtual void __fastcall acceptListElementSelection(Messages::TMessage &Message);
    virtual void __fastcall TxDataSave();
    virtual  void __fastcall TxDataLoad();
    virtual void __fastcall SetRadiationClass();
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmTxVHF *frmTxVHF;
//---------------------------------------------------------------------------
#endif
