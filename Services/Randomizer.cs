namespace ProjektODASAPI.Services
{
    public class Randomizer
    {
        public Random a = new Random();
        public List<int> randomList = new List<int>();
        int MyNumber = 0;
        public void NewNumber()
        {
            MyNumber = a.Next(0, 10);
            if (!randomList.Contains(MyNumber))
                randomList.Add(MyNumber);
        }
    }
}
