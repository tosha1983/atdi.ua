//---------------------------------------------------------------------------

#ifndef uFormsDmH
#define uFormsDmH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <DB.hpp>
#include <IBCustomDataSet.hpp>
#include <IBQuery.hpp>
#include <IBSQL.hpp>
//---------------------------------------------------------------------------
class TdmForms : public TDataModule
{
__published:	// IDE-managed Components
    TIBQuery *ibqNamesPrograms;
    TIBQuery *ibqNumSertificates;
    TIBQuery *ibqListVideoEmission;
    TIBQuery *ibqListSoundEmission;
private:	// User declarations
public:		// User declarations
    __fastcall TdmForms(TComponent* Owner);
};
//---------------------------------------------------------------------------
extern PACKAGE TdmForms *dmForms;
//---------------------------------------------------------------------------
#endif
