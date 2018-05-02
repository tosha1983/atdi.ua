//---------------------------------------------------------------------------

#ifndef uCoordZoneFieldStrH
#define uCoordZoneFieldStrH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
//---------------------------------------------------------------------------
class TfrmFS : public TForm
{
__published:	// IDE-managed Components
    TListBox *lstServices;
    TEdit *edFieldStr;
    TLabel *Label1;
    TButton *btOk;
    void __fastcall lstServicesClick(TObject *Sender);
private:	// User declarations
public:		// User declarations
    __fastcall TfrmFS(TComponent* Owner);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmFS *frmFS;
//---------------------------------------------------------------------------
#endif
