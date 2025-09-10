using Serilog;

namespace Changloger.Utils
{
    public static class StringExtensions
    {
        public static string FillingWidthWithSymbol(this string separatorChair)
        {
            if (string.IsNullOrEmpty(separatorChair))
            {
                Log.Error("Значение должно быть одиночным символом.", nameof(separatorChair));
                throw new ArgumentException("Значение должно быть одиночным символом.", nameof(separatorChair));
            }

            int width = Console.WindowWidth;
            return new string(separatorChair[0], width - 1);
        }

        public static string FormatingStrDate(this string strDate)
        {
            if (string.IsNullOrEmpty(strDate))
            {
                Log.Error("Значение не должно быть пустым.", nameof(strDate));
                throw new ArgumentException("Значение не должно быть пустым.", nameof(strDate));
            }
            
            if (DateTime.TryParse(strDate, out var date) && date != DateTime.MinValue)
            {
                return date.ToString("yyyy-MM-dd HH:mm:ss");
            }

            return strDate;
        }
    }
}
