//---------------------------------------------------------------------------

#ifndef uProgressH
#define uProgressH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ComCtrls.hpp>
#include <memory>
using namespace std;
//---------------------------------------------------------------------------
class T_frmProgress : public TForm
{
__published:	// IDE-managed Components
    TProgressBar *_pb;
    TButton *_btn;
    TLabel *_lbl;
    void __fastcall _btnClick(TObject *Sender);
private:	// User declarations
public:		// User declarations
    __fastcall T_frmProgress(TComponent* Owner);
};
//---------------------------------------------------------------------------
extern PACKAGE T_frmProgress *_frmProgress;
//---------------------------------------------------------------------------

typedef void __fastcall (__closure* TProgressStop)(TObject *Sender, TCloseAction &Action);

class TProgress
{
private:
    AnsiString m_wndCaption;
    AnsiString m_lblCaption;
    int m_maxPosition;
    TProgressStop m_stop;
    bool m_isActive;
    std::auto_ptr<T_frmProgress> m_frmProgress;
    AnsiString __fastcall getWindowCaption(){return m_wndCaption; }
    AnsiString __fastcall getLabelCaption(){return m_lblCaption; }
    int __fastcall getMax(){return m_maxPosition; }
    bool __fastcall getIsActive(){return m_isActive; }
    int __fastcall getCurrentPos();
    void __fastcall setCurrentPos(int n);
    void __fastcall setLabelCaption(AnsiString s);
    void __fastcall setMax(int n);
    void __fastcall controlPosition();
    void __fastcall frmClose(TObject *Sender, TCloseAction &Action);
public:
    TProgress();
    ~TProgress();
//////////////////////////////////////////////////////////////////////////////
/****************************************************************************/
/*   Всё что нужно для работы с класcом:                                     */
    void __fastcall StartProgress(AnsiString wndCaption, AnsiString lblCaption, int max, TProgressStop fncStop = NULL);
    void __fastcall StopProgress();
    void __fastcall IncrementPosition();
/****************************************************************************/
//////////////////////////////////////////////////////////////////////////////

//А это - дополнительно
    void __fastcall ResetPosition();

    __property int  Position = {read = getCurrentPos, write = setCurrentPos};

    __property AnsiString WindowCaption = {read = getWindowCaption};
    __property AnsiString LabelCaption  = {read = getLabelCaption, write = setLabelCaption};
    __property int MaxPosition          = {read = getMax, write = setMax};
    __property bool IsActive            = {read = getIsActive};
};
//---------------------------------------------------------------------------

extern TProgress progress;
//---------------------------------------------------------------------------
#endif
