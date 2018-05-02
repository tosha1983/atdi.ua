//---------------------------------------------------------------------------

#pragma hdrstop

#include "LISXMLConv.h"

#pragma package(smart_init)
//---------------------------------------------------------------------------
__fastcall TLISXMLConv::TLISXMLConv(TComponent* Owner, Xmldoc::TXMLDocument* xmlTmp)
{
	tempOwner = Owner;//�� ������, ��� �������� � ������������ �����������
	start = false;
	AnsiString eqvDBNameA = ExtractFileDir(Application->ExeName)+"\\equi.txt";
	eqvDBName = AnsiString(eqvDBNameA);
	TSearchRec sr;
	if(FindFirst(eqvDBNameA, faAnyFile, sr)!=0)//���� ��� ����� ������������� ���
		throw *(new Exception("���� �������� ��� ������ 'equi.txt' �� ������."));
	//xml.reset(new TXMLDocument(eqvDBNameA));
    xml = xmlTmp;
	xml->DOMVendor = GetDOMVendor("MSXML");
	xml->LoadFromFile(eqvDBName);
	//������ ����������� FXM->XML
	fxm_to  = xml->Node->ChildNodes->FindNode(L"EQUI")->ChildNodes->FindNode(L"FXMTOXML");
	//������ �������� ���������
	ds_info = xml->Node->ChildNodes->FindNode(L"EQUI")->ChildNodes->FindNode(L"DATASETS");

	TClientDataSet *cds = NULL;
	for(int i = 0; i < ds_info->ChildNodes->Count; i++)
	{
		cds = new TClientDataSet(tempOwner);
		cds->Name = AnsiString(ds_info->ChildNodes->Nodes[i]->NodeName);
		mTempDS[ds_info->ChildNodes->Nodes[i]->NodeName] = cds;
	}
	LoadCDS();
}
//---------------------------------------------------------------------------

__fastcall TLISXMLConv::~TLISXMLConv()
{
	TClientDataSet *cds = NULL;
	for(std::map<AnsiString, TClientDataSet*>::iterator i = mTempDS.begin();
		i != mTempDS.end();
		i++)
	{//������� ��� ��������� (� �����) ��������
		try
		{
			cds = i->second;
			delete cds;
			cds = NULL;
		}
		catch(Exception &e)
		{
			throw *(new Exception(AnsiString("���������� TLISXMLConv: ��� ������� �������� ������� 'DataSet' ��������� ������.\n"+e.Message).c_str()));
		}
	}
}
//---------------------------------------------------------------------------

void __fastcall TLISXMLConv::LoadCDS(void)
{
	AnsiString sXML = "";
	std::auto_ptr<TMemoryStream> stream(new TMemoryStream);
	TClientDataSet *cds = NULL;
	for(int i = 0; i < ds_info->ChildNodes->Count; i++)
	{//������� �������� (�������, ������� �� ������� � �����) � ������ �� �����������
		cds = mTempDS[ds_info->ChildNodes->Nodes[i]->NodeName];
		cds->Active = false;
        cds->Filtered = false;
		cds->Filter = "";
		sXML = AnsiString(L"<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?> ");
		sXML += AnsiString(ds_info->ChildNodes->Nodes[i]->ChildNodes->FindNode(L"DATAPACKET")->XML);
		stream->Write(sXML.c_str(), sXML.Length());
		stream->Position = 0;
		cds->LoadFromStream(stream.get());
		stream->Clear();
		cds = NULL;
	}
}
//---------------------------------------------------------------------------

void __fastcall TLISXMLConv::AddDataSetToList(TDataSet* ds)
{
//��������� DS � ������
	if(ds == NULL)
		throw *(new Exception("AddDataSetToList(): �������� �������� 'DataSet'."));
	mDS[AnsiString(ds->Name)] = ds;
}
//---------------------------------------------------------------------------

void __fastcall TLISXMLConv::ClearDataSetList(void)
{
//������� ������ DS
	mDS.clear();
}
//---------------------------------------------------------------------------

void __fastcall TLISXMLConv::ConvertToDatabase(AnsiString FileName)
{
//������������ FXM � IB
//	if(mDS.size() == 0)
//		throw *(new Exception("ConvertToDatabase(): ����� DataSet ����."));
	XmlDoc fxmDoc;
	fxmDoc.docStyle = dsDaDt;
	XmlDocNode *fxmNode = NULL;
	fxmDoc.loadFrom(FileName.c_str());//��������� ���� FXM/T11
	int notice_count = 0;//������� NOTICE
	std::vector<std::pair<std::string, std::string> > element;
	AnsiString AttrName = "";
	AnsiString Dataset_name = "";
	AnsiString Field_name = "";
	AnsiString Field_type = "";
	AnsiString Default = "";
	bool add = false;
	LoadCDS();
	try
	{
		for(int i = 0; i < fxmDoc.rootNodes.size(); i++)
		{//������ �� ������� �����
			fxmNode = fxmDoc.rootNodes[i];
			element = fxmNode->getElements();
			if(AnsiString(fxmDoc.rootNodes[i]->getName().c_str()) == "HEAD")
			{//���� ���� 'HEAD'
				//������������� ������� ������ �������� �������� ����
				StartNet();
			}
			else if(AnsiString(fxmDoc.rootNodes[i]->getName().c_str()) == "NOTICE")
			{//���� ���� 'NOTICE'
				notice_count++;
				if(start)
				{
					for(int j = 0; j < fxm_to->ChildNodes->Count; j++)
					{//������ ������� ���� ��� ���� ����������, �������� � equi.txt
						AttrName = fxm_to->ChildNodes->Nodes[j]->NodeName;
						if(FindParametr(AttrName, element).empty())
							continue;
						if(!AnsiString(fxm_to->ChildNodes->Nodes[j]->Attributes[L"for_part"]).IsEmpty())
						{
							if(AnsiString(fxm_to->ChildNodes->Nodes[j]->Attributes[L"for_part"])
							   !=
							   AnsiString(fxmDoc.rootNodes[i]->getName().c_str()))
							{
								continue;
							}
						}
//						if((AnsiString(fxm_to->ChildNodes->Nodes[j]->Attributes[L"for_part"])
//						   !=
//						   AnsiString(fxmDoc.rootNodes[i]->getName().c_str()))
//						   &&
//						   !AnsiString(fxm_to->ChildNodes->Nodes[j]->Attributes[L"for_part"]).IsEmpty())
//							continue;
						Dataset_name = fxm_to->ChildNodes->Nodes[j]->Attributes[L"dataset_name"];
						Field_name = fxm_to->ChildNodes->Nodes[j]->Attributes[L"field_name"];
						Field_type = fxm_to->ChildNodes->Nodes[j]->Attributes[L"field_type"];
						Default = fxm_to->ChildNodes->Nodes[j]->Attributes[L"default"];
						add = fxm_to->ChildNodes->Nodes[j]->Attributes[L"add"];
						DocumentAddField(AttrName, Dataset_name,
										 Field_name, Field_type,
										 Default,
										 FindParametr(AttrName, element).c_str(),
										 notice_count,
										 add);
					}
					if(fxmDoc.rootNodes[i]->getSubNodes().size() > 0)
						ParsingSubNode(fxmDoc.rootNodes[i]->getSubNodes(), notice_count);
				}
			}
			else if(AnsiString(fxmDoc.rootNodes[i]->getName().c_str()) == "TAIL")
			{//���� ���� 'TAIL'
				//������� ����
				CreateNet();
				//���������� ������� ��������� t_num_notices
				//� ���� ������� - ���������� ��� � notice_count
				///
				//���� �������� �� ����� ��� �� ����� notice_count - ��������
				try
				{
					if(AnsiString(FindParametr(L"t_num_notices", element).c_str()).ToInt()!=notice_count)
						Abort();
				}
				catch(...)
				{
					throw *(new Exception("ConvertToDatabase(): ���� T11 ��������� ��� ����� �������� ������."));
				}
				//������� ������ ��������������� � ��
			}
		}
	}
	__finally
	{
		start = false;
	}
}
//---------------------------------------------------------------------------

void __fastcall TLISXMLConv::ParsingSubNode(std::vector<XmlDocNode*> vSub, int iteration)
{
//��������� ������ ��������� �����
	std::vector<std::pair<std::string, std::string> > element;
	AnsiString AttrName = "";
	AnsiString Dataset_name = "";
	AnsiString Field_name = "";
	AnsiString Field_type = "";
	AnsiString Default = "";
	bool add = false;
	XmlDocNode *subNode;
	std::vector<XmlDocNode*> vSubSub;
	for(int i = 0; i < vSub.size(); i++)
	{//������ ��� ���� ��������� �����
		subNode = vSub[i];
		element = subNode->getElements();
		for(int j = 0; j < fxm_to->ChildNodes->Count; j++)
		{//������ ��������� ���� ��� ���� ����������, �������� � equi.txt
			AttrName = fxm_to->ChildNodes->Nodes[j]->NodeName;
			if(FindParametr(AnsiString(AttrName), element).empty())
				continue;
			if((WideString(fxm_to->ChildNodes->Nodes[j]->Attributes[L"for_part"])
			   !=
			   WideString(subNode->getName().c_str()))
			   &&
			   !WideString(fxm_to->ChildNodes->Nodes[j]->Attributes[L"for_part"]).IsEmpty())
				continue;
			Dataset_name = fxm_to->ChildNodes->Nodes[j]->Attributes[L"dataset_name"];
			Field_name = fxm_to->ChildNodes->Nodes[j]->Attributes[L"field_name"];
			Field_type = fxm_to->ChildNodes->Nodes[j]->Attributes[L"field_type"];
			Default = fxm_to->ChildNodes->Nodes[j]->Attributes[L"default"];
			add = fxm_to->ChildNodes->Nodes[j]->Attributes[L"add"];
			DocumentAddField(AttrName, Dataset_name,
							 Field_name, Field_type,
							 Default,
							 FindParametr(AttrName, element).c_str(),
							 iteration,
							 add);
		}
		vSubSub = subNode->getSubNodes();
		if(vSubSub.size()>0)
			ParsingSubNode(vSubSub, iteration);//� ��� ����� ��������
	}
}
//---------------------------------------------------------------------------

void __fastcall TLISXMLConv::DocumentAddField(AnsiString AttrName,
											  AnsiString Dataset_name,
											  AnsiString Field_name,
											  AnsiString Field_type,
											  AnsiString Default,
											  AnsiString value,
											  int iteration,
											  bool add)
{
//��������� ��������� �������� �������
	if(value.IsEmpty())//���� ���� ��������
		return;
	TClientDataSet *cds = mTempDS[Dataset_name];//�������� �������
	Variant Val = 0;

	AnsiString str(value);
	if(value.IsEmpty())
		str = AnsiString(Default);
	//�������� ��� ������
	if(Field_type == "date1")
	{//���� ���� � ������������� '-'
		char oldDateSeparator = DateSeparator;
		AnsiString oldShortDateFormat = ShortDateFormat;
		try
		{
			DateSeparator = '-';
			ShortDateFormat = "yyyy-mm-dd";
			Val = StrToDate(str);
		}
		__finally
		{
			DateSeparator = oldDateSeparator;
			ShortDateFormat = oldShortDateFormat;
		}
	}
	else if(Field_type == "double1")
	{//���� ����� � ������ '.'
		char oldDecimalSeparator = DecimalSeparator;
		try
		{
			DecimalSeparator = '.';
			Val = double(StrToFloat(str));
		}
		__finally
		{
			DecimalSeparator = oldDecimalSeparator;
		}
	}
	else if(Field_type == "string")
	{//���� ������
		WideString wstr(value);
		if(value.IsEmpty())
			wstr = Default;
		Val = wstr;
	}
	else if(Field_type == "lon1")
	{//���� �������
		std::auto_ptr<TCoordinateConvertor> conv(new TCoordinateConvertor(tempOwner));
		conv->Direction = 'X';
		Val = double(conv->StrToCoord(str));
	}
	else if(Field_type == "lat1")
	{//���� ������
		std::auto_ptr<TCoordinateConvertor> conv(new TCoordinateConvertor(tempOwner));
		conv->Direction = 'Y';
		Val = double(conv->StrToCoord(str));
	}
	else
		return;
	try
	{
		DocumentCorrect(cds, iteration, add);
		cds->Edit();
		cds->FieldByName(Field_name)->Value = Val;
		cds->Post();
	}
	catch(...)
	{
		throw *(new Exception(AnsiString("DocumentAddField(): ���������� �������� ������ � ����� ������ "+cds->Name).c_str()));
	}
}
//---------------------------------------------------------------------------

void __fastcall TLISXMLConv::DocumentCorrect(TClientDataSet *cds, int iteration, bool add)
{
//������������ ������ ��������
//������ ������� �������, ����� �� ����� �������� � ��������� �� ���������
	if(add)
	{
		cds->Append();
		cds->Post();
	}
	else
	{
		for(int i = cds->RecordCount; i < iteration; i++)
		{//���� ������� ���� ��������� �������
			cds->Append();
			cds->Post();
		}
	}
	cds->Edit();
	cds->FieldByName("NUMBER")->AsInteger = iteration;
	cds->Post();
}
//---------------------------------------------------------------------------

void __fastcall TLISXMLConv::StartNet(void)
{
//�������� ������� �������� ����
	TClientDataSet *cds = NULL;
	for(std::map<AnsiString, TClientDataSet*>::iterator i = mTempDS.begin();
		i != mTempDS.end();
		i++)
	{//������� ��� ��������
		try
		{
			cds = i->second;
			for(cds->First(); !cds->Eof; cds->Next())
				cds->Delete();
			cds = NULL;
		}
		catch(Exception &e)
		{
			throw *(new Exception(AnsiString("StartNet(): ��� ������� �������� ������� 'DataSet' ��������� ������:\n"+e.Message).c_str()));
		}
	}
	start = true;
}

//---------------------------------------------------------------------------

std::string __fastcall TLISXMLConv::FindParametr(
		AnsiString nameParam,
		std::vector<std::pair<std::string, std::string> > elements)
{
//��������� ����� ���������
//� ���������� ��������
	std::pair<std::string, std::string> pr;
	try
	{
		for(int i = 0; i < elements.size(); i++)
		{
			pr = elements[i];
			if(AnsiString(pr.first.c_str())==nameParam)
			{
				return pr.second;
			}
		}
		return std::string("");
	}
	catch(...)
	{
		return std::string("");
	}
}
//---------------------------------------------------------------------------

TClientDataSet* __fastcall TLISXMLConv::GetDataSet(AnsiString name)
{
	return mTempDS[name];
}
//---------------------------------------------------------------------------

void __fastcall TLISXMLConv::CreateNet(void)
{
//��������� ������� �������� ��������
	//������� ������� ������ �������� �������� ����
	start = false;
}
//---------------------------------------------------------------------------

void __fastcall TLISXMLConv::CreateFXM(TIBDatabase* ibDB,
									   TIBDataSet* netDS,
									   AnsiString FileName)
{
//��������� �������������� IB->T11
	bool wndNet = false;
	for(int i = 0; i < Application->MainForm->MDIChildCount; i++)
	{//���������� ������� �������� ������������ �����
		if(Application->MainForm->MDIChildren[i]->Caption == "����")
			wndNet = true;
	}
	if(!wndNet)//���� ��� �������� �����
		throw *(new Exception("��� �������� ����. �������� ���������� ����� � �������� ���� ��� ��������������."));
	////////////////////////////////////////////////////////////////////////////
	//���� ��������� ���� - ���������� ��������
	/////
	//������� ������� ��������
	std::auto_ptr<XmlDoc> fxmDoc(new XmlDoc());
	fxmDoc->docStyle = dsDaDt;
	int net_id = netDS->FieldByName(L"ID")->AsInteger;
	if(net_id < 1)
		throw *(new Exception("���� �� ����������."));
	///////////////////////////////////////////////////////////
	//������� ������ ������
	std::auto_ptr<TIBDataSet> dsStation(new TIBDataSet(tempOwner));
	std::auto_ptr<TIBDataSet> dsFrqT(new TIBDataSet(tempOwner));
	std::auto_ptr<TIBDataSet> dsFrqR(new TIBDataSet(tempOwner));
	std::auto_ptr<TIBDataSet> dsCountry(new TIBDataSet(tempOwner));
	std::auto_ptr<TIBDataSet> dsAntenna(new TIBDataSet(tempOwner));
	std::auto_ptr<TIBDataSet> dsLink(new TIBDataSet(tempOwner));
	dsStation->Database      = ibDB;
	dsFrqT->Database         = ibDB;
	dsFrqR->Database         = ibDB;
	dsCountry->Database      = ibDB;
	dsAntenna->Database      = ibDB;
	dsLink->Database         = ibDB;
	dsStation->Transaction   = netDS->Transaction;
	dsFrqT->Transaction      = netDS->Transaction;
	dsFrqR->Transaction      = netDS->Transaction;
	dsCountry->Transaction   = netDS->Transaction;
	dsAntenna->Transaction   = netDS->Transaction;
	dsLink->Transaction      = netDS->Transaction;
	/////////////////STA
	AnsiString sqlSta = "select ID, NAME, TYPES, NET_ID, REGION_ID, \
COUNTRY_ID, EQUIPMENT_ID, POWER, ANTENNA_ID, ANT_HEIGHT, ABS_HEIGHT, \
POL_TRN, POL_REC, AZIMUTH_MIN, AZIMUTH_MAX, ELEVATION_MIN, ELEVATION_MAX, \
DATE_OFBRINGIHG_INTO_USE, NOTE, LONGITUDE, LATTITUDE, SCODE, CREATOR, \
CREAT_DATE, MODIFIER, MODIF_DATE, USER_INS, DATE_INS, USER_UPD, DATE_UPD \
from STATION";
	dsStation->SelectSQL->Add(sqlSta + " where NET_ID = '"+IntToStr(net_id)+"';");
	dsStation->Open();//������ ������� ������ ����
	dsStation->FetchAll();
	/////////////////
	AnsiString sql = "";
	for(dsStation->First(); !dsStation->Eof; dsStation->Next())
	{
		if(sql == "")
			sql+="(STATION_ID = '"+dsStation->FieldByName(L"ID")->AsString+"')";
		else
			sql+=" or (STATION_ID = '"+dsStation->FieldByName(L"ID")->AsString+"')";
	}
	dsFrqT->SelectSQL->Add("select STATION_ID, FREQ, TRN_REC from FREQ where (TRN_REC = 'T') and ("+sql+")");
//ShowMessage(sql);
//ShowMessage(dsFrqT->SelectSQL->Strings[0]);
	dsFrqT->Open();//������ ������� ������������ ���� ������� ������ ����
	dsFrqT->FetchAll();
//	ShowMessage(IntToStr(dsFrqT->RecordCount));
//	return;
	AnsiString sqlCou = "select ID, NAME, CODE from COUNTRY";
	AnsiString sqlAnt = "select ID, NAME, MIN_FREQ, MAX_FREQ, G_TR, G_REC, \
DIAMETR, MAIN_BEAM_WIDTH, USE_EXP_PATERN, NOTE from ANTENNA";
	AnsiString sqlLink = "select ID1, ID2 from LINK";
	///////////////////////////////////////////////////////////
	//!!!��������������� �������!!!
	/*---------------------------------------------------------------*/
	XmlDocNode *tmpNode = fxmDoc->addRootNode("HEAD");//������� ���� "HEAD"
/*---CONST---*/
	tmpNode->addElement("t_adm", "UKR");
	std::auto_ptr<TCoordinateConvertor> coordconv(new TCoordinateConvertor(tempOwner));
	frmProgress->btn->Visible = false;
	frmProgress->Caption = "������� ������ �11";
	frmProgress->lbl->Caption = "��������� ����� �11";
	frmProgress->pb->Max = 100;
	frmProgress->pb->Position = 0;
	frmProgress->Show();
	double step = 100.0/(double)dsFrqT->RecordCount;
	double pos = 0;
	Application->ProcessMessages();
	try
	{
		for(dsFrqT->First(); !dsFrqT->Eof; dsFrqT->Next())
		{
//ShowMessage(dsFrqT->SelectSQL->Strings[0]);
//ShowMessage(IntToStr(dsFrqT->RecNo)+"   "+IntToStr(dsFrqT->RecordCount));
			pos += step;
			frmProgress->pb->Position = (int)pos;
			dsStation->Active = false;
			dsStation->SelectSQL->Clear();
			dsStation->SelectSQL->Add(sqlSta + " where ID = '"+dsFrqT->FieldByName(L"STATION_ID")->AsString+"';");
			dsStation->Open();
			if(dsStation->RecordCount == 0)
			{//���� ������� � ���� ���� �� ����� ������� ���
				MessageBox(NULL,
						   AnsiString(
								"������� � ID="
								+dsFrqT->FieldByName(L"STATION_ID")->AsString
								+" �� ��������� � �������������� ����.").c_str(),
						   "LISFS",
						   MB_ICONWARNING);
				continue;
			}
			tmpNode = fxmDoc->addRootNode("NOTICE");//�������� 1 ����������
			/*---   � � � � � � � � �   � � � �   ---*/
			AnsiString temp = "";
			if(!dsStation->FieldByName(L"DATE_INS")->IsNull)
			{
				char oldDateSeparator = DateSeparator;
				AnsiString oldShortDateFormat = ShortDateFormat;
				try
				{
					DateSeparator = '-';
					ShortDateFormat = "yyyy-mm-dd";
					tmpNode->addElement("t_d_adm_ntc", DateToStr(dsStation->FieldByName(L"DATE_INS")->AsDateTime).c_str());
				}
				__finally
				{
					DateSeparator = oldDateSeparator;
					ShortDateFormat = oldShortDateFormat;
				}
			}
	/*---CONST---*/
			tmpNode->addElement("t_fragment", "NTFD_RR");
	/*---CONST---*/
			tmpNode->addElement("t_prov", "S11.2");
	/*---CONST---*/
			tmpNode->addElement("t_action", "ADD");
	/*---CONST---*/
			tmpNode->addElement("t_is_resub", "FALSE");
	/*---CONST---*/
			tmpNode->addElement("t_adm_ref_id", "02302 FX.2006");
			if(!dsFrqT->FieldByName(L"FREQ")->IsNull)
				tmpNode->addElement("t_freq_assgn", dsFrqT->FieldByName(L"FREQ")->AsFloat);
			if(!dsStation->FieldByName(L"DATE_OFBRINGIHG_INTO_USE")->IsNull)
			{
				char oldDateSeparator = DateSeparator;
				AnsiString oldShortDateFormat = ShortDateFormat;
				try
				{
					DateSeparator = '-';
					ShortDateFormat = "yyyy-mm-dd";
					tmpNode->addElement("t_adm_inuse", DateToStr(dsStation->FieldByName(L"DATE_OFBRINGIHG_INTO_USE")->AsDateTime).c_str());
				}
				__finally
				{
					DateSeparator = oldDateSeparator;
					ShortDateFormat = oldShortDateFormat;
				}
			}
			dsCountry->Active = false;
			dsCountry->SelectSQL->Clear();
			dsCountry->SelectSQL->Add(sqlCou+" where ID = '"+dsStation->FieldByName(L"COUNTRY_ID")->AsString+"'");
			dsCountry->Open();
			if(!dsCountry->FieldByName(L"CODE")->IsNull)
				tmpNode->addElement("t_ctry", dsCountry->FieldByName(L"CODE")->AsString.c_str());
			if(!dsStation->FieldByName(L"NAME")->IsNull)
				tmpNode->addElement("t_site_name", dsStation->FieldByName(L"NAME")->AsString.c_str());
			if(!dsStation->FieldByName(L"LONGITUDE")->IsNull)
				tmpNode->addElement("t_long", ConvertCordToNumber(coordconv->CoordToStr(dsStation->FieldByName(L"LONGITUDE")->AsFloat, 'X')).c_str());
			if(!dsStation->FieldByName(L"LATTITUDE")->IsNull)
				tmpNode->addElement("t_lat", ConvertCordToNumber(coordconv->CoordToStr(dsStation->FieldByName(L"LATTITUDE")->AsFloat, 'Y')).c_str());
	/*---CONST---*/
			tmpNode->addElement("t_stn_cls", "FX");
	/*---CONST---*/
			tmpNode->addElement("t_nat_srv", "CP");
	/*---CONST---*/
			tmpNode->addElement("t_bdwdth_cde", "24M6");
	/*---CONST---*/
			tmpNode->addElement("t_emi_cls", "G7D");
			if(!dsStation->FieldByName(L"ANT_HEIGHT")->IsNull)
			{
				temp = dsStation->FieldByName(L"ABS_HEIGHT")->AsString;
				if(temp.ToDouble() >= 0)
					temp = "+"+temp;
				tmpNode->addElement("t_site_alt", temp.c_str());
			}
	/*---CONST---*/
			tmpNode->addElement("t_op_hh_fr", "00:00");
	/*---CONST---*/
			tmpNode->addElement("t_op_hh_to", "23:59");
	/*---CONST---*/
			tmpNode->addElement("t_addr_code", "A");
	/*---CONST---*/
			tmpNode->addElement("t_op_agcy", "001");
			tmpNode = tmpNode->addNode("ANTENNA");//��������� ��������� �������
	/*---CONST---*/
			tmpNode->addElement("t_pwr_xyz", "Y");
			if(!dsStation->FieldByName(L"POWER")->IsNull)
				tmpNode->addElement("t_pwr_ant", dsStation->FieldByName(L"POWER")->AsString.c_str());
	/*---CONST---*/
			tmpNode->addElement("t_pwr_eiv", "I");
			dsAntenna->Active = false;
			dsAntenna->SelectSQL->Clear();
			dsAntenna->SelectSQL->Add(sqlAnt+" where ID='"+dsStation->FieldByName(L"ANTENNA_ID")->AsString+"'");
			dsAntenna->Open();
			if(dsAntenna->RecordCount > 0)
			{
				double pwr_out = dsStation->FieldByName(L"POWER")->AsFloat+dsAntenna->FieldByName(L"G_TR")->AsFloat;
				if(pwr_out >= 0)
					temp = "+"+FloatToStr(pwr_out);
				else
					temp = FloatToStr(pwr_out);
				tmpNode->addElement("t_pwr_dbw", temp.c_str());
			}
	/*---CONST---*/
			tmpNode->addElement("t_gain_type", "I");
	/*---CONST---*/
			tmpNode->addElement("t_ant_dir", "D");
			if(!dsStation->FieldByName(L"AZIMUTH_MIN")->IsNull)
				tmpNode->addElement("t_azm_max_e", dsStation->FieldByName(L"AZIMUTH_MIN")->AsString.c_str());
			if(!dsAntenna->FieldByName(L"MAIN_BEAM_WIDTH")->IsNull)
				tmpNode->addElement("t_bmwdth", dsAntenna->FieldByName(L"MAIN_BEAM_WIDTH")->AsString.c_str());
			if(!dsStation->FieldByName(L"ELEVATION_MIN")->IsNull)
				tmpNode->addElement("t_elev", dsStation->FieldByName(L"ELEVATION_MIN")->AsString.c_str());
			if(!dsStation->FieldByName(L"POL_TRN")->IsNull)
				tmpNode->addElement("t_polar", dsStation->FieldByName(L"POL_TRN")->AsString.c_str());
			if(!dsStation->FieldByName(L"ANT_HEIGHT")->IsNull)
				tmpNode->addElement("t_hgt_agl", dsStation->FieldByName(L"ANT_HEIGHT")->AsString.c_str());
			if(!dsAntenna->FieldByName(L"G_TR")->IsNull)
				tmpNode->addElement("t_gain_max", dsAntenna->FieldByName(L"G_TR")->AsString.c_str());
			dsLink->Active = false;
			dsLink->SelectSQL->Clear();
			dsLink->SelectSQL->Add(sqlLink + " where ID1 = '"+dsStation->FieldByName(L"ID")->AsString+"'");
//ShowMessage(dsLink->SelectSQL->Strings[0]);
			dsLink->Open();
			XmlDocNode *rxNode = NULL;
			for(dsLink->First(); !dsLink->Eof; dsLink->Next())
			{//��� ���� ������� ���������������
				dsFrqR->Active = false;
				dsFrqR->SelectSQL->Clear();
				dsFrqR->SelectSQL->Add("select STATION_ID, FREQ, TRN_REC from FREQ \
	where (TRN_REC = 'R') and (FREQ = '"+dsFrqT->FieldByName(L"FREQ")->AsString+"') and \
	(STATION_ID = '"+dsLink->FieldByName(L"ID2")->AsString+"')");
				dsFrqR->Open();
				for(dsFrqR->First(); !dsFrqR->Eof; dsFrqR->Next())
				{//��� ���� ��������� ������� ���������������
					dsStation->Active = false;
					dsStation->SelectSQL->Clear();
					dsStation->SelectSQL->Add(sqlSta+" where ID = '"+dsFrqR->FieldByName(L"STATION_ID")->AsString+"'");
//ShowMessage(dsStation->SelectSQL->Strings[0]);
					dsStation->Open();
					if(dsStation->RecordCount == 0)
						continue;
					rxNode = tmpNode->addNode("RX_STATION");//������� RX_STATION
					//...� ������ ���
					dsCountry->Active = false;
					dsCountry->SelectSQL->Clear();
					dsCountry->SelectSQL->Add(sqlCou+" where ID = '"+dsStation->FieldByName(L"COUNTRY_ID")->AsString+"'");
					dsCountry->Open();
					if(dsCountry->RecordCount > 0)
						rxNode->addElement("t_ctry", dsCountry->FieldByName(L"CODE")->AsString.c_str());
					if(!dsStation->FieldByName(L"NAME")->IsNull)
						rxNode->addElement("t_site_name", dsStation->FieldByName(L"NAME")->AsString.c_str());
					if(!dsStation->FieldByName(L"LONGITUDE")->IsNull)
						rxNode->addElement("t_long", ConvertCordToNumber(coordconv->CoordToStr(dsStation->FieldByName(L"LONGITUDE")->AsFloat, 'X')).c_str());
					if(!dsStation->FieldByName(L"LATTITUDE")->IsNull)
						rxNode->addElement("t_lat", ConvertCordToNumber(coordconv->CoordToStr(dsStation->FieldByName(L"LATTITUDE")->AsFloat, 'Y')).c_str());
	/*---CONST---*/
					rxNode->addElement("t_geo_type", "POINT");
				}
			}
			/*---   � � � � � � � � �   � � � �   ---*/
		}
		tmpNode = fxmDoc->addRootNode("TAIL");//������� ������ TAIL
		tmpNode->addElement("t_num_notices", dsFrqT->RecordCount);//���������� �� ������ ���������� NOTICE
	}
	__finally
	{
		frmProgress->Hide();
		frmProgress->btn->Visible = true;
	}
	fxmDoc->saveTo(FileName.c_str());
	/*---------------------------------------------------------------*/
	/////
	////////////////////////////////////////////////////////////////////////////
}
//---------------------------------------------------------------------------

AnsiString __fastcall TLISXMLConv::ConvertCordToNumber(AnsiString cord)
{
//������������� ���������� � �11
	AnsiString temp = GetOnlyDigit(cord);
	AnsiString result = "";
	if(cord.Pos("E") > 0)
	{
		result = "+";
		for(int i = 7; i > temp.Length(); i--)
			result += "0";
		result += temp;
	}
	else if(cord.Pos("W") > 0)
	{
		result = "-";
		for(int i = 7; i > temp.Length(); i--)
			result += "0";
		result += temp;
	}
	else if(cord.Pos("N") > 0)
	{
		result = "+";
		for(int i = 6; i > temp.Length(); i--)
			result += "0";
		result += temp;
	}
	else if(cord.Pos("S") > 0)
	{
		result = "-";
		for(int i = 6; i > temp.Length(); i--)
			result += "0";
		result += temp;
	}
	return result;
}
//---------------------------------------------------------------------------

AnsiString __fastcall TLISXMLConv::GetOnlyDigit(AnsiString str)
{
//���������� ������������������ ���� �� ������ (������� �������)
	AnsiString temp = str.Trim();
	AnsiString result = "";
	for(int i = 1; i <= temp.Length(); i++)
		if(temp[i] >= '0' && temp[i] <= '9')
			result += AnsiString(temp[i]);
	return result;
}
