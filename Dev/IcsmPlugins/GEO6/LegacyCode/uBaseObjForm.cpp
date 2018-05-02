//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uBaseObjForm.h"
#include <Db.hpp>
#include "uMainDm.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TfrmBaseObjForm *frmBaseObjForm;
//---------------------------------------------------------------------------
__fastcall TfrmBaseObjForm::TfrmBaseObjForm(TComponent* Owner)
    : TForm(Owner), objId(0), dataChanged(false)
{
}
//---------------------------------------------------------------------------
void __fastcall TfrmBaseObjForm::actOkExecute(TObject *Sender)
{
    ModalResult = mrNone; // if exception will rise, form will not close
    if (actApply->Enabled)
        actApplyExecute(Sender);
    ModalResult = mrOk;
}
//---------------------------------------------------------------------------
void __fastcall TfrmBaseObjForm::actApplyExecute(TObject *Sender)
{
    SaveData();
    LoadData();
}
//---------------------------------------------------------------------------
void __fastcall TfrmBaseObjForm::actRefreshExecute(TObject *Sender)
{
    LoadData();
}
//---------------------------------------------------------------------------
void __fastcall TfrmBaseObjForm::actCloseExecute(TObject *Sender)
{
    Close();
}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseObjForm::LoadData()
{
    if (tr->InTransaction)
        tr->RollbackRetaining();
    else
        tr->StartTransaction();

    if (objId == 0)
        throw *(new Exception(__FUNC__"(): objId == 0"));

    for (int i = 0; i < ComponentCount; i++)
    {
        TDataSet *ds = dynamic_cast<TDataSet*>(Components[i]);
        if (ds)
        {
            ds->Close();
            TIBDataSet *ibds = dynamic_cast<TIBDataSet*>(Components[i]);
            if (ibds && ibds->Params->Names.Pos("OBJ_ID") > 0)
                ibds->ParamByName("OBJ_ID")->AsInteger = objId;
            if (ibds && ibds->Params->Names.Pos("ID") > 0)
                ibds->ParamByName("ID")->AsInteger = objId;
            ds->Open();
        }
    }

    Caption = m_Caption + " #" + IntToStr(objId) + ((dscObj->DataSet->State == dsInsert) ? " (Новий)" : "");

    dataChanged = false;
}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseObjForm::SaveData()
{
    if (dscObj->DataSet)
        dscObj->DataSet->Post();
    for (int i = 0; i < ComponentCount; i++)
    {
        TDataSet *ds = dynamic_cast<TDataSet*>(Components[i]);
        if (ds && dsEditModes.Contains(ds->State))
            ds->Post();
    }
    tr->CommitRetaining();
}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseObjForm::actApplyUpdate(TObject *Sender)
{
    if (!dataChanged && dscObj->DataSet && dsEditModes.Contains(dscObj->DataSet->State))
        dataChanged = true;
    if (!dataChanged)
    {
        for (int i = 0; i < ComponentCount; i++)
        {
            TDataSet *ds = dynamic_cast<TDataSet*>(Components[i]);
            if (ds && dsEditModes.Contains(ds->State))
            {
                dataChanged = true;
                break;
            }
        }
    }
    actApply->Enabled = dataChanged;
    actRefresh->Enabled = (dscObj->DataSet->State == dsInsert) ? false : dataChanged;
}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseObjForm::FormShow(TObject *Sender)
{
    m_Caption = Caption;
    int pos = 0;
    while (pos = m_Caption.Pos("&"))
        m_Caption.Delete(pos, 1);
    if (m_Caption.Trim().IsEmpty())
        m_Caption = "???";
    LoadData();
}
//---------------------------------------------------------------------------

