using System;
using System.Collections.Generic;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.WebQueryManager;
using Atdi.AppServer.Contracts.WebQuery;
using Atdi.AppServer;

namespace Atdi.AppServer.AppServices.WebQueryManager
{
    public class SaveChangesAppOperationHandler
        : AppOperationHandlerBase
            <
                WebQueryManagerAppService,
                WebQueryManagerAppService.SaveChangesAppOperation,
                SaveChangesAppOperationOptions,
                QueryChangesResult
            >
    {
        public SaveChangesAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {
        }

        public override QueryChangesResult Handle(SaveChangesAppOperationOptions options, IAppOperationContext operationContext)
        {
            QueryChangesResult result = new QueryChangesResult();
            try { 
            Class_ManageSettingInstance v_s = new Class_ManageSettingInstance();
            List<QueryChangesetActionResult> L_chRes = new List<QueryChangesetActionResult>();
            QueryChangesetActionResult chRes = new QueryChangesetActionResult();
            result.QueryRef = new QueryReference();
            result.QueryRef.Id = options.Changeset.QueryRef.Id;
            result.QueryRef.Version = options.Changeset.QueryRef.Version;
            int user_id = options.OtherArgs.UserId;
            Dictionary<string, object> Params = options.OtherArgs.Values;
            int max_Val = ConnectDB.NullI;  int ID_Record = ConnectDB.NullI;
            foreach (KeyValuePair<string, object> p in Params) { if (p.Key == "ID") { ID_Record = Convert.ToInt32(p.Value); break; } }
            StockItems PS = new StockItems();
            QueryMetaD MD = new QueryMetaD();
            List<SettingIRPClass> Las_NameCat = (List<SettingIRPClass>)PS.GetAvailableStocksSettingIRP(false);
            List<QueryMetaD> LQD = (List<QueryMetaD>)PS.GetCacheKeyMetaData(false, options.OtherArgs.UserId);
            if (LQD != null) MD = LQD.Find(t => t.settIRP.ID == options.Changeset.QueryRef.Id);
            SettingIRPClass settIRP = Las_NameCat.Find(z => z.ID == options.Changeset.QueryRef.Id);
            if (settIRP.IS_SQL_REQUEST==false) { 
            List<BlockDataFind> obj_base = new List<BlockDataFind>();
            foreach (QueryChangesetAction act in options.Changeset.Actions) {
                int D_I = ConnectDB.NullI;
                if ((act.Type == ChangesetActionType.Update) && (ID_Record != ConnectDB.NullI)) {
                    obj_base = v_s.GetFieldFromFormFinder(MD, settIRP, true, Params);
                    D_I = v_s.SaveToOrmDataEdit(MD, settIRP, obj_base, null, (int)ID_Record, user_id, out max_Val);
                    if (D_I != ConnectDB.NullI) { chRes.Success = true; chRes.Message = "Succesfully updated record"; } else { chRes.Success = false; chRes.Message = "Error updated record"; }
                    chRes.Type = ChangesetActionType.Update;
                    chRes.RecordRef = new RecordReference();
                    chRes.RecordRef.Id = D_I;
                    L_chRes.Add(chRes);
                }
                else if (act.Type == ChangesetActionType.Create) {
                    obj_base = v_s.GetFieldFromFormFinderCreate(MD, settIRP, true, Params);
                    D_I = v_s.SaveToOrmDataEdit(MD, settIRP, obj_base, null, ConnectDB.NullI, user_id, out max_Val);
                    if (D_I != ConnectDB.NullI) { chRes.Success = true; chRes.Message = "Succesfully created new record"; } else { chRes.Success = false; chRes.Message = "Error created new record"; }
                    chRes.Type = ChangesetActionType.Create;
                    chRes.RecordRef = new RecordReference();
                    chRes.RecordRef.Id = D_I;
                    L_chRes.Add(chRes);
                }
                else if ((act.Type == ChangesetActionType.Delete) && (ID_Record != ConnectDB.NullI)) {
                    if (ConnectDB.SetStatusArchive(ID_Record, MD.TableName))  {
                        chRes.Success = true; chRes.Message = string.Format("Succesfully update status = 'Z' for ID = {0}, table_name={1}",ID_Record, MD.TableName);
                        chRes.Type = ChangesetActionType.Create;
                        chRes.RecordRef = new RecordReference();
                        chRes.RecordRef.Id = ID_Record;
                        L_chRes.Add(chRes);
                    }
                    else {
                        chRes.Success = false; chRes.Message = string.Format("Error updated status = 'Z' for ID = {0}, table_name={1}", ID_Record, MD.TableName);
                        chRes.Type = ChangesetActionType.Delete;
                        chRes.RecordRef = new RecordReference();
                        chRes.RecordRef.Id = ID_Record;
                        L_chRes.Add(chRes);
                    }
                }
            }
            }
            else
            {
                chRes.Success = false;
                chRes.Message = "For mode SQL-only this function not supported!";
                L_chRes.Add(chRes);
            }
            result.Actions = L_chRes.ToArray();
            Logger.Trace(this, options, operationContext);
            }
            catch (Exception ex) { Logger.Error(ex); }
            return result;
        }
    }
}
