//---------------------------------------------------------------------------
//  ������������ �������������� ���������� ����
//  ��� ��������� ���� �������
//
#pragma hdrstop

#include <alloc.h>
#include <mem.h>

#include "uMainDm.h"

#include "FormProvider.h"
#include <LISBCTxServer_TLB.h>
#include "uMainForm.h"
#include "uBaseList.h"
#include "uListAccountCondition.h"
#include "uListAnalogRadioSystem.h"
#include "uListAnalogTeleSystem.h"
#include "uListArea.h"
#include "uListCarrierGuardInterval.h"
#include "uListChannel.h"
#include "uListCity.h"
#include "uListCityModal.h"
#include "uListCountry.h"
#include "uListDigitalSystem.h"
#include "uListDistrict.h"
#include "uListEquipment.h"
#include "uListMinFieldStrength.h"
#include "uListOffsetCarryFreqTVA.h"
#include "uListRadioService.h"
#include "uListStreet.h"
#include "uListSystemCast.h"
#include "uListTypeReceive.h"
#include "uListUncompatibleChannels.h"
#include "uListStand.h"
#include "uListBank.h"
#include "uListOwner.h"
#include "uListTelecomOrganization.h"
#include "uFrmTxVHF.h"
#include "uFrmTxDAB.h"
#include "uFrmTxDVB.h"
#include "uFrmTxTVA.h"
#include "uFrmTxCTV.h"
#include "uListBlockDAB.h"
#include "uListTransmitters.h"
#include "uListTPOnBorder.h"
#include "uListLicense.h"
#include "uListSynhroNet.h"
#include "uListTypeSFN.h"
#include "uExplorer.h"
#include "uUserActivityLog.h"
#include "uSelection.h"
#include "uSearch.h"
#include "uListFrequencyGrid.h"
#include "uPlanning.h"
#include "uParams.h"
#include "uListDigAllotments.h"
#include "uAllotmentForm.h"
#include "uListSubareas.h"
#include "DlgSelectTypeDoc.h"

#include "uBaseObjForm.h"
#include "uOtherTerrSrvc.h"
#include "uFrmTxLfMf.h"

#include "tempvalues.h"
#include "uLisObjectGridForm.h"
#include "uFrmDocumentsSettings.h"
#include "uFrmTxFxm.h"

//---------------------------------------------------------------------------
#pragma package(smart_init)

TFormProvider FormProvider;
std::map<ObjType, String> objName;
std::map<ObjType, String> lstName;

//---------------------------------------------------------------------------
TForm* __fastcall TFormProvider::ShowList(ObjType dictId, HWND caller, int elementId,
                                          String extraCond, String extraCaption, int extraTag)
{
    if (!dictId)
        return NULL;

    TfrmBaseList* bl = NULL;
    //  ����� ����� � ������ dictId � caller ����� MDIChildCount
    for (int i = 0; i < frmMain->MDIChildCount; i++) {
        bl = (TfrmBaseList *)frmMain->MDIChildren[i];
        if (dictId == frmMain->MDIChildren[i]->Tag && caller == ((TfrmBaseList *)frmMain->MDIChildren[i])->Caller) {
            frmMain->MDIChildren[i]->Show();
            break;
        } else {
            bl = NULL;
        }
    }

    if (!bl) {
        //  ���� ���, �� �������
        TempCursor curs(crHourGlass);

        switch (dictId) {
            case otACCOUNT_STATES:
                bl = new TfrmListAccountCondition(Application, caller, elementId);
                break;
            case 3: bl = new TfrmListAnalogRadioSystem(Application, caller, elementId);
                //'������� ����������� ������������'
                break;
            case 4: bl = new TfrmListAnalogTeleSystem(Application, caller, elementId);
                //'������� ����������� ��'
                break;
            case 5: bl = new TfrmListArea(Application, caller, elementId);
                //'&������'
                break;
            case 6: bl = new TfrmListBank(Application, caller, elementId);
                //'&�����'
                break;
            case otBLOCK_DAB: bl = new TfrmListBlockDAB(Application, caller, elementId);
                //'&�����'
                break;
            case 8: bl = new TfrmListCarrierGuardInterval(Application, caller, elementId);
                //'������ �� ������� ��������� ��������� ��'
                break;
            case 10: bl = new TfrmListChannel(Application, caller, elementId);
                //'������ ����������� �����������'
                break;
            case 11: bl = new TfrmListCity(Application, caller, elementId);
                //'&������� ������'
                break;
            case 13: bl = new TfrmListTPOnBorder(Application, caller, elementId);
                //'&�����'
                break;
            case 14: bl = new TfrmListCountry(Application, caller, elementId);
                //'&�����'
                break;
            case 17: bl = new TfrmListDigitalSystem(Application, caller, elementId);
                //'������� ��������� �����������'
                break;
            case 18: bl = new TfrmListDistrict(Application, caller, elementId);
                //'������'
                break;
            case otEQUIP:
                bl = new TfrmListEquipment(Application, caller, elementId);
                break;
            case otLICENSES:
                bl = new TfrmListLicense(Application, caller, elementId);
                break;
            case 23: bl = new TfrmListMinFieldStrength(Application, caller, elementId);
                //'̳������ ������������ ����'
                break;
            case 24: bl = new TfrmListOffsetCarryFreqTVA(Application, caller, elementId);
                //'���� ��� ����������� ��'
                break;
            case otOWNER:
                bl = new TfrmListOwner(Application, caller, elementId);
                //'��� / ���������'
                break;
            case 30: bl = new TfrmListRadioService(Application, caller, elementId);
                //'����������'
                break;
            case otSITES:
                bl = new TfrmListStand(Application, caller, elementId);
                //'&�����'
                break;
            case 34: bl = new TfrmListStreet(Application, caller, elementId);
                //'&������'
                break;
            case 35: bl = new TfrmListSystemCast(Application, caller, elementId);
                //'������� ��������'
                break;
            case otORGANIZATION:
                bl = new TfrmListTelecomOrganization(Application, caller, elementId);
                //'����������'
                break;
            case 40: bl = new TfrmListTypeReceive(Application, caller, elementId);
                //'���� �������'
                break;
            case 41: bl = new TfrmListUncompatibleChannels(Application, caller, elementId);
                //'������� ������ ���������� ��'
                break;
            case 42: frmListCityModal = new TfrmListCityModal(Application, caller, elementId);
                //��������� ������� ��������� ������
                //frmListCityModal->ShowModal();
                //frmListCityModal->Free();
                //frmListCityModal = NULL;
                break;
            case 44: bl = new TfrmListTypeSFN(Application, caller, elementId);
                //  type of synchro net
                break;
            case otSFN:
                bl = new TfrmListSynhroNet(Application, caller, elementId);
                break;
            case 46: bl = new TfrmUserActivityLog(Application, caller, elementId);
                //  user activity log
                break;
            case 47: bl = new TfrmListFrequencyGrid(Application, caller, elementId);
                //  frequency grids
                break;
            case otDIG_ALLOTMENT:
                bl = new TfrmListDigAllotments(Application, caller, elementId);
                break;
            case otDIG_SUBAREAS:
                bl = new TfrmListDigSubareas(Application, caller, elementId);
                break;
            default: // ��� ��������
                break;
        }
    }

    TForm* frm = bl;
    //��������� ���������� ���������
    if (bl)
    {
        if (extraCond.Length() > 0)
        {
            bl->dstList->Close();
            if (bl->dstList->SelectSQL->Text.LowerCase().Pos("where") > 0)
                bl->dstList->SelectSQL->Add(" and " + extraCond);
            else
                bl->dstList->SelectSQL->Add(" where " + extraCond);
            bl->dstList->Open();
        }
    }
    else
    {
        TLisObjectGridForm* lf = NULL;
        String caption;
        for (int i = 0; i < frmMain->MDIChildCount; i++) {
            lf = dynamic_cast<TLisObjectGridForm *>(frmMain->MDIChildren[i]);
            if (lf && (lf->Caption == caption))
            {
                lf->Show();
                break;
            }
            else
                lf = NULL;
        }
        if (!lf)
        {
            String listQry = dmMain->GetObjQuery(dictId);
            if (!listQry.IsEmpty())
            {
                if (extraCond.Length() > 0)
                {
                    int wpos = DelimitedPos(listQry, "where");
                    int opos = DelimitedPos(listQry, "order");
                    String order;
                    if (opos && opos > wpos)
                    {
                        order = listQry.SubString(opos, listQry.Length() - opos + 1);
                        listQry.Delete(opos, listQry.Length() - opos + 1);
                    }
                    if (wpos)
                        listQry += (String(" and (")+extraCond+") " + order);
                    else
                        listQry += (String(" where ")+extraCond + ' ' + order);
                }
                // set caption
                int no = 1;
                String cpt = RemoveAmpersand(GetListName(dictId));
                if (dictId == otTx)
                {
                    // TODO: reflect tx systemcas in caption
                    if (extraCaption.Length())
                        cpt = cpt + " (" + extraCaption + ')';
                    else
                        cpt = cpt + " (��i)";
                }
                while (caption.IsEmpty())
                {
                    caption = cpt + ' ' + IntToStr(no++);
                    for (int i = 0; i < frmMain->MDIChildCount; i++)
                        if (frmMain->MDIChildren[i]->Caption == caption)
                            caption = "";

                }
                lf = new TLisObjectGridForm(Application);
                lf->Caption = caption;
                lf->grid->SetQuery(listQry);
                int syscast = 100;
                String marker = "SYSTEMCAST.ENUMVAL";
                int pos=0;
                pos = extraCond.Pos(marker);
                if(pos != 0)
                {
                    pos += 18;
                    while(!isdigit(extraCond[pos])&& pos < extraCond.Length()) ++pos;
                    syscast = 0;
                    char *buf = NULL;
                    while(pos <= extraCond.Length() && isdigit(extraCond[pos]))
                    {
                        syscast *= 10;
                        buf = &(extraCond[pos]);
                        syscast += atoi(buf);
                        ++pos;
                    }
                }
                lf->Init(dictId, elementId, extraTag, syscast);
                lf->Show();
            }
        }
        frm = lf;
    }

    if (frm)
    {
        static offset = 0;
        if (offset > frmMain->ClientHeight / 2)
            offset = 0;
        frm->Left = offset;
        frm->Top = offset;
        offset += (GetSystemMetrics(SM_CYCAPTION)); //GetSystemMetrics(SM_CYBORDER) +
        if (frm->Width > frmMain->rightSplitter->Left - frmMain->leftSplitter->Left - frm->Left)
            frm->Width = frmMain->rightSplitter->Left - frmMain->leftSplitter->Left - frm->Left - 7;
    }

    return frm;
}
//---------------------------------------------------------------------------
TForm* __fastcall TFormProvider::ShowTx(Lisbctxserver_tlb::ILISBCTx* iTx)
{
    #ifdef _DEBUG
        //ShowMessage(__FUNC__"(): "+IntToStr(__LINE__));
    #endif
    if (!iTx)
        return NULL;

    TCOMILISBCTx tx(iTx, true);

    TfrmTxBase *tf = NULL;
    TfrmAllotment *af = NULL;
    TForm *f = NULL;
    //  ����� ����� � ������ iTx ����� MDIChildCount
    for (int i = 0; i < frmMain->MDIChildCount; i++)
        if ((f = tf = dynamic_cast<TfrmTxBase*>(frmMain->MDIChildren[i])) && (iTx == tf->GetTx())
            ||
            (f = af = dynamic_cast<TfrmAllotment*>(frmMain->MDIChildren[i])) && (iTx == af->GetTx()))
        {
            f->Show();
            return f;
        }

    //  ���� ���, �� �������
    {
        TempCursor curs(crHourGlass);

        Lisbctxserver_tlb::TBCTxType systemcast = tx.systemcast;
        switch (systemcast) {
            case -1: f = new TfrmTxFxm(Application, iTx);
                break;
            case ttTV: f = new TfrmTxTVA(Application, iTx);
                break;
            case ttFM: f = new TfrmTxVHF(Application, iTx);
                break;
            case ttAM: f = new TfrmTxLfMf(Application, iTx);
                break;
            case ttDVB: f = new TfrmTxDVB(Application, iTx);
                break;
            case ttDAB: f = new TfrmTxDAB(Application, iTx);
                break;
            case ttCTV: f = new TfrmTxCTV(Application, iTx);
                break;
            case ttAllot: f = new TfrmAllotment(Application, iTx);
                break;
            default:
                throw *(new Exception("����������� ��� ����������� ("+IntToStr(systemcast)+')'));
                break;

        }
        if (f) {
            //  ��������������� ��������� ��������
            int clientWidth = frmMain->ClientWidth
                        - frmMain->leftSplitter->Width
                        - frmMain->rightSplitter->Width
                        - frmMain->pnlLeftDockPanel->Width
                        - frmMain->pnlRightDockPanel->Width;

            if (f->Width < clientWidth) {
                //  ������ ���� ��� � �������� ����������
                if (f->Left + f->Width > clientWidth)
                    f->Left = clientWidth - f->Width;
                if (f->Left < 0)
                    f->Left = 0;
            }
        }
    }

    return f;
}
//---------------------------------------------------------------------------
TfrmListTransmitters* __fastcall TFormProvider::ShowTxList(HWND caller, int elementId, long TxFlags)
{
    //  ����� ����� TfrmListTransmitters * � ������ caller ����� MDIChildCount
    TfrmListTransmitters* TxList = NULL;
    for (int i = 0; i < frmMain->MDIChildCount; i++) {
        if ((TxList = dynamic_cast<TfrmListTransmitters*>(frmMain->MDIChildren[i])) && caller == (TxList->Caller) && TxFlags == TxList->list_flags) {
            TxList->Show();
            return TxList;
        }
    }
    //  ���� ���, �� �������
    TCursor oldCursor = Screen->Cursor;
    Screen->Cursor = crHourGlass;
    try {

        TxList = new TfrmListTransmitters(Application, caller, elementId, TxFlags);

    } __finally {
        Screen->Cursor = oldCursor;
    }

   return TxList;
}
//---------------------------------------------------------------------------

TfrmSelection* __fastcall TFormProvider::ShowSelection(int selId)
{
    TfrmSelection* fs = NULL;
    for (int i = 0; i < frmMain->MDIChildCount; i++) {
        if ((fs = dynamic_cast<TfrmSelection*>(frmMain->MDIChildren[i])) && selId == fs->GetId()) {
            fs->Show();
            return fs;
        }
    }
    //  ���� ���, �� �������
    TCursor oldCursor = Screen->Cursor;
    Screen->Cursor = crHourGlass;
    try {

        fs = new TfrmSelection(Application, (void*)selId);
        
        fs->Left = 0;
        fs->Top = 0;
        fs->Width = frmMain->ClientWidth
                    - frmMain->leftSplitter->Width
                    - frmMain->rightSplitter->Width
                    - frmMain->pnlLeftDockPanel->Width
                    - frmMain->pnlRightDockPanel->Width
                    - GetSystemMetrics(SM_CXFRAME);

        fs->Height = frmMain->ClientHeight
                    - frmMain->tbrShortcut->Height
                    - frmMain->StatusBar1->Height
                    - GetSystemMetrics(SM_CYFRAME);

    } __finally {
        Screen->Cursor = oldCursor;
    }

    return fs;
}

TfrmSearch* __fastcall TFormProvider::ShowSearch(ObjType ot)
{
    TfrmSearch *fs;
    fs = new TfrmSearch(Application);
    if(ot == otTx)
        fs->Tag = otTxSearch;
    else
        fs->Tag = ot;
    int no = 1;
    String cpt = RemoveAmpersand("�����: " + GetListName(ot));
    fs->Caption = "";
    while (fs->Caption.IsEmpty())
    {
        fs->Caption = cpt + '(' + IntToStr(no++) + ')';
        for (int i = 0; i < frmMain->MDIChildCount; i++)
            if (frmMain->MDIChildren[i] != fs && frmMain->MDIChildren[i]->Caption == fs->Caption)
                fs->Caption = "";

    }
    String rootDir = ExtractFilePath(Application->ExeName);
    String criteriaFile;
    String filedsConfFile;
    switch (ot)
    {
        case otTx:
            criteriaFile = "whereTransmitters.ini";
            filedsConfFile = "txSearchAvailFields.ini";
            fs->pnTx->Visible = true;
            break;
        case otSITES:
            criteriaFile = "SiteCriteria.ini";
            filedsConfFile = "SiteFields.ini";
            fs->pnSite->Visible = true;
            fs->actColumns->Enabled = false;
            fs->actExport->Enabled = false;
            break;
        default:
            delete fs;
            throw *(new Exception("����������� ��� ������� ��� ������"));
    }
    String searchDir = rootDir + "Search\\";
    String oldFileName = rootDir + criteriaFile;
    String newFileName = searchDir + criteriaFile;
    if (!DirectoryExists(searchDir))
        CreateDir(searchDir);
    if (!FileExists(newFileName) && FileExists(oldFileName))
        MoveFileA(oldFileName.c_str(), newFileName.c_str());
    criteriaFile = newFileName;
    fs->fmWhereCriteria1->loadConfig(criteriaFile, fs->sqlCountry->Database);

    oldFileName = rootDir + filedsConfFile;
    newFileName = searchDir + filedsConfFile;
    if (!FileExists(newFileName) && FileExists(oldFileName))
        MoveFileA(oldFileName.c_str(), newFileName.c_str());
    filedsConfFile = newFileName;
    fs->iniFieldsFileName = newFileName;

    fs->LoadFields();
    fs->Visible = true;

    return fs;
}

__fastcall TFormProvider::~TFormProvider()
{
    //
}

TForm * __fastcall TFormProvider::ShowPlanning()
{
    TfrmPlanning *pf = NULL;
    for (int i = 0; i < frmMain->MDIChildCount; i++) {
        if (pf = dynamic_cast<TfrmPlanning *>(frmMain->MDIChildren[i])) {
            pf->Show();
            break;
        }
    }
    if (!pf) {
        pf = new TfrmPlanning(Application);
        pf->Left = 0;
        pf->Top = 0;
        pf->Width = frmMain->ClientWidth
                    - frmMain->leftSplitter->Width
                    - frmMain->rightSplitter->Width
                    - frmMain->pnlLeftDockPanel->Width
                    - frmMain->pnlRightDockPanel->Width
                    - GetSystemMetrics(SM_CXFRAME);

        pf->Height = frmMain->ClientHeight
                    - frmMain->tbrShortcut->Height
                    - frmMain->StatusBar1->Height
                    - GetSystemMetrics(SM_CYFRAME);
    }
    return pf;
}

TfrmExplorer* __fastcall TFormProvider::ShowExplorer()
{
    if (!frmExplorer)
    {
        frmExplorer = new TfrmExplorer(Application);
        frmExplorer->Show();
        frmExplorer->Refresh();
    }
    frmExplorer->trvExplorer->SetFocus();
    return frmExplorer;
}
//---------------------------------------------------------------------------

void __fastcall TFormProvider::UpdateTransmitters(int ID)
{
    if ( frmExplorer )//���� ������� ����������
        frmExplorer->Refresh();//������ �������� ������� �������������

    TfrmListTransmitters* lt = NULL;
    TfrmListStand* ls = NULL;
    TfrmSelection* expert = NULL;
    TLisObjectGridForm* lf = NULL;

    //TCOMILISBCTx tx(txBroker.GetTx(ID), true);

    //������ ������������
    for (int i = 0; i < frmMain->MDIChildCount; i++)
        if ((lf = dynamic_cast<TLisObjectGridForm*>(frmMain->MDIChildren[i])) && lf->objType == otTx)
        {
            lf->grid->Refresh();
        } else if ( lt = dynamic_cast<TfrmListTransmitters*>(frmMain->MDIChildren[i]) )
        {
            int currentRecord = lt->dstList->RecNo;
            lt->dstList->Close();
            lt->dstList->Open();
            lt->dstList->RecNo = currentRecord;
        } else if ( ls = dynamic_cast<TfrmListStand*>(frmMain->MDIChildren[i]) )
        {
            //������ ����
            ls->dstTx->Close();
            ls->dstTx->Open();
        } else if ( expert = dynamic_cast<TfrmSelection*>(frmMain->MDIChildren[i]) )
        {
            for (int i = 0; i < expert->txList.Size; i++)
                if (expert->txList.get_TxId(i) == ID)
                {
                    if ( expert->pcSelection->ActivePage == expert->tshMap )
                        expert->pcSelectionChange(expert);
                    else
                        expert->refresh();
                    break;
                }
        }

}
//---------------------------------------------------------------------------

void __fastcall TFormProvider::UpdateStands(int ID)
{
    if ( frmExplorer )//���� ������� ����������
        frmExplorer->Refresh();//������ �������� ������� �������������

    TfrmListTransmitters* lt = NULL;
    //������ ������������
    for (int i = 0; i < frmMain->MDIChildCount; i++)
        if ( lt = dynamic_cast<TfrmListTransmitters*>(frmMain->MDIChildren[i]) )
        {
            int currentRecord = lt->dstList->RecNo;
            lt->dstList->Close();
            lt->dstList->Open();
            lt->dstList->RecNo = currentRecord;
            break;
        }

    TfrmTxBase* ft = NULL;
    //��������
    for (int i = 0; i < frmMain->MDIChildCount; i++)
        if ( ( ft = dynamic_cast<TfrmTxBase*>(frmMain->MDIChildren[i]) )
          && ( ft->ibqStand->Fields->FieldByName("ID")->AsInteger == ID ) )
          {
            ft->ibqStand->Close();
            ft->ibqStand->Open();

          }
}

//---------------------------------------------------------------------------

void __fastcall TFormProvider::DisableApplicationControls(void)
{
    TfrmTxBase* form = NULL;
    for (int i = 0; i < frmMain->MDIChildCount; i++)
        if ( form = dynamic_cast<TfrmTxBase*>(frmMain->MDIChildren[i]) )
            form->Enabled = false;

    if ( frmExplorer )
        frmExplorer->Enabled = false;

    frmMain->actParams->Enabled = false;
}
//---------------------------------------------------------------------------

void __fastcall TFormProvider::EnableApplicationControls(void)
{
    TfrmTxBase* form = NULL;
    for (int i = 0; i < frmMain->MDIChildCount; i++)
        if ( form = dynamic_cast<TfrmTxBase*>(frmMain->MDIChildren[i]) )
            form->Enabled = true;

    if ( frmExplorer )
        frmExplorer->Enabled = true;

    frmMain->actParams->Enabled = true;
}
//---------------------------------------------------------------------------

int __fastcall TFormProvider::ShowForm(ObjType objType, unsigned objId)
{
    std::auto_ptr<TfrmBaseObjForm> frm;
    switch (objType)
    {
        //case otOTHER_TERR_SVC:
        //    frm.reset(new TfrmOtherTerrSrvc(Application));
        //    break;
        case otDocument:
            frm.reset(new TfrmDocumentsSettings(Application));
            break;
        case otDocTemplate:
            frm.reset(new TSelectTypeDoc(Application));
            break;
        case otTx:
            ShowTx(txBroker.GetTx(objId, dmMain->GetObjClsid(dmMain->GetSc(objId))));
            return objId;
        default: throw *(new Exception("����������� ��� �������"));
    }
    std::auto_ptr<TDataSet> ds(dmMain->GetObject(objType, objId, frm->tr));
    frm->dscObj->DataSet = ds.get();
    frm->Caption = GetObjectName(objType);
    frm->objId = ds->FieldByName("ID")->AsInteger;
    frm->ShowModal();
    return objId;
}

void __fastcall TFormProvider::UpdateViews()
{
    // Lines in selection grid and tx names
    TfrmSelection* form = NULL;
    for ( int i = 0; i < frmMain->MDIChildCount; i++ )
        if ( form = dynamic_cast<TfrmSelection*>(frmMain->MDIChildren[i]) )
        {
            form->UpdateLines();
            form->UpdateTxNames();
        }
}

String __fastcall TFormProvider::GetListName(ObjType objType)
{
    if (lstName.empty())
    {
        lstName[otACCOUNT_STATES] = "������ &�����";
        lstName[otTx] = "&���������i";
        lstName[otTxSearch] = "&���������i";
        lstName[otBLOCK_DAB] = "&����� DAB";
        lstName[otEQUIP] = "&����������";
        lstName[otDocument] = "&���������";
        lstName[otDocTemplate] = "&������� ���������";
        lstName[otLICENSES] = "&˳���糿";
        lstName[otOWNER] = "&��������";
        lstName[otSITES] = "&�����";
        lstName[otORGANIZATION] = "&�������";
        lstName[otSFN] = "��&�";
        lstName[otDIG_ALLOTMENT] = "&������ ��������";
        lstName[otDIG_SUBAREAS] = "&������� �������� �������";
    }
    return lstName[objType];
}

String __fastcall TFormProvider::GetObjectName(ObjType objType)
{
    if (objName.empty())
    {
        objName[otACCOUNT_STATES] = "�������� &����";
        objName[otBLOCK_DAB] = "&���� DAB";
        objName[otTx] = "&���������";
        objName[otTxSearch] = "&���������";
        objName[otDocTemplate] = "&������ ���������";
        objName[otEQUIP] = "&����������";
        objName[otDocument] = "&��������";
        objName[otLICENSES] = "&˳�����";
        objName[otOWNER] = "&�������";
        objName[otSITES] = "&����";
        objName[otORGANIZATION] = "&�������";
        objName[otSFN] = "��&�";
        objName[otDIG_ALLOTMENT] = "&������� ��������";
        objName[otDIG_SUBAREAS] = "&������ ��������� ��������";
    }
    return objName[objType];
}

String __fastcall TFormProvider::RemoveAmpersand(String str)
{
    int pos = 0; String ampersand("&");
    while ((pos = str.Pos(ampersand)) > 0)
        str.Delete(pos, 1);
    return str;
}

int __fastcall TFormProvider::DelimitedPos(String s, String subs)
{
    int len = subs.Length();
    int pos = s.LowerCase().Pos(subs);
    if (pos <= 1 || (s[pos-1] != ' ' && s[pos-1] != '\n') ||
    pos+len >= s.Length() || (s[pos+len] != ' ' && s[pos+len] != '\n'))
        pos = 0;
    return pos;
}
