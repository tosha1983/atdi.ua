//$$---- Form CPP ----
//---------------------------------------------------------------------------
//
//  Module is intended to perfom operation of import GA1/GS1/GT1/GS2/GT2 files,
//  including preview of their content
//  Author: sk
//  last date   :
//  last writer :

#include <vcl.h>
#pragma hdrstop

#include "uItuImport.h"
#include <memory>
#include "uMainDm.h"
#include "uDlgConstrSet.h"
#include "LISBCTxServer_TLB.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TfrmRrc06Import *frmRrc06Import;

static NoticeElements neGa1[] =
{
     { "Notice No"           ,25   ,"No"              ,1  ,""            ,0 }
    ,{ "t_notice_type"       ,40   ,"TYPE"            ,1  ,""            ,0 }
    ,{ "t_action"            ,70   ,"ACTION"          ,1  ,""            ,0 }
    ,{ "t_ctry"              ,40   ,"CTRY"            ,1  ,"CTRY"        ,0 }
    ,{ "t_contour_id"        ,80   ,"CONTOUR ID"      ,1  ,"CONTOUR_ID"  ,0 }
    ,{ "t_nb_test_pts"       ,70   ,"TEST POINTS"     ,1  ,"NB_TEST_PTS" ,0 }
    ,{ "t_remarks"           ,200  ,"REMARKS"         ,1  ,""            ,0 }
};


static NoticeElements neGs1Gt1[] =
{
     { "Notice No"                  ,20  ,"No"                      ,1  ,""                         ,0   }
    ,{ "t_notice_type"              ,25  ,"type"                    ,1  ,""                         ,0   }
    ,{ "t_is_pub_req"               ,30  ,"is_pub_req"              ,1  ,"is_pub_req"               ,'B' }
    ,{ "t_fragment"                 ,40  ,"fragment"                ,1  ,""                         ,0   }
    ,{ "t_action"                   ,40  ,"action"                  ,1  ,""                         ,0   }
    ,{ "t_adm_ref_id"               ,99  ,"adm_ref_id"              ,1  ,"adm_ref_id"               ,0   }
    ,{ "t_plan_entry"               ,25  ,"plan_entry"              ,1  ,"plan_entry"               ,0   }
    ,{ "t_assgn_code"               ,25  ,"assgn_code"              ,1  ,"assgn_code"               ,0   }
    ,{ "t_associated_adm_allot_id"  ,80  ,"asc_adm_allot_id"        ,1  ,"associated_adm_allot_id"  ,0   }
    ,{ "t_associated_allot_sfn_id"  ,50  ,"asc_allot_sfn_id"        ,1  ,"associated_allot_sfn_id"  ,0   }
    ,{ "t_sfn_id"                   ,50  ,"sfn_id"                  ,1  ,""                         ,0   }
    ,{ "t_call_sign"                ,25  ,"call_sign"               ,1  ,"call_sign"                ,0   }
    ,{ "t_freq_assgn"               ,40  ,"freq_assgn"              ,1  ,""                         ,0   }
    ,{ "t_offset"                   ,25  ,"offset"                  ,1  ,"VIDEO_OFFSET_HERZ"        ,0   }
    ,{ "t_d_inuse"                  ,25  ,"d_inuse"                 ,1  ,"DATEINTENDUSE"            ,'D' }
    ,{ "t_d_expiry"                 ,25  ,"d_expiry"                ,1  ,"d_expiry"                 ,'D' }
    ,{ "t_site_name"                ,60  ,"site_name"               ,1  ,""                         ,0   }
    ,{ "t_ctry"                     ,25  ,"ctry"                    ,1  ,""                         ,0   }
    ,{ "t_long"                     ,55  ,"long"                    ,1  ,""                         ,0   }
    ,{ "t_lat"                      ,50  ,"lat"                     ,1  ,""                         ,0   }
    ,{ "t_ref_plan_cfg"             ,30  ,"rpc"                     ,1  ,""                         ,0   }
    ,{ "t_sys_var"                  ,25  ,"sys_var"                 ,1  ,""                         ,0   }
    ,{ "t_rx_mode"                  ,25  ,"rx_mode"                 ,1  ,""                         ,0   }
    ,{ "t_spect_mask"               ,20  ,"s_m"                     ,1  ,"spect_mask"               ,0   }
    ,{ "t_erp_h_dbw"                ,30  ,"erp_h_dbw"               ,1  ,"EPR_SOUND_HOR_PRIMARY"    ,0   }
    ,{ "t_erp_v_dbw"                ,30  ,"erp_v_dbw"               ,1  ,"EPR_SOUND_VERT_PRIMARY"   ,0   }
    ,{ "t_ant_dir"                  ,25  ,"ant_dir"                 ,1  ,"DIRECTION"                ,0   }
    ,{ "t_polar"                    ,25  ,"polar"                   ,1  ,"POLARIZATION"             ,0   }
    ,{ "t_hgt_agl"                  ,25  ,"hgt_agl"                 ,1  ,"HEIGHTANTENNA"            ,0   }
    ,{ "t_site_alt"                 ,25  ,"alt"                     ,1  ,""                         ,0   }
    ,{ "t_eff_hgtmax"               ,30  ,"eff_hgtmax"              ,1  ,"HEIGHT_EFF_MAX"           ,0   }
    ,{ "t_op_agcy"                  ,40  ,"op_agcy"                 ,1  ,"op_agcy"                  ,0   }
    ,{ "t_addr_code"                ,40  ,"addr_code"               ,1  ,"addr_code"                ,0   }
    ,{ "t_op_hh_fr"                 ,40  ,"op_hh_fr"                ,1  ,"op_hh_fr"                 ,'T' }
    ,{ "t_op_hh_to"                 ,40  ,"op_hh_to"                ,1  ,"op_hh_to"                 ,'T' }
    ,{ "t_is_resub"                 ,30  ,"is_resub"                ,1  ,"is_resub"                 ,'B' }
    ,{ "t_remark_conds_met"         ,30  ,"remark_conds_met"        ,1  ,"remark_conds_met"         ,'B' }
    ,{ "t_signed_commitment"        ,30  ,"signed_commitment"       ,1  ,"signed_commitment"        ,'B' }
    ,{ "t_remarks"                  ,90  ,"remarks"                 ,1  ,"remarks"                  ,0   }
};

static NoticeElements neGt2Gs2[] =
{
     { "Notice No"           ,25   ,"No"              ,1  ,""                 ,0  }
    ,{ "t_notice_type"       ,30   ,"TYPE"            ,1  ,"notice_type"      ,0  }
    ,{ "t_fragment"          ,55   ,"FRAGMENT"        ,1  ,"fragment"         ,0  }
    ,{ "t_action"            ,50   ,"ACTION"          ,1  ,"action"           ,0  }
    ,{ "t_is_pub_req"        ,35   ,"PUB"             ,1  ,"is_pub_req"       ,0  }
    ,{ "t_adm_ref_id"        ,130  ,"ADM_REF_ID"      ,1  ,"adm_ref_id"       ,0  }
    ,{ "t_trg_adm_ref_id"    ,130  ,"TRG_ADM_REF_ID"  ,1  ,""                 ,0  }
    ,{ "t_plan_entry"        ,30   ,"PLAN"            ,1  ,"plan_entry"       ,0  }
    ,{ "t_sfn_id"            ,130  ,"SFN"             ,1  ,""                 ,0  }
    ,{ "t_freq_assgn"        ,50   ,"FREQ"            ,1  ,"freq_assign"      ,0  }
    ,{ "t_offset"            ,50   ,"OFFSET"          ,1  ,"offset"           ,0  }
    ,{ "t_d_expiry"          ,90   ,"EXP"             ,1  ,"d_expiry"         ,'D'}
    ,{ "t_allot_name"        ,130  ,"NAME"            ,1  ,"allot_name"       ,0  }
    ,{ "t_ctry"              ,25   ,"CTRY"            ,1  ,"ctry"             ,0  }
    ,{ "t_geo_area"          ,25   ,"AREA"            ,1  ,"geo_area"         ,0  }
    ,{ "t_nb_sub_areas"      ,25   ,"NB_SUB_AR"       ,1  ,"nb_sub_areas"     ,0  }
    ,{ "t_contour_id"        ,130  ,"CONTOUR"         ,1  ,"contour_id"       ,0  }
    ,{ "t_ref_plan_cfg"      ,30   ,"RPC"             ,1  ,"ref_plan_cfg"     ,0  }
    ,{ "t_typ_ref_netwk"     ,25   ,"RN"              ,1  ,"typ_ref_netwk"    ,0  }
    ,{ "t_spect_mask"        ,20   ,"SM"              ,1  ,"spect_mask"       ,0  }
    ,{ "t_polar"             ,20   ,"P"               ,1  ,"polar"            ,0  }
    ,{ "t_remarks"           ,150  ,"REMARKS"         ,1  ,"remarks2"         ,0  }
};

static const char* modeNames[] =
{
     "GA1"
    ,"GS1/GT1/G02/GB1"
    ,"GS2/GT2"
};

//---------------------------------------------------------------------------
__fastcall TfrmRrc06Import::TfrmRrc06Import(TComponent* Owner)
	: TForm(Owner), m_importMode(imNone), noticeElements(NULL),
    minLon(-MaxDouble),
    minLat(-MaxDouble),
    maxLon(MaxDouble),
    maxLat(MaxDouble)
{
    inputFile.docStyle = dsBcNotify;
    lblError->Font->Color = clRed;
    lblConstrContent->Font->Color = clNavy;

    mapForm = new TForm(GroupBox1);
    mapForm->Parent = GroupBox1;
    mapForm->ParentWindow = GroupBox1;
    mapForm->Visible = false;
    mapForm->Align = alClient;
    mapForm->Visible = true;
    mapForm->BorderStyle = bsNone;
    mapForm->OnPaint = FormPaint;

    oldIndex = -1;
    
    SetImportMode(imGa1);

    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Database = dmMain->dbMain;
    sql->SQL->Text = "select ID, SECTIONNAME from DATABASESECTION where ID >= 0 order by SECTIONNAME ";
    for (sql->ExecQuery(); !sql->Eof; sql->Next())
        cbxDbSection->Items->AddObject(sql->Fields[1]->AsString, (TObject*)sql->Fields[0]->AsInteger);

    cbxDbSection->ItemIndex = cbxDbSection->Items->IndexOfObject((TObject*)1);

    constrained.reserve(1);
}
//---------------------------------------------------------------------------


void __fastcall TfrmRrc06Import::btnFileClick(TObject *Sender)
{
    //нажатие кнопки с намеком "..." = открыть_файл
    //выбираем файл и (если выбрали) вызываем LoadFile();
	std::auto_ptr<TOpenDialog> od (new TOpenDialog(this));
    od->Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы|*.*";
    od->FilterIndex = 1;
    if(!od->Execute())
        return;//если пользователь передумал
    edtFileName->Text = od->FileName;
    lblError->Visible = false;

    grdData->RowCount = 2;
    for (int i = 0; i < grdData->ColCount; i++)
        grdData->Cells[i][1] = "";


    btnImport->Enabled = true;//включаем кнопку импорта

	try
	{
		LoadFile(edtFileName->Text);
	}
    catch(Exception &e)
	{
		throw *(new Exception(AnsiString("btnFileClick(): Ошибка при загрузке файла:\n"+e.Message).c_str()));
	}
}
//---------------------------------------------------------------------------

void __fastcall TfrmRrc06Import::LoadFile(AnsiString fName)
{
//закрытая функция, загружает файл с именем fName
		//регулируем высоту списков
		//dblcbAnt->DropDownRows = Screen->Height/3/abs(dblcbAnt->Font->Height);

    Clear();

    int dbSection = 0;
    if (cbxDbSection->ItemIndex > -1)
        dbSection = (int)cbxDbSection->Items->Objects[cbxDbSection->ItemIndex];


    DrawContourData contourData;

    int res = inputFile.LoadFrom(fName.c_str());

    if ( res == 0 )
    {
        char oldDecimalSeparator = DecimalSeparator;
        DecimalSeparator = '.';

        try
        {
            noticeCount = 0;
            for (std::vector<XmlDocNode*>::iterator iNode = inputFile.rootNodes.begin(); iNode != inputFile.rootNodes.end(); iNode++)
            {
                if ( (*iNode)->GetName() == "HEAD" )
                {
                    XmlDocNode::Elements& els = (*iNode)->GetElements();
                    XmlDocNode::Elements::iterator el;
                    for ( el = els.begin(); el != els.end(); el++ )
                    {
                        if (el->first == "t_adm")
                        {
                            t_adm = el->second;
                            adminId = dmMain->getCountryId((char*)t_adm.c_str());
                        }
                    }
                    if (t_adm.empty())
                    {
                        LogError("В секции <HEAD> отсутствует параметр t_adm.");
                        lblError->Visible = true;
                    } else if (adminId < 1) {
                        LogError(AnsiString().sprintf("Для t_adm = '%s' не могу найти страну. ", t_adm.c_str()));
                        lblError->Visible = true;
                    }
                }
                else if ( (*iNode)->GetName() == "NOTICE" )
                {
                    std::string t_notice_type;
                    std::string t_ctry;
                    std::string t_long;
                    std::string t_lat;
                    std::string t_contour_id;
                    std::vector <int> contours;
                    XmlDocNode::Elements& els = (*iNode)->GetElements();
                    XmlDocNode::Elements::iterator el;

                    bool constrFlag = false;

                    //std::map<std::string, std::string> elemMap;
                    for ( el = els.begin(); el != els.end(); el++ )
                    {
                        //elemMap[element->first] = element->second;
                        if (el->first == "t_notice_type")
                            t_notice_type = el->second;
                        else if (el->first == "t_contour_id")
                        {
                            t_contour_id = el->second;
                            try {
                                contours.push_back(AnsiString(el->second.c_str()).ToInt());
                            } catch(Exception &e) {
                                LogError(AnsiString().sprintf("Ошибка обработки t_contour_id = '%s' (NOTICE %d)",
                                                            el->second.c_str(), noticeCount));
                            }
                        }
                        else if (el->first == "t_sfn_id")
                            sfnPresent = true;
                        else if (el->first == "t_lat")
                            t_lat = el->second;
                        else if (el->first == "t_long")
                            t_long = el->second;
                        else if (el->first == "t_ctry")
                            t_ctry = el->second;
                    }

                    if (m_importMode == imGa1 && t_notice_type == "GA1"
                        || m_importMode == imGs1Gt1 && (t_notice_type == "GT1" || t_notice_type == "GS1"
                                                     || t_notice_type == "G02" || t_notice_type == "GB1")
                        || m_importMode == imGs2Gt2 && (t_notice_type == "GT2" || t_notice_type == "GS2"))
                    {
                        noticeCount++;

                        if (t_notice_type == "GA1")
                        {
                            contourData.id = noticeCount;
                            contourData.name = t_contour_id.c_str();
                            contourData.contour.clear();
                            std::vector<XmlDocNode*> nodes = (*iNode)->GetSubNodes();
                            std::vector<XmlDocNode*>::iterator node;
                            //int pointNo = 0;

                            bool atLeastOnePoint = false;

                            for ( node = nodes.begin(); node != nodes.end(); node++ )
                            {
                                std::string nn = (*node)->GetName();
                                if (nn != "POINT")
                                    continue;

                                XmlDocNode::Elements& coords = (*node)->GetElements();
                                XmlDocNode::Elements::iterator coord;

                                std::string t_long; std::string t_lat;

                                for ( coord = coords.begin(); coord != coords.end(); coord++ )
                                {
                                    if (coord->first == "t_lat")
                                        t_lat = coord->second;
                                    if (coord->first == "t_long")
                                        t_long = coord->second;
                                }

                                // TODO: check lat & long
                                double lat = atolat(t_lat.c_str());
                                double lon = atolon(t_long.c_str());

                                BcCoord bc; bc.lon = lon; bc.lat = lat;
                                contourData.contour.push_back(bc);

                                atLeastOnePoint = atLeastOnePoint || CheckPoint(lon, lat);
                            }
                            contoursData.push_back(contourData);

                            constrFlag = !atLeastOnePoint;
                        }

                        if (m_importMode == imGs1Gt1)
                        {
                            // check constraints
                            // TODO: check lat & long
                            double lat = atolat(t_lat.c_str());
                            double lon = atolon(t_long.c_str());
                            constrFlag = !CheckPoint(lon, lat);
                        }

                        if (m_importMode == imGs2Gt2
                            && (minLon > -MaxDouble || minLat > -MaxDouble ||
                                maxLat < MaxDouble || maxLon < MaxDouble || chckIfContourExists)
                            && contours.size() > 0)
                        {
                            // Ид всех контуров - в векторе contours
                            // проверить наличие контуров в БД (если флаг проверки есть)
                            // выгрести все точки контуров и проверить

                            std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
                            sql->Database = dmMain->dbMain;

                            AnsiString contList = AnsiString().sprintf("%04d", contours[0]);
                            std::vector<int>::iterator vi = contours.begin();
                            for (vi++; vi < contours.end(); vi++)
                                contList = contList + AnsiString().sprintf(",%04d", *vi);

                            sql->SQL->Text = AnsiString("select p.LON, p.LAT, c.CONTOUR_ID from DIG_SUBAREAPOINT p "
                                            "left join DIG_CONTOUR c on (p.CONTOUR_ID = c.ID) "
                                            "where c.CTRY = '") + t_ctry.c_str() + "' "
                                            "and c.CONTOUR_ID in (" + contList + ") "
                                            "and c.DB_SECTION_ID = " + IntToStr(dbSection);

                            bool atLeastOnePoint = false;
                            std::vector<int> tempContList = contours;

                            for (sql->ExecQuery(); !sql->Eof; sql->Next())
                            {
                                atLeastOnePoint = atLeastOnePoint || CheckPoint(sql->Fields[0]->AsDouble, sql->Fields[1]->AsDouble);
                                if (chckIfContourExists)
                                {
                                    int cid = sql->Fields[2]->AsInteger;
                                    for (int i = tempContList.size() - 1; i >= 0; i--)
                                        if (tempContList[i] == cid)
                                            tempContList.erase(&tempContList[i]);
                                }
                            }

                            // если ни одна точка не подходит или какого-то контура нет - то до свидания
                            constrFlag = !atLeastOnePoint || tempContList.size() > 0;

                        }

                        // флаг ограничений
                        constrained.push_back(constrFlag);

                        // вывести в сетку
                        grdData->RowCount = noticeCount + 1;
                        for ( el = els.begin(); el != els.end(); el++ )
                        {
                            std::map<std::string, int>::iterator i = elIndices.find(el->first);
                            if (i != elIndices.end())
                            {
                                if (el->first == "t_contour_id" && contours.size() > 0)
                                {
                                    AnsiString value = AnsiString().sprintf("%04d", contours[0]);
                                    std::vector<int>::iterator vi = contours.begin();
                                    for (vi++; vi < contours.end(); vi++)
                                        value = value + AnsiString().sprintf(", %04d", *vi);
                                    grdData->Cells[i->second][noticeCount] = value;
                                } else
                                    grdData->Cells[i->second][noticeCount] = el->second.c_str();
                            } else
                                grdData->Cells[i->second][noticeCount] = "";
                        }
                    }
                }
            }
            if (noticeCount == 0)
            {
                LogError(AnsiString().sprintf("Не обнаружено ни одной секции <NOTICE> %s. ",modeNames[m_importMode]));
                lblError->Visible = true;
            }

            if (noticeCount > 0 && t_adm.size() > 0 && adminId > 0)
                btnImport->Enabled = true;

            if (m_importMode == imGa1)
                mapForm->Invalidate();

        }  __finally {
            DecimalSeparator = oldDecimalSeparator;
            if (lblError->Lines->Count > 0)
                lblError->Visible = true;
        }
    }
    else
        MessageBox(NULL, AnsiString().sprintf("Файл загрузить не удалось\nКод ошибки: %d\n"
                    "%s\nСтрока %d, символ %d",
                    res, inputFile.GetErrorMsg().c_str(), inputFile.GetLine(), inputFile.GetSymbol()).c_str(),
                    "Ошибка", MB_ICONHAND);

    #ifdef _DEBUG
    std::auto_ptr<TForm> f (new TForm(Application));
    f->Width = 700;
    f->Height = 500;
    f->BorderStyle = bsSizeToolWin;
    f->Position = poMainFormCenter;
    f->Caption = "XML Parser Log";
    TMemo *m = new TMemo(f.get());
    m->Parent = f.get();
    m->Align = alClient;
    //m->Font->Name = "Tahoma";
    m->Font->Name = "Courier New";
    m->ReadOnly = true;
    inputFile.xmlLog.CopyTo(m->Lines);
    f->ShowModal();
    #endif

}
//---------------------------------------------------------------------------

void __fastcall TfrmRrc06Import::btnFitAllColumnsClick(TObject *Sender)
{
//кнопка для размещения всех полей первой сетки на экране
	try
	{
		if(grdData->ColCount == 0)
			return;
		//int wdt = (grdSta->Width-60)/grdSta->Columns->Count;
		//for(int i = 0; i < grdSta->Columns->Count; i++)
		//	grdSta->Columns->Items[i]->Width = wdt;
	}
	catch(Exception &e)
	{
		MessageBox(NULL, "Нельзя установить ширину колонок автоматически.", "LISFS" , MB_ICONHAND);
	}
}
//---------------------------------------------------------------------------

void __fastcall TfrmRrc06Import::grdDataKeyDown(TObject *Sender, WORD &Key,
	  TShiftState Shift)
{
//если надавили кнопку на клавиатуре на первой сетке
//устанавливаем фильтр для второй сетки
	//grdCorr->DataSource->DataSet->Filter = "NUMBER = '"+grdSta->DataSource->DataSet->FieldByName(L"NUMBER")->AsString+"'";
    if (m_importMode == imGa1)
    {
        if (oldIndex > 0)
            cd.DrawContour(contoursData[oldIndex - 1], constrained[oldIndex - 1] ? clRed : clBlue, clDkGray);
        if (grdData->Row > 0 && contoursData.size() > 0)
        {
            cd.DrawContour(contoursData[grdData->Row - 1], constrained[grdData->Row - 1] ? clFuchsia : clLime, clWindowText);
            oldIndex = grdData->Row;
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmRrc06Import::grdDataMouseUp(TObject *Sender,
	  TMouseButton Button, TShiftState Shift, int X, int Y)
{
//если отпустили кнопку мыши на первой сетке
//устанавливаем фильтр для второй сетки
	//grdCorr->DataSource->DataSet->Filter = "NUMBER = '"+grdSta->DataSource->DataSet->FieldByName(L"NUMBER")->AsString+"'";
    if (m_importMode == imGa1)
    {
        if (oldIndex > 0)
            cd.DrawContour(contoursData[oldIndex - 1], constrained[oldIndex - 1] ? clRed : clBlue, clDkGray);
        if (grdData->Row > 0 && contoursData.size() > 0)
        {
            cd.DrawContour(contoursData[grdData->Row - 1], constrained[grdData->Row - 1] ? clFuchsia : clLime, clWindowText);
            oldIndex = grdData->Row;
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmRrc06Import::btnCancelClick(TObject *Sender)
{
	Close();
}
//---------------------------------------------------------------------------

inline void SetVector(std::vector<XmlDocNode*>::iterator node, char* tag,
    TIBSQL *sql, AnsiString paramName, int coeff, double offset)
{
    const int ARRLEN = 36;
    double dblArray[ARRLEN]; for (int i=0; i<ARRLEN; i++) dblArray[i] = 0.0;

    XmlDocNode::Elements& els = (*node)->GetElements();
    XmlDocNode::Elements::iterator el;
    int tagLen = strlen(tag);
    for ( el = els.begin(); el != els.end(); el++ )
    {
        if (el->first.substr(0, tagLen) == tag)
        {
            int idx = ::atoi(el->first.substr(tagLen, 3).c_str()) / 10;
            while (idx > ARRLEN) idx -= ARRLEN; while (idx < 0) idx += ARRLEN;
            dblArray[idx] = ::atof(el->second.c_str()) * coeff + offset;
        }
    }
    std::auto_ptr<TMemoryStream> memstream (new TMemoryStream());
    memstream->Write(dblArray, sizeof(double)*ARRLEN);
    sql->ParamByName(paramName)->LoadFromStream(memstream.get());
}

//---------------------------------------------------------------------------

void __fastcall TfrmRrc06Import::btnImportClick(TObject *Sender)
{
    int dbSection = 0;
    if (cbxDbSection->ItemIndex == -1)
        throw *(new Exception("Виберiть роздiл БД, в який треба будете проводити iмпорт"));
    else
        dbSection = (int)cbxDbSection->Items->Objects[cbxDbSection->ItemIndex];

    char *contIns = "insert into DIG_CONTOUR (ID, ADM_ID, CTRY, CONTOUR_ID, NB_TEST_PTS, DB_SECTION_ID) values (:ID, :ADM_ID, :CTRY, :CONTOUR_ID, :NB_TEST_PTS, :DB_SECTION_ID)";
    char *contDel = "delete from DIG_CONTOUR where ADM_ID = :ADM_ID and CONTOUR_ID = :CONTOUR_ID and DB_SECTION_ID = :DB_SECTION_ID";
    char *allotIns = "insert into DIG_ALLOTMENT (ID, ADM_ID, NOTICE_TYPE, IS_PUB_REQ, ADM_REF_ID, PLAN_ENTRY, "
        " SFN_ID_FK, FREQ_ASSIGN, OFFSET, D_EXPIRY, ALLOT_NAME, CTRY, GEO_AREA, NB_SUB_AREAS, REF_PLAN_CFG, "
        " TYP_REF_NETWK, SPECT_MASK, POLAR, "
        "BLOCKDAB_ID, CHANNEL_ID, "
        "REMARKS2, DB_SECTION_ID)"
        " values (:ID, :ADM_ID, :NOTICE_TYPE, :IS_PUB_REQ, :ADM_REF_ID, :PLAN_ENTRY, "
        " :SFN_ID_FK, :FREQ_ASSIGN, :OFFSET, :D_EXPIRY, :ALLOT_NAME, :CTRY, :GEO_AREA, :NB_SUB_AREAS, :REF_PLAN_CFG, "
        " :TYP_REF_NETWK, :SPECT_MASK, :POLAR, "
        ":BLOCKDAB_ID, :CHANNEL_ID, "
        ":REMARKS2, :DB_SECTION_ID)";
    char *allotUpd = "UPDATE DIG_ALLOTMENT SET "
          " ADM_ID = :ADM_ID, "
          " NOTICE_TYPE = :NOTICE_TYPE, "
          " IS_PUB_REQ = :IS_PUB_REQ,  "
          " ADM_REF_ID = :ADM_REF_ID, "
          " PLAN_ENTRY = :PLAN_ENTRY, "
          " SFN_ID_FK = :SFN_ID_FK, "
          " FREQ_ASSIGN = :FREQ_ASSIGN, "
          " OFFSET = :OFFSET, "
          " D_EXPIRY = :D_EXPIRY, "
          " ALLOT_NAME = :ALLOT_NAME, "
          " CTRY = :CTRY, "
          " GEO_AREA = :GEO_AREA, "
          " NB_SUB_AREAS = :NB_SUB_AREAS, "
          " REF_PLAN_CFG = :REF_PLAN_CFG, "
          " TYP_REF_NETWK = :TYP_REF_NETWK, "
          " SPECT_MASK = :SPECT_MASK, "
          " POLAR = :POLAR, "
          " BLOCKDAB_ID = :BLOCKDAB_ID, "
          " CHANNEL_ID = :CHANNEL_ID, "
          " REMARKS1 = :REMARKS1, "
          " REMARKS2 = :REMARKS2 "
          " where ID = :ID";

    std::auto_ptr<TIBSQL> noticeSql (new TIBSQL(this));
    std::auto_ptr<TIBTransaction> tr (new TIBTransaction(this));
    tr->DefaultDatabase = dmMain->dbMain;
    tr->DefaultAction = TARollback;
    tr->StartTransaction();
    noticeSql->Database = dmMain->dbMain;
    noticeSql->Transaction = tr.get();

    std::auto_ptr<TIBSQL> sqlAux (new TIBSQL(this));
    sqlAux->Database = dmMain->dbMain;
    sqlAux->Transaction = tr.get();

    if (sfnPresent)
    {
        sqlAux->SQL->Text = "select SYNHRONETID, ID from SYNHROFREQNET";
        for (sqlAux->ExecQuery(); !sqlAux->Eof; sqlAux->Next())
            sfnMap.insert(std::pair<std::string, int>(sqlAux->Fields[0]->AsString.c_str(), sqlAux->Fields[1]->AsInteger));
        sqlAux->Close();
        sqlAux->SQL->Text = "";
    }

    AnsiString scId;
    int dabScId = 0;
    int dvbScId = 0;
    int tvaScId = 0;
    if (m_importMode == imGs1Gt1 || m_importMode == imGs2Gt2)
    {
        sqlAux->SQL->Text = "select ID, ENUMVAL from SYSTEMCAST";
        for (sqlAux->ExecQuery(); !sqlAux->Eof; sqlAux->Next())
        {
            switch (sqlAux->Fields[1]->AsInteger)
            {
                case ttAllot: scId = sqlAux->Fields[0]->AsString; break;
                case ttTV:  tvaScId = sqlAux->Fields[0]->AsInteger; break;
                case ttDVB: dvbScId = sqlAux->Fields[0]->AsInteger; break;
                case ttDAB: dabScId = sqlAux->Fields[0]->AsInteger; break;
            }
        }

        if (m_importMode == imGs2Gt2 && scId.IsEmpty())
            throw *(new Exception("Не могу найти тип объекта ВЫДЕЛЕНИЕ ("+IntToStr(ttAllot)+") в таблице SYSTEMCAST"));
        if (m_importMode == imGs1Gt1 && dabScId == 0)
            LogError("Не могу найти тип объекта ПРИСВОЕНИЕ DAB ("+IntToStr(ttDAB)+") в таблице SYSTEMCAST");
        if (m_importMode == imGs1Gt1 && dvbScId == 0)
            LogError("Не могу найти тип объекта ПРИСВОЕНИЕ DVB ("+IntToStr(ttDVB)+") в таблице SYSTEMCAST");
        if (m_importMode == imGs1Gt1 && tvaScId == 0)
            LogError("Не могу найти тип объекта ПРИСВОЕНИЕ ЕМФ ("+IntToStr(ttTV)+") в таблице SYSTEMCAST");

        sqlAux->Close();

        sqlAux->SQL->Text = "select (FREQFROM + FREQTO) / 2, ID, FREQCARRIERVISION from CHANNELS";
        for (sqlAux->ExecQuery(); !sqlAux->Eof; sqlAux->Next())
        {
            double freq = sqlAux->Fields[0]->AsDouble;
            if (freq != 0)
                chnMap.insert(std::pair<double, int>(freq, sqlAux->Fields[1]->AsInteger));
            freq = sqlAux->Fields[2]->AsDouble;
            if (freq != 0)
                chnMap.insert(std::pair<double, int>(freq, sqlAux->Fields[1]->AsInteger));
        }
        sqlAux->Close();

        sqlAux->SQL->Text = "select  CENTREFREQ, ID from BLOCKDAB";
        for (sqlAux->ExecQuery(); !sqlAux->Eof; sqlAux->Next())
        {
            double freq = sqlAux->Fields[0]->AsDouble;
            if (freq != 0)
                blkMap.insert(std::pair<double, int>(freq, sqlAux->Fields[1]->AsInteger));
        }
        sqlAux->Close();

        sqlAux->SQL->Text = "select  NAMESYSTEM, ID from DIGITALTELESYSTEM";
        for (sqlAux->ExecQuery(); !sqlAux->Eof; sqlAux->Next())
        {
            std::string sys_type = sqlAux->Fields[0]->AsString.c_str();
            if (!sys_type.empty())
                sysTypeMap.insert(std::pair<std::string, int>(sys_type, sqlAux->Fields[1]->AsInteger));
        }
        sqlAux->Close();

        sqlAux->SQL->Text = "";
    }

    int adminId = dmMain->getCountryId((char*)t_adm.c_str());

    std::vector<int> processedAllots;

    int noticeNumber = 0;
    errorCount = 0;

    lblPb->Visible = true;
    pb->Max = noticeCount;
    pb->Position = 0;
    pb->Visible = true;

    char oldDecimalSeparator = DecimalSeparator;
    DecimalSeparator = '.';

    try
    {
        for (std::vector<XmlDocNode*>::iterator iNode = inputFile.rootNodes.begin(); iNode != inputFile.rootNodes.end(); iNode++)
        {
            if ( (*iNode)->GetName() == "NOTICE" )
            {
                std::vector <int> contours;
                typedef std::map<std::string, std::string> ElemMap;
                ElemMap elemMap;

                XmlDocNode::Elements& els = (*iNode)->GetElements();
                XmlDocNode::Elements::iterator el;
                for ( el = els.begin(); el != els.end(); el++ )
                {
                    elemMap[el->first] = el->second;
                    if (el->first == "t_contour_id")
                    {
                        try {
                            contours.push_back(AnsiString(el->second.c_str()).ToInt());
                        } catch(Exception &e) {
                            LogError(AnsiString().sprintf("Ошибка обработки t_contour_id = '%s' (NOTICE %d)",
                                                        el->second.c_str(), noticeNumber));  
                        }
                    }
                }

                std::string t_notice_type = elemMap["t_notice_type"];
                bool processNotice = m_importMode == imGa1 && t_notice_type == "GA1"
                    || m_importMode == imGs1Gt1 && (t_notice_type == "GT1" || t_notice_type == "GS1"
                                                 || t_notice_type == "G02" || t_notice_type == "GB1")
                    || m_importMode == imGs2Gt2 && (t_notice_type == "GT2" || t_notice_type == "GS2");

                if (processNotice)
                {
                    noticeNumber++;
                    lblPb->Caption = AnsiString().sprintf("%d iз %d", noticeNumber, noticeCount);
                    lblPb->Update();
                    pb->StepIt();
                    pb->Update();

                    if (constrained[noticeNumber - 1])
                        continue;

                    // установить значения переменных (тексты запросов)
                    noticeSql->SQL->Text = "";
                    sqlAux->SQL->Text = "";
                    int id = 0;
                    int siteId = 0;
                    std::string t_action = elemMap["t_action"];
                    std::string t_trg_adm_ref_id = elemMap["t_trg_adm_ref_id"];
                    switch (m_importMode)
                    {
                      case imGa1:
                        if (t_action == "ADD")
                        {
                            id = dmMain->getNewId();
                            noticeSql->SQL->Text = contIns;
                            sqlAux->SQL->Text = "insert into DIG_SUBAREAPOINT (CONTOUR_ID, POINT_NO, LON, LAT) "
                                                " values (" + IntToStr(id) + ", :POINT_NO, :LON, :LAT) ";
                        }
                        else if (t_action == "SUPPRESS")
                        {
                            noticeSql->SQL->Text = contDel;
                            //sqlAux->SQL->Text = "delete from DIG_SUBAREAPOINT where CONTOUR_ID = :ID";
                            // points are being deleted by foreign key
                        }
                        break;
                      case imGs1Gt1:
                        // if no such country - we cannot continue with adding
                        if (t_action == "MODIFY")
                        {
                            if (t_trg_adm_ref_id.empty())
                            {
                                LogError(AnsiString().sprintf("При изменении присвоения не указан t_trg_adm_ref_id (NOTICE %d)", noticeNumber));
                                continue;
                            }
                            noticeSql->SQL->Text = sqlAssgn->ModifySQL->Text;
                            /* find record. if no, insert it*/
                            sqlAux->SQL->Text = "select ID from TRANSMITTERS where ADM_ID = "+IntToStr(adminId)+
                                                " and ADM_REF_ID = '" + t_trg_adm_ref_id.c_str() + "' and STATUS = "+IntToStr(dbSection);
                            sqlAux->ExecQuery();
                            if (!sqlAux->Eof)
                                id = sqlAux->Fields[0]->AsInteger;
                            else // (record not found) - id == 0 and allot will be inserted by following instructions.
                                ;//t_action = "ADD";
                            sqlAux->Close();
                        }
                        if (t_action == "ADD" || (t_action == "MODIFY" && id == 0))
                        {
                            // find site
                            // if not - create new one
                            double lon = atolon(elemMap["t_long"].c_str());
                            double lat = atolat(elemMap["t_lat"].c_str());

                            sqlAux->Close();
                            sqlAux->SQL->Text = "select ID from STAND where NAMESITE_ENG = :NAME and "
                                                "LONGITUDE = :LON and LATITUDE = :LAT and HEIGHT_SEA = :ALT "
                                                "and (AREA_ID <= 228800 or AREA_ID >= 642942)";
                            sqlAux->ParamByName("NAME")->AsString = elemMap["t_site_name"].c_str();
                            sqlAux->ParamByName("LON")->AsDouble = lon;
                            sqlAux->ParamByName("LAT")->AsDouble = lat;
                            sqlAux->ParamByName("ALT")->AsInteger = ::atoi(elemMap["t_site_alt"].c_str());
                            sqlAux->ExecQuery();
                            if (!sqlAux->Eof)
                                siteId = sqlAux->Fields[0]->AsInteger;
                            else // create new site
                            {
                                // find AREA
                                int areaId = 0;
                                sqlAux->Close();
                                sqlAux->SQL->Text = "select ID from AREA where COUNTRY_ID = "+IntToStr(adminId);
                                sqlAux->ExecQuery();
                                if (!sqlAux->Eof)
                                    areaId = sqlAux->Fields[0]->AsInteger;
                                else // no area
                                {
                                    //create new one
                                    sqlAux->Close();
                                    areaId = dmMain->getNewId();
                                    sqlAux->SQL->Text = "insert into AREA (ID, COUNTRY_ID, NAME) values ("+
                                    IntToStr(areaId)+", "+IntToStr(adminId)+", '"+elemMap["t_ctry"].c_str()+"')";
                                    sqlAux->ExecQuery();
                                }

                                sqlAux->Close();
                                sqlAux->SQL->Text = "INSERT INTO STAND (ID,NAMESITE,NAMESITE_ENG,"
                                    "LATITUDE,LONGITUDE,HEIGHT_SEA,AREA_ID,CITY_ID,STREET_ID,DISTRICT_ID,ADDRESS)"
                                    " VALUES (:ID,:NAME,:NAME,:LAT,:LON,:ALT,:AREA_ID,-1,-1,-1,'<unknown>')";
                                siteId = dmMain->getNewId();
                                sqlAux->ParamByName("ID")->AsInteger = siteId;
                                sqlAux->ParamByName("NAME")->AsString = elemMap["t_site_name"].c_str();
                                sqlAux->ParamByName("LAT")->AsDouble = lat;
                                sqlAux->ParamByName("LON")->AsDouble = lon;
                                sqlAux->ParamByName("ALT")->AsInteger = ::atoi(elemMap["t_site_alt"].c_str());
                                sqlAux->ParamByName("AREA_ID")->AsInteger = areaId;
                                sqlAux->ExecQuery();
                                sqlAux->Close();
                                LogError(AnsiString().sprintf("Не найдена опора для t_adm_ref_id = %s. Добавлена новая (NOTICE %d)",
                                    elemMap["t_adm_ref_id"].c_str(), noticeNumber));
                            }

                            //insert clause
                            noticeSql->SQL->Text = sqlAssgn->InsertSQL->Text;
                            id = dmMain->getNewId();
                        }
                        // find site by coordinates and check it's properties
                        break;
                      case imGs2Gt2:
                        if (t_action.substr(0,3) == "MOD")
                        {
                            noticeSql->SQL->Text = allotUpd;
                            if (t_trg_adm_ref_id.empty())
                            {
                                LogError(AnsiString().sprintf("При изменении присвоения не указан t_trg_adm_ref_id (NOTICE %d)", noticeNumber));
                                continue;
                            }
                            /* find record. if no, insert it*/
                            sqlAux->SQL->Text = "select ID from DIG_ALLOTMENT where ADM_ID = "+IntToStr(adminId)+
                                                " and ADM_REF_ID = '" + t_trg_adm_ref_id.c_str() + "' and DB_SECTION_ID = "+IntToStr(dbSection);
                            sqlAux->ExecQuery();
                            if (!sqlAux->Eof)
                                id = sqlAux->Fields[0]->AsInteger;
                           else // (record not found) - id == 0 and allot will be inserted by following instructions.
                                t_action = "ADD";
                            sqlAux->Close();
                        }
                        if (t_action == "ADD" || (t_action.substr(0,3) == "MOD" && id == 0))
                        {
                            noticeSql->SQL->Text = allotIns;
                            id = dmMain->getNewId();
                            sqlAux->SQL->Text = "insert into TRANSMITTERS (ID, LATITUDE, LONGITUDE, SYSTEMCAST_ID, STATUS) "
                                "VALUES ("+IntToStr(id)+", 0.0, 0.0, "+scId+","+IntToStr(dbSection)+")";
                        }
                        break;
                    }

                    if (noticeSql->SQL->Text.IsEmpty())
                    {
                        LogError(AnsiString().sprintf("Нераспознанный t_action = %s (NOTICE %d)", t_action.c_str(), noticeNumber));
                    }

                    if (noticeSql->SQL->Text.IsEmpty())
                    {
                        LogError(AnsiString().sprintf("Не могу определить порядок импорта (NOTICE %d)", noticeNumber));
                    }

                    if (!noticeSql->Prepared)
                        noticeSql->Prepare();

                    // clear all parameters
                    for (int i = 0; i < noticeSql->Params->Count; i++)
                        noticeSql->Params->Vars[i]->Clear();

                    // set up not ordinar params for all types of notices
                    // check availability of exact param via paramlist
                    std::auto_ptr<TStringList> pl(new TStringList());
                    pl->DelimitedText = noticeSql->Params->Names;
                    if (pl->IndexOf("ID") != -1)
                        noticeSql->ParamByName("ID")->AsInteger = id;
                    if (pl->IndexOf("DB_SECTION_ID") != -1)
                        noticeSql->ParamByName("DB_SECTION_ID")->AsInteger = dbSection;
                    if (pl->IndexOf("STATUS") != -1)
                        noticeSql->ParamByName("STATUS")->AsInteger = dbSection;
                    noticeSql->ParamByName("ADM_ID")->AsInteger = adminId;
                    if (pl->IndexOf("TYPEOFFSET") != -1)
                        noticeSql->ParamByName("TYPEOFFSET")->AsString = "Normal";
                    // notice type
                    if (m_importMode == imGs1Gt1)
                    {
                        if (pl->IndexOf("SYSTEMCAST_ID") != -1)
                        {
                            if (t_notice_type == "GS1")
                                noticeSql->ParamByName("SYSTEMCAST_ID")->AsInteger = dabScId;
                            else if (t_notice_type == "GT1")
                                noticeSql->ParamByName("SYSTEMCAST_ID")->AsInteger = dvbScId;
                            else if (t_notice_type == "G02")
                                noticeSql->ParamByName("SYSTEMCAST_ID")->AsInteger = tvaScId;
                            else // assume GB1
                                noticeSql->ParamByName("SYSTEMCAST_ID")->AsInteger = dvbScId;
                        }
                        if (pl->IndexOf("STAND_ID") != -1)
                            noticeSql->ParamByName("STAND_ID")->AsInteger = siteId;
                        if (pl->IndexOf("IS_RESUB") != -1)
                            noticeSql->ParamByName("IS_RESUB")->AsInteger = 0;
                        if (pl->IndexOf("REMARK_CONDS_MET") != -1)
                            noticeSql->ParamByName("REMARK_CONDS_MET")->AsInteger = 0;
                        if (pl->IndexOf("SIGNED_COMMITMENT") != -1)
                            noticeSql->ParamByName("SIGNED_COMMITMENT")->AsInteger = 0;
                        if (pl->IndexOf("EPR_VIDEO_MAX") != -1)
                            noticeSql->ParamByName("EPR_VIDEO_MAX")->AsDouble = -999;
                        if (pl->IndexOf("EPR_SOUND_MAX_PRIMARY") != -1)
                            noticeSql->ParamByName("EPR_SOUND_MAX_PRIMARY")->AsDouble = -999;
                        if (pl->IndexOf("RPC") != -1)
                            noticeSql->ParamByName("RPC")->AsInteger = rpc0;
                    }
                    if (m_importMode == imGs2Gt2)
                    {
                        if (pl->IndexOf("nb_sub_areas") != -1)
                            noticeSql->ParamByName("nb_sub_areas")->AsInteger = contours.size();
                    }

                    // параметры основного запроса - тот, что непосредственно к <NOTICE> относится
                    for ( el = els.begin(); el != els.end(); el++ )
                    {
                        if (el->first.size() > 0 && !el->second.empty())
                        {
                            try {
                                if (el->first == "t_freq_assgn")
                                {
                                    double freq = AnsiString(el->second.c_str()).ToDouble();
                                    if (t_notice_type == "GS2")
                                    {
                                        std::map<double, int>:: iterator i
                                         = blkMap.find(freq);
                                        if (i != blkMap.end())
                                            noticeSql->ParamByName("BLOCKDAB_ID")->AsInteger = i->second;
                                        noticeSql->ParamByName("FREQ_ASSIGN")->AsDouble = freq;
                                    } else if (t_notice_type == "GT2")
                                    {
                                        std::map<double, int>:: iterator i
                                         = chnMap.find(freq);
                                        if (i != chnMap.end())
                                            noticeSql->ParamByName("CHANNEL_ID")->AsInteger = i->second;
                                        noticeSql->ParamByName("FREQ_ASSIGN")->AsDouble = freq;    
                                    } else if (t_notice_type == "GS1")
                                    {
                                        noticeSql->ParamByName("BLOCKCENTREFREQ")->AsString = el->second.c_str();
                                        std::map<double, int>:: iterator i
                                         = blkMap.find(freq);
                                        if (i != blkMap.end())
                                            noticeSql->ParamByName("ALLOTMENTBLOCKDAB_ID")->AsInteger = i->second;
                                        else
                                            LogError(AnsiString().sprintf("Не найден блок по частоте '%s': (NOTICE %d)",
                                            el->second.c_str(), noticeNumber));
                                    } else //if (t_notice_type == "GT1/GB1/G02")
                                    {
                                        noticeSql->ParamByName("VIDEO_CARRIER")->AsString = el->second.c_str();
                                        std::map<double, int>:: iterator i
                                         = chnMap.find(freq);
                                        if (i != chnMap.end())
                                        {
                                            noticeSql->ParamByName("CHANNEL_ID")->AsInteger = i->second;
                                            if (t_notice_type == "G02" || t_notice_type == "GB1")
                                            {
                                                std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
                                                sql->Database = dmMain->dbMain;
                                                sql->SQL->Text = "select FREQCARRIERVISION, FREQCARRIERSOUND, "
                                                                   " FREQFROM, FREQTO from CHANNELS where ID = " +
                                                                    IntToStr(i->second);

                                                sql->ExecQuery();
                                                noticeSql->ParamByName("VIDEO_CARRIER")->AsDouble = sql->FieldByName("FREQCARRIERVISION")->AsDouble;
                                                noticeSql->ParamByName("SOUND_CARRIER_PRIMARY")->AsDouble = sql->FieldByName("FREQCARRIERSOUND")->AsDouble;
                                                noticeSql->ParamByName("CARRIER")->AsDouble = sql->FieldByName("FREQCARRIERVISION")->AsDouble;
                                                noticeSql->ParamByName("BANDWIDTH")->AsDouble = sql->FieldByName("FREQTO")->AsDouble - sql->FieldByName("FREQFROM")->AsDouble;
                                            }
                                        } else
                                            LogError(AnsiString().sprintf("Не найден канал по частоте '%s': (NOTICE %d)",
                                            el->second.c_str(), noticeNumber));
                                    }

                                } else if (el->first == "t_sfn_id")
                                {
                                    std::map<std::string, int>:: iterator i = sfnMap.find(el->second);
                                    if (i != sfnMap.end())
                                        noticeSql->ParamByName("SFN_ID_FK")->AsInteger = i->second;
                                    else
                                        LogError(AnsiString().sprintf("Не найдена ОЧС '%s': (NOTICE %d)",
                                            el->second.c_str(), noticeNumber));

                                } else if (el->first == "t_sys_var")
                                {
                                    std::map<std::string, int>:: iterator i = sysTypeMap.find(el->second);
                                    if (i != sysTypeMap.end())
                                    {
                                        //noticeSql->ParamByName("TYPESYSTEM_ID")->AsInteger = i->second;
                                        noticeSql->ParamByName("TYPESYSTEM")->AsInteger = i->second;
                                    }
                                    else
                                        LogError(AnsiString().sprintf("Не найдена система DVB '%s': (NOTICE %d)",
                                            el->second.c_str(), noticeNumber));

                                } else if (el->first == "t_ref_plan_cfg" && m_importMode == imGs1Gt1)
                                {
                                    noticeSql->ParamByName("RPC")->AsInteger = el->second[3] - '1';
                                } else if (el->first == "t_rx_mode" && m_importMode == imGs1Gt1)
                                {
                                    TBcRxMode rxMode =
                                    el->second == "FX" ? rmFx :
                                    el->second == "MO" ? rmMo :
                                    el->second == "PI" ? rmPi :
                                    el->second == "PO" ? rmPo : -1;
                                    if (rxMode != -1)
                                        noticeSql->ParamByName("RX_MODE")->AsInteger = rxMode;
                                    else
                                        LogError(AnsiString().sprintf("Неизвестный %s = '%s' (NOTICE %d)",
                                        el->first.c_str(), el->second.c_str(), noticeNumber));
                                } else if (el->first == "t_long" && m_importMode == imGs1Gt1)
                                {
                                    noticeSql->ParamByName("LONGITUDE")->AsDouble = atolon(elemMap["t_long"].c_str());
                                } else if (el->first == "t_lat" && m_importMode == imGs1Gt1)
                                {
                                    noticeSql->ParamByName("LATITUDE")->AsDouble = atolat(elemMap["t_lat"].c_str());
                                }
                                else if (el->first == "t_oset_v_12")
                                {
                                    int lines = atof(el->second.c_str()) / 12.0;
                                    noticeSql->ParamByName("VIDEO_OFFSET_LINE")->AsInteger = lines;
                                    noticeSql->ParamByName("VIDEO_OFFSET_HERZ")->AsInteger = lines * 1300;
                                }
                                else if (el->first == "t_oset_v_khz")
                                {
                                    double khz = atof(el->second.c_str());
                                    noticeSql->ParamByName("VIDEO_OFFSET_HERZ")->AsInteger = khz * 1000;
                                    noticeSql->ParamByName("VIDEO_OFFSET_LINE")->AsInteger = khz / 1.3 * 12;
                                }
                                //else if (el->first == "t_oset_s_12")
                                //    noticeSql->ParamByName("
                                else if (el->first == "t_oset_s_khz")
                                    noticeSql->ParamByName("SOUND_OFFSET_PRIMARY")->AsInteger = atof(el->second.c_str()) * 1000;
                                else if (el->first == "t_freq_stabl")
                                    noticeSql->ParamByName("FREQSTABILITY")->AsString = el->second.c_str();
                                else if (el->first == "t_tran_sys")
                                {
                                    std::auto_ptr<TIBSQL> sql(new TIBSQL(this)); sql->Transaction = noticeSql->Transaction;
                                    sql->SQL->Text = String("select ID from ANALOGTELESYSTEM where NAMESYSTEM = '")+el->second.c_str()+'\'';
                                    sql->ExecQuery();
                                    if (!sql->Eof)
                                        noticeSql->ParamByName("TYPESYSTEM")->AsInteger = sql->Fields[0]->AsInteger;
                                    else
                                        LogError(AnsiString().sprintf("Неизвестный %s = '%s' (NOTICE %d)",
                                        el->first.c_str(), el->second.c_str(), noticeNumber));
                                }
                                else if (el->first == "t_color")
                                    noticeSql->ParamByName("SYSTEMCOLOUR")->AsString =
                                                                        el->second == "P" ? "PAL" :
                                                                        el->second == "S" ? "SECAM" : "";
                                else if (el->first == "t_pwr_ratio")
                                    noticeSql->ParamByName("V_SOUND_RATIO_PRIMARY")->AsDouble = atof(el->second.c_str());
                                else if ((el->first == "t_offset") && (pl->IndexOf("VIDEO_OFFSET_HERZ") != -1))
                                    noticeSql->ParamByName("VIDEO_OFFSET_HERZ")->AsInteger = atof(el->second.c_str()) * 1000;

                                // собственно сам поиск поля и его вставка по умолчанию
                                else
                                {
                                  std::map<std::string, AnsiString>::iterator i = elFldNames.find(el->first);
                                  if (i != elFldNames.end() && i->second.Length() > 0)
                                  {
                                    AnsiString fn = i->second;
                                    if (pl->IndexOf(fn) != -1)
                                    {
                                        switch (noticeElements[elIndices[el->first]].fldType)
                                        {
                                          case 'B':
                                            noticeSql->ParamByName(fn)->AsInteger = el->second == "TRUE";
                                            break;
                                          case 'D':
                                            noticeSql->ParamByName(fn)->AsDate = EncodeDate(
                                                el->second[0]*1000+el->second[1]*100+el->second[2]*10+el->second[3],
                                                el->second[5]*10+el->second[6],
                                                el->second[8]*10+el->second[9]);
                                            break;
                                          case 'T':
                                            noticeSql->ParamByName(fn)->AsTime = EncodeTime(
                                                el->second[0]*10+el->second[1],
                                                el->second[2]*10+el->second[3],
                                                0, 0);
                                            break;
                                          default:
                                            if ((fn == "EPR_SOUND_HOR_PRIMARY" || fn == "EPR_SOUND_VERT_PRIMARY")
                                            //if ((fn == "EPR_VIDEO_HOR" || fn == "EPR_VIDEO_VERT")
                                                && el->second.size() > 0)
                                            {
                                                // convert dbw to dbkw
                                                double val = AnsiString(el->second.c_str()).ToDouble() - 30;
                                                TIBXSQLVAR *erp = noticeSql->ParamByName(fn);
                                                TIBXSQLVAR *maxErp = noticeSql->ParamByName("EPR_SOUND_MAX_PRIMARY");
                                                if (t_notice_type == "G02")
                                                {
                                                    erp = noticeSql->ParamByName((fn == "EPR_SOUND_HOR_PRIMARY") ?
                                                                            "EPR_VIDEO_HOR" : "EPR_VIDEO_VERT");
                                                    maxErp = noticeSql->ParamByName("EPR_VIDEO_MAX");
                                                }
                                                erp->AsDouble = val;
                                                if (maxErp->AsDouble < val)
                                                    maxErp->AsDouble = val;
                                                if (t_notice_type == "G02")
                                                {
                                                    TIBXSQLVAR *erpSound = noticeSql->ParamByName(fn);
                                                    TIBXSQLVAR *maxErp = noticeSql->ParamByName("EPR_SOUND_MAX_PRIMARY");
                                                    val -= atof(elemMap["t_pwr_ratio"].c_str());
                                                    erpSound->AsDouble = val;
                                                    if (maxErp->AsDouble < val)
                                                        maxErp->AsDouble = val;
                                                }
                                            } else
                                                // all others
                                                noticeSql->ParamByName(fn)->AsString = el->second.c_str();

                                            break;
                                        }
                                    }
                                  }  
                                }

                            } catch (Exception &e) {
                                LogError(AnsiString().sprintf("Ошибка установки значения поля %s: '%s' (NOTICE %d)",
                                        el->first.c_str(), e.Message.c_str(), noticeNumber));
                            }
                        }
                    }
                    if ((pl->IndexOf("assgn_code") != -1) && (noticeSql->ParamByName("assgn_code")->IsNull))
                        noticeSql->ParamByName("assgn_code")->AsString = "S";

                    // <COORD section>
                    std::vector<XmlDocNode*> nodes = (*iNode)->GetSubNodes();
                    std::vector<XmlDocNode*>::iterator node;
                    std::string coord;
                    for ( node = nodes.begin(); node != nodes.end(); node++ )
                    {
                        std::string nn = (*node)->GetName();
                        if (nn == "COORD" || nn == "COORDINATION")
                        {
                            XmlDocNode::Elements& els = (*node)->GetElements();
                            XmlDocNode::Elements::iterator el;
                            for ( el = els.begin(); el != els.end(); el++ )
                            {
                                if (el->first == "t_adm")
                                {
                                    if (coord.size() > 0)
                                        coord += ' ';
                                    coord += el->second;
                                }
                            }
                        }
                    }
                    if (pl->IndexOf("COORD") > -1)
                        noticeSql->ParamByName("COORD")->AsString = coord.c_str();

                    if (m_importMode == imGs1Gt1)
                    {
                        //set vector data
                        std::vector<XmlDocNode*> nodes = (*iNode)->GetSubNodes();

                        std::vector<XmlDocNode*>::iterator node;

                        bool missedHorErp = true;
                        bool missedVerErp = true;

                        for ( node = nodes.begin(); node != nodes.end(); node++ )
                        {
                            std::string nn = (*node)->GetName();
                            //std::vector<XmlDocNode*>::iterator
                            //double effAntHgt[36]; memset(effAntHgt, 0, 36);
                            //double diagrH[36]; memset(diagrH, 0, 36);
                            //double diagrV[36]; memset(diagrV, 0, 36);
                            //double erpH[36]; memset(erpH, 0, 36);
                            //double erpV[36]; memset(erpV, 0, 36);

                            if (nn == "ANT_HGT")
                                SetVector(node, "t_eff_hgt@azm", noticeSql.get(), "EFFECTHEIGHT", 1, 0);
                            if (nn == "ANT_DIAGR_H") {
                                SetVector(node, "t_attn@azm", noticeSql.get(), "ANT_DIAG_H", -1, 0);
                                SetVector(node, "t_attn@azm", noticeSql.get(), "EFFECTPOWERHOR", -1,
                                                noticeSql->ParamByName("EPR_SOUND_HOR_PRIMARY")->AsDouble);
                                missedHorErp = false;
                            }
                            if (nn == "ANT_DIAGR_V") {
                                SetVector(node, "t_attn@azm", noticeSql.get(), "ANT_DIAG_V", -1, 0);
                                SetVector(node, "t_attn@azm", noticeSql.get(), "EFFECTPOWERVER", -1,
                                                noticeSql->ParamByName("EPR_SOUND_VERT_PRIMARY")->AsDouble);
                                missedVerErp = false;
                            }
                        }

                        // if antenna is ND and has no diagram, something is going wrong. Hack it.
                        if ((noticeSql->ParamByName("DIRECTION")->AsString != "D"))
                        {
                            const int ARRLEN = 36;
                            double dblArray[ARRLEN];
                            std::auto_ptr<TMemoryStream> memstream (new TMemoryStream());

                            if (missedHorErp)
                            {
                                for (int i=0; i<ARRLEN; i++) dblArray[i] = 0.0;
                                memstream->Clear();
                                memstream->Write(dblArray, sizeof(double)*ARRLEN);
                                noticeSql->ParamByName("ANT_DIAG_H")->LoadFromStream(memstream.get());

                                for (int i=0; i<ARRLEN; i++) dblArray[i] = noticeSql->ParamByName("EPR_SOUND_HOR_PRIMARY")->AsDouble;
                                memstream->Clear();
                                memstream->Write(dblArray, sizeof(double)*ARRLEN);
                                noticeSql->ParamByName("EFFECTPOWERHOR")->LoadFromStream(memstream.get());
                            }
                            if (missedVerErp)
                            {
                                for (int i=0; i<ARRLEN; i++) dblArray[i] = 0.0;
                                memstream->Clear();
                                memstream->Write(dblArray, sizeof(double)*ARRLEN);
                                noticeSql->ParamByName("ANT_DIAG_V")->LoadFromStream(memstream.get());

                                for (int i=0; i<ARRLEN; i++) dblArray[i] = noticeSql->ParamByName("EPR_SOUND_VERT_PRIMARY")->AsDouble;
                                memstream->Clear();
                                memstream->Write(dblArray, sizeof(double)*ARRLEN);
                                noticeSql->ParamByName("EFFECTPOWERVER")->LoadFromStream(memstream.get());
                            }
                        }

                    }

                    // lauhcn main query
                    try {

                        noticeSql->ExecQuery();
                        if (!sqlAux->SQL->Text.IsEmpty() && !sqlAux->Prepared)
                            sqlAux->Prepare();

                    } catch (Exception &e) {
                        LogError(AnsiString().sprintf("Ошибка выполнения запроса импорта: %s (NOTICE %d)",
                                        e.Message.c_str(), noticeNumber));
                        noticeSql->Close();
                        continue;
                    }

                    // additional actions

                    switch (m_importMode)
                    {
                      case imGa1:
                        // delete or insert points
                        try {
                            if (t_action == "SUPPRESS")
                            {
                                // this should be done automatically
                            }
                            if (t_action == "ADD")
                            {
                                std::vector<XmlDocNode*> nodes = (*iNode)->GetSubNodes();
                                std::vector<XmlDocNode*>::iterator node;
                                int pointNo = 0;
                                for ( node = nodes.begin(); node != nodes.end(); node++ )
                                {
                                    std::string nn = (*node)->GetName();
                                    if (nn != "POINT")
                                        continue;

                                    XmlDocNode::Elements& coords = (*node)->GetElements();
                                    XmlDocNode::Elements::iterator coord;

                                    std::string t_long; std::string t_lat;

                                    for ( coord = coords.begin(); coord != coords.end(); coord++ )
                                    {
                                        if (coord->first == "t_lat")
                                            t_lat = coord->second;
                                        if (coord->first == "t_long")
                                            t_long = coord->second;
                                    }

                                    // TODO: check lat & long
                                    double lat = atolat(t_lat.c_str());
                                    double lon = atolon(t_long.c_str());

                                    try {
                                        if (!sqlAux->Prepared)
                                            sqlAux->Prepare();
                                        pointNo++;
                                        sqlAux->ParamByName("POINT_NO")->AsInteger = pointNo;
                                        sqlAux->ParamByName("LAT")->AsDouble = lat;
                                        sqlAux->ParamByName("LON")->AsDouble = lon;
                                        sqlAux->ExecQuery();

                                    } catch (Exception &e) {
                                        LogError(AnsiString().sprintf("Ошибка добавления точки контура: %s (NOTICE %d)",
                                                e.Message.c_str(), noticeNumber));
                                    }
                                }
                            }
                        } catch (Exception &e) {
                            LogError(AnsiString().sprintf("Ошибка при записи точек контура: %s (NOTICE %d)",
                                        e.Message.c_str(), noticeNumber));
                        }
                        break;
                      case imGs2Gt2:
                        if (t_action == "ADD")
                        {
                            try {
                                if (!sqlAux->SQL->Text.IsEmpty())
                                {
                                    sqlAux->ExecQuery();
                                }
                            } catch (Exception &e) {
                                LogError(AnsiString().sprintf("Ошибка записи доп. информации при импорте выделения: %s (NOTICE %d)",
                                        e.Message.c_str(), noticeNumber));
                                /* tx cannot be inserted - delete allotment */
                                sqlAux->SQL->Text = "delete from dig_allotment where id = " + IntToStr(id);
                                sqlAux->ExecQuery();

                                continue;
                            }

                            // links to contours
                            if (contours.size() > 0)
                            try {
                                // drop all links to this allotment
                                sqlAux->SQL->Text = "delete from DIG_ALLOT_CNTR_LNK where ALLOT_ID = " + IntToStr(id);
                                sqlAux->ExecQuery();
                                sqlAux->SQL->Text = "insert into DIG_ALLOT_CNTR_LNK (ALLOT_ID, CNTR_ID) values (" +
                                                    IntToStr(id) + ", :CNTR_ID)";
                                sqlAux->Prepare();

                                if (elemMap["t_geo_area"].size() > 0)
                                {
                                    LogError(AnsiString().sprintf("Указан как тег t_geo_area, так и список контуров. Контура игнорированы (NOTICE %d)", noticeNumber));
                                } else {

                                    std::auto_ptr<TIBSQL> sqlContour(new TIBSQL(this));
                                    sqlContour->Database = sqlAux->Database;
                                    sqlContour->Transaction = sqlAux->Transaction;
                                    sqlContour->SQL->Text = "select id from DIG_CONTOUR where ADM_ID = "+IntToStr(adminId)+
                                                            " and CONTOUR_ID = :CONTOUR_ID and DB_SECTION_ID = "+IntToStr(dbSection);
                                    sqlContour->Prepare();
                                    // for every entry in vector
                                    for (std::vector<int>::iterator vi = contours.begin(); vi < contours.end(); vi++)
                                    {
                                        // find contour by id, adm and db_section
                                        sqlContour->Params->Vars[0]->AsInteger = *vi;
                                        sqlContour->ExecQuery();
                                        // if not - log error
                                        if (sqlContour->Eof)
                                            LogError(AnsiString().sprintf("Контур не найден: t_contour = %04d (NOTICE %d)",
                                                *vi, noticeNumber));
                                        else
                                        {
                                            // insert link
                                            sqlAux->Params->Vars[0]->AsInteger = sqlContour->Fields[0]->AsInteger;
                                            sqlAux->ExecQuery();
                                        }
                                        sqlContour->Close();
                                        sqlAux->Close();
                                    }
                                }
                                // cleart contours vector
                            } catch (Exception &e)
                            {
                                LogError(AnsiString().sprintf("Ошибка привязки контуров: %s (NOTICE %d)",
                                        e.Message.c_str(), noticeNumber));
                            }

                            // if we here, we've inserted/updated allotment
                            processedAllots.push_back(id);
                        }
                        sqlAux->Close();
                        sqlAux->SQL->Text = AnsiString("update TRANSMITTERS set COORD = '")+coord.c_str()+"' where ID = "+IntToStr(id);
                        sqlAux->ExecQuery();
                        sqlAux->Close();

                        break;
                    }

                    noticeSql->Close();
                    sqlAux->Close();

                }

            }
        }
        btnImport->Enabled = false;

        lblPb->Visible = false;
        if (errorCount > 0)
            lblError->Visible = true;

        if (errorCount == 0 || MessageBox(NULL, AnsiString().sprintf("Iмпорт виконаний з %d помилками (%d NOTICE's). "
                                        "Пiдтвердити?", errorCount, noticeCount).c_str(),
                                        "Попередження", MB_ICONQUESTION | MB_YESNO) == IDYES)
        {
            tr->Commit();
            MessageBox(NULL, AnsiString().sprintf("Оброблено %d записiв, %d помилок.\n"
                "Обновiть вмiст вiдповiдних вiкон", noticeNumber, errorCount).c_str(), "Записано", MB_ICONINFORMATION);

            // invalidirovat' nax vse allotments
            // TODO: add Invalidate method to broker (to not load those ones that are not loaded yet
            for (std::vector<int>::iterator i = processedAllots.begin(); i < processedAllots.end(); i++)
                //txBroker.Invalidate(*i);
                ;
        }

    } __finally {

        DecimalSeparator = oldDecimalSeparator;
        lblPb->Visible = false;
        pb->Position = 0;
        pb->Visible = false;

        if (lblError->Lines->Count > 0)
            lblError->Visible = true;
    }

    if (tr->Active)
        tr->Rollback();
}
//---------------------------------------------------------------------------

void __fastcall TfrmRrc06Import::Clear()
{
    t_adm.clear();
    adminId = 0;
    lblError->Text = "";
    lblError->Visible = false;
    inputFile.Reset();
    btnImport->Enabled = false;
    sfnMap.clear();
    sfnPresent = false;
    blkMap.clear();
    chnMap.clear();
    sysTypeMap.clear();

    contoursData.clear();

    constrained.clear();

    oldIndex = -1;
}
//---------------------------------------------------------------------------

void __fastcall TfrmRrc06Import::SetImportMode(ImportMode value)
{
    if(m_importMode != value) {
        if (value < imGa1 || value >= imLast)
            throw *(new Exception("Недопустимый режим импорта"));
        m_importMode = value;

        Clear();
        elIndices.clear();
        elFldNames.clear();

        Caption = AnsiString().sprintf("Iмпорт %s", modeNames[m_importMode]);

        grdData->RowCount = 2;
        grdData->ColCount = 2;
        grdData->Cells[0][0] = ""; grdData->Cells[1][0] = ""; grdData->Cells[0][1] = ""; grdData->Cells[1][1] = "";

        noticeElements = NULL;

        mapForm->Visible = false;
        Splitter1->Visible = false;
        grdData->Align = alClient;

        switch (m_importMode)
        {
          case imGa1:
            grdData->ColCount = sizeof(neGa1) / sizeof(NoticeElements);
            noticeElements = neGa1;
            grdData->Align = alLeft;
            grdData->Width = 300;
            Splitter1->Left = grdData->Width;
            Splitter1->Visible = true;
            mapForm->Visible = true;
            break;
          case imGs2Gt2:
            grdData->ColCount = sizeof(neGt2Gs2) / sizeof(NoticeElements);
            noticeElements = neGt2Gs2;
            break;
          case imGs1Gt1:
            grdData->ColCount = sizeof(neGs1Gt1) / sizeof(NoticeElements);
            noticeElements = neGs1Gt1;
            break;
        }

        if (noticeElements)
        {
            for (int i = 0; i < grdData->ColCount; i++)
            {
                grdData->Cells[i][0] = noticeElements[i].label;
                grdData->ColWidths[i] = noticeElements[i].width;
                elIndices[noticeElements[i].elName] = i;
                elFldNames[noticeElements[i].elName] = noticeElements[i].fldName;
            }
        }
    }
}
//---------------------------------------------------------------------------

TfrmRrc06Import::ImportMode __fastcall TfrmRrc06Import::GetImportMode()
{
    return m_importMode;
}
//---------------------------------------------------------------------------

void __fastcall TfrmRrc06Import::LogError(AnsiString msg, bool isError)
{
    if (errorCount > 0)
        lblError->Text = lblError->Text + '\r'+'\n';
    lblError->Text = lblError->Text + " - " + msg;
    if (isError)
        errorCount++;
}
//---------------------------------------------------------------------------

void __fastcall TfrmRrc06Import::FormResize(TObject *Sender)
{
    if (mapForm && mapForm->Visible)
        mapForm->Invalidate();
}
//---------------------------------------------------------------------------

void __fastcall TfrmRrc06Import::FormPaint(TObject *Sender)
{
    if (mapForm && mapForm->Visible)
    {
        cd.SetWc(mapForm);
        cd.contours = &contoursData;
        cd.FitContours();
        cd.DrawContours(-1);
        
        for (int i = 0; i < contoursData.size(); i++)
            if (i != grdData->Row - 1)
                cd.DrawContour(contoursData[i], constrained[i] ? clRed : clBlue, clDkGray);

        if (grdData->Row > 0 && contoursData.size() > 0)
        {
            cd.DrawContour(contoursData[grdData->Row - 1], constrained[grdData->Row - 1] ? clFuchsia : clLime, clWindowText);
            oldIndex = grdData->Row;
        }
    }
}
//---------------------------------------------------------------------------

double __fastcall TfrmRrc06Import::atolat(const char* alat)
{
    double lat = (alat[1]-'0')*10+(alat[2]-'0')+
                 1.0 / 60.0 * ((alat[3]-'0')*10+(alat[4]-'0')) +
                 1.0 / 3600.0 * ((alat[5]-'0')*10+(alat[6]-'0'));
    if (alat[0] == '-') lat *= -1;
    return lat;
}

double __fastcall TfrmRrc06Import::atolon(const char* alon)
{
    double lon = (alon[1]-'0')*100+(alon[2]-'0')*10+(alon[3]-'0')+
                 1.0 / 60.0 * ((alon[4]-'0')*10+(alon[5]-'0')) +
                 1.0 / 3600.0 * ((alon[6]-'0')*10+(alon[7]-'0'));
    if (alon[0] == '-') lon *= -1;
    return lon;
}
//---------------------------------------------------------------------------

void __fastcall TfrmRrc06Import::btnConstrSetClick(TObject *Sender)
{
    if (dlgConstrSet == NULL)
        dlgConstrSet = new TdlgConstrSet(Application);
    dlgConstrSet->minLon = minLon;
    dlgConstrSet->minLat = minLat;
    dlgConstrSet->maxLon = maxLon;
    dlgConstrSet->maxLat = maxLat;
    dlgConstrSet->chbOnlyIfContExist->Checked = chckIfContourExists;
    if (dlgConstrSet->ShowModal() == mrOk)
    {
        minLon = dlgConstrSet->minLon;
        minLat = dlgConstrSet->minLat;
        maxLon = dlgConstrSet->maxLon;
        maxLat = dlgConstrSet->maxLat;
        chckIfContourExists = dlgConstrSet->chbOnlyIfContExist->Checked;
        lblConstrContent->Caption = GetConstrText();
    if (!edtFileName->Text.IsEmpty())
        LoadFile(edtFileName->Text);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmRrc06Import::btnConstrClearClick(TObject *Sender)
{
    minLon = -MaxDouble;
    minLat = -MaxDouble;
    maxLon = MaxDouble;
    maxLat = MaxDouble;
    chckIfContourExists = false;
    lblConstrContent->Caption = GetConstrText();
    if (!edtFileName->Text.IsEmpty())
        LoadFile(edtFileName->Text);
}
//---------------------------------------------------------------------------

void __fastcall TfrmRrc06Import::grdDataDrawCell(TObject *Sender, int ACol,
      int ARow, TRect &Rect, TGridDrawState State)
{
    TStringGrid *sg = dynamic_cast<TStringGrid*>(Sender);
    if (!sg)
        return;

    if (ARow > 0)
        sg->Canvas->Font->Color = constrained[ARow - 1] ? clRed : clWindowText;
    if (State.Contains(gdSelected))
        sg->Canvas->Font->Color = constrained[ARow - 1] ? clFuchsia : clWhite;
    sg->Canvas->TextOutA(Rect.left + 1, Rect.top + 1, sg->Cells[ACol][ARow]);
}
//---------------------------------------------------------------------------

AnsiString __fastcall TfrmRrc06Import::GetConstrText()
{
    std::auto_ptr<TCoordinateConvertor> cc(new TCoordinateConvertor(this));
    AnsiString res;
    if (minLon > -MaxDouble)
        res += (", Довгота не менш за " + cc->CoordToStr(minLon, 'X'));
    if (minLat > -MaxDouble)
        res += (", Широта не менш за " + cc->CoordToStr(minLat, 'Y'));
    if (maxLon < MaxDouble)
        res += (", Довгота не бiльш за " + cc->CoordToStr(maxLon, 'X'));
    if (maxLat < MaxDouble)
        res += (", Широта не бiльш за " + cc->CoordToStr(maxLat, 'Y'));
    if (chckIfContourExists)
        res += (", Видiлення тiльки з iснуючими контурами");

    if (!res.IsEmpty())
        res.Delete(1, 2);
    if (res.IsEmpty())
        res = "Нема обмежень";

    return res;
}
//---------------------------------------------------------------------------

bool __fastcall TfrmRrc06Import::CheckPoint(double lon, double lat)
{
    return lon > minLon && lon < maxLon && lat > minLat && lat < maxLat;
}
//---------------------------------------------------------------------------

void __fastcall TfrmRrc06Import::cbxDbSectionChange(TObject *Sender)
{
    if (
        !edtFileName->Text.IsEmpty()
        && (minLon > -MaxDouble || minLat > -MaxDouble ||
            maxLat < MaxDouble || maxLon < MaxDouble || chckIfContourExists)
        )
        LoadFile(edtFileName->Text);
}
//---------------------------------------------------------------------------

