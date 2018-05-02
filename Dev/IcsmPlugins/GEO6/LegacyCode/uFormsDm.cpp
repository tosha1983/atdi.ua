//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uFormsDm.h"
#include "uMainDm.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TdmForms *dmForms;
//---------------------------------------------------------------------------
__fastcall TdmForms::TdmForms(TComponent* Owner)
    : TDataModule(Owner)
{
}
//---------------------------------------------------------------------------

