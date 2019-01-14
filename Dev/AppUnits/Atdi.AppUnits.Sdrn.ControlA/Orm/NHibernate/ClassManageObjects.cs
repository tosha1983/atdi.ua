using System;
using NHibernate;


namespace Atdi.AppUnits.Sdrn.ControlA
{
    public class ClassObjectsSensorOnSDR
    {

        /// <summary>
        /// Delete record from name table
        /// </summary>
        public void DeleteObject(string NameTable, int ID)
        {
            if (Domain.sessionFactory == null) Domain.Init();
            ISession session = Domain.CurrentSession;
            {
                ITransaction tr_1 = session.BeginTransaction();
                try
                {
                    session.Delete(string.Format("from {0} ", NameTable) + " p where p.ID = ?", ID, NHibernateUtil.Int32);
                    tr_1.Commit();
                }
                catch (Exception) { tr_1.Rollback(); }
                tr_1.Dispose();
                session.Flush();
            }
        }


        /// <summary>
        /// Delete record from table NH_YXbSensor
        /// </summary>
        public void UpdateObject<T>(int ID, T value_new)
        {
            try
            {
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    T persistentEmployee = session.Get<T>(ID);
                    persistentEmployee = value_new;
                    session.SaveOrUpdate(persistentEmployee);
                    session.Flush();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

      
    }
   
}
