//---------------------------------------------------------------------------

/*
    TDBAutoUpgrade - �������� ���������� ������ ���� ������
*/

#pragma hdrstop

#include "uAutoUpgrade.h"
#include <memory>
#include <Dialogs.hpp>

//---------------------------------------------------------------------------

#pragma package(smart_init)
//---------------------------------------------------------------------------

TDBAutoUpgrade::TDBAutoUpgrade(TIBDatabase *db, double version, TUpgradeProgress* progress)
{
    m_db  = db;
    m_ver = version;

    m_tr = new TIBTransaction(db->Owner);
    m_tr->Params->Clear();
    // snapshot transaction
    m_tr->Params->Add("concurrency");
    m_tr->Params->Add("nowait");
    m_tr->DefaultAction = TARollback;
    m_tr->AutoStopAction = saNone;
    m_db->AddTransaction(m_tr);
    m_tr->AddDatabase(m_db);
    m_tr->Active = true;

    m_sql = new TIBSQL(db->Owner);
    m_sql->Database = m_db;
    m_sql->Transaction = m_tr;

    m_prgrs = progress;
}
//---------------------------------------------------------------------------

TDBAutoUpgrade::~TDBAutoUpgrade()
{
    if (m_sql)
    {
        delete m_sql;
        m_sql = NULL;
    }
    if(m_tr != NULL)
    {
        if(m_tr->Active)
            m_tr->Rollback();
        delete m_tr;
        m_tr = NULL;
    }
}
//---------------------------------------------------------------------------

int __fastcall TDBAutoUpgrade::CheckAndUpgrade()
{
    /*
    ���� ����� ��������� �������� ������ ����������,
    ��� ������������� �������� �� �������� (���� ������������ �� ���������)
    � ����������:
        urOk        - �� � �������, ������ �� � �� ���������
        urError     - ��� ���������� ���������� ��������� ������������ ������
        urNewerDb   - �� ��������� � ����������
    */
    double tmp_version = GetDbVersion();

    if(tmp_version > m_ver)
    {//���� ������ �� ���� ������ ��
        Application->MessageBox("������ �� ������, ��� ������ ���������. �������� ��.",
                                Application->Title.c_str(), MB_ICONEXCLAMATION);
        m_db->Connected = false;
        return urNewerDb;
    }
    else if(tmp_version < m_ver)
    {//���� ������ �� ���� ������ ��
        TempVal<char> tempDecSep(DecimalSeparator, '.');
        if(Application->MessageBox(AnsiString("���� ������ (������ "+FloatToStr(tmp_version)+
                                    ") ������� ���������� ��� ������ � ������� ������� ���������� (������ "+
                                    FloatToStr(m_ver)+"). ��� ���������� ���������� �� ������ "
                                    "�������� ������� �������������� ���� ������.\n"
                                    "��������� ������������ ����� ���������.\n\n"
                                    "����� ����������� ������������� ������� ��������� ����� ��\n\n"
                                    "��������� ����������?").c_str(), Application->Title.c_str(),
                                    MB_ICONQUESTION | MB_YESNO) == IDYES)
        {
            try
            {
                UpgradeDb();
                MessageBox(NULL, "���������� �� ������� ���������", "Ok", MB_ICONINFORMATION);
                m_db->Close();
                m_db->Open();
            }
            catch (Exception& e)
            {
                e.Message = "������ ��� ���������� ����������:\n\n" + e.Message;
                if (e.Message.UpperCase().Pos("IS IN USE"))
                    e.Message += "\n\n���������, ��� �� ������ ����������� � ���� �� "
                                "(� �.�. ������� ����������� �� ���� �� ������),"
                                " � ������� ������, ���������� � ������������� ������ ��";
                Application->ShowException(&e);
                m_db->Connected = false;
                return urError;
            }
        }
        else
        {
            m_db->Connected = false;
            return urError;
        }
    }
    return urOk;
}
//---------------------------------------------------------------------------

bool __fastcall TDBAutoUpgrade::RunQuery(AnsiString query)
{
    if (m_sql->Open)
        m_sql->Close();
    m_sql->SQL->Text = query;
    if (!m_tr->Active)
        m_tr->StartTransaction();
    if (!m_tr->Active)
        throw *(new Exception("Failed to start Transaction"));
    m_sql->ExecQuery();
    return m_sql->Eof;
}
//---------------------------------------------------------------------------

bool __fastcall TDBAutoUpgrade::RunQuery(AnsiString query, UdbAction act, UdbObject obj, AnsiString str)
{
    m_prgrs->LabelCaption = GetActionString(act, obj, str);
    return RunQuery(query);
}
//---------------------------------------------------------------------------

AnsiString __fastcall TDBAutoUpgrade::GetActionString (UdbAction act, UdbObject obj, AnsiString name)
{
    return AnsiString (
        act == uaCreate ? "��������" :
        act == uaUpdate ? "����������" :
        act == uaDelete ? "��������" :
        act == uaInsert ? "������� ������ �" : "???") + " " +
        AnsiString (
        obj == uoDomain ? " �����a" :
        obj == uoTable ? (act == uaInsert ? " �������" : " �������") :
        obj == uoTrigger ? " ��������" :
        obj == uoDescription ? (act == uaInsert ? " ��������" : " ��������") :
        obj == uoStoredProc ? (act == uaInsert ? " ���������" : " ���������") : "???")
         + " " +
        name + "...";
}
//---------------------------------------------------------------------------

double __fastcall TDBAutoUpgrade::GetDbVersion()
{
    TempVal<char> tempDecSep(DecimalSeparator, '.');

    Table tbl_ss("SYSTEM_SCHEME", this);
    if (!tbl_ss.Exists())    //���� ��� ������� SYSTEM_SCHEME - ���������� -1
        return -1.0;

    if (!RunQuery("SELECT VERSION FROM SYSTEM_SCHEME WHERE NAME = 'SYSTEM';"))
        return m_sql->FieldByName("VERSION")->AsDouble;
    else
        return 0.0;
}
//---------------------------------------------------------------------------

void __fastcall TDBAutoUpgrade::UpgradeDb()
{
    ShutDownDb();

    Table tbl_ss("SYSTEM_SCHEME", this);
    if(!tbl_ss.Exists())//���� ��� ������� SYSTEM_SCHEME - ������� ������ ������ �� �, ������, ��� �������
    {
        tbl_ss.AddField("NAME", "VARCHAR(32)", true);
        tbl_ss.AddField("VERSION", "DOUBLE PRECISION", true);
        tbl_ss.AddPrimaryKey("NAME");
        tbl_ss.CreateTable();
        RunQuery("grant select on SYSTEM_SCHEME to public");
        m_tr->CommitRetaining();
        RunQuery("INSERT INTO SYSTEM_SCHEME (NAME, VERSION) VALUES ('SYSTEM', 0);");
        m_tr->CommitRetaining();
    }

    return;
}

void __fastcall TDBAutoUpgrade::CreateDomain(AnsiString domainName, AnsiString domainType, AnsiString constr)
{
    // ������� ����� � ��
    m_prgrs->LabelCaption = GetActionString(uaCreate, uoDomain, domainName.UpperCase());
    if(RunQuery("select rdb$field_name from RDB$FIELDS where rdb$field_name = '"+domainName+"';"))
    {
        m_sql->Close();
        RunQuery("CREATE DOMAIN "+domainName+" AS "+domainType+" "+constr);
    }
    m_sql->Close();
}

void __fastcall TDBAutoUpgrade::CreateFunction(AnsiString functionName, AnsiString variablesTypes, AnsiString returnValue, AnsiString entryPoint, AnsiString modulName)
{
    // ������� �������
    m_prgrs->LabelCaption = GetActionString(uaCreate, uoFunction, functionName.UpperCase());
    if(RunQuery("select rdb$function_name from RDB$FUNCTIONS where rdb$function_name = '"+functionName+"';"))
    {
        m_sql->Close();
        RunQuery("DECLARE EXTERNAL FUNCTION "+functionName+" "+variablesTypes+" RETURNS "+returnValue+" ENTRY_POINT '"+entryPoint+"' MODULE_NAME '"+modulName+"'");
    }
    m_sql->Close();

}

void __fastcall TDBAutoUpgrade::CreateDescription(UdbObject obj,AnsiString name, AnsiString description, AnsiString ownerName)
{
    // �������� ��������
    m_prgrs->LabelCaption = GetActionString(uaCreate, uoDescription, name.UpperCase());
    switch(obj)
    {
        case uoDomain : {
                            RunQuery("update RDB$FIELDS set RDB$DESCRIPTION = '"+description+"' where RDB$FIELD_NAME = '"+name+"'");
                        }; break;
        case uoTable :  {
                            RunQuery("update RDB$RELATIONS set RDB$DESCRIPTION = '"+description+"' where RDB$RELATION_NAME = '"+name+"'");
                        }; break;
        case uoFunction : {
                            RunQuery("update RDB$FUNCTIONS set RDB$DESCRIPTION = '"+description+"' where RDB$FUNCTION_NAME = '"+name+"'");
                        }; break;
        case uoStoredProc : {
                            RunQuery("update RDB$PROCEDURES set RDB$DESCRIPTION = '"+description+"' where RDB$PROCEDURE_NAME = '"+name+"'");
                        }; break;
        case uoTrigger : {
                            RunQuery("update RDB$TRIGGERS set RDB$DESCRIPTION = '"+description+"' where RDB$TRIGGER_NAME = '"+name+"'");
                        }; break;
        case uoTableField : {
                            RunQuery("update RDB$RELATION_FIELDS set RDB$DESCRIPTION = '"+description+"' where RDB$FIELD_NAME = '"+name+"' AND RDB$RELATION_NAME ='"+ownerName+"'");
                        }; break;
        case uoProcParameter : {
                            RunQuery("update RDB$PROCEDURE_PARAMETERS set RDB$DESCRIPTION = '"+description+"' where RDB$PARAMETER_NAME = '"+name+"' AND RDB$PROCEDURE_NAME ='"+ownerName+"'");
                        }; break;
    }
}
//---------------------------------------------------------------------------

void __fastcall TDBAutoUpgrade::SetVersionDb(double d_new_version)
{
    // ������������� ����� ������ ��
    m_prgrs->LabelCaption = "������ ������ ������ ("+FloatToStr(d_new_version)+")";
    TempVal<char> tempDecSep(DecimalSeparator, '.');
    m_sql->ParamCheck = false;
    RunQuery("UPDATE SYSTEM_SCHEME SET VERSION = "+FloatToStr(d_new_version)+" WHERE NAME = 'SYSTEM';");
    m_sql->Close();
    m_tr->CommitRetaining();
    m_prgrs->LabelCaption = "������ ������ "+FloatToStr(d_new_version)+" �������������";
    m_prgrs->StepIt();
}
//---------------------------------------------------------------------------

bool TDBAutoUpgrade::CreateTrigger(AnsiString trName, AnsiString tblName, AnsiString trText)
{
    if (!RunQuery("select RDB$FLAGS from RDB$TRIGGERS where RDB$TRIGGER_NAME = '"+trName+'\''))
        RunQuery("DROP TRIGGER "+trName);
    return RunQuery("CREATE TRIGGER "+trName+" FOR "+tblName+" "+trText);
}

//---------------------------------------------------------------------------
TDBAutoUpgrade::Table::Table(AnsiString tableName, TDBAutoUpgrade* _out)
{
    if(tableName.Trim().IsEmpty())
        throw *(new Exception("Table: ��� ������� �� ������."));
    m_upgrader = _out;
    vTempSQL.clear();
    m_tbl_name = tableName;
    m_exists = !m_upgrader->RunQuery(
        "SELECT RDB$RELATION_NAME FROM RDB$RELATIONS WHERE RDB$SYSTEM_FLAG=0 AND RDB$RELATION_NAME='"
            +m_tbl_name.UpperCase()
            +"';");
}
//---------------------------------------------------------------------------

void TDBAutoUpgrade::Table::AddField(AnsiString fieldname, AnsiString domain, bool notNull, AnsiString check)
{
    //��������� ���� � �������
    if (m_upgrader->GetProgress() != NULL)
        m_upgrader->GetProgress()->LabelCaption = m_upgrader->GetActionString(m_exists ? uaUpdate : uaCreate, uoTable, m_tbl_name.UpperCase());
    if (fieldname.Trim().IsEmpty())
        throw *(new Exception("Table AddField: ��� ���� �� ������."));
    if (domain.Trim().IsEmpty())
        throw *(new Exception(AnsiString("Table AddField: ��� ������ ���� "
            +fieldname.UpperCase()
            +" �� �����.").c_str()));
    AnsiString sql = "ALTER TABLE "+m_tbl_name.UpperCase()+" ADD "+fieldname.UpperCase()
        +" "+domain.UpperCase();
    if (notNull)
        sql = sql + " NOT NULL";
    if (!check.Trim().IsEmpty())
        sql = sql + " CONSTRAINT "+check.UpperCase();
    sql = sql + ";";
    RunOrCacheSQL(sql);
}
//---------------------------------------------------------------------------

void TDBAutoUpgrade::Table::AddPrimaryKey(AnsiString fieldList)
{
//��������� �������� ���� � �������
    //������ ����� ��������� - ���� �� � ������ ������� ��������� ����
    if(m_upgrader->GetProgress() != NULL)
        m_upgrader->GetProgress()->LabelCaption = m_upgrader->GetActionString(uaUpdate, uoTable, m_tbl_name.UpperCase());
    AnsiString sql = "SELECT * FROM RDB$RELATION_CONSTRAINTS WHERE RDB$CONSTRAINT_TYPE='PRIMARY KEY' AND RDB$RELATION_NAME='"
        +m_tbl_name
        +"';";
    std::auto_ptr<TIBSQL> ibSQL(new TIBSQL(Application));
    ibSQL->ParamCheck = false;
    if(!m_upgrader->RunQuery(sql))
        throw *(new Exception(AnsiString("Table AddPrimaryKey: ��������� ���� ��� ������� "
            +m_tbl_name.UpperCase()+" ��� ����������.").c_str()));
    if(fieldList.Trim().IsEmpty())
        throw *(new Exception(AnsiString("Table AddPrimaryKey: �������� ����� ��� ���������� ����� ������� "
            +m_tbl_name.UpperCase() +" �� �����.").c_str()));
    sql = "ALTER TABLE "+m_tbl_name.UpperCase()+" ADD CONSTRAINT PK_"+m_tbl_name.UpperCase()
        +" PRIMARY KEY ("+fieldList.UpperCase()+");";
    RunOrCacheSQL(sql);
}
//---------------------------------------------------------------------------

void TDBAutoUpgrade::Table::CreateTable()
{
//������� �������
    if(m_upgrader->GetProgress() != NULL)
        m_upgrader->GetProgress()->LabelCaption = m_upgrader->GetActionString(uaCreate, uoTable, m_tbl_name.UpperCase());
    if(m_exists)
        throw *(new Exception(AnsiString("Table CreateTable: ������� "
            +m_tbl_name.UpperCase()+" ��� ����������.").c_str()));
    AnsiString sql = "CREATE TABLE "+m_tbl_name.UpperCase()+"(TEMP_FIELD_001 INTEGER);";
    m_upgrader->RunQuery(sql);
    for(unsigned int i = 0; i < vTempSQL.size(); i++)
    {
        sql = vTempSQL[i];
        m_upgrader->RunQuery(sql);
    }
    sql = "ALTER TABLE " + m_tbl_name.UpperCase() + " DROP TEMP_FIELD_001;";
    m_upgrader->RunQuery(sql);
    m_exists = true;
}
//---------------------------------------------------------------------------

void TDBAutoUpgrade::Table::AddIndex(AnsiString name, AnsiString field, AnsiString desc)
{
//������� ������
    if(m_upgrader->GetProgress() != NULL)
        m_upgrader->GetProgress()->LabelCaption = m_upgrader->GetActionString(uaUpdate, uoTable, m_tbl_name.UpperCase());
    if(name.Trim().IsEmpty())
        throw *(new Exception(AnsiString("Table AddIndex: ��� ������� ��� ������� "
            +m_tbl_name.UpperCase()+" �� ������.").c_str()));
    if(field.Trim().IsEmpty())
        throw *(new Exception(AnsiString("Table AddIndex: ��� ���� ��� ������� ������� "
            +m_tbl_name.UpperCase()+" �� ������.").c_str()));
    AnsiString sql = "CREATE "+desc.UpperCase()+" INDEX "+name.UpperCase()
        +" ON "+m_tbl_name.UpperCase()+" ("+field.UpperCase()+");";
    RunOrCacheSQL(sql);
}
//---------------------------------------------------------------------------

void TDBAutoUpgrade::Table::AddForeignKey(AnsiString field, AnsiString refTableField, bool cascadeUpd, bool cascadeDel)
{
//������� ������� ����
    if(m_upgrader->GetProgress() != NULL)
        m_upgrader->GetProgress()->LabelCaption = m_upgrader->GetActionString(uaUpdate, uoTable, m_tbl_name.UpperCase());
    if(field.Trim().IsEmpty())
        throw *(new Exception(AnsiString("Table AddForeignKey: ��� ���� ������� "
            +m_tbl_name.UpperCase()+", ����������� ������� ����, �� ������.").c_str()));
    if(refTableField.Trim().IsEmpty())
        throw *(new Exception(AnsiString("Table AddForeignKey: ������� ���� ������� "
            +m_tbl_name.UpperCase()+" �� �����.").c_str()));
    static int n_fk_key = 1;
    if(m_exists)
    {
        while(!m_upgrader->RunQuery("select RDB$CONSTRAINT_NAME from RDB$RELATION_CONSTRAINTS where RDB$CONSTRAINT_NAME = 'FK_"+m_tbl_name.UpperCase()+"_00"+IntToStr(n_fk_key)+"';"))
            n_fk_key++;
    }

    AnsiString sql = "ALTER TABLE "+m_tbl_name.UpperCase()+" ADD CONSTRAINT FK_"
        +m_tbl_name.UpperCase()+"_00"+IntToStr(n_fk_key++)+" FOREIGN KEY ("+field.UpperCase()
        +") REFERENCES "+refTableField.UpperCase();
    if(cascadeUpd)
    {
        sql = sql + " ON UPDATE CASCADE";
    }
    if(cascadeDel)
    {
        sql = sql + " ON DELETE CASCADE";
    }
    sql = sql + ";";
    RunOrCacheSQL(sql);
}
//---------------------------------------------------------------------------

void TDBAutoUpgrade::Table::RunOrCacheSQL(AnsiString sql)
{
//��������� ��� �������� ������
    if(m_exists)
        m_upgrader->RunQuery(sql);//��������� ���������������
    else
        vTempSQL.push_back(sql);//��������
}
//---------------------------------------------------------------------------

void TDBAutoUpgrade::Table::GrantAll(String role)
{
    if (role.Length() == 0)
        role = "public";
    RunOrCacheSQL("grant all on "+m_tbl_name+" to "+role);
}

void __fastcall TDBAutoUpgrade::ShutDownDb()
{
    // shutdown db
    std::auto_ptr<TIBConfigService> confSrv (new TIBConfigService(m_db->Owner));
    String dbName = m_db->DatabaseName;

    int firstColon = dbName.Pos(":");
    if (firstColon == 0 || (firstColon == 2 && dbName.SubString(3, dbName.Length() - 2).Pos(":") == 0))
    {
        confSrv->ServerName = "localhost";
        confSrv->DatabaseName = dbName;
    } else {
        confSrv->ServerName = dbName.SubString(1, firstColon - 1);
        confSrv->DatabaseName = dbName.SubString(firstColon + 1, dbName.Length() - firstColon);
    }
    confSrv->LoginPrompt = false;
    confSrv->Params->Clear();
    confSrv->Params->Values["user_name"] = m_db->Params->Values["user_name"];
    confSrv->Params->Values["password"] = m_db->Params->Values["password"];
    m_db->Connected = false;
    try {
        confSrv->Active = true;
        confSrv->ShutdownDatabase(Forced, 0);
        confSrv->BringDatabaseOnline();
    } catch (Exception &e) {
        MessageBox(NULL, ("�� ������ �������� ������������ ������ � ��:\n" + e.Message +
                    "\n\n�� �� �� �� ���������...").c_str(), "���������� ��", MB_ICONEXCLAMATION);
        Sleep(1000);
    }
    if (confSrv->Active)
        try { confSrv->BringDatabaseOnline(); } catch (Exception &e) {
        MessageBox(NULL, ("Error trying to put DB online: :\n" + e.Message).c_str(), "���������� ��", MB_ICONEXCLAMATION); }
    confSrv->Active = false;
    m_db->Connected = true;
}
//---------------------------------------------------------------------------

void __fastcall TDBAutoUpgrade::DisableTrigger(String trName)
{
    RunQuery("ALTER TRIGGER "+trName+" INACTIVE");
}

void __fastcall TDBAutoUpgrade::EnableTrigger(String trName)
{
    RunQuery("ALTER TRIGGER "+trName+" ACTIVE");
}

bool __fastcall TDBAutoUpgrade::TriggerExists(String trName)
{
    return !RunQuery("select RDB$TRIGGER_NAME from RDB$TRIGGERS where RDB$TRIGGER_NAME = '"+trName+"'");
}

