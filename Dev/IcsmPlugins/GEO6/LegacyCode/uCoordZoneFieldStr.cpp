//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uCoordZoneFieldStr.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TfrmFS *frmFS;
//---------------------------------------------------------------------------
__fastcall TfrmFS::TfrmFS(TComponent* Owner)
    : TForm(Owner)
{
}
//---------------------------------------------------------------------------

void __fastcall TfrmFS::lstServicesClick(TObject *Sender)
{
  double emin;
  switch (lstServices->ItemIndex)
  {
    case 0: emin = 46; break;
    case 1: emin = 49; break;
    case 2: emin = 12; break;
    case 3: emin = 17; break;
    case 4: emin = 21; break;
    case 5: emin = 23; break;
    case 6: emin = 25; break;
    case 7: emin = 17; break;
    case 8: emin = 33; break;
    case 9: emin = 20; break;
    case 10: emin = 12; break;
    case 11: emin = 27; break;
    case 12: emin = 13; break;
    case 13: emin = 29; break;
    case 14: emin = 40; break;
    case 15: emin = 10; break;
    case 16: emin = 23; break;
    case 17: emin = 30; break;
    case 18: emin = 35; break;
    case 19: emin = 20; break;
    default : emin = 46;
  }
  edFieldStr->Text = FloatToStr(emin);
}
//---------------------------------------------------------------------------

