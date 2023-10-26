using System;
namespace FDex.Domain.Entities
{
	public class UserLevelAnalytic
	{
        public Guid Id { get; set; }
		public int Level0 { get; set; }
        public int Level1 { get; set; }
        public int Level2 { get; set; }
        public int Level3 { get; set; }
    }
}
