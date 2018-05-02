//---------------------------------------------------------------------------
#include <IniFiles.hpp>
#pragma hdrstop

#include "SelectColumnsForm.h"
#include "uExportDlg.h"
#include "uLayoutMngr.h"
#include "uMainDm.h"
#include "uSearch.h"

#include "TxBroker.h"
#include "tempvalues.h"
#include "uAnalyzer.h"
#include "FormProvider.h"

//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uWhere"
#pragma link "CustomMap"
#pragma link "NumericEdit"
#pragma resource "*.dfm"

Cfields::Cfields()
{
    alias = "";
    displayLabel = "";
    fieldName = "";
    parsed = false;
    selected = false;
    visible = false;
    width = 0;
}

Cfields::Cfields(const Cfields& value)
{
    alias = value.alias;
    displayLabel = value.displayLabel;
    fieldName = value.fieldName;
    parsed = value.parsed;
    selected = value.selected;
    visible = value.visible;
    width = value.width;
}

Cfields::~Cfields()
{
}

__fastcall TfrmSearch::~TfrmSearch()
{
    std::map<int, LPSAFEARRAY>::iterator ci;
    for (ci = coverage.begin(); ci != coverage.begin(); ci++) {
        SafeArrayDestroy(ci->second);
        ci->second = NULL;
    }
}

enum SearchMapTool {miSelectRegionTool = miLastBmTool};

AnsiString fieldNameFromFullName(AnsiString fullName)
{
    int i = fullName.Pos('.');
    if (i > 0 )
        return fullName.SubString(i + 1, fullName.Length() - i);
    else
        return fullName;
}

bool addSortedFields = false;

void __fastcall TfrmSearch::btnToExcelClick(TObject *)
{
    stopExcel = false;

    //  форма с прогресс-баром и кнопкой отмены
    std::auto_ptr<TForm> frmProgress(new TForm(Application));
      frmProgress->BorderStyle = bsDialog;
      frmProgress->Caption = "Экспорт результатов в Excel";
      frmProgress->FormStyle = fsStayOnTop;
      frmProgress->Height = 90;
      frmProgress->Position = poMainFormCenter;
      frmProgress->OnClose = ExcelFormClose;
      frmProgress->Width = 250;

    TProgressBar *pb = new TProgressBar(frmProgress.get());
      pb->Left = 4;
      pb->Parent = frmProgress.get();
      pb->Step = 1;
      pb->Top = 20;
      pb->Width = frmProgress->ClientWidth - pb->Left * 2;

    TButton *btn = new TButton(frmProgress.get());
      btn->Cancel = true;
      btn->Caption = "Остановить";
      btn->Height = 20;
      btn->Left = (frmProgress->ClientWidth - btn->Width) / 2;
      btn->OnClick = btnCloseClick;
      btn->Parent = frmProgress.get();
      btn->Top = 40;

    TLabel *lbl = new TLabel(frmProgress.get());
      lbl->Caption = "Выборка всех записей с сервера...";
      lbl->Left = 8;
      lbl->Parent = frmProgress.get();
      lbl->Top = 4;

    frmProgress->Visible = true;
    Application->ProcessMessages();
    if (stopExcel)
        return;

    TIBQuery *qry = (TIBQuery *)dbgRsults->DataSource->DataSet;

    if (qry->Active)
        qry->FetchAll();

    pb->Max = qry->RecordCount;

    lbl->Caption = "Создание экземпляра Excel...";
    Application->ProcessMessages();
    if (stopExcel)
        return;

    qry->First();
    qry->DisableControls();
    Variant varXlApp = CreateOleObject("Excel.Application");

    lbl->Caption = "Настройка Excel...";
    frmProgress->Update();

    varXlApp.OlePropertyGet("Application").OlePropertySet<int>("SheetsInNewWorkbook", 1);
    varXlApp.OlePropertySet<WideString>("UserName", "Ярослав Коваленко");
    varXlApp.OlePropertyGet("Workbooks").OleProcedure("Add");

    lbl->Caption = "Заголовки...";
    frmProgress->Update();

    varXlApp.OlePropertyGet<WideString>("Rows", "1:1").OleProcedure("Select");
    varXlApp.OlePropertyGet("Selection").OlePropertyGet("Font").OlePropertySet<VARIANT_BOOL>("Bold", true);

    int colIndex = 0;
    for (int i = 0; i < qry->Fields->Count; i++) {
        if (qry->Fields->Fields[i]->Visible) {
            AnsiString colRef = nToAz(colIndex);

            varXlApp.OlePropertyGet<WideString>("Range", colRef + '1').OleProcedure("Select");
            varXlApp.OlePropertyGet("ActiveCell").OlePropertySet<WideString>("FormulaR1C1", qry->Fields->Fields[i]->DisplayLabel);

            colIndex++;
        }

    }

    int rowIndex = 2;
    while (!qry->Eof) {

        lbl->Caption = AnsiString("Запись ") + (rowIndex - 1) + " из " + qry->RecordCount;
        pb->StepIt();
        Application->ProcessMessages();

        if (stopExcel)
            break;

        int colIndex = 0;
        for (int i = 0; i < qry->Fields->Count; i++) {
            if (qry->Fields->Fields[i]->Visible) {
                //if (!qry->Fields->Fields[i]->IsNull) {
                    AnsiString colRef = nToAz(colIndex);
                    varXlApp.OlePropertyGet<WideString>("Range", colRef + rowIndex).OleProcedure("Select");
                    //приведение Широты и Долготы к читабельному виду
                    if ( qry->Fields->Fields[i]->FieldName.Pos("LATITUDE") != 0 )
                        varXlApp.OlePropertyGet("ActiveCell").OlePropertySet<WideString>("FormulaR1C1", dmMain->coordToStr(qry->Fields->Fields[i]->AsFloat, 'Y'));
                    else if ( qry->Fields->Fields[i]->FieldName.Pos("LONGITUDE") != 0 )
                        varXlApp.OlePropertyGet("ActiveCell").OlePropertySet<WideString>("FormulaR1C1", dmMain->coordToStr(qry->Fields->Fields[i]->AsFloat, 'X'));
                    else if ( qry->Fields->Fields[i]->FieldName.Pos("NAMECHANNEL") != 0 )
                    {
                        int key = qry->Fields->FieldByName("SC_ENUMVAL")->AsInteger;
                        TField *f = NULL;
                        switch( key )
                        {
                            case 1:
                            case 4: f = qry->Fields->FindField("NAMECHANNEL");
                                break;
                            case -1:
                            case 2:
                            case 3: f = qry->Fields->FindField("SOUND_CARRIER_PRIMARY");
                                break;
                            case 5: f = qry->Fields->FindField("BD_NAME");
                                break;
                        }
                        String Text = (f && !f->IsNull) ? f->AsString : String();
                        varXlApp.OlePropertyGet("ActiveCell").OlePropertySet<WideString>("FormulaR1C1", Text);
                    }
                    else if ( qry->Fields->Fields[i]->FieldName.Pos("TYPESYSTEM") != 0 )
                    {
                        int key = qry->Fields->FieldByName("SC_ENUMVAL")->AsInteger;
                        TField *f = NULL;
                        switch( key )
                        {
                            case 1: f = qry->Fields->FindField("ATS_NAMESYSTEM");
                                break;
                            case 2: f = qry->Fields->FindField("ARS_CODSYSTEM");
                                break;
                            case 3: f = qry->Fields->FindField("LFMFSYSTEM");
                                break;
                            case 4: f = qry->Fields->FindField("DTS_NAMESYSTEM");
                                break;
                        }
                        String Text = (f && !f->IsNull) ? f->AsString : String();
                        varXlApp.OlePropertyGet("ActiveCell").OlePropertySet<WideString>("FormulaR1C1", Text);
                    }
                    else
                        varXlApp.OlePropertyGet("ActiveCell").OlePropertySet<WideString>("FormulaR1C1", qry->Fields->Fields[i]->AsString);
                //}
                colIndex++;
            }
        }

        qry->Next();
        rowIndex++;
    }

    qry->EnableControls();

    colIndex--;
    if (colIndex >= 0) {
        varXlApp.OlePropertyGet<WideString>("Columns", AnsiString("A:")+nToAz(colIndex)).OlePropertyGet("EntireColumn").OleProcedure("AutoFit");
        varXlApp.OlePropertyGet<WideString>("Range", "A1").OleProcedure("Select");
    }

    varXlApp.OlePropertySet<VARIANT_BOOL>("Visible", true);
    varXlApp = Unassigned;
}
//---------------------------------------------------------------------------

//  преобразование цифрового индекса колонки в буквенный
AnsiString __fastcall TfrmSearch::nToAz(int n)
{
    static const int base = 'Z' - 'A' + 1;
    AnsiString res;
    int len = 1;
    int val = n;
    while (val /= base)
        len++;
    res.SetLength(len);
    val = n;
    while (len > 0) {
        res[len--] = 'A' + val % base;
        val /= base;
        //  коррекция. без неё ненулевые разряды начнутся с 'B'
        val--;
    }
    return res;
}


void __fastcall TfrmSearch::ExcelFormClose(TObject *, TCloseAction &)
{
    this->stopExcel = true;
}
//---------------------------------------------------------------------------


void __fastcall TfrmSearch::btnCloseClick(TObject *Sender)
{
    TControl *ctl = dynamic_cast<TControl *>(Sender);
    if (ctl)
    {
        TForm *frm = dynamic_cast<TForm *>(ctl->Parent);
        if (frm)
            frm->Close();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSearch::LoadFields()
{
    baseFields.clear();

    std::auto_ptr<TIniFile> ini(new TIniFile(iniFieldsFileName));
    std::auto_ptr<TStrings> Sections(new TStringList());
    ini->ReadSections(Sections.get());

    for(int i = 0; i < Sections->Count; i++)//читаем в циле все секции из файла
    {
            Cfields field;
            field.alias = ini->ReadString(Sections->Strings[i], "Alias", "");
            field.displayLabel = ini->ReadString(Sections->Strings[i], "DisplayLabel", "");
            field.fieldName = Sections->Strings[i];
            field.selected = ini->ReadBool(Sections->Strings[i], "Selected", false);
            field.visible = ini->ReadBool(Sections->Strings[i], "Visible", false);
            field.width = ini->ReadInteger(Sections->Strings[i], "Width", 0);
            baseFields.push_back(field);
    }
}

//---------------------------------------------------------------------------
__fastcall TfrmSearch::TfrmSearch(TComponent* Owner)
    : TForm(Owner)
{
    lat = 0;
    lon = 0;
    edtLon->Text = dmMain->coordToStr(lon, 'X');
    edtLat->Text = dmMain->coordToStr(lat, 'Y');
    pcSearch->ActivePage = tshCriteria;
    chbTerritoryClick(this);
    chbCityClick(this);
    chbNumberClick(this);
    rgrChFBClick(this);
    sqlCountry->Database->Open();
    sqlCountry->Transaction->Active = true;
    sqlCountry->Transaction->CommitRetaining();
    fillCombo(cbxCountry, sqlCountry, 0, 0);

    edSiteName->Hint = "Для задания любой последовательности символов используй знак '%'.\n"
                        "Для задания самого знака '%' в строке используй последовательность '\\%'";

    txList.CreateInstance(CLSID_LISBCTxList);

    ProgressBar1->Top = 2;
    ProgressBar1->Left = 0;
    ProgressBar1->Parent = sbSearch;
    ProgressBar1->Height = sbSearch->Height - 2;
    ProgressBar1->Width = sbSearch->Width;

    /*
    TToolButton *tb7 = new TToolButton(this);
    tb7->Parent = cmf->tb;
    tb7->Grouped = true;
    tb7->Style = tbsCheck;
    tb7->ImageIndex = 10;
    tb7->OnClick = tbtSelectionClick;
    tb7->Hint = "Задать прямоугольный регион поиска";
    */
    tb7->Left = (cmf->tb->ButtonWidth) * 2;

    cmf->omsCallBack = OnObjectSelection;
}
//---------------------------------------------------------------------------
void __fastcall TfrmSearch::edtRegionChange(TObject *)
{
    edtRegion->Text = Trim(edtRegion->Text);
    if ( edtRegion->Text.Length() > 4 )
    {
        AnsiString str = Trim(edtRegion->Text);
        str.Delete(str.Length(), 1);
        edtRegion->Text = str;
        edtTransmitter->SetFocus();
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmSearch::edtTransmitterChange(TObject *)
{
    edtTransmitter->Text = Trim(edtTransmitter->Text);
    if ( edtTransmitter->Text.Length() > 4 )
    {
        AnsiString str = Trim(edtTransmitter->Text);
        str.Delete(str.Length(), 1);
        edtTransmitter->Text = str;
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmSearch::edtLonExit(TObject *Sender)
{
    TEdit *ed = dynamic_cast<TEdit*>(Sender);
    if (ed)
        ed->Text = dmMain->coordToStr(lon = dmMain->strToCoord(ed->Text), 'X');
}
//---------------------------------------------------------------------------
void __fastcall TfrmSearch::edtLatExit(TObject *Sender)
{
    TEdit *ed = dynamic_cast<TEdit*>(Sender);
    if (ed)
        ed->Text = dmMain->coordToStr(lat = dmMain->strToCoord(ed->Text), 'Y');
}
//---------------------------------------------------------------------------
void __fastcall TfrmSearch::edtMaxRadiusExit(TObject *)
{
    edtMaxRadius->Text.ToInt();
}
//---------------------------------------------------------------------------
void __fastcall TfrmSearch::chbNumberClick(TObject *)
{

    bool checked = chbNumber->Checked;
    edtRegion->Enabled = checked;
    edtTransmitter->Enabled = checked;
    Label7->Enabled = checked;
    Label8->Enabled = checked;
    if (checked)
    {
        edtRegion->Color =  clWindow;
        edtTransmitter->Color =  clWindow;
        edtRegion->Font->Color =  clWindowText;
        edtTransmitter->Font->Color =  clWindowText;
    } else
    {
        edtRegion->Color =  Color;
        edtTransmitter->Color =  Color;
        edtRegion->Font->Color =  Color;
        edtTransmitter->Font->Color =  Color;
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmSearch::chbTerritoryClick(TObject *)
{
    bool checked = chbTerritory->Checked;
    for (int i = 0; i < ComponentCount; i++)
    {
        TControl *ctrl = dynamic_cast<TControl*>(Components[i]);
        if (ctrl && ctrl->Parent == gbTerr && ctrl != chbTerritory)
        {
            ctrl->Enabled = checked;
            TEdit *ed = dynamic_cast<TEdit*>(ctrl);
            if (ed)
            {
                ed->Color = checked ? clWindow : Color;
                ed->Font->Color = checked ? clWindowText : Color;
            }
        }
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmSearch::chbCityClick(TObject *)
{
    bool checked = chbCity->Checked;
    Label4->Enabled = checked;
    Label5->Enabled = checked;
    Label6->Enabled = checked;
    cbxCountry->Enabled = checked;
    cbxArea->Enabled = checked;
    cbxCity->Enabled = checked;
    if (checked) {
        cbxCountry->Color = clWindow;
        cbxArea->Color = clWindow;
        cbxCity->Color = clWindow;
        cbxCountry->Font->Color = clWindowText;
        cbxArea->Font->Color = clWindowText;
        cbxCity->Font->Color = clWindowText;
    } else {
        cbxCountry->Color = Color;
        cbxArea->Color = Color;
        cbxCity->Color = Color;
        cbxCountry->Font->Color = Color;
        cbxArea->Font->Color = Color;
        cbxCity->Font->Color = Color;
    }
}
//---------------------------------------------------------------------------
AnsiString __fastcall TfrmSearch::getClause()
{
    AnsiString query("");
    String andStr(" and ");

    if ( chbTerritory->Checked )
    {
        String top, right, down, left;
        if (rbPoint->Checked)
        {
            double lon, lat;

            lon = dmMain->strToCoord(edtLon->Text);
            lat = dmMain->strToCoord(edtLat->Text);

            double distance = edtMaxRadius->Text.ToDouble();
            double lon_out, lat_out;

            txAnalyzer.GetPoint(lon, lat, 0, distance, &lon_out, &lat_out);
            top.sprintf("%f", lat_out);

            txAnalyzer.GetPoint(lon, lat, 90, distance, &lon_out, &lat_out);
            right.sprintf("%f", lon_out);

            txAnalyzer.GetPoint(lon, lat, 180, distance, &lon_out, &lat_out);
            down.sprintf("%f", lat_out);

            txAnalyzer.GetPoint(lon, lat, 270, distance, &lon_out, &lat_out);
            left.sprintf("%f", lon_out);

        } else {
            top.sprintf("%f", dmMain->strToCoord(edLatTop->Text));
            right.sprintf("%f", dmMain->strToCoord(edLonRight->Text));
            down.sprintf("%f", dmMain->strToCoord(edLatBottom->Text));
            left.sprintf("%f", dmMain->strToCoord(edLonLeft->Text));
        }
        query = " ((ST.LATITUDE between " + down + andStr + top + ") and (ST.LONGITUDE between " + left + andStr + right + "))";
    }

    if ( chbCity->Checked )
    {
        if ( ( cbxCity->Text != "" ) && ( cbxCity->Text != "<всі>" ) )
        {
            if ( chbTerritory->Checked )
                query += andStr;
            query += "(CITY_ID=" + IntToStr((int)cbxCity->Items->Objects[cbxCity->ItemIndex]) + ')';
        }
        else
        if ( ( cbxArea->Text != "" ) && ( cbxArea->Text != "<всі>" ) )
        {
            if ( chbTerritory->Checked )
                query += andStr;
            query += "(AREA_ID=" + IntToStr((int)cbxArea->Items->Objects[cbxArea->ItemIndex]) + ')';
        }
        else
        if ( ( cbxCountry->Text != "" ) && ( cbxCountry->Text != "<всі>" ) )
        {
            if ( chbTerritory->Checked )
                query += andStr;
            query += "(AR.COUNTRY_ID=" + IntToStr((int)cbxCountry->Items->Objects[cbxCountry->ItemIndex]) + ')';
        }
    }

    if (pnTx->Visible)
    {
        if (chbNumber->Checked)
        {
            if ( ( chbTerritory->Checked ) || ( chbCity->Checked ) )
                query += andStr;
            if ( edtRegion->Text != "" )
                query += "(AREA.NUMREGION LIKE upper('%" + edtRegion->Text.UpperCase() + "%'))";
            if ( edtTransmitter->Text != "" )
            {
                if ( edtRegion->Text != "" )
                    query += andStr;
                query += "(TX.ADMINISTRATIONID like upper('%" + edtTransmitter->Text.UpperCase() + "%'))";
            }
        }

        AnsiString ChFB("");
        if ( cbxChannel->ItemIndex != -1 )
            ChFB = "(TX.CHANNEL_ID=" + IntToStr((int)cbxChannel->Items->Objects[cbxChannel->ItemIndex]) + ')';
        else if ( edtFrequency->Text != "" )
            ChFB = "(TX.SOUND_CARRIER_PRIMARY=" + edtFrequency->Text + " )";
        else if ( cbxBlock->ItemIndex != -1 )
            ChFB = "(TX.ALLOTMENTBLOCKDAB_ID=" + IntToStr((int)cbxBlock->Items->Objects[cbxBlock->ItemIndex]) + ')';

        if ( ChFB != "" )
        {
            if ( query != "" )
                 query += andStr;
            query += ChFB;
        }
    }

    if (pnSite->Visible)
    {
        if (cbHgtSea->ItemIndex > -1 && !edHgtSea->Text.IsEmpty() && edHgtSea->Text != "0")
        {
            if (!query.IsEmpty())
                query += andStr;
            query += ("ST.HEIGHT_SEA" + cbHgtSea->Text + edHgtSea->Text);
        }
        if (cbHgtGnd->ItemIndex > -1 && !edHgtGnd->Text.IsEmpty() && edHgtGnd->Text != "0")
        {
            if (!query.IsEmpty())
                query += andStr;
            query += ("ST.MAX_ANT_HGT" + cbHgtGnd->Text + edHgtGnd->Text);
        }
        if (!edSiteName->Text.IsEmpty())
        {
            if (!query.IsEmpty())
                query += andStr;
            query += ("ST.NAMESITE like '%" + edSiteName->Text + "%' escape '\'");
        }
    }

    if (fmWhereCriteria1->getClause() != "")
        if ( query != "" )
             query += andStr;
        query +=  fmWhereCriteria1->getClause();

    if (query != "")
    {
        //загружаем поля запроса
        LoadFields();

        //добавляем поля к запросу
        AnsiString selectQuery = "select ";
        if (Tag == otTxSearch)
            selectQuery += "TX.ID TX_ID, ";
        else if (Tag == otSITES)
            selectQuery += "ST.ID ST_ID, ";
            
        for(unsigned int i = 0; i < baseFields.size(); i++)
            if ( baseFields[i].selected )
            {
                selectQuery +=  " " + baseFields[i].fieldName;
                if ( baseFields[i].alias != "" )
                    selectQuery += " " + baseFields[i].alias;
                selectQuery += ',';
            }
        selectQuery.Delete(selectQuery.Length(), 1);

        query =  fmWhereCriteria1->sqlQuery + " where " + query;

        query = selectQuery + " " + query;

        if (( addSortedFields ) && ( sortedFields.size() > 0 ))
        {
            AnsiString queryOrderBy(" order by ");
            for(unsigned int i = 0; i < sortedFields.size(); i++)
                queryOrderBy += (sortedFields[i].fieldName + ",");
            queryOrderBy.Delete(queryOrderBy.Length(), 1);

            query += queryOrderBy;
        }
    }

    return query;
}
//---------------------------------------------------------------------------
void __fastcall TfrmSearch::actRunExecute(TObject *)
{
    TempCursor tc(crHourGlass);
    
    ibqQuery->Close();

    pcSearch->ActivePage = tshResults;

    ibqQuery->SQL->Text = getClause();

    txList.Clear();

    if ( ibqQuery->SQL->Text == "" )
        throw Exception("Не заданы критерии поиска");
    else
        ibqQuery->Open();

    for (int i = 0; i < ibqQuery->Fields->Count; i++)
    {
        TFloatField *ff = dynamic_cast<TFloatField*>(ibqQuery->Fields->Fields[i]);
        if (ff && (ff->Origin.UpperCase().Pos("ERP") > 0 || ff->Origin.UpperCase().Pos("EPR") > 0 || ff->Origin.UpperCase().Pos("POWER") > 0))
            ff->DisplayFormat = "#.00";
    }
}
//---------------------------------------------------------------------------

void TfrmSearch::ParseIni(AnsiString source, std::vector<Cfields> &where)
{
    where.clear();

    std::auto_ptr<TIniFile> ini(new TIniFile(source));
    std::auto_ptr<TStrings> Sections(new TStringList());
    ini->ReadSections(Sections.get());

    for(int i = 0; i < Sections->Count; i++)//читаем в циле все секции из файла
    {
        Cfields field;
        field.alias = ini->ReadString(Sections->Strings[i], "Alias", "");
        field.displayLabel = ini->ReadString(Sections->Strings[i], "DisplayLabel", "");
        if ( field.displayLabel == "" )
            field.displayLabel = Sections->Strings[i];
        field.fieldName = Sections->Strings[i];
        field.selected = ini->ReadBool(Sections->Strings[i], "Selected", false);
        field.visible = ini->ReadBool(Sections->Strings[i], "Visible", false);
        field.width = ini->ReadInteger(Sections->Strings[i], "Width", 0);
        where.push_back(field);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSearch::actColumnsExecute(TObject *Sender)
//для выбора полей
{
    std::auto_ptr<TfrmSelectColumns> frmSelectColumns(new TfrmSelectColumns(Application));
      frmSelectColumns->Caption = "Выбор полей";

    std::vector<Cfields> fields;//вектор для хранения полей из INI
    ParseIni(iniFieldsFileName, fields);

    //добавляем в листы элементы массива
    for(unsigned int i = 0; i < fields.size(); i++)
        if( fields[i].visible )
            if( fields[i].selected )
                frmSelectColumns->lbSelectedFields->Items->AddObject(fields[i].displayLabel, (TObject *)i);
            else
                frmSelectColumns->lbAccessibleFields->Items->AddObject(fields[i].displayLabel, (TObject *)i);

    if ( frmSelectColumns->ShowModal() == mrOk )
    {
        std::auto_ptr<TIniFile> ini(new TIniFile(iniFieldsFileName));

        std::auto_ptr<TStrings> Sections(new TStringList());
        ini->ReadSections(Sections.get());

        //удаляем из INI все секции
        for(int i = 0; i < Sections->Count; i++)
            ini->EraseSection(Sections->Strings[i]);

        //делаем невыбранные поля невыбранными
        for(int i = 0; i < frmSelectColumns->lbAccessibleFields->Items->Count; i++)
            fields[(int)frmSelectColumns->lbAccessibleFields->Items->Objects[i]].selected = false;

        //делаем выбранные поля выбранными
        for(int i = 0; i < frmSelectColumns->lbSelectedFields->Items->Count; i++)
            fields[(int)frmSelectColumns->lbSelectedFields->Items->Objects[i]].selected = true;

        for(unsigned int i = 0; i < fields.size(); i++)
            if ( ( fields[i].fieldName == "CH.NAMECHANNEL" ) || ( fields[i].fieldName == "TX.TYPESYSTEM" ) )
                if ( fields[i].selected )
                    for(unsigned int i = 0; i < fields.size(); i++)
                        if ( ( fields[i].fieldName == "TX.SYSTEMCAST_ID" ) || ( fields[i].fieldName == "SC.ENUMVAL" ) )
                            fields[i].selected = true;


        baseFields.clear();

        //пишем в вектор все выбранные поля ( с сортировкой )
        for(int i = 0; i < frmSelectColumns->lbSelectedFields->Items->Count; i++)
        {
            baseFields.push_back(fields[(unsigned int)(frmSelectColumns->lbSelectedFields->Items->Objects[i])]);
            fields[(unsigned int)(frmSelectColumns->lbSelectedFields->Items->Objects[i])].parsed = true;
        }

        //в вектор все оставшиеся элементы
        for(unsigned int i = 0; i < fields.size(); i++)
            if ( !fields[i].parsed )
                baseFields.push_back(fields[i]);

        //пишем в INI новый список полей
        for(unsigned int i = 0; i < baseFields.size(); i++)
        {
            AnsiString name = baseFields[i].fieldName;

            ini->WriteString(name, "Alias", baseFields[i].alias);
            ini->WriteString(name, "DisplayLabel", baseFields[i].displayLabel);
            ini->WriteBool(name, "Selected", baseFields[i].selected);
            ini->WriteBool(name, "Visible", baseFields[i].visible);
            ini->WriteInteger(name, "Width", baseFields[i].width);
        }
    }

    if ( ( ibqQuery->Active ) || ( pcSearch->ActivePage == tshResults ) )
        actRunExecute(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TfrmSearch::actSortOrderExecute(TObject *Sender)
{
//для сортировки полей
    std::auto_ptr<TfrmSelectColumns> frmSelectColumns(new TfrmSelectColumns(Application));
      frmSelectColumns->Caption = "Сортировка";
      frmSelectColumns->lbAccessibleFields->Sorted = true;

    //загружаем список полей
    LoadFields();

    //добавляем в лист элементы массива
    for(unsigned int i = 0; i < baseFields.size(); i++)
        if( ( baseFields[i].visible ) && ( baseFields[i].selected ) )
            if ( baseFields[i].displayLabel != "" )
                frmSelectColumns->lbAccessibleFields->Items->AddObject(baseFields[i].displayLabel, (TObject *)i);
            else
                frmSelectColumns->lbAccessibleFields->Items->AddObject(baseFields[i].fieldName, (TObject *)i);

    if ( frmSelectColumns->ShowModal() == mrOk )
    {
        sortedFields.clear();
        //пишем в вектор выбрнные и отсортированные поля
        for(int i = 0; i < frmSelectColumns->lbSelectedFields->Items->Count; i++)
        {
            if ( baseFields[(int)frmSelectColumns->lbSelectedFields->Items->Objects[i]].fieldName == "CH.NAMECHANNEL" )
            {//при сортировке по К/Ч/Б сортируем по несущей
                for(unsigned int j = 0; j < baseFields.size(); j++)
                    if( baseFields[j].fieldName == "TX.CARRIER")
                        sortedFields.push_back(baseFields[j]);
            }
            else
                sortedFields.push_back(baseFields[(int)frmSelectColumns->lbSelectedFields->Items->Objects[i]]);
        }

        addSortedFields = true;

        actRunExecute(NULL);
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmSearch::FormClose(TObject *, TCloseAction &Action)
{
    Action = caFree;
}
//---------------------------------------------------------------------------
void __fastcall TfrmSearch::fillCombo(TComboBox* cbx, TIBSQL* sql, int parentId, int elementId)
{
    //  заполяем указанный TComboBox значениями из выборки указанного sql с параметром parentId
    //  и устанавливаем текущий элемент на elementId, если есть.
    cbx->Items->Clear();
    cbx->Text = "";
    if (cbx != cbxCountry)
        cbx->Items->AddObject("<всі>", (TObject*)0);
    sql->Close();
    sql->Transaction->CommitRetaining();
    if (!sql->Params->Names.IsEmpty())
        sql->Params->Vars[0]->AsInteger = parentId;
    sql->ExecQuery();
    while (!sql->Eof) {
        cbx->Items->AddObject(sql->Fields[1]->AsString, (TObject*)sql->Fields[0]->AsInteger);
        sql->Next();
    }
    sql->Close();

    //  установить нужный элемент
    if (elementId) {
        cbx->ItemIndex = cbx->Items->IndexOfObject((TObject*)elementId);
    }
    //else if (cbx->Items->Count > 0) {
    //    cbx->ItemIndex = 0;
    //}
    if (cbx->OnChange)
        cbx->OnChange(this);
    if (cbx->Items->Count < Screen->Height / cbx->ItemHeight - 5)
        cbx->DropDownCount = cbx->Items->Count;
    else
        cbx->DropDownCount = Screen->Height / cbx->ItemHeight - 5;
}

void __fastcall TfrmSearch::cbxCountryChange(TObject *)
{
    if (cbxCountry->ItemIndex != -1)
        fillCombo(cbxArea, sqlArea, (int)cbxCountry->Items->Objects[cbxCountry->ItemIndex], 0);
}
//---------------------------------------------------------------------------

void __fastcall TfrmSearch::cbxAreaChange(TObject *)
{
    if (cbxArea->ItemIndex != -1)
        fillCombo(cbxCity, sqlCity, (int)cbxArea->Items->Objects[cbxArea->ItemIndex], 0);
}
//---------------------------------------------------------------------------


void __fastcall TfrmSearch::tshSQLShow(TObject *)
{
    memSQL->Text = getClause();
}
//---------------------------------------------------------------------------
void __fastcall TfrmSearch::query_lat_lon_OnGetText(TField* Sender, AnsiString &Text, bool DisplayText)
{
    DisplayText = true;
    if ( Sender->IsNull )
        Text = "";
    else
        if ( Sender->FieldName.Pos("LATITUDE") != 0 )
            Text = dmMain->coordToStr(Sender->AsFloat, 'Y');
        else
            Text = dmMain->coordToStr(Sender->AsFloat, 'X');
}
//---------------------------------------------------------------------------
void __fastcall TfrmSearch::query_K_CH_B_OnGetText(TField* , AnsiString &Text, bool DisplayText)
{
    DisplayText = true;
    int key = ibqQuery->Fields->FieldByName("SC_ENUMVAL")->AsInteger;
    TField *f = NULL;
    switch( key )
    {
        case 1: 
        case 4: f = ibqQuery->Fields->FindField("NAMECHANNEL");
            break;
        case -1:
        case 2:
        case 3: f = ibqQuery->Fields->FindField("SOUND_CARRIER_PRIMARY");
            break;
        case 5: f = ibqQuery->Fields->FindField("BD_NAME");
            break;
    }
    Text = (f && !f->IsNull) ? f->AsString : String();
}
//---------------------------------------------------------------------------
void __fastcall TfrmSearch::query_SystemType_OnGetText(TField*, AnsiString &Text, bool DisplayText)
{
    DisplayText = true;
    int key = ibqQuery->Fields->FieldByName("SC_ENUMVAL")->AsInteger;
    TField *f = NULL;
    switch( key )
    {
        case 1: f = ibqQuery->Fields->FindField("ATS_NAMESYSTEM");
            break;
        case 2: f = ibqQuery->Fields->FindField("ARS_CODSYSTEM");
            break;
        case 3: f = ibqQuery->Fields->FindField("LFMFSYSTEM");
            break;
        case 4: f = ibqQuery->Fields->FindField("DTS_NAMESYSTEM");
            break;
    }
    Text = (f && !f->IsNull) ? f->AsString : String();
}
//---------------------------------------------------------------------------

void __fastcall TfrmSearch::ibqQueryAfterOpen(TDataSet *DataSet)
{
    for(int i = 1; i <= ibqQuery->FieldCount; i++)
    {
        //цепляем обработчик OnGetText для приведения LATITUDE и LONGITUDE к читабельному виду
        if ( ( DataSet->Fields->FieldByNumber(i)->FieldName.Pos("LATITUDE") != 0 ) || ( DataSet->Fields->FieldByNumber(i)->FieldName.Pos("LONGITUDE") != 0 ) )
            DataSet->Fields->FieldByNumber(i)->OnGetText =  query_lat_lon_OnGetText;
        //обработчик для К/Ч/Б
        if ( DataSet->Fields->FieldByNumber(i)->FieldName.Pos("NAMECHANNEL") != 0 )
            DataSet->Fields->FieldByNumber(i)->OnGetText =  query_K_CH_B_OnGetText;
        //обработчик для "Тип системы"
        if ( DataSet->Fields->FieldByNumber(i)->FieldName.Pos("TYPESYSTEM") != 0 )
            DataSet->Fields->FieldByNumber(i)->OnGetText =  query_SystemType_OnGetText;

        //присваиваем полю читабельное название
        for(unsigned int j = 0; j < baseFields.size(); j++)
        {
            if (DataSet->Fields->FieldByNumber(i)->DisplayName == "TX_ID1"
            && DataSet->Fields->FieldByNumber(i)->DisplayName == "ST_ID1")//TEST CODE
                DataSet->Fields->FieldByNumber(i)->Visible = false;
            else if ( baseFields[j].alias != "" )
            {
                if ( baseFields[j].alias == DataSet->Fields->FieldByNumber(i)->DisplayName )
                {
                    if ( baseFields[j].displayLabel != "" )
                        DataSet->Fields->FieldByNumber(i)->DisplayLabel = baseFields[j].displayLabel;
                    else
                      if ( baseFields[j].alias != "" )
                        DataSet->Fields->FieldByNumber(i)->DisplayLabel = baseFields[j].alias;
                      else
                        DataSet->Fields->FieldByNumber(i)->DisplayLabel = baseFields[j].fieldName;
                    DataSet->Fields->FieldByNumber(i)->Visible = baseFields[j].visible;
                }
            }
            else
              if ( fieldNameFromFullName(baseFields[j].fieldName).Pos(DataSet->Fields->FieldByNumber(i)->DisplayName) != 0 )
              {
                  if ( baseFields[j].displayLabel != "" )
                      DataSet->Fields->FieldByNumber(i)->DisplayLabel = baseFields[j].displayLabel;
                  else
                    if ( baseFields[j].alias != "" )
                      DataSet->Fields->FieldByNumber(i)->DisplayLabel = baseFields[j].alias;
                    else
                       DataSet->Fields->FieldByNumber(i)->DisplayLabel = baseFields[j].fieldName;
                  DataSet->Fields->FieldByNumber(i)->Visible = baseFields[j].visible;
              }

        }
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmSearch::dbgRsultsMouseMove(TObject *,  TShiftState Shift, int x, int y)
{
    //для Drag&Drop
    if (Tag == otTxSearch)
    {
        if ((Shift == (TShiftState() << ssLeft)) && (!dbgRsults->Options.Contains(dgTitles) || (y > abs(dbgRsults->TitleFont->Height)+4)))
            dbgRsults->BeginDrag(false, 3);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSearch::dbgRsultsDblClick(TObject *)
{
    //вызов карточки передатчика
    if (Tag == otTxSearch)
    {
        int objId = ibqQuery->Fields->FieldByName("TX_ID")->AsInteger;
        FormProvider.ShowTx(txBroker.GetTx(objId, dmMain->GetObjClsid(dmMain->GetSc(objId))));
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmSearch::actExportExecute(TObject *Sender)
{
    //экспорт списка телевизионных передатчиков
    if (Tag == otTxSearch)
    {
        if (ibqQuery->RecordCount == 0) {
            Application->MessageBox("Список пустий.", Application->Title.c_str(), MB_ICONEXCLAMATION);
            return;
        }

        if (!dlgExport)
            dlgExport = new TdlgExport(Application);

        if (dlgExport->ShowModal() == mrOk)
            txExporter.exportTxGrid(dlgExport->rgFormat->ItemIndex, dlgExport->rgList->ItemIndex, dbgRsults, dlgExport->edtFilename->Text);
    }
}
//---------------------------------------------------------------------------


void __fastcall TfrmSearch::FormCreate(TObject *Sender)
{
    try {
        LayoutManager.loadLayout(this);
        LayoutManager.EnsureShortcut(this);
    } catch (...) {
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSearch::FormDestroy(TObject *Sender)
{
    try {
        LayoutManager.saveLayout(this);
        LayoutManager.DeleteShortcut(this);
    } catch (...) {
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSearch::rgrChFBClick(TObject *Sender)
{
    cbxChannel->Clear();
    cbxChannel->Color =  Color;
    cbxChannel->Enabled = false;
    cbxChannel->Font->Color =  Color;
    cbxChannel->ItemIndex = -1;
    btChannelClear->Enabled = false;

    edtFrequency->Color =  Color;
    edtFrequency->Enabled = false;
    edtFrequency->Font->Color =  Color;
    edtFrequency->Text = "";
    btFrequencyClear->Enabled = false;

    cbxBlock->Clear();
    cbxBlock->Color =  Color;
    cbxBlock->Enabled = false;
    cbxBlock->Font->Color =  Color;
    cbxBlock->ItemIndex = -1;
    btBlockClear->Enabled = false;

    if ( rgrChFB->ItemIndex == 0)
    {
        cbxChannel->Color = clWindow;
        cbxChannel->Enabled = true;
        cbxChannel->Font->Color = clWindowText;
        btChannelClear->Enabled = true;
    }
    else if ( rgrChFB->ItemIndex == 1)
    {
        edtFrequency->Color = clWindow;
        edtFrequency->Enabled = true;
        edtFrequency->Font->Color = clWindowText;
        btFrequencyClear->Enabled = true;
    }
    else if ( rgrChFB->ItemIndex == 2)
    {
        cbxBlock->Color = clWindow;
        cbxBlock->Enabled = true;
        cbxBlock->Font->Color = clWindowText;
        btBlockClear->Enabled = true;
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSearch::btChannelClearClick(TObject *Sender)
{
    cbxChannel->Items->Clear();
    cbxChannel->ItemIndex = -1;
    cbxChannel->Text = "";
}
//---------------------------------------------------------------------------



void __fastcall TfrmSearch::btFrequencyClearClick(TObject *Sender)
{
    edtFrequency->Text = "";
}
//---------------------------------------------------------------------------

void __fastcall TfrmSearch::btBlockClearClick(TObject *Sender)
{
    cbxBlock->Items->Clear();
    cbxBlock->ItemIndex = -1;
    cbxBlock->Text = "";
}
//---------------------------------------------------------------------------

void __fastcall TfrmSearch::cbxChannelDropDown(TObject *Sender)
{
    std::auto_ptr<TIBQuery> sql(new TIBQuery(this));
      sql->Database = dmMain->dbMain;
      sql->SQL->Text = "select ch.ID, NAMECHANNEL, FREQUENCYGRID.NAME FREQUENCYGRID_NAME from CHANNELS ch left outer join FREQUENCYGRID on (CHANNELS.FREQUENCYGRID_ID = FREQUENCYGRID.ID) order by FREQUENCYGRID.NAME, NAMECHANNEL";
      sql->Transaction = dmMain->trMain;
    sql->Open();

    sql->First();

    cbxChannel->Clear();
    while ( !sql->Eof )
    {
        cbxChannel->Items->AddObject(sql->Fields->FieldByName("NAMECHANNEL")->AsString + " ( " + sql->Fields->FieldByName("FREQUENCYGRID_NAME")->AsString + " )", (TObject*)sql->Fields->FieldByName("ID")->AsInteger);
        sql->Next();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSearch::cbxBlockDropDown(TObject *Sender)
{
    std::auto_ptr<TIBQuery> sql(new TIBQuery(this));
      sql->Database = dmMain->dbMain;
      sql->SQL->Text = "select ID, NAME from BLOCKDAB";
      sql->Transaction = dmMain->trMain;
    sql->Open();

    sql->First();

    cbxBlock->Clear();
    while ( !sql->Eof )
    {
        cbxBlock->Items->AddObject(sql->Fields->FieldByName("NAME")->AsString, (TObject*)sql->Fields->FieldByName("ID")->AsInteger);
        sql->Next();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSearch::actNoneExecute(TObject *Sender)
{
    cmf->bmf->Map->CurrentTool = miArrowTool;
    cmf->bmf->Map->Cursor = crGetTx;
}
//---------------------------------------------------------------------------

void __fastcall TfrmSearch::actDistanceExecute(TObject *Sender)
{
    //frmMap->theMap->LeftMouseMode = mmToolDistance;
    cmf->bmf->Map->Cursor = crCross;
}
//---------------------------------------------------------------------------

void __fastcall TfrmSearch::actPanExecute(TObject *Sender)
{
    cmf->bmf->Map->CurrentTool = miPanTool;
    cmf->bmf->Map->Cursor = crDefault;
}
//---------------------------------------------------------------------------

void __fastcall TfrmSearch::actZoomInExecute(TObject *Sender)
{
    cmf->bmf->Map->CurrentTool = miZoomOutTool;
    cmf->bmf->Map->Cursor = crDefault;
}
//---------------------------------------------------------------------------

void __fastcall TfrmSearch::actZoomOutExecute(TObject *Sender)
{
    cmf->bmf->Map->CurrentTool = miZoomInTool;
    cmf->bmf->Map->Cursor = crDefault;
}
//---------------------------------------------------------------------------

void __fastcall TfrmSearch::actZoomFitExecute(TObject *Sender)
{
    cmf->bmf->FitObjects();
}
//---------------------------------------------------------------------------

void __fastcall TfrmSearch::actSaveBmpExecute(TObject *Sender)
{
    //frmMap->saveToBmp();
}
//---------------------------------------------------------------------------

void __fastcall TfrmSearch::pcSearchChange(TObject *Sender)
{
    if (pcSearch->ActivePage == tshMap)
        ActivateMapSheet();
}
//---------------------------------------------------------------------------


void __fastcall TfrmSearch::ActivateMapSheet()
{
    bool firstTime = false;
    try {
        cmf->Init();
        firstTime = true;

        cmf->bmf->SetCenter(31.25, 48.60);
        cmf->bmf->SetScale(1545.);
        cmf->bmf->actPanButtons->Checked = false;
        cmf->bmf->actPanButtonsExecute(this);

        cmf->bmf->Map->CreateCustomTool(miSelectRegionTool, miToolTypeMarquee, TVariant(miCrossCursor), TVariant(NULL), TVariant(NULL), TVariant(0));
        cmf->bmf->OnToolUsed = MapToolUsed;

    } catch(...) {}

    cmf->Clear(-1);
    shapeIdx.clear();
    actNoneExecute(this);

    TempCursor tc(crHourGlass);
    if (ibqQuery->Active)
    {
        TempCursor tc(crHourGlass);
        TBookmark bm = ibqQuery->GetBookmark();
        try {
            ibqQuery->DisableControls();
            ibqQuery->FetchAll();
            ibqQuery->First();

            if (Tag == otTxSearch)
            {
                //  заполнить список передатчиков
                if (txList.Size == 0)
                {
                    while (!ibqQuery->Eof)
                    {
                        txList.AddTx(txBroker.GetTx(ibqQuery->Fields->FieldByName("TX_ID")->AsInteger,
                                                    dmMain->GetObjClsid(ibqQuery->Fields->FieldByName("SC_ENUMVAL")->AsInteger)));
                        ibqQuery->Next();
                    }
                }
                if (txList.Size != 0)
                {
                    drawTxs();
                    getCoverage(false);
                }
            }
            else if (Tag == otSITES)
            {
                std::auto_ptr<TIBSQL> sql(new TIBSQL(Application));
                sql->Database = ibqQuery->Database;
                sql->SQL->Text = "select t.ID, HEIGHTANTENNA, DB.SECTIONNAME, CH.NAMECHANNEL, "
                                 "t.BLOCKCENTREFREQ, t.SOUND_CARRIER_PRIMARY, s.CODE, s.ENUMVAL "
                                 "from TRANSMITTERS t left join CHANNELS ch on ch.id = t.CHANNEL_ID "
                                 "left join DATABASESECTION db on db.id = t.STATUS "
                                 "left join SYSTEMCAST s on s.id = t.SYSTEMCAST_ID "
                                 "where t.STAND_ID = :STAND_ID ";
                sql->Prepare();
                std::auto_ptr<TCoordinateConvertor> cc(new TCoordinateConvertor(this));
                while (!ibqQuery->Eof)
                {
                    sql->Close(); sql->Params->Vars[0]->AsInteger = ibqQuery->FieldByName("ST_ID")->AsInteger;
                    int maxAH = 0;
                    String txListStr;
                    for (sql->ExecQuery(); !sql->Eof; sql->Next())
                    {
                        TBCTxType tt = sql->FieldByName("ENUMVAL")->AsInteger;
                        String freq;
                        if (tt == ttTV || tt == ttDVB)
                            freq = sql->FieldByName("NAMECHANNEL")->AsString;
                        else if (tt == ttDAB)
                            freq = (sql->FieldByName("BLOCKCENTREFREQ")->AsString + " MHz");
                        else
                            freq = (sql->FieldByName("SOUND_CARRIER_PRIMARY")->AsString + " MHz");
                        String txStr = sql->FieldByName("CODE")->AsString + ' ' + freq + " - " + sql->FieldByName("SECTIONNAME")->AsString;
                        txListStr += (String('\n')+txStr);
                        if (maxAH < sql->FieldByName("HEIGHTANTENNA")->AsInteger)
                            maxAH = sql->FieldByName("HEIGHTANTENNA")->AsInteger;
                    }
                    String hint = cc->CoordToStr(ibqQuery->FieldByName("longitude")->AsFloat, 'X') + ' ' +
                                   cc->CoordToStr(ibqQuery->FieldByName("latitude")->AsFloat, 'Y') + '\n' +
                                   ibqQuery->FieldByName("HEIGHT_SEA")->AsString + " m above sea\n" +
                                   IntToStr(maxAH) + " m max antenna height" + txListStr;
                    cmf->ShowPoint(ibqQuery->FieldByName("longitude")->AsFloat,
                                   ibqQuery->FieldByName("latitude")->AsFloat,
                                   clMaroon, 5, ptPoint,
                                   ibqQuery->FieldByName("NAMESITE")->AsString, hint
                                   );
                    ibqQuery->Next();
                }
            }
        } __finally {
            ibqQuery->GotoBookmark(bm);
            ibqQuery->FreeBookmark(bm);
            ibqQuery->EnableControls();
        }
        if (firstTime || BCCalcParams.mapAutoFit)
            cmf->bmf->FitObjects();
    }
    cmf->bmf->Map->Refresh();
}

void TfrmSearch::drawTxs()
{
    for (int i = 0; i < txList.Size; i++) {
       TCOMILISBCTx tx(txList.get_Tx(i), true);
       MapPoint* mp = cmf->ShowStation(tx.longitude, tx.latitude, getTxName(tx), getTxName(tx));
       mp->color = i == 0 ? clBlue : clMaroon;
       shapeIdx[mp->GetId()] = tx.id;
    }
}

void __fastcall TfrmSearch::getCoverage(bool recalcAll)
{
    //  сбросим параметры рельефа
    TRSAPathParams param;
    if (!BCCalcParams.TheoPathTheSame && BCCalcParams.FPathSrv.IsBound())
    {

        param.CalcHEff = BCCalcParams.UseHeffTheo;
        param.CalcTxClearance = BCCalcParams.UseTxClearenceTheo;
        param.CalcRxClearance = BCCalcParams.UseRxClearenceTheo;
        param.CalcSeaPercent =  BCCalcParams.UseMorfologyTheo;
        param.Step = BCCalcParams.StepTheo;

        BCCalcParams.FPathSrv->Set_Params(param);
    }

    ProgressBar1->Position = ProgressBar1->Min;
    ProgressBar1->Max = txList.Size;
    ProgressBar1->Visible = true;
    Screen->Cursor = crHourGlass;

    //std::map<int, double36>::iterator vi;
    LPSAFEARRAY zone;

    //ofstream fs("CoverageZones.log", ios_base::trunc);
    try {
        for (int i = 0; i < txList.Size; i++) {
            if (txList.get_TxUseInCalc(i) || i == 0) {
                int ID = txList.get_TxId(i);

                if (coverage.find(ID) == coverage.end()) {
                    // зоны нет, будем создавать
                    zone = NULL;
                } else {
                    //  зона уже есть
                    if (recalcAll) {
                        //  удалить
                        zone = coverage[ID];
                        SafeArrayDestroy(zone);
                        zone = NULL;
                    } else
                        // ничего делать не надо
                        continue;
                }

                TCOMILISBCTx Tx(txList.get_Tx(i), true);
                txAnalyzer.GetTxZone(Tx, &zone);

                coverage[ID] = zone;

            }
            ProgressBar1->StepIt();
            Update();
        }

    } __finally {
        ProgressBar1->Visible = false;
        Screen->Cursor = crDefault;
        //  восстановим параметры расчёта
        BCCalcParams.load();
        Update();
        //fs << "================================================================================" << endl << endl;
    }
    drawCoverage();
}

void __fastcall TfrmSearch::drawCoverage()
{
    for (int i = 0; i < txList.Size; i++) {
        if (txList.get_TxShowOnMap(i) || i == 0) {
            int ID = txList.get_TxId(i);
            std::map<int, LPSAFEARRAY>::iterator vi;
            if ((vi = coverage.find(ID)) != coverage.end()) {
                TCOMILISBCTx Tx(txList.get_Tx(i), true);
                std::vector<double> zone;
                zone.reserve(36);
                for (int i = 0; i < vi->second->rgsabound[0].cElements; i++)
                    zone.push_back(((double*)vi->second->pvData)[i]);
                MapPolygon *pgn = cmf->ShowCoverageZone(Tx.longitude, Tx.latitude, zone);
                pgn->color = txList.get_TxUseInCalc(i) ? BCCalcParams.lineColorZoneCover : clGray;
                pgn->width = txList.get_TxUseInCalc(i) ? BCCalcParams.lineThicknessZoneCover : 1;
            }
        }
    }
}

AnsiString __fastcall TfrmSearch::getTxName(ILISBCTx* iTx)
{
    TCOMILISBCTx tx(iTx, true);
    AnsiString txName = tx.station_name;
    switch(tx.systemcast) {
        case ttTV: case ttDVB:
            txName = txName + ' ' + dmMain->getChannelName(tx.channel_id); break;
        case ttFM:
            txName = txName + ' ' + FormatFloat("0.###", tx.sound_carrier_primary); break;
        case ttDAB:
            txName = txName + ' ' + dmMain->getDabBlockName(tx.blockcentrefreq); break;
        default: break;
    }
    return txName;
}

void __fastcall TfrmSearch::actEditExecute(TObject *Sender)
{
    int id = shapeIdx[((TComponent*)Sender)->Tag];
    if (id != 0)
        FormProvider.ShowTx(txBroker.GetTx(id, dmMain->GetObjClsid(dmMain->GetSc(id))));
}
//---------------------------------------------------------------------------

void __fastcall TfrmSearch::dbgRsultsKeyDown(TObject *Sender, WORD &Key,
      TShiftState Shift)
{
    if (Key == VK_RETURN && Shift == TShiftState() && ibqQuery->Active && ibqQuery->RecordCount > 0)
        dbgRsultsDblClick(Sender);
}
//---------------------------------------------------------------------------
void __fastcall TfrmSearch::OnObjectSelection(TObject * Sender, TBaseMapFrame::Shapes shapes, TMouseButton Button, TShiftState Shift)
{
    if (Button == Controls::mbRight)
    {
        // bring tx menu (cascaded if (shapes.size() > 1))
        pmnTx->Items->Clear();
        if (Tag == otTxSearch)
        {
            for (int i = 0; i < shapes.size(); i++)
            {
                TMenuItem *mni = new TMenuItem(pmnTx);
                mni->Caption = shapes[i]->name;
                mni->OnClick = actEditExecute;
                mni->Tag = shapes[i]->GetId();
                pmnTx->Items->Add(mni);
            }
        }
    }
}

void __fastcall TfrmSearch::cmftb3Click(TObject *Sender)
{
    cmf->bmf->Cursor = crDefault;
    cmf->bmf->Map->CurrentTool = miZoomInTool;
}
//---------------------------------------------------------------------------

void __fastcall TfrmSearch::cmftb4Click(TObject *Sender)
{
    cmf->bmf->Cursor = crDefault;
    cmf->bmf->Map->CurrentTool = miZoomOutTool;
}
//---------------------------------------------------------------------------
void __fastcall TfrmSearch::edRegCoordEnter(TObject *Sender)
{
    rbRegion->Checked = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmSearch::edPntCoordEnter(TObject *Sender)
{
    rbPoint->Checked = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmSearch::MapToolUsed(TObject *Sender,
      short ToolNum, double X1, double Y1, double X2, double Y2,
      double Distance, VARIANT_BOOL Shift, VARIANT_BOOL Ctrl,
      VARIANT_BOOL *EnableDefault)
{
    switch (ToolNum) {
        case miSelectRegionTool:
            chbCity->Checked = false;
            chbTerritory->Checked = true;
            rbRegion->Checked = true;
            edLatTop->Text = dmMain->coordToStr(max(Y1, Y2), 'Y');
            edLatBottom->Text = dmMain->coordToStr(min(Y1, Y2), 'Y');
            edLonLeft->Text = dmMain->coordToStr(min(X1, X2), 'X');
            edLonRight->Text = dmMain->coordToStr(max(X1, X2), 'X');
            actRunExecute(this);
            pcSearch->ActivePage = tshMap;
            pcSearchChange(this);
            break;
        default:
            txAnalyzer.MapToolUsed(Sender, ToolNum, X1, Y1, X2, Y2, Distance, Shift, Ctrl, EnableDefault);
            break;
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSearch::tbtSelectionClick(TObject *Sender)
{
    cmf->bmf->Cursor = crDefault;
    cmf->bmf->Map->CurrentTool = miSelectRegionTool;
}
//---------------------------------------------------------------------------

