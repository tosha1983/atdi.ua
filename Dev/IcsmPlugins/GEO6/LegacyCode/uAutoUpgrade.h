//---------------------------------------------------------------------------

#ifndef uAutoUpgradeH
#define uAutoUpgradeH
//---------------------------------------------------------------------------

#include <IBDatabase.hpp>
#include <IBServices.hpp>
#include <IBSQL.hpp>
#include <vector>
#include <set>
#include "tempvalues.h"
using namespace std;
//---------------------------------------------------------------------------

enum UdbAction { uaCreate, uaUpdate, uaDelete, uaInsert };
enum UdbObject { uoDomain, uoTable, uoTrigger, uoStoredProc, uoFunction, uoDescription, uoTableField, uoProcParameter };

enum UpdateResult { urError = -1, urNewerDb = 0, urOk = 1 };

class TUpgradeProgress
{
protected:
    virtual AnsiString __fastcall GetLabelCaption() = 0;
    virtual void __fastcall SetLabelCaption(AnsiString s) = 0;
    virtual int __fastcall GetMax() = 0;
    virtual void __fastcall SetMax(int n) = 0;
public:
    virtual void __fastcall StepIt() = 0;
    __property AnsiString LabelCaption  = {read = GetLabelCaption, write = SetLabelCaption};
    __property int MaxPosition          = {read = GetMax, write = SetMax};
};
//---------------------------------------------------------------------------

class TDBAutoUpgrade
{
protected:
    TIBDatabase* m_db;
    TIBTransaction* m_tr;
    TIBSQL* m_sql;
    TUpgradeProgress* m_prgrs;
    void* m_param;
    double m_ver;
    void __fastcall SetVersionDb(double d_new_version);
    void __fastcall CreateDomain(AnsiString domainName, AnsiString domainType, AnsiString constr = "");
    void __fastcall CreateFunction(AnsiString functionName, AnsiString variablesTypes, AnsiString returnValue , AnsiString entryPoint, AnsiString modulName);
    void __fastcall CreateDescription(UdbObject obj,AnsiString name, AnsiString description, AnsiString ownerName = "");
public:
    TDBAutoUpgrade(TIBDatabase *db, double version, TUpgradeProgress* progress);
    ~TDBAutoUpgrade();
    int __fastcall CheckAndUpgrade();
    bool __fastcall RunQuery(AnsiString query);
    bool __fastcall RunQuery(AnsiString query, UdbAction act, UdbObject obj, AnsiString str);
    AnsiString __fastcall GetActionString (UdbAction act, UdbObject obj, AnsiString name);
    double __fastcall GetDbVersion();
    virtual void __fastcall UpgradeDb();
    TUpgradeProgress* GetProgress() { return m_prgrs; };
    bool CreateTrigger(AnsiString trName, AnsiString tblName, AnsiString trText);
    void __fastcall ShutDownDb();
    void __fastcall DisableTrigger(String trName);
    void __fastcall EnableTrigger(String trName);
    bool __fastcall TriggerExists(String trName);


    class Table
    {
    private:
        bool m_exists;
        TDBAutoUpgrade* m_upgrader;
        AnsiString m_tbl_name;
        std::vector<AnsiString> vTempSQL;
    public:
        Table(AnsiString tableName, TDBAutoUpgrade* _out); // объ€вить таблицу
        bool Exists(){return m_exists; }; // проверить, существует ли таблица в Ѕƒ
        void AddField(AnsiString fieldname, AnsiString domain, bool notNull = false, AnsiString check = ""); // добавить поле
        void AddPrimaryKey(AnsiString fieldList); // объ€вить первичный ключ
        void CreateTable(); // создать таблицу на сервере
        void AddIndex(AnsiString name, AnsiString field, AnsiString desc = ""); // добавить индекс
        void AddForeignKey(AnsiString field, AnsiString refTableField, bool cascadeUpd = true, bool cascadeDel = false);
        void GrantAll(String role = String()); // добавить внешний ключ
    protected:
        void RunOrCacheSQL(AnsiString sql);
    };

};

// helper macros
#define START_UPD_VER(curVersion, maxVersion) \
    if (curVersion < maxVersion) \
    {                            \
        double __maxVersion = maxVersion; \
        double& __curVersion = curVersion;

#define END_UPD_VER() \
        SetVersionDb(__curVersion = __maxVersion); \
    } else \
        if (m_prgrs && (m_prgrs->MaxPosition)) \
            m_prgrs->MaxPosition--;

//---------------------------------------------------------------------------
#endif
