using System.Text;

namespace Utils.Utils
{
    public static class StringExtensions
    {
        public static string HideSensivityText(this string strData, string[] args)
        {
            StringBuilder baseText = new StringBuilder();
            StringBuilder clearText = new StringBuilder();
            int index = 0;

            baseText.Append(strData);

            while (index < args.Length)
            {
                string text = clearText.Length <= 0 ? baseText.ToString() : clearText.ToString();
                clearText.Clear();
                var fi =  text.IndexOf(args[index]);
                if (fi >= 0)
                {
                    int li = fi + args[index].Length - 1;
                    for (int p = 0; p < text.Length; p++)
                    {
                        if(p >= fi && p <= li)
                            clearText.Append('*');
                        else
                            clearText.Append(text[p]);
                    }
                    index++;
                }
                
            }
            return clearText.ToString();
            
        }
    }
}
