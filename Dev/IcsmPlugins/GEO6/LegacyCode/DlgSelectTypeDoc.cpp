//---------------------------------------------------------------------
#include <vcl.h>
#pragma hdrstop

#include "DlgSelectTypeDoc.h"
#include "uMainDm.h"
#include <memory>
//---------------------------------------------------------------------
#pragma link "xlcClasses"
#pragma link "xlEngine"
#pragma link "xlReport"
#pragma resource "*.dfm"
//---------------------------------------------------------------------
__fastcall TSelectTypeDoc::TSelectTypeDoc(TComponent* AOwner)
	: TfrmBaseObjForm(AOwner)
{
// frm = AOwner;
}
//---------------------------------------------------------------------
void __fastcall TSelectTypeDoc::RadioGroup1Click(TObject *Sender)
{
 if (RadioGroup1->ItemIndex == 2) {
     edtFileName->Enabled = true;
     btnLoadDoc->Enabled = true;
     Label1->Enabled = true;
     TypeDoc  = tdFromFile;
     OpenDialog1->Filter = "Word files (*.doc)|*.DOC;";
 } else {
     if(RadioGroup1->ItemIndex == 1)
     {
        TypeDoc  = tdExcel;
        edtFileName->Enabled = false;
        btnLoadDoc->Enabled = false;
        Label1->Enabled = false;
     }
     else
     {
        TypeDoc  = tdWord;
        edtFileName->Enabled = true;
        btnLoadDoc->Enabled = true;
        Label1->Enabled = true;
        OpenDialog1->Filter = "PiFolio files (*.dot)|*.DOT;";
     }
 }
}
//---------------------------------------------------------------------------

void __fastcall TSelectTypeDoc::btnLoadDocClick(TObject *Sender)
{
    TDataSet* dstList = dscObj->DataSet;
    if(RadioGroup1->ItemIndex == 2 || RadioGroup1->ItemIndex == 0)
        if (OpenDialog1->Execute())
        {
            edtFileName->Text = OpenDialog1->FileName;
            FileName = edtFileName->Text;
        //    if(dstList->State != dsInsert)
        //    {
                try {
                    dstList->Edit();
                    if(TypeDoc == tdFromFile)
                    {
                        tmpContainer->DestroyObject();
                        tmpContainer->CreateObjectFromFile(FileName, true);
                        TStream* blobStream = dstList->CreateBlobStream(dstList->FieldByName("TEMPLATE"), bmWrite);
                        if (tmpContainer->State != osEmpty)
                        {
                            blobStream->Seek(0,0);
                            tmpContainer->SaveToStream(blobStream);
                        }
                    }
                    if(TypeDoc == tdWord)
                    {
                        std::auto_ptr<TFileStream> file(new TFileStream(FileName, fmOpenRead | fmShareDenyWrite));
                        TStream* stream = dstList->CreateBlobStream(dstList->FieldByName("TEMPLATE2"), bmWrite);
                        stream->Size = 0;
                        stream->CopyFrom(file.get(), file->Size);
                    }
                    } catch (Exception &e) {
                        throw *(new Exception(AnsiString("Помилка створення шаблону з файлу " + OpenDialog1->FileName + e.Message).c_str()));
                        dstList->Cancel();
                    }
        //    }
        }
}
//---------------------------------------------------------------------------

void __fastcall TSelectTypeDoc::LoadData()
{
    TfrmBaseObjForm::LoadData();
    TDataSet* ds = dscObj->DataSet;
    if(ds->State != dsInsert)
    {
         edTempCode->Text = ds->FieldByName("CODE")->AsString;
         edTempName->Text = ds->FieldByName("NAME")->AsString;
        if (ds->FieldByName("TTYPE")->AsString.IsEmpty())
        {
            RadioGroup1->ItemIndex = 2;
            TypeDoc = tdFromFile;
        }
        else
            if(ds->FieldByName("TTYPE")->AsString == "word")
            {
                RadioGroup1->ItemIndex = 0;
                TypeDoc = tdWord;
            }
            else
            {
                RadioGroup1->ItemIndex = 1;
                TypeDoc = tdExcel;
            }

        if(!ds->FieldByName("DOCTYPE")->IsNull && ds->FieldByName("DOCTYPE")->AsInteger != -1)
                rgDT->ItemIndex = ds->FieldByName("DOCTYPE")->AsInteger;
        else
                rgDT->ItemIndex = -1;

        if(!ds->FieldByName("RTTYPE")->IsNull && ds->FieldByName("RTTYPE")->AsInteger != -1)
                rgRadtech->ItemIndex = ds->FieldByName("RTTYPE")->AsInteger;
        else
                rgRadtech->ItemIndex = -1;

    }
    else
    {
        edTempCode->Text = "";
        edTempName->Text = "";
    }
}
//---------------------------------------------------------------------------

void __fastcall TSelectTypeDoc::SaveData()
{
  TDataSet* dstList = dscObj->DataSet;
  dstList->Edit();
  dstList->FieldByName("CODE")->AsString = edTempCode->Text;
  dstList->FieldByName("NAME")->AsString = edTempName->Text;

  if(rgDT->ItemIndex != -1)
        dstList->FieldByName("DOCTYPE")->AsInteger = rgDT->ItemIndex;
  if(rgRadtech->ItemIndex != -1)
        dstList->FieldByName("RTTYPE")->AsInteger = rgRadtech->ItemIndex;

  if(dstList->State == dsInsert)
  {
      if(RadioGroup1->ItemIndex == 2 && edtFileName->Text == "")
        throw *(new Exception(AnsiString("Не задане им`я файлу!")));
      if(edTempName->Text == "")
       throw *(new Exception(AnsiString("Не задана назва шаблону!")));
      if(edTempCode->Text == "")
       throw *(new Exception(AnsiString("Не задан код шаблону!")));

      if(tr->Active)
        tr->CommitRetaining();
      else
        tr->StartTransaction();

      TCreateInfo info;
      info.CreateType = ctNewObject;
      info.ShowAsIcon = false;
      info.IconMetaPict = 0;
      info.FileName = "";
      info.DataObject = NULL;

      switch (TypeDoc)  {
        case tdWord:
                      dstList->FieldByName("TTYPE")->AsString = "word";
                      try {
                      if(FileName == "")
                      {
                        info.ClassID = ProgIDToClassID("WordDocument");
                        try {
                            tmpContainer->CreateObjectFromInfo(info);
                        } catch (Exception& e) {
                            throw *(new Exception(AnsiString("Помилка створення WORD-шаблону!" + e.Message).c_str()));
                        }
                      }
                 } catch (Exception& e) {
                    throw *(new Exception(AnsiString("Помилка створення WORD-шаблону!" + e.Message).c_str()));
                 }
            break;
        case tdExcel:
                     info.ClassID = ProgIDToClassID("ExcelWorksheet");
                     try {
                        tmpContainer->CreateObjectFromInfo(info);
                     } catch (Exception& e) {
                        throw *(new Exception(AnsiString("Помилка створення ЕХСЕL-шаблону!" + e.Message).c_str()));
                     }
                     dstList->FieldByName("TTYPE")->AsString = "excel";
            break;
        case tdFromFile:
            try {
                tmpContainer->DestroyObject();
                tmpContainer->CreateObjectFromFile(FileName, true);
                TStream* blobStream = dstList->CreateBlobStream(dstList->FieldByName("TEMPLATE"), bmWrite);
                if (tmpContainer->State != osEmpty) {
                    blobStream->Seek(0,0);
                    tmpContainer->SaveToStream(blobStream);
                }
            } catch (Exception &e) {
                throw *(new Exception(AnsiString("Помилка створення шаблону з файлу " + OpenDialog1->FileName + e.Message).c_str()));
                dstList->Cancel();
            }
            dstList->FieldByName("TTYPE")->AsString = "";
            break;
        default : dstList->Cancel();
        }
  }
  else
  {
    if (tmpContainer->Modified)
        saveContainer();
  }
  TfrmBaseObjForm::SaveData();
}
//---------------------------------------------------------------------------

void __fastcall TSelectTypeDoc::DownloadTemplate(AnsiString& fileName, TDataSet *dataSet, Db::TField* field)
{
    char tempPath[256];
    DWORD tempPathLength = GetTempPath(256, tempPath);
    if ( tempPathLength > 0 )
    {
        fileName = AnsiString(tempPath, tempPathLength) + "Template" + Now().FormatString("yyyymmddhhmmss") + ".doc";

        std::auto_ptr<TFileStream> fileStream(new TFileStream(fileName, fmCreate | fmShareDenyWrite));

        std::auto_ptr<TStream> stream(dataSet->CreateBlobStream(field, bmRead));

        stream->Position = 0;
        fileStream->CopyFrom(stream.get(), stream->Size);
    }
}      
//---------------------------------------------------------------------------

void __fastcall TSelectTypeDoc::loadUpContainer()
{
    TDataSet* dstList = dscObj->DataSet;
    tmpContainer->DestroyObject();
    TStream* blobStream;
    if(TypeDoc == tdExcel)
        blobStream = dstList->CreateBlobStream(dstList->FieldByName("TEMPLATE2"), bmRead);
    if(TypeDoc == tdFromFile)
        blobStream = dstList->CreateBlobStream(dstList->FieldByName("TEMPLATE"), bmRead);
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
    tmpContainer->Modified = false;
}
//---------------------------------------------------------------------------

void __fastcall TSelectTypeDoc::createNewContainer()
{
    TDataSet* dstList = dscObj->DataSet;

    TCreateInfo info;
    info.CreateType = ctNewObject;
    info.ShowAsIcon = false;
    info.IconMetaPict = 0;
    info.FileName = "";
    info.DataObject = NULL;

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
            TStream* blobStream = dstList->CreateBlobStream(dstList->FieldByName("TEMPLATE"), bmWrite);
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
}
//---------------------------------------------------------------------------

void __fastcall TSelectTypeDoc::saveContainer()
{
    TDataSet* dstList = dscObj->DataSet;
    TStream* blobStream;
    if(TypeDoc == tdFromFile)
        blobStream = dstList->CreateBlobStream(dstList->FieldByName("TEMPLATE"), bmWrite);
    else
        blobStream = dstList->CreateBlobStream(dstList->FieldByName("TEMPLATE2"), bmWrite);
    try {
        if (tmpContainer->State != osEmpty) {
            blobStream->Seek(0,0);
            tmpContainer->SaveToStream(blobStream);
        }
    } __finally {
        blobStream->Free();
    }
    tmpContainer->Modified = false;
}

void __fastcall TSelectTypeDoc::ShowWordDocumentAndWait(AnsiString& path, bool& modified)
{
    Variant word = CreateOleObject("Word.Application");
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
    word = Unassigned;
}
//---------------------------------------------------------------------------

void __fastcall TSelectTypeDoc::UpdateDocument(TField* field, TDataSet *dataSet, AnsiString filePath)
{
    try
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
    }
}
//---------------------------------------------------------------------------

void __fastcall TSelectTypeDoc::FormShow(TObject *Sender)
{
    TDataSet* dstList = dscObj->DataSet;
    LoadData();
    tmpContainer->Caption = AnsiString('[')+Application->Title+']';
    if(RadioGroup1->ItemIndex == 2)
    {
        OpenDialog1->Filter = "";
    }
    else
        OpenDialog1->Filter = "PiFolio files (*.doc);(*.dot)|*.DOC;*.DOT;";
    if(dstList->State == dsInsert)
    {
        RadioGroup1->ItemIndex = 0;
        TypeDoc = tdWord;
        RadioGroup1->Enabled = true;
        edtFileName->Enabled = true;
        Label1->Enabled = true;
        btnLoadDoc->Enabled = true;
    }
    else
    {
        RadioGroup1->Enabled = false;
        if(RadioGroup1->ItemIndex == 1)
        {
            edtFileName->Enabled = false;
            Label1->Enabled = false;
            btnLoadDoc->Enabled = false;
            TypeDoc = tdExcel;
        }
        else
            if(RadioGroup1->ItemIndex == 2)
                TypeDoc = tdFromFile;
            else
                TypeDoc = tdWord;
    }
}
//---------------------------------------------------------------------------

void __fastcall TSelectTypeDoc::btEditTemplClick(TObject *Sender)
{
    TDataSet* dstList = dscObj->DataSet;
    if ( dstList->FieldByName("TTYPE")->AsString.LowerCase() == "word" ) // pifolio
    {
        if(dstList->State != dsInsert)
        {
            AnsiString templatePath;
            DownloadTemplate(templatePath, dstList, dstList->FieldByName("TEMPLATE2"));

            bool modified = false;
            ShowWordDocumentAndWait(templatePath, modified);

            if ( modified )
                UpdateDocument(dstList->FieldByName("TEMPLATE2"), dstList, templatePath);

            DeleteFile(templatePath);
        }
    }
    else if( dstList->FieldByName("TTYPE")->AsString.LowerCase() == "excel" )
    {
        loadUpContainer();
        tmpContainer->DoVerb(ovShow);
    }
        else // old style word bookmarks
        {
            loadUpContainer();
            tmpContainer->DoVerb(ovShow);
        }
    DataChange();
}
//---------------------------------------------------------------------------

void __fastcall TSelectTypeDoc::DataChange()
{
    dataChanged = true;
}
//---------------------------------------------------------------------------

void __fastcall TSelectTypeDoc::edChange(TObject *Sender)
{
    DataChange();
}
//---------------------------------------------------------------------------



