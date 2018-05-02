//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop
//---------------------------------------------------------------------------
#include <atl\atlmod.h>
#include <atl\atlmod.h>
#include "CoCalcProgressImpl.h"
#include "CoPWRCallbackImpl.h"
#include "CoStorageImpl.h"
USEFORM("uMainForm.cpp", frmMain);
USEFORM("uMainDm.cpp", dmMain); /* TDataModule: File Type */
USEFORMNS("uConnect.pas", Uconnect, dlgConnect);
USEFORMNS("uEditBases.pas", Ueditbases, dlgEditBase);
USEFORMNS("uAbout.pas", Uabout, dlgAboutBox);
USEFORM("uFrmTxBase.cpp", frmTxBase);
USEFORM("uFrmTxBaseAir.cpp", frmTxBaseAir);
USEFORM("uFrmTxBaseAirAnalog.cpp", frmTxBaseAirAnalog);
USEFORM("uFrmTxBaseAirDigital.cpp", frmTxBaseAirDigital);
USEFORM("uFrmTxLfMf.cpp", frmTxLfMf);
USEFORM("uFrmTxVHF.cpp", frmTxVHF);
USEFORM("uFrmTxTVA.cpp", frmTxTVA);
USEFORM("uFrmTxDAB.cpp", frmTxDAB);
USEFORM("uFrmTxDVB.cpp", frmTxDVB);
USEFORM("uFrmTxCTV.cpp", frmTxCTV);
USEFORM("uFrmTxFxm.cpp", frmTxFxm);
USEFORM("uFrmDocumentsSettings.cpp", frmDocumentsSettings);
USEFORM("uCalcParamsDlg.cpp", dlgCalcParams);
USEFORM("uBaseObjForm.cpp", frmBaseObjForm);
USEFORM("uPlanning.cpp", frmPlanning);
USEFORM("uSelection.cpp", frmSelection);
USEFORM("uNewTxWizard.cpp", frmNewTxWizard);
USEFORM("uTable36.cpp", frmTable36);
USEFORM("uExplorer.cpp", frmExplorer);
USEFORM("uNewSelection.cpp", dlgNewSelection);
USEFORM("uBaseList.cpp", frmBaseList);
USEFORM("uBaseListTree.cpp", frmBaseListTree);
USEFORM("uSelectTxTree.cpp", frmSelectTxTree);
USEFORM("uSectorDlg.cpp", dlgSector);
USEFORM("uOffsetRangeDlg.cpp", OffsetRangeDlg);
USEFORM("uUserActivityLog.cpp", frmUserActivityLog);
USEFORM("uWhere.cpp", fmWhereCriteria); /* TFrame: File Type */
USEFORM("uSearch.cpp", frmSearch);
USEFORM("uListFrequencyGrid.cpp", frmListFrequencyGrid);
USEFORM("uFormOwner.cpp", frmFormTRK);
USEFORM("uDuelResult.cpp", frmDuelResult);
USEFORM("DlgSelectTypeDoc.cpp", SelectTypeDoc);
USEFORM("uProfileView.cpp", fmProfileView); /* TFrame: File Type */
USEFORM("uReliefFrm.cpp", frmRelief);
USEFORM("SelectColumnsForm.cpp", frmSelectColumns);
USEFORM("uExportDlg.cpp", dlgExport);
USEFORM("uContourForm.cpp", frmContour);
USEFORM("uFrmPoint.cpp", frmPoint);
USEFORM("uEnterCoordDlg.cpp", dlgEnterCoord);
USEFORM("uImportDigLayer.cpp", dlgImportDigLayer);
USEFORM("uItuImport.cpp", frmRrc06Import);
USEFORM("uItuExport.cpp", frmRrc06Export);
USEFORM("uAllotmentForm.cpp", frmAllotment);
USEFORM("uDlgList.cpp", dlgList);
USEFORM("uDlgConstrSet.cpp", dlgConstrSet);
USEFORM("uDlgEminAndNote.cpp", dlgEminAndNote);
USEFORM("BaseMap.cpp", BaseMapFrame); /* TFrame: File Type */
USEFORM("CustomMap.cpp", CustomMapFrame); /* TFrame: File Type */
USEFORM("uDlgMapConf.cpp", dlgMapConf);
USEFORM("uOtherTerrSrvc.cpp", frmOtherTerrSrvc);
USEFORM("uSelectStations.cpp", dgSelectStations);
USEFORM("uNewChPlanDlg.cpp", dgCreateChannelPlan);
USEFORM("uSaveServer.cpp", dlgSaveServer);
USEFORM("uSelectServer.cpp", dlgSelectServer);
USEFORM("uLisObjectGrid.cpp", LisObjectGrid); /* TFrame: File Type */
USEFORM("uListAccountCondition.cpp", frmListAccountCondition);
USEFORM("uListAnalogRadioSystem.cpp", frmListAnalogRadioSystem);
USEFORM("uListAnalogTeleSystem.cpp", frmListAnalogTeleSystem);
USEFORM("uListArea.cpp", frmListArea);
USEFORM("uListBank.cpp", frmListBank);
USEFORM("uListBlockDAB.cpp", frmListBlockDAB);
USEFORM("uListCarrierGuardInterval.cpp", frmListCarrierGuardInterval);
USEFORM("uListChannel.cpp", frmListChannel);
USEFORM("uListCity.cpp", frmListCity);
USEFORM("uListCityModal.cpp", frmListCityModal);
USEFORM("uListCountry.cpp", frmListCountry);
USEFORM("uListDigAllotments.cpp", frmListDigAllotments);
USEFORM("uListDigitalSystem.cpp", frmListDigitalSystem);
USEFORM("uListDistrict.cpp", frmListDistrict);
USEFORM("uListEquipment.cpp", frmListEquipment);
USEFORM("uListLicense.cpp", frmListLicense);
USEFORM("uListMinFieldStrength.cpp", frmListMinFieldStrength);
USEFORM("uListOffsetCarryFreqTVA.cpp", frmListOffsetCarryFreqTVA);
USEFORM("uListOwner.cpp", frmListOwner);
USEFORM("uListRadioService.cpp", frmListRadioService);
USEFORM("uListStand.cpp", frmListStand);
USEFORM("uListStreet.cpp", frmListStreet);
USEFORM("uListSynhroNet.cpp", frmListSynhroNet);
USEFORM("uListSystemCast.cpp", frmListSystemCast);
USEFORM("uListSubareas.cpp", frmListDigSubareas);
USEFORM("uListTelecomOrganization.cpp", frmListTelecomOrganization);
USEFORM("uListTPOnBorder.cpp", frmListTPOnBorder);
USEFORM("uListTransmitters.cpp", frmListTransmitters);
USEFORM("uListTypeReceive.cpp", frmListTypeReceive);
USEFORM("uListTypeSFN.cpp", frmListTypeSFN);
USEFORM("uListUncompatibleChannels.cpp", frmListUncompatibleChannels);
USEFORM("uLisObjectGridForm.cpp", LisObjectGridForm);
USEFORM("uCoordZoneFieldStr.cpp", uCoordZoneFieldStr);
//---------------------------------------------------------------------------
TComModule _ProjectModule(0 /*InitATLServer*/);
TComModule &_Module = _ProjectModule;

// The ATL Object map holds an array of _ATL_OBJMAP_ENTRY structures that
// described the objects of your OLE server. The MAP is handed to your
// project's CComModule-derived _Module object via the Init method.
//
BEGIN_OBJECT_MAP(ObjectMap)
  OBJECT_ENTRY(CLSID_CoCalcProgress, TCoCalcProgressImpl)
  OBJECT_ENTRY(CLSID_CoPWRCallback, TCoPWRCallbackImpl)
  OBJECT_ENTRY(CLSID_CoLisBcStorage, TCoLisBcStorageImpl)
END_OBJECT_MAP()
//---------------------------------------------------------------------------
WINAPI WinMain(HINSTANCE, HINSTANCE, LPSTR, int)
{
    try
    {
         Application->Initialize();
         Application->Title = "ЛІС-Мовлення";
         Application->CreateForm(__classid(TdmMain), &dmMain);
         Application->CreateForm(__classid(TfrmMain), &frmMain);
         Application->Run();
     }
    catch (Exception &exception)
    {
         Application->ShowException(&exception);
    }
    catch (...)
    {
         try
         {
             throw Exception("");
         }
         catch (Exception &exception)
         {
             Application->ShowException(&exception);
         }
    }
    return 0;
}
//---------------------------------------------------------------------------

