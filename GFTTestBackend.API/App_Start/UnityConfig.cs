using GFTTestBackend.API.Services;
using GFTTestBackend.API.Utils;
using System.Configuration;
using System.Web.Http;
using Unity;
using Unity.Injection;
using Unity.WebApi;

namespace GFTTestBackend.API
{
	public static class UnityConfig
	{
		public static void RegisterComponents()
		{
			var container = new UnityContainer();

			string key = ConfigurationManager.AppSettings["EncryptionKey"];
			string gftDbUser = ConfigurationManager.AppSettings["GFTDBUser"];
			string gftDbPassword = ConfigurationManager.AppSettings["GFTDBPassword"];
			string connectionString = string.Format(ConfigurationManager.ConnectionStrings["GFTEntities"].ToString(), Encryption.Decrypt(gftDbUser, key), Encryption.Decrypt(gftDbPassword, key));

			container.RegisterType<ClientService>(new InjectionConstructor(connectionString));

			GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
		}
	}
}