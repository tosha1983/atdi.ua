//---------------------------------------------------------------------------

#ifndef uListSubareasH
#define uListSubareasH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include "uBaseListTree.h"
#include <ActnList.hpp>
#include <ComCtrls.hpp>
#include <DB.hpp>
#include <DBGrids.hpp>
#include <ExtCtrls.hpp>
#include <Grids.hpp>
#include <IBCustomDataSet.hpp>
#include <IBDatabase.hpp>
#include <IBQuery.hpp>
#include <IBSQL.hpp>
#include <ImgList.hpp>
#include <ToolWin.hpp>
#include <vector>
#include <map>
#include "BcDrawer.h"
//---------------------------------------------------------------------------
class TfrmListDigSubareas : public TfrmBaseListTree
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TIntegerField *dstListADM_ID;
    TIBStringField *dstListCTRY;
    TIntegerField *dstListNB_TEST_PTS;
    TIntegerField *dstListCONTOUR_ID;
    TToolButton *ToolButton1;
    TPanel *panDbSection;
    TLabel *lblDbSection;
    TComboBox *cbxDbSection;
    TToolButton *tbtMoveToSect;
    TAction *actMoveToSection;
    TAction *actCopyToSection;
    TToolButton *tbtCopyToSect;
    TSplitter *Splitter1;
    TPanel *panMap;
    TIBSQL *sqlContours;
    TIntegerField *dstListDB_SECTION_ID;
    TImageList *imageList;
    TToolBar *tbContours;
    TToolButton *tbtZoomIn;
    TToolButton *tbtZoomOut;
    TToolButton *ToolButton2;
    TToolButton *tbtLeft;
    TToolButton *tbtRight;
    TToolButton *tbtDown;
    TToolButton *tbtUp;
    TToolButton *ToolButton3;
    TToolButton *tbtFit;
    TToolButton *ToolButton4;
    TToolButton *tbtSelCountry;
    TToolButton *ToolButton5;
    TPanel *panChannel;
    TLabel *lblChannel;
    TComboBox *cbChannel;
    TIBStringField *dstListChannelList;
    TToolButton *ToolButton6;
    TToolButton *tbtExport;
    TAction *actExport;
    void __fastcall dstListCONTOUR_IDGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall cbxDbSectionChange(TObject *Sender);
    void __fastcall actMoveToSectionExecute(TObject *Sender);
    void __fastcall actCopyToSectionExecute(TObject *Sender);
    void __fastcall FormPaint(TObject *Sender);
    void __fastcall dstListAfterOpen(TDataSet *DataSet);
    void __fastcall dstListAfterDelete(TDataSet *DataSet);
    void __fastcall dstListAfterScroll(TDataSet *DataSet);
    void __fastcall panMapResize(TObject *Sender);
    void __fastcall actListInsertExecute(TObject *Sender);
    void __fastcall actListDeleteExecute(TObject *Sender);
    void __fastcall tbtZoomInClick(TObject *Sender);
    void __fastcall tbtZoomOutClick(TObject *Sender);
    void __fastcall tbtLeftClick(TObject *Sender);
    void __fastcall tbtRightClick(TObject *Sender);
    void __fastcall tbtDownClick(TObject *Sender);
    void __fastcall tbtUpClick(TObject *Sender);
    void __fastcall tbtFitClick(TObject *Sender);
    void __fastcall cbChannelChange(TObject *Sender);
    void __fastcall dstListCalcFields(TDataSet *DataSet);
    void __fastcall dstListChannelListGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall FormShow(TObject *Sender);
    void __fastcall actExportExecute(TObject *Sender);
private:	// User declarations
    std::vector<int> dbSectIds;
    __fastcall TfrmListDigSubareas(TComponent* Owner);
    ContourDrawer cd;
    std::vector<DrawContourData> contours;
    std::map<int, int> idx; // ID - index;
    int oldIndex;
    TForm *mapForm;
    void __fastcall ReloadContours();
    void __fastcall LoadChannels();

    struct ChannelInfo {
        int type; // 1 - channel, 2 - block
        int id;
        ChannelInfo() : type(0), id(0) {};
        ChannelInfo(int src_type, int src_id) : type(src_type), id(src_id) {};
        ChannelInfo(const ChannelInfo& src) : type(src.type), id(src.id) {};
        // operator = is not neccesary - fields are copied as dump by default
    };
    std::vector<ChannelInfo> channels;
    typedef std::map<int, AnsiString> ChannelMap;
    ChannelMap channelMap;
    AnsiString __fastcall GetChannelList(int id);

public:		// User declarations
    __fastcall TfrmListDigSubareas(TComponent* Owner, HWND callerWin, int elementId);
    virtual void __fastcall EditElement();
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListDigSubareas *frmListDigSubareas;
//---------------------------------------------------------------------------
#endif
