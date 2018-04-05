using System;

namespace GFTTestBackend.API.Models
{
	public class ClientDTO
	{
		public int ClientID { get; set; }

		public string Name { get; set; }

		public string Email { get; set; }

		public DateTime? DateOfBirth { get; set; }

		public ClientTypeDTO ClientType { get; set; }
	}
}