//---------------------------------------------------------------------------

#include <vcl.h>

#include <DBTables.hpp>

#include <memory>
#include "uMainDm.h"
#include "FormProvider.h"
#pragma hdrstop

#include "uListDocTemplate.h"
#include "DlgSelectTypeDoc.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseList"
#pragma resource "*.dfm"
TfrmListDocTemplate *frmListDocTemplate;
//---------------------------------------------------------------------------
__fastcall TfrmListDocTemplate::TfrmListDocTemplate(TComponent* Owner)
    : TfrmBaseList(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListDocTemplate::TfrmListDocTemplate(TComponent* Owner, HWND callerWin, int elementId)
    : TfrmBaseList(Owner, callerWin, elementId)
{
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDocTemplate::actListEditExecute(TObject *Sender)
{
    std::auto_ptr<TSelectTypeDoc> SelectTypeDoc (new TSelectTypeDoc(Application));
    SelectTypeDoc->objId = dstList->FieldByName("ID")->AsInteger;
    SelectTypeDoc->Caption = FormProvider.GetObjectName(otDocTemplate);
    SelectTypeDoc->dscObj->DataSet = dsrList->DataSet;
    dstList->Edit();
    SelectTypeDoc->ShowModal();
}
//---------------------------------------------------------------------------
void __fastcall TfrmListDocTemplate::FormCreate(TObject *Sender)
{
    TfrmBaseList::FormCreate(Sender);
}
//---------------------------------------------------------------------------
void __fastcall TfrmListDocTemplate::dstListNewRecord(TDataSet *DataSet)
{
    std::auto_ptr<TSelectTypeDoc> SelectTypeDoc (new TSelectTypeDoc(Application));
    SelectTypeDoc->objId = dmMain->getNewId();
    SelectTypeDoc->Caption = FormProvider.GetObjectName(otDocTemplate);
    SelectTypeDoc->dscObj->DataSet = dstList;
    SelectTypeDoc->ShowModal();
}
//---------------------------------------------------------------------------
void __fastcall TfrmListDocTemplate::dstListBeforePost(TDataSet *DataSet)
{
  /*  TfrmBaseList::dstListBeforePost(DataSet);
    if (tmpContainer->Modified)
        saveContainer();   */
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDocTemplate::DownloadTemplate(AnsiString& fileName, TDataSet *dataSet, Db::TField* field)
{
/*    char tempPath[256];
    DWORD tempPathLength = GetTempPath(256, tempPath);
    if ( tempPathLength > 0 )
    {
        fileName = AnsiString(tempPath, tempPathLength) + "Template" + Now().FormatString("yyyymmddhhmmss") + ".doc";

        std::auto_ptr<TFileStream> fileStream(new TFileStream(fileName, fmCreate | fmShareDenyWrite));

        std::auto_ptr<TStream> stream(dataSet->CreateBlobStream(field, bmRead));

        stream->Position = 0;
        fileStream->CopyFrom(stream.get(), stream->Size);
    }       */
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDocTemplate::dstListAfterScroll(TDataSet *DataSet)
{
 //   if (dstList->RecordCount && (dstList->State == dsBrowse))
 //       loadUpContainer();
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDocTemplate::loadUpContainer()
{
 /*   tmpContainer->DestroyObject();
    TStream* blobStream = dstList->CreateBlobStream(dstListTEMPLATE, bmRead);
    try {
        if (blobStream->Size != 0) {
            blobStream->Seek(0,0);
            try {
                tmpContainer->LoadFromStream(blobStream);
            } catch (Exception& E) {
                if (Application->MessageBox((AnsiString("Не могу прочесть документ: ")+E.Message+"\nСоздать новый?").c_str(),Application->Title.c_str(), MB_ICONQUESTION | MB_YESNO) == IDYES)
                    createNewContainer();
            }
        } else {
            if (Application->MessageBox("Поле документа пустое. Создать новый документ?", Application->Title.c_str(), MB_ICONQUESTION | MB_YESNO) == IDYES)
                createNewContainer();
        }
    } __finally {
        blobStream->Free();
    }
    tmpContainer->Modified = false;    */
}
//---------------------------------------------------------------------------
void __fastcall TfrmListDocTemplate::createNewContainer()
{
/*
        TCreateInfo info;
        info.CreateType = ctNewObject;
        info.ShowAsIcon = false;
        info.IconMetaPict = 0;
        info.FileName = "";
        info.DataObject = NULL;


    std::auto_ptr<TSelectTypeDoc> SelectTypeDoc (new TSelectTypeDoc(Application));
    FileName = "";
    TypeDoc = -1;
    SelectTypeDoc->ShowModal();
    switch (TypeDoc)  {
    case tdWord: info.ClassID = ProgIDToClassID("WordDocument");
                 try {
                    tmpContainer->CreateObjectFromInfo(info);
                 } catch (Exception& e) {
                    throw *(new Exception(AnsiString("Помилка створення WORD-шаблону!" + e.Message).c_str()));
                 }
        break;
    case tdExcel: info.ClassID = ProgIDToClassID("ExcelWorksheet");
                 try {
                    tmpContainer->CreateObjectFromInfo(info);
                 } catch (Exception& e) {
                    throw *(new Exception(AnsiString("Помилка створення ЕХСЕL-шаблону!" + e.Message).c_str()));
                 }
        break;
    case tdFromFile:

        try {
            tmpContainer->DestroyObject();
            tmpContainer->CreateObjectFromFile(FileName, true);
            TStream* blobStream = dstList->CreateBlobStream(dstListTEMPLATE, bmWrite);
            if (tmpContainer->State != osEmpty) {
                blobStream->Seek(0,0);
                tmpContainer->SaveToStream(blobStream);
            }
        } catch (Exception &e) {
            throw *(new Exception(AnsiString("Помилка створення шаблону з файлу " + OpenDialog1->FileName + e.Message).c_str()));
            dstList->Cancel();
        }
        break;
    default : dstList->Cancel();;
    }
           */
}
//---------------------------------------------------------------------------
void __fastcall TfrmListDocTemplate::saveContainer()
{
 /*   TStream* blobStream = dstList->CreateBlobStream(dstListTEMPLATE, bmWrite);
    try {
        if (tmpContainer->State != osEmpty) {
            blobStream->Seek(0,0);
            tmpContainer->SaveToStream(blobStream);
        }
    } __finally {
        blobStream->Free();
    }
    tmpContainer->Modified = false;        */
}
//---------------------------------------------------------------------------


void __fastcall TfrmListDocTemplate::dgrListKeyDown(TObject *Sender,
      WORD &Key, TShiftState Shift)
{
 /*   TfrmBaseList::dgrListKeyDown(Sender, Key, Shift);
    if (!m_caller) {
        if (Key == 13 && Shift == TShiftState()) {
            dstListNAME->AsString = dstListNAME->AsString;
            tmpContainer->DoVerb(ovShow);
        }
    }     */
}
//---------------------------------------------------------------------------
/*
void __fastcall TfrmListDocTemplate::dgrListDblClick(TObject *Sender)
{
   try
    {
        TfrmBaseList::dgrListDblClick(Sender);
        if (!m_caller)
        {
            if ( dstListTTYPE->AsString.LowerCase() == "word" ) // pifolio
            {
                AnsiString templatePath;
                DownloadTemplate(templatePath, dstList, dstListTEMPLATE2);

                bool modified = false;
                ShowWordDocumentAndWait(templatePath, modified);

                if ( modified )
                    UpdateDocument(dstListTEMPLATE2, dstList, templatePath);

                DeleteFile(templatePath);
            }
            else if( dstListTTYPE->AsString.LowerCase() == "excel" )
            {
                tmpContainer->DoVerb(ovShow);
            }
            else // old style word bookmarks
                tmpContainer->DoVerb(ovShow);
        }          

    }
    catch (Exception& e)
    {
        Application->ShowException(&e);
    }
}      */
//---------------------------------------------------------------------------

void __fastcall TfrmListDocTemplate::ShowWordDocumentAndWait(AnsiString& path, bool& modified)
{
 /*   Variant word = CreateOleObject("Word.Application");
      word.OlePropertySet("Visible", true);
    Variant wordDocument = word.OlePropertyGet("Documents").OleFunction("Open", WideString(path), false);

    try
    {
        while ( 1 )
        {
            path = AnsiString(wordDocument.OlePropertyGet("FullName"));
            if ( !modified )
                modified = !(wordDocument.OlePropertyGet("Saved"));
            Sleep(100);
        }
    }
    catch(...)
    {
    }

    wordDocument = Unassigned;
    word = Unassigned;        */
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDocTemplate::UpdateDocument(TField* field, TDataSet *dataSet, AnsiString filePath)
{
  /*  try
    {
        dataSet->Edit();

        std::auto_ptr<TFileStream> file(new TFileStream(filePath, fmOpenRead | fmShareDenyWrite));

        TStream* stream = dataSet->CreateBlobStream(field, bmWrite);
        stream->Size = 0;
        stream->CopyFrom(file.get(), file->Size);

        dataSet->Post();
    }
    catch(...)
    {
    }      */
}
//---------------------------------------------------------------------------



