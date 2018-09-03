﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns;
using Atdi.DataModels.Sdrns.Device;
using System.IO;
using Atdi.AppServer;
using System.Data.Common;
using Oracle.DataAccess.Client;

namespace Atdi.AppServer.AppService.SdrnsControllerv2_0
{
    public class ClassDBEntity
    {
        public static ILogger logger;
        public ClassDBEntity(ILogger log)
        {
            if (logger == null) logger = log;
        }


        public void SaveEntity(Atdi.DataModels.Sdrns.Device.Entity entity)
        {
            Yyy yyy = new Yyy();
            DbConnection dbConnect = yyy.NewConnection(yyy.GetConnectionString());
            if (dbConnect.State == System.Data.ConnectionState.Open)
            {
                DbTransaction transaction = dbConnect.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                try
                {
                    YxbsEntity yxbsEntity = new YxbsEntity();
                    yxbsEntity.Format("*");
                    yxbsEntity.Filter = "ID=-1";
                    yxbsEntity.OpenRs();
                    yxbsEntity.New();
                    yxbsEntity.m_contenttype = entity.ContentType;
                    yxbsEntity.m_description = entity.Description;
                    yxbsEntity.m_encoding = entity.Encoding;
                    yxbsEntity.m_hashalgoritm = entity.HashAlgorithm;
                    yxbsEntity.m_hashcode = entity.HashCode;
                    yxbsEntity.m_name = entity.Name;
                    yxbsEntity.m_parentid = entity.ParentId;
                    yxbsEntity.m_parenttype = entity.ParentType;
                    yxbsEntity.m_partindex = entity.PartIndex;
                    yxbsEntity.m_entityid = entity.EntityId;
                    yxbsEntity.m_eof = entity.EOF == true ? 1 : 0;
                    yxbsEntity.m_content = entity.Content;
                    yxbsEntity.Save(dbConnect, transaction);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    transaction.Rollback();
                }
                finally
                {
                    transaction.Dispose();
                    dbConnect.Close();
                    dbConnect.Dispose();
                }
            }

        }
        public void SaveEntityPart(Atdi.DataModels.Sdrns.Device.EntityPart entityPart)
        {
            Yyy yyy = new Yyy();
            DbConnection dbConnect = yyy.NewConnection(yyy.GetConnectionString());
            if (dbConnect.State == System.Data.ConnectionState.Open)
            {
                DbTransaction transaction = dbConnect.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                try
                {
                    YxbsEntitypart yxbsEntitypart = new YxbsEntitypart();
                    yxbsEntitypart.Format("*");
                    yxbsEntitypart.Filter = "ID=-1";
                    yxbsEntitypart.OpenRs();
                    yxbsEntitypart.New();
                    yxbsEntitypart.m_entityid = entityPart.EntityId;
                    yxbsEntitypart.m_eof = entityPart.EOF == true ? 1 : 0;
                    yxbsEntitypart.m_partindex = entityPart.PartIndex;
                    yxbsEntitypart.m_content = entityPart.Content;
                    yxbsEntitypart.Save(dbConnect, transaction);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    transaction.Rollback();
                }
                finally
                {
                    transaction.Dispose();
                    dbConnect.Close();
                    dbConnect.Dispose();
                }
            }

        }

    }
}
