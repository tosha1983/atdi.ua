//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop
#include "uFrmDocumentsSettings.h"
#include "DlgSelectTypeDoc.h"
#include "uFrmTxBase.h"
#include "uMainDm.h"
#include "tempvalues.h"
#include <DBTables.hpp>
#include "word_2k.h"
#include "CoPWRCallbackImpl.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseObjForm"
#pragma link "xlcClasses"
#pragma link "xlEngine"
#pragma link "xlReport"
#pragma resource "*.dfm"

TCOMIWordReporter4 WordReporter;
CoPWRCallbackPtr callback;
int logHandle;

//---------------------------------------------------------------------------
__fastcall TfrmDocumentsSettings::TfrmDocumentsSettings(TComponent* Owner)
    : TfrmBaseObjForm(Owner)
{
    tmpContainer->Caption = AnsiString('[')+Application->Title+']';
}
//---------------------------------------------------------------------------

void __fastcall TfrmDocumentsSettings::LoadData()
{
    Inherited::LoadData();
    TDataSet* ds = dscObj->DataSet;
    if (ds->FieldByName("TTYPE")->AsString.IsEmpty())
    {
        if ((ds->RecordCount) && (ds->State == dsBrowse) && (tmpContainer->State != osOpen))
            LoadUpContainer();
    }
    rbInOut->ItemIndex = ds->FieldByName("TYPELETTER")->AsInteger;
    if (rbInOut->ItemIndex == 0) {
        edtNum->Text = ds->FieldByName("NUMOUT")->AsString;
        dtpDocDate->Date = ds->FieldByName("CREATEDATEOUT")->AsDateTime;
    } else {
        edtNum->Text = ds->FieldByName("NUMIN")->AsString;
        dtpDocDate->Date = ds->FieldByName("CREATEDATEIN")->AsDateTime;
    }
    TIBSQL *sql = new TIBSQL(this);
    sql->Database = dmMain->dbMain;
    sql->SQL->Text = "select c.* from TRANSMITTERS t left join SYSTEMCAST c on t.SYSTEMCAST_ID = c.ID where t.ID =" + ds->FieldByName("TRANSMITTERS_ID")->AsString;
    //ShowMessage(sql->SQL->Text);
    sql->ExecQuery();
    TRadTech rt;
    switch(sql->FieldByName("ENUMVAL")->AsInteger)
    {
        case ttFM: rt = trtFM; break;
        case ttDAB: rt = trtTV; break;
        case ttTV: rt = trtTV; break;
        case ttDVB: rt = trtDVB; break;
        default: rt = trtNone;
    }
    cbxDocType->Clear();
    if (ds->FieldByName("tempId")->AsString.IsEmpty())
    {
      
        sql->Close();
        AnsiString queryText = "select ID, NAME, DOCTYPE, RTTYPE from DOCUMENT ";
        if(rt != trtNone)
              queryText += " where RTTYPE =" + IntToStr((int)rt) + " ";
        queryText += "order by NAME";
        sql->SQL->Text = queryText;
        sql->ExecQuery();
        while (!sql->Eof) {
            cbxDocType->Items->AddObject(sql->Fields[1]->AsString, (TObject*)sql->Fields[0]->AsInteger);
            sql->Next();
        }
        cbxDocType->ItemIndex = 0;
        cbxDocType->DropDownCount = 30;
        sql->Close();  
    }
    else
    {
         cbxDocType->Items->AddObject(ds->FieldByName("TemplName")->AsString, (TObject*)ds->FieldByName("tempId")->AsInteger);
         cbxDocType->ItemIndex = 0;
         rgDoctype->ItemIndex = ds->FieldByName("DOCTYPE")->AsInteger;
    }
    cbxAccountState->ItemIndex = cbxAccountState->Items->IndexOfObject((TObject*)(ds->FieldByName("ACCOUNTCONDITION_ID")->AsInteger));
//    cbxDocType->ItemIndex = cbxDocType->Items->IndexOfObject((TObject*)(ds->FieldByName("DOCUMENT_ID")->AsInteger));
    if (ds->State == dsEdit)
        ds->Cancel();
    dataChanged = false;    
}

void __fastcall TfrmDocumentsSettings::SaveData()
{
    TDataSet* ds = dscObj->DataSet;

    if (rbInOut->ItemIndex == 0 && cbxDocType->ItemIndex == -1)
        throw *(new Exception("Для исходящего документа нужно выбрать шаблон"));

    ds->FieldByName("ACCOUNTCONDITION_ID")->AsInteger = cbxAccountState->ItemIndex > -1 ?
        (int)cbxAccountState->Items->Objects[cbxAccountState->ItemIndex] : 0;
    ds->FieldByName("DOCUMENT_ID")->AsInteger = (cbxDocType->ItemIndex > -1 && rbInOut->ItemIndex == 0) ?
        (int)cbxDocType->Items->Objects[cbxDocType->ItemIndex] : 0;

    if (rbInOut->ItemIndex == 1) {
        ds->FieldByName("NUMIN")->AsString = edtNum->Text;
        ds->FieldByName("CREATEDATEIN")->AsDateTime = dtpDocDate->Date;
    } else {
        ds->FieldByName("NUMOUT")->AsString = edtNum->Text;
        ds->FieldByName("CREATEDATEOUT")->AsDateTime = dtpDocDate->Date;
        {
            std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
            sql->Database = dmMain->dbMain;
            sql->Transaction = tr;
       /*     if (rgAddType->ItemIndex == 0)
            {
                sql->SQL->Text = "update TRANSMITTERS "
                                 "set EMC_CONCL_NUM = :EMC_CONCL_NUM, EMC_CONCL_FROM = :EMC_CONCL_FROM "
                                 "where ID = " + ds->FieldByName("TRANSMITTERS_ID")->AsString;
                sql->ParamByName("EMC_CONCL_NUM")->AsString = edtNum->Text;
                sql->ParamByName("EMC_CONCL_FROM")->AsDateTime = dtpDocDate->Date;
            } else if (rgAddType->ItemIndex == 1)
            {
                sql->SQL->Text = "update TRANSMITTERS "
                                 "set EMC_CONCL_NUM = :EMC_CONCL_NUM, EMC_CONCL_FROM = :EMC_CONCL_FROM, "
                                    "PERMUSE_OWNER_ID = :PERMUSE_OWNER_ID "
                                 "where ID = " + ds->FieldByName("TRANSMITTERS_ID")->AsString;
                sql->ParamByName("NUMPERMUSE")->AsString = edtNum->Text;
                sql->ParamByName("DATEPERMUSEFROM")->AsDateTime = dtpDocDate->Date;
            }
            if (!sql->SQL->Text.IsEmpty())
                sql->ExecQuery();

            if (rgAddType->ItemIndex == 1)
            {
                sql->Close();
                sql->SQL->Text = "update TRANSMITTERS "
                                 "set PERMUSE_OWNER_ID = OPERATOR_ID "
                                 "where ID = " + ds->FieldByName("TRANSMITTERS_ID")->AsString + " and "
                                 "(PERMUSE_OWNER_ID is null or PERMUSE_OWNER_ID = 0)";
                sql->ExecQuery();
            }     */
        }
    }

    if ((rbInOut->ItemIndex == 1)&& (lblDocName->Caption != "")) {
        try {
            tmpContainer->DestroyObject();
            tmpContainer->CreateObjectFromFile(OpenDialog1->FileName, true);
            TStream* blobStream = ds->CreateBlobStream(ds->FieldByName("COPYLETTER"), bmWrite);
            if (tmpContainer->State != osEmpty)
            {
                blobStream->Seek(0,0);
                tmpContainer->SaveToStream(blobStream);
            }
        } __finally {
            //???ds->FieldByName("DOCUMENT_ID") = NULL;
        }
    } 

    //from BEFORE POST
    if ( ds->FieldByName("ACCOUNTCONDITION_ID")->IsNull )
        throw *(new Exception("Треба заповнити поле стану"));

    if (ds->FieldByName("ID")->IsNull)
        ds->FieldByName("ID")->AsInteger = dmMain->getNewId();

    ds->FieldByName("TYPELETTER")->AsInteger = rbInOut->ItemIndex;

    if ( (ds->FieldByName("DOCUMENT_ID") != NULL) && (ds->FieldByName("COPYLETTER")->IsNull) )
    {
        TempCursor tc(crHourGlass);
        try
        {
            dmMain->ibqTxOuery->Close();
            dmMain->ibqTxOuery->ParamByName("ID")->AsInteger = ds->FieldByName("TRANSMITTERS_ID")->AsInteger;
            dmMain->ibqTxOuery->Open();

            std::auto_ptr<TIBQuery> query(new TIBQuery(NULL));
              query->Database = dmMain->dbMain;
              query->SQL->Text = "select TEMPLATE, TEMPLATE2, tType from DOCUMENT where id = " + ds->FieldByName("DOCUMENT_ID")->AsString;
            query->Open();

            String ttype = query->FieldByName("tType")->AsString.LowerCase();
            if ( ttype == "word" ) // pifolio 
            {
                AnsiString documentPath;
                AnsiString templatePath;
                DownloadTemplate(templatePath, ds->FieldByName("DOCUMENT_ID")->AsInteger);
                CreatePwrDocument(documentPath, templatePath, dmMain->ibqTxOuery);
                UploadDocument(ds->FieldByName("COPYLETTER"), ds, documentPath);

                DeleteFile(templatePath);

                bool modified = false;
                ShowWordDocumentAndWait(documentPath, modified);

                if ( modified )
                    UploadDocument(ds->FieldByName("COPYLETTER"), ds, documentPath);
                ds->FieldByName("TTYPE")->AsString = "word";
                DeleteFile(documentPath);
            }
            else if ( ttype == "excel" )
            {
                TStream* blobStream = query->CreateBlobStream(query->Fields->Fields[1], bmRead);

                dynamic_cast<TBlobField*>(ds->FieldByName("COPYLETTER"))->LoadFromStream(blobStream);

                LoadUpContainer();

                if (tmpContainer->OleClassName.SubString(0,5) == "Excel")
                {
                    ExcelDocumentFill(tmpContainer, dmMain->ibqTxOuery);

                    tmpContainer->Modified = true;
                    SaveContainer();
                }
            }
            else // old-style word bookmarks
            {
                TStream* blobStream = query->CreateBlobStream(query->Fields->Fields[0], bmRead);

                dynamic_cast<TBlobField*>(ds->FieldByName("COPYLETTER"))->LoadFromStream(blobStream);

                LoadUpContainer();

                if (tmpContainer->OleClassName.SubString(0,4) == "Word")
                    WordDocumentFill(tmpContainer, dmMain->ibqTxOuery);
                else if (tmpContainer->OleClassName.SubString(0,5) == "Excel")
                    ExcelDocumentFill(tmpContainer, dmMain->ibqTxOuery);

                tmpContainer->Modified = true;
                SaveContainer();
            }
        }
        __finally
        {
            dmMain->ibqTxOuery->Close();
        }
    }

    Inherited::SaveData();
}
//---------------------------------------------------------------------------

void __fastcall TfrmDocumentsSettings::rgDoctypeClick(TObject *Sender)
{
    cbxDocType->Clear();
    TDataSet* ds = dscObj->DataSet;
    TRadTech rt;
    TIBSQL *sql = new TIBSQL(this);
    sql->Database = dmMain->dbMain;
    sql->SQL->Text = "select c.* from TRANSMITTERS t left join SYSTEMCAST c on t.SYSTEMCAST_ID = c.ID where t.ID =" + ds->FieldByName("TRANSMITTERS_ID")->AsString;
    sql->ExecQuery();
    switch(sql->FieldByName("ENUMVAL")->AsInteger)
    {
        case ttFM: rt = trtFM; break;
        case ttDAB: rt = trtTV; break;
        case ttTV: rt = trtTV; break;
        case ttDVB: rt = trtDVB; break;
        default: rt = trtNone;
    }
    sql->Close();
    AnsiString queryText = "select ID, NAME, DOCTYPE, RTTYPE from DOCUMENT ";
    if (rgDoctype->ItemIndex != -1)
    {
        if(rt != trtNone)
            queryText += " where RTTYPE =" + IntToStr((int)rt) + " and DOCTYPE =" + IntToStr(rgDoctype->ItemIndex) + " ";
        else
            queryText += " where DOCTYPE =" + IntToStr(rgDoctype->ItemIndex) + " ";
        queryText += " order by NAME";
        sql->SQL->Text = queryText;
        sql->ExecQuery();
        while (!sql->Eof) {
            cbxDocType->Items->AddObject(sql->Fields[1]->AsString, (TObject*)sql->Fields[0]->AsInteger);
            sql->Next();
        }
        cbxDocType->ItemIndex = 0;
        cbxDocType->DropDownCount = 30;
        sql->Close();
    }
    else
    {
        if(rt != trtNone)
            queryText += "where RTTYPE =" + IntToStr((int)rt) + " ";
        queryText += "order by NAME";
        sql->SQL->Text = queryText;
        sql->ExecQuery();
        while (!sql->Eof) {
            cbxDocType->Items->AddObject(sql->Fields[1]->AsString, (TObject*)sql->Fields[0]->AsInteger);
            sql->Next();
        }
        cbxDocType->ItemIndex = 0;
        cbxDocType->DropDownCount = 30;
        sql->Close();
    }  
}
//---------------------------------------------------------------------------

void __fastcall TfrmDocumentsSettings::btnInDocImageClick(TObject *Sender)
{
    TDataSet *ds = dscObj->DataSet;
    if (!ds->FieldByName("COPYLETTER")->IsNull &&
    MessageBox(NULL, "Шаблон документа уже задан.\nЗадать новый?", "Вопрос", MB_ICONQUESTION|MB_YESNO) == IDNO)
        return;
    OpenDialog1->Execute();
}
//---------------------------------------------------------------------------


void __fastcall TfrmDocumentsSettings::OpenDialog1SelectionChange(
      TObject *Sender)
{
    lblDocName->Caption = OpenDialog1->FileName;
}
//---------------------------------------------------------------------------

void __fastcall TfrmDocumentsSettings::FormCreate(TObject *Sender)
{
    TIBSQL *sql = new TIBSQL(this);
    sql->Database = dmMain->dbMain;
    sql->SQL->Text = "select ID, NAME from ACCOUNTCONDITION order by NAME";
    sql->ExecQuery();
    while (!sql->Eof) {
        cbxAccountState->Items->AddObject(sql->Fields[1]->AsString, (TObject*)sql->Fields[0]->AsInteger);
        sql->Next();
    }
    cbxAccountState->DropDownCount = 30;
    cbxAccountState->ItemIndex = 0;
    sql->Close();

    sql->SQL->Text = "select ID, NAME from DOCUMENT order by NAME";
    sql->ExecQuery();
    while (!sql->Eof) {
        cbxDocType->Items->AddObject(sql->Fields[1]->AsString, (TObject*)sql->Fields[0]->AsInteger);
        sql->Next();
    }
    cbxDocType->ItemIndex = 0;
    cbxDocType->DropDownCount = 30;
    sql->Close();
    btnInDocImage->Enabled = false;
    lblDocName->Caption = cbxDocType->Items->Strings[cbxDocType->ItemIndex];
}
//---------------------------------------------------------------------------

void __fastcall TfrmDocumentsSettings::DownloadTemplate(AnsiString& fileName, int id)
{
    std::auto_ptr<TIBQuery> query(new TIBQuery(NULL));
      query->Database = dmMain->dbMain;
      query->SQL->Text = "SELECT TEMPLATE2 FROM Document WHERE ( id = :id )";
      query->ParamByName("id")->Value = id;
    query->Open();
    query->First();

    if ( !query->Eof )
    {
        char tempPath[256];
        DWORD tempPathLength = GetTempPath(256, tempPath);
        if ( tempPathLength > 0 )
        {
            fileName = AnsiString(tempPath, tempPathLength) + "Template" + Now().FormatString("yyyymmddhhmmss") + ".dot";

            std::auto_ptr<TFileStream> fileStream(new TFileStream(fileName, fmCreate | fmShareDenyWrite));

            std::auto_ptr<TStream> stream(query->CreateBlobStream(query->Fields->Fields[0], bmRead));

            stream->Position = 0;
            fileStream->CopyFrom(stream.get(), stream->Size);
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmDocumentsSettings::CreatePwrDocument(AnsiString& documentName, AnsiString templateName, TIBQuery* query)
{
    AnsiString currentDir = ExtractFilePath(Application->ExeName);
                                      //        +  "results\\Document"
    documentName = currentDir + Now().FormatString("yyyymmddhhmmss") + ".doc";

    std::auto_ptr<TSaveDialog> saveDialog(new TSaveDialog(NULL));
      saveDialog->DefaultExt = "doc";
      saveDialog->Filter = "Microsoft Word files (*.doc)|*.doc|All files (*.*)|*.*";
      saveDialog->InitialDir = currentDir;
      saveDialog->Options = TOpenOptions() << ofOverwritePrompt;

    //if ( saveDialog->Execute() )
    {

        if ( FileExists(documentName) )
            DeleteFile(documentName);

        WordReporter.CreateInstance(CLSID_WordReporter4);

        if ( !WordReporter.IsBound() )
        {
            Application->MessageBox("Компонент PiFolio WordReporter не загружен.", Application->Title.c_str(), MB_ICONHAND);
            return;
        }

        if ( WordReporter.IsBound() )
        {
            callback.CreateInstance(CLSID_CoPWRCallback);
            callback->SetStatus((wchar_t*)query);
            OleCheck( WordReporter.set_CallbackIntf((PWRCallback*)callback) );

            WordReporter.set_TempFolder(WideString(currentDir).c_bstr());

   	    	WordReporter.set_LicenseKey(WideString("DEMO").c_bstr());
   	 		WordReporter.set_OnCompletedShowWord(false);
   	 		WordReporter.set_CreateNewWordInstance(true);
	    	WordReporter.MergeKeepSourcefiles = true;
	 		WordReporter.MergeType = PWR_VALUE_Merge_PageBreak;
   	 		WordReporter.FileAction = PWR_VALUE_Overwrite;


            OleCheck( WordReporter.set_ClientHandle((long)Handle) );
            OleCheck( WordReporter.set_FileFormat(PWR_VALUE_Fileformat_DOC) );

            OleCheck( WordReporter.set_WordTemplate(WideString(templateName)) );
            AnsiString tempMergeThrash = currentDir + "MergeThrash.doc";



            OleCheck( WordReporter.set_MergeFinalFileName(WideString(tempMergeThrash)) );
            OleCheck( WordReporter.set_FinalFileName(WideString(documentName)) );

            if ( WordReporter.InitReporter() )
            {
                Application->MessageBox("Ошибка инициализации (вызов InitReporter())", Application->Title.c_str(), MB_ICONHAND);
                return;
            }

            bool makeCorrupted = WordReporter.MakeWordDocument();
            //frmMain->StatusBar1->SimpleText = "Вызов KillReporter()...";
            //frmMain->StatusBar1->Repaint();
            #ifdef _DEBUG
                //Application->MessageBox("вызов KillReporter()...", Application->Title.c_str(), MB_ICONINFORMATION);
            #endif

            WordReporter.KillReporter();

            if(FileExists(tempMergeThrash))
                DeleteFile(tempMergeThrash);
            if (makeCorrupted)
                MessageDlg("Ошибка при генерации документа (вызов MakeWordDocument())", mtError, TMsgDlgButtons() << mbOK, 0);
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmDocumentsSettings::UploadDocument(TField* field, TDataSet *dataSet, AnsiString fileName)
{
    try
    {
        std::auto_ptr<TFileStream> file(new TFileStream(fileName, fmOpenRead | fmShareDenyWrite));

        TStream* stream = dataSet->CreateBlobStream(field, bmWrite);
        stream->Size = 0;
        stream->CopyFrom(file.get(), file->Size);
    }
    catch(...)
    {
    }
}
//---------------------------------------------------------------------------


void __fastcall TfrmDocumentsSettings::btOpenDocClick(TObject *Sender)
{
    TDataSet *ds = dscObj->DataSet;
    if (!ds->FieldByName("COPYLETTER")->IsNull)
    {
        AnsiString documentType = GetDocumentType(ds->FieldByName("DOCUMENT_ID")->AsInteger).LowerCase();
        if( documentType == "word" ) // pifolio
        {
            AnsiString documentPath;
            DownloadDocument(documentPath, ds->FieldByName("id")->AsInteger);

            bool modified = false;
            ShowWordDocumentAndWait(documentPath, modified);

            if ( !modified )
                UpdateDocument(ds->FieldByName("COPYLETTER"), ds, documentPath);

            DeleteFile(documentPath);
        }
        else if( documentType == "excel" )
        {
            LoadUpContainer();
            tmpContainer->DoVerb(ovShow);
        }
        else // old style bookmark word
            tmpContainer->DoVerb(ovShow);
    } else
        throw *(new Exception("Поле документа пусто"));
}
//---------------------------------------------------------------------------

void __fastcall TfrmDocumentsSettings::ShowWordDocumentAndWait(AnsiString& path, bool& modified)
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

void __fastcall TfrmDocumentsSettings::DownloadDocument(AnsiString& fileName, int id)
{                                                                                                 
    std::auto_ptr<TIBQuery> query(new TIBQuery(NULL));
      query->Database = dmMain->dbMain;
      query->SQL->Text = "SELECT copyLetter FROM Letters WHERE ( id = :id )";
      query->ParamByName("id")->Value = id;
    query->Open();
    query->First();

    if ( !query->Eof )
    {
        char tempPath[256];
        DWORD tempPathLength = GetTempPath(256, tempPath);
        if ( tempPathLength > 0 )
        {
            fileName = AnsiString(tempPath, tempPathLength) + "Document" + Now().FormatString("yyyymmddhhmmss") + ".doc";

            std::auto_ptr<TFileStream> fileStream(new TFileStream(fileName, fmCreate | fmShareDenyWrite));

            std::auto_ptr<TStream> stream(query->CreateBlobStream(query->Fields->Fields[0], bmRead));

            stream->Position = 0;
            fileStream->CopyFrom(stream.get(), stream->Size);
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmDocumentsSettings::ExcelDocumentFill(TOleContainer* oleContainer, TDataSet *dataSet)
{
    try
    {
        char tempPath[256];
        Variant excel;
        DWORD tempPathLength = GetTempPath(256, tempPath);
        if ( tempPathLength > 0 )
        {
            AnsiString fileExcel = AnsiString(tempPath, tempPathLength) + "BCDocTemp.xls";
            DeleteFile(fileExcel);
            AnsiString fileExcel2 = AnsiString(tempPath, tempPathLength) + "BCDocTemp2.xls";
            DeleteFile(fileExcel2);

            oleContainer->DoVerb(ovShow);
            excel = oleContainer->OleObject;

            excel.OlePropertyGet("Application").OlePropertyGet("ActiveWorkbook").OleFunction("SaveAs", fileExcel.c_str());
            excel.OlePropertyGet("Application").OlePropertyGet("ActiveWorkbook").OleFunction("Close");
            oleContainer->DestroyObject();
            xlReport1->DataSources->Clear();
            TxlDataSource *xlDataSource = xlReport1->DataSources->Add();
            xlDataSource->DataSet = dataSet;
            xlReport1->XLSTemplate = fileExcel;
            xlReport1->ReportTo("", fileExcel2);
            DeleteFile(fileExcel);
            oleContainer->CreateObjectFromFile(fileExcel2, true);
            DeleteFile(fileExcel2);
        }
    }
    catch (Exception &e)
    {
        throw Exception(AnsiString("Помилка створення ЕХСЕL-документу:\n " + e.Message));
    }
}
//---------------------------------------------------------------------------

AnsiString __fastcall TfrmDocumentsSettings::GetDocumentType(int documentId)
{
    std::auto_ptr<TIBSQL> sql(new TIBSQL(NULL));
      sql->Database = dmMain->dbMain;
      sql->GoToFirstRecordOnExecute = true;
      sql->SQL->Text = "SELECT tType FROM Document WHERE ( id = :id )";
      sql->ParamByName("id")->Value = documentId;
    sql->ExecQuery();

    return sql->Fields[0]->AsString;
}
//---------------------------------------------------------------------------

void __fastcall TfrmDocumentsSettings::UpdateDocument(TField* field, TDataSet *dataSet, AnsiString fileName)
{
    try
    {
        dataSet->Edit();

        std::auto_ptr<TFileStream> file(new TFileStream(fileName, fmOpenRead | fmShareDenyWrite));

        TStream* stream = dataSet->CreateBlobStream(field, bmWrite);
        stream->Size = 0;
        stream->CopyFrom(file.get(), file->Size);
    }
    catch(...)
    {
    }
}
//---------------------------------------------------------------------------

void TfrmDocumentsSettings::WordDocumentFill(TOleContainer* oleContainer, TDataSet *DataSet)
{
    Variant Word;
    oleContainer->DoVerb(ovShow);
    Word = oleContainer->OleObject;
    //Word.OlePropertyGet("Application").OlePropertyGet("Visible") = false;
    //Variant selection;
    //selection = (IDispatch*)Word.OlePropertyGet("Application").OlePropertyGet("Selection");

    //code below is just because this all doesnt work without it
    TAutoDriver<IDispatch> wrdDrv;
    wrdDrv.Bind((IDispatch*)Word, true);
    long metId = 0;
    OleCheck(wrdDrv.GetIDsOfNames(L"Application", metId));
    /*
    TAutoArgs<0> args;
    DispatchFlag flag;
    wrdDrv.Invoke(dfPropGet, L"Application", &args, true);
    Variant app = *(args.GetRetVal());
    */
    Variant app = Word.OlePropertyGet(L"Application");
    Variant sel = app.OlePropertyGet(L"Selection");
    TAutoDriver<IDispatch> selDrv;
    selDrv.Bind((IDispatch*)sel, true);
    long gotoId = 0;
    long typeTextId = 0;
    OleCheck(selDrv.GetIDsOfNames(L"Goto", gotoId));
    OleCheck(selDrv.GetIDsOfNames(L"TypeText", typeTextId));
    TAutoArgs<4> args4;
    args4[1] = wdGoToBookmark;
    TAutoArgs<1> args1;

    AnsiString errLog;
    double max_epr_hor, max_epr_vert;
    if (dmMain->ibqTxOuerySC_CODE->AsString.Trim() == "АТБ") {
        max_epr_hor =  dmMain->ibqTxOueryEPR_VIDEO_HOR->AsFloat;
        max_epr_vert = dmMain->ibqTxOueryEPR_VIDEO_VERT->AsFloat;
    } else {
        max_epr_hor  = dmMain->ibqTxOueryEPR_SOUND_HOR_PRIMARY->AsFloat;
        max_epr_vert = dmMain->ibqTxOueryEPR_SOUND_VERT_PRIMARY->AsFloat;
    }
    if (DataSet->Active == true) {
        for (int i = 0; i < dmMain->ibqTxOuery->FieldCount ; i++) {
            TField *fld = dmMain->ibqTxOuery->Fields->Fields[i];
            //Memo1->Lines->Add(fld->FieldName);
            AnsiString bmName = fld->FieldName;
            WideString bmText = fld->AsString;
            try {
                //Word.OlePropertyGet("Application").OlePropertyGet("Selection").OleFunction("Goto", wdGoToBookmark , NULL, NULL, word_bookmark.c_str());
                args4[4] = WideString(bmName);
                OleCheck(selDrv.OleFunction(gotoId, args4));
                if (bmName == "POLARIZATION") {
                    if (fld->AsString == "H")
                        bmText = "Г";
                    else if (fld->AsString == "V")
                        bmText = "В";
                    else
                        bmText = "З";
                }
                else if (bmName == "DIRECTION") {
                    if (fld->AsString == "D")
                        bmText = "С";
                    else
                        bmText = "НС";
                }
                else if (bmName == "DIRECTION2")
                    bmText = bmText.SubString(1,1);
                else if (bmName == "MONOSTEREO_PRIMARY") {
                    if (fld->AsInteger)
                        bmText = "стерео";
                    else
                        bmText = "моно";
                }
                else if (bmName == "SUMMATORPOWERS") {
                    if (fld->AsInteger)
                        bmText = "Так";
                    else
                        bmText = "Ні";
                }
                else if (bmName == "TYPEOFFSET2") {
                    bmText = bmText.SubString(1,1);
                }
                else if ( bmName.UpperCase() == "POWER_VIDEO" )
                    //выводится в кВт, а надо в Вт
                    bmText = FormatFloat("0.###", fld->AsFloat * 1000.0);
                else if ( bmName.UpperCase() == "POWER_SOUND_PRIMARY" )
                    //выводится в кВт, а надо в Вт
                    bmText = FormatFloat("0.###", fld->AsFloat * 1000.0);
                else if ( bmName.UpperCase() == "VIDEO_OFFSET_HERZ" )
                    //выводится в Гц, а надо в кГц
                    bmText = FormatFloat("0.###", fld->AsFloat / 1000.0);
                else if ( bmName.UpperCase() == "EPR_VIDEO_MAX" )
                    //выводится в дБкВт, а надо в дБВт
                    bmText = FormatFloat("0.###", fld->AsFloat + 3);
                else if (fld->DataType == ftFloat)
                    if (bmName.SubString(1,9)=="EFFPOWHOR") {
                        if (max_epr_hor==0)
                            bmText = "0";
                        else
                            bmText = FormatFloat("0.###",max_epr_hor-fld->AsFloat);
                    }
                    else if (bmName.SubString(1,9)=="EFFPOWVER") {
                        if (max_epr_vert==0)
                            bmText = "0";
                        else
                            bmText = FormatFloat("0.###",max_epr_vert-fld->AsFloat);
                    }
                    else if (fld->AsFloat < -300) {
                        if ((bmName == "EPR_SOUND_HOR_PRIMARY_DBW")||(bmName == "EPR_SOUND_VERT_PRIMARY_DBW"))
                            bmText =  "0";
                        else
                            bmText =  "";
                    }
                    else if (bmName.SubString(1,3)=="EPR")
                        bmText = FormatFloat("0.###",fld->AsFloat);
                    else
                        bmText = FormatFloat("0.###",fld->AsFloat);

                //Word.OlePropertyGet("Application").OlePropertyGet("Selection").OleProcedure("TypeText", bmText.c_str());
                //sel.OleProcedure<WideString>("TypeText", bmText);
                args1[1] = bmText;
                OleCheck(selDrv.OleProcedure(typeTextId, args1));
            } catch (Exception & e) {
                errLog += (bmName+" ("+ (fld->DataType == ftBlob ?
                                        AnsiString("<BLOB>") : fld->AsString) +
                           "): "+ AnsiString(e.ClassName())+": "+e.Message+'\n');
            }
        }

        AnsiString Equipment = "", Manufacture = "";
        TIBSQL *sql = new TIBSQL(this);
        sql->Database = dmMain->dbMain;
        sql->SQL->Text = "select eq.NAME, eq.MANUFACTURE from TRANSMITTEREQUIPMENT te "
                         " left outer join EQUIPMENT eq on (eq.ID = te.EQUIPMENT_ID) "
                         " where te.TRANSMITTERS_ID = " + dscObj->DataSet->FieldByName("TRANSMITTERS_ID")->AsString;
        sql->ExecQuery();
        while (!sql->Eof) {
            Equipment += (sql->Fields[0]->AsString + "; ");
            Manufacture += (sql->Fields[1]->AsString + "; ");
            sql->Next();
        }
        delete sql;
        if (Equipment != "") {
          try {
            Word.OlePropertyGet("Application").OlePropertyGet("Selection").OleFunction("Goto", wdGoToBookmark , NULL, NULL, "EQUIPMENT");
            Word.OlePropertyGet("Application").OlePropertyGet("Selection").OleProcedure("TypeText", Equipment.c_str());
            Word.OlePropertyGet("Application").OlePropertyGet("Selection").OleFunction("Goto", wdGoToBookmark , NULL, NULL, "EQUIPMENT_MANUFACTURE");
            Word.OlePropertyGet("Application").OlePropertyGet("Selection").OleProcedure("TypeText", Manufacture.c_str());
          } catch (Exception & e) {
            errLog += ("EQUIPMENT,MANUFACTURE: "+AnsiString(e.ClassName())+": "+e.Message+'\n');
          }
          try {
            Word.OlePropertyGet("Application").OlePropertyGet("Selection").OleFunction("Goto", wdGoToBookmark , NULL, NULL, "NUM_DOC");
            Word.OlePropertyGet("Application").OlePropertyGet("Selection").OleProcedure("TypeText", edtNum->Text.c_str());
            Word.OlePropertyGet("Application").OlePropertyGet("Selection").OleFunction("Goto", wdGoToBookmark , NULL, NULL, "DATA_DOC");
            Word.OlePropertyGet("Application").OlePropertyGet("Selection").OleProcedure("TypeText", DateToStr(dtpDocDate->Date).c_str());
          } catch (Exception & e) {
            errLog += ("NUM_DOC,DATA_DOC: "+AnsiString(e.ClassName())+": "+e.Message+'\n');
          }
        }
    }
    //if (errLog.Length() > 0)
    //    MessageBox(NULL, errLog.c_str(), "Ошибки при формировании документа", MB_ICONEXCLAMATION);
}

void __fastcall TfrmDocumentsSettings::LoadUpContainer()
{
    TDataSet *ds = dscObj->DataSet;
    tmpContainer->DestroyObject();
    TStream* blobStream = ds->CreateBlobStream(ds->FieldByName("COPYLETTER"), bmRead);
    try {
        if (blobStream->Size != 0) {
            blobStream->Seek(0,0);
            try {
                tmpContainer->LoadFromStream(blobStream);
            } catch (Exception& E) {
                if (Application->MessageBox((AnsiString("Не могу прочесть документ: ")+E.Message+"\nСоздать новый?").c_str(),Application->Title.c_str(), MB_ICONQUESTION | MB_YESNO) == IDYES)
                    CreateNewContainer();
            }
        } else {
            if (Application->MessageBox("Поле документа пустое. Создать новый документ?", Application->Title.c_str(), MB_ICONQUESTION | MB_YESNO) == IDYES)
                CreateNewContainer();
        }
    } __finally {
        blobStream->Free();
    }
    tmpContainer->Modified = false;
}
//---------------------------------------------------------------------------
void __fastcall TfrmDocumentsSettings::CreateNewContainer()
{
    TCreateInfo createInfo;
    createInfo.CreateType = ctNewObject;
    createInfo.ShowAsIcon = false;
    createInfo.IconMetaPict = 0;
    createInfo.FileName = "";
    createInfo.ClassID = ProgIDToClassID("WordDocument");
    createInfo.DataObject = NULL;
    try {
        tmpContainer->CreateObjectFromInfo(createInfo);
        //  look whether container is modified
    } catch (Exception& E) {
        MessageBox(0,"Ошибка при создании пустого документа ", Application->Title.c_str(), MB_OK | MB_ICONINFORMATION);
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmDocumentsSettings::SaveContainer()
{
    TDataSet *ds = dscObj->DataSet;
    TStream* blobStream = ds->CreateBlobStream(ds->FieldByName("COPYLETTER"), bmWrite);
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
//---------------------------------------------------------------------------


void __fastcall TfrmDocumentsSettings::FormKeyDown(TObject *Sender,
      WORD &Key, TShiftState Shift)
{
    if (Key == 13 && Shift == TShiftState()) {
        if (!dscObj->DataSet->FieldByName("COPYLETTER")->IsNull)
            tmpContainer->DoVerb(ovShow);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmDocumentsSettings::edtNumChange(TObject *Sender)
{
    if (dscObj->DataSet)
    {
        dscObj->DataSet->Edit();
        if (rbInOut->ItemIndex == 1) {
            dscObj->DataSet->FieldByName("NUMIN")->AsString = edtNum->Text;
        } else {
            dscObj->DataSet->FieldByName("NUMOUT")->AsString = edtNum->Text;
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmDocumentsSettings::dtpDocDateChange(TObject *Sender)
{
    if (dscObj->DataSet)
    {
        dscObj->DataSet->Edit();
        if (rbInOut->ItemIndex == 1) {
            dscObj->DataSet->FieldByName("CREATEDATEIN")->AsDateTime = dtpDocDate->Date;
        } else {
            dscObj->DataSet->FieldByName("CREATEDATEOUT")->AsDateTime = dtpDocDate->Date;
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmDocumentsSettings::cbxAccountStateChange(
      TObject *Sender)
{
    if (dscObj->DataSet)
    {
        dscObj->DataSet->Edit();
        dscObj->DataSet->FieldByName("ACCOUNTCONDITION_ID")->AsInteger =
            (int)cbxAccountState->Items->Objects[cbxAccountState->ItemIndex];
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmDocumentsSettings::cbxDocTypeChange(TObject *Sender)
{
    if (dscObj->DataSet)
    {
        dscObj->DataSet->Edit();
        dscObj->DataSet->FieldByName("DOCUMENT_ID")->AsInteger =
            (int)cbxDocType->Items->Objects[cbxDocType->ItemIndex];
    }
}
//---------------------------------------------------------------------------
  
void __fastcall TfrmDocumentsSettings::rbInOutClick(TObject *Sender)
{
    if (rbInOut->ItemIndex == 1)
    {
        cbxDocType->Enabled = false;
        btnInDocImage->Enabled = true;
        lblDocName->Caption = "";
    } else {
        cbxDocType->Enabled = true;
        btnInDocImage->Enabled = false;
        lblDocName->Caption = cbxDocType->Items->Strings[cbxDocType->ItemIndex];
    }
    if (dscObj->DataSet)
    {
        dscObj->DataSet->Edit();
        dscObj->DataSet->FieldByName("TYPELETTER")->AsInteger = rbInOut->ItemIndex;
    }
}
//---------------------------------------------------------------------------






