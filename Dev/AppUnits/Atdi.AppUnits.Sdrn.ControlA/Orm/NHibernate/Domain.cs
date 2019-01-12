using System.Runtime.Remoting.Messaging;
using System.Web;
using NHibernate;
using NHibernate.Cfg;

namespace Atdi.AppUnits.Sdrn.ControlA
{
	public static class Domain
	{
		private const string sessionKey = "NHib.SessionKey";
		public static ISessionFactory sessionFactory;

		public static ISession CurrentSession
		{
			get
			{
				return GetSession(true);
			}
		}

		static Domain()
		{
		}

		public static void Init()
		{
			sessionFactory = new Configuration().Configure(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Nhibernate.cfg.xml").BuildSessionFactory();
		}

		public static void Close()
		{
			ISession currentSession = GetSession(false);

			if (currentSession != null)
			{
				currentSession.Close();
			}
		}

        private static ISession GetSession(bool getNewIfNotExists)
        {
            ISession currentSession;
            currentSession = CallContext.GetData(sessionKey) as ISession;
            if (currentSession == null && getNewIfNotExists)
            {
                currentSession = sessionFactory.OpenSession();
                CallContext.SetData(sessionKey, currentSession);
            }
            return currentSession;
        }
	}
}