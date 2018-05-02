//---------------------------------------------------------------------------
#include <vcl.h>

#include <memory>

#include <DBTables.hpp>
#include <Dialogs.hpp>

#include "uAnalyzer.h"
#include "uExplorer.h"
#include "uFrmDocumentsSettings.h"
#include "uFrmTxBase.h"
#include "uMainDm.h"
#include "uMainForm.h"
#include "uNewSelection.h"
#include "uParams.h"
#include "uReliefFrm.h"
#include "uSelection.h"
#include "uSelectTxTree.h"

#include "FormProvider.h"
#include "RSAGeography_TLB.h"
#include "TxBroker.h"
#include "uBaseList.h"
#pragma hdrstop

#include "tempcursor.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "CustomMap"
#pragma link "uLisObjectGrid"
#pragma resource "*.dfm"

//---------------------------------------------------------------------------
__fastcall TfrmTxBase::TfrmTxBase(TComponent* Owner) : TForm (Owner)
{
    throw *(new Exception("TfrmTxBase(TComponent* Owner) - нельзя"));
}

class TLicProxi: public TComponent, public TLisObjectGridProxi
{
  public:
    TIBSQL *qry;
    TIBSQL *qryTemp;
    __fastcall TLicProxi(TComponent* owner);
    void RunQuery(TIBSQL* sql, String query);

    bool RunListQuery(TLisObjectGrid* sender, String query);
    String RunQuery(TLisObjectGrid* sender, String query);
    bool Next(TLisObjectGrid* sender) { qry->Next(); return qry->Eof; } ;
    String GetVal(TLisObjectGrid* sender, int row, LisColumnInfo info){return qry->FieldByName(info.fldName)->AsString;};
    bool IsEof(TLisObjectGrid* sender) { return qry->Eof; } ;
    int GetId(TLisObjectGrid* sender, int row);
    String GetFieldSpecFromAlias(TLisObjectGrid* sender, String alias) { return alias; } ;
    void FormatCanvas(TLisObjectGrid* sender, int aCol, int aRow, TCanvas *Canvas, bool &draw);
    virtual void SortGrid(TLisObjectGrid* sender, int colIdx) {};
    virtual int CreateObject(TLisObjectGrid* sender) { return 0; };
    virtual int CopyObject(TLisObjectGrid* sender, int objId) { return 0; };
    virtual bool DeleteObject(TLisObjectGrid* sender, int objId) { return 0; };
    virtual bool EditObject(TLisObjectGrid* sender, int objId) { return false; };
    virtual void PickObject(TLisObjectGrid* sender, int objId) {};
};

__fastcall TLicProxi::TLicProxi(TComponent* owner): TComponent(owner)
{
    qry = new TIBSQL(this);
    qry->Database = dmMain->dbMain;
    qryTemp = new TIBSQL(this);
    qryTemp->Database = dmMain->dbMain;
}

void TLicProxi::RunQuery(TIBSQL* sql, String query)
{
    sql->Close();
    sql->SQL->Text = query;
    sql->ExecQuery();
}

String TLicProxi::RunQuery(TLisObjectGrid* sender, String query)
{
    RunQuery(qryTemp, query);
    if (!qryTemp->Eof && qryTemp->Current()->Count > 0)
        return qryTemp->Fields[0]->AsString;
    else
        return String();
}

bool TLicProxi::RunListQuery(TLisObjectGrid* sender, String query)
{
    RunQuery(qry, query);
    return qry->Eof;
}

int TLicProxi::GetId(TLisObjectGrid* sender, int id)
{
    if (!qry->Eof && qry->Current()->Names.Pos("ID") > 0)
        return qry->FieldByName("ID")->AsInteger;
    else
        return 0;
}

void TLicProxi::FormatCanvas(TLisObjectGrid* sender, int aCol, int aRow, TCanvas *Canvas, bool &draw)
{
    if (aCol == 0)
        Canvas->Font->Style = Canvas->Font->Style << fsBold;
}

__fastcall TfrmTxBase::TfrmTxBase(TComponent* Owner, ILISBCTx *in_Tx)
        : TForm(Owner), Tx(in_Tx, true)
{
    Tx->get_systemcast(&type_form);
    OleCheck(Tx->get_id(&id));
    flag_owner = foNON;
    lastLicCaller = NULL;

    pcData->ActivePage = tshCommon;
    pcRemark->ActivePage = tshNote;

    reply_required = false;

    popupMenu = new TPopupMenu(this);

    objGrdLic->ClearColumns();
    objGrdLic->AddColumn("Номер", "NUMLICENSE", "NUMLICENSE", taLeftJustify, ptString, 70);
    objGrdLic->AddColumn("Дата видачи", "DATEFROM", "DATEFROM", taLeftJustify, ptString, 80);
    objGrdLic->AddColumn("Дата кiнця", "DATETO", "DATETO", taLeftJustify, ptString, 80);
    objGrdLic->AddColumn("Власник", "NAMEORGANIZATION", "NAMEORGANIZATION", taLeftJustify, ptString, 200);
    objGrdLic->SetProxi(new TLicProxi(this));

    TxDataLoad();
}
//---------------------------------------------------------------------------

AnsiString TfrmTxBase::Passband2Str(double video_carrier)
{
    AnsiString result = "";

    if ( ( video_carrier >= 0.001 ) && ( video_carrier <= 0.009 ) )
        result = "H00" + FloatToStrF(floor(video_carrier * 1000), ffGeneral, 0, 1);
    else if ( ( video_carrier >= 0.01 ) && ( video_carrier <= 0.099 ) )
        result = "H0" + FloatToStrF(floor(video_carrier * 1000), ffGeneral, 0, 2);
    else if ( ( video_carrier >= 0.1 ) && ( video_carrier <= 0.999 ) )
        result = "H" + FloatToStrF(floor(video_carrier * 1000), ffGeneral, 0, 3);
    else if ( ( video_carrier >= 1 ) && ( video_carrier <= 9.99 ) )
        result = FloatToStr(floor(video_carrier)) + "H" + FloatToStrF(floor(( video_carrier - floor(video_carrier) ) * 100), ffGeneral, 0, 2);
    else if ( ( video_carrier >= 10 ) && ( video_carrier <= 99.9 ) )
        result = FloatToStr(floor(video_carrier)) + "H" + FloatToStrF(floor(( video_carrier - floor(video_carrier) ) * 10), ffGeneral, 0, 1);
    else if ( ( video_carrier >= 100 ) && ( video_carrier <= 999 ) )
        result = FloatToStr(floor(video_carrier)) + "H";
    else if ( ( video_carrier >= 1000 ) && ( video_carrier <= 9990 ) )
    {
        video_carrier /= 1000;
        result = FloatToStr(floor(video_carrier)) + "K" + FloatToStrF(floor(( video_carrier - floor(video_carrier) ) * 100), ffGeneral, 0, 2);
    }
    else if ( ( video_carrier >= 10000 ) && ( video_carrier <= 99900 ) )
    {
        video_carrier /= 1000;
        result = FloatToStr(floor(video_carrier)) + "K" + FloatToStrF(floor(( video_carrier - floor(video_carrier) ) * 10), ffGeneral, 0, 1);
    }
    else if ( ( video_carrier >= 100000 ) && ( video_carrier <= 999000 ) )
    {
        video_carrier /= 1000;
        result = FloatToStr(floor(video_carrier)) + "K";
    }
    else if ( ( video_carrier >= 1000000 ) && ( video_carrier <= 9990000 ) )
    {
        video_carrier /= 1000000;
        result = FloatToStr(floor(video_carrier)) + "M" + FloatToStrF(floor(( video_carrier - floor(video_carrier) ) * 100), ffGeneral, 0, 0);
    }
    else if ( ( video_carrier >= 10000000 ) && ( video_carrier <= 99900000 ) )
    {
        video_carrier /= 1000000;
        result = FloatToStr(floor(video_carrier)) + "M" + FloatToStrF(floor(( video_carrier - floor(video_carrier) ) * 10), ffGeneral, 0, 0);
    }
    else if ( ( video_carrier >= 100000000 ) && ( video_carrier <= 999000000 ) )
    {
        video_carrier /= 1000000;
        result = FloatToStr(floor(video_carrier)) + "M";
    }
    else if ( ( video_carrier >= 1000000000 ) && ( video_carrier <= 9990000000 ) )
    {
        video_carrier /= 1000000000;
        result = FloatToStr(floor(video_carrier)) + "G" + FloatToStrF(floor(( video_carrier - floor(video_carrier) ) * 100), ffGeneral, 0, 2);
    }
    else if ( ( video_carrier >= 10000000000 ) && ( video_carrier <= 99900000000 ) )
    {
        video_carrier /= 1000000000;
        result = FloatToStr(floor(video_carrier)) + "G" + FloatToStrF(floor(( video_carrier - floor(video_carrier) ) * 10), ffGeneral, 0, 1);
    }
    else if ( ( video_carrier >= 100000000000 ) && ( video_carrier <= 999000000000 ) )
    {
        video_carrier /= 1000000000;
        result = FloatToStr(floor(video_carrier)) + "G";
    }

    return result;
}


void __fastcall TfrmTxBase::acceptListElementSelection(Messages::TMessage &Message)
{
    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Database = dmMain->dbMain;
    Variant var_rez;

    switch (Message.WParam) {
         case otSITES:
            // опора
            if (ibdsStantionsBaseSTATUS->AsInteger != tsDraft)
                return;
            ibdsStantionsBase->Edit();
            ibdsStantionsBaseSTAND_ID->AsInteger =  Message.LParam;

            sql->SQL->Text = "select S.ID, S.NAMESITE, S.CITY_ID, S.AREA_ID, S.HEIGHT_SEA, AREA.NAME AREA_NAME, "
                                    " CITY.NAME CITY_NAME, STREET.NAME STREET_NAME, S.ADDRESS, AREA.NUMREGION NUMREG "
                                    ", s.latitude, s.longitude "
                                    " from STAND S "
                                    " left outer join AREA on (STAND.AREA_ID = AREA.ID) "
                                    " left outer join STREET on (STAND.STREET_ID = STREET.ID) "
                                    " left outer join CITY on (STAND.CITY_ID = CITY.ID) where S.ID = " + AnsiString(Message.LParam);
            sql->ExecQuery();
            edtStand->Text = sql->FieldByName("NAMESITE")->AsString;
            edtHPoint->Text = sql->FieldByName("HEIGHT_SEA")->AsString;
            edtAreaName->Text = sql->FieldByName("AREA_NAME")->AsString;
            edtCityName->Text = sql->FieldByName("CITY_NAME")->AsString;
            edtNumRegion->Text = sql->FieldByName("NUMREG")->AsString;
            edtAdress->Text = sql->FieldByName("STREET_NAME")->AsString + ", "+ sql->FieldByName("ADDRESS")->AsString;

            double t_lon; t_lon = sql->FieldByName("LONGITUDE")->AsDouble;
            double t_lat; t_lat = sql->FieldByName("LATITUDE")->AsDouble;
            Tx->set_longitude(t_lon);
            Tx->set_latitude(t_lat);
            edtLong->Text = dmMain->coordToStr(t_lon, 'X');
            edtLat->Text = dmMain->coordToStr(t_lat, 'Y');

            break;
         case otOWNER:
            // владелец/оператор
            if (ibdsStantionsBaseSTATUS->AsInteger != tsDraft)
                return;
            if (ibdsStantionsBase->State != dsEdit)
                ibdsStantionsBase->Edit();
            sql->SQL->Text = "select NAMEORGANIZATION from OWNER where ID = " + AnsiString(Message.LParam);
            sql->ExecQuery();

            if (flag_owner == foTRK) {
                ibdsStantionsBaseOWNER_ID->AsInteger =  Message.LParam;
                ibdsStantionsBaseTRK_NAME->AsString = sql->FieldByName("NAMEORGANIZATION")->AsString;
            } else {
                ibdsStantionsBaseOPERATOR_ID->AsInteger = Message.LParam;
                ibdsStantionsBaseOPERATOR_NAME->AsString = sql->FieldByName("NAMEORGANIZATION")->AsString;
            }
            break;
         case otORGANIZATION:
            // организация, с которой переписка ведётся
            var_rez = ibdsTelecomOrg->Lookup("TELECOMORG_ID", AnsiString(Message.LParam), "TELECOMORG_ID");
            if (VarType(var_rez) == varNull)
            {
                ibdsTelecomOrg->Append();
                ibdsTelecomOrg->FieldByName("TELECOMORG_ID")->AsInteger =   Message.LParam;
                ibdsTelecomOrg->FieldByName("TRANSMITTER_ID")->AsInteger =  ibdsStantionsBaseID->AsInteger;
                if ( ibdsStantionsBaseACCOUNTCONDITION_IN->AsInteger > 0 )
                    ibdsTelecomOrg->FieldByName("ACCOUNTCONDITION_ID")->AsInteger = ibdsStantionsBaseACCOUNTCONDITION_IN->AsInteger;
                else
                {

                    sql->Close();
                    sql->SQL->Text = "select ID from ACCOUNTCONDITION";
                    sql->ExecQuery();

                    ibdsTelecomOrg->FieldByName("ACCOUNTCONDITION_ID")->AsInteger = sql->Fields[0]->AsInteger;
                }

                ibdsTelecomOrg->Post();
            }

            ibdsTelecomOrg->Active = false;
            ibdsTelecomOrg->ParamByName("TRANSMITTER_ID")->AsInteger = ibdsStantionsBaseID->AsInteger;
            ibdsTelecomOrg->Active = true;
            ibdsTelecomOrg->Last();

            ibdsDocuments->Active = false;
            ibdsDocuments->ParamByName("TELECOMORGANIZATION_ID")->AsInteger = Message.LParam;
            ibdsDocuments->Active = true;
            //btnDocCreateClick(this);
            break;
        case otEQUIP:
            // оборудование
            sql->SQL->Text = "select * from EQUIPMENT where ID = " + AnsiString(Message.LParam);
            sql->ExecQuery();
            if (ibdsEquipment->State != dsInsert)
                ibdsEquipment->Insert();
            ibdsEquipmentEQUIPMENT_ID->AsInteger = sql->FieldByName("ID")->AsInteger;
            ibdsEquipmentTRANSMITTERS_ID->AsInteger = ibdsStantionsBaseID->AsInteger;
            ibdsEquipmentNAME->AsString = sql->FieldByName("NAME")->AsString;
	        ibdsEquipmentTYPEEQUIPMENT->AsString = sql->FieldByName("TYPEEQUIPMENT")->AsString;
            ibdsEquipmentMANUFACTURE->AsString = sql->FieldByName("MANUFACTURE")->AsString;
            ibdsEquipment->Post();
            break;
        case otLICENSES:
            if (lastLicCaller == btResLic)
            {
                sql->SQL->Text = "select l.ID, l.NUMLICENSE, l.DATEFROM, l.DATETO, o.ID O_ID, o.NAMEORGANIZATION"
                        " from LICENSE l left outer join OWNER o on (o.id = l.OWNER_ID) "
                        " where l.ID = " + AnsiString(Message.LParam);
                sql->ExecQuery();
                if (ibdsLicenses->State != dsEdit)
                    ibdsLicenses->Edit();
                ibdsLicensesLICENSE_RFR_ID->AsInteger = Message.LParam;
                ibdsLicensesL_RFR_NUMLICENSE->AsString = sql->FieldByName("NUMLICENSE")->AsString;
                ibdsLicensesL_RFR_DATEFROM->AsString = sql->FieldByName("DATEFROM")->AsString;
                ibdsLicensesL_RFR_DATETO->AsString = sql->FieldByName("DATETO")->AsString;
                //ibdsLicensesL_RFR_O_ID->AsString = sql->FieldByName("O_ID")->AsString;
                ibdsLicensesL_RFR_O_NAME->AsString = sql->FieldByName("NAMEORGANIZATION")->AsString;
                ibdsLicenses->Post();
                ibdsLicenses->Edit(); // remain it in Edit state to enable Apply action
            } else if (lastLicCaller == btnAttach)
            {
                dmMain->RunQuery(String("insert into NR_LIC_LINK (TX_ID, LIC_ID) values (")+id+", "+Message.LParam+')', ParamList(), tr);
                ibdsLicenses->Edit(); // remain it in Edit state to enable Apply action
                objGrdLic->Refresh();
            }
            {
                //todo: permission owner
            }
            {
                //todo: ?????? NR application ????
            }
            {
                //todo: ?????? RFR license owner ????
            }
            break;
        default:
            break;
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::TxDataLoad()
{
    double DValue;
    long LValue;

    OLECHECK(Tx->get_id(&id));
    edtID->Text = AnsiString(id);

    Tx->get_longitude(&DValue);
    edtLong->Text = dmMain->coordToStr(DValue,'X');
    Tx->get_latitude(&DValue);
    edtLat->Text = dmMain->coordToStr(DValue,'Y');

    if (!tr->Active)
        tr->StartTransaction();

    ibqAccCondNameIn->Active  = false;     ibqAccCondNameIn->Active  = true;
                        ibqAccCondNameIn->FetchAll();
    ibqAccCondNameOut->Active  = false;    ibqAccCondNameOut->Active  = true;
                        ibqAccCondNameOut->FetchAll();
    ibqUserName->Active  = false;          ibqUserName->Active  = true;
    ibqTRKName->Active  = false;           ibqTRKName->Active  = true;
    ibqSystemCastName->Active = false;     ibqSystemCastName->Active = true;
    ibqOwner->Active = false;              ibqOwner->Active = true;

    ibdsStantionsBase->Active = false;
    ibdsStantionsBase->ParamByName("ID")->AsInteger = id;
    ibdsStantionsBase->Active = true;
    ibqStand->ParamByName("ID")->AsInteger = ibdsStantionsBaseSTAND_ID->AsInteger;
    ibqStand->Active = false;              ibqStand->Active = true;

    TBCTxType systemcast;

    Tx->get_systemcast(&systemcast);

    actApply->Enabled = false;
    actLoad->Enabled = false;

    UpdateDbSectLook();
    int status = ibdsStantionsBaseSTATUS->AsInteger;

    if (status == tsDeleted)
        actIntoarchives->Enabled = false;
    else
        actIntoarchives->Enabled = true;

    if (status == tsDraft)
        actIntoBeforeBase->Enabled = false;
    else
        actIntoBeforeBase->Enabled = true;

    if (status != tsDraft) {
        actIntoBase->Enabled = false;
        cbProgramm->Field->ReadOnly = true;
        edtCode->ReadOnly = true;
        edtLong->ReadOnly = true;
        edtLat->ReadOnly = true;
    } else {
        actIntoBase->Enabled = true;
        cbProgramm->Field->ReadOnly = false;
        edtCode->ReadOnly = false;
        edtLong->ReadOnly = false;
        edtLat->ReadOnly = false;
    }

    ibdsLicenses->ParamByName("ID")->AsInteger = id;

    String qry("select link.LIC_ID ID, link.TX_ID, lic.NUMLICENSE, lic.DATEFROM, lic.DATETO, o.NAMEORGANIZATION "
               "from NR_LIC_LINK link "
               "left outer join LICENSE lic on (lic.ID = link.LIC_ID) "
               "left outer join OWNER o on (lic.OWNER_ID = o.ID) "
               "where link.TX_ID = ");
    objGrdLic->SetQuery(qry+IntToStr(id));
    objGrdLic->Refresh();

    //todo: set data of appropriate components
    /*
    // from 'base air'
    if ((ibdsAirDATEPERMBUILDFROM->IsNull)||(!ibdsAirDATEPERMBUILDFROM->AsFloat)) dtpPermBuildBeg->Date = Date();
    else  dtpPermBuildBeg->Date = ibdsAirDATEPERMBUILDFROM->AsDateTime;

    if ((ibdsAirDATEPERMBUILDTO->IsNull)||(!ibdsAirDATEPERMBUILDTO->AsFloat)) dtpPermBuildEnd->Date = Date();
    else  dtpPermBuildEnd->Date = ibdsAirDATEPERMBUILDTO->AsDateTime;
    */
    isChanged = false;
}

void __fastcall TfrmTxBase::TxDataSave()
{
    if (ibdsStantionsBaseSTATUS->AsInteger == tsDraft)
        HrCheck(Tx->save());
    else
        ::MessageBox(NULL, "Увага!\nЗбереження технiчних даних передавача можливо тiльки в ПЕРЕДБАЗI (чорновику)",
                    Application->Title.c_str(), MB_ICONEXCLAMATION);

    Tx->invalidate();

    if ((ibdsStantionsBase->State == dsEdit)||(ibdsStantionsBase->State == dsInsert))
        ibdsStantionsBase->Post();
    if ((ibdsTelecomOrg->State == dsEdit)||(ibdsTelecomOrg->State == dsInsert))
        ibdsTelecomOrg->Post();
    if ((ibdsEquipment->State == dsEdit)||(ibdsEquipment->State == dsInsert))
        ibdsEquipment->Post();

    if ((ibdsDocuments->State == dsEdit)||(ibdsDocuments->State == dsInsert))
        ibdsDocuments->Post();
    if ((ibdsLicenses->State == dsEdit)||(ibdsLicenses->State == dsInsert))
        ibdsLicenses->Post();

    dmMain->trMain->CommitRetaining();
    tr->CommitRetaining();

    isChanged = false;
}


//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::edtLongExit(TObject *Sender)
{
    double DValue;
    try {
        Tx->set_longitude(dmMain->strToCoord(edtLong->Text));
        Tx->get_longitude(&DValue);
        edtLong->Text = dmMain->coordToStr(DValue, 'X');
    } catch (Exception &e) {
        Tx->get_longitude(&DValue);
        edtLong->Text = dmMain->coordToStr(DValue, 'X');
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::edtLatExit(TObject *Sender)
{
    double DValue;
    try {
        Tx->set_latitude(dmMain->strToCoord(edtLat->Text));
        Tx->get_latitude(&DValue);
        edtLat->Text = dmMain->coordToStr(DValue, 'Y');
    } catch (Exception &e) {
        Tx->get_latitude(&DValue);
        edtLat->Text = dmMain->coordToStr(DValue, 'Y');
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::FormClose(TObject *Sender,
      TCloseAction &Action)
{
    Action = caFree;
}
//---------------------------------------------------------------------------


void __fastcall TfrmTxBase::btnStandClick(TObject *Sender)
{
    FormProvider.ShowList(otSITES, this->Handle, ibdsStantionsBaseSTAND_ID->AsInteger);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::btnTRKClick(TObject *Sender)
{
    flag_owner = foTRK;
    FormProvider.ShowList(otOWNER, this->Handle, ibdsStantionsBaseOWNER_ID->AsInteger, " O."+GetOwnerFilter());
}
//---------------------------------------------------------------------------
void __fastcall TfrmTxBase::btnOperatorClick(TObject *Sender)
{
    flag_owner = foOPERATOR;
    FormProvider.ShowList(otOWNER, this->Handle, ibdsStantionsBaseOPERATOR_ID->AsInteger, " O."+GetOwnerFilter());
}


void __fastcall TfrmTxBase::btnDocCreateClick(TObject *Sender)
{
    if ( !ibdsTelecomOrgTELECOMORG_ID->IsNull )
        CreateDoc(Sender);
    else
        Application->MessageBox(AnsiString("Спочатку треба вибрати організацію").c_str(),Application->Title.c_str(), MB_ICONWARNING | MB_OK);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::btnNewTelecomOrgClick(TObject *Sender)
{
    FormProvider.ShowList(otORGANIZATION, this->Handle, 0);
}
//---------------------------------------------------------------------------


void __fastcall TfrmTxBase::dbgDocumentsEditButtonClick(TObject *Sender)
{
    if (dbgDocuments->SelectedField->FieldName == "TYPEINOUT") {
        if ((ibdsDocuments->State != dsEdit) && (ibdsDocuments->State != dsInsert))
            ibdsDocuments->Edit();
        if (ibdsDocumentsTYPELETTER->AsInteger == 1)
            ibdsDocumentsTYPELETTER->AsInteger = 0;
        else ibdsDocumentsTYPELETTER->AsInteger =1;
    } else if (dbgDocuments->SelectedField->FieldName == "ANSWER_NAME") {
        if ((ibdsDocuments->State != dsEdit) && (ibdsDocuments->State != dsInsert))
            ibdsDocuments->Edit();
        if (ibdsDocumentsANSWERIS->AsInteger == 1)
            ibdsDocumentsANSWERIS->AsInteger = 0;
        else
            ibdsDocumentsANSWERIS->AsInteger =1;
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::dbgDocumentsDblClick(TObject *Sender)
{
    if (ibdsDocumentsID->AsInteger > 0)
        FormProvider.ShowForm(otDocument, ibdsDocumentsID->AsInteger);
}
//---------------------------------------------------------------------------


void __fastcall TfrmTxBase::btnDocDelClick(TObject *Sender)
{
    if (Application->MessageBox(AnsiString("Видалити документ?").c_str(),Application->Title.c_str(), MB_ICONQUESTION | MB_YESNO) == IDYES)
        ibdsDocuments->Delete();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::FormCreate(TObject *Sender)
{
    Caption = Caption + " - ";
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::UpdateDbSectLook()
{
    AnsiString str = Caption;
    for( int i = str.Length(); i > 0; i--)
        if( str[i] == '-' )
        {
            str.Delete(i + 2, str.Length() - i);
            break;
        }

    int status = ibdsStantionsBaseSTATUS->AsInteger;

    str += dmMain->getDbSectionInfo(status).name;

    Caption = str;

    lblTxType->Font->Color = status == -1 ? clRed :
                            status == 0 ? clNavy :
                            status == 1 ? clGreen :
                                        clWindowText;
}
//---------------------------------------------------------------------------

bool __fastcall TfrmTxBase::LastRecord(int id)
{
   std::auto_ptr<TIBSQL> sql(new TIBSQL(NULL));
     sql->Database = dmMain->dbMain;
     sql->SQL->Text = "SELECT id FROM Letters WHERE ( letters_Id = :id )";
     sql->ParamByName("id")->Value = id;
   sql->ExecQuery();

   return sql->Eof;
}
//---------------------------------------------------------------------------
/////////////////////////////////////////////////////////////////////////

void __fastcall TfrmTxBase::ibdsDocumentsAfterScroll(TDataSet *DataSet)
{
    btnGotoLast->Enabled = (bool)(ibdsDocumentsLETTERS_ID->AsInteger);
    btnGotoNext->Enabled = false;
    if ( !ibdsDocumentsID->IsNull )
        btnGotoNext->Enabled = !LastRecord(ibdsDocumentsID->AsInteger);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::ibdsTelecomOrgBeforePost(TDataSet *DataSet)
{
    if (ibdsTelecomOrgID->IsNull) {
        int newId = dmMain->getNewId();
        if (newId > 0)
            ibdsTelecomOrgID->AsInteger = newId;
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::btnDocEditClick(TObject *Sender)
{
    dbgDocumentsDblClick(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::btnDocAnswerClick(TObject *Sender)
{
    if ( !ibdsDocumentsID->IsNull )
        CreateDoc(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::btnDocSaveClick(TObject *Sender)
{
 if ((ibdsDocuments->State == dsEdit) || (ibdsDocuments->State == dsInsert))
        ibdsDocuments->Post();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::ibdsTelecomOrgAfterScroll(TDataSet *DataSet)
{
  ibdsDocuments->Active = false;
  ibdsDocuments->ParamByName("TRANSMITTERS_ID")->AsInteger =
            ibdsStantionsBaseID->AsInteger;
  ibdsDocuments->ParamByName("TELECOMORGANIZATION_ID")->AsInteger =
            ibdsTelecomOrgTELECOMORG_ID->AsInteger;
  ibdsDocuments->Active = true;

}
//---------------------------------------------------------------------------
void __fastcall TfrmTxBase::tshEquipmentShow(TObject *Sender)
{
    ibdsEquipment->Active = false;
    ibdsEquipment->ParamByName("ID")->AsInteger = ibdsStantionsBaseID->AsInteger;
    ibdsEquipment->Active = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::btnEqAddClick(TObject *Sender)
{
    FormProvider.ShowList(otEQUIP, this->Handle, 0);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::btnEqDelClick(TObject *Sender)
{
    ibdsEquipment->Delete();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::ibdsEquipmentBeforePost(TDataSet *DataSet)
{
    if (ibdsEquipmentID->IsNull) {
        int newId = dmMain->getNewId();
        if (newId > 0)
            ibdsEquipmentID->AsInteger = newId;
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::btnGotoLastClick(TObject *Sender)
{
   Set<TLocateOption,0,1> flags;
   flags << loCaseInsensitive;
   ibdsDocuments->Locate("ID", ibdsDocumentsLETTERS_ID->AsString, flags);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::btnGotoNextClick(TObject *Sender)
{
    Set<TLocateOption,0,1> flags;
    flags << loCaseInsensitive;
    ibdsDocuments->Locate("LETTERS_ID", ibdsDocumentsID->AsString, flags);
}
//---------------------------------------------------------------------------


void __fastcall TfrmTxBase::ibdsDocumentsTYPELETTERGetText(TField *Sender,
      AnsiString &Text, bool DisplayText)
{
    DisplayText = true;
    if (Sender->IsNull)
        Text="";
    else if (Sender->AsInteger)
        Text = "Вх";
    else
        Text = "Вих";
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::ibdsDocumentsANSWERISGetText(TField *Sender,
      AnsiString &Text, bool DisplayText)
{
    DisplayText = true;
    if (Sender->IsNull)
        Text="";
    else if (Sender->AsInteger)
        Text = "позитивна";
    else
        Text = "негативна";
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::FormCloseQuery(TObject *Sender, bool &CanClose)
{
    long isChanged;
    if (Tx) {
        Tx->get_data_changes(&isChanged);

        if ((ibdsStantionsBaseSTATUS->AsInteger == tsDraft)&&((isChanged && !lblEditing->Visible)||(actApply->Enabled == true))) {
            int rep = Application->MessageBox("Сохранить изменения?", Application->Title.c_str(), MB_ICONQUESTION | MB_YESNOCANCEL);
            switch (rep) {
                case IDYES:
                    actApply->Execute();
                    break;
                case IDNO:
                    //dmMain->trMain->RollbackRetaining();
                    break;
                case IDCANCEL: CanClose = false;
                    break;
            }
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::tshTestpointShow(TObject *Sender)
{
    ibdsTestpoint->Active = false;
    ibdsTestpoint->ParamByName("TRANSMITTERS_ID")->AsInteger = ibdsStantionsBaseID->AsInteger;
    ibdsTestpoint->Active = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::ibdsTestpointAfterInsert(TDataSet *DataSet)
{
    ibdsTestpointTRANSMITTERS_ID->AsInteger = ibdsStantionsBaseID->AsInteger;
}

//---------------------------------------------------------------------------
void __fastcall TfrmTxBase::ibdsTestpointBeforePost(TDataSet *DataSet)
{
    if (ibdsTestpointID->IsNull) {
        int newId = dmMain->getNewId();
        if (newId > 0)
            ibdsTestpointID->AsInteger = newId;
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::actOkUpdate(TObject *Sender)
{
    if (Tx) {
        long ch;
        Tx->get_data_changes(&ch);
        if (lblEditing->Visible != ch)
            lblEditing->Visible = ch;
        if (ch)
            isChanged = true;
    }

    if (!isChanged)
    {
        TDataSet *ds = NULL;
        for (int i = 0; i < ComponentCount; i++)
            if (ds = dynamic_cast<TDataSet*>(Components[i]))
                if (ds->State == dsEdit)
                {
                    isChanged = true;
                    break;
                }
    }

    actLoad->Enabled = isChanged;
    actApply->Enabled = isChanged;

}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::actOkExecute(TObject *Sender)
{
    if (actApply->Enabled) {
        actApplyExecute(Sender);
    }
    Close();
}
//---------------------------------------------------------------------------


void __fastcall TfrmTxBase::actApplyExecute(TObject *Sender)
{
    try {
        TxDataSave();

        TxDataLoad();
        lblEditing->Visible = false;
        actApply->Enabled = false;
        actLoad->Enabled = false;
        frmMain->FormListTxRefresh();
        FormProvider.UpdateTransmitters(id);

    } catch (Exception &e) {
        throw *(new Exception(AnsiString("Помилка збереження: ")+e.Message));
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::actCloseExecute(TObject *Sender)
{
    Close();
}

void __fastcall TfrmTxBase::actLoadExecute(TObject *Sender)
{
    if (Application->MessageBox(AnsiString("Обновити дані передавача?").c_str(),Application->Title.c_str(), MB_ICONQUESTION | MB_YESNO) == IDYES){

          if (Tx)
              Tx->invalidate();
          else
              throw *(new Exception("Об'єкт не проініціалізований"));
      /**************************** valick ***************/
          dmMain->trMain->RollbackRetaining();
      /**************************** valick ***************/
          TxDataLoad();
          SetRadiationClass();
          actApply->Enabled = false;
          actLoad->Enabled = false;
          lblEditing->Visible = false;
    }
}

//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::ibdsStantionsBaseAfterEdit(TDataSet *DataSet)
{
    actApply->Enabled = true;
    actLoad->Enabled = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::ibdsTelecomOrgAfterEdit(TDataSet *DataSet)
{
    actApply->Enabled = true;
    actLoad->Enabled = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::ibdsTestpointAfterEdit(TDataSet *DataSet)
{
    actApply->Enabled = true;
    actLoad->Enabled = true;
}
//---------------------------------------------------------------------------


void __fastcall TfrmTxBase::ibdsLicensesAfterEdit(TDataSet *DataSet)
{
    actApply->Enabled = true;
    actLoad->Enabled = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::ibdsEquipmentAfterEdit(TDataSet *DataSet)
{
   actApply->Enabled = true;
   actLoad->Enabled = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::ibdsTestpointLATITUDEGetText(TField *Sender,
      AnsiString &Text, bool DisplayText)
{
    if (Sender->IsNull)
        Text = "";
    else
        Text = dmMain->coordToStr(Sender->AsFloat, 'Y');
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::ibdsTestpointLONGITUDEGetText(TField *Sender,
      AnsiString &Text, bool DisplayText)
{
    if (Sender->IsNull)
        Text = "";
    else
        Text = dmMain->coordToStr(Sender->AsFloat, 'X');
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::ibdsTestpointLATITUDESetText(TField *Sender,
      const AnsiString Text)
{
    double DValue;
    try {
        DValue = dmMain->strToCoord(Text);
        Sender->AsFloat = DValue;
    } catch (Exception &e) { ;
        throw *(new Exception(AnsiString("Помилка вводу!")+e.Message));
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::ibdsTestpointTESTPOINT_TYPESetText(TField *Sender, const AnsiString Text)
{
    if ( Text == "кордон" )
        Sender->AsInteger = 2;
    else if ( Text == "ретр-тор" )
        Sender->AsInteger = 1;
    else
        Sender->AsInteger = 0;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::ibdsTestpointTESTPOINT_TYPEGetText(
      TField *Sender, AnsiString &Text, bool DisplayText)
{
    if (Sender->IsNull)
        Text = "";
    else
        if (Sender->AsInteger == 2) Text = "кордон";
          else if (Sender->AsInteger == 1) Text = "ретр-тор";
          else Text = "кругова";
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::ibdsTelecomOrgBeforeDelete(TDataSet *DataSet)
{
    if (ibdsDocuments->RecordCount != 0)
        throw *(new Exception(AnsiString("Треба спочатку видалити документи, що пов'язані з організацією.")));
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::btnDelOrgClick(TObject *Sender)
{
    ibdsTelecomOrg->Delete();
}
//---------------------------------------------------------------------------


void __fastcall TfrmTxBase::actIntoProjectExecute(TObject *Sender)
{
    SendMessage(frmMain->showExplorer(), WM_LIST_ELEMENT_SELECTED, 39, ibdsStantionsBaseID->AsInteger);
    UpdateDbSectLook();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::actIntoBaseExecute(TObject *Sender)
{
    if (Application->MessageBox("Перенести передавач в базу?", Application->Title.c_str(), MB_ICONWARNING | MB_YESNO) != IDYES)

        return;

    else if (!actApply->Enabled) {

        double DValue;
        long LValue;
        int TxStereo;
        TBCTxType  TxType;
        TBCDirection TxDirect;
        TBCPolarization TxPolar;

        Tx->get_systemcast(&TxType);
        Tx->get_direction(&TxDirect);
        Tx->get_polarization(&TxPolar);
        Tx->get_monostereo_primary(&LValue); TxStereo = LValue;

        if (TxType == ttTV) {
            Tx->get_power_video(&DValue);
            if (DValue < -300)
                throw *(new Exception(AnsiString("Потужність відео = " + AnsiString(DValue)).c_str()));
            Tx->get_epr_video_max(&DValue);
            if (DValue < -300)
                throw *(new Exception(AnsiString("EBP відео макс. = " + AnsiString(DValue)).c_str()));
            if ((TxPolar == plHOR) || (TxPolar == plMIX)) {
                Tx->get_epr_video_hor(&DValue);
                if (DValue < -300) throw *(new Exception(AnsiString("EBP відео гор. = " + AnsiString(DValue)).c_str()));
            }
            if ((TxPolar == plVER) || (TxPolar == plMIX)) {
                Tx->get_epr_video_vert(&DValue);
                if (DValue < -300)
                    throw *(new Exception(AnsiString("ЕВР відео верт = " + AnsiString(DValue)).c_str()));
            }

            Tx->get_v_sound_ratio_primary(&DValue);
            if (DValue < -300)
                throw *(new Exception(AnsiString("Відношення видео/аудио = " + AnsiString(DValue)).c_str()));
            /*
            if (TxStereo == 1){
                Tx->get_v_sound_ratio_second(&DValue);
                if (DValue < -300)
                    throw *(new Exception(AnsiString("Віднош потужностей видео/звук2 = " + AnsiString(DValue)).c_str()));
                Tx->get_power_sound_second(&DValue);
                if (DValue < -300)
                    throw *(new Exception(AnsiString("Потужність звуку2 = " + AnsiString(DValue)).c_str()));
                Tx->get_epr_sound_max_second(&DValue); // [37]
                if (DValue < -300)
                    throw *(new Exception(AnsiString("ЕВР max звуку2 = " + AnsiString(DValue)).c_str()));
            }
            */
        }

        Tx->get_epr_sound_max_primary(&DValue);
        if (DValue < -300)
            throw *(new Exception(AnsiString("EBP звуку max = " + AnsiString(DValue)).c_str()));


        if ((TxPolar == plVER) || (TxPolar == plMIX)) {
            for (int i = 0; i<36; i++) {
                   Tx->get_effectpowervert(i, &DValue);
                if (DValue < -300) throw *(new Exception(AnsiString("EBP верт "+AnsiString(i*10)+"градусів = " + AnsiString(DValue)).c_str()));
            }
            Tx->get_epr_sound_vert_primary(&DValue);
            if (DValue < -300)
                throw *(new Exception(AnsiString("EBP звуку верт ="+ AnsiString(DValue)).c_str()));
            /*
            if (TxStereo != 0) {
                Tx->get_epr_sound_vert_second(&DValue);
                if (DValue < -300)
                    throw *(new Exception(AnsiString("EBP звуку2 верт = "+ AnsiString(DValue)).c_str()));
            }
            */
        }

        if ((TxPolar == plHOR) || (TxPolar == plMIX)) {
            for (int i = 0; i<36; i++) {
                Tx->get_effectpowerhor(i, &DValue);
                if (DValue < -300)
                    throw *(new Exception(AnsiString("EBP гор. "+AnsiString(i*10)+"градусів = " + AnsiString(DValue)).c_str()));
            }
            Tx->get_epr_sound_hor_primary(&DValue);
            if (DValue < -300)
                throw *(new Exception(AnsiString("EBP звуку гор. ="+ AnsiString(DValue)).c_str()));
            /*
            if (TxStereo != 0) {
                Tx->get_epr_sound_hor_second(&DValue);
                if (DValue < -300)
                    throw *(new Exception(AnsiString("EBP звуку2 гор.  ="+ AnsiString(DValue)).c_str()));
            }
            */
        }

        AnsiString string_id1, string_id2;

        ///////////////////////ADMINISTRATIONID
        bool generateId = true;
        AnsiString id = ibdsStantionsBaseADMINISTRATIONID->AsString;

        if (!id.IsEmpty()) {
            for (int i = 1; i <= id.Length(); i++) {
                if (id[i] != ' ' && id[i] != '0') {
                    generateId = false;
                    break;
                }
            }
        }

        if (generateId) {

        //  генерируем новый ADMINISTRATIONID

            int id;

            sqlNewAdminId->Close();
            sqlNewAdminId->Params->Vars[0]-> AsInteger = ibdsStantionsBaseSTAND_ID->AsInteger;
            sqlNewAdminId->ExecQuery();
            if (!sqlNewAdminId->Eof)
                string_id1 = sqlNewAdminId->Fields[0]->AsString;
            else
                string_id1 = "0";
            if (string_id1.Length() == 0)
                string_id1 = "0";
            //  ищем числовую составляющую, буквы запоминаем
            bool completed = false;
            while (!completed) {
                try {
                    id = string_id1.ToInt();
                    completed = true;
                } catch (...) {
                    //  убираем первый символ и пытаемся снова
                    if (string_id1.Length() == 0) {
                        id = 0;
                        completed = true;
                    } else {
                        string_id2 = string_id2 + string_id1[1];
                        string_id1.Delete(1,1);
                    }
                }
            }
            //  имеем префикс string_id2 и число id
            string_id1 = IntToStr(++id);
            while (sqlNewAdminId->Fields[0]->Size > string_id2.Length() + string_id1.Length())
                string_id1.Insert("0",1);
            //  итого
        }
        /////////////////////////////////////

        std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
        sql->Database = dmMain->dbMain;
        AnsiString update_sql ="update TRANSMITTERS set STATUS = 0, DATECREATE = NULL";
        if (generateId)
            update_sql += ", ADMINISTRATIONID = '" + string_id2+string_id1+"'";
        update_sql += " where ID = " + ibdsStantionsBaseID->AsString;
        sql->SQL->Text = update_sql;
        sql->ExecQuery();
        dmMain->trMain->CommitRetaining();
        TxDataLoad();
        sql->Close();
        actIntoBase->Enabled = false;
        actIntoarchives->Enabled = true;
        actIntoBeforeBase->Enabled = true;
        frmMain->FormListTxRefresh();
        TxDataSave();

        UpdateDbSectLook();
    } else  if (Application->MessageBox("Спочатку треба зберігти зміни. Зберігти зміни?", Application->Title.c_str(), MB_ICONWARNING | MB_YESNO) == IDYES) {
        TxDataSave();
        actIntoBaseExecute(this);
    }
}
//---------------------------------------------------------------------------


void __fastcall TfrmTxBase::actIntoarchivesExecute(TObject *Sender)
{
 if (Application->MessageBox("Перенести передавач в архів?", Application->Title.c_str(), MB_ICONWARNING | MB_YESNO) != IDYES)
    return;
 else if (!actApply->Enabled) {
   if (Application->MessageBox("Перенести передавач в архів?", Application->Title.c_str(), MB_ICONWARNING | MB_YESNO) == IDYES) {
      TIBSQL *sql = new TIBSQL(this);
      sql->Database = dmMain->dbMain;
      sql->SQL->Text = "update TRANSMITTERS set STATUS = -1, ORIGINALID = 0 where ID = " + ibdsStantionsBaseID->AsString;
      sql->ExecQuery();
      dmMain->trMain->CommitRetaining();
      sql->Close();
      delete sql;
      actIntoBase->Enabled = true;
      actIntoBeforeBase->Enabled = true;
      actIntoarchives->Enabled = false;
      frmMain->FormListTxRefresh();

      TxDataLoad();
  } else  if (Application->MessageBox("Спочатку треба зберігти зміни. Зберігти зміни?", Application->Title.c_str(), MB_ICONWARNING | MB_YESNO) == IDYES) {
        TxDataSave();
        actIntoarchivesExecute(this);
  }
  }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::actIntoBeforeBaseExecute(TObject *Sender)
{

    change_systemcast = false;

    TBCTxType systemcast;
    Tx->get_systemcast(&systemcast);
    switch (systemcast) {
        case ttTV:  pmIntoBeforeBase->Items->Items[0]->Caption = "Зробити копію в предбазі АТБ->АТБ";
                    pmIntoBeforeBase->Items->Items[1]->Caption = "Зробити копію в предбазі АТБ->ЦТБ";
                    pmIntoBeforeBase->Items->Items[1]->Visible = true;
                    new_systemcast = (int)ttDVB;
            break;
        case ttAM:
        case ttFM:  pmIntoBeforeBase->Items->Items[0]->Caption = "Зробити копію в предбазі АРМ->АРМ";
                    pmIntoBeforeBase->Items->Items[1]->Caption = "Зробити копію в предбазі АРМ->ЦРМ";
                    pmIntoBeforeBase->Items->Items[1]->Visible = true;
                    new_systemcast = (int)ttDAB;
            break;
        default:    new_systemcast = 0;
                    pmIntoBeforeBase->Items->Items[1]->Visible = false;
            break;
    }

    pmIntoBeforeBase->Popup(sbIntoBeforeBase->ClientOrigin.x + sbIntoBeforeBase->Width/2,
                         sbIntoBeforeBase->ClientOrigin.y + sbIntoBeforeBase->Height/2);

}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::NewTx()
{
    AnsiString TableName = "TRANSMITTERS";//ibdsStantionsBase->Fields->Fields[0]->Origin;
    // копировать значения полей в записи
    std::map<AnsiString, Variant> id;
    id["ID"] = ibdsStantionsBase->Fields->Fields[0]->AsInteger;
    std::map<AnsiString, Variant> params;
    params["STATUS"] = 1;
    params["WAS_IN_BASE"] = 0;
    params["ADMINISTRATIONID"] = Variant();

    if  (change_systemcast) {
        if (new_systemcast != 0) {
            if (!dmMain->ibdsScList->Active)
                dmMain->ibdsScList->Active = true;
            dmMain->ibdsScList->Locate("ENUMVAL", new_systemcast, TLocateOptions());
            params["SYSTEMCAST_ID"] = dmMain->ibdsScListID->AsInteger;
            if(new_systemcast == ttDVB)
            {
                params["TYPESYSTEM"] = Variant();
                params["CHANNEL_ID"] = Variant();
                params["VIDEO_CARRIER"] = Variant();
                params["VIDEO_OFFSET_LINE"] = Variant();
                params["VIDEO_OFFSET_HERZ"] = Variant();
                params["TYPEOFFSET"] = Variant();
                params["SYSTEMCOLOUR"] = Variant();
                params["POWER_VIDEO"] = Variant();
                params["EPR_VIDEO_MAX"] = Variant();
                params["EPR_VIDEO_HOR"] = Variant();
                params["EPR_VIDEO_VERT"] = Variant();
                params["POLARIZATION"] = Variant();
                params["ANTENNAGAIN"] = Variant();
                params["DIRECTION"] = Variant();
                params["ACCOUNTCONDITION_IN"] = Variant();
                params["ACCOUNTCONDITION_OUT"] = Variant();
                params["VIDEO_EMISSION"] = Variant();
                params["SOUND_CARRIER_PRIMARY"] = Variant();
                params["SOUND_OFFSET_PRIMARY"] = Variant();
                params["SOUND_EMISSION_PRIMARY"] = Variant();
                params["POWER_SOUND_PRIMARY"] = Variant();
                params["EPR_SOUND_MAX_PRIMARY"] = Variant();
                params["EPR_SOUND_HOR_PRIMARY"] = Variant();
                params["EPR_SOUND_VERT_PRIMARY"] = Variant();
                params["V_SOUND_RATIO_PRIMARY"] = Variant();
                params["MONOSTEREO_PRIMARY"] = Variant();
                params["SOUND_CARRIER_SECOND"] = Variant();
                params["SOUND_OFFSET_SECOND"] = Variant();
                params["SOUND_SYSTEM_SECOND"] = Variant();
                params["NAMEPROGRAMM"] = Variant();
                params["OWNER_ID"] = Variant();
                params["OPERATOR_ID"] = Variant();
                params["CARRIER"] = Variant();
                params["ANT_DIAG_H"] = Variant();
                params["ANT_DIAG_V"] = Variant();
                params["EMC_CONCL_NUM"] = Variant();
                params["EMC_CONCL_FROM"] = Variant();
                params["EMC_CONCL_TO"] = Variant();
                params["NUMPERMBUILD"] = Variant();
                params["DATEPERMBUILDFROM"] = Variant();
                params["NUMPERMUSE"] = Variant();
                params["DATEPERMBUILDTO"] = Variant();
                params["DATEPERMUSEFROM"] = Variant();
                params["DATEPERMUSETO"] = Variant();
                params["REMARKS"] = Variant();
                params["BANDWIDTH"] = Variant();
                params["REMARKS_ADD"] = Variant();
                params["NR_REQ_NO"] = Variant();
                params["NR_REQ_DATE"] = Variant();
                params["NR_CONCL_NO"] = Variant();
                params["NR_CONCL_DATE"] = Variant();
                params["NR_APPL_NO"] = Variant();
                params["NR_APPL_DATE"] = Variant();
            }
        } else
            return;
        change_systemcast = false;
    }

    NewElementId = dmMain->RecordCopy(TableName, id, params);
    if (NewElementId == -1)
        return;

    if(NewElementId > 0)
        dmMain->trMain->CommitRetaining();
    else
        dmMain->trMain->RollbackRetaining();

    frmMain->FormListTxRefresh();
    if (NewElementId > 0)
        FormProvider.ShowTx(txBroker.GetTx(NewElementId));
}

void __fastcall TfrmTxBase::ibdsDocumentsDOCTYPE_NAMEGetText(
      TField *Sender, AnsiString &Text, bool DisplayText)
{
    if (ibdsDocumentsDOCUMENT_ID == NULL) Text = "";
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::mniCopyToDraftAnalClick(TObject *Sender)
{
    if (!actApply->Enabled) {
        if (Application->MessageBox("Копировать текущий передатчик?", Application->Title.c_str(), MB_ICONQUESTION | MB_YESNO) != IDYES)
            return;
        change_systemcast = false;
        NewTx();
    } else  if (Application->MessageBox("Спочатку треба зберігти зміни. Зберігти зміни?", Application->Title.c_str(), MB_ICONWARNING | MB_YESNO) == IDYES) {
        TxDataSave();
        mniCopyToDraftAnalClick(this);
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmTxBase::mniCopyToDraftDigClick(TObject *Sender)
{
    if (!actApply->Enabled) {
        change_systemcast = true;
        NewTx();
    } else  if (Application->MessageBox("Спочатку треба зберігти зміни. Зберігти зміни?", Application->Title.c_str(), MB_ICONWARNING | MB_YESNO) == IDYES) {
        TxDataSave();
        mniCopyToDraftDigClick(this);
    }
}

void __fastcall TfrmTxBase::mniMoveToDraftClick(TObject *Sender)
{
    if (Application->MessageBox("Перенести передавач в предбазу?", Application->Title.c_str(), MB_ICONWARNING | MB_YESNO) != IDYES)
        return;
    else if (!actApply->Enabled) {
        TIBSQL *sql = new TIBSQL(this);
        sql->Database = dmMain->dbMain;

        sql->SQL->Text = "update TRANSMITTERS set STATUS = 1 where ID = " + ibdsStantionsBaseID->AsString;
        sql->ExecQuery();
        dmMain->trMain->CommitRetaining();
        sql->Close();
        delete sql;

        actIntoBase->Enabled = true;
        actIntoarchives->Enabled = true;
        actIntoBeforeBase->Enabled = false;
        if (Tx)
            Tx->invalidate();
        frmMain->FormListTxRefresh();
    } else  if (Application->MessageBox("Спочатку треба зберігти зміни. Зберігти зміни?", Application->Title.c_str(), MB_ICONWARNING | MB_YESNO) == IDYES) {
        TxDataSave();
        mniMoveToDraftClick(this);
    }
    frmMain->FormListTxRefresh();
    TxDataLoad();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::dtpPermUseBegChange(TObject *Sender)
{
    if (ibdsLicenses->State != dsEdit)
        ibdsLicenses->Edit();
    ibdsLicensesDATEPERMUSEFROM->AsDateTime = dtpPermUseBeg->Date;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::dtpPermUseEndChange(TObject *Sender)
{
    if (ibdsLicenses->State != dsEdit)
        ibdsLicenses->Edit();
    ibdsLicensesDATEPERMUSETO->AsDateTime = dtpPermUseEnd->Date;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::dtpDatCertificateChange(TObject *)
{
    if (ibdsLicenses->State != dsEdit)ibdsLicenses->Edit();
    ibdsLicensesDATESTANDCERTIFICATE->AsDateTime = dtpDatCertificate->Date;
}
//---------------------------------------------------------------------------


void __fastcall TfrmTxBase::dbgEquipmentEnter(TObject *Sender)
{
    if (ibdsStantionsBaseSTATUS->AsInteger != tsDraft)
        ((TDBGrid *)Sender)->ReadOnly = true;
    else
        ((TDBGrid *)Sender)->ReadOnly = false;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::dbgTestpointsEnter(TObject *Sender)
{
    if (ibdsStantionsBaseSTATUS->AsInteger != tsDraft)
        ((TDBGrid *)Sender)->ReadOnly = true;
    else
        ((TDBGrid *)Sender)->ReadOnly = false;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::btnInListClick(TObject *)
{
    unsigned int txDraft = 0x10000000;
    unsigned int txReg   = 0x20000000;
    unsigned int txDel   = 0x40000000;

    int Tag;
    if (ibdsStantionsBaseSTATUS->AsInteger == tsDeleted) Tag = txDel;
    else if (ibdsStantionsBaseSTATUS->AsInteger == tsDraft) Tag = txDraft;
    else Tag = txReg;

    TBCTxType systemcast;
    Tx->get_systemcast(&systemcast);

    Tag = Tag | (1 << systemcast);

  FormProvider.ShowTxList(0, ibdsStantionsBaseID->AsInteger, Tag);
  new TfrmSelectTxTree(ibdsStantionsBaseID->AsInteger, frmMain);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::actTxCopyExecute(TObject *)
{
//  try {
//      pmIntoBeforeBase->Items->Items[2]->Visible = false;
//      actIntoBeforeBaseExecute(this);
//  } __finally {
//      pmIntoBeforeBase->Items->Items[2]->Vismble$= true;
//  }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::actIntoListExecute(TObject *)
{
    unsigned int txDraft = 0x10000000;
    unsigned int txReg   = 0x20000000;
    unsigned int txDel   = 0x40000000;

    int Tag;
    if (ibdsStantionsBaseSTATUS->AsInteger == tsDeleted) Tag = txDel;
    else if (ibdsStantionsBaseSTATUS->AsInteger == tsDraft) Tag = txDraft;
    else Tag = txReg;

    TBCTxType systemcast;
    Tx->get_systemcast(&systemcast);

    Tag = Tag | (1 << systemcast);

  FormProvider.ShowTxList(0, ibdsStantionsBaseID->AsInteger, Tag);

}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::actIntoTreeExecute(TObject *Sender)
{
  new TfrmSelectTxTree(ibdsStantionsBaseID->AsInteger, frmMain);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::ibdsTestpointDISTANCEGetText(TField *Sender,
      AnsiString &Text, bool DisplayText)
{
  Text = FormatFloat("#.###",Sender->AsFloat);
}
//---------------------------------------------------------------------------



void __fastcall TfrmTxBase::pcDataChange(TObject *Sender)
{
    if (pcData->ActivePage == tshChangeLog) {
    
        if (!(ibdsUserActLog->Active)) {
            if (Application->MessageBox("Показати журнал змін?", Application->Title.c_str(), MB_ICONQUESTION | MB_YESNO) == IDYES) {
                ibdsUserActLog->ParamByName("ID")->AsInteger = id;
                ibdsUserActLog->Active = true;
            }
        }

    } else if (pcData->ActivePage == tshMap) {
        //  отобразить карту и зоны передатчика

        //  1. карта
        bool firstTime = false;
        try {
            cmf->Init();
            firstTime = true;
            cmf->bmf->OnToolUsed  = txAnalyzer.MapToolUsed;
        } catch (...) {}

        TCOMILISBCTx tx(Tx, true);
        WideString stationName;
        stationName.Attach(tx.station_name);

        cmf->Clear(-1);
        cmf->SetCenter(tx.longitude, tx.latitude);
        cmf->ShowStation(tx.longitude, tx.latitude, tx.station_name, tx.station_name);

        //  2. зоны
        //  теоретическая
        ILisBcLfMfPtr lfmf;
        LPSAFEARRAY zoneCover = NULL;
        if (type_form == ttAM)
        {
            Tx->QueryInterface(IID_ILisBcLfMf, (void**)&lfmf);
            zoneCover = txAnalyzer.GetEtalonZone(Tx, lfmf->is_day);
        }
        else
            txAnalyzer.GetTxZone(tx, &zoneCover);

        std::vector<double> zone;
        for (int i = 0; i < zoneCover->rgsabound[0].cElements; i++)
            zone.push_back(((double*)zoneCover->pvData)[i]);
        MapPolygon* pgn = cmf->ShowCoverageZone(tx.longitude, tx.latitude, zone);
        pgn->color = lfmf.IsBound() ? (lfmf->is_day ? clYellow : clDkGray) : clGreen;

        if (type_form == ttAM && lfmf.IsBound())
        {
            SafeArrayDestroy(zoneCover);
            if (lfmf->is_day && lfmf->night_op || !lfmf->is_day && lfmf->day_op)
            {
                lfmf->is_day = !lfmf->is_day;
                zoneCover = txAnalyzer.GetEtalonZone(Tx, lfmf->is_day);
                zone.clear();
                for (int i = 0; i < zoneCover->rgsabound[0].cElements; i++)
                    zone.push_back(((double*)zoneCover->pvData)[i]);
                MapPolygon* pgn = cmf->ShowCoverageZone(tx.longitude, tx.latitude, zone);
                pgn->color = lfmf->is_day ? clYellow : clDkGray;
                lfmf->is_day = !lfmf->is_day;
            }
        }
        // удалить массив, конечно
        SafeArrayDestroy(zoneCover);

        if ((firstTime || BCCalcParams.mapAutoFit) && (type_form == ttAM))
            cmf->bmf->FitObjects();

        //  координационная
        double zoneCoord[36];
        if (type_form == ttAM)
        {
            for (int i = 0; i < 36; i++)
                zoneCoord[i] = 10000.;
        } else {
            txAnalyzer.GetCoordinationZone(tx, zoneCoord);
            {//test коррекция координационных расстояний
                for ( int i = 0; i < 36; i++ )
                    if ( zoneCoord[i] > 2000. )
                        zoneCoord[i] = 2000.;
            }
        }

        zone.clear();
        for (int i = 0; i < 36; i++)
            zone.push_back(zoneCoord[i]);
        pgn = cmf->ShowCoordZone(tx.longitude, tx.latitude, zone);
        pgn->color = clRed;

        if ((firstTime || BCCalcParams.mapAutoFit) && (type_form != ttAM))
            cmf->bmf->FitObjects();

    } else if (pcData->ActivePage == tshCoordination) {

        ibqDocType->Active = false;
        ibqDocType->Active = true;

        ibdsTelecomOrg->Active = false;
        ibdsTelecomOrg->ParamByName("TRANSMITTER_ID")->AsInteger = ibdsStantionsBaseID->AsInteger;
        ibdsTelecomOrg->Active = true;

        ibdsDocuments->Active = false;
        ibdsDocuments->ParamByName("TRANSMITTERS_ID")->AsInteger = ibdsStantionsBaseID->AsInteger;
        ibdsDocuments->ParamByName("TELECOMORGANIZATION_ID")->AsInteger = ibdsTelecomOrgTELECOMORG_ID->AsInteger;
        ibdsDocuments->Active = true;

    } else if (pcData->ActivePage == tshLicenses) {

        ibdsLicenses->Active = true;

        // tsh children read their state from resource stream only when tsh is first time shown
        objGrdLic->RecreateHeader();
        objGrdLic->Refresh();

        if ((ibdsLicensesDATEINTENDUSE->IsNull)||(!ibdsLicensesDATEINTENDUSE->AsFloat))
            dtIntoUse->Date = Date();
        else
            dtIntoUse->Date = ibdsLicensesDATEINTENDUSE->AsDateTime;

        if ((ibdsLicensesD_EXPIRY->IsNull)||(!ibdsLicensesD_EXPIRY->AsFloat))
            dtExpired->Date = Date();
        else
            dtExpired->Date = ibdsLicensesD_EXPIRY->AsDateTime;

        if ((ibdsLicensesDATEPERMUSEFROM->IsNull)||(!ibdsLicensesDATEPERMUSEFROM->AsFloat))
            dtpPermUseBeg->Date = Date();
        else
            dtpPermUseBeg->Date = ibdsLicensesDATEPERMUSEFROM->AsDateTime;

        if ((ibdsLicensesDATEPERMUSETO->IsNull)||(!ibdsLicensesDATEPERMUSETO->AsFloat))
            dtpPermUseEnd->Date = Date();
        else
            dtpPermUseEnd->Date = ibdsLicensesDATEPERMUSETO->AsDateTime;

        if ((ibdsLicensesDATESTANDCERTIFICATE->IsNull)||(!ibdsLicensesDATESTANDCERTIFICATE->AsFloat))
            dtpDatCertificate->Date = Date();
        else
            dtpDatCertificate->Date = ibdsLicensesDATESTANDCERTIFICATE->AsDateTime;

        if(ibdsLicensesNR_REQ_DATE->AsFloat == NULL)
            dtpNrReq->Date = Now();
        else
            dtpNrReq->Date = ibdsLicensesNR_REQ_DATE->AsDateTime;
        if(ibdsLicensesNR_CONCL_DATE->AsFloat == NULL)
            dtpNrConcl->Date = Now();
        else
            dtpNrConcl->Date = ibdsLicensesNR_CONCL_DATE->AsDateTime;
        if(ibdsLicensesNR_APPL_DATE->AsFloat == NULL)
            dtpNrAppl->Date = Now();
        else
            dtpNrAppl->Date = ibdsLicensesNR_APPL_DATE->AsDateTime;
        if(ibdsLicensesEMC_CONCL_TO->AsFloat == NULL)
            dtpEmsDateEnd->Date = Now();
        else
            dtpEmsDateEnd->Date = ibdsLicensesEMC_CONCL_TO->AsDateTime;
        if(ibdsLicensesEMC_CONCL_FROM->AsFloat == NULL)
            dtpEmsDateBeg->Date = Now();
        else
            dtpEmsDateBeg->Date = ibdsLicensesEMC_CONCL_FROM->AsDateTime;
       
    }
}
//---------------------------------------------------------------------------



void __fastcall TfrmTxBase::ibqStandCalcFields(TDataSet *DataSet)
{
    ibqStandFULL_ADDR->AsString = Trim(ibqStandSTREET_NAME->AsString)+", "+
                               Trim(ibqStandADDRESS->AsString);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::ibdsDocumentsFilterRecord(TDataSet *DataSet,
      bool &Accept)
{
    if (reply_required)
        Accept = Now()+3.0 > ibdsDocumentsANSWERDATE->AsDateTime;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::btnReplyRequiredClick(TObject *Sender)
{
    ibdsDocuments->Filtered = false;
    ibdsDocuments->Filtered = true;
    btnReplyRequired->Caption = reply_required ? "Без відповіді" : "Всі докум-ти";
    reply_required = !reply_required;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::actPlanningExecute(TObject *Sender)
{
    if ( txAnalyzer.planningTx.IsBound() )
    {
        if ( txAnalyzer.planningTx.id != id )
            if ( !txAnalyzer.isNewPlan && txAnalyzer.wasChanges )
            {
                int reply = Application->MessageBox("Зберігти результати поточного планування?", Application->Title.c_str(), MB_ICONQUESTION | MB_YESNOCANCEL);
                switch (reply)
                {
                    case IDYES:
                        txAnalyzer.SaveToDb();
                        break;
                    case IDCANCEL:
                        //FormProvider.ShowPlanning();
                        return;
                    default:
                        break;
                }
            }
    }
    else
    {
        txAnalyzer.planningTx.Bind(txBroker.GetTx(id), true);
        txAnalyzer.LoadFromDb();
    }

    FormProvider.ShowPlanning();
    if (txAnalyzer.planVector.empty())
        txAnalyzer.PerformPlanning(id);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::actExaminationExecute(TObject *Sender)
{
    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
      sql->Database = dmMain->dbMain;
      sql->SQL->Text = "select ID, CREATEDATE, NAMEQUERIES from SELECTIONS where TRANSMITTERS_ID = " + edtID->Text + " and SELTYPE = 'E' order by NAMEQUERIES";
      sql->Transaction = dmMain->trMain;
    sql->ExecQuery();

    popupMenu->Items->Clear();
    while (!sql->Eof)
    {//заполнение меню экспертизами
        TMenuItem *menuItem = new TMenuItem(popupMenu);

        menuItem->Caption = sql->FieldByName("NAMEQUERIES")->AsString + " ["+sql->FieldByName("CREATEDATE")->AsString + "]";
        menuItem->OnClick = MenuItem_OnClick;
        menuItem->Tag = sql->FieldByName("ID")->AsInteger;
        popupMenu->Items->Add(menuItem);

        sql->Next();
    }
    sql->Close();

    if ( popupMenu->Items->Count > 0 )
    {
        TMenuItem *menuItem = new TMenuItem(popupMenu);
        menuItem->Caption = "-";
        popupMenu->Items->Add(menuItem);
    }

    {
        TMenuItem *menuItem = new TMenuItem(popupMenu);
        menuItem->Caption = "Експертиза...";
        menuItem->OnClick = MenuItem_OnClick;
        menuItem->Tag = 0;
        popupMenu->Items->Add(menuItem);
    }

    POINT point;
    if ( GetCursorPos(&point) != 0 )
        popupMenu->Popup(point.x, point.y);
}
//---------------------------------------------------------------------------
void __fastcall TfrmTxBase::MenuItem_OnClick(TObject *Sender)
{
    if ( dynamic_cast<TMenuItem *>(Sender)->Tag != 0 )
    {
        TfrmSelection* fs = NULL;
        for (int i = 0; i < frmMain->MDIChildCount; i++)
            if ((fs = dynamic_cast<TfrmSelection*>(frmMain->MDIChildren[i])) && (dynamic_cast<TMenuItem *>(Sender)->Tag == fs->GetId()) )
                fs->Show();

        //  если нет, то создать
        TempCursor tc(crHourGlass);

        fs = new TfrmSelection(Application, (void*)(dynamic_cast<TMenuItem *>(Sender)->Tag));
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
    }
    else
    {
        txAnalyzer.MakeNewSelection(id, nsExpertise);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::dtpNrReqChange(TObject *Sender)
{
    ibdsLicenses->Edit();
    ibdsLicensesNR_REQ_DATE->AsDateTime = dtpNrReq->Date;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::dtpNrConclChange(TObject *Sender)
{
    ibdsLicenses->Edit();
    ibdsLicensesNR_CONCL_DATE->AsDateTime = dtpNrConcl->Date;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::dtpNrApplChange(TObject *Sender)
{
    ibdsLicenses->Edit();
    ibdsLicensesNR_APPL_DATE->AsDateTime = dtpNrAppl->Date;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::dtpEmsDateEndChange(TObject *Sender)
{
    ibdsLicenses->Edit();
    ibdsLicensesEMC_CONCL_TO->AsDateTime = dtpEmsDateEnd->Date;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::dtpEmsDateBegChange(TObject *Sender)
{
    ibdsLicenses->Edit();
    ibdsLicensesEMC_CONCL_FROM->AsDateTime = dtpEmsDateBeg->Date;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::btResLicClick(TObject *Sender)
{
    FormProvider.ShowList(otLICENSES, this->Handle, 0, " O."+GetOwnerFilter());
    lastLicCaller = btResLic;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::dtpResLicDateBegChange(TObject *Sender)
{
    //todo: ???
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::dtpResLicDateEndChange(TObject *Sender)
{
    //todo: ???
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::btnFieldListClick(TObject *Sender)
{
    // list of available fields of Report (out document)
    TempCursor tc(crHourGlass);

    Variant xl = CreateOleObject("Excel.Application");
    xl.OlePropertyGet("Application").OlePropertySet<long>("SheetsInNewWorkbook", 1);
    xl.OlePropertyGet("Workbooks").OleProcedure("Add");
    xl.OlePropertyGet<WideString>("Rows", "1:1").OleProcedure("Select");
    xl.OlePropertyGet("Selection").OlePropertyGet("Font").OlePropertySet<VARIANT_BOOL>("Bold", true);
    xl.OlePropertyGet<WideString>("Range", L"A1").OleProcedure("Select");
    xl.OlePropertyGet("ActiveCell").OlePropertySet<WideString>("FormulaR1C1", L"Закладка");
    xl.OlePropertyGet<WideString>("Range", L"B1").OleProcedure("Select");
    xl.OlePropertyGet("ActiveCell").OlePropertySet<WideString>("FormulaR1C1", L"Поле");
    xl.OlePropertyGet<WideString>("Range", L"C1").OleProcedure("Select");
    xl.OlePropertyGet("ActiveCell").OlePropertySet<WideString>("FormulaR1C1", L"Тип");
    xl.OlePropertyGet<WideString>("Range", L"D1").OleProcedure("Select");
    xl.OlePropertyGet("ActiveCell").OlePropertySet<WideString>("FormulaR1C1", L"Длина строки");

    xl.OlePropertyGet<WideString>("Columns", L"A:A").OlePropertySet<double>("ColumnWidth", 35.0);
    xl.OlePropertyGet<WideString>("Columns", L"B:B").OlePropertySet<double>("ColumnWidth", 43.0);
    xl.OlePropertyGet<WideString>("Columns", L"C:C").OlePropertySet<double>("ColumnWidth", 13.0);

    for (int i = 0; i < dmMain->ibqTxOuery->Fields->Count; i++)
    {
        TField *fld = dmMain->ibqTxOuery->Fields->Fields[i];
        xl.OlePropertyGet<WideString>("Range", String("A")+IntToStr(i+2)).OleProcedure("Select");
        xl.OlePropertyGet("ActiveCell").OlePropertySet<WideString>("FormulaR1C1", WideString(fld->FieldName));
        xl.OlePropertyGet<WideString>("Range", String("B")+IntToStr(i+2)).OleProcedure("Select");
        xl.OlePropertyGet("ActiveCell").OlePropertySet<WideString>("FormulaR1C1", WideString(fld->Origin));
        xl.OlePropertyGet<WideString>("Range", String("C")+IntToStr(i+2)).OleProcedure("Select");
        xl.OlePropertyGet("ActiveCell").OlePropertySet<WideString>("FormulaR1C1",
                fld->FieldKind == fkCalculated ? L"Вычисляемое" : L"Из запроса");
        if (fld->DataType == ftString)
        {
            xl.OlePropertyGet<WideString>("Range", String("D")+IntToStr(i+2)).OleProcedure("Select");
            xl.OlePropertyGet("ActiveCell").OlePropertySet<WideString>("FormulaR1C1", WideString(IntToStr(fld->Size)));
        }
    }
    xl.OlePropertyGet<WideString>("Range", L"A1").OleProcedure("Select");
    xl.OlePropertySet<VARIANT_BOOL>("Visible", true);
    xl = Unassigned;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::btnAttachClick(TObject *Sender)
{
    FormProvider.ShowList(otLICENSES, this->Handle, 0, " O."+GetOwnerFilter());
    lastLicCaller = btnAttach;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::btnDetachClick(TObject *Sender)
{
    int lic_id = objGrdLic->GetCurrentId();
    if (lic_id > 0 && MessageBox(NULL, "Освободить лицензию?", "Подтверждение",
                                MB_ICONQUESTION | MB_YESNO) == IDYES)
    {
        dmMain->RunQuery(String("delete from NR_LIC_LINK where TX_ID = ")+id+" and LIC_ID = "+lic_id, ParamList(), tr);
        objGrdLic->Refresh();
        ibdsLicenses->Edit(); // remain it in Edit state to enable Apply action
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::btPermOwnerClick(TObject *Sender)
{
    FormProvider.ShowList(otOWNER, this->Handle, 0);
}
//---------------------------------------------------------------------------

String __fastcall TfrmTxBase::GetOwnerFilter()
{
    String serviceFilter;

    TBCTxType systemcast;
    Tx->get_systemcast(&systemcast);
    switch (systemcast)
    {
        case ttTV: serviceFilter = "AVB"; break;
        case ttAM:
        case ttFM: serviceFilter = "AAB"; break;
        case ttDVB: serviceFilter = "DVB"; break;
        case ttDAB: serviceFilter = "DAB"; break;
    }

    if (serviceFilter.Length() > 0)
        serviceFilter = serviceFilter+" = 1 ";

    return serviceFilter;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::dtIntoUseChange(TObject *Sender)
{
    ibdsLicenses->Edit();
    ibdsLicensesDATEINTENDUSE->AsDateTime = dtIntoUse->Date;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::dtExpiredChange(TObject *Sender)
{
    ibdsLicenses->Edit();
    ibdsLicensesD_EXPIRY->AsDateTime = dtExpired->Date;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBase::CreateDoc(TObject *Sender)
{
    if (actApply->Enabled)
    {
        if (MessageBox(NULL, "Треба зберiгти змiни в картцi передавача.\n"
                             "Виповнити збереження?", "LISBC", MB_ICONQUESTION | MB_YESNO) == IDNO)
            throw *(new Exception("При генерации документов изменения должны быть сохранены"));
        actApplyExecute(Sender);
    }

    std::auto_ptr<TfrmDocumentsSettings> frm(new TfrmDocumentsSettings(Application));
    std::auto_ptr<TDataSet> ds(dmMain->GetObject(otDocument, 0, frm->tr));
    ds->FieldByName("TELECOMORGANIZATION_ID")->AsInteger = ibdsTelecomOrgTELECOMORG_ID->AsInteger;
    ds->FieldByName("TRANSMITTERS_ID")->AsInteger = ibdsStantionsBaseID->AsInteger;
    ds->FieldByName("LETTERS_ID")->AsInteger = (Sender == btnDocAnswer) ? ibdsDocumentsID->AsInteger : 0;
    ds->FieldByName("TYPELETTER")->AsInteger = 0;
    ds->FieldByName("CREATEDATEOUT")->AsDateTime = Date();
    ds->FieldByName("ANSWERIS")->AsInteger = 0;

    frm->dscObj->DataSet = ds.get();
    frm->Caption = FormProvider.GetObjectName(otDocument);
    frm->objId = ds->FieldByName("ID")->AsInteger;
    if (frm->ShowModal() == mrOk)
    {
        ibdsLicenses->Close();
        ibdsLicenses->Open();
        ibdsDocuments->Close();
        ibdsDocuments->Open();
        ibdsDocuments->Locate("ID", ds->FieldByName("ID")->AsInteger, TLocateOptions());
    }
}
