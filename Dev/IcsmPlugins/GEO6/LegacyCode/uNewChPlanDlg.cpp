//---------------------------------------------------------------------
#include <vcl.h>
#pragma hdrstop

#include "uNewChPlanDlg.h"
#include "tempvalues.h"
#include "uMainDm.h"
//---------------------------------------------------------------------
#pragma resource "*.dfm"
#undef StrToInt
TdgCreateChannelPlan *dgCreateChannelPlan;
//---------------------------------------------------------------------
__fastcall TdgCreateChannelPlan::TdgCreateChannelPlan(TComponent* AOwner)
	: TForm(AOwner)
{
}
//---------------------------------------------------------------------

bool __fastcall TdgCreateChannelPlan::CreatePlan()
{
    std::auto_ptr<TIBTransaction> trChannelList(new TIBTransaction(this));
    TempCursor tc(crHourGlass);

    try
    {
        trChannelList->DefaultDatabase = dmMain->dbMain;

        std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
        sql->Database = dmMain->dbMain;
        sql->Transaction = trChannelList.get();

        dmMain->dbMain->Open();

        if(FreqGridName->Text == "" || FirstFreq->Text== "" || LastFreq->Text== "" || Spacing->Text == "" || ChanWidth->Text == "")
            throw *(new Exception("Введена неполная информация!"));

        if(FreqGridName->Text.Length()>4)
            throw *(new Exception("Слишком длинное имя сетки частот!"));

        if(StrToFloat(Spacing->Text) < StrToFloat(ChanWidth->Text))
            throw *(new Exception("Ширина канала превышает шаг между каналами!"));

        float lastFreq;

        if(LastOrNumb->ItemIndex == 0)
        {
            if(StrToFloat(FirstFreq->Text) >= StrToFloat(LastFreq->Text))
                throw *(new Exception("Начальная частота превышает конечную!"));
            if(StrToFloat(Spacing->Text) < 0 || StrToFloat(Spacing->Text)/2. > (StrToFloat(LastFreq->Text) - StrToFloat(FirstFreq->Text)))
                throw *(new Exception("Промежуток между каналами превышает размер сетки частот!"));
            lastFreq=StrToFloat(LastFreq->Text);
        }
        else
        {
            if(StrToInt(LastFreq->Text) <= 0)
                throw *(new Exception("Введено неверное количество каналов!"));
            if(StrToFloat(Spacing->Text) < 0)
                throw *(new Exception("Не верно введен промежуток между каналами!"));
            lastFreq=StrToFloat(FirstFreq->Text)+(StrToInt(LastFreq->Text)-1)*StrToFloat(Spacing->Text);
        }

        

        float vidRem=0;
        float soundRem=0;

        if(VideoRem->Text!="")
            vidRem=StrToFloat(VideoRem->Text);

        if(SoundRem->Text!="")
            soundRem=StrToFloat(SoundRem->Text);

        if(StrToFloat(ChanWidth->Text)/2. < std::abs(vidRem) || StrToFloat(ChanWidth->Text)/2. < std::abs(soundRem))
            throw *(new Exception("Смещение превышает ширину канала!"));

        dmMain->sqlGetNewId->Transaction=trChannelList.get();
        trChannelList->StartTransaction();

        int gridId=dmMain->getNewId();
        sql->SQL->Text="insert into FREQUENCYGRID (ID,NAME) values ("+IntToStr(gridId)+",'"+FreqGridName->Text+"');";
        sql->ExecQuery();

        float spacing=StrToFloat(Spacing->Text);
        float chanWidth=StrToFloat(ChanWidth->Text);
        float curFreq=StrToFloat(FirstFreq->Text);

        sql->SQL->Clear();
        sql->SQL->Text = "insert into CHANNELS (ID,FREQUENCYGRID_ID,NAMECHANNEL,FREQFROM,FREQTO,FREQCARRIERVISION,FREQCARRIERSOUND) values (:ID,:FR_ID,:NAME,:FRFROM,:FRTO,:FRCARVIS,:FRCARSOUND);";

        int chanNum;
        if(FirstChanNum->Text == "")
            chanNum = 0;
        else
            chanNum = StrToInt(FirstChanNum->Text);

        int numbQuant;
        if(NumQuant->Text == "")
            numbQuant = 2;
        else
            numbQuant = StrToInt(NumQuant->Text);

        AnsiString s,s1,chanName;
        sql->ParamByName("FR_ID")->AsInteger=gridId;

        while(curFreq<=lastFreq)
        {
            sql->ParamByName("ID")->AsInteger = dmMain->getNewId();
            s1 = IntToStr(numbQuant);
            s1 = "%s%0"+s1+"d%s";
            chanName = s.sprintf(s1.c_str(),NamePref->Text.c_str(),chanNum,NameSuf->Text.c_str());

            if(chanName.Length()>4)
                throw Exception("Длина имени канала не должна превышать четырех символов!!");

            sql->ParamByName("NAME")->AsString=chanName;
            sql->ParamByName("FRFROM")->AsDouble=curFreq-chanWidth/2.;
            sql->ParamByName("FRTO")->AsDouble=curFreq+chanWidth/2.;
            sql->ParamByName("FRCARVIS")->AsDouble=curFreq+vidRem;
            sql->ParamByName("FRCARSOUND")->AsDouble=curFreq+soundRem;
            sql->ExecQuery();

            curFreq += spacing;
            ++chanNum;
        }

        trChannelList->CommitRetaining();
        tc.Reset();
        return true;
    }

    catch (Exception &e)
    {

        if(trChannelList->InTransaction==true)
            trChannelList->RollbackRetaining();

        Application->ShowException(&e);
        tc.Reset();
        return false;
    }
    
}
//---------------------------------------------------------------------------

void __fastcall TdgCreateChannelPlan::btOkClick(TObject *Sender)
{
    if (CreatePlan())
        ModalResult = mrOk;
}
//---------------------------------------------------------------------------

void __fastcall TdgCreateChannelPlan::CheckFreqOnExit(TObject *Sender)
{
    if (dynamic_cast<TEdit*>(Sender))
        ((TEdit*)Sender)->Text = FormatFloat("#.###", ((TEdit*)Sender)->Text.ToDouble());
}
//---------------------------------------------------------------------------

