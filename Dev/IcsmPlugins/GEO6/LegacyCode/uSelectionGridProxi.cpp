//---------------------------------------------------------------------------


#pragma hdrstop

#include "uSelectionGridProxi.h"
#include "uSelection.h"
#include "uParams.h"
//#include "uMainDm.h"

//---------------------------------------------------------------------------

#pragma package(smart_init)

TSelectionProxi* selProxi;

String TSelectionProxi::RunQuery(TLisObjectGrid* sender, String query)
{
    return String();
}

bool TSelectionProxi::RunListQuery(TLisObjectGrid* sender, String query)
{
    TfrmSelection* sf = dynamic_cast<TfrmSelection*>(sender->Owner);
    if (sf)
    {
        maxPos = sf->txList.Size;
        curPos = 1;
    }
    return IsEof(sender);
}

bool TSelectionProxi::Next(TLisObjectGrid* sender)
{
    if (!IsEof(sender))
        ++curPos;
    return IsEof(sender);
};

String TSelectionProxi::GetVal(TLisObjectGrid* sender, int row, LisColumnInfo info)
{
    String res;
    TfrmSelection* sf = dynamic_cast<TfrmSelection*>(sender->Owner);
    if (sf)
    {
        String attrName = info.fldName;
        int txIdx = row+1;
        ILISBCTx* obj = NULL;
        sf->txList->get_Tx(txIdx, &obj);

        if (obj == NULL)
            return "";

        TCOMILISBCTx tx(obj, true);
        TCOMILisBcDigAllot allot;
        if (tx.systemcast == ttAllot)
            tx->QueryInterface(IID_ILisBcDigAllot, (void**)&allot);
        TCOMILisBcLfMf lfmf;
        if (tx.systemcast == ttAM)
            tx->QueryInterface(IID_ILisBcLfMf, (void**)&lfmf);

        if (attrName == "") {
        } else if (attrName == "No") {
            if (txIdx <= sf->tags.size())
                res = sf->tags[txIdx-1];
        } else if (attrName == "SubNo") {
            res = "";
        } else if (attrName == "IsUsedInCalc") {
            res = sf->txList.get_TxUseInCalc(txIdx) ? "(+)" : "";
        } else if (attrName == "IsShownOnMap") {
            res = sf->txList.get_TxShowOnMap(txIdx) ? "(+)" : "";
        } else if (attrName == "ZoneOverlapped") {
            res = sf->txList.get_TxZoneOverlapping(txIdx) > 0 ? "(+)" : "";
        } else if (attrName == "IsDay") {
            res = lfmf->is_day ? "(+)" : "";
        } else if (attrName == "Name") {
            res = tx.station_name;
        } else if (attrName == "Channel") {

            WideString dab_dvb;
            std::auto_ptr<TIBSQL> sql (new TIBSQL(this));
            sql->Database = dmMain->dbMain;
            long l;

            switch(tx.systemcast) {
                case ttTV: case ttDVB: //case ttAllot:
                    res = dmMain->getChannelName(tx.channel_id); break;
                case ttAM:
                    res = FormatFloat("0.###", tx.sound_carrier_primary*1000.); break;
                case ttFM:
                    res = FormatFloat("0.###", tx.sound_carrier_primary); break;
                case ttDAB:
                    res = dmMain->getDabBlockName(tx.blockcentrefreq); break;
                case ttCTV:
                    res = " -- "; break;
                case ttAllot:
                    HrCheck(allot->get_notice_type(&dab_dvb));
                    if (dab_dvb == WideString(L"GS2") || dab_dvb == WideString(L"DS2"))
                    {
                        l = 0;
                        allot->get_channel_id(&l);
                        sql->SQL->Text = "select b.ID, b.NAME from blockdab b where b.ID = :ID";
                        sql->ParamByName("ID")->AsInteger = l;
                        sql->ExecQuery();
                        res = sql->Fields[1]->AsString;
                    }
                    else 
                        res = dmMain->getChannelName(tx.channel_id);
                    break;
                default: break;
            }
        } else if (attrName == "Dist") {
            res = FormatFloat("0.0", sf->txList.get_TxDistance(txIdx));
        } else if (attrName == "Azm") {
            res = FormatFloat("0", sf->txList.get_TxAzimuth(txIdx));
        } else if (attrName == "ErpMax") {
            switch(tx.systemcast) {
                case ttTV:
                    res = FormatFloat("0.00", tx.epr_video_max);
                    break;
                case ttAM: case ttFM: case ttDAB: case ttDVB:
                    res = FormatFloat("0.00", tx.epr_sound_max_primary);
                    break;
                default:
                    res = "";
                    break;
            }
        } else if (attrName == "EahMax") {
            if (allot.IsBound())
                res = "";
            else
                res = tx.height_eft_max;
        } else if (attrName == "Polar") {
            char p;
            if (allot.IsBound())
                allot->get_polar(&p), res = AnsiString(&p, 1);
            else
                res = dmMain->getPolarizationName(tx.polarization);
        } else if (attrName == "Offset") {
            long offset = 0;
            if (allot.IsBound())
                res = allot->get_offset(&offset), IntToStr(offset);
            else
                res = tx.video_offset_line;
        } else if (attrName == "State") {
            res = dmMain->getConditionName(tx.acin_id) + " / " + dmMain->getConditionName(tx.acout_id);
        } else if (attrName == "IntfWant") {
            res = FormatFloat("0.00", sf->txList.get_TxWantInterfere(txIdx));
        } else if (attrName == "IntfUnwant") {
            res = FormatFloat("0.00", sf->txList.get_TxUnwantInterfere(txIdx));
        } else if (attrName == "IntfWantKind") {
            res = String().sprintf("%c",(char)sf->txList.get_TxWantedKind(txIdx));// ? "Ï" : "Ò";
        } else if (attrName == "IntfUnwantKind") {
            res = String().sprintf("%c",(char)sf->txList.get_TxUnwantedKind(txIdx));// ? "Ï" : "Ò";
        } else if (attrName == "RegNum") {
            res = tx.numregion;
        } else if (attrName == "AdmId") {
            int adminid = tx.adminid;
            if (adminid > 0)
                res = String().sprintf("%04d", adminid);
        } else if (attrName == "Lat") {
            TBCTxType tt = tx.systemcast;
            if (tt != ttAllot)
                res = dmMain->coordToStr(tx.latitude, 'Y');
            else
                res = "";
        } else if (attrName == "Lon") {
            TBCTxType tt = tx.systemcast;
            if (tt != ttAllot)
                res = dmMain->coordToStr(tx.longitude, 'X');
            else
                res = "";
        } else if (attrName == "Sys") {
            long sys = 0;
            switch(tx.systemcast) {
                case ttTV:  res = dmMain->getAtsName(tx.typesystem); break;
                case ttDVB: res = dmMain->getDvbSystemName(tx.dvb_system); break;
                case ttAM:  lfmf->get_lfmf_system(&sys); res = dmMain->GetLfMfName(sys); break;
                case ttFM:  res = dmMain->getArsName(tx.fm_system); break;
                default :   res = ""; break;
            }
        } else if (attrName == "Color") {
            if (tx.systemcast == ttTV)
                res = dmMain->getColorName(tx.systemcolor);
            else
                res = "";
        }

    }
    return res;
}

int TSelectionProxi::GetId(TLisObjectGrid* sender, int row)
{
    TfrmSelection* sf = dynamic_cast<TfrmSelection*>(sender->Owner);
    long id = 0;
    if (sf && row > -1)
        HrCheck(sf->txList.get_TxId(row+1, &id));
    return id;
}

void TSelectionProxi::FormatCanvas(TLisObjectGrid* sender, int aCol, int aRow, TCanvas *Canvas, bool &draw)
{
    TfrmSelection* sf = dynamic_cast<TfrmSelection*>(sender->Owner);
    if (sf)
    {
        // set font style for name
        if (sf->grid->columnsInfo[aCol].fldName == "Name")
        {
            if (sf->grid->dg->Cells[0][aRow].Pos(".") > 0) // indirect sign that this is tx under allot
                Canvas->Font->Style = Canvas->Font->Style << fsItalic;
            else
            {
                int txIdx = aRow + 1;
                ILISBCTx* obj = NULL;
                sf->txList->get_Tx(txIdx, &obj);
                if (obj != NULL)
                {
                    TBCTxType tt = ttUNKNOWN;
                    obj->get_systemcast(&tt);
                    if (tt == ttAllot)
                        Canvas->Font->Style = Canvas->Font->Style << fsBold;
                }
            }
        }
    }
}

void TSelectionProxi::SortGrid(TLisObjectGrid* sender, int colIdx)
{
    TfrmSelection* sf = dynamic_cast<TfrmSelection*>(sender->Owner);
    if (sf)
    {
    }
}
