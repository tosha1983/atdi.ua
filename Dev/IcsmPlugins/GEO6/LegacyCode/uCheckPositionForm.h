//---------------------------------------------------------------------------

#ifndef uCheckPositionFormH
#define uCheckPositionFormH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include "CustomMap.h"
#include <Menus.hpp>
//---------------------------------------------------------------------------
class TfrmCheckPosition : public TForm
{
__published:	// IDE-managed Components
    TCustomMapFrame *cmf;
    void __fastcall FormClose(TObject *Sender, TCloseAction &Action);
private:	// User declarations
public:		// User declarations
    __fastcall TfrmCheckPosition(TComponent* Owner);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmCheckPosition *frmCheckPosition;
//---------------------------------------------------------------------------
#endif
