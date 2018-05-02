//---------------------------------------------------------------------------

#ifndef uReliefFrmH
#define uReliefFrmH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include "uProfileView.h"
//---------------------------------------------------------------------------
class TfrmRelief : public TForm
{
__published:	// IDE-managed Components
    TfmProfileView *fmProfileView1;
    void __fastcall FormDeactivate(TObject *Sender);
private:	// User declarations
public:		// User declarations
    __fastcall TfrmRelief(TComponent* Owner);
};
//---------------------------------------------------------------------------
//---------------------------------------------------------------------------
#endif
