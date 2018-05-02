//---------------------------------------------------------------------------

#ifndef uListCityModalH
#define uListCityModalH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include "uListCity.h"
#include <ActnList.hpp>
#include <ComCtrls.hpp>
#include <DB.hpp>
#include <DBGrids.hpp>
#include <ExtCtrls.hpp>
#include <Grids.hpp>
#include <IBCustomDataSet.hpp>
#include <IBDatabase.hpp>
#include <IBSQL.hpp>
#include <ImgList.hpp>
#include <ToolWin.hpp>
//---------------------------------------------------------------------------
class TfrmListCityModal : public TfrmListCity
{
__published:	// IDE-managed Components
    void __fastcall FormDestroy(TObject *Sender);
private:	// User declarations
    __fastcall TfrmListCityModal(TComponent* Owner);
public:		// User declarations
    __fastcall TfrmListCityModal(TComponent* Owner, HWND caller, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListCityModal *frmListCityModal;
//---------------------------------------------------------------------------
#endif
