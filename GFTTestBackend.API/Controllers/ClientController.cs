using GFTTestBackend.API.Models;
using GFTTestBackend.API.Services;
using System.Collections.Generic;
using System.Web.Http;

namespace GFTTestBackend.API.Controllers
{
	[RoutePrefix("api/Client")]
	public class ClientController : ApiController
	{
		ClientService clientSvc;

		public ClientController(ClientService clientSvc)
		{
			this.clientSvc = clientSvc;
		}

		[HttpGet]
		[Route("SearchClient/{name}/{ageRange}")]
		public IHttpActionResult SearchClient(string name, int ageRange)
		{
			List<ClientDTO> clients = clientSvc.SearchClient(name, (AgeRangeEnum)ageRange);
			return Ok(clients);
		}

		[HttpPost]
		[Route("AddClient")]
		public IHttpActionResult AddClient([FromBody] ClientDTO client)
		{
			ClientDTO insertedClient = clientSvc.AddClient(client);
			return Ok(insertedClient);
		}

		[HttpPut]
		[Route("UpdateClient")]
		public IHttpActionResult UpdateClient([FromBody] ClientDTO client)
		{
			clientSvc.UpdateClient(client);
			return Ok();
		}

		[HttpDelete]
		[Route("DeleteClient/{clientID}")]
		public IHttpActionResult DeleteClient(int clientID)
		{
			clientSvc.DeleteClient(clientID);
			return Ok();
		}


	}
}
