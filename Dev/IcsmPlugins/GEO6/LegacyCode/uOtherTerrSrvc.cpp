//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uOtherTerrSrvc.h"
#include "uMainDm.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseObjForm"
#pragma resource "*.dfm"
TfrmOtherTerrSrvc *frmOtherTerrSrvc;
//---------------------------------------------------------------------------
__fastcall TfrmOtherTerrSrvc::TfrmOtherTerrSrvc(TComponent* Owner)
    : TfrmBaseObjForm(Owner)
{
    dmMain->GetList("select ID, CODE from COUNTRY where ID >= 0 order by 2", cbAdm->Items);
    cbTxCtry->Items->AddStrings(cbAdm->Items);
    cbRxCtry->Items->AddStrings(cbAdm->Items);
    sgEah->Cells[0][0] = "Кут";
    sgEah->Cells[1][0] = "Вис.";
    for (int i = 0; i < 36; i++)
        sgEah->Cells[0][i+1] = String().sprintf("%d\xB0", i*10);
}
//---------------------------------------------------------------------------

void __fastcall TfrmOtherTerrSrvc::dstObjNewRecord(TDataSet *DataSet)
{
    dscObj->DataSet->FieldByName("COORD_REQ")->AsInteger = true;
    dscObj->DataSet->FieldByName("SIGNED_COMM")->AsInteger = false;
    sgEah->Enabled = false;
    sgEah->Visible = false;
    btAllMax->Enabled = false;
    btAllMax->Visible = false;

}
//---------------------------------------------------------------------------

void __fastcall TfrmOtherTerrSrvc::chRotatingClick(TObject *Sender)
{
    Label33->Enabled = !chRotating->Checked;
    Label34->Enabled = chRotating->Checked;
    Label35->Enabled = chRotating->Checked;
    edAzm->Enabled = !chRotating->Checked;
    edAzmStart->Enabled = chRotating->Checked;
    edAzmEnd->Enabled = chRotating->Checked;
    edAzm->Font->Color = !chRotating->Checked ? clWindowText: clBtnFace;
    edAzmStart->Font->Color = chRotating->Checked ? clWindowText: clBtnFace;
    edAzmEnd->Font->Color = chRotating->Checked ? clWindowText: clBtnFace;
    edAzm->Color = !chRotating->Checked ? clWindow: clBtnFace;
    edAzmStart->Color = chRotating->Checked ? clWindow: clBtnFace;
    edAzmEnd->Color = chRotating->Checked ? clWindow: clBtnFace;
}
//---------------------------------------------------------------------------

void __fastcall TfrmOtherTerrSrvc::dstObjAfterOpen(TDataSet *DataSet)
{
    chRotating->Checked = dscObj->DataSet->FieldByName("AZM")->IsNull;
    chRotatingClick(this);
    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Transaction = tr;
    sql->SQL->Text = "select EAH from OTHER_PRIM_TERR where ID = "+IntToStr(objId);
    sql->ExecQuery();
    short eah[36];
    bool eahOk = false;
    if (!sql->Eof && !sql->Fields[0]->IsNull && dmMain->GetIbArray(tr, sql->Fields[0], eah, sizeof(eah)))
        eahOk = true;
    for (int i = 0; i < 36; i++)
        sgEah->Cells[1][i+1] = eahOk ? IntToStr(eah[i]) : String();
    eahChanged = false;
}
//---------------------------------------------------------------------------

void __fastcall TfrmOtherTerrSrvc::sgEahDrawCell(TObject *Sender, int ACol,
      int ARow, TRect &Rect, TGridDrawState State)
{
    if (sgEah->Cells[ACol][ARow].Length() > 0)
    {
        sgEah->Canvas->FillRect(Rect);
        Rect.bottom--;
        ::DrawText(sgEah->Canvas->Handle,
                    sgEah->Cells[ACol][ARow].c_str(), sgEah->Cells[ACol][ARow].Length(),
                    &Rect, (ARow == 0 ? DT_CENTER : (Rect.right-=2, DT_RIGHT)) | DT_BOTTOM | DT_SINGLELINE);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmOtherTerrSrvc::sgEahSetEditText(TObject *Sender,
      int ACol, int ARow, const AnsiString Value)
{
    int hmax = 0;
    if (sgEah->Cells[1][1].Length() > 0)
        try { hmax = sgEah->Cells[1][1].ToInt(); } catch (...) {}
    for (int i = 2; i <= 36; i++)
        try {
            int h = sgEah->Cells[1][i].ToInt();
            if (hmax < h) hmax = h;
        } catch (...) {}
    eahChanged = true;
    dataChanged = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmOtherTerrSrvc::sgEahExit(TObject *Sender)
{
    RefreshEah();
}
//---------------------------------------------------------------------------

void __fastcall TfrmOtherTerrSrvc::RefreshEah()
{
    for (int i = 0; i < 36; i++)
    {
        int val = 0;
        try { val = sgEah->Cells[1][i+1].ToInt(); } catch(...) {}
        sgEah->Cells[1][i+1] = IntToStr(val);
    }
}

void __fastcall TfrmOtherTerrSrvc::SaveData()
{
    if (eahChanged && sgEah->Visible)
    {
        std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
        sql->Transaction = tr;
        sql->SQL->Text = "update OTHER_PRIM_TERR set EAH = :EAH where ID = "+IntToStr(objId);
        short eah[36];
        for (int i = 0; i < 36; i++)
            try { eah[i] = sgEah->Cells[1][i+1].ToInt(); }
            catch (...) { sgEah->Cells[1][i+1] = IntToStr(eah[i] = 0); };

        if (dmMain->PutIbArray(tr, sql->Params->Vars[0], eah, sizeof(eah)))
            sql->ExecQuery();
        eahChanged = false;
    }

    TfrmBaseObjForm::SaveData();

    sgEah->Enabled = true;
    sgEah->Visible = true;
    btAllMax->Enabled = true;
    btAllMax->Visible = true;
}

void __fastcall TfrmOtherTerrSrvc::btAllMaxClick(TObject *Sender)
{
    String val = dscObj->DataSet->FieldByName("MAX_EAH")->AsString;
    for (int i = 1; i <= 36; i++)
        sgEah->Cells[1][i] = val;
    eahChanged = true;
    dataChanged = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmOtherTerrSrvc::CoordFieldGetText(TField *Sender,
      AnsiString &Text, bool DisplayText)
{
     dmMain->CoordFieldGetText(Sender, Text, DisplayText);
}
//---------------------------------------------------------------------------

void __fastcall TfrmOtherTerrSrvc::CoordFieldSetText(TField *Sender,
      const AnsiString Text)
{
     dmMain->CoordFieldSetText(Sender, Text);
}
//---------------------------------------------------------------------------

