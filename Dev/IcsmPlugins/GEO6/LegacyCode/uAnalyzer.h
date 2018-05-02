//---------------------------------------------------------------------------

#ifndef uAnalyzerH
#define uAnalyzerH

#include <vcl.h>
#include <ComCtrls.hpp>
#include <string>
#include <vector>
#include "LISBC_TLB.h"
#include <LISBCCalc_TLB.h>
#include "RSAGeography_TLB.h"
#include <values.h>

#include <set>

//---------------------------------------------------------------------------

enum NewSelectionType {nsExpertise, nsPlanning};

struct PointDuelResult {
    double radius;              //  радиус
    double azimuth;             //  азимут
    TRSAGeoPoint geoPoint;      //  координаты точки
    int azimuthIdx;             //  индекс массива
    double eMin;                //  Е мин
    double eInt;                //  Е помехи
    double aOffset;             //  защитное отношение (СНЧ?)
    double aDiscr;              //  ослабление антенны
    double eUsable;             //  Е исп
    unsigned char intType;      //  Тип помехи
};

struct  TControlPointResult {
    double e_int; // напряженность поля от меш. перед. (без ЗО и дискриминации ант.)
    TBCSInterferenceType int_type;
    double a_pr;
    double a_discr;
    double a_polar;
};

class TxAnalyzer {
private:
    static TCOMILISProgress FProgress;
    static TCOMIRSASpherics FSpherics;
    TProgressBar *progressBar;
    TForm *frmProgress;
    TForm * __fastcall CreateProgressForm(AnsiString FormCaption);
    void __fastcall OnProgressNotify(TObject *Sender);
    void __fastcall OnCancel(TObject *Sender);
    TDateTime startedAt;

public:
    bool wasChanges;
    bool isNewPlan;

    struct PlanVectorElement {
        long id;
        int channelId;
        double frequency;
        std::string name;
        ILISBCTxList *txList;
        int maxWantIdx;
        int maxUnwantIdx;

        PlanVectorElement(): id(0), txList(NULL) {};
        //PlanVectorElement(long src_id, ILISBCTxList *src_lst): id(src_id), txList(src_lst) { txList->AddRef(); };
        //~PlanVectorElement() { try { txList->Release(); } catch(...) {} };
      protected:
        //PlanVectorElement(PlanVectorElement&);
        //operator=(const PlanVectorElement&);
    };

    typedef std::vector<PlanVectorElement> PlanVector;
    PlanVector planVector;

    TCOMILISBCTx planningTx;
    double time0;
    bool cancelled;

    __fastcall TxAnalyzer();
    __fastcall ~TxAnalyzer() {}

    void __fastcall PerformPlanning(int txId);
    void __fastcall DoAnalysis();
    void __fastcall SaveToDb();
    void __fastcall LoadFromDb();

    void __fastcall GetCoordinationZoneFromBase(ILISBCTx* ptrTx, double* zone);
    void __fastcall GetCoordinationZone(ILISBCTx* ptrTx, double* zone, double e_trig = -MAXDOUBLE);

    void __fastcall ShowDuelResult(ILISBCTx *pTxA, ILISBCTx *pTxB, int idxA, int idxB,
                                            const TDuelResult2&, const TPointDuelResult*);
    static AnsiString __fastcall GetDuelObjString(ILISBCTx* iTx, double azimuth);
    void __fastcall CalcDuelPlan(int veId, int duelId);
    void __fastcall CalcDuel3(ILISBCTx *tx0, ILISBCTx *tx1, TDuelResult2 duel_result, TPointDuelResult *pointDuelResult);
    bool __fastcall DoProgress(long perc);
    void __fastcall CalcDuelInterfere(TCOMILISBCTxList & txList, AnsiString ProgressFormCaption);
    void __fastcall GetTxZone(TCOMILISBCTxList &TxList, LPSAFEARRAY* zone, AnsiString ProgressFormCaption);
    void __fastcall GetSfnZone(TCOMILISBCTxList &sfn, double* eMin, LPSAFEARRAY* zone);
    void __fastcall Shutdown();

    void __fastcall ShowProgress(AnsiString ProgressFormCaption, unsigned max = 100);
    void __fastcall HideProgress();
    void __fastcall GetCpE(ILISBCTxList*, double lon, double lat, LPSAFEARRAY res);
    void __fastcall GetInterfZones(ILISBCTxList *list, int uw1pos, int uw2pos,
                                ILISBCTxList *isfn1, ILISBCTxList *isfn2,
                                LPSAFEARRAY* zone1, LPSAFEARRAY* zone2);
    void __fastcall SortTxList(ILISBCTxList* list, String attrib, String dir);
    long __fastcall CopyTx(ILISBCTxList *to, ILISBCTxList *from, int idx);
    void __fastcall MapToolUsed(TObject *Sender,
          short ToolNum, double X1, double Y1, double X2, double Y2,
          double Distance, VARIANT_BOOL Shift, VARIANT_BOOL Ctrl,
          VARIANT_BOOL *EnableDefault);
    double __fastcall GetSumE(ILISBCTxList* ilist, double x, double y);
    TForm * __fastcall MakeNewSelection(int txId, NewSelectionType);
    String __fastcall GetTxNominalString(ILISBCTx *);
    void __fastcall Clear();
    void __fastcall ClearPlan();
    PlanVector::reference __fastcall AddPlanEntry(long id, std::string name,
                                                int chId, double freq, int maxWant, int maxUnwant);
    double __fastcall GetAllotEmin(ILISBCTx* tx);
    void __fastcall SwitchSfn(ILISBCTxList* list, ILISBCTxList* sfn, bool on);
    float __fastcall GetGndCond(float x, float y);
    long __fastcall GetNoiseZone(float x, float y);
    void __fastcall CheckIdwm();
    TBCTxType __fastcall GetDiapason(ILISBCTxList*);
    void __fastcall CheckLfMfCalc();
    void __fastcall CalcDuel(ILISBCTx* tx1, ILISBCTx* tx2, TDuelResult2* dr2, TPointDuelResult* pda);
    double __fastcall GetEmin(ILISBCTx*);
    double __fastcall GetE(ILISBCTx* tx, double lon, double lat, char* type);
    double __fastcall GetE50(ILISBCTx* tx, double lon, double lat, char* type);
    double __fastcall GetPr(ILISBCTx* tx1, ILISBCTx* tx2);
    double __fastcall GetVal(LPSAFEARRAY zone, double az);
    double __fastcall GetZoneByDir(ILISBCTx*, ILISBCTxList*, double az);
    void __fastcall GetTxZone(ILISBCTx* ptx, LPSAFEARRAY* zone);
    double __fastcall GetGndCond(double x, double y);
    double __fastcall GetUsableE(ILISBCTxList*, double lon, double lat);
    double __fastcall GetZoneByAzm(ILISBCTxList*, double azm);
    double __fastcall GetAzimuth(double lon1, double lat1, double lon2, double lat2);
    void __fastcall GetPoint(double lon1, double lat1, double az, double dist, double* lon2, double* lat2);
    double __fastcall GetDistance(double lon1, double lat1, double lon2, double lat2);
    LPSAFEARRAY __fastcall GetEtalonZone(ILISBCTx* ptx, long mode);
    void __fastcall DropEtalonZone(ILISBCTx* ptx, long mode);
    LPSAFEARRAY __fastcall GetCoverage(ILISBCTx* tx, double azBeg, double azEnd, double azStep, double radBeg, double radEnd, double radStep);
};

extern TxAnalyzer txAnalyzer;

#endif
