//---------------------------------------------------------------------------

#include <vcl.h>
#include <math.h>
#include <values.h>
#pragma hdrstop

#include "uTable36.h"

#include "TxBroker.h"
#include "uFrmTxBaseAirDigital.h"
#include "uFrmTxTVA.h"
#include "uFrmTxLfMf.h"
#include "uMainDm.h"
#include "uParams.h"
#include "uSelection.h"

//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TfrmTable36 *frmTable36;
//---------------------------------------------------------------------------
__fastcall TfrmTable36::TfrmTable36(TComponent* Owner, Table36type in_type, ILISBCTx *in_Tx)
    : TForm(Owner)
{
    type = in_type;
    Tx = in_Tx;
    frmTx = dynamic_cast<TfrmTxBase*>(Owner);
    if (frmTx == NULL)
        throw *(new Exception("TfrmTable36: Owner is not TfrmTxBase"));
}
//---------------------------------------------------------------------------

using namespace std;

void __fastcall TfrmTable36::FormCreate(TObject *Sender)
{
    sgTable36->ColWidths[0] = 25;
    sgTable36->ColWidths[1] = 35;
    sgTable36old->ColWidths[0] = 25;
    sgTable36old->ColWidths[1] = 35;                                           

    Top = frmTx->Top + 50;
    Left = frmTx->Left + 50*((int)type);

    LPSAFEARRAY degradationSector = NULL;

    if ( type == t36DegradationSector )
    {
        TBCTxType  txtype;
        Tx->get_systemcast(&txtype);

        if ( (txtype == ttTV) || (txtype == ttDVB) )
            BCCalcParams.FCalcSrv.GetErpDegradation(1, BCCalcParams.treshVideo, &degradationSector);
        else
            BCCalcParams.FCalcSrv.GetErpDegradation(1, BCCalcParams.treshAudio, &degradationSector);

        sgTable36->Options -= TGridOptions() << goEditing;
    }

    ILisBcAntPattPtr ap;
    if (type == t36GAIN_H || type == t36DISCR_H || type == t36GAIN_V || type == t36DISCR_V)
    {
        Tx->QueryInterface<ILisBcAntPatt>(&ap);
        if (!ap.IsBound())
            throw *(new Exception("Интерфейс ILisBcAntPatt не поддерживается сервером передатчиков"));
    }

    double maxgain;
    Tx->get_antennagain(&maxgain);
    double DValue;
    long bounds[2];
    if (!sgTable36old->Visible)
        for (long i = 0; i < 36; i++) {
            sgTable36->Cells[0][i] = AnsiString(i*10);
            switch (type) {
                case t36EPRH:
                    Tx->get_effectpowerhor(i, &DValue);
                    sgTable36->Cells[1][i] = FormatFloat("0.#",DValue);
                    break;
                case t36EPRV:
                    Tx->get_effectpowervert(i, &DValue);
                    sgTable36->Cells[1][i] = FormatFloat("0.#",DValue);
                    break;
                case t36HEFF:
                    Tx->get_effectheight(i, &DValue);
                    sgTable36->Cells[1][i] = FormatFloat("0",DValue);
                    break;
                case t36GAIN_H:
                    ap->get_gain_h(i, &DValue);
                    sgTable36->Cells[1][i] = FormatFloat("0.##",DValue);
                    break;
                case t36GAIN_V:
                    ap->get_gain_v(i, &DValue);
                    sgTable36->Cells[1][i] = FormatFloat("0.##",DValue);
                    break;
                case t36DISCR_H:
                    ap->get_gain_h(i, &DValue);
                    DValue = maxgain - DValue;
                    sgTable36->Cells[1][i] = FormatFloat("0.##",DValue);
                    break;
                case t36DISCR_V:
                    ap->get_gain_v(i, &DValue);
                    DValue = maxgain - DValue;
                    sgTable36->Cells[1][i] = FormatFloat("0.##",DValue);
                    break;
                case t36DegradationSector:
                    Tx->get_effectpowerhor(i, &DValue);
                    sgTable36->Cells[1][i] = FormatFloat("0.#",DValue);
                    SafeArrayGetElement(degradationSector, &i, &DValue);
                    sgTable36old->Cells[1][i] = FormatFloat("0.##", DValue);
                    sgTable36old->Cells[0][i] = IntToStr(i*10);
                    break;
                default: ;
            }
        }

    switch (type) {
        case t36EPRH:
                Caption = "Діаграма ефективної потужності за азимутом (гор)";
                sgTable36->Enabled = false;
                sgTable36old->Enabled = false;
                break;
        case t36EPRV:
                Caption = "Діаграма ефективної потужності за азимутом (верт)";
                sgTable36->Enabled = false;
                sgTable36old->Enabled = false;
                break;
        case t36HEFF: Caption = "Діаграма ефективних висот за азимутом";   break;
        case t36GAIN_H: Caption = "Діаграма коефіцієнтів підсилення антени за азимутом (H)";   break;
        case t36DISCR_H: Caption = "Діаграма послаблення антени за азимутом (H)";   break;
        case t36GAIN_V: Caption = "Діаграма коефіцієнтів підсилення антени за азимутом (V)";   break;
        case t36DISCR_V: Caption = "Діаграма послаблення антени за азимутом (V)";   break;
        case t36DegradationSector: Caption = "Діаграма сектора послаблення";   break;
        default: ;
    }

    data_changes = false;
/*********** valick ******************
    if (frmTx->ibdsStantionsBaseSTATUS->AsInteger != 1)
        sgTable36->Enabled = false;
    else
*********** valick ******************/

    if ( type == t36DegradationSector )
    {
        SafeArrayDestroy(degradationSector);


        sgTable36old->Show();
        Width += sgTable36old->Width;
    }

        sgTable36->Enabled = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTable36::FormClose(TObject *Sender, TCloseAction &Action)
{
    double val;
    double maxVal = -MAXDOUBLE;
    TfrmTxBaseAir* frm = dynamic_cast<TfrmTxBaseAir*>(frmTx);

    ILisBcAntPattPtr antPatt;
    if (type == t36GAIN_H || type == t36DISCR_H || type == t36GAIN_V || type == t36DISCR_V)
    {
        Tx->QueryInterface<ILisBcAntPatt>(&antPatt);
        if (!antPatt.IsBound())
            MessageBox(NULL, "Интерфейс ILisBcAntPatt не поддерживается сервером передатчиков", "Error", MB_ICONHAND);
    }

    if (data_changes) {
        double maxgain;
        Tx->get_antennagain(&maxgain);

        for(int ARow=0 ; ARow< 36; ARow++)
        {
           if (!sgTable36old->Visible)
           {
                try {
                    val = StrToFloat(sgTable36->Cells[1][ARow]);
                } catch (...) {
                    val = 0;
                }
           } else {
                try {
                    val = StrToFloat(sgTable36old->Cells[1][ARow]);
                } catch (...) {
                    val = 0;
                }
           }
           maxVal = max(maxVal, val);

           switch (type)
           {
                case t36EPRH:
                    Tx->set_effectpowerhor(ARow, val);
                    break;
                case t36EPRV:
                    Tx->set_effectpowervert(ARow, val);
                    break;
                case t36HEFF:
                    Tx->set_effectheight(ARow, val);
                    break;
                case t36GAIN_H:
                    if (antPatt.IsBound())
                        antPatt->set_gain_h(ARow, val);
                    break;
                case t36DISCR_H:
                    if (antPatt.IsBound())
                    {
                        double temp1, temp2;
                        Tx->get_effectpowerhor(ARow, &temp1);
                        antPatt->get_gain_h(ARow, &temp2);
                        temp1 += maxgain - val - temp2;
                        Tx->set_effectpowerhor(ARow, temp1);
                        antPatt->set_gain_h(ARow, maxgain - val);
                    }
                    break;
                case t36GAIN_V:
                    if (antPatt.IsBound())
                        antPatt->set_gain_v(ARow, val);
                    break;
                case t36DISCR_V:
                    if (antPatt.IsBound())
                    {
                        double temp1, temp2;
                        Tx->get_effectpowervert(ARow, &temp1);
                        antPatt->get_gain_v(ARow, &temp2);
                        temp1 += maxgain - val - temp2;
                        Tx->set_effectpowervert(ARow, temp1);
                        antPatt->set_gain_v(ARow, maxgain - val);
                    }
                    break;
                default:
                    break;
            }
        }

        TBCTxType tt = ttUNKNOWN;
        Tx->get_systemcast(&tt);
        switch (type)
        {
            case t36EPRH:
                if (tt == ttTV)
                    Tx->set_epr_video_hor(maxVal);
                else
                    Tx->set_epr_sound_hor_primary(maxVal);
                if (tt == ttTV)
                {
                    ((TfrmTxTVA *)frmTx)->edtEPRGVideo->OldValue = FormatFloat("0.##",maxVal);
                    try {
                    } catch (...) {;}
                } else {
                    frm->edtEPRGAudio1->OldValue = FormatFloat("0.##",maxVal);
                    try {
                        if (maxVal > StrToFloat(frm->edtEPRGAudio1->Text))
                        Tx->set_epr_sound_max_primary(maxVal);
                        frm->edtEPRmaxAudio1->OldValue = FormatFloat("0.##",maxVal);
                    } catch (...) {;}
                }
                break;
            case t36EPRV:
                if (tt == ttTV)
                    Tx->set_epr_video_vert(maxVal);
                else
                    Tx->set_epr_sound_vert_primary(maxVal);
                if (tt == ttTV)
                    ((TfrmTxTVA *)frmTx)->edtEPRVVideo->Text = FormatFloat("0.#",maxVal);
                else
                    frm->edtEPRVAudio1->OldValue = FormatFloat("0.##",maxVal);
                break;
            case t36HEFF:
                if (sgTable36old->Visible == false)
                    frm->edtHeffMax->OldValue = FormatFloat("0",maxVal);
                frm->edtHeffMaxValueChange(this);

                break;
            case t36GAIN_H: case t36GAIN_V:
                //frm->edtGain->OldValue = FormatFloat("0.###",maxVal);
                //frm->edtGainValueChange(this);
                break;
            default:
                break;;
        }
    } // data_changes

    switch (type)
    {
        case t36EPRH:
            if (frm)
                frm->t36_flag_eprH = false;
            break;
        case t36EPRV:
            if (frm)
                frm->t36_flag_eprV  = false;
            break;
        case t36HEFF:
            if (frm)
                frm->t36_flag_Heff  = false;
            sgTable36old->Visible = false;
            Shape1->Visible = false;
            Label1->Visible = false;
            Shape2->Visible = false;
            Label2->Visible = false;
            break;
        case t36GAIN_H:
            if (frm)
                frm->t36_flag_Gain_h  = NULL;
            break;
        case t36DISCR_H:
            if (frm)
                frm->t36_flag_Discr_h  = NULL;
            break;
        case t36GAIN_V:
            if (frm)
                frm->t36_flag_Gain_v  = NULL;
            break;
        case t36DISCR_V:
            if (frm)
                frm->t36_flag_Discr_v  = NULL;
            break;
        default:
            break;
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTable36::FormDeactivate(TObject *Sender)
{
    Close();
    TfrmTxBaseAir* frm = dynamic_cast<TfrmTxBaseAir*>(frmTx);
    if (frm)
    {
        frm->FormToTx();
        frm->TxToForm();
    } else {
        TfrmTxLfMf* frm = dynamic_cast<TfrmTxLfMf*>(frmTx);
        frm->ShowOperData();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTable36::sgTable36SetEditText(TObject *Sender, int ACol, int ARow, const AnsiString Value)
{
    data_changes = true;
    Repaint();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTable36::FormPaint(TObject *Sender)
{
    int centreX = Width/2 + (sgTable36->Left+sgTable36->Width)/2;
    int centreY = Height/2-15;
    int b_x, b_y, x, y;

    double array[36];double array2[36];
    double max = -MAXDOUBLE;
    double min = MAXDOUBLE;
    array[0] = 0.0;
    array2[0] = 0.0;

/******************valick***************************/
    double erp;
    TBCPolarization pol;
    Tx->get_polarization(&pol);
    TBCTxType  txtype;
    Tx->get_systemcast(&txtype);
    /*
    if (txtype == ttTV)
        Tx->get_epr_video_max(&erp);
    else
    {
        if (pol == plVER)
            Tx->get_epr_sound_vert_primary(&erp);
        else
            Tx->get_epr_sound_hor_primary(&erp);
    }
    */
    erp = 0;
    /*
    if (type == t36DISCR_H || type == t36DISCR_V)
        Tx->get_antennagain(&erp);
    */
    #ifdef _DEBUG
    Hint = String().sprintf("erp = %f\n", erp);
    #endif

/******************valick***************************/

    try {
        if (sgTable36->Cells[1][0].Length() > 0){
            array[0] = min = max = StrToFloat(sgTable36->Cells[1][0]);

/******************valick***************************/
            if (type == t36DISCR_H || type == t36DISCR_V) array[0] = erp - array[0];
            if (type == t36GAIN_H || type == t36GAIN_V) array[0] = erp + array[0];
            min = max = array[0];
/******************valick***************************/

        }
    } catch (...) {
        array[0] = -999;
        max = -999; min = -999;
    }
    if (sgTable36old->Visible)
        try {
            if (sgTable36old->Cells[1][0].Length() > 0)
                array2[0] = StrToFloat(sgTable36old->Cells[1][0]);
            else
                array2[0] = 0.0;
        } catch (...) {
            array2[0] =  -999;
        }


    for(int i = 0; i < 36; i++) {

        double val;
        try {
            if (sgTable36->Cells[1][i].Length() > 0){
                val = array[i] = StrToFloat(sgTable36->Cells[1][i]);
/******************valick***************************/
                if (type == t36DISCR_H || type == t36DISCR_V) array[i] = erp - array[i];
                if (type == t36GAIN_H || type == t36GAIN_V) array[i] = erp + array[i];
                val = array[i];
/******************valick***************************/
            }
            else
                val = array[i] = 0.0;

        } catch (...) {
            val = array[i] = -999;
        }
        if (val > max) max = val;
        if (val < min) min = val;



        if (sgTable36old->Visible) {
            try {
                if (sgTable36old->Cells[1][i].Length() > 0)
                {
                    if (type == t36DegradationSector)
                        val = array2[i] = StrToFloat(sgTable36->Cells[1][i]) - StrToFloat(sgTable36old->Cells[1][i]);
                    else
                        val = array2[i] = StrToFloat(sgTable36old->Cells[1][i]);
                }
                else
                    val = array2[i] = 0.0;
            } catch (...) {
                val = array2[i] = -999;
            }
        }
        if (val > max) max = val;
        if (val < min) min = val;
    }

    #ifdef _DEBUG
    Hint += (String().sprintf("min = %f, max = %f\n", min, max));
    #endif // _DEBUG

    if (min < 0) {
        max += -min;
        for(int i = 0; i < 36; i++) {
            array[i] += -min;
            array2[i] += -min;
        }
    }
    if (max==0) {
        for(int i = 0; i < 36; i++) array[i]=1;
        max = 1;
    }

    if ((type == t36EPRH)||(type == t36EPRV)) {
        double dlt = (max - min) / 4;
        for(int i = 0; i < 36; i++) array[i] += dlt;
        max += dlt;
    }

    double scale = ((double)centreY)/max*0.9;
    double os1 = max*scale;;
    Canvas->Pen->Color = clBlack;
    Canvas->Pen->Width = 1;
    Canvas->Ellipse(centreX-os1, centreY-os1,centreX+os1,centreY+os1);
    Canvas->Ellipse(centreX-os1*0.75, centreY-os1*0.75,centreX+os1*0.75,centreY+os1*0.75);
    Canvas->Ellipse(centreX-os1*0.5, centreY-os1*0.5,centreX+os1*0.5,centreY+os1*0.5);
    Canvas->Ellipse(centreX-os1*0.25, centreY-os1*0.25,centreX+os1*0.25,centreY+os1*0.25);

    double line_len = centreY-5;
    for (int i = 0; i < 18; i++ ) {
        x = centreX - line_len * sin(0.1745*i);
        y = centreY - line_len * cos(0.1745*i);
        Canvas->MoveTo(x, y);
        x = centreX + line_len * sin(0.1745*i);
        y = centreY + line_len * cos(0.1745*i);
        Canvas->LineTo(x, y);
    }
    x = b_x = centreX;
    try {
        y = b_y = centreY - array[0]*scale;
    } catch (...) {
        y = b_y = centreY;
    }
    Canvas->MoveTo(x, y);
    Canvas->Pen->Color = clBlue;
    Canvas->Pen->Width = 3;
    for (int i = 1; i < 36; i++ ) {
        try {
            x = centreX + array[i]*scale*sin(0.1746*i);
            y = centreY - array[i]*scale*cos(0.1746*i);
            Canvas->LineTo(x, y);
        } catch (...) {;}
    }
    Canvas->LineTo(b_x, b_y);
    //////////////////////////////////////////
    #ifdef _DEBUG
    //ShowMessage(__FUNC__"():"+IntToStr(__LINE__)+", in_type = "+IntToStr(in_type));
    for (int i = 0; i < 36; i++ )
        Hint += (FloatToStr(array[i])+'\n');
    #endif // _DEBUG

    if (sgTable36old->Visible) {
        Canvas->Pen->Color = clRed;
        x = b_x = centreX;
        try {
            y = b_y = centreY - array2[0]*scale;
        } catch (...) {
            y = b_y = centreY;
        }
        Canvas->MoveTo(x, y);
        for (int i = 1; i < 36; i++ ) {
            try {
                x = centreX + array2[i]*scale*sin(0.1746*i);
                y = centreY - array2[i]*scale*cos(0.1746*i);
                Canvas->LineTo(x, y);
            } catch (...) {;}
        }
        Canvas->LineTo(b_x, b_y);
    }
    Canvas->Font->Color = clGreen;
    Canvas->TextOutA(centreX-3,0,"0");
    Canvas->TextOutA(centreX-8,Height-45,"180");
    Canvas->TextOutA(sgTable36->Left+sgTable36->Width+10,centreY-15,"270");
    Canvas->TextOutA(Width-30,centreY-15,"90");
}
//---------------------------------------------------------------------------


void __fastcall TfrmTable36::btnSaveNewClick(TObject *Sender)
{
    double val;
    double max = -MAXDOUBLE;
    if (Application->MessageBox("Записати розраховані значення?", Application->Title.c_str(), MB_ICONQUESTION | MB_YESNO) == IDYES)
    {
        for (int i = 0; i < 36; i++)
        {
            sgTable36old->Cells[1][i] = sgTable36->Cells[1][i];
            try {
                if ((val = StrToFloat(sgTable36old->Cells[1][i])) > max)
                    max = val;
            } catch (...) {;}
        }
        data_changes = true;
        Repaint();
        Tx->set_height_eft_max(max);
        ((TfrmTxBaseAir*)frmTx)->edtHeffMax->Text = FormatFloat("0.",max);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTable36::sgTable36DrawCell(TObject *Sender, int ACol,
      int ARow, TRect &Rect, TGridDrawState State)
{
    TStringGrid *sg = dynamic_cast<TStringGrid*>(Sender);
    if (!sg)
        return;
    AnsiString text = Trim(sg->Cells[ACol][ARow]);
    int xOffset = Rect.Right - Rect.left - sg->Canvas->TextWidth(text) - 7 ;
    sg->Canvas->TextRect(Rect, Rect.left + xOffset, Rect.Top, text);
}
//---------------------------------------------------------------------------


void __fastcall TfrmTable36::FormKeyDown(TObject *Sender, WORD &Key,
      TShiftState Shift)
{
 if (Key == 27) Close();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTable36::sgTable36KeyPress(TObject *Sender, char &Key)
{
    AnsiString str = sgTable36->Cells[sgTable36->Col][sgTable36->Row];
    if (((Key < 48) || (Key > 57)) && (Key != '\b'))
    if ((str.c_str()[str.Length()-1] == '.')||(str.c_str()[str.Length()-1] == ','))
        Key = 0;
    else if ((DecimalSeparator == '.') && (Key == ','))
        Key = '.';
    else if ((DecimalSeparator == ',') && (Key == '.'))
        Key = ',';
    else if ((DecimalSeparator == ',') && (Key == ','))
        ;
    else if ((DecimalSeparator == '.') && (Key == '.'))
        ;
    else if (Key == '-') {
        if (str.Length() > 0 && str[1] == '-')
            Key = 0;
    } else
        Key = 0;
}
//---------------------------------------------------------------------------


void __fastcall TfrmTable36::sgTable36KeyDown(TObject *Sender, WORD &Key,
      TShiftState Shift)
{
    if ((Key == 13)&& (sgTable36->Row < 35))
        sgTable36->Row = sgTable36->Row + 1;
}
//---------------------------------------------------------------------------


