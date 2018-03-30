using System;

namespace Debug_Server
{
    public enum Languages
    {
        CSharp,
        C,
        Cpp,
        Python,
        Java
    }
    public class Debug
    {
        public string _id { get; set; }
        public string Code { get; set; }
        public Languages Language { get; set; }
        public string SuccessResult { get; set; }
        public string ErrorResult { get; set; }
        public DateTime CreationDate { get; set; }
        public Debug()
        {
            _id = Guid.NewGuid().ToString();
            CreationDate = DateTime.Now;
        }
    }
}
