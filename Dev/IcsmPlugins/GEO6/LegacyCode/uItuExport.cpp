//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uItuExport.h"
#include "uMainDm.h"
#include <Dialogs.hpp>
#include <memory>
#include <fstream>
#include <IniFiles.hpp>
#include <DBTables.hpp>
#include "tempcursor.h"
#include "CoordConv.hpp"
#include <iomanip>
#include "LISBCTxServer_TLB.h"
//---------------------------------------------------------------------------

#pragma package(smart_init)
#pragma resource "*.dfm"

static char *expToGa1Main = "select t1.ID, t1.CTRY t_ctry, t1.CONTOUR_ID t_contour_id, t1.NB_TEST_PTS t_nb_test_pts "
"from DIG_CONTOUR t1 ";

static char *expToGa1Det1 = "select p.CONTOUR_ID, p.POINT_NO, p.LAT, p.LON "
"from DIG_SUBAREAPOINT p "
"where CONTOUR_ID = :ID ";

static char *expToGs1Gt1Main = "SELECT t1.ID ID, sc.ENUMVAL t_notice_type, 'GE06D' T_FRAGMENT, t1.IS_PUB_REQ, "
"'TRUE' T_IS_PUB_REQ, t1.ADM_REF_ID T_ADM_REF_ID, t1.ADM_REF_ID T_TRG_ADM_REF_ID, t1.PLAN_ENTRY T_PLAN_ENTRY, "
"t1.ASSGN_CODE T_ASSGN_CODE, t1.ASSOCIATED_ADM_ALLOT_ID T_ASSOCIATED_ADM_ALLOT_ID, "
"t1.ASSOCIATED_ALLOT_SFN_ID T_ASSOCIATED_ALLOT_SFN_ID, SFN.SYNHRONETID T_SFN_ID, t1.CALL_SIGN T_CALL_SIGN, "
"t1.CARRIER T_FREQ_ASSGN, t1.VIDEO_OFFSET_HERZ T_OFFSET, t1.DATEINTENDUSE T_D_INUSE, t1.D_EXPIRY T_D_EXPIRY, "
"S.NAMESITE_ENG T_SITE_NAME, COUNTRY.CODE T_CTRY, S.LONGITUDE T_LONG, S.LATITUDE T_LAT, 'RPC' || cast(t1.RPC+1 as char(1)) t_ref_plan_cfg, "
"dts.NAMESYSTEM t_sys_var, t1.RX_MODE T_RX_MODE, t1.SPECT_MASK T_SPECT_MASK, "
"t1.EPR_SOUND_HOR_PRIMARY t_erp_h_dbkw, t1.EPR_SOUND_VERT_PRIMARY t_erp_v_dbkw, "
"t1.EPR_VIDEO_HOR t_erp_h_dbkw_video, t1.EPR_VIDEO_VERT t_erp_v_dbkw_video, "
"t1.DIRECTION T_ANT_DIR, t1.POLARIZATION T_POLAR, t1.HEIGHTANTENNA T_HGT_AGL, "
"S.HEIGHT_SEA T_SITE_ALT, t1.OP_AGCY T_OP_AGCY, "
"t1.ADDR_CODE T_ADDR_CODE, "
"t1.OP_HH_FR T_OP_HH_FR, "
"t1.OP_HH_TO T_OP_HH_TO, t1.IS_RESUB T_IS_RESUB, t1.REMARK_CONDS_MET T_REMARK_CONDS_MET, "
"t1.SIGNED_COMMITMENT T_SIGNED_COMMITMENT, t1.REMARKS T_REMARKS, t1.EFFECTHEIGHT, t1.ANT_DIAG_H, t1.ANT_DIAG_V, "
"t1.COORD, t1.CARRIER, t1.HEIGHT_EFF_MAX t_eff_hgtmax, t1.ANTENNAGAIN, "
"t1.VIDEO_OFFSET_HERZ t_oset_v_hz, "
"t1.VIDEO_OFFSET_LINE t_oset_v_12, "
"t1.SOUND_OFFSET_PRIMARY t_oset_s_khz, "
"t1.FREQSTABILITY t_freq_stabl, "
"t1.SYSTEMCOLOUR t_color, "
"t1.V_SOUND_RATIO_PRIMARY t_pwr_ratio, "
"ats.NAMESYSTEM t_tran_sys "

"FROM TRANSMITTERS t1 "
"   LEFT OUTER JOIN SYSTEMCAST sc ON (t1.SYSTEMCAST_ID = sc.ID) "
"   LEFT OUTER JOIN STAND S ON (t1.STAND_ID = S.ID) "
"   LEFT OUTER JOIN AREA ON (S.AREA_ID = AREA.ID) "
"   LEFT OUTER JOIN COUNTRY ON (AREA.COUNTRY_ID = COUNTRY.ID) "
"   LEFT OUTER JOIN SYNHROFREQNET SFN ON (t1.IDENTIFIERSFN = SFN.ID) "
"   LEFT OUTER JOIN DIGITALTELESYSTEM dts ON (t1.TYPESYSTEM = dts.ID) "
"   LEFT OUTER JOIN ANALOGTELESYSTEM ats ON (t1.TYPESYSTEM = ats.ID) ";

static char *expToGt2Gs2Main = "SELECT t1.ID ID, t1.NOTICE_TYPE T_NOTICE_TYPE, 'GE06D' T_FRAGMENT, "
"t1.IS_PUB_REQ t_IS_PUB_REQ, "
"t1.ADM_REF_ID t_ADM_REF_ID, t1.ADM_REF_ID t_trg_ADM_REF_ID, t1.PLAN_ENTRY t_PLAN_ENTRY, "
"sfn.SYNHRONETID T_SFN_ID, t1.FREQ_ASSIGN t_FREQ_ASSIGN, t1.OFFSET t_OFFSET, t1.D_EXPIRY t_D_EXPIRY, "
"t1.ALLOT_NAME t_ALLOT_NAME, t1.CTRY t_CTRY, t1.GEO_AREA t_GEO_AREA, t1.NB_SUB_AREAS t_NB_SUB_AREAS, "
"t1.REF_PLAN_CFG t_REF_PLAN_CFG, t1.TYP_REF_NETWK t_TYP_REF_NETWK, t1.SPECT_MASK t_SPECT_MASK, t1.POLAR t_POLAR, "
"t1.REMARKS1 t_REMARKS, t2.COORD "
"FROM DIG_ALLOTMENT t1 "
"   LEFT OUTER JOIN SYNHROFREQNET sfn ON (t1.SFN_ID_FK = sfn.ID) "
"   LEFT OUTER JOIN TRANSMITTERS t2 ON (t1.ID = t2.ID) ";

static char *expToGt2Gs2Det1 = "SELECT c.CONTOUR_ID "
"FROM DIG_ALLOT_CNTR_LNK l "
"   LEFT OUTER JOIN DIG_CONTOUR c ON (l.CNTR_ID = c.ID) "
"   WHERE l.ALLOT_ID = :ID ";

static const char* modeNames[] =
{
     "GA1"
    ,"GS1/GT1/G02"
    ,"GS2/GT2"
};


__fastcall TfrmRrc06Export::TfrmRrc06Export(TComponent* Owner) : TForm(Owner)
{
    sgHead->ColWidths[0] = sgHead->ClientWidth / 3 - 1;
    sgHead->ColWidths[1] = sgHead->ClientWidth / 3 * 2;
    sgHead->Cells[0][0] = " Параметр";
    sgHead->Cells[1][0] = " Значення";

    LoadConf();
    exported = false;
}

__fastcall TfrmRrc06Export::~TfrmRrc06Export()
{
    if (exported)
        SaveConf();
}

int __fastcall TfrmRrc06Export::ExportRrc006FromGrid(ExportMode type, TDBGrid* grid)
{
    TempCursor tempCursor(crHourGlass);

    std::vector<int> ids;
    TIBDataSet *ds = dynamic_cast<TIBDataSet*>(grid->DataSource->DataSet);

    TBookmark bm = ds->GetBookmark();
    TBookmarkList *bml = grid->SelectedRows;

    for (int i = 0; i < bml->Count; i++)
    {
        ds->GotoBookmark((void *)bml->Items[i].c_str());
        ids.push_back(ds->Fields->Fields[0]->AsInteger);
    }

    if (bml->Count == 0)
    {
        std::auto_ptr<TIBSQL> sql(new TIBSQL(Application));
        sql->Database = ds->Database;
        sql->Transaction = ds->Transaction;
        sql->SQL->Text = ds->SelectSQL->Text;
        sql->Prepare();
        for (int i = 0; i < ds->Params->Count; i ++)
            sql->Params->Vars[i]->AsVariant = ds->Params->Vars[i]->AsVariant;
        for (sql->ExecQuery(); !sql->Eof; sql->Next())
            ids.push_back(sql->Fields[0]->AsInteger);
    }

    ds->GotoBookmark(bm);
    ds->FreeBookmark(bm);

    return TfrmRrc06Export::ExportRrc006(type, ids);
}

int __fastcall TfrmRrc06Export::ExportRrc006(ExportMode type, std::vector<int> ids)
{
    if (ids.empty())
        throw *(new Exception("Список идентификаторов пуст"));

    static TfrmRrc06Export *frm = NULL;
    if (frm == NULL)
        frm = new TfrmRrc06Export(Application);

    frm->Caption = AnsiString("Експорт ") + modeNames[type];
    if (type == emGa1 && frm->cbxAction->ItemIndex == 1)
        frm->cbxAction->ItemIndex = 0;

    if (type == emGa1 || type == emGs2Gt2)
    {
        frm->rgArt->ItemIndex = 0;
        frm->rgArt->Enabled = false;
    } else {
        frm->rgArt->Enabled = true;
    }

    if (frm->ShowModal() != mrOk)       
        return 0;

    if (type == emGa1 && frm->cbxAction->Text == "MODIFY")
        throw *(new Exception("Для GA1 t_action не может быть равен \"MODIFY\""));
    if ((type == emGs1Gt1 || type == emGs2Gt2) && frm->cbxAction->Text == "SUPPRESS")
        throw *(new Exception("Для этого формата t_action не может быть равен \"SUPPRESS\""));

    std::auto_ptr<TIBSQL> sql (new TIBSQL(Application));
    sql->Database = dmMain->dbMain;
    std::auto_ptr<TIBSQL> sqlAux (new TIBSQL(Application));
    sqlAux->Database = dmMain->dbMain;
    AnsiString query;
    switch (type)
    {
        case emGa1: query = expToGa1Main;       sqlAux->SQL->Text = expToGa1Det1;   break;
        case emGs1Gt1: query = expToGs1Gt1Main;                                     break;
        case emGs2Gt2: query = expToGt2Gs2Main; sqlAux->SQL->Text = expToGt2Gs2Det1;break;
        default: throw *(new Exception("Неизвестный формат экспорта"));
    }
    query += " where t1.ID in (" + IntToStr(ids[0]);
    std::vector<int>::iterator vi = ids.begin();
    for (vi++; vi < ids.end(); vi++)
        query = query + ',' + IntToStr(*vi);

    query += ')';

    sql->SQL->Text = query;
    sql->ExecQuery();
    if (sql->Eof)
        throw *(new Exception("Ни одного объекта для экспорта"));
    if (sqlAux->SQL->Text.Length() > 0)
        sqlAux->Prepare();

    std::ofstream of(frm->edtFileName->Text.c_str());
    if (!of.is_open() || of.bad())
        throw *(new Exception("Ошибка создания файла '"+frm->edtFileName->Text+'\''));

    // Comment
    AnsiString comment = frm->memComment->Lines->Text.Trim();
    if (comment.Length() > 0)
        of << comment.c_str() << '\n';
    // <HEAD>
    of << "<HEAD>\n";
    for (int i = 0; i < frm->sgHead->RowCount - 1; i++)
        of << frm->sgHead->Cells[0][i + 1].c_str() << " = " << frm->sgHead->Cells[1][i + 1].c_str() << '\n';
    of << "</HEAD>\n";

    std::auto_ptr<TCoordinateConvertor> cc(new TCoordinateConvertor(frm));
    cc->NoDividers = true;
    cc->SignMandatory = true;

    int notices = 0;

    TempVal<char> tempDecSep(DecimalSeparator, '.');

    for (; !sql->Eof; sql->Next(), notices++)
    {
        of << "<NOTICE>\n";
        if (type == emGa1)
        {
            of << "t_notice_type = GA1\n"
               << "t_action = "         << frm->cbxAction->Text.c_str() << '\n'
               << "t_ctry = "           << sql->FieldByName("t_ctry")->AsString.c_str() << '\n'
               << "t_contour_id = "     << sql->FieldByName("t_contour_id")->AsString.c_str() << '\n'
               << "t_nb_test_pts = "    << sql->FieldByName("t_nb_test_pts")->AsInteger << '\n';

            sqlAux->Close();
            sqlAux->Params->Vars[0]->AsInteger = sql->Fields[0]->AsInteger;
            for (sqlAux->ExecQuery(); !sqlAux->Eof; sqlAux->Next())
                of << "<POINT>\n"
                   << "t_long = " << cc->CoordToStr(sqlAux->FieldByName("LON")->AsDouble, 'X').c_str() << '\n'
                   << "t_lat = " << cc->CoordToStr(sqlAux->FieldByName("LAT")->AsDouble, 'Y').c_str() << '\n'
                   << "</POINT>\n";
        }
        else if (type == emGs1Gt1)
        {
            int sc = sql->FieldByName("t_notice_type")->AsInteger;
            char* noticeType = sc == ttDAB ? "GS1" : sc == ttDVB ? "GT1" : sc == ttTV ? "G02" : "cannot determine Tx systemcast";
            int rm = sql->FieldByName("t_rx_mode")->AsInteger;
            String rxMode = rm == rmMo ? "MO" : rm == rmPi ? "PI" : rm == rmPo ? "PO" : "FX";
            bool vert = !sql->FieldByName("t_POLAR")->IsNull && (sql->FieldByName("t_POLAR")->AsString[1] == 'V' || sql->FieldByName("t_POLAR")->AsString[1] == 'M');
            bool hor  = !sql->FieldByName("t_POLAR")->IsNull && (sql->FieldByName("t_POLAR")->AsString[1] == 'H' || sql->FieldByName("t_POLAR")->AsString[1] == 'M');
            double dtUse = sql->FieldByName("t_d_inuse")->AsDate;
            double dtExp = sql->FieldByName("t_d_expiry")->AsDate;
            String dir = sql->FieldByName("t_ant_dir")->AsString.Trim();
            String rpc = sql->FieldByName("t_ref_plan_cfg")->AsString.Trim();
            String fragment = frm->rgArt->ItemIndex == 0 ? "GE06D" : "NTFD_RR";

            of << "t_notice_type = "              << noticeType << '\n'
               << "t_action = "                   << frm->cbxAction->Text.c_str() << '\n';
            if (frm->rgArt->ItemIndex == 0) // Art.4
                of << "t_is_pub_req = "           << (sql->FieldByName("t_is_pub_req")->AsString == "" ?
                                                      frm->cbIsPubReq->Text.c_str() :
                                                      sql->FieldByName("t_is_pub_req")->AsString.c_str())             << '\n';
            //of << "t_fragment = "                 << sql->FieldByName("t_fragment")->AsString.c_str()                 << '\n'
            of << "t_fragment = "                 << fragment.c_str()                                                 << '\n'
               << "t_adm_ref_id = "               << sql->FieldByName("t_adm_ref_id")->AsString.c_str()               << '\n';
            if (frm->cbxAction->Text == "MODIFY")
                of << "t_trg_adm_ref_id = "       << sql->FieldByName("t_adm_ref_id")->AsString.c_str()               << '\n';
            if (sc == ttDAB || sc == ttDVB)
            {
               of << "t_plan_entry = "               << sql->FieldByName("t_plan_entry")->AsString.c_str()               << '\n'
               << "t_assgn_code = "               << sql->FieldByName("t_assgn_code")->AsString.c_str()               << '\n'
               << "t_associated_adm_allot_id = "  << sql->FieldByName("t_associated_adm_allot_id")->AsString.c_str()  << '\n'
               << "t_associated_allot_sfn_id = "  << sql->FieldByName("t_associated_allot_sfn_id")->AsString.c_str()  << '\n'
               << "t_sfn_id = "                   << sql->FieldByName("t_sfn_id")->AsString.c_str()                   << '\n';
            }
            if (frm->rgArt->ItemIndex == 1) // Art.5
                of << "t_call_sign = "            << sql->FieldByName("t_call_sign")->AsString.c_str()                << '\n';
            of << "t_freq_assgn = "               << sql->FieldByName("CARRIER")->AsString.c_str()                    << '\n';
            if (sc == ttDAB || sc == ttDVB)
            {
               of << "t_offset = "                   << FloatToStr(sql->FieldByName("t_offset")->AsInteger / 1000.).c_str()                   << '\n';
            }
            of << "t_d_inuse = "                  << (dtUse > 0 ? FormatDateTime("yyyy-mm-dd\n", dtUse).c_str() : "\n")
               << "t_d_expiry = "                 << (dtExp > 0 ? FormatDateTime("yyyy-mm-dd\n", dtExp).c_str() : "\n")
               << "t_site_name = "                << sql->FieldByName("t_site_name")->AsString.c_str()                << '\n'
               << "t_ctry = "                     << sql->FieldByName("t_ctry")->AsString.c_str()                     << '\n'
               << "t_long = "                     << cc->CoordToStr(sql->FieldByName("t_long")->AsDouble, 'X').c_str()<< '\n'
               << "t_lat = "                      << cc->CoordToStr(sql->FieldByName("t_lat")->AsDouble, 'Y').c_str() << '\n';

            if (sc == ttDAB || sc == ttDVB)
            {
               of  << (sc == ttDAB || frm->rgArt->ItemIndex == 0 ? ("t_ref_plan_cfg = " + rpc + '\n').c_str() : "")
               << (sc == ttDVB && (frm->rgArt->ItemIndex == 1 || rpc.IsEmpty()) ? ("t_sys_var = " + sql->FieldByName("t_sys_var")->AsString + '\n').c_str() : "")
               << (sc == ttDVB && (frm->rgArt->ItemIndex == 1 || rpc.IsEmpty()) ? ("t_rx_mode = " + rxMode + '\n').c_str() : "")
               << "t_spect_mask = "               << sql->FieldByName("t_spect_mask")->AsString.c_str()               << '\n';
               of << (hor ?  AnsiString().sprintf("t_erp_h_dbw = %.2f\n", sql->FieldByName("t_erp_h_dbkw")->AsDouble).c_str() : "")
               << (vert ? AnsiString().sprintf("t_erp_v_dbw = %.2f\n", sql->FieldByName("t_erp_v_dbkw")->AsDouble).c_str() : "");
            }
            if (sc == ttTV)
            {
                of << (hor ?  AnsiString().sprintf("t_erp_h_dbw = %.2f\n", sql->FieldByName("t_erp_h_dbkw_video")->AsDouble + 30.0).c_str() : "")
                << (vert ? AnsiString().sprintf("t_erp_v_dbw = %.2f\n", sql->FieldByName("t_erp_v_dbkw_video")->AsDouble + 30.0).c_str() : "");
                if (!sql->FieldByName("t_oset_v_hz")->IsNull)
                {
                    double khz = sql->FieldByName("t_oset_v_hz")->AsDouble / 1000;
                    of << "t_oset_v_khz = " << FloatToStr(khz).c_str() << '\n';
                    of << "t_oset_v_12 = " << IntToStr(int(khz / 1.3 * 12)).c_str() << '\n';
                } else {
                    double lines = sql->FieldByName("t_oset_v_12")->AsInteger;
                    of << "t_oset_v_12 = " << IntToStr(int(lines * 12)).c_str() << '\n';
                    of << "t_oset_v_khz = " << FloatToStr(lines * 1.3).c_str() << '\n';
                }
                of << "t_oset_s_khz = " << sql->FieldByName("t_oset_s_khz")->AsString.c_str() << '\n';
                of << "t_freq_stabl = " << sql->FieldByName("t_freq_stabl")->AsString.c_str() << '\n';
                of << "t_pwr_ratio = " << sql->FieldByName("t_pwr_ratio")->AsString.c_str() << '\n';
                of << "t_tran_sys = " << sql->FieldByName("t_tran_sys")->AsString.c_str() << '\n';
                if (!sql->FieldByName("t_color")->IsNull)
                    of << "t_color = " << sql->FieldByName("t_color")->AsString[1] << '\n';
            }
            of << "t_ant_dir = "                  << dir.c_str()                                                      << '\n'
               << "t_polar = "                    << sql->FieldByName("t_POLAR")->AsString.c_str()                    << '\n'
               << "t_hgt_agl = "                  << sql->FieldByName("t_hgt_agl")->AsString.c_str()                  << '\n'
               << "t_site_alt = "                 << sql->FieldByName("t_site_alt")->AsString.c_str()                 << '\n'
               << "t_eff_hgtmax = "               << sql->FieldByName("t_eff_hgtmax")->AsString.c_str()               << '\n';
            if (frm->rgArt->ItemIndex == 1) // Art.5
                of << "t_op_agcy = "              << (sql->FieldByName("t_op_agcy")->AsString == "" ?
                                                    "A" :
                                                    sql->FieldByName("t_op_agcy")->AsString.c_str())                  << '\n'
                  << "t_addr_code = "             << (sql->FieldByName("t_addr_code")->AsString == "" ?
                                                    "00" :
                                                    sql->FieldByName("t_addr_code")->AsString.c_str())                << '\n'
                  << "t_op_hh_fr = "              << (sql->FieldByName("t_op_hh_fr")->IsNull ? "0000" :
                                                    FormatDateTime("hhnn",sql->FieldByName("t_op_hh_fr")->AsTime).c_str()) << '\n'
                  << "t_op_hh_to = "              << (sql->FieldByName("t_op_hh_to")->IsNull ? "2400" :
                                                    FormatDateTime("hhnn",sql->FieldByName("t_op_hh_to")->AsTime).c_str()) << '\n'
                  << "t_is_resub = "              << frm->cbIsResub->Text.c_str()             << '\n'
                  << "t_remark_conds_met = "      << frm->cbRemarkCondsMet->Text.c_str()     << '\n'
                  << "t_signed_commitment = "     << frm->cbSignedCommitment->Text.c_str()    << '\n';

            of << "t_remarks = "                  << sql->FieldByName("t_remarks")->AsString.c_str()                  << '\n';

            ExportVector(sql->FieldByName("EFFECTHEIGHT"), of, "ANT_HGT", "t_eff_hgt", 1, 0);
            if (hor && dir == "D")
                ExportVector(sql->FieldByName("ANT_DIAG_H"), of, "ANT_DIAGR_H", "t_attn", -1, sql->FieldByName("ANTENNAGAIN")->AsDouble);
            if (vert && dir == "D")
                ExportVector(sql->FieldByName("ANT_DIAG_V"), of, "ANT_DIAGR_V", "t_attn", -1, sql->FieldByName("ANTENNAGAIN")->AsDouble);

            ExportCoord(sql->FieldByName("COORD"), of);

        }
        else if (type == emGs2Gt2)
        {
            AnsiString noticeType = sql->FieldByName("t_notice_type")->AsString;
            if (noticeType.Length() > 0)
                noticeType[1] = 'G';
            double dtExp = sql->FieldByName("t_d_expiry")->AsDate;
            
            of << "t_notice_type = " << noticeType.c_str() << '\n';
            of << "t_action = "      << frm->cbxAction->Text.c_str() << '\n'
               << "t_fragment = "        << "GE06D" << '\n'
               //<< "t_fragment = "        << sql->FieldByName("t_fragment"     )->AsString.c_str() << '\n'
               << "t_is_pub_req = "      << (sql->FieldByName("t_is_pub_req"   )->AsString == "" ?
                                            frm->cbIsPubReq->Text.c_str() :
                                            sql->FieldByName("t_is_pub_req"   )->AsString.c_str())<< '\n'
               << "t_adm_ref_id = "      << sql->FieldByName("t_adm_ref_id"   )->AsString.c_str() << '\n';
            if (frm->cbxAction->Text == "MODIFY")
            of << "t_trg_adm_ref_id = "  << sql->FieldByName("t_trg_adm_ref_id")->AsString.c_str() << '\n';
            of << "t_plan_entry = "      << sql->FieldByName("t_plan_entry"   )->AsString.c_str() << '\n'
               << "t_sfn_id = "          << sql->FieldByName("t_sfn_id"       )->AsString.c_str() << '\n'
               << "t_freq_assgn = "      << sql->FieldByName("t_freq_assign"  )->AsString.c_str() << '\n'
               << "t_offset = "          << sql->FieldByName("t_offset"       )->AsString.c_str() << '\n'
               << "t_d_expiry = "        << (dtExp > 0 ? FormatDateTime("yyyy-mm-dd\n", dtExp).c_str() : "\n")
               << "t_allot_name = "      << sql->FieldByName("t_allot_name"   )->AsString.c_str() << '\n'
               << "t_ctry = "            << sql->FieldByName("t_ctry"         )->AsString.c_str() << '\n'
               << "t_geo_area = "        << sql->FieldByName("t_geo_area"     )->AsString.c_str() << '\n'
               << "t_nb_sub_areas = "    << sql->FieldByName("t_nb_sub_areas" )->AsString.c_str() << '\n';

            sqlAux->Close();
            sqlAux->Params->Vars[0]->AsInteger = sql->Fields[0]->AsInteger;
            for (sqlAux->ExecQuery(); !sqlAux->Eof; sqlAux->Next())
                of << "t_contour_id = " << sqlAux->Fields[0]->AsInteger << '\n';

            of << "t_ref_plan_cfg = "    << sql->FieldByName("t_ref_plan_cfg" )->AsString.c_str() << '\n'
               << "t_typ_ref_netwk = "   << sql->FieldByName("t_typ_ref_netwk")->AsString.c_str() << '\n'
               << "t_spect_mask = "      << sql->FieldByName("t_spect_mask"   )->AsString.c_str() << '\n'
               << "t_polar = "           << sql->FieldByName("t_polar"        )->AsString.c_str() << '\n'
               << "t_remarks = "         << sql->FieldByName("t_remarks"      )->AsString.c_str() << '\n';

            ExportCoord(sql->FieldByName("COORD"), of);
        }
        of << "</NOTICE>\n";
    }
    sql->Close();

    of << "<TAIL>\n";
    of << "t_num_notices = " << notices << '\n';
    of << "</TAIL>\n";

    of.flush();
    of.close();

    if (frm->chbOpenFile->Checked)
    {
        AnsiString fn = frm->edtFileName->Text;
        if (fn.Pos(" ") > 0)
            fn = "\"" + fn + "\"";
        WinExec(("notepad.exe "+fn).c_str(), SW_SHOW);
    }

    frm->exported = true;
    return 0;
}

void __fastcall TfrmRrc06Export::btnFileClick(TObject *Sender)
{
	std::auto_ptr<TSaveDialog> sd (new TSaveDialog(Application));
    sd->Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы|*.*";
    sd->FilterIndex = 1;
    sd->DefaultExt = "txt";
    sd->FileName = edtFileName->Text;
    sd->Options = sd->Options << ofPathMustExist;
    if(sd->Execute())
        edtFileName->Text = sd->FileName;
}
//---------------------------------------------------------------------------


void __fastcall TfrmRrc06Export::LoadConf()
{
    std::auto_ptr<TIniFile> ini(new TIniFile(ExtractFilePath(Application->ExeName) + "Rrc06ExpHead.ini"));
    std::auto_ptr<TStringList> sl(new TStringList());
    ini->ReadSectionValues("Comment", sl.get());

    memComment->Lines->Clear();
    if (sl->Count == 0)
        memComment->Lines->Text = "The Ukrainian State Centre of Radio Frequencies\n"
                                  "15 km pr. Peremogy, 03179 Kyiv, Ukraine\n\n"
                                  "File created by LIS-Broadcast (c) 2003-2008, LIS Llc\n\n";
    else
    {
        while (sl->Count > 0)
        {
            AnsiString str = sl->Strings[0];
            memComment->Lines->Add(str.SubString(str.Pos("=")+1, str.Length()).Trim());
            sl->Delete(0);
        }
    }

    ini->ReadSectionValues("HEAD", sl.get());
    if (sl->Count == 0)
    {
        sgHead->Cells[0][1] = "t_char_set";
        sgHead->Cells[1][1] = "windows-1251";
        sgHead->Cells[0][2] = "t_adm";
        sgHead->Cells[1][2] = "UKR";
        sgHead->Cells[0][3] = "t_email_addr";
        sgHead->Cells[1][3] = "centre@ucrf.gov.ua";
    } else {
        for (int i = 0; i < sgHead->RowCount - 1; i++)
        {
            if (i < sl->Count)
            {
                AnsiString str = sl->Strings[i];
                sgHead->Cells[0][i + 1] = str.SubString(1, str.Pos("=")-1).Trim();
                sgHead->Cells[1][i + 1] = str.SubString(str.Pos("=")+1, str.Length()).Trim();
            } else {
                sgHead->Cells[0][i + 1] = "";
                sgHead->Cells[1][i + 1] = "";
            }
        }
    }
}
//---------------------------------------------------------------------------

void TfrmRrc06Export::SaveConf()
{
    std::auto_ptr<TIniFile> ini(new TIniFile(ExtractFilePath(Application->ExeName) + "Rrc06ExpHead.ini"));
    ini->EraseSection("Comment");
    for (int i = 0; i < memComment->Lines->Count; i++)
        ini->WriteString("Comment", "line"+IntToStr(i), memComment->Lines->Strings[i]);
    ini->EraseSection("HEAD");
    for (int i = 0; i < sgHead->RowCount - 1; i++)
        ini->WriteString("HEAD", sgHead->Cells[0][i + 1], sgHead->Cells[1][i + 1]);
}
//---------------------------------------------------------------------------

void __fastcall TfrmRrc06Export::btnImportClick(TObject *Sender)
{
    if(edtFileName->Text.IsEmpty())
    {
        ModalResult = mrNone;
        throw *(new Exception("Задайте имя файла"));
    }
}
//---------------------------------------------------------------------------

const char * __fastcall TfrmRrc06Export::BoolToStr(bool val)
{
    static const char *t = "TRUE"; static const char *f = "FALSE";
    return val ? t : f;
}

void __fastcall TfrmRrc06Export::ExportCoord(TIBXSQLVAR* fld, std::ofstream& of)
{
    if (fld == NULL || fld->IsNull)
        return;
    std::auto_ptr<TStringList> sl(new TStringList());
    sl->Delimiter = ' ';
    sl->DelimitedText = fld->AsString;
    if (sl->Count > 0)
    {
        of << "<COORD>\n";
        for (int i = 0; i < sl->Count; i++)
            of << "t_adm = " << sl->Strings[i].c_str() << '\n';
        of << "</COORD>\n";
    }
}

void __fastcall TfrmRrc06Export::ExportVector(TIBXSQLVAR* fld, std::ofstream& of, char* tag, char* pref, int k, double c)
{
    std::auto_ptr<TMemoryStream> ms (new TMemoryStream());
    fld->SaveToStream(ms.get());
    int size = ms->Size / sizeof(double);
    if (size > 0)
    {
        double *a = new double[size];
        try {
            of << '<' << tag << '>' << '\n';
            ms->Position = 0;
            ms->Read((void*)a, ms->Size);
            for (int i = 0; i < size; i++)
                of << pref << (AnsiString().sprintf("@azm%03d = ", 360 / size * i) + FormatFloat("0.#\n",a[i] * k + c)).c_str();
            of << '<'<<'/' << tag << '>' << '\n';
        } __finally {
            delete[] a;
        }
    }
}
