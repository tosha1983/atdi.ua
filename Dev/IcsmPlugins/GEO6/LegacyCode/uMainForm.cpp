
//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uLayoutMngr.h"
#include "uMainForm.h"
#include "uConnect.hpp"
#include "uMainDm.h"
#include "uAbout.hpp"
#include "FormProvider.h"
#include "TxBroker.h"
#include "uNewTxWizard.h"
#include "uExplorer.h"
#include "uCalcParamsDlg.h"
#include "uListStand.h"
#include "uListTransmitters.h"
#include "uParams.h"
#include "uSearch.h"
#include <memory>
#include "uAnalyzer.h"
#include "uBcAutoUpgrade.h"
#include "uItuImport.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TfrmMain *frmMain;

const double CURRENT_VERSION = 20111021.1332;
extern ILisBcStoragePtr storage;

String captionAnl("Aналог");
String captionDig("Цифра");
//---------------------------------------------------------------------------
__fastcall TfrmMain::TfrmMain(TComponent* Owner)
    : TForm(Owner)
{
    //Application->OnHint
    Caption = Application->Title;

    Screen->Cursors[crGetE] = LoadCursor(HInstance, "CR_GET_E");
    Screen->Cursors[crGetTx] = LoadCursor(HInstance, "CR_SEL_TX");
    Screen->Cursors[crGetRelief] = LoadCursor(HInstance, "CR_GET_RELIEF");

    try {
        BCCalcParams.load();
    } catch (Exception& E) {
        E.Message = AnsiString("Помилка при завантаженні параметрів\n") + E.Message;
        Application->ShowException(&E);
    }
    DisplayCalcParams();

    int dbMenuPos = 5;

    //AddTxListMenu(miTxDb, 0, dbMenuPos++);
    AddMenu(miTxDb, "-", NULL, dbMenuPos++);
    AddMenu(miTxDb, "Пошук передавачiв...", actSearch, otTx, dbMenuPos++);
    AddMenu(miTxDb, "Пошук опор...", actSearch, otSITES, dbMenuPos++);

    AddListMenu(miDocs, otDocument);
    AddListMenu(miDocs, otDocTemplate);

    AddMenu(miRrc06, "Iмпорт контурiв з файлiв GA1...", actImpRrc06, TfrmRrc06Import::imGa1);
    AddMenu(miRrc06, "Iмпорт видiлень з файлiв GS2/GT2...", actImpRrc06, TfrmRrc06Import::imGs2Gt2);
    AddMenu(miRrc06, "Iмпорт присвоень з файлiв GS1/GT1/G02/GB1...", actImpRrc06, TfrmRrc06Import::imGs1Gt1);

    #ifndef _DEBUG
    actMemInfo->Visible = false;
    miSep->Visible = false;
    actUnloadAll->Visible = false;
    actUnloadAllForced->Visible = false;
    #endif
}
//---------------------------------------------------------------------------
void __fastcall TfrmMain::actAboutExecute(TObject *Sender)
{
    if (!dlgAboutBox)
        dlgAboutBox = new TdlgAboutBox(Application);
    dlgAboutBox->ShowModal();
}
//---------------------------------------------------------------------------
void __fastcall TfrmMain::actListExecute(TObject *Sender)
{
    FormProvider.ShowList(((TComponent*)Sender)->Tag, 0, 0);
}
//---------------------------------------------------------------------------
void __fastcall TfrmMain::actNewTxExecute(TObject *Sender)
{
    std::auto_ptr<TfrmNewTxWizard> tw(new TfrmNewTxWizard(Application));
    if (tw->ShowModal() == mrOk) {
        //TCOMILISBCTx tx(txBroker.GetTx(tw->new_id), true);
        //FormProvider.ShowTx(tx);
    }
}
//---------------------------------------------------------------------------
HANDLE __fastcall TfrmMain::showExplorer()
{
    frmExplorer = FormProvider.ShowExplorer();
    return frmExplorer->Handle;
}
//---------------------------------------------------------------------------
void __fastcall TfrmMain::FormShow(TObject *Sender)
{
    static bool start = true;

    if (start) {

        TCOMILisBcObjectServerVersion versObj;
        try {
            OleCheck(versObj.CreateInstance(CLSID_LisBcObjectServerVersion));
            double version = 0.0;
            OleCheck(versObj.GetVersion(&version));
            if ((double)BC_TX_SERVER_VERSION > version)
                throw *(new Exception(AnsiString().sprintf("Исполняемый модуль предполагает использование "
                                "сервера старшей версии. \nСервер: %f, Модуль: %f. \n\n"
                                "Обновите сервер.", version, (double)BC_TX_SERVER_VERSION)));
            if ((double)BC_TX_SERVER_VERSION < version)
                if (Application->MessageBox(AnsiString().sprintf(
                                "Версия сервера старше, чем версия исполняемого модуля. \n"
                                "Сервер: %f, Модуль: %f.\n"
                                "Продолжить выполнение программы?", version, (double)BC_TX_SERVER_VERSION).c_str(),
                        "Проверка версии сервера передатчиков",
                        MB_ICONEXCLAMATION | MB_YESNO) == IDNO)
                    throw *(new Exception("Обновите исполняемый модуль"));

        } catch (Exception &e) {
            Application->MessageBox((AnsiString("Проверка версии сервера передатчиков: \n\n") + e.Message).c_str(),
                                    "Ошибка", MB_ICONHAND);
            Application->ShowMainForm = False;
            Application->Terminate();
            return;
        }

        try {
            try {
                BCCalcParams.CheckCalcServVersion();
            } catch (EUpgradeHost &e) {
                /*
                if (Application->MessageBox((e.Message+"\n\nПродолжить выполнение программы?").c_str(),
                                            "Проверка версии сервера расчёта",
                                            MB_ICONEXCLAMATION | MB_YESNO) == IDNO)
                    throw *(new Exception("Обновите исполняемый модуль"));
                */
            }
        } catch (Exception &e) {
            if (Application->MessageBox((AnsiString("Проверка версии сервера расчёта: \n\n") + e.Message+
                                    "\n\nНеобходимо обновить библиотеку. Продолжить выполнение?").c_str(),
                                    "Ошибка", MB_ICONHAND | MB_YESNO) == IDNO)
            {
                Application->ShowMainForm = False;
                Application->Terminate();
                return;
            }
        }

        std::auto_ptr<TdlgConnect> dlgConnect(new TdlgConnect((TComponent*)NULL));
        dlgConnect->Init(sAppRegPath, dmMain->dbMain);
        while (!dmMain->dbMain->Connected)
            if (dlgConnect->ShowModal() == mrCancel) {
                Application->ShowMainForm = False;
                Application->Terminate();
                return;
            }

        BcAutoUpgrade upgrade(dmMain->dbMain, CURRENT_VERSION);
        int res = upgrade.CheckAndUpgrade();
        if (res == urNewerDb && MessageBox(NULL, "Рекомендуется обновить программу.\nПродолжить работу?",
                                                "Старое ПО на новой БД", MB_ICONQUESTION | MB_YESNO) == IDYES)
            dmMain->dbMain->Connected = true;
        else if (res == urError || dmMain->dbMain->Connected == false)
        {
            Application->ShowMainForm = false;
            Application->Terminate();
            return;
        }
        if (!dmMain->trMain->Active)
            dmMain->trMain->StartTransaction();
        StatusBar1->Panels->Items[0]->Text = dlgConnect->edtName->Text;
        StatusBar1->Panels->Items[1]->Text = dlgConnect->StatusBar1->SimpleText;

        dmMain->InitDbControls();

        std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
        sql->Database = dmMain->dbMain;
        sql->SQL->Text = "select ENUMVAL, IS_USED from SYSTEMCAST";
        typedef std::set<int> IndexSet;
        IndexSet scIds;
        for (sql->ExecQuery(); !sql->Eof; sql->Next())
            if (sql->Fields[1]->AsInteger)
                scIds.insert(sql->Fields[0]->AsInteger);

        int dbMenuPos = 0;
        sql->Close();
        sql->SQL->Text = "select ID, SECTIONNAME from DATABASESECTION ";
        if (0)
            sql->SQL->Add("where ID < 100");
        sql->SQL->Add(" order by ID ");
        for (sql->ExecQuery(); !sql->Eof; sql->Next())
        {
            TMenuItem *mi = new TMenuItem(this);
            int dbSect = sql->Fields[0]->AsInteger;
            mi->Caption = sql->Fields[1]->AsString;
            mi->Tag = dbSect;
            miTxDb->Add(mi);
            mi->MenuIndex = dbMenuPos++;
            for (IndexSet::iterator itt = scIds.begin(); itt != scIds.end(); itt++)
                AddTxListMenu(mi, *itt);
            if (scIds.size() > 1)
                AddTxListMenu(mi, 0);
        }

        start = false;
    }

    try {
        LayoutManager.loadLayout(this);
    } catch (...) {
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmMain::actExplorerExecute(TObject *Sender)
{
    showExplorer();
    actExplorer->Checked = true;
}
//---------------------------------------------------------------------------
void __fastcall TfrmMain::actParamsExecute(TObject *Sender)
{
    if (!dlgCalcParams)
        dlgCalcParams = new TdlgCalcParams(Application);
    dlgCalcParams->ShowModal();
    DisplayCalcParams();
    FormProvider.UpdateViews();
}
//---------------------------------------------------------------------------

void __fastcall TfrmMain::FormListTxRefresh()
{
 for (int i = 0; i < MDIChildCount; i++) {
    if (dynamic_cast<TfrmListTransmitters *>(MDIChildren[i])) {
          TfrmListTransmitters *frmTxList = (TfrmListTransmitters *)MDIChildren[i];
          int id = frmTxList->dstList->FieldByName("TX_ID")->AsInteger;
          frmTxList->dsrList->Enabled = false;
          frmTxList->dstList->Close();
          frmTxList->dstList->Open();
          Set<TLocateOption,0,1> flags;
          frmTxList->dstList->Locate("TX_ID", AnsiString(id), flags);
          frmTxList->dsrList->Enabled = true;
    }
 }
}

//---------------------------------------------------------------------------

void __fastcall TfrmMain::FormListStandRefresh()
{
    for (int i = 0; i < MDIChildCount; i++)
        if (dynamic_cast<TfrmListStand *>(MDIChildren[i]))
        {
            TfrmListStand *form = (TfrmListStand *)MDIChildren[i];
            int id = form->dstList->FieldByName("ID")->AsInteger;
            form->dsrList->Enabled = false;
            form->dstList->Close();
            form->dstList->Open();
            Set<TLocateOption,0,1> flags;
            form->dstList->Locate("ID", AnsiString(id), flags);
            form->dsrList->Enabled = true;
        }
}
//---------------------------------------------------------------------------

void __fastcall TfrmMain::actSearchExecute(TObject *Sender)
{
    FormProvider.ShowSearch(((TComponent*)Sender)->Tag);
}
//---------------------------------------------------------------------------

void __fastcall TfrmMain::FormDestroy(TObject *Sender)
{
    //
}
//---------------------------------------------------------------------------

void __fastcall TfrmMain::mniFormsClick(TObject *Sender)
{
    //
}
//---------------------------------------------------------------------------

void __fastcall TfrmMain::mniChildWindowClick(TObject *Sender)
{
    TComponent* comp = dynamic_cast<TComponent*>(Sender);
    for (int i = 0; i < MDIChildCount; i++) {
        if (MDIChildren[i]->Handle == (HANDLE)comp->Tag) {
            MDIChildren[i]->Show();
            if (MDIChildren[i]->WindowState == wsMinimized)
                MDIChildren[i]->WindowState = wsNormal;
            break;
        }
    }
    TMenuItem* mni = dynamic_cast<TMenuItem*>(Sender);
    if (mni)
        mni->Checked = true;
    TToolButton* tbt = dynamic_cast<TToolButton*>(Sender);
    if (tbt)
        tbt->Down = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmMain::actShowPlanningExecute(TObject *Sender)
{
    FormProvider.ShowPlanning();    
}
//---------------------------------------------------------------------------

void __fastcall TfrmMain::actDocumentsExecute(TObject *Sender)
{
    throw new Exception("Documents out of date (where  ANSWERDATE < :DATE)");
}
//---------------------------------------------------------------------------


void __fastcall TfrmMain::DockPanelDockDrop(TObject *Sender, TDragDockObject *Source, int X, int Y)
{
    TPanel* SenderPanel = dynamic_cast<TPanel*>(Sender);
    if (SenderPanel == NULL)
        throw EInvalidCast("");

    if (SenderPanel == pnlLeftDockPanel)
        leftSplitter->Show();
    else
        rightSplitter->Show();

    //SenderPanel->Width = reinterpret_cast<TfrmExplorer*>(Source)->width;
    SenderPanel->Width = dynamic_cast<TfrmExplorer*>(Source->Control)->width;

    if (SenderPanel == pnlLeftDockPanel)
        leftSplitter->Left = SenderPanel->Width + leftSplitter->Width;
    else
        rightSplitter->Left = this->ClientWidth - SenderPanel->Width;

    SenderPanel->DockManager->ResetBounds(true);// Make DockManager repaints it's clients.
}
//---------------------------------------------------------------------------

void __fastcall TfrmMain::DockPanelDockOver(TObject *Sender, TDragDockObject *Source, int X, int Y, TDragState State, bool &Accept)
{
  TForm * dockForm = dynamic_cast<TForm*>(Source->Control);
  Accept = (dynamic_cast<TfrmExplorer*>(Source->Control) != NULL);
  if (Accept)
  {
    // Modify the DockRect to preview dock area.
    if ( Sender == pnlLeftDockPanel )
    {
      Types::TPoint TopLeft = pnlLeftDockPanel->ClientToScreen(TPoint(0, 0));
      Types::TPoint BottomRight = pnlLeftDockPanel->ClientToScreen(TPoint(dockForm->Width, pnlLeftDockPanel->Height));
      Source->DockRect = Types::TRect(TopLeft, BottomRight);
    }
    else
    {
      Types::TPoint TopLeft = pnlRightDockPanel->ClientToScreen(TPoint(0 - dockForm->Width, 0));
      Types::TPoint BottomRight = pnlRightDockPanel->ClientToScreen(TPoint(0, pnlRightDockPanel->Height));
      Source->DockRect = Types::TRect(TopLeft, BottomRight);
    }
  }
}
//---------------------------------------------------------------------------

void __fastcall TfrmMain::DockPanelGetSiteInfo(TObject *Sender, TControl *DockClient, TRect &InfluenceRect, TPoint &MousePos, bool &CanDock)
{
  // If CanDock is true, the panel will not automatically draw the preview rect.
  CanDock = (dynamic_cast<TfrmExplorer*>(DockClient) != NULL);
}
//---------------------------------------------------------------------------

void __fastcall TfrmMain::DockPanelUnDock(TObject *Sender, TControl *Client, TWinControl *NewTarget, bool &Allow)
{
  TPanel* SenderPanel = dynamic_cast<TPanel*>(Sender);
  if (SenderPanel == NULL)
    throw EInvalidCast("");

  if (SenderPanel == pnlLeftDockPanel)
    leftSplitter->Hide();
  else
    rightSplitter->Hide();

  SenderPanel->Width = 0;
}
//---------------------------------------------------------------------------

void __fastcall TfrmMain::FormClose(TObject *Sender, TCloseAction &Action)
{
    txAnalyzer.Shutdown();
    try {
        LayoutManager.saveLayout(frmMain);
        if ( ( frmMain->actExplorer->Checked ) && ( frmExplorer != NULL ) )
            frmExplorer->Close();
    } catch (...) {
    }

    for (int i = 0; i < frmMain->MDIChildCount; i++)
        delete frmMain->MDIChildren[i];

    #ifndef _DEBUG
    txBroker.UnloadAll(true);
    #else
    txBroker.UnloadAll();
    if (txBroker.GetCount() == 0 || MessageBox(NULL, "Tx objects left in memory.\n"
                                    "Continue?", "DEBUG WARNING", MB_YESNO | MB_ICONEXCLAMATION) == IDYES)
    #endif
    if (storage.IsBound())
    {
        storage->AddRef();
        while (storage->Release() > 1)
            ;
        storage.Unbind();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmMain::FormCreate(TObject *Sender)
{
    pb->Parent = StatusBar1;
    pb->ParentWindow = StatusBar1;
    pb->Top = 2;
    pb->Height = frmMain->StatusBar1->ClientHeight - 4;
    pb->Left = 300;
    pb->Width = StatusBar1->ClientWidth - pb->Left;
}
//---------------------------------------------------------------------------
void __fastcall TfrmMain::FormCloseQuery(TObject *Sender, bool &CanClose)
{
    if ( BCCalcParams.QueryOnMainormClose )
        if ( Application->MessageBoxA("Завершити роботу з програмою?", "Вихід", MB_YESNO | MB_ICONEXCLAMATION) == IDYES )
            CanClose = true;
        else
            CanClose = false;
}
//---------------------------------------------------------------------------
void __fastcall TfrmMain::StatusBar1Resize(TObject *Sender)
{
    pb->Width = StatusBar1->ClientWidth - pb->Left;
}
//---------------------------------------------------------------------------

void __fastcall TfrmMain::actImpRrc06Execute(TObject *Sender)
{
    TComponent* cmpnt = dynamic_cast<TComponent*>(Sender);
    if (cmpnt)
        dmMain->ImportRrc06(cmpnt->Tag);
}
//---------------------------------------------------------------------------

void __fastcall TfrmMain::DisplayCalcParams()
{
    AnsiString as;

    if (BCCalcParams.UseHeff)
        as = as + "Ев:";
    if (BCCalcParams.UseTxClearence)
        as = as + "Пд:";
    if (BCCalcParams.UseRxClearence)
        as = as + "Пм:";
    if (BCCalcParams.UseMorfology)
        as = as + "Гдр:";
    as = as + FormatFloat("0.###", BCCalcParams.Step)+"км";

    StatusBar1->Panels->Items[2]->Text = BCCalcParams.CalcServerName;
    if ( BCCalcParams.calcMethod == 0 )
        StatusBar1->Panels->Items[2]->Text += " (Сум)";
    else if ( BCCalcParams.calcMethod == 1 )
        StatusBar1->Panels->Items[2]->Text += " (Множ)";
    else if ( BCCalcParams.calcMethod == 2 )
        StatusBar1->Panels->Items[2]->Text += " (Ч97)";

    StatusBar1->Panels->Items[3]->Text = BCCalcParams.PropagServerName;
    StatusBar1->Panels->Items[4]->Text = BCCalcParams.ReliefServerName;
    StatusBar1->Panels->Items[5]->Text = as;
}

void __fastcall TfrmMain::MemoryInfoDblClick(TObject *Sender)
{
    TMemo *m = dynamic_cast<TMemo*>(Sender);
    if (m)
    {
        int pos = m->SelStart;
        String text = m->Lines->Text;
        while (pos >= 0 && text[pos+1] != '#')
            pos --;
        if (pos > -1)
        {
            text = text.SubString(pos+2, text.Length() - pos + 2);
            for (pos = 2; pos < text.Length() && isdigit(text[pos]); pos++)
                ;
            text.SetLength(pos-1);
            int txId = 0;
            try { txId = text.ToInt(); } catch(...) {}
            if (txId > 0)
                FormProvider.ShowTx(txBroker.GetTx(txId, dmMain->GetObjClsid(dmMain->GetSc(txId))));
        }
    }
}
//---------------------------------------------------------------------------


void __fastcall TfrmMain::actMemInfoExecute(TObject *Sender)
{
    String info("storage: ");
    if (!storage.IsBound())
        info += "not allocated\n";
    else
    {
        storage->AddRef();
        info += (IntToStr(storage->Release()) + '\n');
    }
    //ShowMessage(info + txBroker.GetInfo());

    std::auto_ptr<TForm> f (new TForm(Application));
    f->Width = 400;
    f->Height = Screen->Height / 5 * 4;
    f->BorderStyle = bsSizeToolWin;
    f->Position = poMainFormCenter;
    f->Caption = "Memory Info";
    TMemo *m = new TMemo(f.get());
    m->Parent = f.get();
    m->Align = alClient;
    //m->Font->Name = "Tahoma";
    m->Font->Name = "Courier New";
    m->ParentColor = true;
    m->ReadOnly = true;
    m->OnDblClick = MemoryInfoDblClick;
    m->Lines->Text = info + txBroker.GetInfo();
    f->ShowModal();
}
//---------------------------------------------------------------------------

void __fastcall TfrmMain::actUnloadAllExecute(TObject *Sender)
{
    txAnalyzer.Clear();
    txBroker.UnloadAll(Sender == miUnloadAllForced || Sender == actUnloadAllForced);
    if (storage.IsBound())
    {
        storage->AddRef();
        if (storage->Release() == 1)
            storage.Unbind();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmMain::actUnloadAllForcedExecute(TObject *Sender)
{
    if (MDIChildCount > 0)
        throw *(new Exception("Закройте все окна перед форсированной выгрузкой"));
    //if (MessageBox(NULL, "При форсированной выгрузке все окна (планирование, экспертизы, карточки и др.)\n "
    //                     "должны быть закрыты. Выгрузить передатчики?s")
    actUnloadAllExecute(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TfrmMain::actUnloadAllForcedUpdate(TObject *Sender)
{
    #ifndef _DEBUG
    if (MDIChildCount == 0)
        actUnloadAllForcedExecute(Sender);
    #endif
}
//---------------------------------------------------------------------------

TMenuItem* __fastcall TfrmMain::AddMenu(TMenuItem* parent, String caption, TAction* action, int tag, int index)
{
    TMenuItem *mni = new TMenuItem(this);
    parent->Add(mni);
    //mni->Action = action;
    if (action)
        mni->OnClick = action->OnExecute;
    mni->Tag = tag;
    mni->Caption = caption;
    if (index > -1)
        mni->MenuIndex = index;
    return mni;
}

TMenuItem* __fastcall TfrmMain::AddListMenu(TMenuItem* parent, int tag, int index)
{
    return AddMenu(parent, FormProvider.GetListName(tag), actList, tag, index);
}

__fastcall TfrmMain::AddTxListMenu(TMenuItem* parent, int sc, int index)
{
    String mcaption = FormProvider.GetListName(otTx);
    if (sc)
        mcaption = mcaption + ' '+dmMain->GetSystemCastName(sc)+' ';
    else
        mcaption = mcaption + " (всi)";
    if (sc == ttAM)
    {
        AddMenu(parent, mcaption + ' ' + captionAnl, actTxList, sc, index);
        AddMenu(parent, mcaption + ' ' + captionDig, actTxList, sc, index);
    }
    else
    {
        AddMenu(parent, mcaption, actTxList, sc, index);
    }
}

void __fastcall TfrmMain::actTxListExecute(TObject *Sender)
{
    String typeFilter;
    String typeCaption;
    TMenuItem *mi = dynamic_cast<TMenuItem*>(Sender);
    int dbSect = 0;
    if (mi && mi->Parent != miTxDb)
        dbSect = mi->Parent->Tag;

    if (mi && mi->Tag)
    {
        typeFilter = String().sprintf("SYSTEMCAST.ENUMVAL = %d", mi->Tag);
        typeCaption = dmMain->GetSystemCastName(mi->Tag);
        if (mi->Tag == ttAM)
        {
            if (mi->Caption.Pos(captionAnl) > 0)
            {
                typeFilter += " and (TRANSMITTERS.TYPESYSTEM is null or TRANSMITTERS.TYPESYSTEM = 1)";
                typeCaption = typeCaption +  ' ' + captionAnl;
            }
            else if (mi->Caption.Pos(captionDig) > 0)
            {
                typeFilter += " and TRANSMITTERS.TYPESYSTEM <> 1";
                typeCaption = typeCaption +  ' ' + captionDig;
            }
        }
    }
        
    FormProvider.ShowList(otTx, 0, 0, typeFilter, typeCaption, dbSect);
}
//---------------------------------------------------------------------------

