//$$---- Form HDR ----
//---------------------------------------------------------------------------

#ifndef uItuImportH
#define uItuImportH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <DB.hpp>
#include <DBGrids.hpp>
//#include "CoordConv.hpp"
#include <Buttons.hpp>
#include <DBCtrls.hpp>
#include <DBClient.hpp>
#include <math.h>
#include <Grids.hpp>
#include "uXmlDoc.h"
#include <ComCtrls.hpp>
#include <ExtCtrls.hpp>
#include <IBCustomDataSet.hpp>
#include <map>
#include "BcDrawer.h"

using namespace std;

typedef struct {char* elName; int width; char* label; bool show; char* fldName; char fldType; } NoticeElements;

//---------------------------------------------------------------------------
class TfrmRrc06Import : public TForm
{
__published:
	TEdit *edtFileName;
	TLabel *Label1;
	TButton *btnFile;
	TDataSource *dsSta;
	TGroupBox *GroupBox1;
    TStringGrid *grdData;
	TDataSource *dsCorr;
	TButton *btnCancel;
	TButton *btnImport;
	TDataSource *dsTempNet;
	TClientDataSet *cdsTempNet;
    TMemo *lblError;
    TProgressBar *pb;
    TLabel *lblPb;
    TLabel *lblDbSection;
    TComboBox *cbxDbSection;
    TSplitter *Splitter1;
    TIBDataSet *sqlAssgn;
    TLabel *lblConstr;
    TLabel *lblConstrContent;
    TButton *btnConstrSet;
    TButton *btnConstrClear;
	void __fastcall btnFileClick(TObject *Sender);
	void __fastcall btnFitAllColumnsClick(TObject *Sender);
	void __fastcall grdDataKeyDown(TObject *Sender, WORD &Key, TShiftState Shift);
	void __fastcall grdDataMouseUp(TObject *Sender, TMouseButton Button,
		  TShiftState Shift, int X, int Y);
	void __fastcall btnCancelClick(TObject *Sender);
	void __fastcall btnImportClick(TObject *Sender);
    void __fastcall FormResize(TObject *Sender);
    void __fastcall FormPaint(TObject *Sender);
    void __fastcall btnConstrSetClick(TObject *Sender);
    void __fastcall btnConstrClearClick(TObject *Sender);
    void __fastcall grdDataDrawCell(TObject *Sender, int ACol, int ARow,
          TRect &Rect, TGridDrawState State);
    void __fastcall cbxDbSectionChange(TObject *Sender);

public:

    typedef enum { imNone = -1, imGa1, imGs1Gt1, imGs2Gt2, imLast } ImportMode;
    void __fastcall SetImportMode(ImportMode value);
    ImportMode __fastcall GetImportMode();

private:

	void __fastcall LoadFile(AnsiString fName);
	void __fastcall ShowCoordinate(const TRect &Rect, TField *Field, TDBGrid *grd);
    XmlDoc inputFile;
    std::map<std::string, int> elIndices;
    std::map<std::string, AnsiString> elFldNames;
    std::string t_adm;
    int adminId;
    ImportMode m_importMode;

    int errorCount;

    NoticeElements *noticeElements;
    int noticeCount;
    bool sfnPresent;
    std::map<std::string, int> sfnMap;
    std::map<double, int> blkMap;
    std::map<double, int> chnMap;
    std::map<std::string, int> sysTypeMap;

    TForm * mapForm;
    ContourDrawer cd;
    std::vector<DrawContourData> contoursData;
    int oldIndex;

    double __fastcall atolat(const char*);
    double __fastcall atolon(const char*);

    bool chckMinLat;
    bool chckMinLon;
    bool chckMaxLat;
    bool chckMaxLon;

    double minLat;
    double minLon;
    double maxLat;
    double maxLon;

    bool chckIfContourExists;

    std::vector<bool> constrained;
    AnsiString __fastcall GetConstrText();
    bool __fastcall CheckPoint(double lon, double lat);

public:

	__fastcall TfrmRrc06Import(TComponent* Owner);
    void __fastcall Clear();
    void __fastcall LogError(AnsiString msg, bool isError = true);
    __property ImportMode importMode  = { read=GetImportMode, write=SetImportMode };
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmRrc06Import *frmRrc06Import;
//---------------------------------------------------------------------------
#endif
