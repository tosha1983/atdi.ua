//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uCalcParamsDlg.h"
#include "uFrmTxBaseAir.h"
#include "uFrmTxTVA.h"
#include "uMainDm.h"
#include "uSectorDlg.h"
#include "uTable36.h"
#include "FormProvider.h"
#include "uParams.h"
#include <values.h>
#include <math>
#include <Registry.hpp>
#include "uAnalyzer.h"
#include <strutils.hpp>
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uFrmTxBase"
#pragma link "xlcClasses"
#pragma link "xlEngine"
#pragma link "xlReport"
#pragma link "NumericEdit"
#pragma link "CustomMap"
#pragma link "uLisObjectGrid"
#pragma resource "*.dfm"

//  блядская ATL
#ifdef StrToInt
#undef StrToInt
#endif
//---------------------------------------------------------------------------
__fastcall TfrmTxBaseAir::TfrmTxBaseAir(TComponent* Owner, ILISBCTx *in_Tx)
        : TfrmTxBase(Owner, in_Tx)
{
    t36_flag_eprH = false;
    t36_flag_eprV = false;
    t36_flag_Heff = false;
    t36_flag_Gain_h = NULL;
    t36_flag_Discr_h = NULL;
    t36_flag_Gain_v = NULL;
    t36_flag_Discr_v = NULL;

    if (edtSummAtten->Field && edtSummAtten->Field->OnChange == NULL)
        edtSummAtten->Field->OnChange = ibdsAirSUMMATORATTENUATIONChange;

    erp_cng = false;

    SetRadiationClass();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::acceptListElementSelection(Messages::TMessage &Message)
{
    TfrmTxBase::acceptListElementSelection(Message);
}

void __fastcall TfrmTxBaseAir::TxDataLoad()
{
    TfrmTxBase::TxDataLoad();

    ibdsAir->Active = false;
       ibdsAir->ParamByName("ID")->AsInteger = ibdsStantionsBaseID->AsInteger;
    ibdsAir->Active = true;

// мощность
    double DValue;
    long LValue;
    Tx->get_power_sound_primary(&DValue);
    edtPowerAudio1->OldValue = FormatFloat("0.####", DValue);

    if (DValue > 0) PowerInput = piPOWER; else PowerInput = piERP;
    

    Tx->get_epr_sound_hor_primary(&DValue);
    edtEPRGAudio1->OldValue = FormatFloat("0.##", DValue);

    /* TODO : вставить массив effectpowerhor */

    Tx->get_epr_sound_vert_primary(&DValue);
    edtEPRVAudio1->OldValue = FormatFloat("0.##", DValue);
    /* TODO : вставить массив effectpowervert */

    Tx->get_epr_sound_max_primary(&DValue);
    edtEPRmaxAudio1->OldValue = FormatFloat("0.##", DValue);
// антенна
    TBCPolarization pol;
    Tx->get_polarization(&pol);
    cbxPolarization->ItemIndex = (int)pol;
    TBCDirection DirectionValue;
    Tx->get_direction(&DirectionValue);
    cbxDirect->ItemIndex = (int)DirectionValue;

    if((int)pol == 2)
    {
        edtEPRmaxAudio1->Enabled = false;
        edtEPRmaxAudio1->Color = clDisabledEdit;
    }

    AdjustDirControls();

    /* TODO : вставить массив эффект высот антенн */

    Tx->get_heightantenna(&LValue);
    edtHeight->OldValue = AnsiString(LValue);

    /* TODO : вставить массив effectheight */

    Tx->get_height_eft_max(&LValue);
    edtHeffMax->OldValue = AnsiString(LValue);

    Tx->get_antennagain(&DValue);
    edtGain->OldValue = FormatFloat("0.###", DValue);

    Tx->get_angleelevation_vert(&LValue);
    edtAngle->OldValue = AnsiString(LValue);

    Tx->get_angleelevation_hor(&LValue);
    edtAngle2->OldValue = FormatFloat("0.###", LValue);

    Tx->get_fiderloss(&DValue);
    edtFiderLoss->OldValue = FormatFloat("0.####", DValue);

    Tx->get_fiderlenght(&LValue);
    edtFiderLength->OldValue = AnsiString(LValue);

    if (ibdsAirSUMMATORPOWERS->AsInteger)
        chbSummator->State = cbChecked;
    else
        chbSummator->State = cbUnchecked;

    cbProgramm->ReadOnly = (ibdsStantionsBaseSTATUS->AsInteger != tsDraft);
}

void __fastcall TfrmTxBaseAir::TxDataSave()
{
    double DValue, power;
    if (ibdsAir->State == dsEdit)
        ibdsAir->Post();
    TBCDirection direct;
    Tx->get_direction(&direct);

    TBCTxType TxType;
    Tx->get_systemcast(&TxType);

    TBCPolarization pol;
    Tx->get_polarization(&pol);

    if (TxType == ttTV || TxType == ttDVB)
        Tx->get_power_video(&power);
    else
        Tx->get_power_sound_primary(&power);

    if (pol == plVER) {
        for (int i = 0 ; i < 36; i++) Tx->set_effectpowerhor(i, -999.0);
        Tx->set_epr_sound_hor_primary(-999.0);
    } else if (pol == plHOR) {
        for (int i = 0 ; i < 36; i++) Tx->set_effectpowervert(i, -999.0);
        Tx->set_epr_sound_vert_primary(-999.0);
    } else if (pol == plMIX){
        // ? - Это тут на какой хер?
        //for (int i = 0 ; i < 36; i++) Tx->set_effectpowervert(i, -999.0);
        //for (int i = 0 ; i < 36; i++) Tx->set_effectpowerhor(i, -999.0);
        //Tx->set_epr_sound_vert_primary(-999.0);
        //Tx->set_epr_sound_hor_primary(-999.0);
    }

    // если изvенялись данные. то нужно пересчитать максимальное координационное расстояние
    long data_changes;
    Tx->get_data_changes(&data_changes);
    if (data_changes) {
        double zone[36];
        setmem (zone, sizeof(double) * 36, 0);
        txAnalyzer.GetCoordinationZone(Tx, zone);

        double maxCoordDist = 50;
        for (int i = 0; i < 36; i++) {
            if (maxCoordDist < zone[i]) {
                maxCoordDist = zone[i];
            }
        }
        if ( maxCoordDist > 1050 )
            maxCoordDist = 1050;

        Tx->set_maxCoordDist(maxCoordDist);
    }

    TfrmTxBase::TxDataSave();
}

//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::cbxDirectChange(TObject *Sender)
{
    TBCDirection newDir = cbxDirect->ItemIndex == 0 ? drD : drND;

    Tx->set_direction(newDir);

    AdjustDirControls();

    if (newDir == drD)
    {
        if (btnAntPattH->Visible)
            btnAntPattClick(btnAntPattH);
        else if (btnAntPattV->Visible)
            btnAntPattClick(btnAntPattV);
    } else {
        // newDir == drND - let tx server object rearranges gain by directions
        double gain;
        OleCheck(Tx->get_antennagain(&gain));
        OleCheck(Tx->set_antennagain(gain));
    }

    RefreshAll();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::cbxPolarizationChange(TObject *Sender)
{
    char pol = cbxPolarization->Text.IsEmpty() ? 0 : cbxPolarization->Text[1];
    TBCPolarization p = plCIR;
    switch (pol)
    {
        case 'H': p = plHOR; break;
        case 'V': p = plVER; break;
        case 'M': p = plMIX; break;
        default : p = plCIR; break;
    }

    Tx->set_polarization(p);

    AdjustDirControls();

    if (btnAntPattH->Visible)
        btnAntPattClick(btnAntPattH);
    else if (btnAntPattV->Visible)
        btnAntPattClick(btnAntPattV);

    RefreshAll();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::gbAntennaExit(TObject *Sender)
{
    try {
        Tx->set_fiderlenght(StrToInt(edtFiderLength->Text));
    } catch (Exception &e) {
        long DValue;
        Tx->get_fiderlenght(&DValue);
        edtFiderLength->Text = AnsiString(DValue);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::btnEBPG1Click(TObject *Sender)
{
    erp_cng = true;
    TfrmTable36 *TableForm;
    if (!t36_flag_eprH) {
        t36_flag_eprH = true;
        TableForm = new TfrmTable36(this, t36EPRH, Tx);
        TableForm->Show();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::btnEBPV1Click(TObject *Sender)
{
    erp_cng = true;
    TfrmTable36 *TableForm;
    if (!t36_flag_eprV) {
        t36_flag_eprV = true;
        TableForm = new TfrmTable36(this, t36EPRV, Tx);
        TableForm->Show();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::btnHeffClick(TObject *Sender)
{
    TfrmTable36 *TableForm;
    if (!t36_flag_Heff) {
        t36_flag_Heff = true;
        TableForm = new TfrmTable36(this, t36HEFF, Tx);
        TableForm->Show();
    }
}
//---------------------------------------------------------------------------



void __fastcall TfrmTxBaseAir::ibdsAirAfterEdit(TDataSet *DataSet)
{
    actApply->Enabled = true;
    actLoad->Enabled = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::chbSummatorClick(TObject *Sender)
{
if (chbSummator->State == cbChecked)
    { gbSummator->Visible = true;
      if (!ibdsAirSUMMATORPOWERS->AsInteger) {
           if (ibdsAir->State != dsEdit) ibdsAir->Edit();
           ibdsAirSUMMATORPOWERS->AsInteger = 1;
           }
    }
      else {
      gbSummator->Visible = false;
      if (ibdsAirSUMMATORPOWERS->AsInteger) {
          if (ibdsAir->State != dsEdit) ibdsAir->Edit();
          ibdsAirSUMMATORPOWERS->AsInteger = 0;
          }
      }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::btnCalcHeffClick(TObject *Sender)
{
    double DValue, maxHeff = -MAXDOUBLE;
    long LValue;
/*
   Загружаем информацию о моделях рельефа из реестра
   с помощью TLISBCCalcParams *params_tmp;
*/
    TForm *dlg_select_relief;
    Application->CreateForm(__classid(TForm), &dlg_select_relief);
    dlg_select_relief->BorderStyle = bsDialog;
    dlg_select_relief->Caption = "Моделі рельєфа місцевості";

    dlg_select_relief->Width = 200;
    dlg_select_relief->Height = 100;
    int x, y;
    x = 5;
    y = 5;
    std::auto_ptr<TLISBCCalcParams> params_tmp(new TLISBCCalcParams());
    params_tmp->load();

    int lowb, highb;
    lowb = 0;
    highb = params_tmp->ReliefServerArray.size();

    for (int i=lowb; i < highb; i++){
        TButton *b;
        b = new TButton(&dlg_select_relief);
        b->ModalResult = -(i + 1);
        b->Parent = dlg_select_relief;
        b->Caption = params_tmp->ReliefServerArray[i].name;
        b->Top = y;
        b->Left = x;
        b->Width = dlg_select_relief->Width - 12;
        y += b->Height + 3;
        dlg_select_relief->Height = y +  b->Height;
    }

    TButton *b;
    b = new TButton(&dlg_select_relief);
    b->ModalResult = mrCancel;
    b->Parent = dlg_select_relief;
    b->Caption = "Відмінити";
    y += 10;
    b->Top = y;
    b->Left = x;
    b->Width = dlg_select_relief->Width - 12;
    dlg_select_relief->Height = y + 2* b->Height + 3;


    dlg_select_relief->Position = poMainFormCenter;
    dlg_select_relief->ModalResult = -1;
    int idx = dlg_select_relief->ShowModal();
    delete dlg_select_relief;

    if (idx != mrCancel)
    {
        idx = -idx;
        idx--;
        /*
        Величина ModalResult-1 каждой кнопки соответствует индексу в массиве ReliefServerArray.
        */
        params_tmp->ReliefServerArrayGUID = params_tmp->ReliefServerArray[idx].guid;
        params_tmp->ReliefServerName = params_tmp->ReliefServerArray[idx].name;
        params_tmp->ReliefPath = params_tmp->ReliefServerArray[idx].params;


        int defUseHeff = params_tmp->UseHeff;
        if (!defUseHeff)
            params_tmp->UseHeff = 1;
        params_tmp->Step = 0.05;

        params_tmp->load();

        if (params_tmp->FPathSrv.IsBound()) {

            for(int i = 0; i < 36 ; i++)
            {
                double buf;
                Tx->get_effectheight(i, &buf);
                data_36_old[i] = buf;

                Rsageography_tlb::TRSAGeoPathResults Results;

                Rsageography_tlb::TRSAGeoPathData Data;

                Tx->get_heightantenna(&LValue);
                Data.TxHeight = Data.RxHeight = (double)LValue;

                Rsageography_tlb::TRSAGeoPoint A;
                Tx->get_longitude(&DValue);
                A.L = DValue;
                Tx->get_latitude(&DValue);
                A.H = DValue;

                Rsageography_tlb::TRSAAzimuth Az = (double)i*10;
                Rsageography_tlb::TRSADistance Dist = 15.0;
           /*     double lon_out, lat_out;
                txAnalyzer.GetPoint(A.L, A.H, Az, 3.0, &lon_out, &lat_out);
                A.L = lon_out;
                A.H = lat_out;     */

                params_tmp->FPathSrv->RunOnAzimuth(A, Az, Dist,  Data, &Results);
                data_36[i] = Results.HEff;
            }
            /*
            if (!defUseHeff)
            {
                params_tmp->UseHeff = 0;
                params_tmp->save();
                params_tmp->load();
            }
            */

            TfrmTable36 *TableForm;
            if (!t36_flag_Heff)
            {
                t36_flag_Heff = true;
                TableForm = new TfrmTable36(this, t36HEFF, Tx);
                for(int i = 0 ; i < 36; i++)
                {
                    TableForm->sgTable36old->Cells[1][i] = FormatFloat("0",data_36_old[i]);
                    TableForm->sgTable36old->Cells[0][i] = AnsiString(i*10);
                    TableForm->sgTable36->Cells[1][i] = FormatFloat("0",data_36[i]);
                    TableForm->sgTable36->Cells[0][i] = AnsiString(i*10);
                }
                TableForm->sgTable36->ColWidths[0] = 0;
                TableForm->sgTable36->Width = 35;
                TableForm->Width += TableForm->sgTable36old->Width;
                TableForm->sgTable36old->Font->Color = clRed;
                TableForm->sgTable36->Font->Color = clBlue;
                TableForm->sgTable36old->Visible = true;
                TableForm->Shape1->Visible = true;
                TableForm->Label1->Visible = true;
                TableForm->Shape2->Visible = true;
                TableForm->Label2->Visible = true;
                TableForm->btnSaveNew->Visible = true;
                TableForm->Show();

                if (ibdsStantionsBaseSTATUS->AsInteger != tsDraft)
                    TableForm->btnSaveNew->Visible = false;
            }

        } else
            Application->MessageBox("Сервер рельєфу не завантажений", Application->Title.c_str(), MB_ICONERROR | MB_OK);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::btnTRKEnter(TObject *Sender)
{
    if (ibdsStantionsBaseSTATUS->AsInteger != tsDraft)
        ((TEdit *)Sender)->ReadOnly = true;
    else
        ((TEdit *)Sender)->ReadOnly = false;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::btnOperatorEnter(TObject *Sender)
{
    if (ibdsStantionsBaseSTATUS->AsInteger != tsDraft)
        ((TEdit *)Sender)->ReadOnly = true;
    else
        ((TEdit *)Sender)->ReadOnly = false;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::cbProgrammEnter(TObject *Sender)
{
    if (ibdsStantionsBaseSTATUS->AsInteger != tsDraft)
        ((TEdit *)Sender)->ReadOnly = true;
    else
        ((TEdit *)Sender)->ReadOnly = false;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::edtEPRmaxAudio1ValueChange(TObject *Sender)
{
    if (((TNumericEdit*)Sender)->Text.IsEmpty())
        ((TNumericEdit*)Sender)->Text = "0";

    double erp = StrToFloat(edtEPRmaxAudio1->Text);

    double eprgv = erp - 3;//10 * Log10(2);

    try {
        Tx->set_epr_sound_max_primary(erp);
        if(cbxPolarization->Text == "M")
        {
            Tx->set_epr_sound_hor_primary(eprgv);
            Tx->set_epr_sound_vert_primary(eprgv);
            edtEPRVAudio1->Text = FloatToStr(eprgv);
            edtEPRGAudio1->Text = FloatToStr(eprgv);
        }
    } catch (Exception &e) {
        double DValue;
        Tx->get_epr_sound_max_primary(&DValue);
        edtEPRmaxAudio1->OldValue = FormatFloat("0.##",DValue);
    }
    TBCPolarization pol;
    Tx->get_polarization(&pol);

    #ifdef _DEBUG
//        ShowMessage(__FUNC__"(): set_epr_sound_xxx_primary(), DValue = "+edtEPRmaxAudio1->Text);
    #endif
    if (pol==plHOR) {
        edtEPRGAudio1->Text = edtEPRmaxAudio1->Text;
        double DValue;
        DValue = StrToFloat(edtEPRGAudio1->Text);
        Tx->set_epr_sound_hor_primary(DValue);
        Tx->set_epr_sound_vert_primary(-999);
        edtEPRVAudio1->Text = "-999";
    }

    if (pol==plVER) {
        edtEPRVAudio1->Text = edtEPRmaxAudio1->Text;
        double DValue;
        DValue = StrToFloat(edtEPRVAudio1->Text);
        Tx->set_epr_sound_vert_primary(DValue);
        Tx->set_epr_sound_hor_primary(-999);
        edtEPRGAudio1->Text = "-999";
    }


    for(int i = 0; i < 36 ; i++) {
        if (pol==plHOR) {
                Tx->set_effectpowerhor(i, erp);
                Tx->set_effectpowervert(i, -999);
        }
        if (pol==plVER) {
                Tx->set_effectpowerhor(i, -999);
                Tx->set_effectpowervert(i, erp);
        }
    }
    erp_cng = true;
    RefreshAll();
    erp_cng = false;
//  RJustifyEdit((TEdit *)Sender);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::edtEPRGAudio1ValueChange(TObject *Sender)
{
    if (((TNumericEdit*)Sender)->Text.IsEmpty())
        ((TNumericEdit*)Sender)->Text = "0";

    double val1, val = -999;
    try {
        Tx->set_epr_sound_hor_primary(val = StrToFloat(edtEPRGAudio1->Text));
        
    } catch (Exception &e) {
        double DValue;
        Tx->get_epr_sound_hor_primary(&DValue);
        edtEPRGAudio1->OldValue = AnsiString(DValue);
    }
    if (val != -999) {
        double valV, max;
        Tx->get_epr_sound_vert_primary(&valV);
        if(cbxPolarization->Text == "M")
        {
            val1 = StrToFloat(edtEPRVAudio1->Text);
            max = 10 * Log10(pow(10, val / 10.) + pow(10, val1 / 10.));
            edtEPRmaxAudio1->Text = FloatToStr(max);
        }
        else
        {
            if (val > valV)
                max = val;
            else
                max = valV;
        }
        Tx->set_epr_sound_max_primary(max);
        edtEPRmaxAudio1->OldValue = FormatFloat("0.##",max);

    }
    erp_cng = true;
    RefreshAll();
    erp_cng = false;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::edtEPRVAudio1ValueChange(TObject *Sender)
{
    if (((TNumericEdit*)Sender)->Text.IsEmpty())
        ((TNumericEdit*)Sender)->Text = "0";

    double val1, val = -999;
    try {
        #ifdef _DEBUG
     //       ShowMessage(__FUNC__"(): set_epr_sound_vert_primary()");
        #endif
        Tx->set_epr_sound_vert_primary(val = StrToFloat(edtEPRVAudio1->Text));
        
    } catch (Exception &e) {
        double DValue;
        Tx->get_epr_sound_vert_primary(&DValue);
        edtEPRVAudio1->OldValue = FormatFloat("0.##",DValue);
    }
    if (val != -999) {
        double valG, max;
        Tx->get_epr_sound_hor_primary(&valG);
        if(cbxPolarization->Text == "M")
        {
            val1 = StrToFloat(edtEPRGAudio1->Text);
            max = 10 * Log10(pow(10, val / 10.) + pow(10, val1 / 10.));
            edtEPRmaxAudio1->Text = FloatToStr(max);
        }
        else
        {
            if (val > valG)
                max = val;
            else
                max = valG;
        }
        Tx->set_epr_sound_max_primary(max);
        edtEPRmaxAudio1->OldValue = FormatFloat("0.##",max);
        
    }
    erp_cng = true;
    RefreshAll();
    erp_cng = false;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::edtPowerAudio1ValueChange(TObject *Sender)
{
    if (((TNumericEdit*)Sender)->Text.IsEmpty())
        ((TNumericEdit*)Sender)->Text = "0";

    double LValue;
    double erp = 10. * log10(StrToFloat(edtPowerAudio1->Text));
    double eprgv = erp - 3;
    Tx->get_power_sound_primary(&LValue);  
    try {
        Tx->set_power_sound_primary(StrToFloat(edtPowerAudio1->Text));
        if(cbxPolarization->Text == "M")
        {
            Tx->set_epr_sound_hor_primary(eprgv);
            Tx->set_epr_sound_vert_primary(eprgv);
            edtEPRVAudio1->Text = FloatToStr(eprgv);
            edtEPRGAudio1->Text = FloatToStr(eprgv);
        }
    } catch (Exception &e) {
        Tx->get_power_sound_primary(&LValue);
        edtPowerAudio1->Text = AnsiString(LValue);
    }
    RefreshAll();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::edtHeightValueChange(TObject *Sender)
{
    if (((TNumericEdit*)Sender)->Text.IsEmpty())
        ((TNumericEdit*)Sender)->Text = "0";
    try {
        Tx->set_heightantenna(::StrToInt(edtHeight->Text));
    } catch (Exception &e) {
        long DValue;
        Tx->get_heightantenna(&DValue);
        edtHeight->OldValue = AnsiString(DValue);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::edtHeffMaxValueChange(TObject *Sender)
{
    if (((TNumericEdit*)Sender)->Text.IsEmpty())
        ((TNumericEdit*)Sender)->Text = "0";
    long oldVal;
    Tx->get_height_eft_max(&oldVal);
    try {
        long val = long(StrToFloat(edtHeffMax->Text));
        if (val != oldVal) {
            Tx->set_height_eft_max(val);
            edtHeffMax->OldValue = AnsiString(val);
        }
    } catch (Exception &e) {
        edtHeffMax->OldValue = AnsiString(oldVal);
    }
    AnsiString s;
    s = ((TComponent*)Sender)->Name;

    if (!AnsiContainsText(s, "frmTable36"))
    if (Application->MessageBox("Установити значення Неф макс. за всіма напрямками?", Application->Title.c_str(), MB_ICONQUESTION | MB_YESNO) == IDYES)
        for (int i=0; i<36; i++){
          Tx->set_effectheight(i, StrToFloat(edtHeffMax->Text));
        }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::edtGainValueChange(TObject *Sender)
{
    if (((TNumericEdit*)Sender)->Text.IsEmpty())
        ((TNumericEdit*)Sender)->Text = "0";

    double DValueOld,DValue = 0.0;
    Tx->get_antennagain(&DValueOld);
    try {
        OleCheck(Tx->set_antennagain(DValue = StrToFloat(edtGain->Text)));
    } catch (Exception &e) {
        Tx->get_antennagain(&DValue);
        edtGain->OldValue = AnsiString(DValue);
    }
    if ((fabs(DValue-DValueOld) > 0.0001)||(Sender != (TObject *)edtGain)) {
        if (dynamic_cast<TfrmTxTVA *>(this))
            ((TfrmTxTVA *)this)->edtPowerVideoValueChange(this);
        else
            edtPowerAudio1ValueChange(this);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::edtAngle2ValueChange(TObject *Sender)
{
    if (((TNumericEdit*)Sender)->Text.IsEmpty())
        ((TNumericEdit*)Sender)->Text = "0";
    try {
        Tx->set_angleelevation_hor(StrToInt(edtAngle2->Text));
    } catch (Exception &e) {
        long DValue;
        Tx->get_angleelevation_hor(&DValue);
        edtAngle2->OldValue = AnsiString(DValue);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::edtAngleValueChange(TObject *Sender)
{
    if (((TNumericEdit*)Sender)->Text.IsEmpty())
        ((TNumericEdit*)Sender)->Text = "0";
    try {
        Tx->set_angleelevation_vert(StrToInt(edtAngle->Text));
    } catch (Exception &e) {
        long DValue;
        Tx->get_angleelevation_vert(&DValue);
        edtAngle->OldValue = AnsiString(DValue);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::edtFiderLossValueChange(TObject *Sender)
{
    if (((TNumericEdit*)Sender)->Text.IsEmpty())
        ((TNumericEdit*)Sender)->Text = "0";
    try {
        Tx->set_fiderloss(StrToFloat(edtFiderLoss->Text));
        if (dynamic_cast<TfrmTxTVA *>(this))
            ((TfrmTxTVA *)this)->edtPowerVideoValueChange(this);
        else
            edtPowerAudio1ValueChange(this);
    } catch (Exception &e) {
        double DValue;
        Tx->get_fiderloss(&DValue);
        edtFiderLoss->OldValue = AnsiString(DValue);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::edtFiderLengthValueChange(TObject *Sender)
{
    if (((TNumericEdit*)Sender)->Text.IsEmpty())
        ((TNumericEdit*)Sender)->Text = "0";
    try {
        Tx->set_fiderlenght(StrToInt(edtFiderLength->Text));
        if (dynamic_cast<TfrmTxTVA *>(this))
            ((TfrmTxTVA *)this)->edtPowerVideoValueChange(this);
        else
            edtPowerAudio1ValueChange(this);
    } catch (Exception &e) {
        long LValue;
        Tx->get_fiderlenght(&LValue);
        edtFiderLength->OldValue = AnsiString(LValue);
    }
}
//---------------------------------------------------------------------------

/*
  В зависимости от типа ввода мощности оператором (вводим ЭИМ или
  вводим мощн. кВт отключает ненужные эдиты)
*/
void _fastcall TfrmTxBaseAir::SetPowerInput(TPowerInput pi)
{
    // if (!(_power_input==pi))
    {
        _power_input = pi;
        TColor erpcolor;
        TColor powercolor;
        bool flag_power_input = (pi == piPOWER);

        if (CheckBoxPower->Checked != flag_power_input) CheckBoxPower->Checked = flag_power_input;
        if (CheckBoxERP->Checked != !flag_power_input) CheckBoxERP->Checked = !flag_power_input;

        if (flag_power_input) {
            CheckBoxERP->Checked = false;
            erpcolor = clDisabledEdit;
            powercolor = clEnabledEdit;
        } else{
            CheckBoxPower->Checked = false;
            powercolor = clDisabledEdit;
            erpcolor = clEnabledEdit;
        }
        edtEPRVAudio1->Enabled = !flag_power_input;
        edtEPRGAudio1->Enabled = !flag_power_input;
        if(cbxPolarization->Text == "M")
        {
            edtEPRmaxAudio1->Enabled = false;
            edtEPRmaxAudio1->Color = clDisabledEdit;
        }
        else
        {
            edtEPRmaxAudio1->Enabled = !flag_power_input;
            edtEPRmaxAudio1->Color = erpcolor;
        }

        edtPowerAudio1->Enabled = flag_power_input;

        edtPowerAudio1->Color = powercolor;
        edtEPRVAudio1->Color = erpcolor;
        edtEPRGAudio1->Color = erpcolor;     

        edtFiderLength->Color = powercolor;
        edtFiderLoss->Color = powercolor;
        edtFiderLength->Enabled = flag_power_input;
        edtFiderLoss->Enabled = flag_power_input;

        edtGain->Enabled = flag_power_input;
        edtGain->Color = powercolor;

        lblAntDiscr->Caption = _power_input == piPOWER ? "Дiаграма пiдсил., дБ" : "Дiаграма послаб., дБ";
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::CheckBoxERPClick(TObject *Sender)
{
    if (((TCheckBox*)Sender)->Checked)
        PowerInput = piERP;
    else
        CheckBoxPower->Checked = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::CheckBoxPowerClick(TObject *Sender)
{
    if (((TCheckBox*)Sender)->Checked)
        PowerInput = piPOWER;
    else
        CheckBoxERP->Checked = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::FormCreate(TObject *Sender)
{
    TfrmTxBase::FormCreate(Sender);

    if (Tx)
    {
        TBCTxType tt;
        Tx->get_systemcast(&tt);
        double power;
        if (tt == ttTV)
            Tx->get_power_video(&power);
        else
            Tx->get_power_sound_primary(&power);
        PowerInput = (power > 0) ? piPOWER : piERP;
        
        TxToForm();
    } else
        PowerInput = piPOWER;

}
//---------------------------------------------------------------------------


/*
Обрабатывваем всю форму и записываем значения в объект передатчика
*/

void _fastcall TfrmTxBaseAir::FormToTx()
{
    TBCPolarization pol;
    Tx->get_polarization(&pol);
    ILisBcAntPattPtr antPatt;
    Tx->QueryInterface<ILisBcAntPatt>(&antPatt);
    if (!antPatt.IsBound())
        throw *(new Exception("Интерфейс ILisBcAntPatt не поддерживается сервером передатчиков"));

    if (PowerInput == piPOWER){
        if (edtPowerAudio1->Text.IsEmpty()) edtPowerAudio1->Text = 0;
        if (edtGain->Text.IsEmpty()) edtGain->Text = 0;
        if (edtFiderLoss->Text.IsEmpty()) edtFiderLoss->Text = 0;
        if (edtFiderLength->Text.IsEmpty()) edtFiderLength->Text = 0;

        double p_kw = StrToFloat(edtPowerAudio1->Text);
        double p_dbkw;

        if (p_kw <= 0)
        {
              edtEPRmaxAudio1->Text = "-999";
              p_dbkw = -999;
        }
        else
        {
            double gain = StrToFloat(edtGain->Text);
            double floss = StrToFloat(edtFiderLoss->Text);
            double flength = StrToFloat(edtFiderLength->Text);
            double summAtten = 0;
            if (edtSummAtten->Field)
                summAtten = edtSummAtten->Field->AsFloat;
            p_dbkw = 10 * log10(p_kw);
            double erp = p_dbkw + gain - floss * flength - summAtten;
            edtEPRmaxAudio1->Text = FormatFloat("0.##", erp);

            double maxerp=-999;
            double maxerp_h=-999;
            double maxerp_v=-999;
            double tmp_maxerp;
            for (int i=0; i<36; i++){
                double gain_h;
                double gain_v;
                antPatt->get_gain_h(i, &gain_h);
                antPatt->get_gain_v(i, &gain_v);
                double erp_h;
                double erp_v;
                if (pol == plHOR || pol == plVER)
                {
                    erp_h = p_dbkw + gain_h - floss * flength - summAtten;
                    erp_v = p_dbkw + gain_v - floss * flength - summAtten;
                }
                else if(pol == plMIX)
                {
                    erp_h = p_dbkw - 3 + gain_h - floss * flength - summAtten;
                    erp_v = p_dbkw - 3 + gain_v - floss * flength - summAtten;
                }
                if (pol == plHOR && erp_h > maxerp)
                    maxerp = erp_h;
                if (pol == plVER && erp_v > maxerp)
                    maxerp = erp_v;
                if (pol == plMIX)
                {
                if (erp_h > maxerp_h)
                    maxerp_h = erp_h;
                if (erp_v > maxerp_v)
                    maxerp_v = erp_v;
                tmp_maxerp = 10. * Log10(pow(10, erp_v / 10.) + pow(10, erp_h / 10.)) - 0.01;
                if (tmp_maxerp > maxerp)
                    maxerp = tmp_maxerp;
                }

                Tx->set_effectpowerhor(i, -999);
                Tx->set_effectpowervert(i, -999);
                if (pol == plHOR || pol == plMIX)
                    Tx->set_effectpowerhor(i, erp_h);
                if (pol == plVER || pol == plMIX)
                    Tx->set_effectpowervert(i, erp_v);

            }
            
            Tx->set_epr_sound_max_primary(maxerp);
            Tx->set_epr_sound_hor_primary(-999);
            Tx->set_epr_sound_vert_primary(-999);
            if (pol == plHOR)
                Tx->set_epr_sound_hor_primary(maxerp);
            if (pol == plVER)
                Tx->set_epr_sound_vert_primary(maxerp);
            if (pol == plMIX) {
                Tx->set_epr_sound_hor_primary(maxerp_h);
                Tx->set_epr_sound_vert_primary(maxerp_v);
            }
        }

    }

    /*
    Если вводится непосредственно ЭИМ, то надо убрать все
    лишние параметры - мощность кВт, фидер, усиление и т.п.
    */
    if (PowerInput == piERP){
        if (edtEPRmaxAudio1->Text.IsEmpty()) edtEPRmaxAudio1->Text = 0;
        double temp_h, dh = -999.0;
        double temp_v, dv = -999.0;
        double erp = StrToFloat(edtEPRmaxAudio1->Text);
        Tx->set_epr_sound_max_primary(erp);
        Tx->set_power_sound_primary(0);
        Tx->set_fiderloss(0);
        Tx->set_fiderlenght(0);
        double gainmax;
        Tx->get_antennagain(&gainmax);
        // Оставим - по правильному надо бы нормализовать, вообщесьто
        //Tx->set_antennagain(0);
        ILisBcAntPattPtr antPatt;
        Tx->QueryInterface<ILisBcAntPatt>(&antPatt);
        if (!antPatt.IsBound())
            throw *(new Exception("Интерфейс ILisBcAntPatt не поддерживается сервером передатчиков"));

        double maxerp = -999;
        double tmp_maxerp;
        double erp_h, maxh = -999;
        double erp_v, maxv = -999;
        if(erp_cng) {
        if (pol == plMIX)
        {
            for (int i = 0; i < 36; i++)
            {
                Tx->get_effectpowervert(i, &temp_v);
                Tx->get_effectpowerhor(i, &temp_h);
                if(temp_v > dv)
                    dv = temp_v;
                if(temp_h > dh)
                    dh = temp_h;
            }
            dh = StrToFloat(edtEPRGAudio1->Text) - dh;
            dv = StrToFloat(edtEPRVAudio1->Text) - dv;
        }
          }
        for (int i = 0; i < 36; i++){
            double gain_h;
            double gain_v;
            antPatt->get_gain_h(i, &gain_h);
            antPatt->get_gain_v(i, &gain_v);
            if (pol == plHOR) {
                Tx->set_effectpowerhor(i, erp + gain_h - gainmax);
                Tx->set_effectpowervert(i, -999);
                antPatt->set_gain_h(i, gain_h - gainmax);
                antPatt->set_gain_v(i, 0);
            }
            else if (pol == plVER) {
                Tx->set_effectpowervert(i, erp + gain_v - gainmax);
                Tx->set_effectpowerhor(i, -999);
                antPatt->set_gain_h(i, 0);
                antPatt->set_gain_v(i, gain_v - gainmax);
            }
            else {
                Tx->get_effectpowervert(i, &temp_v);
                Tx->get_effectpowerhor(i, &temp_h);
                if(erp_cng)
                {
                    erp_v = temp_v + dv;
                    erp_h = temp_h + dh;
                }
                else
                {
                    erp_v = temp_v;
                    erp_h = temp_h;
                }
                Tx->set_effectpowervert(i, erp_v);
                Tx->set_effectpowerhor(i, erp_h);
                antPatt->set_gain_h(i, gain_h - gainmax);
                antPatt->set_gain_v(i, gain_v - gainmax);
                
                tmp_maxerp = 10. * Log10(pow(10, erp_v / 10.) + pow(10, erp_h / 10.)) - 0.01;
                if (tmp_maxerp > maxerp)
                    maxerp = tmp_maxerp;
                if(erp_h > maxh)
                    maxh = erp_h;
                if(erp_v > maxv)
                    maxv = erp_v;
            }
        }
        Tx->set_antennagain(0);
        if (pol == plHOR) {
            Tx->set_epr_sound_hor_primary(erp);
            Tx->set_epr_sound_vert_primary(-999);
        }
        else if (pol == plVER) {
            Tx->set_epr_sound_vert_primary(erp);
            Tx->set_epr_sound_hor_primary(-999);
        }
        else if (pol == plMIX) {
          //  temp_h = StrToFloat(edtEPRGAudio1->Text);
          //  temp_v = StrToFloat(edtEPRVAudio1->Text);
            Tx->set_epr_sound_max_primary(maxerp);
            Tx->set_epr_sound_hor_primary(maxh);
            Tx->set_epr_sound_vert_primary(maxv);
        }
        else {
            Tx->set_epr_sound_vert_primary(erp);
            Tx->set_epr_sound_hor_primary(erp);
        }
        if(erp_cng)
            erp_cng = false;
    }       

    actLoad->Enabled = true;
    if (ibdsStantionsBaseSTATUS->AsInteger == tsDraft) actApply->Enabled = true;
}
//---------------------------------------------------------------------------

/*
  Данные из объекта передатчика отображаем на форму
*/

void _fastcall TfrmTxBaseAir::TxToForm()
{
    double erp;
    double power;
    double gain;
    double loss;
    long len;

    Tx->get_power_sound_primary(&power);

    if (power > 0) PowerInput = piPOWER; else PowerInput = piERP;

    Tx->get_epr_sound_max_primary(&erp);
    edtEPRmaxAudio1->OldValue = FormatFloat("0.##", erp);

    Tx->get_epr_sound_hor_primary(&erp);
    edtEPRGAudio1->OldValue = FormatFloat("0.##", erp);

    Tx->get_epr_sound_vert_primary(&erp);
    edtEPRVAudio1->OldValue = FormatFloat("0.##", erp);

    Tx->get_power_sound_primary(&power);
    edtPowerAudio1->OldValue = FormatFloat("0.####", power);

    Tx->get_antennagain(&gain);
    edtGain->OldValue = FormatFloat("0.###", gain);

    Tx->get_fiderloss(&loss);
    edtFiderLoss->OldValue = FormatFloat("0.###", loss);

    Tx->get_fiderlenght(&len);
    edtFiderLength->OldValue = IntToStr(len);

}
//---------------------------------------------------------------------------


void TfrmTxBaseAir::RefreshAll()
{
    // Обновляем форму
    FormToTx();
    TxToForm();
}
//---------------------------------------------------------------------------

const char* TfrmTxBaseAir::CalcVideoEmission()
{
    TBCTvSystems tvsys;
    TBCTvStandards std;
    TBCTxType txtype;
    TBCDVBSystem dvbsys;

    Tx->get_typesystem(&tvsys);
    Tx->get_systemcolor(&std);
    Tx->get_systemcast(&txtype);
    Tx->get_dvb_system(&dvbsys);

    if ((txtype == ttTV) && (tvsys == tvD)) return "7M25C3F";
    if ((txtype == ttTV) && (tvsys == tvK)) return "7M25C3F";
    if ((txtype == ttTV) && (tvsys == tvB)) return "6M25C3F";
    if ((txtype == ttTV) && (tvsys == tvG)) return "6M25C3F";
    if ((txtype == ttDVB)) return "8M00X7FXF";

    return "";
}

const char* TfrmTxBaseAir::CalcSoundEmission()
{
    TBCFMSystem fmsys;
    // TBCTvSystems tvsys;
    TBCTxType txtype;

    Tx->get_fm_system(&fmsys);
    Tx->get_systemcast(&txtype);

    if (txtype == ttDVB) return "8M00X7FXF";
    else if (txtype == ttDAB) return "8M00X7FXF";
    else if ((txtype == ttFM) && (fmsys == fm1)) return "180KF3EGN";
    else if ((txtype == ttFM) && (fmsys == fm2)) return "130KF3EGN";
    else if ((txtype == ttFM) && (fmsys == fm3)) return "180KF3EHN";
    else if ((txtype == ttFM) && (fmsys == fm4)) return "300KP9EHF";
    else if ((txtype == ttFM) && (fmsys == fm5)) return "300KF8EHN";
    else if (txtype == ttTV) return "750KF3EGN";
    else return "";
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAir::ibdsAirSUMMATORATTENUATIONChange(
      TField *Sender)
{
    // recalc ERP values
    if (dynamic_cast<TfrmTxTVA *>(this))
        ((TfrmTxTVA *)this)->edtPowerVideoValueChange(this);
    else
        edtPowerAudio1ValueChange(this);
}
//---------------------------------------------------------------------------

void TfrmTxBaseAir::AdjustDirControls()
{
    TBCDirection dir;
    Tx->get_direction(&dir);
    TBCPolarization pol;
    Tx->get_polarization(&pol);

    bool isDirected = dir == drD;
    lblAntDiscr->Visible = isDirected;
    btnEBPG1->Visible = isDirected;
    btnEBPV1->Visible = isDirected;

    bool isVer = pol == plVER;
    bool isHor = pol == plHOR;

    if (dir == drND)
    {
        btnAntPattH->Visible = false;
        btnAntPattV->Visible = false;
    } else {
        btnAntPattH->Visible = !isVer;
        btnAntPattV->Visible = !isHor;
        lblAntDiscr->Top = isHor ? btnAntPattH->Top + (btnAntPattH->Height - lblAntDiscr->Height) / 2 :
                           isVer ? btnAntPattV->Top + (btnAntPattV->Height - lblAntDiscr->Height) / 2 :
            btnAntPattH->Top + (btnAntPattV->Top - btnAntPattH->Top + btnAntPattV->Height - lblAntDiscr->Height) / 2;
    }

    edtEPRVAudio1->Visible = !isHor;
    btnEBPV1->Visible = !isHor;
    lblEBPV->Visible = !isHor;
    edtEPRGAudio1->Visible = !isVer;
    btnEBPG1->Visible = !isVer;
    lblEBPG->Visible = !isVer;
}

void __fastcall TfrmTxBaseAir::btnAntPattClick(TObject *Sender)
{
    TForm** tableForm = &t36_flag_Discr_h;
    Table36type type = t36DISCR_H;

    if (_power_input == piERP)
    {
        if (Sender == btnAntPattV)
        {
            type = t36DISCR_V;
            tableForm = &t36_flag_Discr_v;
        }
    }
    else if (_power_input == piPOWER)
    {
        if (Sender == btnAntPattV)
        {
            type = t36GAIN_V;
            tableForm = &t36_flag_Gain_v;
        } else {
            type = t36GAIN_H;
            tableForm = &t36_flag_Gain_h;
        }
    } else
        tableForm = NULL;

    if (tableForm && !(*tableForm)) {
        (*tableForm) = new TfrmTable36(this, type, Tx);
        (*tableForm)->Show();
    }

}
//---------------------------------------------------------------------------




