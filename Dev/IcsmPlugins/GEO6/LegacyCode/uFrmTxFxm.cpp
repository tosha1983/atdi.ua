//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uFrmTxFxm.h"
#include "uParams.h"
#include "uOtherParams.hpp"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "CustomMap"
#pragma link "uFrmTxBaseAir"
#pragma link "uLisObjectGrid"
#pragma link "NumericEdit"
#pragma resource "*.dfm"
TfrmTxFxm *frmTxFxm;
#ifdef StrToInt
#undef StrToInt
#endif

//---------------------------------------------------------------------------
__fastcall TfrmTxFxm::TfrmTxFxm(TComponent* Owner, ILISBCTx *in_Tx)
    : Inherited(Owner, in_Tx)
{
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxFxm::SetRadiationClass()
{

}
//---------------------------------------------------------------------------

void __fastcall TfrmTxFxm::AcceptListElementSelection(Messages::TMessage &Message)
{
    Inherited::acceptListElementSelection(Message);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxFxm::TxDataLoad()
{
    cbServ->Items->Clear();
    cbServ->Items->AddObject("Undefined", (TObject*)NULL);
    typedef struct { char* n; unsigned long v; } InitValues;
    InitValues init_values[] = {
        { "AL", osAL },
        { "CA", osCA },
        { "DA", osDA },
        { "DB", osDB },
        { "IA", osIA },
        { "MA", osMA },
        { "MT", osMT },
        { "MU", osMU },
        { "M1", osM1 },
        { "M2", osM2 },
        { "RA1", osRA1 },
        { "RA2", osRA2 },
        { "R1", osR1 },
        { "R3", osR3 },
        { "R4", osR4 },
        { "XA", osXA },
        { "XB", osXB },
        { "XE", osXE },
        { "XM", osXM },
        { "AA8", osAA8 },
        { "AB", osAB },
        { "BA", osBA },
        { "AA2", osAA2 },
        { "BC", osBC },
        { "BD", osBD },
        { "FF", osFF },
        { "FH", osFH },
        { "FK", osFK },
        { "FK7", osFK7 },
        { "FK8", osFK8 },
        { "NX", osNX },
        { "NR", osNR },
        { "NS", osNS },
        { "NT", osNT },
        { "NA", osNA },
        { "NB", osNB },
        { "NB8", osNB8 },
        { "XG", osXG },
        { "PL", osPL },
        { "NY", osNY },
        { "XA8", osXA8 },
        { "XB8", osXB8 },
        { "ZC8C", osZC8C },
        { "ZC8N", osZC8N }
    };

    for (int i = 0; i < sizeof(init_values) / sizeof(InitValues); i++)
    {
        TObject *p = (TObject*)init_values[i].v;
        cbServ->Items->AddObject(init_values[i].n, p);
    }
    cbServ->DropDownCount = cbServ->Items->Count;

    Inherited::TxDataLoad();

    double freq = 0.;
    Tx->get_sound_carrier_primary(&freq);
    edFreq->OldValue = freq > 0. ? FormatFloat("0.###", freq) : String();

    ILisBcFxmPtr fxm;
    OleCheck(Tx->QueryInterface(IID_ILisBcFxm, (void**)&fxm));

    unsigned long st = osUnknown;
    fxm->get_fxm_system(&st);
    cbServ->ItemIndex = cbServ->Items->IndexOfObject((TObject*)st);

    double bw = 0.;
    fxm->get_fxm_bandwidth(&bw);
    edBw->OldValue = FormatFloat("0.###", bw * 1000.);

    dsFxm->Close();
    dsFxm->Open();

}
//---------------------------------------------------------------------------

void __fastcall TfrmTxFxm::TxDataSave()
{
    Inherited::TxDataSave();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxFxm::edFreqValueChange(TObject *Sender)
{
    double freq = 0.;
    try {
        if (edFreq->Text.Length() > 0)
            freq = StrToFloat(edFreq->Text);
        HrCheck(Tx->set_sound_carrier_primary(freq));
        HrCheck(Tx->set_video_carrier(freq));
    } catch (Exception &e) {
        Tx->get_sound_carrier_primary(&freq);
        Application->ShowException(&e);
    }
    edFreq->OldValue = freq > 0. ? FormatFloat("0.###", freq) : String();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxFxm::edBwValueChange(TObject *Sender)
{
    double bw = 0.;
    if (edBw->Text.Length() > 0)
    {
        bw = StrToFloat(edBw->Text) / 1000;
        ILisBcFxmPtr fxm;
        OleCheck(Tx->QueryInterface(IID_ILisBcFxm, (void**)&fxm));
        HrCheck(fxm->set_fxm_bandwidth(bw));
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxFxm::cbServChange(TObject *Sender)
{
    ILisBcFxmPtr fxm;
    OleCheck(Tx->QueryInterface(IID_ILisBcFxm, (void**)&fxm));
    unsigned long st = osUnknown;
    fxm->get_fxm_system(&st);
    if (st != (unsigned long)cbServ->Items->Objects[cbServ->ItemIndex])
        fxm->set_fxm_system((unsigned long)cbServ->Items->Objects[cbServ->ItemIndex]);
}
//---------------------------------------------------------------------------


