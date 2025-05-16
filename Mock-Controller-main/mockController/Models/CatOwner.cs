using System;
using Microsoft.AspNetCore.Identity;

namespace mock.depart.Models
{
	public class CatOwner: IdentityUser
	{
		public CatOwner()
		{
		}

		public virtual List<Cat> Cats { get; set; } = new List<Cat>();
	}
}

