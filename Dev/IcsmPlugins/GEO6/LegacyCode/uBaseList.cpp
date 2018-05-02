//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uBaseList.h"
#include "uMainDM.h"
#include <Registry.hpp>
#include "FormProvider.h"
#include "uMainForm.h"
#include "uExplorer.h"
#include "uLayoutMngr.h"
#include "uListTransmitters.h"

//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TfrmBaseList *frmBaseList;
int TfrmBaseList::last_id[1000];
//---------------------------------------------------------------------------

__fastcall TfrmBaseList::TfrmBaseList(TComponent* Owner)
    : TForm(Owner)
{
    _MDIChild = true;
}
//---------------------------------------------------------------------------

__fastcall TfrmBaseList::TfrmBaseList(bool MDIChild)
    : m_caller(0), m_elementId(0), TForm((TComponent*)NULL)
{
    _MDIChild = MDIChild;
    isInitialized = false;

    try {
        Initialize();
    } catch(Exception &e) {
        Application->MessageBox(e.Message.c_str(), Application->Title.c_str(), MB_ICONHAND);
    }

    isInitialized = true;
}
//---------------------------------------------------------------------------

__fastcall TfrmBaseList::TfrmBaseList(TComponent* Owner, HWND caller, int elementId)
    : m_caller(caller), m_elementId(elementId), TForm(Owner)
{
    _MDIChild = true;
    isInitialized = false;

    try {
        Initialize();
    } catch(Exception &e) {
        Application->MessageBox(e.Message.c_str(), Application->Title.c_str(), MB_ICONHAND);
    }

    isInitialized = true;
}
//---------------------------------------------------------------------------
void __fastcall TfrmBaseList::actOkExecute(TObject *Sender)
{
    //  выбор элемента
    //  если нужно, сохраняет изменения, посылает сообщение вызвавшей форме и закрывает текущую
    if (actListApply->Enabled) {
        actListApplyExecute(Sender);
        actListApply->Enabled = false;
    }
    if (m_caller && dstList->Fields->Count > 0)
        SendMessage(m_caller, WM_LIST_ELEMENT_SELECTED, Tag, dstList->Fields->Fields[0]->AsInteger);
    Close();
}
//---------------------------------------------------------------------------
void __fastcall TfrmBaseList::actCloseExecute(TObject *Sender)
{
    Close();
}
//---------------------------------------------------------------------------
void __fastcall TfrmBaseList::actListApplyExecute(TObject *Sender)
{
    //  сохранение изменений
    try {
        dstList->Post();
        //dstList->Transaction->CommitRetaining();
    } catch (Exception &e) {
        dstList->Cancel();
        throw *(new Exception(AnsiString("Ошибка сохранения: ")+e.Message));
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmBaseList::actListInsertExecute(TObject *Sender)
{
    //  новый элемент
    NewElement();
}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseList::actListCopyExecute(TObject *Sender)
{
    //  Копирование текущей записи
    if (Application->MessageBox("Копировать текущую запись?", Application->Title.c_str(), MB_ICONQUESTION | MB_YESNO) != IDYES)
        return;

    if (dstList->Eof)
        throw *(new Exception("Справочник пуст"));
    if (dsEditModes.Contains(dstList->State))
        dstList->Post();
    AnsiString TableName = dstList->Fields->Fields[0]->Origin;
    std::map<AnsiString, Variant> id;
    id["ID"] = dstList->Fields->Fields[0]->AsInteger;
    std::map<AnsiString, Variant> params;

    int new_id =  dmMain->RecordCopy(TableName,id, params);
    if (new_id == -1)
        return;
    NewElementId = new_id;
    changeQueryCopy();
    if(new_id > 0)
        dmMain->trMain->CommitRetaining();
    else
        dmMain->trMain->RollbackRetaining();
    if (!dynamic_cast<TfrmListTransmitters *>(this)) {
        dstList->Active = false;
        dstList->Active = true;
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmBaseList::actListEditExecute(TObject *Sender)
{
    //  Редактирование элемента
    EditElement();
}
//---------------------------------------------------------------------------
void __fastcall TfrmBaseList::EditElement()
{
    dstList->Edit();
}
//---------------------------------------------------------------------------
void __fastcall TfrmBaseList::actListCancelExecute(TObject *Sender)
{
    //  Отмена изменений
    dstList->Cancel();
    dstList->Transaction->RollbackRetaining();
}
//---------------------------------------------------------------------------
void __fastcall TfrmBaseList::actListDeleteExecute(TObject *Sender)
{
    //  удаление элемента
    dstList->Delete();
}
//---------------------------------------------------------------------------
void __fastcall TfrmBaseList::FormClose(TObject *Sender,
      TCloseAction &Action)
{
    //  запоминает ИД текущей записи, закрывает набор и транзакцию
    if (dstList->Fields->Count > 0)
        //  В Tag указан номер списка
        last_id[Tag] = dstList->Fields->Fields[0]->AsInteger;
    dstList->Close();
    if (trList->Active)
        trList->Rollback();
    Action = caFree;
}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseList::setElement(Messages::TMessage &Message)
{
    //  подчинённый список прислал сообщение о выборе элемента
    //  находит нужное поле, устанавливает новое значения, обновляет поля
    for (int i = 0; i < dstList->Fields->Count; i++)
        if (dstList->Fields->Fields[i]->Tag == Message.WParam) {
            if (dstList->State != dsEdit && dstList->State != dsInsert)
                dstList->Edit();
            dstList->Fields->Fields[i]->AsInteger = Message.LParam;
            updateLookups();
        }
}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseList::FormCloseQuery(TObject *Sender,
      bool &CanClose)
{
    //  запрос на сохранение изменений (если есть)
    if (actListApply->Enabled) {
        int rep = Application->MessageBox("Сохранить изменения?", Application->Title.c_str(), MB_ICONQUESTION | MB_YESNOCANCEL);
        switch (rep) {
            case IDYES: actListApplyExecute(this);
                break;
            case IDNO:
                break;
            case IDCANCEL: CanClose = false;
                break;
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseList::FormCreate(TObject *Sender)
{
    _MDIChild = true;
    if ( _MDIChild )
        try {
            LayoutManager.loadLayout(this);
            LayoutManager.EnsureShortcut(this);
        } catch (...) {
        }
}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseList::FormDestroy(TObject *Sender)
{
    if ( _MDIChild )
        try {
            LayoutManager.saveLayout(this);
            LayoutManager.DeleteShortcut(this);
        } catch (...) {
        }
}
//---------------------------------------------------------------------------
void __fastcall TfrmBaseList::updateLookups()
{
}
//---------------------------------------------------------------------------
void __fastcall TfrmBaseList::actListSearchExecute(TObject *Sender)
{
    //  устанавливает edtIncSearch на нужное место, показывает и передаёт фокус
    if (ListFilterField.size()>0)
        if (Application->MessageBox("Видалилити існуючий фільтр?", Application->Title.c_str(), MB_ICONQUESTION | MB_YESNO) == IDYES) {
        ListFilterField.clear();
        ListFilterValue.clear();
    }
    edtIncSearch->Left = 13;

    for (int i = 0; i < dgrList->SelectedIndex; i++)
        edtIncSearch->Left += dgrList->Columns->Items[i]->Width + 1;

     AnsiString SearchParam = "";
     std::vector<AnsiString>::iterator val = ListFilterValue.begin();
     for(std::vector<AnsiString>::iterator field = ListFilterField.begin(); field != ListFilterField.end(); field++) {
        SearchParam += "(";
        SearchParam += (dstList->FieldByName(*field)->DisplayName + "='");
        SearchParam += (*val + "*') ");
        val++;
     }
     lblSearchParam->Caption = SearchParam;

    panSearch->Visible = false;
    edtIncSearch->Text = "";
    panSearch->Visible = true;
    edtIncSearch->Width = dgrList->Columns->Items[dgrList->SelectedIndex]->Width;
    edtIncSearch->SetFocus();
}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseList::actListApplyUpdate(TObject *Sender)
{
    if (dstList->State == dsInsert || dstList->State == dsEdit) {
        actListApply->Enabled = true;
        actListCancel->Enabled = true;
    } else {
        actListApply->Enabled = false;
        actListCancel->Enabled = false;
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmBaseList::dstListBeforePost(TDataSet *DataSet)
{
    if (dstList->Fields->Fields[0]->IsNull) {
        int newId = dmMain->getNewId();
        if (newId > 0)
            dstList->Fields->Fields[0]->AsInteger = newId;
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmBaseList::dstListBeforeDelete(TDataSet *DataSet)
{
    if (Application->MessageBox("Удалить текущую запись?", Application->Title.c_str(), MB_ICONQUESTION | MB_YESNO) != IDYES)
        Abort();
}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseList::dstListAfterPost(TDataSet *DataSet)
{
    dstList->Transaction->CommitRetaining();
}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseList::dstListAfterDelete(TDataSet *DataSet)
{
    dstList->Transaction->CommitRetaining();
}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseList::FormKeyDown(TObject *Sender, WORD &Key,
      TShiftState Shift)
{
    if (panSearch->Visible)
        return;
    if (m_caller) {
        if (Key == 27 && Shift == TShiftState())
            actCloseExecute(this);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseList::edtIncSearchExit(TObject *Sender)
{
    panSearch->Visible = false;
}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseList::dgrListEditButtonClick(TObject *Sender)
{
    FormProvider.ShowList(dgrList->DataSource->DataSet->Fields->Fields[dgrList->SelectedField->Tag]->Tag,
                        this->Handle,
                        dgrList->DataSource->DataSet->Fields->Fields[dgrList->SelectedField->Tag]->AsInteger);
}
//---------------------------------------------------------------------------
void __fastcall TfrmBaseList::Initialize()
{
    //  открывает транзакцию, набор, устанавливает заданную запись текущей
    if (!dstList->Transaction->Active)
        dstList->Transaction->StartTransaction();
    else
        dstList->Transaction->CommitRetaining();
        
    dstList->Open();

    setCaption();
    //  В Tag указан номер списка
    if (!m_elementId)
        m_elementId = last_id[Tag];
    dstList->Locate(dstList->Fields->Fields[0]->FieldName, Variant(m_elementId), TLocateOptions());
}
//---------------------------------------------------------------------------
void __fastcall TfrmBaseList::dgrListDblClick(TObject *Sender)
{
    if (m_caller)
        this->actOkExecute(this);
    else
        this->actListEditExecute(this);
}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseList::dgrListKeyDown(TObject *Sender, WORD &Key,
      TShiftState Shift)
{
    if (m_caller) {
        if (Key == 13 && Shift == TShiftState())
            this->actOkExecute(this);
        if (Key == 27 && Shift == TShiftState())
            this->actCloseExecute(this);
    } else {
        if (Key == 13 && Shift == TShiftState())
            this->actListEditExecute(this);
    }
    //  сортировка
    if (Shift == (TShiftState() << ssCtrl) && Key >= '0' && Key <= '9') {
        //  перекинем "0" на "10"
        if (Key == '0')
            Key += 10;
        //  проверим количество полей
        if (dgrList->Columns->Count > Key - '1') {
            AnsiString orderByString = AnsiString("order by ")+dgrList->Columns->Items[Key - '1']->Field->Origin;
            //  Проблема - если один Ориджин несколько раз встречается, то сортировать будет по последнему
            //  можно использовать Индекс, но для этого нужно привести в соответствие порядок полей в ДатаСете к порядку в запросе
            setCaption();
            Caption = Caption + " - сортировка по " + dgrList->Columns->Items[Key - '1']->Title->Caption;
            bool isActive = dstList->Active;
            int oldId = dstList->Fields->Fields[0]->AsInteger;
            int oldParam;
            int dbSection;
            if (dstList->Params->Names.Pos("GRP_ID"))
                oldParam = dstList->ParamByName("GRP_ID")->AsInteger;
            if (dstList->Params->Names.Pos("DB_SECTION_ID")) 
                dbSection = dstList->ParamByName("DB_SECTION_ID")->AsInteger;
            dstList->DisableControls();
            dstList->Close();
            // найдём 'order by' в запросе
            int idx;
            for (idx = 0; idx < dstList->SelectSQL->Count; idx++)
                if (dstList->SelectSQL->Strings[idx].SubString(1, 8).LowerCase() == "order by") {
                    dstList->SelectSQL->Strings[idx]= orderByString;
                    break;
                }
            if (idx == dstList->SelectSQL->Count)
                dstList->SelectSQL->Add(orderByString);
            if (dstList->Params->Names.Pos("GRP_ID"))
                dstList->ParamByName("GRP_ID")->AsInteger = oldParam;
            if (dstList->Params->Names.Pos("DB_SECTION_ID"))
                dstList->ParamByName("DB_SECTION_ID")->AsInteger = dbSection;
            if (isActive) {
                dstList->Open();
                dstList->Locate(dstList->Fields->Fields[0]->FieldName, oldId, TLocateOptions());
            }
            dstList->EnableControls();
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseList::setCaption()
{
    if (m_caller) {
        Caption = panList->Caption + " (вибір)";
    } else {
        Caption = panList->Caption + " (довідник)";
    }
}
void __fastcall TfrmBaseList::dstListAfterClose(TDataSet *DataSet)
{
    dstList->Transaction->CommitRetaining();
}
//---------------------------------------------------------------------------


void __fastcall TfrmBaseList::edtIncSearchKeyDown(TObject *Sender,
      WORD &Key, TShiftState Shift)
{
    if (Key == 13 || Key == 27) {
        //panSearch->Visible = false;
        dgrList->SetFocus();
        Key = 0;
    }
}
//---------------------------------------------------------------------------


void __fastcall TfrmBaseList::edtIncSearchChange(TObject *Sender)
{
//   dstList->Locate(dgrList->SelectedField->FieldName, edtIncSearch->Text.c_str(), TLocateOptions() << loCaseInsensitive << loPartialKey);
//       AnsiString Filter = "";
//      TFilterOption filter_option;
//      filter_option <<
//      dstList->FilterOptions
//      Filter = dgrList->SelectedField->FieldName + " = ";
//          if (dgrList->SelectedField->DataType == ftString) Filter += "'";
//      Filter += edtIncSearch->Text;
//          if (dgrList->SelectedField->DataType == ftString) Filter += "*'";

    if (panSearch->Visible == true) {

        if (ListFilterField.size() > 0) {
            if (ListFilterField.back() == dgrList->SelectedField->FieldName) {
                ListFilterValue.back() = edtIncSearch->Text;
            } else {
                ListFilterField.push_back(dgrList->SelectedField->FieldName);
                ListFilterValue.push_back(edtIncSearch->Text);
            }
        } else {
            ListFilterField.push_back(dgrList->SelectedField->FieldName);
            ListFilterValue.push_back(edtIncSearch->Text);
        }

        AnsiString SearchParam = "";
        std::vector<AnsiString>::iterator val = ListFilterValue.begin();
        for(std::vector<AnsiString>::iterator field = ListFilterField.begin(); field != ListFilterField.end(); field++)
        {
            if (((*field == dgrList->SelectedField->FieldName)&&(field != ListFilterField.end()-1))||(*val == ""))
            {
                field--;
                val--;
                ListFilterField.erase(field+1);
                ListFilterValue.erase(val+1);
            } else {
                SearchParam += "(";
                SearchParam += (dstList->FieldByName(*field)->DisplayName + " = '*");
                SearchParam += (*val + "*') ");
            }
            val++;
        }
        lblSearchParam->Caption = SearchParam;

        int SelField = dgrList->SelectedIndex;

        //     if (!dynamic_cast<TfrmListTransmitters *>(this)){
        //     dstList->Filtered = false;
        //     dstList->Filtered = true;
        //     }
        Db::TDataSource* DataSource = dgrList->DataSource;
        dgrList->Enabled = false;
        dgrList->DataSource = NULL;
        dgrList->DataSource = DataSource;
        dgrList->Enabled = true;

        dgrList->SelectedIndex  = SelField;
    }
    actSearchCancel->Enabled = ListFilterField.size() != 0;
}
//---------------------------------------------------------------------------

__fastcall TfrmBaseList::changeQueryCopy()
{
    return true;
}
void __fastcall TfrmBaseList::dstListFilterRecord(TDataSet *DataSet,
      bool &Accept)
{
    int accept_count = 0;
    std::vector<AnsiString>::iterator val = ListFilterValue.begin();
    for(std::vector<AnsiString>::iterator field = ListFilterField.begin(); field != ListFilterField.end(); field++) {
        if (dstList->FieldByName(*field)->AsString.UpperCase().AnsiPos((*val).UpperCase()) > 0)
            accept_count++;
        val++;
    }

    Accept = (accept_count == ListFilterField.size());

    if (dynamic_cast<TfrmListTransmitters *>(this))
        if (((TfrmListTransmitters *)this)->chbOnlyRoot->Checked)
            if (dstList->Fields->Fields[19]->AsInteger) {
                Accept = false;
                return;
            }
}
//---------------------------------------------------------------------------


void __fastcall TfrmBaseList::actSearchCancelExecute(TObject *Sender)
{
    ListFilterValue.clear();
    ListFilterField.clear();
    Db::TDataSource* DataSource = dgrList->DataSource;
    dgrList->Enabled = false;
    dgrList->DataSource = NULL;
    dgrList->DataSource = DataSource;
    dgrList->Enabled = true;
    dgrList->SetFocus();
}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseList::NewElement()
{
    dstList->Insert();
}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseList::actRefreshExecute(TObject *Sender)
{
    TField *idFld = dstList->FindField("ID");
    if (idFld)
        m_elementId = idFld->AsInteger;
    dstList->Close();
    dstList->Transaction->CommitRetaining();

    Initialize();
}
//---------------------------------------------------------------------------

