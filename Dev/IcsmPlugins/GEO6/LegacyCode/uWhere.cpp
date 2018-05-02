//---------------------------------------------------------------------------
#pragma hdrstop

#include "uWhere.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"

#define WC_ALL      (0)
#define WC_ANY      (1)
#define WC_NOT      (2)
#define WC_NOTALL   (3)
#define WC_CONTROLTHRESHOLD (10)

typedef enum {
    Operand=1,
    Operation,
    Value
} WhereClicked;

bool ComboBoxHiding;
WhereClicked WhatEditing;
int ComboPosition = 0;  //"позиция" для ComboBox
                        //0 -- операнд
                        //1 -- операция
                        //2 -- значение

//---------------------------------------------------------------------------
__fastcall TfmWhereCriteria::TfmWhereCriteria(TComponent* Owner)
    : TFrame(Owner)
{
    comboBox = new TComboBox(this);
      comboBox->OnCloseUp = ComboBox_OnCloseUp;
      comboBox->OnExit = ComboBox_OnExit;
      comboBox->OnKeyDown = ComboBox_OnKeyDown;
      comboBox->Parent = this;
      comboBox->ParentWindow = this;
      comboBox->Hide();
      comboBox->SendToBack();

    tvwCriteria->Items->AddObject(NULL, "Кожен крітерій співпадає", (TObject*)0);
}
//---------------------------------------------------------------------------

void TfmWhereCriteria::loadConfig(AnsiString iniFileName, TIBDatabase *db)
{
    if (!FileExists(iniFileName))
    {
        MessageBox(NULL, ("Error initializing search conditions frame:\n"
                            "File '"+iniFileName+"' does not exist.").c_str(), "Error", MB_ICONHAND);
        return;
    }

    std::auto_ptr<TIniFile> ini(new TIniFile(iniFileName));
    std::auto_ptr<TStrings> Sections(new TStringList());
    ini->ReadSections(Sections.get());

    if (Sections->Count > 0)
        sqlQuery = ini->ReadString(Sections->Strings[0], "clause", "");

    for(int i = 1; i < Sections->Count; i++)//читаем в циле все секции из файла ( пропустив первую )
    {
        CIniItem IniItem;
        std::auto_ptr<TStringList> Val(new TStringList());
        IniItem.fieldCaption = ini->ReadString(Sections->Strings[i], "fieldCaption", "");
        IniItem.fieldName = ini->ReadString(Sections->Strings[i], "fieldName", "");
        if ( IniItem.fieldName == "" )
            IniItem.fieldName = Sections->Strings[i];

        AnsiString fieldType = ini->ReadString(Sections->Strings[i], "fieldType", "");
        if ( fieldType == "ftOrdinaryNumeric"){
            IniItem.fieldType = "ftOrdinaryNumeric";
            IniItem.allowableActions->Add("="); IniItem.allowableActions->Add("<>"); IniItem.allowableActions->Add("<"); IniItem.allowableActions->Add(">"); IniItem.allowableActions->Add("<="); IniItem.allowableActions->Add(">=");
            IniItem.Values->Add(IntToStr(ini->ReadInteger(Sections->Strings[i], "fieldValue", -1)));
        }
        else
         if ( fieldType == "ftOrdinaryString" ){
              IniItem.fieldType = "ftOrdinaryString";
              IniItem.allowableActions->Add("="); IniItem.allowableActions->Add("<>"); IniItem.allowableActions->Add("<"); IniItem.allowableActions->Add(">"); IniItem.allowableActions->Add("<="); IniItem.allowableActions->Add(">="); IniItem.allowableActions->Add("похоже на");
              IniItem.Values->Add(ini->ReadString(Sections->Strings[i], "fieldValue", ""));
          }
          else
            if ( fieldType == "ftEnumeration" ){
                IniItem.fieldType = "ftEnumeration";
                IniItem.allowableActions->Add("="); IniItem.allowableActions->Add("<>");
                AnsiString value = " ";
                int valueCount = 1;//номер считываемого параметр-перечисления
                while ( value != "" ){
                    value = ini->ReadString(Sections->Strings[i], "value"+IntToStr(valueCount), "");
                    if ( value != "" ) IniItem.Values->Add(value);
                    valueCount++;
                }
            }
            else
            { //ftReference:
                IniItem.fieldType = "ftReference";
                IniItem.allowableActions->Add("="); IniItem.allowableActions->Add("<>");
                IniItem.Loaded = false;
                IniItem.lookupDataBase = db;
                IniItem.lookupKey = ini->ReadString(Sections->Strings[i], "lookupKey", "");
                IniItem.lookupTable = ini->ReadString(Sections->Strings[i], "lookupTable", "");
                IniItem.lookupValue = ini->ReadString(Sections->Strings[i], "lookupValue", "");
            }

        IniItems.push_back(IniItem);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfmWhereCriteria::EditOnKeyDown(TObject *Sender, WORD &Key, TShiftState Shift)
{
    TEdit * edit = dynamic_cast<TEdit*>(Sender);
    if ( (edit->Text != "") && (Key == VK_RETURN) )
    {
        edit->Hide();
        for (unsigned int itemIndex = 0; itemIndex < paramNodes.size(); itemIndex++)
            if ( paramNodes[itemIndex].ItemId == tvwCriteria->Selected->ItemId )
            {
                if ( edit->Text.Pos(",") != 0 )
                {
                    AnsiString str = edit->Text;
                    str.Insert(".", str.Pos(",")+1);
                    str.Delete(str.Pos(","), 1);
                    edit->Text = str;
                }
                paramNodes[itemIndex].Value = edit->Text;
                tvwCriteria->Selected->Text = paramNodes[itemIndex].Operand + "  " + paramNodes[itemIndex].Operation + "  " + edit->Text;
                break;
            }

        tvwCriteria->Parent->SetFocus();
        tvwCriteria->SetFocus();
        tvwCriteria->Selected->Focused = true;
    }
    else if ( ( Key == VK_CANCEL ) || ( Key == VK_CLEAR) || ( Key == VK_ESCAPE ) )
    {
        edit->Hide();

        tvwCriteria->Parent->SetFocus();
        tvwCriteria->SetFocus();
        tvwCriteria->Selected->Focused = true;
    }
}
//---------------------------------------------------------------------------
void __fastcall TfmWhereCriteria::EditOnExit(TObject *Sender)
{
    dynamic_cast<TWinControl*>(Sender)->Hide();

    tvwCriteria->Parent->SetFocus();
    tvwCriteria->SetFocus();
    tvwCriteria->Selected->Focused = true;
}
//---------------------------------------------------------------------------

void __fastcall TfmWhereCriteria::tvwCriteriaMouseDown(TObject *Sender, TMouseButton Button, TShiftState Shift, int X, int Y)
{
    THitTests ht = tvwCriteria->GetHitTestInfoAt(X, Y);
    Tag = ht.Contains(htAbove);
    bool Above        = ht.Contains(htAbove);
    bool Below        = ht.Contains(htBelow);
    bool Nowhere      = ht.Contains(htNowhere);
    bool OnItem       = ht.Contains(htOnItem);
    bool OnButton     = ht.Contains(htOnButton);
    bool OnIcon       = ht.Contains(htOnIcon);
    bool OnIndent     = ht.Contains(htOnIndent);
    bool OnLabel      = ht.Contains(htOnLabel);
    bool OnRight      = ht.Contains(htOnRight);
    bool OnStateIcon  = ht.Contains(htOnStateIcon);
    bool ToLeft       = ht.Contains(htToLeft);
    bool ToRight      = ht.Contains(htToRight);

    if ( (OnItem && (int)tvwCriteria->Selected->Data < WC_CONTROLTHRESHOLD) && ( Button == mbLeft ) ){
        comboBox->Items->Clear();
        comboBox->Items->AddObject("Кожен крітерій співпадає", (TObject*)0);
        comboBox->Items->AddObject("Будь-який крітерій співпадає", (TObject*)1);
        comboBox->Items->AddObject("Жоден крітерій не співпадає", (TObject*)2);
        comboBox->Items->AddObject("Хоча б один крітерій не співпадає", (TObject*)3);

        comboBox->Left = tvwCriteria->Indent;
        TTreeNode *tn = tvwCriteria->Selected;
        while (tn = tn->Parent)
            comboBox->Left = comboBox->Left + tvwCriteria->Indent;
        for (int i = 0; i < comboBox->Items->Count; i++) {
            if (tvwCriteria->Selected->Data == comboBox->Items->Objects[i]) {
                comboBox->ItemIndex = i;
                break;
            }
        }
        int itemHeight = (tvwCriteria->Canvas->TextExtent(tvwCriteria->Selected->Text).cy + 3);
        comboBox->Top = Y / itemHeight * itemHeight;
        if (comboBox->Top > this->ClientHeight - itemHeight)
            comboBox->Top = this->ClientHeight;
        comboBox->Visible = true;
        comboBox->SetFocus();
        comboBox->DroppedDown = true;
    }
      else if ( ( OnItem ) && ( Button == mbLeft ) )
      {
          bool needComboBox = true;//нужен ли ComboBox ( для введения значения)
          TTreeNode *tn = tvwCriteria->Selected;

          unsigned int itemIndex = 0;
          bool contain = false;
          //проверяем входит ли текущая нода в массив с параметрами
          for (itemIndex = 0; itemIndex < paramNodes.size(); itemIndex++)
              if ( paramNodes[itemIndex].ItemId == tn->ItemId )
              {
                  contain = true;
                  break;
              }

          TTreeNode * tnNode = tvwCriteria->Selected;
          int leftBorder = tvwCriteria->Indent;
          while (tnNode = tnNode->Parent)
              leftBorder += tvwCriteria->Indent;

          if ( contain )
          {
              if ( ( paramNodes[itemIndex].Operation != "" ) && ( X > leftBorder + tvwCriteria->Canvas->TextWidth(paramNodes[itemIndex].Operand + "  " + paramNodes[itemIndex].Operation + " ") ) )
              //введена операция и щелчок за операцией -- вводим значение
              {
                  WhatEditing = Value;
                  needComboBox = true;

                  if ( IniItems[paramNodes[itemIndex].IniItemsIndex].fieldType == "ftEnumeration" )
                  {
                      comboBox->Items->Clear();
                      for(int i = 0; i < IniItems[paramNodes[itemIndex].IniItemsIndex].Values->Count; i++)
                          comboBox->Items->AddObject(IniItems[paramNodes[itemIndex].IniItemsIndex].Values->Strings[i], (TObject *)i);
                  }
                  else
                    if ( IniItems[paramNodes[itemIndex].IniItemsIndex].fieldType == "ftReference" )
                    {//загружаем список значений
                        if ( !IniItems[paramNodes[itemIndex].IniItemsIndex].Loaded )
                        {
                            IniItems[paramNodes[itemIndex].IniItemsIndex].Loaded = true;
                            std::auto_ptr<TIBQuery> ibqQuery(new TIBQuery(IniItems[paramNodes[itemIndex].IniItemsIndex].lookupDataBase));
                              ibqQuery->Database = IniItems[paramNodes[itemIndex].IniItemsIndex].lookupDataBase;
                              ibqQuery->Transaction = IniItems[paramNodes[itemIndex].IniItemsIndex].lookupDataBase->DefaultTransaction;
                              ibqQuery->SQL->Text = "SELECT " + IniItems[paramNodes[itemIndex].IniItemsIndex].lookupKey + ", " + IniItems[paramNodes[itemIndex].IniItemsIndex].lookupValue + " FROM " + IniItems[paramNodes[itemIndex].IniItemsIndex].lookupTable + " ORDER BY " + IniItems[paramNodes[itemIndex].IniItemsIndex].lookupValue;
                              ibqQuery->Open();
                              ibqQuery->First();

                            while ( !ibqQuery->Eof )
                            {
                                IniItems[paramNodes[itemIndex].IniItemsIndex].Values->AddObject(ibqQuery->FieldByName(IniItems[paramNodes[itemIndex].IniItemsIndex].lookupValue)->AsString, (TObject *)ibqQuery->FieldByName(IniItems[paramNodes[itemIndex].IniItemsIndex].lookupKey)->AsInteger);
                                ibqQuery->Next();
                            }
                        }

                        comboBox->Items->Clear();
                        comboBox->Items->AddStrings(IniItems[paramNodes[itemIndex].IniItemsIndex].Values);
                    }
                    else
                        needComboBox = false;
              }
              else
                if ( ( paramNodes[itemIndex].Operation != "" ) && (( X > leftBorder + tvwCriteria->Canvas->TextWidth(paramNodes[itemIndex].Operand + " ") ) && ( X <= leftBorder + tvwCriteria->Canvas->TextWidth(paramNodes[itemIndex].Operand + "  " + paramNodes[itemIndex].Operation + " ") )) )
                //введена операция и щелчок на операции -- вводим операцию
                {
                    WhatEditing = Operation;
                    comboBox->Items->Clear();
                    for(int i = 0; i < IniItems[paramNodes[itemIndex].IniItemsIndex].allowableActions->Count; i++)
                        comboBox->Items->AddObject(IniItems[paramNodes[itemIndex].IniItemsIndex].allowableActions->Strings[i], (TObject *)i);
                }
                else
                  if ( ( paramNodes[itemIndex].Operation != "" ) && (( X > leftBorder ) && ( X <= leftBorder + tvwCriteria->Canvas->TextWidth(paramNodes[itemIndex].Operand + " ") )) )
                  //введена операция и щелчок на операнде  -- вводим операнд
                  {
                      WhatEditing = Operand;
                      comboBox->Items->Clear();
                      for(unsigned int i = 0; i < IniItems.size(); i++)
                          comboBox->Items->AddObject(IniItems[i].fieldCaption, (TObject *)i);
                  }
                  else
                    if ( X >= leftBorder + tvwCriteria->Canvas->TextWidth(paramNodes[itemIndex].Operand + " ") )
                    //введен только операнд и щелчок за операндом -- вводим операцию
                    {
                        WhatEditing = Operation;
                        comboBox->Items->Clear();
                        for(int i = 0; i < IniItems[paramNodes[itemIndex].IniItemsIndex].allowableActions->Count; i++)
                            comboBox->Items->AddObject(IniItems[paramNodes[itemIndex].IniItemsIndex].allowableActions->Strings[i], (TObject *)i);
                    }
                    else
                    //введен только операнд и щелчек на операнде -- вводим операнд
                    {
                        WhatEditing = Operand;
                        comboBox->Items->Clear();
                        for(unsigned int i = 0; i < IniItems.size(); i++)
                            comboBox->Items->AddObject(IniItems[i].fieldCaption, (TObject *)i);
                    }
          } else
            //первое вхождение -- вводим операнд
            {
                WhatEditing = Operand;
                comboBox->Items->Clear();
                for(unsigned int i = 0; i < IniItems.size(); i++)
                    comboBox->Items->AddObject(IniItems[i].fieldCaption, (TObject *)i);
            }

          if ( needComboBox )
          {
              comboBox->Left = tvwCriteria->Indent;
              while (tn = tn->Parent)
                  comboBox->Left += tvwCriteria->Indent;
              if ( contain )
              {
                  if ( WhatEditing > Operand )
                      comboBox->Left += tvwCriteria->Canvas->TextWidth(paramNodes[itemIndex].Operand + "  ");
                  if ( WhatEditing > Operation )
                      comboBox->Left += tvwCriteria->Canvas->TextWidth(paramNodes[itemIndex].Operation + "  ");
              }
              int itemHeight = (tvwCriteria->Canvas->TextExtent(tvwCriteria->Selected->Text).cy + 3);
              comboBox->Top = Y / itemHeight * itemHeight;
              if ( comboBox->Top > this->ClientHeight - itemHeight)
                  comboBox->Top = this->ClientHeight;

              comboBox->Width = 0;
              for ( int i = 0; i < comboBox->Items->Count; i++)
                  if ( comboBox->Canvas->TextWidth(comboBox->Items->Strings[i] + "   ") > comboBox->Width ) comboBox->Width = comboBox->Canvas->TextWidth(comboBox->Items->Strings[i] + "         ");

              int comboHeight = itemHeight*comboBox->Items->Count;
              if ( itemHeight*comboBox->Items->Count > Screen->Height - comboBox->Top )
                  comboHeight = Screen->Height - comboBox->Top;
              comboBox->DropDownCount = comboHeight / itemHeight;
              comboBox->Sorted = true;
              comboBox->Show();
              comboBox->SetFocus();
              comboBox->DroppedDown = true;
          }
          else
          {//использем TEdit
              edit->OnExit = EditOnExit;
              edit->OnKeyDown = EditOnKeyDown;

              edit->Left = tvwCriteria->Indent;
              while (tn = tn->Parent)
                  edit->Left += tvwCriteria->Indent;
              if ( contain )
                  edit->Left += tvwCriteria->Canvas->TextWidth(paramNodes[itemIndex].Operand + "  " + paramNodes[itemIndex].Operation + "  ");
              edit->Width = this->ClientWidth - edit->Left;
              int itemHeight = (tvwCriteria->Canvas->TextExtent(tvwCriteria->Selected->Text).cy + 3);
              edit->Top = Y / itemHeight * itemHeight;
              if ( edit->Top > this->ClientHeight - itemHeight)
                  edit->Top = this->ClientHeight;

              edit->Text = paramNodes[itemIndex].Value;
              edit->Show();
              edit->BringToFront();
              edit->SetFocus();
          }

      }
}
//---------------------------------------------------------------------------

void __fastcall TfmWhereCriteria::ComboBox_OnCloseUp(TObject *Sender)
{
    if ( !ComboBoxHiding )
        ComboBox_OnKeyDown(Sender, VK_RETURN, TShiftState());

    tvwCriteria->Parent->SetFocus();
    tvwCriteria->SetFocus();
    tvwCriteria->Selected->Focused = true;
}
//---------------------------------------------------------------------------

void __fastcall TfmWhereCriteria::ComboBox_OnExit(TObject *Sender)
{
    dynamic_cast<TWinControl*>(Sender)->Hide();
    dynamic_cast<TComboBox*>(Sender)->OnKeyDown = ComboBox_OnKeyDown;
}
//---------------------------------------------------------------------------

void __fastcall TfmWhereCriteria::ComboBox_OnKeyDown(TObject *Sender, WORD &Key, TShiftState Shift)
{
    ComboBoxHiding = false;
    if ( ( (int)tvwCriteria->Selected->Data < WC_CONTROLTHRESHOLD ) && ( Key == VK_RETURN ) && ( Shift == TShiftState() ) )
    {//параметры составного критерия
        TComboBox * comboBox = dynamic_cast<TComboBox*>(Sender);
        comboBox->Hide();

        tvwCriteria->Selected->Text = comboBox->Items->Strings[comboBox->ItemIndex];
        tvwCriteria->Selected->Data = comboBox->Items->Objects[comboBox->ItemIndex];

        tvwCriteria->Parent->SetFocus();
        tvwCriteria->SetFocus();
        tvwCriteria->Selected->Focused = true;
    }
    else if ( ( Key == VK_RETURN ) && ( Shift == TShiftState() ) )
    {
        TComboBox * comboBox = dynamic_cast<TComboBox*>(Sender);
        if ( comboBox->Visible )
            comboBox->Hide();

        if ( comboBox->ItemIndex != -1 )
        {
            if ( WhatEditing == Operand )
            {

                bool isNodeNew = true;
                //проверяем нет ли такой ноды в уже введенных
                for (unsigned int itemIndex = 0; itemIndex < paramNodes.size(); itemIndex++)
                    if ( paramNodes[itemIndex].ItemId == tvwCriteria->Selected->ItemId )
                    {
                        isNodeNew = false;

                        paramNodes[itemIndex].IniItemsIndex = (int)comboBox->Items->Objects[comboBox->ItemIndex];
                        paramNodes[itemIndex].Operand = comboBox->Items->Strings[comboBox->ItemIndex];
                        paramNodes[itemIndex].Operation = "";
                        paramNodes[itemIndex].Value = "";

                        break;
                    }

                if ( isNodeNew )
                {
                    CNodeParam nodeParam;
                    nodeParam.Assigned = true;
                    nodeParam.ItemId = tvwCriteria->Selected->ItemId;

                    nodeParam.IniItemsIndex = (int)comboBox->Items->Objects[comboBox->ItemIndex];
                    nodeParam.Operand = comboBox->Items->Strings[comboBox->ItemIndex];
                    paramNodes.push_back(nodeParam);
                }
                tvwCriteria->Selected->Text = comboBox->Items->Strings[comboBox->ItemIndex] + "          ";

            }
            else if ( WhatEditing == Operation )
            {//обработчик введения операции
                for (unsigned int itemIndex = 0; itemIndex < paramNodes.size(); itemIndex++)
                    if ( paramNodes[itemIndex].ItemId == tvwCriteria->Selected->ItemId )
                    {
                        paramNodes[itemIndex].Operation = comboBox->Items->Strings[comboBox->ItemIndex];
                        tvwCriteria->Selected->Text = paramNodes[itemIndex].Operand + "  " + comboBox->Items->Strings[comboBox->ItemIndex] + "          ";
                        break;
                    }
            }
            else if ( WhatEditing == Value )
            {
                for (unsigned int itemIndex = 0; itemIndex < paramNodes.size(); itemIndex++)
                    if ( paramNodes[itemIndex].ItemId == tvwCriteria->Selected->ItemId )
                    {
                        tvwCriteria->Selected->Text = paramNodes[itemIndex].Operand + "  " + paramNodes[itemIndex].Operation + "  " + comboBox->Items->Strings[comboBox->ItemIndex];

                        if ( IniItems[paramNodes[itemIndex].IniItemsIndex].fieldType == "ftReference" )
                            paramNodes[itemIndex].Value = IntToStr((int)comboBox->Items->Objects[comboBox->ItemIndex]);
                        else
                            paramNodes[itemIndex].Value = comboBox->Items->Strings[comboBox->ItemIndex];
                        break;
                    }
            }
        }
        tvwCriteria->Refresh();

        tvwCriteria->Parent->SetFocus();
        tvwCriteria->SetFocus();
        tvwCriteria->Selected->Focused = true;
    }
    else if ( Key == VK_LEFT )
    {
        ComboBoxHiding = true;
        ComboPosition--;
        if ( ComboPosition < 0 )
            ComboPosition = 0;
        tvwCriteriaKeyDown(Sender, VK_RETURN, TShiftState());//эмуляция нжатия кнопки
    }
    else if ( Key == VK_RIGHT )
    {
        ComboBoxHiding = true;
        comboBox->Hide();
        //проверяем нет ли такой ноды в уже введенных
        for (unsigned int itemIndex = 0; itemIndex < paramNodes.size(); itemIndex++)
            if ( paramNodes[itemIndex].ItemId == tvwCriteria->Selected->ItemId )
                if ( (ComboPosition == 0)&&(paramNodes[itemIndex].Operand != "") || (ComboPosition == 1)&&(paramNodes[itemIndex].Operation != "") )
                {
                        ComboPosition++;
                        tvwCriteriaKeyDown(Sender, VK_RETURN, TShiftState());//эмуляция нжатия кнопки
                        break;
                }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfmWhereCriteria::tvwCriteriaKeyDown(TObject *Sender, WORD &Key, TShiftState Shift)
{//для управления кнопками
    AnsiString senderClassName = Sender->ClassName();
    if ( senderClassName != "TComboBox" )
        ComboPosition = 0;

    bool contain = false;
    bool needComboBox = false;
    unsigned int itemIndex = 0;

    if ( ( Key == VK_RETURN ) && ( Shift == TShiftState() ) && ( (int)tvwCriteria->Selected->Data < WC_CONTROLTHRESHOLD ) )//изменение статуса
    {
        needComboBox = true;
        comboBox->Items->Clear();
        comboBox->Items->AddObject("Кожен крітерій співпадає", (TObject*)0);
        comboBox->Items->AddObject("Будь-який крітерій співпадає", (TObject*)1);
        comboBox->Items->AddObject("Жоден крітерій не співпадає", (TObject*)2);
        comboBox->Items->AddObject("Хоча б один крітерій не співпадає", (TObject*)3);
    }
    else if ( ( Key == VK_RETURN ) && ( Shift == TShiftState() ) && ( (int)tvwCriteria->Selected->Data == WC_CONTROLTHRESHOLD ) )//добавление нового критерия
    {
        needComboBox = true;//нужен ли ComboBox ( для введения значения)
        TTreeNode *tn = tvwCriteria->Selected;

        //проверяем входит ли текущая нода в массив с параметрами
        for (itemIndex = 0; itemIndex < paramNodes.size(); itemIndex++)
            if ( paramNodes[itemIndex].ItemId == tn->ItemId )
            {
                contain = true;
                break;
            }

        if ( ( !contain ) || ( ComboPosition == 0) )
        {//первое вхождение -- вводим операнд
            needComboBox = true;
            ComboPosition = 0;
            WhatEditing = Operand;
            comboBox->Hide();
            comboBox->Items->Clear();
            for(unsigned int i = 0; i < IniItems.size(); i++)
                comboBox->Items->AddObject(IniItems[i].fieldCaption, (TObject *)i);
        }
        else
        {
            if ( ComboPosition == 2 )
            {//вводим значение

                comboBox->Hide();
                WhatEditing = Value;
                needComboBox = true;

                if ( IniItems[paramNodes[itemIndex].IniItemsIndex].fieldType == "ftEnumeration" )
                {
                    comboBox->Items->Clear();
                    for(int i = 0; i < IniItems[paramNodes[itemIndex].IniItemsIndex].Values->Count; i++)
                        comboBox->Items->AddObject(IniItems[paramNodes[itemIndex].IniItemsIndex].Values->Strings[i], (TObject *)i);
                }
                else if ( IniItems[paramNodes[itemIndex].IniItemsIndex].fieldType == "ftReference" )
                {
                        if ( !IniItems[paramNodes[itemIndex].IniItemsIndex].Loaded )
                        {
                            IniItems[paramNodes[itemIndex].IniItemsIndex].Loaded = true;
                            std::auto_ptr<TIBQuery> ibqQuery(new TIBQuery(IniItems[paramNodes[itemIndex].IniItemsIndex].lookupDataBase));
                              ibqQuery->Database = IniItems[paramNodes[itemIndex].IniItemsIndex].lookupDataBase;
                              ibqQuery->Transaction = IniItems[paramNodes[itemIndex].IniItemsIndex].lookupDataBase->DefaultTransaction;
                              ibqQuery->SQL->Text = "SELECT " + IniItems[paramNodes[itemIndex].IniItemsIndex].lookupKey + ", " + IniItems[paramNodes[itemIndex].IniItemsIndex].lookupValue + " FROM " + IniItems[paramNodes[itemIndex].IniItemsIndex].lookupTable + " ORDER BY " + IniItems[paramNodes[itemIndex].IniItemsIndex].lookupValue;
                              ibqQuery->Open();
                              ibqQuery->First();

                            while ( !ibqQuery->Eof )
                            {
                                IniItems[paramNodes[itemIndex].IniItemsIndex].Values->AddObject(ibqQuery->FieldByName(IniItems[paramNodes[itemIndex].IniItemsIndex].lookupValue)->AsString, (TObject *)ibqQuery->FieldByName(IniItems[paramNodes[itemIndex].IniItemsIndex].lookupKey)->AsInteger);
                                ibqQuery->Next();
                            }
                        }

                        comboBox->Items->Clear();
                        comboBox->Items->AddStrings(IniItems[paramNodes[itemIndex].IniItemsIndex].Values);
                }
                else
                {
                    needComboBox = false;
                    comboBox->Hide();
                }
            }
            else
            {//вводим операцию
                WhatEditing = Operation;
                comboBox->Hide();
                comboBox->Items->Clear();
                for(int i = 0; i < IniItems[paramNodes[itemIndex].IniItemsIndex].allowableActions->Count; i++)
                    comboBox->Items->AddObject(IniItems[paramNodes[itemIndex].IniItemsIndex].allowableActions->Strings[i], (TObject *)i);
            }
        }

        if ( !needComboBox )
        {//использем TEdit
            edit->OnExit = EditOnExit;
            edit->OnKeyDown = EditOnKeyDown;

            tn = tvwCriteria->Selected;
            edit->Left = tvwCriteria->Indent;
            while (tn = tn->Parent)
                edit->Left += tvwCriteria->Indent;
            if ( contain )
                edit->Left += tvwCriteria->Canvas->TextWidth(paramNodes[itemIndex].Operand + "  " + paramNodes[itemIndex].Operation + "  ");
            edit->Width = ClientWidth - edit->Left;
            int itemHeight = (tvwCriteria->Canvas->TextExtent(tvwCriteria->Selected->Text).cy + 3);
            edit->Top = tvwCriteria->Top + tvwCriteria->Selected->AbsoluteIndex * itemHeight;
            if ( edit->Top > ClientHeight - itemHeight)
                edit->Top = ClientHeight;

            edit->Text = paramNodes[itemIndex].Value;
            edit->BringToFront();
            edit->Show();
            edit->SetFocus();
        }
    }

    if ( needComboBox )
    {
        for(int i = 0; i < comboBox->Items->Count; i++)
            if( comboBox->ClientWidth < 1.05*comboBox->Canvas->TextWidth(comboBox->Items->Strings[i]) )
                comboBox->ClientWidth = 1.05*comboBox->Canvas->TextWidth(comboBox->Items->Strings[i]);

        comboBox->Left = tvwCriteria->Indent;
        TTreeNode *tn = tvwCriteria->Selected;
        while (tn = tn->Parent)
            comboBox->Left += tvwCriteria->Indent;
        if ( contain )
        {
            if ( WhatEditing > Operand )
                comboBox->Left = tvwCriteria->Canvas->TextWidth(paramNodes[itemIndex].Operand + "  ");
            if ( WhatEditing > Operation )
                comboBox->Left = tvwCriteria->Canvas->TextWidth(paramNodes[itemIndex].Operation + "  ");
        }

        int itemHeight = (tvwCriteria->Canvas->TextExtent(tvwCriteria->Selected->Text).cy + 3);

        comboBox->Top = tvwCriteria->Top + tvwCriteria->Selected->AbsoluteIndex * itemHeight;
        if (comboBox->Top > this->ClientHeight - itemHeight)
            comboBox->Top = this->ClientHeight;

         int comboHeight = itemHeight*comboBox->Items->Count;
         if ( itemHeight*comboBox->Items->Count > Screen->Height - comboBox->Top )
              comboHeight = Screen->Height - comboBox->Top;
         comboBox->DropDownCount = comboHeight / itemHeight;

        comboBox->Sorted = true;
        comboBox->Show();
        comboBox->ItemIndex = 0;
        comboBox->SetFocus();
        comboBox->DroppedDown = true;
    }
}
//---------------------------------------------------------------------------

void __fastcall TfmWhereCriteria::mniAddConditionClick(TObject *Sender)
{
    if ((int)tvwCriteria->Selected->Data < WC_CONTROLTHRESHOLD)
        tvwCriteria->Items->AddChildObject(tvwCriteria->Selected, "новий крітерій", (void*)WC_CONTROLTHRESHOLD);
    else
        tvwCriteria->Items->AddObject(tvwCriteria->Selected, "новий крітерій", (void*)WC_CONTROLTHRESHOLD);
    tvwCriteria->Selected->Expand(false);
}
//---------------------------------------------------------------------------

void __fastcall TfmWhereCriteria::mniAddComposeConditionClick(TObject *Sender)
{
    if ((int)tvwCriteria->Selected->Data < WC_CONTROLTHRESHOLD)
        tvwCriteria->Items->AddChildObject(tvwCriteria->Selected, "Кожен крітерій співпадає", (TObject*)0);
    else
        tvwCriteria->Items->AddObject(tvwCriteria->Selected, "Кожен крітерій співпадає", (TObject*)0);
    tvwCriteria->Selected->Expand(false);
 }
//---------------------------------------------------------------------------

void __fastcall TfmWhereCriteria::mniMakeComposeConditionClick(TObject *Sender)
{
    if ((int)tvwCriteria->Selected->Data >= WC_CONTROLTHRESHOLD) {
        tvwCriteria->Selected->Text = comboBox->Items->Strings[0];
        tvwCriteria->Selected->Data = comboBox->Items->Objects[0];
        mniAddConditionClick(Sender);
    }
}
//---------------------------------------------------------------------------
void __fastcall TfmWhereCriteria::DeleteConditionClick(TObject *Sender)
{
    tvwCriteria->Items->Delete(tvwCriteria->Selected);
}
//---------------------------------------------------------------------------
AnsiString __fastcall TfmWhereCriteria::getClause()
{
    AnsiString query = "";
    TTreeNode * tnNode = tvwCriteria->Items->GetFirstNode();
    int currentLevel = 0; int openedBrakeds = 0;
    if ( ((int)tnNode->Data == 2) || ((int)tnNode->Data == 3) )
      query += "NOT";
    query += " ( "; openedBrakeds++;
    while ( tnNode = tnNode->GetNext() )
    {
        if ( tnNode->Level <= currentLevel )
        {
            if ( tnNode->Level < currentLevel )
            {
                for (int i = 1; i < currentLevel - tnNode->Level; i++ )
                {//закрываем скобки
                    query += " ) ";
                    openedBrakeds--;
                }
            }
            if ( ((int)tnNode->Parent->Data == 0) || ((int)tnNode->Parent->Data == 2) )
                query += " AND ";
            else if ( ((int)tnNode->Parent->Data == 1) || ((int)tnNode->Parent->Data == 3) )
                query += " OR ";
        }
        currentLevel = tnNode->Level;
        if ( tnNode->HasChildren )
        {
            if ( ((int)tnNode->Data == 2) || ((int)tnNode->Data == 3) )
                query += " NOT";
            query += " ( ";openedBrakeds++;
        }
        else
        {
            query += " ( ";openedBrakeds++;
            for (unsigned int itemIndex = 0; itemIndex < paramNodes.size(); itemIndex++)
                if ( paramNodes[itemIndex].ItemId == tnNode->ItemId )
                {
                    query += IniItems[paramNodes[itemIndex].IniItemsIndex].fieldName;
                    if ( paramNodes[itemIndex].Operation == "похоже на" )
                        query += " LIKE '%" + paramNodes[itemIndex].Value.UpperCase() + "%'";
                    else
                    {
                        query += " " + paramNodes[itemIndex].Operation;
                        if ( ( IniItems[paramNodes[itemIndex].IniItemsIndex].fieldType == "ftOrdinaryString" ) || (( IniItems[paramNodes[itemIndex].IniItemsIndex].fieldType == "ftEnumeration" )) )
                            query += " '" + paramNodes[itemIndex].Value + "'";
                        else
                            query += " " + paramNodes[itemIndex].Value;
                    }
                    break;
                }
            query += " ) ";openedBrakeds--;
        }
    }
    for (int i = 0; i < openedBrakeds; i++ )//закрываем скобки
        query += " ) ";

    if ( ( query == " (  ) " ) || ( query == "NOT (  ) " ) )
        query = "";
    return query;
}
//---------------------------------------------------------------------------
