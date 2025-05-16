using System;
using mock.depart.Data;
using mock.depart.Models;

namespace mock.depart.Services
{
	public class CatsService:BaseService<Cat>
	{
		public CatsService()
		{
		}

		public CatsService(ApplicationDbContext context) : base(context)
		{
		}
    }
}

