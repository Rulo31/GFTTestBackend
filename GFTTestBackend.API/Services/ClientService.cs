using GFTTestBackend.DAL;
using GFTTestBackend.API.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;

namespace GFTTestBackend.API.Services
{
	public class ClientService
	{
		private string connectionString;

		public ClientService(string connectionString)
		{
			this.connectionString = connectionString;
		}

		public List<ClientDTO> SearchClient(string name, AgeRangeEnum ageRange)
		{
			bool lockWasTaken = false;
			List<ClientDTO> clients = new List<ClientDTO>();

			using (GFTEntities dbContext = new GFTEntities(connectionString))
			{
				try
				{
					Monitor.Enter(dbContext, ref lockWasTaken);

					if (ageRange == AgeRangeEnum.LessThan20)
						clients = dbContext.Clients.Where(x => x.Name.StartsWith(name) && DbFunctions.DiffYears(x.Date_Of_Birth, DateTime.Now) < 20).Select(x => new ClientDTO() { ClientID = x.Client_Id, Name = x.Name, Email = x.Email, DateOfBirth = x.Date_Of_Birth, ClientType = new ClientTypeDTO() { ClientTypeID = x.Client_Type.Client_Type_Id, Type = x.Client_Type.Type } }).ToList();
					else if (ageRange == AgeRangeEnum.From21To40)
						clients = dbContext.Clients.Where(x => x.Name.StartsWith(name) && DbFunctions.DiffYears(x.Date_Of_Birth, DateTime.Now) > 20 && DbFunctions.DiffYears(x.Date_Of_Birth, DateTime.Now) < 40).Select(x => new ClientDTO() { ClientID = x.Client_Id, Name = x.Name, Email = x.Email, DateOfBirth = x.Date_Of_Birth, ClientType = new ClientTypeDTO() { ClientTypeID = x.Client_Type.Client_Type_Id, Type = x.Client_Type.Type } }).ToList();
					else if (ageRange == AgeRangeEnum.MoreThan40)
						clients = dbContext.Clients.Where(x => x.Name.StartsWith(name) && DbFunctions.DiffYears(x.Date_Of_Birth, DateTime.Now) > 40).Select(x => new ClientDTO() { ClientID = x.Client_Id, Name = x.Name, Email = x.Email, DateOfBirth = x.Date_Of_Birth, ClientType = new ClientTypeDTO() { ClientTypeID = x.Client_Type.Client_Type_Id, Type = x.Client_Type.Type } }).ToList();
					else if (ageRange == AgeRangeEnum.All)
						clients = dbContext.Clients.Where(x => x.Name.StartsWith(name)).Select(x => new ClientDTO() { ClientID = x.Client_Id, Name = x.Name, Email = x.Email, DateOfBirth = x.Date_Of_Birth, ClientType = new ClientTypeDTO() { ClientTypeID = x.Client_Type.Client_Type_Id, Type = x.Client_Type.Type } }).ToList();
					else
						clients = dbContext.Clients.Where(x => x.Name.StartsWith(name)).Select(x => new ClientDTO() { ClientID = x.Client_Id, Name = x.Name, Email = x.Email, DateOfBirth = x.Date_Of_Birth, ClientType = new ClientTypeDTO() { ClientTypeID = x.Client_Type.Client_Type_Id, Type = x.Client_Type.Type } }).ToList();

					if (lockWasTaken)
					{
						Monitor.Exit(dbContext);
						lockWasTaken = false;
					}
				}
				catch (Exception)
				{
					if (lockWasTaken)
					{
						Monitor.Exit(dbContext);
						lockWasTaken = false;
					}

					throw;
				}

				return clients;
			}
		}

		public ClientDTO AddClient(ClientDTO client)
		{
			bool lockWasTaken = false;
			Client toBeInserted = new Client();

			using (GFTEntities dbContext = new GFTEntities(connectionString))
			{
				try
				{
					Monitor.Enter(dbContext, ref lockWasTaken);

					toBeInserted = FromClientDTOToClient(client);

					dbContext.Clients.Add(toBeInserted);
					dbContext.SaveChanges();

					client.ClientID = toBeInserted.Client_Id;

					if (lockWasTaken)
					{
						Monitor.Exit(dbContext);
						lockWasTaken = false;
					}
				}
				catch (Exception)
				{
					if (lockWasTaken)
					{
						Monitor.Exit(dbContext);
						lockWasTaken = false;
					}

					throw;
				}

				return client;
			}
		}

		public void UpdateClient(ClientDTO client)
		{
			bool lockWasTaken = false;
			Client clientDB = new Client();

			using (GFTEntities dbContext = new GFTEntities(connectionString))
			{
				try
				{
					Monitor.Enter(dbContext, ref lockWasTaken);

					clientDB = dbContext.Clients.Where(x => x.Client_Id == client.ClientID).FirstOrDefault();
					clientDB.Name = client.Name;
					clientDB.Email = client.Email;
					clientDB.Date_Of_Birth = client.DateOfBirth;
					clientDB.Client_Type_Id = client.ClientType.ClientTypeID;

					dbContext.SaveChanges();

					if (lockWasTaken)
					{
						Monitor.Exit(dbContext);
						lockWasTaken = false;
					}
				}
				catch (Exception)
				{
					if (lockWasTaken)
					{
						Monitor.Exit(dbContext);
						lockWasTaken = false;
					}

					throw;
				}
			}
		}


		public void DeleteClient(int clientID)
		{
			bool lockWasTaken = false;

			using (GFTEntities dbContext = new GFTEntities(connectionString))
			{
				try
				{
					Monitor.Enter(dbContext, ref lockWasTaken);

					Client client = new Client() { Client_Id = clientID };
					dbContext.Clients.Attach(client);
					dbContext.Clients.Remove(client);
					dbContext.SaveChanges();

					if (lockWasTaken)
					{
						Monitor.Exit(dbContext);
						lockWasTaken = false;
					}
				}
				catch (Exception)
				{
					if (lockWasTaken)
					{
						Monitor.Exit(dbContext);
						lockWasTaken = false;
					}

					throw;
				}
			}
		}

		private Client FromClientDTOToClient(ClientDTO client)
		{
			return new Client() { Client_Id = client.ClientID, Name = client.Name, Email = client.Email, Date_Of_Birth = client.DateOfBirth, Client_Type_Id = client.ClientType.ClientTypeID };
		}

		private ClientDTO FromClientToClientDTO(Client client)
		{
			return new ClientDTO() { ClientID = client.Client_Id, Name = client.Name, Email = client.Email, DateOfBirth = client.Date_Of_Birth, ClientType = client.Client_Type != null ? FromClientTypeToClientTypeDTO(client.Client_Type) : new ClientTypeDTO() { ClientTypeID = client.Client_Type_Id } };
		}

		private Client_Type FromClientTypeDTOToClientType(ClientTypeDTO clientType)
		{
			return new Client_Type() { Client_Type_Id = clientType.ClientTypeID, Type = clientType.Type };
		}

		private ClientTypeDTO FromClientTypeToClientTypeDTO(Client_Type clientType)
		{
			return new ClientTypeDTO() { ClientTypeID = clientType.Client_Type_Id, Type = clientType.Type };
		}
	}
}