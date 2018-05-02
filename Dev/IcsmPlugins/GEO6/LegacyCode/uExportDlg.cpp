//---------------------------------------------------------------------
#include <vcl.h>
#pragma hdrstop

#include "uExportDlg.h"
#include "uMainForm.h"
#include "TxBroker.h"
#include "uMainDm.h"
//---------------------------------------------------------------------
#pragma resource "*.dfm"
TdlgExport *dlgExport;
TxExporter txExporter;
//---------------------------------------------------------------------
__fastcall TdlgExport::TdlgExport(TComponent* AOwner)
	: TForm(AOwner)
{
}
//---------------------------------------------------------------------
void __fastcall TdlgExport::btnFilenameClick(TObject *Sender)
{
    opdFile->FileName = ExtractFileName(edtFilename->Text);
    if (opdFile->Execute())
        edtFilename->Text = opdFile->FileName;
}
//---------------------------------------------------------------------------

void __fastcall TdlgExport::rgFormatClick(TObject *Sender)
{
    switch (rgFormat->ItemIndex) {
        case 0:
            opdFile->Filter = "����� TVA (*.TVA)|*.TVA|�� ����� (*.*)|*.*";
            opdFile->DefaultExt = "TVA";
            if (!edtFilename->Text.IsEmpty())
                edtFilename->Text = ChangeFileExt(edtFilename->Text, ".TVA");
            break;
        case 1:
            opdFile->Filter = "����� TVA (*.TVD)|*.TVD|�� ����� (*.*)|*.*";
            opdFile->DefaultExt = "TVD";
            if (!edtFilename->Text.IsEmpty())
                edtFilename->Text = ChangeFileExt(edtFilename->Text, ".TVD");
            break;
        default:
            opdFile->Filter = "�� ����� (*.*)|*.*";
            opdFile->DefaultExt = "";
            break;
    }
}
//---------------------------------------------------------------------------

void __fastcall TdlgExport::OKBtnClick(TObject *Sender)
{
    if (edtFilename->Text.IsEmpty()) {
        ModalResult = mrNone;
        throw Exception("�� ������ ��'� �����");
    }

    AnsiString dir = ExtractFilePath(edtFilename->Text);
    if (!DirectoryExists(dir)) {
        ModalResult = mrNone;
        throw Exception("�������� '" + dir + "'�� ����");
    }   
}
//---------------------------------------------------------------------------
void __fastcall TxExporter::exportTxGrid(TExpParam mode, TListKind listKind, TDBGrid* grid, AnsiString filename)
{
    TIBCustomDataSet* ds = NULL;
    if (grid && grid->DataSource && grid->DataSource->DataSet)
        ds = dynamic_cast<TIBCustomDataSet*>(grid->DataSource->DataSet);
    if(!ds)
        throw Exception("���� ����� ������������");

    // ������������� ������� ��������
    if (!expPtr.IsBound()) try {
        OLECHECK(expPtr.CreateInstance(CLSID_Exp));
        OLECHECK(expPtr->Init());
    } catch (Exception &e) {
        throw Exception("������� ����������� ������� ��������.\n" + e.Message);
    }

    TField* idField = ds->Fields->Fields[0];
    if (!idField)
        throw Exception("��������� ��������� ���� ��������������.");
    TField* scField = ds->FindField("SC_ENUMVAL");
    if (!scField)
        scField = ds->FindField("S_ENUMVAL");
    if (!scField)
        throw Exception("��������� ��������� ���� ���� ������� ('SC_ENUMVAL' ��� 'S_ENUMVAL').");

    // ���������� ������
    TCOMILISBCTxList exportList;
    exportList.CreateInstance(CLSID_LISBCTxList);

    int locateId = idField->AsInteger;
    ds->DisableControls();
    Screen->Cursor = crHourGlass;

    frmMain->StatusBar1->SimplePanel = true;

    try {
        frmMain->StatusBar1->SimpleText = "���������� ������...";
        frmMain->StatusBar1->Update();

        ds->FetchAll();
        ds->First();
        frmMain->pb->Visible = true;
        frmMain->pb->Min = 0;
        frmMain->pb->Position = 0;
        frmMain->pb->Max = ds->RecordCount;

        try {

            int expListCount = 0;
            while (!ds->Eof) {
                if (listKind == lkAll ||
                    listKind == lkSelected && grid->SelectedRows->CurrentRowSelected) {
                    exportList.AddTx(txBroker.GetTx(idField->AsInteger, dmMain->GetObjClsid(scField->AsInteger)));
                    exportList.set_TxUseInCalc(expListCount++, true);
                }
                ds->Next();
                frmMain->pb->StepBy(1);
                frmMain->pb->Update();
            }

            ds->Locate(idField->FieldName, locateId, TLocateOptions());
        } __finally {
            frmMain->pb->Visible = false;
        }


        frmMain->StatusBar1->SimpleText = "���������...";
        frmMain->StatusBar1->Update();

        txBroker.EnsureList(exportList, frmMain->pb);

        frmMain->StatusBar1->SimpleText = "�������...";
        frmMain->StatusBar1->Update();

        OLECHECK(expPtr->Export(WideString(filename), exportList, mode));

    } __finally {
        ds->EnableControls();
        Screen->Cursor = crDefault;
        frmMain->StatusBar1->SimplePanel = false;
    }
}

void __fastcall TxExporter::exportTxList(TExpParam mode, TListKind listKind, ILISBCTxList * pTxList, AnsiString filename)
//---------------------------------------------------------------------------
{
    // ������������� ������� ��������
    if (!expPtr.IsBound()) try {
        OLECHECK(expPtr.CreateInstance(CLSID_Exp));
        OLECHECK(expPtr->Init());
    } catch (Exception &e) {
        throw Exception("������� ����������� ������� ��������.\n" + e.Message);
    }

    TCOMILISBCTxList txList(pTxList, true);
    int txListSize = txList.Size;
    if (txListSize == 0)
        throw Exception("������ ������.");

    // ���������� ������
    TCOMILISBCTx tx(txList.get_Tx(0), true);
    TBCTxType txType = tx.get_systemcast();

    TCOMILISBCTxList exportList;
    exportList.CreateInstance(CLSID_LISBCTxList);
    exportList.AddTx(tx);
    exportList.set_TxUseInCalc(0, true);
    int expListCount = 1;
    for (int i = 1; i < txListSize; i++) {
        tx.Bind(txList.get_Tx(i), true);
        txType = tx.get_systemcast();
        if((mode == epTVA && txType == ttTV ||
            mode == epTVD && txType == ttDVB ||
            mode == epTVATVD && (txType == ttTV || txType == ttDVB))
          &&
            // ���� ������ ��� ������ ���������
           (listKind == lkAll ||
            listKind == lkSelected && txList.get_TxUseInCalc(i)))
        {
            exportList.AddTx(tx);
            exportList.set_TxUseInCalc(expListCount++, true);
        }
    }
    OLECHECK(expPtr->Export(WideString(filename), exportList, mode));
}

