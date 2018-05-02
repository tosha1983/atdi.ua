//---------------------------------------------------------------------
#include <vcl.h>
#pragma hdrstop

#include "uSelectServer.h"
#include <wstring.h>
#include "uSaveServer.h"
#include <memory>
#include <vcl\mtshlpr.h>
//---------------------------------------------------------------------
#pragma resource "*.dfm"
TdlgSelectServer *dlgSelectServer;
//---------------------------------------------------------------------
__fastcall TdlgSelectServer::TdlgSelectServer(TComponent* AOwner)
	: TForm(AOwner)
{
}
//---------------------------------------------------------------------
void __fastcall TdlgSelectServer::btFileClick(TObject *Sender)
{
    if (!OpenDialog1->Execute())
        return;

    Clear();

    edFileName->Text = OpenDialog1->FileName;

    ITypeLib *typeLib;
    GUID servIid,iid2;
    TLIBATTR *libAtr;
    
    if (LoadTypeLib(PWideChar(WideString(edFileName->Text)), &typeLib) == S_OK)
    {
        typeLib->AddRef();

        //if(RegisterTypeLib(typeLib,PWideChar(WideString(edFileName->Text)),NULL) == S_OK) {
        typeLib->GetLibAttr(&libAtr);

        unsigned int typeCount;
        TYPEKIND tKind;
        ITypeInfo *typeInfo;

        typeCount = typeLib->GetTypeInfoCount();

        wchar_t *nameBuf, *infoBuf;
        WideString name,info,buf,servGuid;
        TYPEATTR *typeAtr;
        IUnknown *pTest,*pTest2;
        short interfaceQuant;
        IClassFactory *classFact;

        for(int k = 0; k < typeCount; ++k)
        {
            typeLib->GetTypeInfoType(k,&tKind);

            if (tKind == TKIND_COCLASS)
            {
                typeLib->GetTypeInfo(k,&typeInfo);
                typeInfo->GetTypeAttr(&typeAtr);
                if (CoGetClassObject(typeAtr->guid,0x1,NULL,IID_IClassFactory,(void**)&classFact) == S_OK)
                {
                //typeInfo->CreateInstance(,,)    SUCCEEDED(      libAtr->guid       IID_IUnknown   //REGDB_E_CLASSNOTREG)//
                //CoCreateInstance(typeAtr->guid,NULL,0x1,iid ,(void**)&pTest) ==  S_OK)
                    if (classFact->CreateInstance(NULL,IID_IUnknown,(void**)&pTest) == S_OK)
                        if (pTest->QueryInterface(iid,(void**)&pTest2) == S_OK)
                        {
                            servGuid = Sysutils::GUIDToString(typeAtr->guid);
                            typeLib->GetDocumentation(k,&nameBuf,&infoBuf,NULL,NULL);
                            name = nameBuf;
                            info = infoBuf;
                            buf = "";
                            if (info != buf)
                                info = " - " + info;
                            ServerList->AddItem(servGuid+ " : " + name + info, NULL);
                            guidList.push_back(servGuid);
                            nameList.push_back(name + info);
                        }
                }
                typeInfo->ReleaseTypeAttr(typeAtr);
                typeInfo->Release();
            }  
        }
        //  UnRegisterTypeLib(libAtr->guid,libAtr->wMajorVerNum,libAtr->wMinorVerNum,libAtr->lcid,libAtr->syskind);
        //  }
        typeLib->Release();

        if (ServerList->Count > 0)
        {
            ServerList->ItemIndex = 0;
            if (ServerList->OnClick)
                ServerList->OnClick(Sender);
            ServerList->Selected[0] = true;
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TdlgSelectServer::OKBtnClick(TObject *Sender)
{
    if (ServerList->Count == 0)
    {
        ModalResult = mrNone;
        throw *(new Exception("Список пуст"));
    }
    
    if (ServerList->Count == 1)
        ServerList->Selected[0] = true;

    bool nothingSelected = true;
    for (int i = 0; i < ServerList->Count; i++)
        if (ServerList->Selected[i])
        {
            nothingSelected = false;
            break;
        }
    if (nothingSelected)
    {
        ModalResult = mrNone;
        throw *(new Exception("Выберите хотя бы один сервер"));
    }

    if (dlgSaveServer == NULL)
        dlgSaveServer = new TdlgSaveServer(Application);
    dlgSaveServer->Caption = "Сохранить данный сервер?";
    dlgSaveServer->OKBtn->Caption = "Сохранить";
    dlgSaveServer->CancelBtn->Caption = "Не сохранять";
    dlgSaveServer->Height = 189;
    dlgSaveServer->Label3->Visible = false;
    dlgSaveServer->edServName->Visible = false;

    for (int i = 0; i < ServerList->Items->Count; ++i)
    {
        if (ServerList->Selected[i] == true)
        {
            dlgSaveServer->edGuid->Text = guidList[i];
            dlgSaveServer->memDescr->Lines->Text = nameList[i];
            if (dlgSaveServer->ShowModal() == mrOk)
            {
                ServParams bufServ;
                bufServ.guid = dlgSaveServer->edGuid->Text;
                bufServ.name = dlgSaveServer->memDescr->Lines->Text;
                arr.push_back(bufServ);
                ++addCount;
            }
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TdlgSelectServer::AddNewServClick(TObject *Sender)
{
    if (dlgSaveServer == NULL)
        dlgSaveServer = new TdlgSaveServer(Application);

    dlgSaveServer->Caption = "Добавление нового сервера";
    dlgSaveServer->OKBtn->Caption = "Добавить";
    dlgSaveServer->CancelBtn->Caption = "Отменить";
    dlgSaveServer->edGuid->Text = "";
    dlgSaveServer->edGuid->Enabled = true;
    dlgSaveServer->edGuid->Color = clWindow;

    dlgSaveServer->memDescr->Lines->Text = "пустой GUID означает, что сервер не используется";
    dlgSaveServer->edServName->Text = "";
    dlgSaveServer->Height = 220;
    dlgSaveServer->Label3->Visible = true;
    dlgSaveServer->edServName->Visible = true;
    if(dlgSaveServer->ShowModal() == mrOk && dlgSaveServer->edServName->Text != "")
    {
        ServerList->AddItem(dlgSaveServer->edGuid->Text.Length() > 0 ?
                dlgSaveServer->edGuid->Text + " : " + dlgSaveServer->edServName->Text :
                dlgSaveServer->edServName->Text,
                NULL);
        ServerList->Selected[ServerList->Count-1] = true;
        guidList.push_back(dlgSaveServer->edGuid->Text);
        nameList.push_back(dlgSaveServer->edServName->Text);
    }
}
//---------------------------------------------------------------------------

void __fastcall TdlgSelectServer::Clear()
{
    edFileName->Text = "";
    ServerList->Clear();
    guidList.clear();
    nameList.clear();
    addCount = 0;
}
