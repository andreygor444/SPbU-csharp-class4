namespace Task2
{
    public class Task2
    {
       
        public static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Ошибка: неправильное количество аргументов");
                return;
            }
            string filename = args[0], encFrom = args[1], encTo = args[2];

            try
            {
                StreamReader reader = new StreamReader(filename, System.Text.Encoding.GetEncoding(encFrom));
                string? text = reader.ReadToEnd();
                var stream = new FileStream(filename, FileMode.Append);
                StreamWriter writer = new StreamWriter(stream, System.Text.Encoding.GetEncoding(encTo));
                writer.Write(text);
                Console.WriteLine("Успешно");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Ошибка: отсутствует кодировка");
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Ошибка: отсутствует директория");
            }
        }
    }
}
