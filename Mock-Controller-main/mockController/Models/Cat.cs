namespace mock.depart.Models
{

    public enum Cuteness
    {
        BarelyOk = 0,
        YouCanKeepIt,
        Amazing
    }

    public class Cat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Cuteness CuteLevel { get; set; }
        public virtual CatOwner? CatOwner { get; set; }
    }

}

